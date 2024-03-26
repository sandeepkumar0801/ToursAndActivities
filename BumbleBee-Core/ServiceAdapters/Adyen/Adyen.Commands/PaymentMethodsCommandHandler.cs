using Logger.Contract;
using ServiceAdapters.Adyen.Adyen.Commands.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using ServiceAdapters.Adyen.Constants;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Commands
{
    public class PaymentMethodsCommandHandler : CommandHandlerBase, IPaymentMethodsCommandHandler
    {
        public PaymentMethodsCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(AdyenCriteria apexxCriteria, int adyenMerchantType = 1)
        {
            string blockData = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenBlockPaymentMethods);
            string assignBlockData = !string.IsNullOrEmpty(blockData) ? blockData : string.Empty;
            var PaymentMethodRequest = new PaymentMethodRequest()
            {
                CountryCode = apexxCriteria.CountryCode,// "NL",
                MerchantAccount = GetAdyenMerchantAccount(adyenMerchantType),
                ShopperLocale = apexxCriteria.ShopperLocale, //"nl-NL",
                Amount = new PaymentMethodAmount()
                {
                    Currency = apexxCriteria.Currency, //"USD",
                    Value = Convert.ToDecimal(apexxCriteria.Amount)//100
                }
            };
            if (!string.IsNullOrEmpty(assignBlockData))
            {
                string[] block = assignBlockData.Split(',');
                if (block != null && block.Count() > 0)
                {
                    PaymentMethodRequest.BlockedPaymentMethods = block;
                }
            };
            return PaymentMethodRequest;
        }

        protected override object AdyenApiRequest(object inputJson, string token)
        {
            var asyncClient = new AsyncClient();
            var ServiceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenBaseUrl)}{Constant.AdyenPaymentMethodsUrl}";
            
            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostAdyenJsonAsync(inputString, ServiceUrl);

            return response;
        }
    }
}