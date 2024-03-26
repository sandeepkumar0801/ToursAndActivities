using Newtonsoft.Json;

namespace Isango.Entities.Tiqets
{
    public class ConfirmOrderResponse
    {
        [JsonProperty(PropertyName = "order_reference_id")]
        public string OrderReferenceId { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}