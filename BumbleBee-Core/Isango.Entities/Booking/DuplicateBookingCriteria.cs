using System;

namespace Isango.Entities.Booking
{
    public class DuplicateBookingCriteria
    {
        public int SmcPasswordId { get; set; }
        public DateTime TravelDate { get; set; }
        public int ServiceOptionId { get; set; }
        public int AdultCount { get; set; }
        public string LeadPaxName { get; set; }
        public string AffiliateId { get; set; }
        public string UserEmailId { get; set; }
        public string AvailabilityReferenceIds { get; set; }
    }
}