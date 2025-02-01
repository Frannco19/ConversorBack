using Data.Entities;
using Data.Models.DTOs.CurrencyDTOs;
using Data.Repositories;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CurrencyService : ICurrencyServices
    {
        private readonly CurrencyRepository _currencyRepository;

        public CurrencyService(CurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

     
        public List<CurrencyDTO> GetAllCurrencies()
        {
            var currencies = _currencyRepository.GetGlobalCurrencies()
                .Select(c => new CurrencyDTO
                {
                    CurrencyId = c.CurrencyId,
                    CurrencyCode = c.CurrencyCode,
                    CurrencyLegend = c.CurrencyLegend,
                    CurrencySymbol = c.CurrencySymbol,
                    ConvertibilityCurrency = c.ConversionRate
                }).ToList();

            return currencies;
        }

   
        public CurrencyDTO GetCurrencyById(int id)
        {
            var currency = _currencyRepository.GetCurrencyById(id);

            if (currency == null)
                return null;

            return new CurrencyDTO
            {
                CurrencyId = currency.CurrencyId,
                CurrencyCode = currency.CurrencyCode,
                CurrencyLegend = currency.CurrencyLegend,
                CurrencySymbol = currency.CurrencySymbol,
                ConvertibilityCurrency = currency.ConversionRate
            };
        }

  
        public int AddCurrency(CurrencyCreateUpdateDTO currencyDto)
        {
            var currency = new Currency
            {
                CurrencyCode = currencyDto.CurrencyCode,
                CurrencyLegend = currencyDto.CurrencyLegend,
                CurrencySymbol = currencyDto.CurrencySymbol,
                ConversionRate = currencyDto.ConvertibilityCurrency
            };

            return _currencyRepository.AddCurrency(currency);
        }

     
        public bool UpdateCurrency(int id, CurrencyCreateUpdateDTO currencyDto)
        {
            var existingCurrency = _currencyRepository.GetCurrencyById(id);
            if (existingCurrency == null)
                return false;

            existingCurrency.CurrencyCode = currencyDto.CurrencyCode;
            existingCurrency.CurrencyLegend = currencyDto.CurrencyLegend;
            existingCurrency.CurrencySymbol = currencyDto.CurrencySymbol;
            existingCurrency.ConversionRate = currencyDto.ConvertibilityCurrency;

            _currencyRepository.UpdateCurrency(existingCurrency);
            return true;
        }


        
        public bool DeleteCurrency(int id)
        {
            
            var existingCurrency = _currencyRepository.GetCurrencyById(id);

            if (existingCurrency == null)
            {
               
                return false;
            }

            _currencyRepository.RemoveCurrency(existingCurrency);
            return true;
        }


      
        public ConversionResultDTO ConvertCurrency(ConversionRequestDTO request)
        {
            var fromCurrency = _currencyRepository.GetCurrencyByCode(request.FromCurrencyCode);
            var toCurrency = _currencyRepository.GetCurrencyByCode(request.ToCurrencyCode);

            if (fromCurrency == null || toCurrency == null)
                return null;

            decimal convertedAmount = request.Amount * (fromCurrency.ConversionRate / toCurrency.ConversionRate);

            return new ConversionResultDTO
            {
                FromCurrencyCode = request.FromCurrencyCode,
                ToCurrencyCode = request.ToCurrencyCode,
                OriginalAmount = request.Amount,
                ConvertedAmount = convertedAmount
            };
        }
    }


}
