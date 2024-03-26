namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class PaymentData : EntityBase
    {
        public string Description { get; set; }
        public string PaymentType { get; set; }
        public string InvoiceCompanyCode { get; set; }
        public string InvoiceCompanyName { get; set; }
        public string InvoiceCompanyRegistrationNumber { get; set; }
    }
}