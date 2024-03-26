using Newtonsoft.Json;
using System;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class Customer
    {
        [JsonProperty("customer_type_rate")]
        public Int64 CustomerTypeRate { get; set; }
    }
}