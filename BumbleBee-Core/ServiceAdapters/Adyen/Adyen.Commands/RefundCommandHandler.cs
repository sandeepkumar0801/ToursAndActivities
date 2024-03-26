using Logger.Contract;
using ServiceAdapters.Adyen.Adyen.Commands.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using ServiceAdapters.Adyen.Constants;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Commands
{
    public class RefundCommandHandler : CommandHandlerBase, IRefundCommandHandler
    {
        public RefundCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(AdyenCriteria adyenCriteria, int adyenMerchantType = 1)
        {
            var refundReq = new RefundRequest
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

            return refundReq;
        }

        protected override object AdyenApiRequest(object inputJson, string token)
        {
            var refundReq = (RefundRequest)inputJson;
            var asyncClient = new AsyncClient();
            var ServiceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenPaymentsBaseUrl)}{Constant.AdyenRefundUrl}";

            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostAdyenJsonAsync(inputString, ServiceUrl);

            return response;
        }
    }
}