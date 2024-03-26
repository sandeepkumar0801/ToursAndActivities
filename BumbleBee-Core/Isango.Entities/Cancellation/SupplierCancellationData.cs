namespace Isango.Entities.Cancellation
{
    public class SupplierCancellationData
    {
        public int ApiType { get; set; }
        public string ServiceLongName { get; set; }
        public string SupplierBookingReferenceNumber { get; set; }
        public string SupplierBookingLineNumber { get; set; }
        public string OfficeCode { get; set; }
        public string BookingReferenceNumber { get; set; }
        public int BookedOptionId { get; set; }
        public string Status { get; set; }
        public string ServiceOptionName { get; set; }
        public string TravelDate { get; set; }
        public int BookedOptionStatusId { get; set; }
        public int? CountryId { get; set; }
        public string FHBSupplierShortName { get; set; }
        public string CostCurrencyCode { get; set; }
        public string CountryName { get; set; }
    }
}