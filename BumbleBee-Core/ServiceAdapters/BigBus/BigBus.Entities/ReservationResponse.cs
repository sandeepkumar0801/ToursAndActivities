using Newtonsoft.Json;
using System;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class ReservationResponse
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "reservationResult")]
        public ReservationResult ReservationResult { get; set; }
    }

    public class ReservationResult
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "reservationReference")]
        public string ReservationReference { get; set; }

        [JsonProperty(PropertyName = "dateOfReservation")]
        public DateTime DateOfReservation { get; set; }

        [JsonProperty(PropertyName = "underName")]
        public UnderNameBase UnderName { get; set; }

        [JsonProperty(PropertyName = "products")]
        public Products Products { get; set; }
    }
}