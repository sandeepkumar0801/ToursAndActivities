using System;

namespace Isango.Entities.Cancellation
{
    public class CancelBookingMailDetail
    {
        public string BookingReferenceNumber { get; set; }
        public int ServiceId { get; set; }
        public string TokenId { get; set; }
        public string APIBookingReferenceNumber { get; set; }
        public DateTime TravelDate { get; set; }
        public string CustomerEmailId { get; set; }
        public string ContactNumber { get; set; }
        public string APICancellationStatus { get; set; }
        public string ApiTypeName { get; set; }
        public string IsangoBookingCancellationStatus { get; set; }
        public string PaymentRefundStatus { get; set; }
        public string PaymentRefundAmount { get; set; }
        public string ServiceName { get; set; }
        public string OptionName { get; set; }
        public int? BookedOptionID { get; set; }
    }
}