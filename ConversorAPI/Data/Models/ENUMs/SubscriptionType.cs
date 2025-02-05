using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.ENUMs
{
    public enum SubscriptionType
    {
        NoSubscription = 0,
        Free = 10,      // Límite de 5 conversiones
        Trial = 100,   // Límite de 100 conversiones
        Pro = int.MaxValue // Sin límite
    }
}
