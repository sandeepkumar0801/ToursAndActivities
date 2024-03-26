using Logger.Contract;
using ServiceAdapters.Adyen.Adyen.Commands.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using ServiceAdapters.Adyen.Constants;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Commands
{
    public class GeneratePaymentCommandHandler : CommandHandlerBase, IGeneratePaymentCommandHandler
    {
        public GeneratePaymentCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(AdyenCriteria apexxCriteria, int adyenMerchantType = 1)
        {
            var PaymentMethodRequest = new GeneratePaymentRequest()
            {
                Reference= Guid.NewGuid().ToString(),
                GeneratePaymentAmount = new GeneratePaymentAmountRQ()
                {
                    Currency = apexxCriteria.Currency, //"USD",
                    Value = Convert.ToDecimal(apexxCriteria.Amount)//100
                },
                ShopperReference ="",
                Description="",
                CountryCode= apexxCriteria.CountryCode,// "NL",
                MerchantAccount= ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenMerchantAccountCOMPAYBYLINK),
                ShopperLocale = apexxCriteria.ShopperLocale, //"nl-NL",
              };
              return PaymentMethodRequest;
        }

        protected override object AdyenApiRequest(object inputJson, string token)
        {
            var asyncClient = new AsyncClient();
            var ServiceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenBaseUrl)}{Constant.AdyenPaymentLinksUrl}";
            
            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostAdyenJsonAsync(inputString, ServiceUrl);

            return response;
        }
    }
}