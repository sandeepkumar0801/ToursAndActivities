using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Ventrata.Ventrata.Entities.Request
{
    public class AvailabilityReq
    {
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "optionId")]
        public string OptionId { get; set; }

        [JsonProperty(PropertyName = "localDateStart")]
        public string CheckinDate { get; set; }

        [JsonProperty(PropertyName = "localDateEnd")]
        public string CheckoutDate { get; set; }

        [JsonProperty(PropertyName = "units")]
        public List<PassengerDetails> PassengerDetails { get; set; }
    }

    public class PassengerDetails {
        [JsonProperty(PropertyName = "id")]
        public string PassengerId { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }
    }
}
