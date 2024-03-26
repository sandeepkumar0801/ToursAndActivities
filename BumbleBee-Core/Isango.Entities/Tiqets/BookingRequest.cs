using System.Collections.Generic;

namespace Isango.Entities.Tiqets
{
    public class BookingRequest
    {
        public object RequestObject { get; set; }
        public string LanguageCode { get; set; }
        public string IsangoBookingReference { get; set; }
        public string TiquetsLanguageCode { get; set; }

        public string AffiliateId { get; set; }

        public List<TiqetsPackage> PackageId { get; set; }
    }
}