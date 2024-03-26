namespace ServiceAdapters.Apexx.Apexx.Entities
{
    public class ApexxCriteria
    {
        public MethodType MethodType { get; set; }
        public string BaseUrl { get; set; }
        public string TransactionId { get; set; }
        public string Pares { get; set; }

        //public string Account { get; set; }
        public string Organisation { get; set; }

        public string Currency { get; set; }
        public string Amount { get; set; }
        public bool CaptureNow { get; set; }
        public string DynamicDescriptor { get; set; }
        public string MerchantReference { get; set; }
        public string CustomerIp { get; set; }
        public string UserAgent { get; set; }
        public string ThreeDsRequired { get; set; }

        public string CardNumber { get; set; }
        public string SecurityCode { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CaptureRefernce { get; set; }

        public string Reason { get; set; }

        public BillingAddress BillingAddress { get; set; }

        public string CaptureGuid { get; set; }

        public bool IsOnRequestProduct { get; set; }

        public string ApexxToken { get; set; }
    }
}