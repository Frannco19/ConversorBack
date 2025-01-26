using Data.Models.ENUMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.DTOs.UserDTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public int MaxConversionsAllowed { get; set; }
        public int ConversionsMaked { get; set; }
    }
}
