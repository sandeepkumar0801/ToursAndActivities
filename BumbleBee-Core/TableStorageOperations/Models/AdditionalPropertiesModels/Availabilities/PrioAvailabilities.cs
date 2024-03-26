namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class PrioAvailabilities : BaseAvailabilitiesEntity
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Vacancies { get; set; }
        public int PrioTicketClass { get; set; }
        public string PickupPoints { get; set; }
    }
}