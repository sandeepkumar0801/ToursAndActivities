using System;

namespace Isango.Entities.GoogleMaps
{
    public class ServiceAvailabilityFeed
    {
        public string ProductCode { get; set; }
        public string Modality { get; set; }
        public DateTime AvailableOn { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string ProductClass { get; set; }
        public int Factsheetid { get; set; }
        public decimal TicketOfficePrice { get; set; }
        public int MinAdult { get; set; }
        public int ActivityId { get; set; }
        public decimal CommissionPercent { get; set; }
        public decimal SellPrice { get; set; }
        public string Status { get; set; }
        public int Serviceoptionid { get; set; }
        public TimeSpan StartTime { get; set; }
        public string Variant { get; set; }
        public int PassengerTypeId { get; set; }
        public int UnitType { get; set; }
        public string FromAge { get; set; }
        public string ToAge { get; set; }
    }
}