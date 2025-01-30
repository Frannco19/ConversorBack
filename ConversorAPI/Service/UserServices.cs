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


            // Crear la entidad de usuario administrador
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

            // Guardar en la base de datos
            _userRepository.AddUser(adminUser);

            return adminUser;
        }

        // Crear un nuevo usuario
        public User CreateUser(UserRegistrationDTO registrationDto)
        {
            // Crear el usuario
            var user = new User
            {
                Username = registrationDto.Username,
                Password = registrationDto.Password, // Idealmente, encriptar la contraseña aquí
                Email = registrationDto.Email,
                SubscriptionId = registrationDto.SubscriptionId,
                conversionEnabled = true,
                ConversionsMaked = 0 // Inicializa las conversiones realizadas
            };

            _userRepository.AddUser(user);
            return user;
        }

        // Obtener usuario por ID
        public User GetById(int userId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new KeyNotFoundException($"El usuario con ID {userId} no existe.");
            return user;
        }

        // Validar credenciales de usuario
        public User ValidateUser(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);
            if (user != null && user.Password == password) // Comparación directa; se recomienda encriptar las contraseñas
                return user;

            return null; // Credenciales no válidas
        }

        // Incrementar conversiones realizadas
        public User IncrementConversionsUsed(int userId)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
                throw new ArgumentException("Usuario no encontrado.");

            // Incrementar conversiones realizadas
            user.ConversionsMaked++;

            // Desactivar usuario si excede el límite
            if (user.ConversionsMaked >= user.Subscription.ConversionLimit)
            {
                user.conversionEnabled = false;
            }

            // Actualizar en el repositorio
            _userRepository.Update(user);
            return user;
        }

        // Obtener el ID de usuario desde el token
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

            return 0; // Retorna 0 si no se pudo obtener el ID
        }

        // Actualizar un usuario existente
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


        // Verificar si el usuario puede realizar más conversiones
        public bool CanConvert(int userId)
        {
            var user = _userRepository.GetById(userId);
            if (user.Subscription != null && user.conversionEnabled)
            {
                if (user.ConversionsMaked >= user.Subscription.ConversionLimit)
                {
                    user.conversionEnabled = false; // Desactivar excedió el límite
                    _userRepository.Update(user); // Actualizar el estado en la base de datos
                    return false;
                }
                return true; // Puede realizar conversiones
            }
            return false; // Límite alcanzado o no tiene suscripción válida
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
