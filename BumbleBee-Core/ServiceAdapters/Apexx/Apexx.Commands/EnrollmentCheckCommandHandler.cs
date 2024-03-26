using Logger.Contract;
using ServiceAdapters.Apexx.Apexx.Commands.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using ServiceAdapters.Apexx.Constants;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Commands
{
    public class EnrollmentCheckCommandHandler : CommandHandlerBase, IEnrollmentCheckCommandHandler
    {
        public EnrollmentCheckCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest(ApexxCriteria apexxCriteria)
        {

            var enrollmentCheckRequest = new EnrollmentCheckRequest()
            {
                Amount = apexxCriteria.Amount,
                Currency = apexxCriteria?.Currency?.ToUpperInvariant(),
                Organisation = apexxCriteria?.Organisation,
                //Account = apexxCriteria.Account,  //Instead of Account we will use Organisation and Currency
                CaptureNow = apexxCriteria.CaptureNow,
                CustomerIp = string.Empty,//apexxCriteria.CustomerIp,
                DynamicDescriptor = apexxCriteria.DynamicDescriptor,
                MerchantReference = apexxCriteria.MerchantReference,
                UserAgent = apexxCriteria.UserAgent,
                Card = new Card()
                {
                    CardNumber = apexxCriteria.CardNumber,
                    SecurityCode = apexxCriteria.SecurityCode,
                    ExpiryYear = apexxCriteria.ExpiryYear,
                    ExpiryMonth = apexxCriteria.ExpiryMonth
                },
                ThreeDs = new ThreeDS
                {
                    ThreeDSRequired = apexxCriteria.ThreeDsRequired
                },
                BillingAddress = apexxCriteria.BillingAddress
            };
            enrollmentCheckRequest.IsOnRequestProduct = apexxCriteria.IsOnRequestProduct;
            if (apexxCriteria.IsOnRequestProduct)
            {
                enrollmentCheckRequest.RecurringType = "first";
                if (enrollmentCheckRequest.Card != null)
                {
                    enrollmentCheckRequest.Card.CreateToken = "true";
                 }
            }
            return enrollmentCheckRequest;
        }

      

        protected override object ApexxApiRequest(object inputJson, string token)
        {
            var asyncClient = new AsyncClient();
            var ServiceUrl =
                $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApexxBaseUrl)}{Constant.EnrollmentCheckUrl}";

            var inputString = SerializeDeSerializeHelper.Serialize(inputJson);
            var response = asyncClient.PostApexxJsonAsync(inputString, ServiceUrl);

            return response;
        }
    }
}