using System;

namespace Isango.Entities.ConsoleApplication.ServiceAvailability
{
    public class TempHBServiceDetail
    {
        public string ProductCode { get; set; }
        public string Modality { get; set; }
        public DateTime AvailableOn { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string ProductClass { get; set; }
        public int FactSheetID { get; set; }

        // public DateTime? CreatedOn { get; set; }
        public decimal TicketOfficePrice { get; set; }

        public int MinAdult { get; set; }
        public int ActivityId { get; set; }
        public decimal CommissionPercent { get; set; }
        public decimal SellPrice { get; set; }
        public string Status { get; set; }

        //TODO: Added as per table type '[AvailabilityTableType]'
        public int ServiceOptionID { get; set; }

        public TimeSpan StartTime { get; set; }
        public string Variant { get; set; }

        #region Pax Type Changes

        public int PassengerTypeId { get; set; }
        public string UnitType { get; set; }

        #endregion Pax Type Changes

        public int Capacity { get; set; }

        public bool IsFinalGateAmount { get; set; }
        public string CancellationPolicy { get; set; }

        public string supplieroptionname { get; set; }

    }
}