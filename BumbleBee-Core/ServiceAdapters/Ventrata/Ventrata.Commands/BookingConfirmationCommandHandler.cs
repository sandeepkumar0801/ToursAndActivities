using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Commands.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Entities;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using Util;
using ConstantsForVentrata = ServiceAdapters.Ventrata.Constants.Constants;
using RequestVentrata = ServiceAdapters.Ventrata.Ventrata.Entities.Request;

namespace ServiceAdapters.Ventrata.Ventrata.Commands
{
    public class BookingConfirmationCommandHandler : CommandHandlerBase, IBookingConfirmationCommandHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly RequestVentrata.BookingConfirmationReq _bookingConfirmationReq;
        private readonly string _isTestMode;

        public BookingConfirmationCommandHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();            
            _bookingConfirmationReq = new RequestVentrata.BookingConfirmationReq();
            _isTestMode = ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.IsRequestInTestMode);
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            _bookingConfirmationReq.ResellerReference = inputContext.ResellerReference;
            _bookingConfirmationReq.Uuid = inputContext.Uuid;
            _bookingConfirmationReq.ContactDetails = new RequestVentrata.Contact() {
                EmailAddress = inputContext.ContactDetails.EmailAddress,
                FullName = inputContext.ContactDetails.FullName,
                PhoneNo = inputContext.ContactDetails.PhoneNo
            };

            //Create headers required for this call
            Dictionary<string, string> HeadersToAddToRequest = new Dictionary<string, string>();
            HeadersToAddToRequest.Add(ConstantsForVentrata.AuthorizationHeaderKey, ConstantsForVentrata.Bearer + " " + inputContext.SupplierBearerToken);
            HeadersToAddToRequest.Add(ConstantsForVentrata.TestModeKey, _isTestMode);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoEnvKey, _isTestMode.ToLower() == "true" ? ConstantsForVentrata.OctoTestKey : ConstantsForVentrata.OctoLiveKey);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, ConstantsForVentrata.OctoContentKey + ',' + ConstantsForVentrata.OctoPricingKey + ',' + ConstantsForVentrata.OctoPickupsKey + ',' + ConstantsForVentrata.OctoQuestionsKey + ',' + ConstantsForVentrata.OctoPackagesKey/* + ',' + ConstantsForVentrata.OctoOffersKey*/);

            var jsonRequest = SerializeDeSerializeHelper.Serialize(_bookingConfirmationReq);
            return new Tuple<string, object,string>(jsonRequest, HeadersToAddToRequest, inputContext.VentrataBaseURL);
        }

        private string FormUrlForBookingConfirmation(string uuid)
        {
            return $"{Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.VentrataAPIUrl))}{ConstantsForVentrata.BookingReservation}/{uuid}/{ConstantsForVentrata.Confirm}";
        }
        private string FormUrlForBookingConfirmation(string baseURL,string uuid)
        {
            return baseURL+ConstantsForVentrata.BookingReservation+"/"+uuid+"/"+ConstantsForVentrata.Confirm;
        }
        protected override object GetJsonResults(object requestObject, string token, string uuid)
        {
            var response = new HttpResponseMessage();
            var inputTupleOfJsonStringAndHeaders = requestObject as Tuple<string, object,string>;
            //TODO Replace method with our own method suitable for all calls for Ventrata. For this particular one, discuss how to add path parameters
            //Create AsyncClient serviceUrl here "https://api.ventrata.com/octo/bookings/:uuid/confirm"

            //Set Dynamic value from database;
            var baseURLGet = inputTupleOfJsonStringAndHeaders?.Item3;
            if (!string.IsNullOrEmpty(baseURLGet))
            {
                _asyncClient.ServiceURL = FormUrlForBookingConfirmation(baseURLGet, uuid);
            }
            else
            {
              _asyncClient.ServiceURL = FormUrlForBookingConfirmation(uuid);
            }
            var httpResponse = _asyncClient.PostJsonWithHeadersAsync(inputTupleOfJsonStringAndHeaders.Item1, (Dictionary<string, string>)inputTupleOfJsonStringAndHeaders.Item2);
            if (httpResponse != null)
            {
                response = httpResponse.GetAwaiter().GetResult();
                return response;
            }
            return response;
        }

        protected override object GetResults(object jsonResult)
        {
            return SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<BookingConfirmationRes>(jsonResult.ToString());
        }
    }
}
