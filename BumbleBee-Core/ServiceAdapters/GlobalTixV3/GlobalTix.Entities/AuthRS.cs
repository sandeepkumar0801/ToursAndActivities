using Newtonsoft.Json;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
    public class AuthRS
    {

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "data")]
        public Datas Data { get; set; }

        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        

        public class Datas
        {
            //[JsonProperty(PropertyName = "roles")]
            //public string[] Roles { get; set; }

            [JsonProperty(PropertyName = "token_type")]
            public string Token_type { get; set; }

            [JsonProperty(PropertyName = "access_token")]
            public string Access_token { get; set; }

            //[JsonProperty(PropertyName = "user")]
            //public User User { get; set; }

            [JsonProperty(PropertyName = "expires_in")]
            public int Expires_in { get; set; }

            //[JsonProperty(PropertyName = "refresh_token")]
            //public string Refresh_token { get; set; }
        }

        //public class User
        //{
        //    public int id { get; set; }
        //    public object firstname { get; set; }
        //    public object lastname { get; set; }
        //    public string username { get; set; }
        //    public object merchant { get; set; }
        //    public object supplierApi { get; set; }
        //    public Reseller reseller { get; set; }
        //    public object backoffice { get; set; }
        //    //public Currency currency { get; set; }
        //    public bool isProxyUser { get; set; }
        //    public bool isUsing2FA { get; set; }
        //    public bool enable2fa { get; set; }
        //}

        //public class Reseller
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //    public object accountManager { get; set; }
        //    public object attachmentLogoUrl { get; set; }
        //    public string code { get; set; }
        //    public object commissionBasedAgent { get; set; }
        //    //public Country country { get; set; }
        //    //public Createby createBy { get; set; }
        //    public object createdBy { get; set; }
        //    //public Credit credit { get; set; }
        //    public float creditCardFee { get; set; }
        //    public bool creditCardPaymentOnly { get; set; }
        //    public object customEmailAPI { get; set; }
        //    public string customEmailFilename { get; set; }
        //    //public Customemailtype customEmailType { get; set; }
        //    public DateTime dateCreated { get; set; }
        //    //public Emailconfig emailConfig { get; set; }
        //    public string emailLogoUrl { get; set; }
        //    //public Externalreseller[] externalReseller { get; set; }
        //    public object globalMarkup { get; set; }
        //    public bool hasBeenNotifiedLowCreditLimit { get; set; }
        //    //public Headquarters headquarters { get; set; }
        //    public string internalEmail { get; set; }
        //    public object isAttachmentLogo { get; set; }
        //    public bool isEmailLogo { get; set; }
        //    public bool isMerchantBarcodeOnly { get; set; }
        //    public bool isSubAgentOnly { get; set; }
        //    public DateTime lastUpdated { get; set; }
        //    public string lastUpdatedBy { get; set; }
        //    public float lowCreditLimit { get; set; }
        //    public bool mainReseller { get; set; }
        //    public object[] merchantGroups { get; set; }
        //    public string mobileNumber { get; set; }
        //    public object monthlyFee { get; set; }
        //    public string name { get; set; }
        //    public bool noAttachmentPurchase { get; set; }
        //    public bool noCustomerEmail { get; set; }
        //    public bool notifyLowCredit { get; set; }
        //    public string notifyLowCreditEmail { get; set; }
        //    //public Onlinestore onlineStore { get; set; }
        //    //public Ownmerchant ownMerchant { get; set; }
        //    public object paymentProcessingFee { get; set; }
        //    //public Pos pos { get; set; }
        //    //public Presetgroup[] presetGroups { get; set; }
        //    //public Primarypresetgroup primaryPresetGroup { get; set; }
        //    public object salesCommission { get; set; }
        //    public bool sendCustomEmail { get; set; }
        //    //public Size size { get; set; }
        //    //public Status status { get; set; }
        //    //public Subagentgroup[] subAgentGroups { get; set; }
        //    public int transactionExpire { get; set; }
        //    public float transactionFee { get; set; }
        //    public object transactionFeeCap { get; set; }
        //    //public Transactionfeetype transactionFeeType { get; set; }
        //    public object useCallback { get; set; }
        //    //public Usecase useCase { get; set; }
        //   // public Webhook[] webhook { get; set; }
        //}

        //public class Country
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Createby
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Credit
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Customemailtype
        //{
        //    public string enumType { get; set; }
        //    public string name { get; set; }
        //}

        //public class Emailconfig
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Headquarters
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Onlinestore
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Ownmerchant
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Pos
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Primarypresetgroup
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Size
        //{
        //    public string enumType { get; set; }
        //    public string name { get; set; }
        //}

        //public class Status
        //{
        //    public string enumType { get; set; }
        //    public string name { get; set; }
        //}

        //public class Transactionfeetype
        //{
        //    public string enumType { get; set; }
        //    public string name { get; set; }
        //}

        //public class Usecase
        //{
        //    public string enumType { get; set; }
        //    public string name { get; set; }
        //}

        //public class Externalreseller
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Presetgroup
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Subagentgroup
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Webhook
        //{
        //    public string _class { get; set; }
        //    public int id { get; set; }
        //}

        //public class Currency
        //{
        //    public string code { get; set; }
        //    public string description { get; set; }
        //    public float markup { get; set; }
        //    public float roundingUp { get; set; }
        //    public float creditCardFee { get; set; }
        //}

    }
}
