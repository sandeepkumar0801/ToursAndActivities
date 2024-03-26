using Logger.Contract;
using ServiceAdapters.Apexx.Apexx.Commands.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using ServiceAdapters.Apexx.Constants;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Commands
{
    public class RefundCommandHandler : CommandHandlerBase, IRefundCommandHandler
    {
        public RefundCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(ApexxCriteria apexxCriteria)
        {
            var refundReq = new RefundRequest
            {
                TransactionId = apexxCriteria.TransactionId,
                Amount = apexxCriteria.Amount,
                Reason = apexxCriteria.Reason,
                CaptureId=apexxCriteria.CaptureGuid
            };

            return refundReq;
        }

        protected override object ApexxApiRequest(object inputJson, string token)
        {
            var refundReq = (RefundRequest)inputJson;
            var asyncClient = new AsyncClient();
            var ServiceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApexxBaseUrl)}{Constant.RefundUrl}{refundReq.TransactionId}";

            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostApexxJsonAsync(inputString, ServiceUrl);

            return response;
        }
    }
}