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
                SubCount = 0, // Inicializa el contador de conversiones
                ConversionsMaked = 0, // Inicializa las conversiones realizadas
                Role = Role.User // Rol predeterminado
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

            // Verificar si se alcanzó el límite de conversiones
            if (user.ConversionsMaked >= user.Subscription.ConversionLimit && user.Subscription.ConversionLimit != 0)
            {
                user.SubCount = (int)user.Subscription.ConversionLimit; // Establece el límite alcanzado
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
            var existingUser = _userRepository.GetById(user.UserId);
            if (existingUser == null)
                throw new KeyNotFoundException($"El usuario con ID {user.UserId} no existe.");

            // Actualizar solo los campos relevantes
            existingUser.Username = user.Username ?? existingUser.Username;
            existingUser.Email = user.Email ?? existingUser.Email;
            existingUser.Password = user.Password ?? existingUser.Password;

            return _userRepository.Update(existingUser);
        }

        // Verificar si el usuario puede realizar más conversiones
        public bool CanConvert(int userId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new KeyNotFoundException($"El usuario con ID {userId} no existe.");

            var subscription = user.Subscription;

            // Verifica si tiene suscripción válida y puede convertir
            if (subscription != null && subscription.ConversionLimit == 0) // Suscripción Pro (sin límite)
                return true;

            if (subscription != null && user.ConversionsMaked < subscription.ConversionLimit)
                return true;

            return false; // Límite alcanzado o no tiene suscripción válida
        }
    }

}
