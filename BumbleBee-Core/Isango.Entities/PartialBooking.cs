using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class PartialBooking
    {
        public int Id { get; set; }
        public int SelectedProductId { get; set; }
        public PartialBookingStatus ItemStatus { get; set; }
        public string AvailabilityReferenceId { get; set; }
        public string BookingReferenceNumber { get; set; }
        public int OperationType { get; set; }
    }
}