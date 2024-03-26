namespace ServiceAdapters.Bokun.Bokun.Entities.CheckAvailabilities
{
    public class CheckAvailabilitiesRq
    {
        public string StartDate { get; set; }
        public int? ActivityId { get; set; }
        public string EndDate { get; set; }
        public string CurrencyIsoCode { get; set; }
    }
}