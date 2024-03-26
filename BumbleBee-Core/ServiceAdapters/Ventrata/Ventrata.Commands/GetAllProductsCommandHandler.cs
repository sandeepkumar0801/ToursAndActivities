using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Commands.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Entities;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using Util;
using ConstantsForVentrata = ServiceAdapters.Ventrata.Constants.Constants;

namespace ServiceAdapters.Ventrata.Ventrata.Commands
{
    public class GetAllProductsCommandHandler : CommandHandlerBase, IGetAllProductsCommandHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly string _isTestMode;


        public GetAllProductsCommandHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient()
            {
                ServiceURL = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.VentrataAPIUrl)) + ConstantsForVentrata.Products
            };
            _isTestMode = ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.IsRequestInTestMode);
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
           //Create headers required for this call
            Dictionary<string, string> HeadersToAddToRequest = new Dictionary<string, string>();
            HeadersToAddToRequest.Add(ConstantsForVentrata.AuthorizationHeaderKey, ConstantsForVentrata.Bearer + " " + inputContext.SupplierBearerToken);
            HeadersToAddToRequest.Add(ConstantsForVentrata.TestModeKey, _isTestMode);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoEnvKey, _isTestMode.ToLower() == "true" ? ConstantsForVentrata.OctoTestKey : ConstantsForVentrata.OctoLiveKey);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, ConstantsForVentrata.OctoContentKey + ',' + ConstantsForVentrata.OctoPricingKey + ',' + ConstantsForVentrata.OctoPickupsKey + ',' + ConstantsForVentrata.OctoQuestionsKey + ',' + ConstantsForVentrata.OctoPackagesKey/* + ',' + ConstantsForVentrata.OctoOffersKey*/);
            return new Tuple<string, object,string>(string.Empty, HeadersToAddToRequest, inputContext.VentrataBaseURL);
        }

        protected override object GetJsonResults(object requestObject,
            string token, string uuid)
        {
            var response = new HttpResponseMessage();
            var inputTupleOfJsonStringAndHeaders = requestObject as Tuple<string, object,string>;
            //Set Dynamic value from database;
            var baseURLGet = inputTupleOfJsonStringAndHeaders?.Item3;
            if (!string.IsNullOrEmpty(baseURLGet))
            {
                _asyncClient.ServiceURL = baseURLGet + ConstantsForVentrata.Products;//+"?_capabilities=octo/packages";
            }
            var httpResponse = _asyncClient.PostJsonWithHeadersAsync(
                inputTupleOfJsonStringAndHeaders.Item1,
                (Dictionary<string, string>)inputTupleOfJsonStringAndHeaders.Item2, "GET");
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
            return SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<List<ProductRes>>(jsonResult.ToString());
        }
    }
}
