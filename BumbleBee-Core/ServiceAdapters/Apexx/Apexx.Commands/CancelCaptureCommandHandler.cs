using Logger.Contract;
using ServiceAdapters.Apexx.Apexx.Commands.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using ServiceAdapters.Apexx.Constants;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Commands
{
    internal class CancelCaptureCommandHandler : CommandHandlerBase, ICancelCaptureCommandHandler
    {
        public CancelCaptureCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(ApexxCriteria apexxCriteria)
        {
            var cancelRequest = new CancelCaptureTransactionRequest
            {
                CaptureId = apexxCriteria.TransactionId,
                CancelCaptureReference= apexxCriteria.CaptureRefernce
            };

            return cancelRequest;
        }

        protected override object ApexxApiRequest(object inputJson, string token)
        {
            var captureRequest = (CancelCaptureTransactionRequest)inputJson;
            var asyncClient = new AsyncClient();
            var ServiceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApexxBaseUrl)}{Constant.CancelCaptureUrl}{captureRequest.CaptureId}";
            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostApexxJsonAsync(inputString, ServiceUrl);

            return response;
        }
    }
}