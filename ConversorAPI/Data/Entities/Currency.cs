using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Currency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CurrencyId { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        [Required]
        public string? CurrencyLegend { get; set; }

        [MaxLength(4)]
        public string? CurrencySymbol { get; set; }

        [Required]
        public decimal ConversionRate { get; set; } // valor en relación al dólar

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
