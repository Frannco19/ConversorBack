using Data.Models.DTOs.CurrencyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ICurrencyServices
    {
        List<CurrencyDTO> GetAllCurrencies();
        CurrencyDTO GetCurrencyById(int id);
        int AddCurrency(CurrencyCreateUpdateDTO currencyDto);

        public bool UpdateCurrency(int id, CurrencyCreateUpdateDTO currencyDto);
        public bool DeleteCurrency(int id);
        ConversionResultDTO ConvertCurrency(ConversionRequestDTO request);
    }
}
