using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels.CheckAvailability
{
    public class BundleOption
    {
        public int Id { get; set; }
        public string BundleOptionName { get; set; }
        public decimal BasePrice { get; set; }
        public decimal GateBasePrice { get; set; }
        public string BundleOptionReferenceIds { get; set; }
        public string CurrencyIsoCode { get; set; }
        public int BundleOptionOrder { get; set; }
        public List<Option> ComponentOptions { get; set; }
        public bool IsCapacityCheckRequired { get; set; }
        public int Capacity { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Variant { get; set; }
    }
}