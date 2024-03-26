using Logger.Contract;
using ServiceAdapters.Adyen.Adyen.Commands.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using ServiceAdapters.Adyen.Constants;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Commands
{
    internal class CancelCommandHandler : CommandHandlerBase, ICancelCommandHandler
    {
        public CancelCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(AdyenCriteria adyenCriteria, int adyenMerchantType = 1)
        {
            var cancelRequest = new CancelRequest
            {
                MerchantAccount = GetAdyenMerchantAccount(adyenMerchantType),
                OriginalReference =adyenCriteria.PspReference,
                Reference=adyenCriteria.MerchantReference
            };

            return cancelRequest;
        }

        protected override object AdyenApiRequest(object inputJson, string token)
        {
            var captureRequest = (CancelRequest)inputJson;
            var asyncClient = new AsyncClient();
            var ServiceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenPaymentsBaseUrl)}{Constant.AdyenCancelUrl}";

            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostAdyenJsonAsync(inputString, ServiceUrl);

            return response;
        }
    }
}