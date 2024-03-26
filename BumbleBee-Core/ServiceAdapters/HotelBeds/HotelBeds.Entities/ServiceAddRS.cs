namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class ServiceAddRs : EntityBase
    {
        public string EchoToken { get; set; }
        public AuditData AuditData { get; set; }
        public Purchase Purchase { get; set; }

        public object InputCritera { get; set; }
    }
}