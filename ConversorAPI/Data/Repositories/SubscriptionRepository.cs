﻿using Data.Entities;
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

        public Subscription GetSubscriptionByName(string name)
        {
            return _context.Subscriptions.FirstOrDefault(s => s.SubscriptionName == name);
        }

        
        public User GetUserBySubscriptionId(int subscriptionId)
        {
            return _context.Users.Include(u => u.Subscription)
                                 .FirstOrDefault(u => u.SubscriptionId == subscriptionId);

        }

     
        public User AssignSubscriptionToUser(int userId, int subscriptionId)
        {
            var user = _context.Users.Include(u => u.Subscription).FirstOrDefault(u => u.UserId == userId);
            if (user == null)
                return null;

            user.SubscriptionId = subscriptionId;
            user.conversionEnabled = true;
            _context.SaveChanges();

            return user;
        }

        public int GetRemainingConversions(int userId)
        {
            var user = _context.Users.Include(u => u.Subscription).FirstOrDefault(u => u.UserId == userId);
            if (user == null || user.Subscription == null)
                return 0;

            return user.Subscription.ConversionLimit - user.ConversionsMaked;
        }

       
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
