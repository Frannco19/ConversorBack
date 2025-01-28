using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ConversorContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Currency> CurrenciesConvert { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public ConversorContext(DbContextOptions<ConversorContext> options)
            : base(options)
        {
        }

 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de la entidad User
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Subscription)
                .WithMany()
                .HasForeignKey(u => u.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la entidad Currency
            modelBuilder.Entity<Currency>()
                .HasKey(c => c.CurrencyId);

            modelBuilder.Entity<Currency>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la entidad Subscription
            modelBuilder.Entity<Subscription>()
                .HasKey(s => s.SubscriptionId);

            // Seed data para Subscription
            modelBuilder.Entity<Subscription>().HasData(
                new Subscription { SubscriptionId = 1, SubscriptionName = "Free", ConversionLimit = 10 },
                new Subscription { SubscriptionId = 2, SubscriptionName = "Trial", ConversionLimit = 100 },
                new Subscription { SubscriptionId = 3, SubscriptionName = "Pro", ConversionLimit = 0 }
            );

            // Seed data para Currency
            modelBuilder.Entity<Currency>().HasData(
                new Currency { CurrencyId = 1, CurrencyCode = "ARS", CurrencyLegend = "Peso Argentino", CurrencySymbol = "$", ConversionRate = 0.002M},
                new Currency { CurrencyId = 2, CurrencyCode = "EUR", CurrencyLegend = "Euro", CurrencySymbol = "€", ConversionRate = 1.09M},
                new Currency { CurrencyId = 3, CurrencyCode = "KC", CurrencyLegend = "Corona Checa", CurrencySymbol = "Kč", ConversionRate = 0.043M},
                new Currency { CurrencyId = 4, CurrencyCode = "USD", CurrencyLegend = "Dólar Americano", CurrencySymbol = "$", ConversionRate = 1.0M}
            );

        }
    }
}
