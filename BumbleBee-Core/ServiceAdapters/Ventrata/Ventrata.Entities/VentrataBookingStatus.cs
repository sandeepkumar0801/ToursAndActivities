using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Ventrata.Ventrata.Entities
{
    
    public static class VentrataBookingStatus
    {
        public const string Cancelled = "CANCELLED";
        public const string Confirmed = "CONFIRMED";
        public const string OnHold = "ON_HOLD";
        public const string Expired = "EXPIRED";
    }

}
