using Newtonsoft.Json;

namespace ServiceAdapters.Apexx.Apexx.Entities
{
    public class ThreeDVerifyRequest
    {
        [JsonProperty(PropertyName = "_id")]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "paRes")]
        public string PaRes { get; set; }
    }
}