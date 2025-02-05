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
            // Configurar la relación entre User y Subscription
            modelBuilder.Entity<User>()
                .HasOne(u => u.Subscription)
                .WithMany()
                .HasForeignKey(u => u.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Definir valores por defecto para la suscripción
            modelBuilder.Entity<User>()
                .Property(u => u.SubscriptionId)
                .HasDefaultValue(1); // No Subscription


            modelBuilder.Entity<Subscription>().HasData(
                new Subscription { SubscriptionId = 1, SubscriptionName = "No Subscription", ConversionLimit = 0 },
                new Subscription { SubscriptionId = 2, SubscriptionName = "Free", ConversionLimit = 10 },
                new Subscription { SubscriptionId = 3, SubscriptionName = "Trial", ConversionLimit = 100 },
                new Subscription { SubscriptionId = 4, SubscriptionName = "Pro", ConversionLimit = int.MaxValue }
            );
        }
    }
}
