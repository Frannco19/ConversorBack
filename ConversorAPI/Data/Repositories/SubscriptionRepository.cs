using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class SubscriptionRepository
    {
        private readonly ConversorContext _context;

        public SubscriptionRepository(ConversorContext context)
        {
            _context = context;
        }

        // Obtener suscripción por nombre, validando que exista
        public Subscription GetSubscriptionByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("El nombre de la suscripción no puede estar vacío.", nameof(name));

            var subscription = _context.Subscriptions
                .FirstOrDefault(s => s.SubscriptionName.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (subscription == null)
                throw new KeyNotFoundException($"La suscripción con nombre '{name}' no existe.");

            return subscription;
        }

        // Obtener un usuario específico por su ID, incluyendo su suscripción
        public User GetUserById(int userId)
        {
            var user = _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefault(u => u.SubscriptionId == userId);

            if (user == null)
                throw new KeyNotFoundException($"El usuario con ID {userId} no existe.");

            return user;
        }

        // Asignar una suscripción a un usuario, validando que ambos existan
        public User AssignSubscriptionToUser(int userId, int subscriptionId)
        {
            var user = _context.Users.Include(u => u.Subscription).FirstOrDefault(u => u.SubscriptionId == userId);
            if (user == null)
                throw new KeyNotFoundException($"El usuario con ID {userId} no existe.");

            var subscription = _context.Subscriptions.Find(subscriptionId);
            if (subscription == null)
                throw new KeyNotFoundException($"La suscripción con ID {subscriptionId} no existe.");

            user.SubscriptionId = subscriptionId;
            _context.SaveChanges();

            return user;
        }

        // Obtener las conversiones restantes para un usuario
        public int GetRemainingConversions(int userId)
        {
            var user = _context.Users.Include(u => u.Subscription).FirstOrDefault(u => u.SubscriptionId == userId);
            if (user == null)
                throw new KeyNotFoundException($"El usuario con ID {userId} no existe.");

            if (user.Subscription == null)
                throw new InvalidOperationException("El usuario no tiene una suscripción asignada.");

            if (user.Subscription.ConversionLimit <= 0) // Suscripción Pro (sin límite)
                return int.MaxValue;

            return (int)(user.Subscription.ConversionLimit - user.ConversionsMaked);
        }

        // Crear una nueva suscripción
        public int AddSubscription(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            if (string.IsNullOrEmpty(subscription.SubscriptionName))
                throw new ArgumentException("El nombre de la suscripción no puede estar vacío.");

            if (_context.Subscriptions.Any(s => s.SubscriptionName.Equals(subscription.SubscriptionName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Ya existe una suscripción con el nombre '{subscription.SubscriptionName}'.");

            _context.Subscriptions.Add(subscription);
            _context.SaveChanges();
            return subscription.SubscriptionId;
        }

        // Actualizar una suscripción existente
        public bool UpdateSubscription(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var existingSubscription = _context.Subscriptions.Find(subscription.SubscriptionId);
            if (existingSubscription == null)
                throw new KeyNotFoundException($"La suscripción con ID {subscription.SubscriptionId} no existe.");

            existingSubscription.SubscriptionName = subscription.SubscriptionName ?? existingSubscription.SubscriptionName;
            existingSubscription.ConversionLimit = subscription.ConversionLimit;

            _context.SaveChanges();
            return true;
        }

        // Eliminar una suscripción por ID
        public bool DeleteSubscription(int subscriptionId)
        {
            var subscription = _context.Subscriptions.Find(subscriptionId);
            if (subscription == null)
                throw new KeyNotFoundException($"La suscripción con ID {subscriptionId} no existe.");

            _context.Subscriptions.Remove(subscription);
            _context.SaveChanges();
            return true;
        }

        // Obtener todas las suscripciones desde la base de datos
        public List<Subscription> GetAllSubscriptions()
        {
            return _context.Subscriptions.ToList();
        }

        public Subscription GetSubscriptionById(int subscriptionId)
        {
            return _context.Subscriptions.FirstOrDefault(s => s.SubscriptionId == subscriptionId);
        }
    }
}
