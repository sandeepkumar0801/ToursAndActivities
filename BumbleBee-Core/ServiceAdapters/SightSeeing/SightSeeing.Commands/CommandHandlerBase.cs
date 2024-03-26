using Logger.Contract;
using ServiceAdapters.SightSeeing.Constants;
using ServiceAdapters.SightSeeing.SightSeeing.Entities;
using System.Net;
using Util;

namespace ServiceAdapters.SightSeeing.SightSeeing.Commands
{
    public abstract class CommandHandlerBase
    {
        #region "Properties"

        public MethodType Handles { get; set; }

        protected HttpClient HttpClient { get; set; }

        private readonly ILogger _log;

        #endregion "Properties"

        //Constructor
        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            HttpClient = new HttpClient();
            _log = log;
        }

        /// <summary>
        /// This method adds the headers and Base address to Http Client.
        /// </summary>
        private void AddRequestHeadersAndAddressToApi()
        {
            // SET TLS version for Framework 4.5
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            if (HttpClient.BaseAddress == null)
            {
                HttpClient.Timeout = TimeSpan.FromMinutes(3);
                HttpClient.BaseAddress = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SightSeeingApi));
            }
        }

        /// <summary>
        /// This method used to call the API and Log the JSON Files.
        /// </summary>
        /// <param name="inputContext">Input Parameter</param>
        /// <param name="methodType">Method Type</param>
        /// <param name="token"></param>
        /// <param name="requestXml"></param>
        /// <param name="responseXml"></param>
        /// <returns></returns>
        public string Execute(InputContext inputContext, MethodType methodType, string token, out string requestXml, out string responseXml)
        {
            AddRequestHeadersAndAddressToApi();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = SightSeeingApiRequest(inputRequest);
            watch.Stop();
            requestXml = SerializeDeSerializeHelper.Serialize(inputRequest);
            responseXml = SerializeDeSerializeHelper.Serialize(responseApi);
            _log.WriteTimer(methodType.ToString(), token, "SightSeeing", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), responseApi, methodType.ToString(), token, "SightSeeing");
            return responseApi;
        }

        public string Execute(InputContext inputContext, MethodType methodType, string token)
        {
            AddRequestHeadersAndAddressToApi();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = SightSeeingApiRequest(inputRequest);
            watch.Stop();
            _log.WriteTimer(methodType.ToString(), token, "SightSeeing", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), responseApi, methodType.ToString(), token, "SightSeeing");
            return responseApi;
        }

        /// <summary>
        /// This method used to call the API asynchronously and Log the JSON Files.
        /// </summary>
        /// <param name="inputContext">Input Parameter</param>
        /// <param name="methodType">Method Type</param>
        /// <param name="token"></param>
        public async Task<string> ExecuteAsync(InputContext inputContext, MethodType methodType, string token)
        {
            AddRequestHeadersAndAddressToApi();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = await SightSeeingApiRequestAsync(inputRequest);
            watch.Stop();
            _log.WriteTimer(methodType.ToString(), token, "SightSeeing", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), responseApi, methodType.ToString(), token, "SightSeeing");
            return responseApi;
        }

        protected abstract string SightSeeingApiRequest<T>(T inputContext);

        protected abstract Task<string> SightSeeingApiRequestAsync<T>(T inputContext);

        protected abstract object CreateInputRequest(InputContext inputContext);
    }
}