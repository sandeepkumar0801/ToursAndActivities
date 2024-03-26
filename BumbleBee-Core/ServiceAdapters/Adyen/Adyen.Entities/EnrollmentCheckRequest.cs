using Newtonsoft.Json;

namespace ServiceAdapters.Adyen.Adyen.Entities
{
    public class EnrollmentCheckRequest
    {
        [JsonProperty(PropertyName = "amount")]
        public Amount Amount { get; set; }
        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }
        [JsonProperty(PropertyName = "paymentMethod")]
        public Paymentmethod PaymentMethod { get; set; }
        [JsonProperty(PropertyName = "browserInfo")]
        public Browserinfo BrowserInfo { get; set; }
        [JsonProperty(PropertyName = "returnUrl")]
        public string ReturnUrl { get; set; }
        [JsonProperty(PropertyName = "merchantAccount")]
        public string MerchantAccount { get; set; }
        [JsonProperty(PropertyName = "additionalData")]
        public AdditionalDataRequest AdditionalData { get; set; }
        [JsonProperty(PropertyName = "origin")]
        public string Origin { get; set; }
        [JsonProperty(PropertyName = "shopperEmail")]
        public string ShopperEmail { get; set; }
        [JsonProperty(PropertyName = "shopperIP")]
        public string ShopperIP { get; set; }
        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }
        //[JsonProperty(PropertyName = "billingAddress")]
        //public BillingAddress BillingAddress { get; set; }
    }

    public class Amount
    {
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }
    }

    public class Paymentmethod
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }

    public class PaymentMethodScheme : Paymentmethod
    {
        
        [JsonProperty(PropertyName = "encryptedCardNumber")]
        public string EncryptedCardNumber { get; set; }
        [JsonProperty(PropertyName = "encryptedExpiryMonth")]
        public string EncryptedExpiryMonth { get; set; }
        [JsonProperty(PropertyName = "encryptedExpiryYear")]
        public string EncryptedExpiryYear { get; set; }
        [JsonProperty(PropertyName = "encryptedSecurityCode")]
        public string EncryptedSecurityCode { get; set; }
    }

    public class PaymentMethodGPay : Paymentmethod
    {
        [JsonProperty(PropertyName = "googlePayToken")]
        public string GooglePayToken { get; set; }
    }

    public class PaymentMethodApplePay : Paymentmethod
    {
        [JsonProperty(PropertyName = "applePayToken")]
        public string ApplePayToken { get; set; }
    }
    public class PaymentMethodPayPal : Paymentmethod
    {
        [JsonProperty(PropertyName = "subtype")]
        public string Subtype { get; set; }
    }
    public class PaymentMethodSofort : Paymentmethod
    {
        [JsonProperty(PropertyName = "issuer")]
        public string Issuer { get; set; }
    
}

    public class AdditionalDataRequest
    {
        [JsonProperty(PropertyName = "authorisationType")]
        public string AuthorisationType { get; set; }
        [JsonProperty(PropertyName = "allow3DS2")]
        public bool Allow3DS2 { get; set; }
    }

    public class Browserinfo
    {
        [JsonProperty(PropertyName = "userAgent")]
        public string UserAgent { get; set; }
        [JsonProperty(PropertyName = "acceptHeader")]
        public string AcceptHeader { get; set; }
        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }
        [JsonProperty(PropertyName = "colorDepth")]
        public int ColorDepth { get; set; }
        [JsonProperty(PropertyName = "screenHeight")]
        public int ScreenHeight { get; set; }
        [JsonProperty(PropertyName = "screenWidth")]
        public int ScreenWidth { get; set; }
        [JsonProperty(PropertyName = "timeZoneOffset")]
        public int TimeZoneOffset { get; set; }
        [JsonProperty(PropertyName = "javaEnabled")]
        public bool JavaEnabled { get; set; }
    }

    //public class BillingAddress
    //{
    //    [JsonProperty(PropertyName = "houseNumberOrName")]
    //    public string HouseNumberOrName { get; set; }
    //    [JsonProperty(PropertyName = "street")]
    //    public string Street { get; set; }
    //    [JsonProperty(PropertyName = "stateOrProvince")]
    //    public string StateOrProvince { get; set; }
    //    [JsonProperty(PropertyName = "country")]
    //    public string Country { get; set; }
    //    [JsonProperty(PropertyName = "city")]
    //    public string City { get; set; }
    //    [JsonProperty(PropertyName = "postalCode")]
    //    public string PostalCode { get; set; }
    //}
}