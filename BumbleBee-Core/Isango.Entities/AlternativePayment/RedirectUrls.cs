using Newtonsoft.Json;

namespace Isango.Entities.AlternativePayment
{
    public class RedirectUrls
    {
        [JsonProperty(PropertyName = "returnUrl")]
        public string ReturnUrl { get; set; }

        [JsonProperty(PropertyName = "cancelUrl")]
        public string CancelUrl { get; set; }
    }
}