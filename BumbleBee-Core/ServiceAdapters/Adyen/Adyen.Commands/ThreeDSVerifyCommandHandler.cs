using Logger.Contract;
using ServiceAdapters.Adyen.Adyen.Commands.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using ServiceAdapters.Adyen.Constants;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Commands
{
    public class ThreeDsVerifyCommandHandler : CommandHandlerBase, IThreeDSVerifyCommandHandler
    {
        public ThreeDsVerifyCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(AdyenCriteria adyenCriteria, int adyenMerchantType = 1)
        {
            var threeDsVerifyRequest = new ThreeDVerifyRequest();
            if (string.IsNullOrEmpty(adyenCriteria.fingerprint)
                && string.IsNullOrEmpty(adyenCriteria.Challenge) &&
                string.IsNullOrEmpty(adyenCriteria.FacilitatorAccessToken))
            {
                threeDsVerifyRequest.PaymentData = adyenCriteria.PaymentData;
                threeDsVerifyRequest.Details = new Dictionary<string, string>
                    {
                        { "MD", adyenCriteria.MD },
                        {"PaRes", adyenCriteria.Pares}
                    };
                
            }
            else if (!string.IsNullOrEmpty(adyenCriteria.fingerprint)&&
               (adyenCriteria.fingerprint=="sofort") || (adyenCriteria.fingerprint == "ideal"))
              {
                threeDsVerifyRequest.PaymentData = adyenCriteria.PaymentData;
                threeDsVerifyRequest.Details = new Dictionary<string, string>
                    {
                        {"redirectResult", adyenCriteria.Pares}
                    };
            }
            else if (!string.IsNullOrEmpty(adyenCriteria.fingerprint) && string.IsNullOrEmpty(adyenCriteria.MD) && string.IsNullOrEmpty(adyenCriteria.Challenge))
            {
                threeDsVerifyRequest.PaymentData = adyenCriteria.PaymentData;
                threeDsVerifyRequest.Details = new Dictionary<string, string>
                {
                    { "threeds2.fingerprint", adyenCriteria.fingerprint }
                };
            }
            else if (!string.IsNullOrEmpty(adyenCriteria.Challenge))
            {
                threeDsVerifyRequest.PaymentData = adyenCriteria.PaymentData;
                threeDsVerifyRequest.Details = new Dictionary<string, string>
                {
                    { "threeds2.challengeResult", adyenCriteria.Challenge },
                    //{ "threeds2.fingerprint", adyenCriteria.fingerprint}
                };
            }
            else if (!string.IsNullOrEmpty(adyenCriteria.FacilitatorAccessToken))
            {
                var PayPalResponse = new PaypalPaymentResponse();
                threeDsVerifyRequest.PaymentData = adyenCriteria.PaymentData;
                PayPalResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PaypalPaymentResponse>(adyenCriteria?.FacilitatorAccessToken);
                threeDsVerifyRequest.Details = new Dictionary<string, string>
                {
                    { "orderID", PayPalResponse?.OrderID },
                    { "payerID", PayPalResponse?.PayerID },
                    { "paymentID", PayPalResponse?.PaymentID },
                    { "billingToken", PayPalResponse?.BillingToken },
                    { "facilitatorAccessToken", PayPalResponse?.FacilitatorAccessToken }
                };
            }
            else
            {
                threeDsVerifyRequest.PaymentData = adyenCriteria.PaymentData;
                threeDsVerifyRequest.Details = new Dictionary<string, string>
                    {
                        { "MD", adyenCriteria.MD },
                        { "PaRes", adyenCriteria.Pares},
                        //{ "threeds2.fingerprint", adyenCriteria.fingerprint}
                    };
             }

            return threeDsVerifyRequest;
        }

        protected override object AdyenApiRequest(object inputJson, string token)
        {
            var asyncClient = new AsyncClient();
            var serviceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenBaseUrl)}{Constant.AdyenPaymentDetailUrl}";

            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostAdyenJsonAsync(inputString, serviceUrl);

            return response;
        }
    }
}