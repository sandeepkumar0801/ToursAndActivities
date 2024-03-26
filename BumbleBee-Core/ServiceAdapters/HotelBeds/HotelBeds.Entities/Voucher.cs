namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class Voucher : EntityBase
    {
        public string Code { get; set; }
        public string Url { get; set; }
        public string MimeType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LanguageCode { get; set; }
    }
}