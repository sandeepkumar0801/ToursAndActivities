namespace ServiceAdapters.Adyen.Adyen.Entities
{
    public class AdyenCriteria
    {
        public MethodType MethodType { get; set; }
        public string BaseUrl { get; set; }
        public string TransactionId { get; set; }
        public string fingerprint { get; set; }
        public string Challenge { get; set; }
        public string Pares { get; set; }
        public string Organisation { get; set; }

        public string Currency { get; set; }
        public string Amount { get; set; }
        public bool CaptureNow { get; set; }
        
        public string MerchantReference { get; set; }
        public string CustomerIp { get; set; }
        public string UserAgent { get; set; }
        public string ThreeDsRequired { get; set; }
        public string CustomerEmail { get; set; }

        public string Origin { get; set; }

        

        public string CardNumber { get; set; }
        public string SecurityCode { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CaptureRefernce { get; set; }


        public string UserCountry { get; set; }
        public string UserCity { get; set; }
        public string UserStreet { get; set; }
        public string UserPostalCode { get; set; }
        public string CountryCode { get; set; }
        public string UserStateOrProvince { get; set; }
        public string MerchantAccount { get; set; }
        public string ShopperLocale { get; set; }
        public string PaymentData { get; set; }
        public string MD { get; set; }

        
        public string PspReference { get; set; }
        public Browserinfo Browserinfo { get; set; }
        public string CardType { get; set; }

        public string FacilitatorAccessToken { get; set; }
    }

}