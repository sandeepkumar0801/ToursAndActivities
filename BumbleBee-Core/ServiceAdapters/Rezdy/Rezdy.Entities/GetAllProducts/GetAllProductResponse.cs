using Newtonsoft.Json;

using System.Collections.Generic;

namespace ServiceAdapters.Rezdy.Rezdy.Entities.ProductDetails
{
    public class GetAllProductResponse
    {
        [JsonProperty("products")]
        public List<Product> Products { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }
    }
}