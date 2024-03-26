using System.Collections.Generic;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class AvailableModality : EntityBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string SupplierOption { get; set; }
        public Contract Contract { get; set; }
        public List<TicketPrice> PriceList { get; set; }
        public AvailableModalityType Type { get; set; }
        public Mode Mode { get; set; }
        public List<OperationDate> OperationDateList { get; set; }
        public ChildAge ChildAge { get; set; }
        public List<PriceRange> PriceRangeList { get; set; }
        public char TicketGeneration { get; set; }
        public int ContentSequence { get; set; }
        public List<CancellationPolicy> CancellationCharges { get; set; }
    }
}