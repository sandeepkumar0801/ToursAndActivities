using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Reservation
{
    public class ReservationResponse
    {
        [JsonProperty("reservationId")]
        public string ReservationId { get; set; }
        [JsonProperty("reservationExpiration")]
        public string ReservationExpiration { get; set; }
    }
}