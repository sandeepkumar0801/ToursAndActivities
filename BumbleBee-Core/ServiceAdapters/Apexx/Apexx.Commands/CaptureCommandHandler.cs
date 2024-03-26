using Logger.Contract;
using ServiceAdapters.Apexx.Apexx.Commands.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using ServiceAdapters.Apexx.Constants;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Commands
{
    public class CaptureCommandHandler : CommandHandlerBase, ICaptureCommandHandler
    {
        public CaptureCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(ApexxCriteria apexxCriteria)
        {
            var captureRequest = new CaptureRequest
            {
                Amount = apexxCriteria.Amount,
                CaptureReference = apexxCriteria.CaptureRefernce,
                TransactionId = apexxCriteria.TransactionId
            };

            return captureRequest;
        }

        protected override object ApexxApiRequest(object inputJson, string token)
        {
            var captureRequest = (CaptureRequest)inputJson;
            var asyncClient = new AsyncClient();
            var ServiceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApexxBaseUrl)}{Constant.CaputureUrl}{captureRequest.TransactionId}";

            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostApexxJsonAsync(inputString, ServiceUrl);

            return response;
        }
    }
}