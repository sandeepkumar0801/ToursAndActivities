using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Adyen.Adyen.Entities
{
    public class ThreeDVerifyRequest
    {
        [JsonProperty(PropertyName = "paymentData")]
        public string PaymentData { get; set; }
        [JsonProperty(PropertyName = "details")]
        public Dictionary<string, string> Details { get; set; }
    }

    public class DetailsThreeD
    {
        [JsonProperty(PropertyName = "MD")]
        public string MD { get; set; }
        [JsonProperty(PropertyName = "PaRes")]
        public string PaRes { get; set; }
    }

}