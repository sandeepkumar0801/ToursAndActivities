namespace ServiceAdapters.Apexx.Constants
{
    public sealed class Constant
    {
        public const string ApexxBaseUrl = "ApexxBaseUrl";
        public const string Status = "status";
        public const string ThreeDSRequired = "ThreeDSRequired";

        public const string EnrollmentCheckUrl = "payment/direct";
        public const string ThreeDSVerifyUrl = "payment/3ds/authenticate";
        public const string CaputureUrl = "capture/";
        public const string RefundUrl = "refund/";
        public const string CancelUrl = "/cancel";
        public const string CancelCaptureUrl = "cancel/capture/";
        public const string CreateCardTransactionUrl = "payment/direct";
        //public const string Account = "b5ca22022ff0479d922df91c78dbc8cf";
        public const string Organisation = "ApexxOrganisation";

        public const string DynamicDescriptor = "Isango";
        public const string MerchantReference = "ISANGO0001";
        public const string ApexxCountry = "GB";
        public const string SaveInStorage = "SaveInStorage";
        public const string SaveInStorageValue = "1";

        public const string DefaultUserAgent =
            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB;rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 (.NET CLR 3.5.30729)";
    }
}