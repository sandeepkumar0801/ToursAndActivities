namespace ServiceAdapters.FareHarbor.FareHarbor.Entities
{
    public class FareHarborRequest
    {
        public string ShortName { get; set; }
        public string Item { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Availability { get; set; }
        public string Uuid { get; set; }
    }
}