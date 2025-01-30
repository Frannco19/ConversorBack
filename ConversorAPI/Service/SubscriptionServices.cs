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

        // Asignar una suscripción a un usuario
        public UserSubStatusDTO MakeSubscriptionToUser(UserSubscriptionDTO userSubscriptionDto)
        {
            // Asignar la suscripción al usuario
            var updatedUser = _subscriptionRepository.AssignSubscriptionToUser(
                userSubscriptionDto.UserId,
                userSubscriptionDto.SubscriptionId
            );

            if (updatedUser == null)
            {
                // Retornar null si no se encontró el usuario o la suscripción
                return null;
            }

            // Calcular conversiones restantes
            int conversionsUsed = updatedUser.ConversionsMaked;

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

        // Listar todas las suscripciones
        public List<SubscriptionDTO> GetAllSubscriptions()
        {
            // Obtener todas las suscripciones desde el repositorio
            var subscriptions = _subscriptionRepository.GetAllSubscriptions();

            // Mapear a DTOs
            return subscriptions.Select(subscription => new SubscriptionDTO
            {
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionName = subscription.SubscriptionName,
                Subcount = subscription.ConversionLimit
            }).ToList();
        }

        // Obtener una suscripción por ID
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
