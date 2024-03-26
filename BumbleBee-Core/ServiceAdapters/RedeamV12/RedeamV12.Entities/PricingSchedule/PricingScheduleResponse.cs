using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.RedeamV12.RedeamV12.Entities.PricingSchedule
{
    public class PricingScheduleResponse
    {
        public string id { get; set; }
        public List<string> labels { get; set; }
        public string name { get; set; }
        public Net net { get; set; }
        public Retail retail { get; set; }
        public Original original { get; set; }
        public Travelertype travelerType { get; set; }
    }

    public class Net
    {
        public decimal amount { get; set; }
        public string currency { get; set; }
    }

    public class Retail
    {
        public decimal amount { get; set; }
        public string currency { get; set; }
    }
    public class Original
    {
        public decimal amount { get; set; }
        public string currency { get; set; }
    }

    public class Travelertype
    {
        public string ageBand { get; set; }
        public string name { get; set; }
    }

}
















