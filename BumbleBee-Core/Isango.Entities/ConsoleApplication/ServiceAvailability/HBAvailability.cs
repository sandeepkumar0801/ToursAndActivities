using System;

namespace Isango.Entities.ConsoleApplication.ServiceAvailability
{
    public class HBAvailability
    {
        public int Id { get; set; }
        public int? IsangoServiceId { get; set; }
        public string ProductCode { get; set; }
        public DateTime? AvailableOn { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; }
        public int? FactSheetId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? IsangoRegionId { get; set; }
        public decimal? TicketOfficePrice { get; set; }
        public int? MinAdult { get; set; }
        public decimal? ChildPrice { get; set; }
    }
}