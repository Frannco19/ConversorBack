using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.DTOs.SubscriptionDTOs
{
    public class UserSubStatusDTO
    {
        public int UserId { get; set; }
        public bool SubscriptionIsActive { get; set; }
        public string SubscriptionName { get; set; }
        public int MaxCountConvertions { get; set; }
        public int ConversionMaked { get; set; }
        public int ConversionsRemaining { get; set; }
    }
}
