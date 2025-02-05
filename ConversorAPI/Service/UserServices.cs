using Data.Entities;
using Data.Models.DTOs.UserDTOs;
using Data.Models.ENUMs;
using Data.Repositories;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserServices : IUserService
    {
        private readonly UserRepository _userRepository;

        public UserServices(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public bool IsAdmin(int userId)
        {

            var user = _userRepository.GetById(userId);
            if (user == null) throw new ArgumentException("Usuario no encontrado.");
            return user.IsAdmin;
        }

        public User CreateAdminUser(AdminUserDTO adminUserDTO)
        {


          
            var adminUser = new User
            {
                Username = adminUserDTO.Username,
                Password = adminUserDTO.Password,
                Email = adminUserDTO.Email,
                conversionEnabled = true,
                ConversionsMaked = 0,
                IsAdmin = true,
                SubscriptionId = 3
            };

            
            _userRepository.AddUser(adminUser);

            return adminUser;
        }

        
        public User CreateUser(UserRegistrationDTO registrationDto)
        {
           
            var user = new User
            {
                Username = registrationDto.Username,
                Password = registrationDto.Password, 
                Email = registrationDto.Email,
                SubscriptionId = 1,
                conversionEnabled = false,
                ConversionsMaked = 0 
            };

            _userRepository.AddUser(user);
            return user;
        }

      
        public User GetById(int userId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new KeyNotFoundException($"El usuario con ID {userId} no existe.");
            return user;
        }

        public User ValidateUser(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);
            if (user != null && user.Password == password) 
                return user;

            return null;
        }


        public User IncrementConversionsUsed(int userId)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
                throw new ArgumentException("Usuario no encontrado.");

      
            user.ConversionsMaked++;

           
            if (user.ConversionsMaked >= user.Subscription.ConversionLimit)
            {
                user.conversionEnabled = false;
            }

          
            _userRepository.Update(user);
            return user;
        }

        
        public int GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;

                if (int.TryParse(userIdClaim, out int userId))
                    return userId;
            }

            return 0; 
        }

       
        public User UpdateUser(User user)
        {
            return _userRepository.Update(user);
        }

        public User UpdateUserAdmin(UpdateAdminDTO user, int adminUserId)
        {
            if (!IsAdmin(adminUserId))
            {
                throw new UnauthorizedAccessException("No tienes permisos para actualizar usuarios.");
            }
            var existingUser = _userRepository.GetById(user.Id);
            if (existingUser == null)
            {
                throw new ArgumentException("Usuario no encontrado.");
            }
            _userRepository.Update(existingUser);
            return existingUser;
        }


        
        public bool CanConvert(int userId)
        {
            var user = _userRepository.GetById(userId);
            if (user.Subscription != null && user.conversionEnabled)
            {
                if (user.ConversionsMaked >= user.Subscription.ConversionLimit)
                {
                    user.conversionEnabled = false; // Desactivar excedió el límite
                    _userRepository.Update(user); 
                    return false;
                }
                return true;
            }
            return false;
        }

        public void DeactivateUser(int userId, int adminUserId)
        {
            if (!IsAdmin(adminUserId))
            {
                throw new UnauthorizedAccessException("No tienes permisos para desactivar usuarios.");
            }

            var user = _userRepository.GetById(userId);
            if (user == null) throw new ArgumentException("Usuario no encontrado.");

            user.conversionEnabled = false;
            _userRepository.Update(user);
        }
    }

}
