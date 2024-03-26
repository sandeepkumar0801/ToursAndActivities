using System;

namespace Isango.Entities.Mailer
{
    public class FailureMailContext
    {
        public string BookingReferenceNumber { get; set; }
        public int ServiceId { get; set; }
        public string TokenId { get; set; }
        public string APIBookingReferenceNumber { get; set; }
        public DateTime TravelDate { get; set; }
        public string CustomerEmailId { get; set; }
        public string ContactNumber { get; set; }
        public bool APICancellationStatus { get; set; }

        public string ApiTypeName { get; set; }
        public string ServiceOptionId { get; set; }
        public string AvailabilityReferenceId { get; set; }
        public string OptionName { get; set; }

        public string SupplierOptionCode { get; set; }
        public string BookingErrors { get; set; }
        public string ApiRequest { get; set; }
        public string ApiResponse { get; set; }

        public string CustomerName { get; set; }

        public string ApiErrorMessage { get; set; }

    }
}