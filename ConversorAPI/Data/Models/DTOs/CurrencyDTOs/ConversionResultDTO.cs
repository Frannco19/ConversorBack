using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.DTOs.CurrencyDTOs
{
    public class ConversionResultDTO
    {
        public string FromCurrencyCode { get; set; }  // Código de la moneda de origen
        public string ToCurrencyCode { get; set; }    // Código de la moneda de destino
        public decimal OriginalAmount { get; set; }   // Cantidad original
        public decimal ConvertedAmount { get; set; }  // Cantidad convertida
    }
}
