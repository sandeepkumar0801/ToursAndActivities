using Logger.Contract;
using ServiceAdapters.Apexx.Apexx.Commands.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using ServiceAdapters.Apexx.Constants;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Commands
{
    internal class CancelCommandHandler : CommandHandlerBase, ICancelCommandHandler
    {
        public CancelCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(ApexxCriteria apexxCriteria)
        {
            var cancelRequest = new CancelRequest
            {
                TransactionId = apexxCriteria.TransactionId
            };

            return cancelRequest;
        }

        protected override object ApexxApiRequest(object inputJson, string token)
        {
            var captureRequest = (CancelRequest)inputJson;
            var asyncClient = new AsyncClient();
            var ServiceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApexxBaseUrl)}{captureRequest.TransactionId}{Constant.CancelUrl}";

            var response = asyncClient.PostApexxJsonAsync(string.Empty, ServiceUrl);

            return response;
        }
    }
}