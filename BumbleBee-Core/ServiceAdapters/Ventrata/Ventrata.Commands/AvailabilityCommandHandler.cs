using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Commands.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Entities;
using ServiceAdapters.Ventrata.Ventrata.Entities.Request;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using Util;
using ConstantsForVentrata = ServiceAdapters.Ventrata.Constants.Constants;

namespace ServiceAdapters.Ventrata.Ventrata.Commands
{
    public class AvailabilityCommandHandler : CommandHandlerBase, IAvailabilityCommandHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly AvailabilityReq _availablityReq;
        private readonly string _isTestMode;


        public AvailabilityCommandHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient() {
                ServiceURL = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.VentrataAPIUrl)) + ConstantsForVentrata.Availability
            };
            _availablityReq = new AvailabilityReq();
            _isTestMode = ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.IsRequestInTestMode);
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            _availablityReq.ProductId = inputContext.ProductId.Trim();
            _availablityReq.OptionId = inputContext.OptionCode;
            _availablityReq.CheckinDate = DateTime.Parse(inputContext.CheckInDate).ToString(ConstantsForVentrata.DateFormat);
            _availablityReq.CheckoutDate = DateTime.Parse(inputContext.CheckOutDate).ToString(ConstantsForVentrata.DateFormat);
            _availablityReq.PassengerDetails = new List<Entities.Request.PassengerDetails>();
            foreach (var thisPassenger in inputContext.PassengerDetails) {
                var thisPassengerDetails = new Entities.Request.PassengerDetails
                {
                    PassengerId = thisPassenger.PassengerType,
                    Quantity = thisPassenger.Quantity
                };
                _availablityReq.PassengerDetails.Add(thisPassengerDetails);
            }

            //Create headers required for this call
            Dictionary<string, string> HeadersToAddToRequest = new Dictionary<string, string>();
            HeadersToAddToRequest.Add(ConstantsForVentrata.AuthorizationHeaderKey, ConstantsForVentrata.Bearer + " " + inputContext.SupplierBearerToken);
            HeadersToAddToRequest.Add(ConstantsForVentrata.TestModeKey, _isTestMode);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoEnvKey, _isTestMode.ToLower() == "true" ? ConstantsForVentrata.OctoTestKey : ConstantsForVentrata.OctoLiveKey);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, ConstantsForVentrata.OctoContentKey + ',' + ConstantsForVentrata.OctoPricingKey + ',' + ConstantsForVentrata.OctoPickupsKey + ',' + ConstantsForVentrata.OctoQuestionsKey + ',' + ConstantsForVentrata.OctoPackagesKey/* + ',' + ConstantsForVentrata.OctoOffersKey*/);
            //HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, );
            //HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, );
            //HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, );
            //HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, );
            //HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, );

            var jsonRequest = SerializeDeSerializeHelper.Serialize(_availablityReq);
            return new Tuple<string, object, string>(jsonRequest, HeadersToAddToRequest ,inputContext.VentrataBaseURL);
        }

        protected override object GetJsonResults(object requestObject, string token, string uuid)
        {
            var response = new HttpResponseMessage();
            var inputTupleOfJsonStringAndHeaders = requestObject as Tuple<string, object,string>;
            //Set Dynamic value from database;
            var baseURLGet = inputTupleOfJsonStringAndHeaders?.Item3;
            if (!string.IsNullOrEmpty(baseURLGet))
            {
                _asyncClient.ServiceURL = baseURLGet+ ConstantsForVentrata.Availability;
            }
            var httpResponse = _asyncClient.PostJsonWithHeadersAsync(inputTupleOfJsonStringAndHeaders.Item1, (Dictionary<string, string>)inputTupleOfJsonStringAndHeaders.Item2);
            //httpResponse.Wait();
            if (httpResponse != null)
            {
                response = httpResponse.GetAwaiter().GetResult();
                //var responseString = httpResponse.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return response;
            }
            return response;
        }

        protected override object GetResults(object jsonResult)
        {
            return SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<List<AvailabilityRes>>(jsonResult.ToString());
        }
    }
}
