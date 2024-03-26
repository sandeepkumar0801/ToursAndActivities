namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class TourCMSAvailabilities : BaseAvailabilitiesEntity
    {
        public string TourCMSProductId { get; set; }
        public string AvailabilityId { get; set; }
        public string PickupPointsDetails { get; set; }
        public string MeetingPointDetails { get; set; }
        //public string OpeningHoursDetails { get; set; }
    }
}
