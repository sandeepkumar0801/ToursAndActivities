using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Adyen.Adyen.Entities
{
    public class EnrollmentCheckResponse
    {
        [JsonProperty(PropertyName = "additionalData")]
        public AdditionalData AdditionalData { get; set; }

        [JsonProperty(PropertyName = "action")]
        public Action Action { get; set; }

        [JsonProperty(PropertyName = "details")]
        public List<Details> Details { get; set; }

        [JsonProperty(PropertyName = "paymentData")]
        public string PaymentData { get; set; }

        [JsonProperty(PropertyName = "redirect")]
        public Redirect Redirect { get; set; }

        [JsonProperty(PropertyName = "pspReference")]
        public string PspReference { get; set; }

        [JsonProperty(PropertyName = "resultCode")]
        public string ResultCode { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public AmountData Amount { get; set; }

        [JsonProperty(PropertyName = "merchantReference")]
        public string MerchantReference { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "errorType")]
        public string ErrorType { get; set; }

        [JsonProperty(PropertyName = "outputDetails")]
        public OutputDetails OutputDetails { get; set; }

        [JsonProperty(PropertyName = "refusalReason")]
        public string RefusalReason { get; set; }

        [JsonProperty(PropertyName = "refusalReasonCode")]
        public string RefusalReasonCode { get; set; }
    }

    public class AmountData
    {
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }
    }

    public class AdditionalData
    {
        [JsonProperty(PropertyName = "issuerCountry")]
        public string IssuerCountry { get; set; }
    }

    public class Action
    {
        [JsonProperty(PropertyName = "paymentData")]
        public string PaymentData { get; set; }

        [JsonProperty(PropertyName = "paymentMethodType")]
        public string PaymentMethodType { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }

        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
        [JsonProperty(PropertyName = "sdkData")]
        public SdkData SdkData { get; set; }
    }

    public class Data
    {
        [JsonProperty(PropertyName = "MD")]
        public string MD { get; set; }

        [JsonProperty(PropertyName = "PaReq")]
        public string PaReq { get; set; }

        [JsonProperty(PropertyName = "TermUrl")]
        public string TermUrl { get; set; }
    }

    public class Details
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }

    public class Redirect
    {
        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }

        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }

    public class SdkData
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }

    public class OutputDetails
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}