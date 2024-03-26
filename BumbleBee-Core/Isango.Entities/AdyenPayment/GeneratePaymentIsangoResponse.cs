using System.Collections.Generic;

namespace Isango.Entities.AdyenPayment
{
    public class GeneratePaymentIsangoResponse
    {
        public GeneratePaymentAmountRS GeneratePaymentAmount { get; set; }
        public string CountryCode { get; set; }
        public string Description { get; set; }
        public string ExpiresAt { get; set; }
        public string Id { get; set; }
        public string MerchantAccount { get; set; }
        public string Reference { get; set; }
        public string ShopperLocale { get; set; }
        public string ShopperReference { get; set; }
        public string Url { get; set; }
    }
    public class GeneratePaymentAmountRS
    {
       public string Value { get; set; }
       public string Currency { get; set; }
    }
}