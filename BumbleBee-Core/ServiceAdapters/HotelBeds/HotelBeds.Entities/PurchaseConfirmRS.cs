namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class PurchaseConfirmRs : EntityBase
    {
        public string EchoToken { get; set; }
        public AuditData AuditData { get; set; }
        public Purchase Purchase { get; set; }
    }
}