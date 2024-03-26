using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels
{
    public class CancelRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("json_booking_info")]
        public BookingInfo BookingInfo { get; set; }
    }

    public class BookingInfo
    {
        [JsonProperty("PNRList")]
        public List<string> PnrList { get; set; }
    }
}