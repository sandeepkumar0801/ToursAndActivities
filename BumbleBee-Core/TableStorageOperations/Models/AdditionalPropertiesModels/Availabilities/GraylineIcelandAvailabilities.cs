namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class GraylineIcelandAvailabilities : BaseAvailabilitiesEntity
    {
        public int TourDepartureId { get; set; }
        public string TourNumber { get; set; }
        public string PickupLocations { get; set; }
    }
}