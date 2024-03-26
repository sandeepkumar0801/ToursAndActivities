namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class AotAvailabilities : BaseAvailabilitiesEntity
    {
        public string RoomType { get; set; }
        public string ServiceType { get; set; }
        public string OptionType { get; set; }
        public string SupplierCancellationPolicy { get; set; }
    }
}