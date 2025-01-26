using Data.Models.ENUMs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public int SubCount { get; set; } // Cantidad de conversiones contador

        public int ConversionsMaked { get; set; }

        public int SubscriptionId { get; set; }

        public Role Role { get; set; }

        [ForeignKey("SubscriptionId")]
        public virtual Subscription Subscription { get; set; }
    }
}
