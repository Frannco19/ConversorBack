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

        // Obtener suscripción por nombre
        public Subscription GetSubscriptionByName(string name)
        {
            return _context.Subscriptions.FirstOrDefault(s => s.SubscriptionName == name);
        }

        // Obtener un usuario específico por su ID, incluyendo su suscripción
        public User GetUserBySubscriptionId(int subscriptionId)
        {
            return _context.Users.Include(u => u.Subscription)
                                 .FirstOrDefault(u => u.SubscriptionId == subscriptionId);

        }

        // Asignar una suscripción a un usuario
        public User AssignSubscriptionToUser(int userId, int subscriptionId)
        {
            var user = _context.Users.Include(u => u.Subscription).FirstOrDefault(u => u.SubscriptionId == userId);
            if (user == null)
                return null;

            user.SubscriptionId = subscriptionId;
            _context.SaveChanges();

            return user;
        }

        // Obtener las conversiones restantes para un usuario
        public int GetRemainingConversions(int userId)
        {
            var user = _context.Users.Include(u => u.Subscription).FirstOrDefault(u => u.SubscriptionId == userId);
            if (user == null || user.Subscription == null)
                return 0;

            return user.Subscription.ConversionLimit - user.ConversionsMaked;
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
