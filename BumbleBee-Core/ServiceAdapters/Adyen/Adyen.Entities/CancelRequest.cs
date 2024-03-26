using Newtonsoft.Json;

namespace ServiceAdapters.Adyen.Adyen.Entities
{
    public class CancelRequest
    {
        [JsonProperty(PropertyName = "originalReference")]
        public string OriginalReference { get; set; }
        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }
        [JsonProperty(PropertyName = "merchantAccount")]
        public string MerchantAccount { get; set; }
    }


}

