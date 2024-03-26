namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class VentrataAvailabilities : BaseAvailabilitiesEntity
    {
        public string VentrataProductId { get; set; }
        public string AvailabilityId { get; set; }
        //TODO - Offer code value may be an object
        public string OfferCode { get; set; }
        public string OfferTitle { get; set; }
        public string PickupPointsDetailsForVentrata { get; set; }
        public string MeetingPointDetails { get; set; }
        public string OpeningHoursDetails { get; set; }

        public string VentrataSupplierId { get; set; }

        public string VentrataBaseURL { get; set; }
    }
}
