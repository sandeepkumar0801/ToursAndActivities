using System.Collections.Generic;
using Isango.Entities.GoogleMaps.BookingServer;
using Newtonsoft.Json;

namespace WebAPI.Models.GoogleMapsBookingServer
{
    public class ListOrdersResponse
    {
        [JsonProperty("order")]
        public List<Order> Orders { get; set; }
    }
}