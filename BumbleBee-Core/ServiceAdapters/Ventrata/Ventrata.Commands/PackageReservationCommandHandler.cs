using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Commands.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Entities;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using Util;
using ConstantsForVentrata = ServiceAdapters.Ventrata.Constants.Constants;
using RequestVentrata = ServiceAdapters.Ventrata.Ventrata.Entities.Request;

namespace ServiceAdapters.Ventrata.Ventrata.Commands
{
    public class PackageReservationCommandHandler : CommandHandlerBase, IPackageReservationCommandHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly RequestVentrata.BookingPackageReservationReq _reservationReq;
        private readonly string _isTestMode;


        public PackageReservationCommandHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient()
            {
                ServiceURL = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.VentrataAPIUrl)) + ConstantsForVentrata.BookingReservation
            };
            _reservationReq = new RequestVentrata.BookingPackageReservationReq();
            _isTestMode = ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.IsRequestInTestMode);
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            _reservationReq.ProductId = inputContext.ProductId.Trim();
            _reservationReq.OptionId = inputContext.OptionCode.Trim();
            _reservationReq.UnitItems = new List<RequestVentrata.Unititem>();
            foreach (var unitId in inputContext.UnitIdsForBooking)
            {
                var unit = new RequestVentrata.Unititem();
                unit.UnitId = unitId;
                _reservationReq.UnitItems.Add(unit);
            }
            _reservationReq.PackageBookings = new List<RequestVentrata.Packagebooking>();

            foreach (var package in inputContext.packages)
            {
                var packages = new RequestVentrata.Packagebooking();
                packages.PackageIncludeId = package.PackageIncludeId;
                packages.AvailabilityId = inputContext.AvailabilityId;
                if (packages != null)
                {
                    _reservationReq.PackageBookings.Add(packages);
                }
            }


            if (inputContext.pickUpRequested)
            {
                _reservationReq.IsPickUpRequested = inputContext.pickUpRequested;
                _reservationReq.PickUpPointId = inputContext.pickUpId;
            }
            else
            {
                _reservationReq.PickUpPointId = string.Empty;
            }

            //Create headers required for this call
            Dictionary<string, string> HeadersToAddToRequest = new Dictionary<string, string>();
            HeadersToAddToRequest.Add(ConstantsForVentrata.AuthorizationHeaderKey, ConstantsForVentrata.Bearer + " " + inputContext.SupplierBearerToken);
            HeadersToAddToRequest.Add(ConstantsForVentrata.TestModeKey, _isTestMode);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoEnvKey, _isTestMode.ToLower() == "true" ? ConstantsForVentrata.OctoTestKey : ConstantsForVentrata.OctoLiveKey);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, ConstantsForVentrata.OctoContentKey + ',' + ConstantsForVentrata.OctoPricingKey + ',' + ConstantsForVentrata.OctoPickupsKey + ',' + ConstantsForVentrata.OctoQuestionsKey + ',' + ConstantsForVentrata.OctoPackagesKey/* + ',' + ConstantsForVentrata.OctoOffersKey*/);

            var jsonRequest = SerializeDeSerializeHelper.Serialize(_reservationReq);
            return new Tuple<string, object, string>(jsonRequest, HeadersToAddToRequest,inputContext.VentrataBaseURL);
        }

        protected override object GetJsonResults(object requestObject, string token, string uuid)
        {
            var response = new HttpResponseMessage();
            var inputTupleOfJsonStringAndHeaders = requestObject as Tuple<string, object, string>;

            //Set Dynamic value from database;
            var baseURLGet = inputTupleOfJsonStringAndHeaders?.Item3;
            if (!string.IsNullOrEmpty(baseURLGet))
            {
                _asyncClient.ServiceURL = baseURLGet+ ConstantsForVentrata.BookingReservation;
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
            return SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<BookingReservationRes>(jsonResult.ToString());
        }
    }
}
