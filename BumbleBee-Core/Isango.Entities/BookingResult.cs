using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class BookingResult : ErrorList
    {
        public BookingStatus BookingStatus { get; set; }
        public string StatusMessage { get; set; }
        public string RequestHtml { get; set; }
        public string BookingRefNo { get; set; }
        public string Url { get; set; }
        public string TransactionGuwid { get; set; }
        public string TransactionId { get; set; }
        public bool IsDuplicateBooking { get; set; }
        public bool IsWebhookReceived { get; set; }
        public string FallbackFingerPrint { get; set; }
    }
}