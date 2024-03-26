using Newtonsoft.Json;
using System;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{
    public class CancelReservationRs : EntityBase
    {
        [JsonProperty(PropertyName = "response_type")]
        public string ResponseType { get; set; }

        [JsonProperty(PropertyName = "data")]
        public CancelReservationRsData Data { get; set; }
    }

    public class CancelReservationRsData
    {
        [JsonProperty(PropertyName = "reservation_reference")]
        public string ReservationReference { get; set; }

        [JsonProperty(PropertyName = "distributor_reference")]
        public string DistributorReference { get; set; }

        [JsonProperty(PropertyName = "booking_status")]
        public string BookingStatus { get; set; }

        [JsonProperty(PropertyName = "cancellation_date_time")]
        public DateTime CancellationDateTime { get; set; }
    }
}