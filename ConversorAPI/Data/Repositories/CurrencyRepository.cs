using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class CurrencyRepository 
    {
        private readonly ConversorContext _context;

        public CurrencyRepository(ConversorContext context)
        {
            _context = context;
        }

        // Obtener todas las monedas sin basarse en un estado.
        public IEnumerable<Currency> GetGlobalCurrencies()
        {
            return _context.CurrenciesConvert.ToList();
        }

        // Obtener moneda por ID, validando existencia.
        public Currency GetCurrencyById(int id)
        {
            var currency = _context.CurrenciesConvert.Find(id);
            if (currency == null)
                throw new KeyNotFoundException($"La moneda con ID {id} no existe.");

            return currency;
        }

        // Método para agregar una nueva moneda.
        public int AddCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            if (_context.CurrenciesConvert.Any(c => c.CurrencyCode.Equals(currency.CurrencyCode, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Ya existe una moneda con el código {currency.CurrencyCode}.");

            _context.CurrenciesConvert.Add(currency);
            _context.SaveChanges();
            return currency.CurrencyId;
        }

        // Actualizar moneda, validando existencia.
        public Currency UpdateCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            var existingCurrency = _context.CurrenciesConvert.Find(currency.CurrencyId);
            if (existingCurrency == null)
                throw new KeyNotFoundException($"La moneda con ID {currency.CurrencyId} no existe.");

            // Actualizar los campos relevantes
            existingCurrency.CurrencyCode = currency.CurrencyCode;
            existingCurrency.CurrencyLegend = currency.CurrencyLegend;
            existingCurrency.CurrencySymbol = currency.CurrencySymbol;
            existingCurrency.ConversionRate = currency.ConversionRate;

            _context.SaveChanges();
            return existingCurrency;
        }

        // Eliminar moneda por ID, validando existencia.
        public bool DeleteCurrency(int id)
        {
            var currency = _context.CurrenciesConvert.Find(id);
            if (currency == null)
                throw new KeyNotFoundException($"La moneda con ID {id} no existe.");

            _context.CurrenciesConvert.Remove(currency);
            _context.SaveChanges();
            return true;
        }

        // Obtener moneda por código, validando existencia.
        public Currency GetCurrencyByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("El código de la moneda no puede estar vacío.", nameof(code));

            var currency = _context.CurrenciesConvert
                .FirstOrDefault(c => c.CurrencyCode.Equals(code, StringComparison.OrdinalIgnoreCase));

            if (currency == null)
                throw new KeyNotFoundException($"La moneda con código {code} no existe.");

            return currency;
        }

        public void RemoveCurrency(Currency currency)
        {
            _context.CurrenciesConvert.Remove(currency);
            _context.SaveChanges();
        }

    }
}
