using Newtonsoft.Json;
using System.Collections.Generic;

namespace Isango.Entities.AdyenPayment
{
  
    public class PaymentMethodsResponse
    {
        [JsonProperty(PropertyName = "paymentMethods")]
        public List<Paymentmethods> PaymentMethods { get; set; }
    }

    public class Paymentmethods
    {
        [JsonProperty(PropertyName = "details")]
        public List<Detail> Details { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "brands")]
        public List<string> Brands { get; set; }
        [JsonProperty(PropertyName = "configuration")]
        public Configuration Configuration { get; set; }
    }

    public class Configuration
    {
        [JsonProperty(PropertyName = "merchantId")]
        public string MerchantId { get; set; }
        [JsonProperty(PropertyName = "merchantName")]
        public string MerchantName { get; set; }
        [JsonProperty(PropertyName = "intent")]
        public string Intent { get; set; }
        
    }

    public class Detail
    {
        [JsonProperty(PropertyName = "items")]
        public List<Item> Items { get; set; }
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "optional")]
        public bool Optional { get; set; }
    }

    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

}