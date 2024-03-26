namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class RaynaAvailabilities : BaseAvailabilitiesEntity
    {
        public string TourId { get; set; }
        public string TourOptionId { get; set; }
        public string TransferId { get; set; }
        public int TimeSlotId { get; set; }
        public string TourStartTime { get; set; }
    }
}
