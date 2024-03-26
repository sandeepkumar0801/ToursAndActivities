using Isango.Entities;

namespace TableStorageOperations.Models.Booking
{
    public class BookingCallBackRequest : CustomTableEntity
    {
        public string BookingReferenceNumber { get; set; }

        public string BookingResponse { get; set; }

        public string Status { get; set; }

        public string Token { get; set; }

        public string StatusCode { get; set; }
    }
}