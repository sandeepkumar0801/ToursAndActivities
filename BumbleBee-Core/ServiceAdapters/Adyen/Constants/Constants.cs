namespace ServiceAdapters.Adyen.Constants
{
    public sealed class Constant
    {
        public const string AdyenBaseUrl = "AdyenBaseUrl";
        public const string AdyenEnrollmentCheckUrl = "payments";
        public const string AdyenPaymentsBaseUrl = "AdyenPaymentsBaseUrl";
        public const string AdyenPaymentMethodsUrl = "paymentMethods";
        public const string AdyenPaymentDetailUrl = "payments/details";
        public const string AdyenMerchantAccount = "AdyenMerchantAccount";
        public const string AdyenMerchantAccountNew = "AdyenMerchantAccountNew";
        public const string AdyenMerchantAccountStringent = "AdyenMerchantAccountStringent";
        public const string AdyenMerchantAccountCOMPAYBYLINK = "AdyenMerchantAccountCOMPAYBYLINK";
        public const string AdyenReleaseDate = "AdyenReleaseDate";
        public const string AdyenCaptureCheckUrl = "capture";
        public const string AdyenRefundUrl = "refund";
        public const string AdyenCancelUrl = "cancel";
        public const string Status = "status";
        public const string ThreeDSRequired = "ThreeDSRequired";
        public const string SaveInStorage = "SaveInStorage";
        public const string SaveInStorageValue = "1";
        public const string DefaultUserAgent =
            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB;rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 (.NET CLR 3.5.30729)";
        public const string AdyenPaymentLinksUrl = "paymentLinks";
        public const string AdyenBlockPaymentMethods = "AdyenBlockPaymentMethods";
        
    }

    public enum AdyenMerchantType
    {
        AdyenOldAccount = 0,
        AdyenNewAccount = 1,
        AdyenStringentAccount = 2,
    }
}