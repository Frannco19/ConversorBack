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

        // Obtener todas las monedas
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

        // Obtener una moneda por ID
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

        // Crear una nueva moneda
        public int AddCurrency(CurrencyCreateUpdateDTO currencyDto)
        {
            var currency = new Currency
            {
                CurrencyCode = currencyDto.CurrencyCode,
                CurrencyLegend = currencyDto.CurrencyLegend,
                CurrencySymbol = currencyDto.CurrencySymbol,
                ConversionRate = currencyDto.ConvertibilityCurrency,
                UserId = 0 // Puede ser un valor predeterminado si no es relevante
            };

            return _currencyRepository.AddCurrency(currency);
        }

        // Actualizar una moneda existente
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


        // Eliminar directamente una moneda de la base de datos
        public bool DeleteCurrency(int id)
        {
            // Intentar obtener la moneda desde el repositorio
            var existingCurrency = _currencyRepository.GetCurrencyById(id);

            if (existingCurrency == null)
            {
                // Retornar false si no existe
                return false;
            }

            // Eliminar la moneda directamente
            _currencyRepository.RemoveCurrency(existingCurrency);
            return true;
        }


        // Convertir una moneda a otra
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
