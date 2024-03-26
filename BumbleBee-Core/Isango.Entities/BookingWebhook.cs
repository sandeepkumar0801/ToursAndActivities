namespace Isango.Entities
{
    public class BookingWebhookRequest
    {
        public string BookingReferenceNumber { get; set; }
        public string VoucherURL { get; set; }
        public string BookingStatus { get; set; }
    }
}