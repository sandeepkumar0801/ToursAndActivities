namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class BarcodeImage : EntityBase
    {
        public int Order { get; set; }
        public string Type { get; set; }
        public string Encoding { get; set; }
        public string Url { get; set; }
    }
}