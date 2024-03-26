using System;

namespace Isango.Entities
{
    public class ActivityOffers
    {
        public int OfferId { get; set; }
        public int ServiceId { get; set; }
        public string OfferText { get; set; }
        public int OfferOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}