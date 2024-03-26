using System;

namespace Isango.Entities
{
    public class PaxPriceRequest
    {
        public string AffiliateId { get; set; }
        public int ServiceId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string PaxDetail { get; set; }
    }
}