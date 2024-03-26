using Newtonsoft.Json;

namespace ServiceAdapters.Apexx.Apexx.Entities
{
    public class CancelRequest
    {
        [JsonProperty(PropertyName = "_id")]
        [JsonIgnore]
        public string TransactionId { get; set; }
    }
}