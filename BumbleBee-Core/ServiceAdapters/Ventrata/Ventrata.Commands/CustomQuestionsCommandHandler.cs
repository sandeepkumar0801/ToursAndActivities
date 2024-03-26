using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Commands.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Entities;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using System.Text;
using Util;
using ConstantsForVentrata = ServiceAdapters.Ventrata.Constants.Constants;

namespace ServiceAdapters.Ventrata.Ventrata.Commands
{
    public class CustomQuestionsCommandHandler : CommandHandlerBase, ICustomQuestionsCommandHandler
    {
        private readonly HttpClient _asyncClient;
        private readonly string _isTestMode;


        public CustomQuestionsCommandHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new HttpClient()
            {
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
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, "octo/questions");
            return new Tuple<string, object,string>(string.Empty, HeadersToAddToRequest, inputContext.VentrataBaseURL);
        }

        protected override object GetJsonResults(object requestObject,
            string token, string uuid)
        {
            var inputTupleOfJsonStringAndHeaders = requestObject as Tuple<string, object, string>;
            var completePathGet ="";
            var basePathOnly = "";
            if (string.IsNullOrEmpty(completePathGet))
            {
                basePathOnly = inputTupleOfJsonStringAndHeaders?.Item3 + ConstantsForVentrata.Products;
            }

            completePathGet = basePathOnly + "/" + uuid;
            using (var httpClient = AddRequestHeadersAndAddressToApi(basePathOnly, (Dictionary<string, string>)inputTupleOfJsonStringAndHeaders.Item2))
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, completePathGet);
                var response = httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();

                byte[] buf = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                string content = Encoding.UTF8.GetString(buf);
                var responseHttp = new HttpResponseMessage();
                responseHttp.Content = new StringContent(content);
                return responseHttp;
            }
        }

        protected override object GetResults(object jsonResult)
        {
            return SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<CustomQuestions>(jsonResult.ToString());
        }
    }
}
