namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class GoldenToursAvailabilities : BaseAvailabilitiesEntity
    {
        public string ScheduleId { get; set; }
        public string ProductType { get; set; }
        public string RefNo { get; set; }
        public string PickupLocations { get; set; }
        public string TransferOptions { get; set; }
    }
}