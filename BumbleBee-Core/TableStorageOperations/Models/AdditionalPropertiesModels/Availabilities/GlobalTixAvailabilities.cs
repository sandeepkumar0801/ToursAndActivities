namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class GlobalTixAvailabilities : BaseAvailabilitiesEntity
    {
		public string RateKey { get; set; }
        
        public int TourDepartureId { get; set; }

        public string TicketTypeIds { get; set; }
        public string ContractQuestions { get; set; }
    }
}