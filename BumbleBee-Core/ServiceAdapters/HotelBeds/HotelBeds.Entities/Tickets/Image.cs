namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class Image : EntityBase
    {
        public string Type { get; set; }
        public int Order { get; set; }
        public int VisualizationOrder { get; set; }
        public string Url { get; set; }
    }
}