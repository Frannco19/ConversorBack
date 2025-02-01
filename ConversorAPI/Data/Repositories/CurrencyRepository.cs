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

        public IEnumerable<Currency> GetGlobalCurrencies()
        {
            return _context.CurrenciesConvert.ToList();
        }

   
        public Currency GetCurrencyById(int id)
        {
            var currency = _context.CurrenciesConvert.Find(id);
            if (currency == null)
                return null;

            return currency;
        }

        public int AddCurrency(Currency currency)
        {
            _context.CurrenciesConvert.Add(currency);
            _context.SaveChanges();
            return currency.CurrencyId;
        }

        public Currency UpdateCurrency(Currency currency)
        {
            var existingCurrency = _context.CurrenciesConvert.Find(currency.CurrencyId);
            _context.CurrenciesConvert.Update(currency);
            _context.SaveChanges();
            return currency;

        }

      
        public bool DeleteCurrency(int id)
        {
            var currency = _context.CurrenciesConvert.Find(id);
            if (currency == null)
                throw new KeyNotFoundException($"La moneda con ID {id} no existe.");

            _context.CurrenciesConvert.Remove(currency);
            _context.SaveChanges();
            return true;
        }

        public Currency GetCurrencyByCode(string code)
        {

            return _context.CurrenciesConvert
                    .AsEnumerable()
                    .FirstOrDefault(c => c.CurrencyCode.Equals(code, StringComparison.OrdinalIgnoreCase));

        }

        public void RemoveCurrency(Currency currency)
        {
            _context.CurrenciesConvert.Remove(currency);
            _context.SaveChanges();
        }

    }
}
