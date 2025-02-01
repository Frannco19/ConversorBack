using Data.Models.DTOs.SubscriptionDTOs;
using Data.Repositories;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly SubscriptionRepository _subscriptionRepository;

        public SubscriptionService(SubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

     
        public UserSubStatusDTO MakeSubscriptionToUser(UserSubscriptionDTO userSubscriptionDto)
        {
          
            var updatedUser = _subscriptionRepository.AssignSubscriptionToUser(userSubscriptionDto.UserId,userSubscriptionDto.SubscriptionId);

            if (updatedUser == null)
            {
               
                return null;
            }

            
            int conversionsUsed = _subscriptionRepository.GetRemainingConversions(updatedUser.UserId);


            return new UserSubStatusDTO
            {
                UserId = updatedUser.UserId,
                SubscriptionIsActive = updatedUser.conversionEnabled,
                SubscriptionName = updatedUser.Subscription.SubscriptionName,
                MaxCountConvertions = updatedUser.Subscription.ConversionLimit,
                ConversionMaked = conversionsUsed,
                ConversionsRemaining = updatedUser.Subscription.ConversionLimit == int.MinValue
                                       ? int.MaxValue // Suscripción Pro (sin límite)
                                       : updatedUser.Subscription.ConversionLimit - conversionsUsed
            };
        }

        
        public List<SubscriptionDTO> GetAllSubscriptions()
        {
           
            var subscriptions = _subscriptionRepository.GetAllSubscriptions();

           
            return subscriptions.Select(subscription => new SubscriptionDTO
            {
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionName = subscription.SubscriptionName,
                Subcount = subscription.ConversionLimit
            }).ToList();
        }

    
        public SubscriptionDTO GetSubscriptionById(int subscriptionId)
        {
            var subscription = _subscriptionRepository.GetSubscriptionById(subscriptionId);
            if (subscription == null)
                return null;

            return new SubscriptionDTO
            {
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionName = subscription.SubscriptionName,
                Subcount = (int)subscription.ConversionLimit
            };
        }
    }

}
