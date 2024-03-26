using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Booking
{
    public class BookingResponse
    {
        [JsonProperty("orderCode")]
        public string OrderCode { get; set; }
        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }
        [JsonProperty("lines")]
        public List<Line> Lines { get; set; }

    }
    public class Line
    {
        [JsonProperty("orderLineCode")]
        public string OrderLineCode { get; set; }
        [JsonProperty("rate")]
        public string Rate { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("qrCode")]
        public string QrCode { get; set; }
     }
}