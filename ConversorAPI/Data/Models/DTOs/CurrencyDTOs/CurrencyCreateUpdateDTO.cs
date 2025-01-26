using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.DTOs.CurrencyDTOs
{
    public class CurrencyCreateUpdateDTO
    {
        public string CurrencyCode { get; set; }
        public string CurrencyLegend { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal ConvertibilityCurrency { get; set; }
    }
}
