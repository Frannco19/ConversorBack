using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.DTOs.SubscriptionDTOs
{
    public class SubscriptionDTO
    {
        public int SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }          // Nombre de la suscripción (Free, Trial, Pro)
        public int Subcount { get; set; }    // Límite de conversiones permitido para esta suscripción
    }
}
