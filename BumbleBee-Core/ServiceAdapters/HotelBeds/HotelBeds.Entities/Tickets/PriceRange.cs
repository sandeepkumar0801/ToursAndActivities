namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class PriceRange : EntityBase
    {
        public string Type { get; set; }
        public int AgeFrom { get; set; }
        public int AgeTo { get; set; }
        public decimal UnitPrice { get; set; }
    }
}