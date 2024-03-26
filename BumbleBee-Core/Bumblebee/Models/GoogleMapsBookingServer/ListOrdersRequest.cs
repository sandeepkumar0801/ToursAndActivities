using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebAPI.Models.GoogleMapsBookingServer
{
    public class ListOrdersRequest
    {
        [JsonProperty("user_id ")]
        public string UserId { get; set; }
        [JsonProperty("order_ids ")]
        public OrderIds OrderIds { get; set; }
    }

    public class OrderIds
    {
        [JsonProperty("order_id ")]
        public List<string> OrderId { get; set; }
    }
}