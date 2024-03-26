namespace Isango.Entities.RedeamV12
{
    public class RateData
    {
        public string RateId { get; set; }
        public string RateCode { get; set; }
        public string RateName { get; set; }
        public bool Cancelable { get; set; }
        public int Cutoff { get; set; }
        public bool Holdable { get; set; }
        public int HoldablePeriod { get; set; }
        public string Hours { get; set; }
        public int MaxTravelers { get; set; }
        public int MinTravelers { get; set; }
        public string PartnerId { get; set; }
        public string Type { get; set; }
        public bool IsRefundable { get; set; }

        public string PricingType { get; set; }

        public string ProductId { get; set; }
    }
}