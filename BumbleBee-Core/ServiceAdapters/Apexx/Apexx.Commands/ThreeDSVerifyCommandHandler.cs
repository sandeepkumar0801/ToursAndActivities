using Logger.Contract;
using ServiceAdapters.Apexx.Apexx.Commands.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using ServiceAdapters.Apexx.Constants;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Commands
{
    public class ThreeDsVerifyCommandHandler : CommandHandlerBase, IThreeDSVerifyCommandHandler
    {
        public ThreeDsVerifyCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(ApexxCriteria apexxCriteria)
        {
            var threeDsVerifyRequest = new ThreeDVerifyRequest
            {
                TransactionId = apexxCriteria.TransactionId,
                PaRes = apexxCriteria.Pares
            };

            return threeDsVerifyRequest;
        }

        protected override object ApexxApiRequest(object inputJson, string token)
        {
            var asyncClient = new AsyncClient();
            var serviceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApexxBaseUrl)}{Constant.ThreeDSVerifyUrl}";

            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostApexxJsonAsync(inputString, serviceUrl);

            return response;
        }
    }
}