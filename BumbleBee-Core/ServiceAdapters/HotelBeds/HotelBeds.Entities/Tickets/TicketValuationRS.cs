using Isango.Entities.HotelBeds;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class TicketValuationRs : EntityBase
    {
        public string EchoToken { get; set; }
        public AuditData AuditData { get; set; }
        public ServiceList ServiceTicket { get; set; }

        public HotelBedsSelectedProduct HotelBedsSelectedProduct { get; set; }
    }
}