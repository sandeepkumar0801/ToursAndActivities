using Logger.Contract;
using ServiceAdapters.GoldenTours.GoldenTours.Commands.Contracts;
using ServiceAdapters.GoldenTours.GoldenTours.Entities;
using System.Diagnostics;
using System.Xml;
using Util;
using Constant = ServiceAdapters.GoldenTours.Constants.Constant;

namespace ServiceAdapters.GoldenTours.GoldenTours.Commands
{
    public class CommandHandlerBase : ICommandHandlerBase
    {
        #region Properties

        private readonly ILogger _log;
        protected HttpClient _httpClient { get; set; }
        protected string _key { get; set; }
        protected string _agentId { get; set; }
        protected string _baseAddress { get; set; }

        #endregion Properties

        public CommandHandlerBase(ILogger log)
        {
            _log = log;
            _key = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Key);
            _agentId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AgentId);
            _baseAddress = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.BaseAddress);
            _httpClient = new HttpClient();
        }

        #region Public Methods

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token)
        {
            AddBaseAddress();
            var watch = Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = GoldenToursApiRequest(inputRequest);
            watch.Stop();
            _log.WriteTimer(methodType.ToString(), token, "GoldenTours", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.SerializeXml(inputRequest), responseApi.ToString(), methodType.ToString(), token, "GoldenTours");
            return CheckIfErrorOccurredWhileApiHit(responseApi) ? null : responseApi;
        }

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token, out string request, out string response)
        {
            AddBaseAddress();
            var watch = Stopwatch.StartNew();
            request = "";
            response = "";
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = GoldenToursApiRequest(inputRequest);
            watch.Stop();
            request = SerializeDeSerializeHelper.SerializeXml(inputRequest);
            response = responseApi.ToString();
            _log.WriteTimer(methodType.ToString(), token, "GoldenTours", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.SerializeXml(inputRequest), responseApi.ToString(), methodType.ToString(), token, "GoldenTours");
            return CheckIfErrorOccurredWhileApiHit(responseApi) ? null : responseApi;
        }

        public virtual async Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token)
        {
            AddBaseAddress();
            var watch = Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = await GoldenToursApiRequestAsync(inputRequest);
            watch.Stop();
            _log.WriteTimer(methodType.ToString(), token, "GoldenTours", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.SerializeXml(inputRequest), responseApi.ToString(), methodType.ToString(), token, "GoldenTours");
            return CheckIfErrorOccurredWhileApiHit(responseApi) ? null : responseApi;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        protected virtual object GoldenToursApiRequest<T>(T inputContext)
        {
            return null;
        }

        protected virtual async Task<object> GoldenToursApiRequestAsync<T>(T inputContext)
        {
            return await Task.FromResult<object>(null);
        }

        protected object ValidateApiResponse(HttpResponseMessage result)
        {
            var apiResponse = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            string resultTagValue;
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(apiResponse);
                resultTagValue = doc.GetElementsByTagName("result")[0].InnerText;
            }
            catch
            {
                // Temporary fix for the response models not returning 'Result' field
                resultTagValue = apiResponse.Contains("<error>") ? "1" : "0";
            }

            if (resultTagValue != "0") // "0" means success
            {
                apiResponse = $"{APIHitErrors.ErrorWhileAPIHit.ToString()} : {result.StatusCode.ToString()} \n-----------------------------------\n\n<Request Message> -----------------------------------\n{result.RequestMessage} \n<Response Message> -----------------------------------\n {result.Content.ReadAsStringAsync().Result}";
            }
            return apiResponse;
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// This method adds base address to Http Client.
        /// </summary>
        private void AddBaseAddress()
        {
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.Timeout = TimeSpan.FromMinutes(3);
                _httpClient.BaseAddress = new Uri(_baseAddress);
            }
        }

        private bool CheckIfErrorOccurredWhileApiHit(object responseApi)
        {
            return Convert.ToString(responseApi).Contains(APIHitErrors.ErrorWhileAPIHit.ToString());
        }

        #endregion Private Methods
    }
}