using Logger.Contract;
using ServiceAdapters.Adyen.Adyen.Commands.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using ServiceAdapters.Adyen.Constants;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Commands
{
    public class EnrollmentCheckCommandHandler : CommandHandlerBase, IEnrollmentCheckCommandHandler
    {
        public EnrollmentCheckCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(AdyenCriteria adyenCriteria, int adyenMerchantType = 1)
        {
            //adyenCriteria.BaseUrl = "http://localhost:62015";


            if (adyenCriteria.CardType?.ToLower()== "gpay" 
                || adyenCriteria.CardType?.ToLower() == "applepay"
                || adyenCriteria.CardType?.ToLower() == "paypal" 
                || adyenCriteria.CardType?.ToLower() == "sofort"
                || adyenCriteria.CardType?.ToLower() == "ideal")
            {
               var PaymentRequest = new EnrollmentCheckRequest()
                {
                    Origin = adyenCriteria.BaseUrl,
                    Amount = new Amount()
                    {
                        Currency = adyenCriteria.Currency,
                        Value = Convert.ToInt32(adyenCriteria.Amount)
                    },
                   MerchantAccount = GetAdyenMerchantAccount(adyenMerchantType),
                   BrowserInfo = new Browserinfo()
                    {
                        UserAgent = string.IsNullOrEmpty(adyenCriteria.Browserinfo?.UserAgent) ? @"Mozilla\/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit\/537.36 (KHTML, like Gecko) Chrome\/70.0.3538.110 Safari\/537.36" : adyenCriteria?.Browserinfo?.UserAgent,
                        AcceptHeader = adyenCriteria.Browserinfo.AcceptHeader,
                        Language = adyenCriteria.Browserinfo?.Language,//"en-US",
                        ColorDepth = adyenCriteria.Browserinfo.ColorDepth,
                        ScreenHeight = adyenCriteria.Browserinfo.ScreenHeight,
                        ScreenWidth = adyenCriteria.Browserinfo.ScreenWidth,
                        TimeZoneOffset = adyenCriteria.Browserinfo.TimeZoneOffset,
                        JavaEnabled = adyenCriteria.Browserinfo.JavaEnabled
                    },
                    
                    Reference = adyenCriteria.MerchantReference,
                    ReturnUrl = $"{adyenCriteria.BaseUrl}{ConfigurationManagerHelper.GetValuefromAppSettings("AdyenTermUrl")}",
                    AdditionalData = new AdditionalDataRequest()
                    {
                        AuthorisationType = "PreAuth",
                        Allow3DS2 = true
                    },
                    ShopperEmail = adyenCriteria.CustomerEmail,
                    ShopperIP = adyenCriteria.CustomerIp,
                    Channel = "web",//,
                   //BillingAddress = new BillingAddress()
                   //{
                   //    HouseNumberOrName = adyenCriteria.UserStreet.Split(' ').FirstOrDefault(),
                   //    Street = adyenCriteria?.UserStreet,
                   //    City = adyenCriteria?.UserCity,
                   //    Country = adyenCriteria?.UserCountry,
                   //    PostalCode = adyenCriteria?.UserPostalCode,
                   //    StateOrProvince = adyenCriteria?.UserCountry.ToLower() == "us" ? (string.IsNullOrEmpty(adyenCriteria?.UserStateOrProvince) ? "CA" : adyenCriteria?.UserStateOrProvince)
                   //                         : adyenCriteria?.UserCountry.ToLower() == "ca" ? (string.IsNullOrEmpty(adyenCriteria?.UserStateOrProvince) ? "ON" : adyenCriteria?.UserStateOrProvince)
                   //                         : string.Empty,
                   //}
               };

                if (adyenCriteria.CardType?.ToLower() == "gpay")
                {
                    PaymentRequest.PaymentMethod = new PaymentMethodGPay()
                    {
                        GooglePayToken = adyenCriteria.CardNumber,
                        Type = "paywithgoogle"
                    };
                }
                else if (adyenCriteria.CardType?.ToLower() == "applepay")
                {
                    PaymentRequest.PaymentMethod = new PaymentMethodApplePay()
                    {
                        ApplePayToken = adyenCriteria.CardNumber,
                        Type = "applepay"
                    };
                }
                else if (adyenCriteria.CardType?.ToLower() == "paypal")
                {
                    PaymentRequest.PaymentMethod = new PaymentMethodPayPal()
                    {
                        Subtype = "sdk",
                        Type = "paypal"
                    };
                }
                else if (adyenCriteria.CardType?.ToLower() == "sofort")
                {
                    PaymentRequest.PaymentMethod = new PaymentMethodSofort()
                    {
                        Type = "directEbanking"
                    };
                }
                else if (adyenCriteria.CardType?.ToLower() == "ideal")
                {
                    PaymentRequest.PaymentMethod = new PaymentMethodSofort()
                    {
                        Type = "ideal",
                        Issuer= adyenCriteria.CardNumber
                    };
                }

                return PaymentRequest;
            }
            else
            {
                var PaymentRequest = new EnrollmentCheckRequest()
                {
                    Origin = adyenCriteria.BaseUrl,
                    Amount = new Amount()
                    {
                        Currency = adyenCriteria.Currency,
                        Value = Convert.ToInt32(adyenCriteria.Amount)
                    },
                    MerchantAccount = GetAdyenMerchantAccount(adyenMerchantType),
                    BrowserInfo = new Browserinfo()
                    {
                        UserAgent = string.IsNullOrEmpty(adyenCriteria?.Browserinfo?.UserAgent) ? @"Mozilla\/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit\/537.36 (KHTML, like Gecko) Chrome\/70.0.3538.110 Safari\/537.36" : adyenCriteria?.Browserinfo?.UserAgent,
                        AcceptHeader = adyenCriteria.Browserinfo.AcceptHeader,
                        Language = adyenCriteria.Browserinfo?.Language,//"en-US",
                        ColorDepth = adyenCriteria.Browserinfo.ColorDepth,
                        ScreenHeight = adyenCriteria.Browserinfo.ScreenHeight,
                        ScreenWidth = adyenCriteria.Browserinfo.ScreenWidth,
                        TimeZoneOffset = adyenCriteria.Browserinfo.TimeZoneOffset,
                        JavaEnabled = adyenCriteria.Browserinfo.JavaEnabled
                    },
                    PaymentMethod = new PaymentMethodScheme()
                    {
                        EncryptedCardNumber = adyenCriteria.CardNumber,
                        EncryptedExpiryMonth = adyenCriteria.ExpiryMonth,
                        EncryptedExpiryYear = adyenCriteria.ExpiryYear,
                        EncryptedSecurityCode = adyenCriteria.SecurityCode,
                        Type = "scheme",
                    },
                    Reference = adyenCriteria.MerchantReference,
                    ReturnUrl = $"{adyenCriteria.BaseUrl}{ConfigurationManagerHelper.GetValuefromAppSettings("AdyenTermUrl")}",
                    AdditionalData = new AdditionalDataRequest()
                    {
                        AuthorisationType = "PreAuth",
                        Allow3DS2 = true
                    },
                    ShopperEmail = adyenCriteria.CustomerEmail,
                    ShopperIP = adyenCriteria.CustomerIp,
                    Channel = "web",//,
                    //BillingAddress = new BillingAddress()
                    //{
                    //    HouseNumberOrName = adyenCriteria?.UserStreet?.Split(' ')?.FirstOrDefault(),
                    //    Street = adyenCriteria?.UserStreet,
                    //    StateOrProvince = adyenCriteria?.UserCountry?.ToLower() == "us" ? (string.IsNullOrEmpty(adyenCriteria?.UserStateOrProvince) ? "CA" : adyenCriteria?.UserStateOrProvince)
                    //                        : adyenCriteria?.UserCountry?.ToLower() == "ca" ? (string.IsNullOrEmpty(adyenCriteria?.UserStateOrProvince) ? "ON" : adyenCriteria?.UserStateOrProvince)
                    //                        : string.Empty,
                    //    City = adyenCriteria?.UserCity,
                    //    Country = adyenCriteria?.UserCountry,
                    //    PostalCode = adyenCriteria?.UserPostalCode
                    //}
                };

                return PaymentRequest;
            }
        }

        protected override object AdyenApiRequest(object inputJson, string token)
        {
            var asyncClient = new AsyncClient();
            var ServiceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenBaseUrl)}{Constant.AdyenEnrollmentCheckUrl}";

            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostAdyenJsonAsync(inputString, ServiceUrl);

            return response;
        }
    }
}