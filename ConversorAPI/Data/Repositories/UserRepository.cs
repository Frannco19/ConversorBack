using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UserRepository 
    {
        private readonly ConversorContext _context;

        public UserRepository(ConversorContext context)
        {
            _context = context;
        }

        // Agregar un nuevo usuario, validando duplicados
        public int AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.Username))
                throw new ArgumentException("El nombre de usuario no puede estar vacío.");

            if (_context.Users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"El usuario con nombre '{user.Username}' ya existe.");

            _context.Users.Add(user);
            _context.SaveChanges();
            return user.UserId;
        }

        // Actualizar un usuario, validando que exista
        public User Update(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var existingUser = _context.Users.Find(user.UserId);
            if (existingUser == null)
                throw new KeyNotFoundException($"El usuario con ID {user.UserId} no existe.");

            // Actualizar los campos relevantes según los nombres en la entidad
            existingUser.Username = user.Username ?? existingUser.Username;
            existingUser.Password = user.Password ?? existingUser.Password;
            existingUser.Email = user.Email ?? existingUser.Email;
            existingUser.SubscriptionId = user.SubscriptionId != 0 ? user.SubscriptionId : existingUser.SubscriptionId;

            // Actualizar el rol solo si es proporcionado (opcional)
            if (user.Role != 0)
                existingUser.Role = user.Role;

            _context.SaveChanges();
            return existingUser;
        }

        // Obtener todos los usuarios
        public List<User> GetUsers()
        {
            return _context.Users.Include(u => u.Subscription).ToList();
        }

        // Obtener usuario por ID, validando que exista
        public User GetById(int id)
        {
            var user = _context.Users.Include(u => u.Subscription).FirstOrDefault(u => u.UserId == id);
            if (user == null)
                throw new KeyNotFoundException($"El usuario con ID {id} no existe.");

            return user;
        }

        // Obtener usuario por nombre de usuario, validando que exista
        public User GetUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(username));

            var user = _context.Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
                throw new KeyNotFoundException($"El usuario con nombre '{username}' no existe.");

            return user;
        }

        // Eliminar usuario por ID, validando que exista
        public bool DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                throw new KeyNotFoundException($"El usuario con ID {id} no existe.");

            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }

        // Verificar si un usuario ya existe por nombre de usuario
        public bool DoesUsernameExist(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(username));

            return _context.Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}
