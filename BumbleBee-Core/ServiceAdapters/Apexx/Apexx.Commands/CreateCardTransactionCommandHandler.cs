using Logger.Contract;
using ServiceAdapters.Apexx.Apexx.Commands.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using ServiceAdapters.Apexx.Constants;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Commands
{
    public class CreateCardTransactionCommandHandler : CommandHandlerBase, ICreateCardTransactionCommandHandler
    {
        public CreateCardTransactionCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(ApexxCriteria apexxCriteria)
        {
            var createCardTransactionRequest = new CreateCardTransactionRequest
            {
               Amount=Convert.ToInt32(apexxCriteria.Amount),
               CaptureNow=Convert.ToString(apexxCriteria.CaptureNow),
               MerchantReference= apexxCriteria.MerchantReference,
               RecurringType= "recurring",
               ThreeDs = new ThreeDS()
               {
                    ThreeDSRequired = "false"
               },
               Currency = apexxCriteria?.Currency?.ToUpperInvariant(),
               Organisation = apexxCriteria?.Organisation
            };
            
            var card = new Card
            {
                Token = apexxCriteria.ApexxToken
            };
            createCardTransactionRequest.Card = card;
            return createCardTransactionRequest;
        }

        protected override object ApexxApiRequest(object inputJson, string token)
        {
            var asyncClient = new AsyncClient();
            var serviceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApexxBaseUrl)}{Constant.CreateCardTransactionUrl}";

            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostApexxJsonAsync(inputString, serviceUrl);

            return response;
        }
    }
}