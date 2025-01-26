using Data.Models.DTOs.SubscriptionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ISubscriptionService
    {
        UserSubStatusDTO MakeSubscriptionToUser(UserSubscriptionDTO userSubscriptionDto);
        List<SubscriptionDTO> GetAllSubscriptions(); // Firma del método para obtener todas las suscripciones
        SubscriptionDTO GetSubscriptionById(int subscriptionId);
    }
}
