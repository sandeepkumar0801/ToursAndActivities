namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class BokunAvailabilities : BaseAvailabilitiesEntity
    {
        public string PricingCategoryId { get; set; }
        public int StartTimeId { get; set; }
        public string RateId { get; set; }
    }
}