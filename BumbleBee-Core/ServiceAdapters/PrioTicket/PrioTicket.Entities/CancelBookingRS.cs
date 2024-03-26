using Newtonsoft.Json;
using System;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{
    public class CancelBookingRs : EntityBase
    {
        [JsonProperty(PropertyName = "response_type")]
        public string ResponseType { get; set; }

        [JsonProperty(PropertyName = "data")]
        public CancelBookingRsData Data { get; set; }
    }

    public class CancelBookingRsData
    {
        [JsonProperty(PropertyName = "booking_reference")]
        public string BookingReference { get; set; }

        [JsonProperty(PropertyName = "distributor_reference")]
        public string DistributorReference { get; set; }

        [JsonProperty(PropertyName = "booking_status")]
        public string BookingStatus { get; set; }

        [JsonProperty(PropertyName = "cancellation_date_time")]
        public DateTime CancellationDateTime { get; set; }
    }
}