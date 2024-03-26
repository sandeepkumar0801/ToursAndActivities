using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.GoCity.GoCity.Entities.Product
{
    public class ProductResponse
    {
        [JsonProperty("productResponseList")]
        public List<Productresponselist> ProductResponseList { get; set; }
        [JsonProperty("message")]
        public object Message { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }

    public class Productresponselist
    {
        [JsonProperty("requestProductSkuCode")]
        public string RequestProductSkuCode { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("productSkuCodeName")]
        public string ProductSkuCodeName { get; set; }
    }

}