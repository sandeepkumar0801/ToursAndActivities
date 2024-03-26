using Newtonsoft.Json;

namespace ServiceAdapters.Bokun.Bokun.Entities.CancelBooking
{
    public class CancelBookingRq
    {
        [JsonIgnore]
        public string BookingConfirmationCode { get; set; }

        public string Note { get; set; }
        public bool Notify { get; set; }
        public bool Refund { get; set; }

        public CancelBookingRq()
        {
            Note = "Cancellation through Bokun API";
            Notify = true;
            Refund = true;
        }
    }
}