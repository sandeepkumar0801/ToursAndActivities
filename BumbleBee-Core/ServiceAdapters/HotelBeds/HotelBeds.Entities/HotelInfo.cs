namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class HotelInfo : EntityBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public Destination Destination { get; set; }
        public Position Position { get; set; }
    }
}