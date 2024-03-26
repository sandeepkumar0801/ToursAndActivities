using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Commands.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Entities;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using Util;
using ConstantsForVentrata = ServiceAdapters.Ventrata.Constants.Constants;
using RequestVentrata = ServiceAdapters.Ventrata.Ventrata.Entities.Request;

namespace ServiceAdapters.Ventrata.Ventrata.Commands
{
    public class BookingAndReservationCancellationCommandHandler : CommandHandlerBase, IBookingAndReservationCancellationCommandHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly RequestVentrata.BookingAndReservationCancellationReq _bookingAndReservationCancellationReq;
        private readonly string _isTestMode;

        public BookingAndReservationCancellationCommandHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();
            _bookingAndReservationCancellationReq = new RequestVentrata.BookingAndReservationCancellationReq();
            _isTestMode = ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.IsRequestInTestMode);
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            _bookingAndReservationCancellationReq.ReasonForCancellation = inputContext.ReasonForCancellation;

            //Create headers required for this call
            Dictionary<string, string> HeadersToAddToRequest = new Dictionary<string, string>();
            HeadersToAddToRequest.Add(ConstantsForVentrata.AuthorizationHeaderKey, ConstantsForVentrata.Bearer + " " + inputContext.SupplierBearerToken);
            HeadersToAddToRequest.Add(ConstantsForVentrata.TestModeKey, _isTestMode);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoEnvKey, _isTestMode.ToLower() == "true" ? ConstantsForVentrata.OctoTestKey : ConstantsForVentrata.OctoLiveKey);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, ConstantsForVentrata.OctoContentKey + ',' + ConstantsForVentrata.OctoPricingKey + ',' + ConstantsForVentrata.OctoPickupsKey + ',' + ConstantsForVentrata.OctoQuestionsKey + ',' + ConstantsForVentrata.OctoPackagesKey/* + ',' + ConstantsForVentrata.OctoOffersKey*/);

            var jsonRequest = SerializeDeSerializeHelper.Serialize(_bookingAndReservationCancellationReq);
            return new Tuple<string, object,string>(jsonRequest, HeadersToAddToRequest, inputContext.VentrataBaseURL);
        }

        private string FormUrlForCancellation(string uuid)
        {
            return $"{Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.VentrataAPIUrl))}{ConstantsForVentrata.BookingReservation}/{uuid}";
        }
        private string FormUrlForCancellation(string baseURL, string uuid)
        {
            return baseURL+ConstantsForVentrata.BookingReservation+"/"+uuid;
        }
        protected override object GetJsonResults(object requestObject, string token, string uuid)
        {
            var response = new HttpResponseMessage();
            var inputTupleOfJsonStringAndHeaders = requestObject as Tuple<string, object, string>;

            //Set Dynamic value from database;
            var baseURLGet = inputTupleOfJsonStringAndHeaders?.Item3;
            if (!string.IsNullOrEmpty(baseURLGet))
            {
                _asyncClient.ServiceURL = FormUrlForCancellation(baseURLGet,uuid);
            }
            else
            {
                _asyncClient.ServiceURL = FormUrlForCancellation(uuid);
            }
            var httpResponse = _asyncClient.PostJsonWithHeadersAsync(inputTupleOfJsonStringAndHeaders.Item1, (Dictionary<string, string>)inputTupleOfJsonStringAndHeaders.Item2, ConstantsForVentrata.HttpDeleteMethodType);
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
