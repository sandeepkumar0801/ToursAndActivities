namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class RedeamAvailabilities : BaseAvailabilitiesEntity
    {
        public string PriceId { get; set; }
        public string RateId { get; set; }
        public string SupplierId { get; set; }
        public bool Cancellable { get; set; }
        public bool Holdable { get; set; }
        public bool Refundable { get; set; }
        public string Type { get; set; }
        public int HoldablePeriod { get; set; }
        public string Time { get; set; }

        public string RedeamAvailabilityId { get; set; }

        public string RedeamAvailabilityStart { get; set; }
    }
}