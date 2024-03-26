namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class MoulinRougeAvailabilities : BaseAvailabilitiesEntity
    {
        public int? CatalogDateId { get; set; }
        public int? RateId { get; set; }
        public int? CategoryId { get; set; }
        public int? BlocId { get; set; }
        public int? FloorId { get; set; }
        public int? ContingentId { get; set; }
    }
}