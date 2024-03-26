using Newtonsoft.Json;
using System;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Booking
{
    public class CancelBookingRequest
    {
        [JsonProperty("externalServiceRefCode")]
        public string ExternalServiceRefCode { get; set; }
        [JsonProperty("reservationId")]
        public string ReservationId { get; set; }
    }
}