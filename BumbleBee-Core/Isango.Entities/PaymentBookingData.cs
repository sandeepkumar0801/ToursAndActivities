namespace Isango.Entities
{
    public class PaymentBookingData
    {
        public string AffiliateId { get; set; }

        public string LanguageCode { get; set; }

        public string BookingRefNo { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmailAdd { get; set; }

        public string CustomerPhoneNo { get; set; }

        public string BookedProductName { get; set; }

        public string BookedOptionName { get; set; }

        public int BookedOptionId { get; set; }

        public string CurrencySymbol { get; set; }

        public string CurrencyName { get; set; }

        public decimal ChargedAmount { get; set; }

        public string CSRemarks { get; set; }

        public int UserId { get; set; }

        public bool IsOnRequest { get; set; }

        public string CompanyWebsite { get; set; }

        public string AffiliateName { get; set; }

        public bool Error { get; set; }

        public int CustomerCountryId { get; set; }
    }
}