namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class Promotion : EntityBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Observations { get; set; }
    }
}