using Logger.Contract;
using ServiceAdapters.Adyen.Adyen.Commands.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using ServiceAdapters.Adyen.Constants;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Commands
{
    public class CaptureCommandHandler : CommandHandlerBase, ICaptureCommandHandler
    {
        public CaptureCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(AdyenCriteria adyenCriteria, int adyenMerchantType = 1)
        {
            var captureRequest = new CaptureRequest()
            {
                ModificationAmount = new Amount()
                {
                    Currency = adyenCriteria.Currency,
                    Value = Convert.ToInt32(adyenCriteria.Amount)
                },
                MerchantAccount = GetAdyenMerchantAccount(adyenMerchantType),
                Reference = adyenCriteria.MerchantReference,
                OriginalReference = adyenCriteria.PspReference
            };
            return captureRequest;
        }

        protected override object AdyenApiRequest(object inputJson, string token)
        {
            var asyncClient = new AsyncClient();
            var ServiceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenPaymentsBaseUrl)}{Constant.AdyenCaptureCheckUrl}";

            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostAdyenJsonAsync(inputString, ServiceUrl);

            return response;
        }
    }
}
