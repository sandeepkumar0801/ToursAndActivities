using Logger.Contract;
using Newtonsoft.Json.Linq;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Entities;
using System.Net;
using System.Security.Authentication;
using System.ServiceModel;
using Util;

namespace ServiceAdapters.FareHarbor.FareHarbor.Commands
{
    public class CommandHandlerBase
    {
        #region "Properties"

        public MethodType Handles { get; set; }

        protected HttpClient HttpClient { get; set; }

        private readonly ILogger _log;
        private readonly string _apiName;

        public OperationContextScope OperationContextScope { get; set; }

        #endregion "Properties"

        //Constructor
        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            HttpClient = new HttpClient();
            _log = log;
            _apiName = Constant.ApiNameFareHarbor;
        }

        /// <summary>
        /// This method adds the headers and Base address to Http Client.
        /// </summary>
        protected virtual void AddRequestHeadersAndAddressToApi<T>(T inputContext)
        {
            var httpClientHandler = new HttpClientHandler();

            // Set TLS versions for .NET Core 6.0
            httpClientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            HttpClient = new HttpClient(httpClientHandler);
            // SET TLS version for Framework 4.5
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            if (HttpClient.BaseAddress == null)
            {
                HttpClient.Timeout = TimeSpan.FromMinutes(3);
                HttpClient.BaseAddress = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborUri));
                HttpClient.DefaultRequestHeaders.Add(Constant.XFareHarborApiApp, ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborAppKey));
                HttpClient.DefaultRequestHeaders.Add(Constant.XFareHarborApiUser, ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborUserKey));
            }
        }

        /// <summary>
        /// This method used to call the API and Log the JSON Files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext">Input Parameter</param>
        /// <param name="methodType">Method Type</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual object Execute<T>(T inputContext, MethodType methodType, string token)
        {
            var responseText = string.Empty;
            var requestText = string.Empty;
            var responseApi = Execute(inputContext, methodType, token, out requestText, out responseText);
            return responseApi;
        }

        /// <summary>
        /// This method used to call the API and Log the JSON Files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext">Input Parameter</param>
        /// <param name="methodType">Method Type</param>
        /// <param name="token"></param>
        /// <param name="requestText"></param>
        /// <param name="responseText"></param>
        /// <returns></returns>
        public virtual object Execute<T>(T inputContext, MethodType methodType, string token, out string requestText, out string responseText)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            AddRequestHeadersAndAddressToApi(inputContext);
            var responseApi = FareHarborApiRequest(inputRequest);
            responseText = string.Empty;
            requestText = string.Empty;
            watch.Stop();
            if (methodType == MethodType.Availabilities)
            {
                inputRequest = string.Empty;
            }
            responseApi = RetryOnConflict(inputRequest, responseApi, methodType, token, out requestText, out responseText);
            _log.WriteTimer(methodType.ToString(), token, _apiName, watch.Elapsed.ToString());
            return responseApi;
        }

        /// <summary>
        /// This method used to call the API asynchronously and Log the JSON Files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext">Input Parameter</param>
        /// <param name="methodType">Method Type</param>
        /// <param name="token"></param>
        public virtual async Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            AddRequestHeadersAndAddressToApi(inputContext);
            var responseApi = await FareHarborApiRequestAsync(inputRequest);
            var responseText = string.Empty;
            var requestText = string.Empty;
            watch.Stop();
            responseApi = RetryOnConflict(inputRequest, responseApi, methodType, token, out requestText, out responseText);
            _log.WriteTimer(methodType.ToString(), token, _apiName, watch.Elapsed.ToString());
            return responseApi;
        }

        protected virtual object FareHarborApiRequest<T>(T inputContext)
        {
            return null;
        }

        protected virtual async Task<object> FareHarborApiRequestAsync<T>(T inputContext)
        {
            return await Task.FromResult<object>(null);
        }

        /// <summary>
        /// Remove Properties from the Booking object before it is passed to the api
        /// </summary>
        /// <param name="jSon">Request json string</param>
        /// /// <param name="isReBooking"></param>
        /// <returns>Json string without removed properties</returns>
        public virtual string RemoverProperties(string jSon, bool isReBooking)
        {
            var jsonJObject = JObject.Parse(jSon);
            try
            {
                jsonJObject.Descendants()
                        .OfType<JProperty>()
                        .Where(attr => attr.Name.StartsWith(Constant.ShortName) || attr.Name.StartsWith(Constant.AvailabilityId) || attr.Name.StartsWith(Constant.UuId) || (isReBooking && attr.Name.StartsWith(Constant.ReBooking)) || attr.Name.StartsWith(Constant.UserKey) || attr.Name.StartsWith(Constant.Lodging))
                        .ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
                        .ForEach(attr => attr.Remove()); // removing unwanted attributes
                jSon = jsonJObject.ToString(); // backing result to json
            }
            catch (Exception ex)
            {
                _log.Error(new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = nameof(CommandHandlerBase),
                    MethodName = nameof(RemoverProperties),
                }, ex);
            }
            return jSon;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected virtual object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        private object RetryOnConflict(object inputRequest, object responseApi, MethodType methodType, string token, out string requestText, out string responseText)
        {
            //RequestMessage
            requestText = SerializeDeSerializeHelper.Serialize(inputRequest);
            responseText = string.Empty;
            var mtd = $"{methodType.ToString()}";
            var httpResponseMessage = responseApi as HttpResponseMessage;
            var statusCode = httpResponseMessage?.StatusCode ?? HttpStatusCode.NotFound;

            if (httpResponseMessage != null && statusCode == HttpStatusCode.Conflict)
            {
                for (int i = 1; i <= 3 && statusCode == HttpStatusCode.Conflict; i++)
                {
                    try
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        mtd = $"{mtd}-Attempt {i}";
                        var timeElapsed = watch.Elapsed.ToString();
                        responseApi = FareHarborApiRequest(inputRequest);
                        watch.Stop();
                        httpResponseMessage = responseApi as HttpResponseMessage;
                        statusCode = httpResponseMessage?.StatusCode ?? HttpStatusCode.NotFound;
                        responseText = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();

                        var req = $"StatusCode:{statusCode}\r\nRequestMessage: {httpResponseMessage?.RequestMessage}\r\nRequestBody: {requestText}";
                        var res = responseText;
                        Task.Run(() => _log.WriteTimer(mtd, token, _apiName, timeElapsed));
                        Task.Run(() => _log.Write(req, res, mtd, token, _apiName));

                        if (statusCode == HttpStatusCode.Conflict)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(new Isango.Entities.IsangoErrorEntity
                        {
                            ClassName = nameof(CommandHandlerBase),
                            MethodName = nameof(RetryOnConflict),
                            Token = token,
                            Params = requestText,
                        }, ex);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(requestText))
                {
                    requestText = $"StatusCode:{statusCode}\r\nRequestMessage: {httpResponseMessage?.RequestMessage}\r\nRequestBody: {requestText}";
                }
                else
                {
                    requestText = $"StatusCode:{statusCode}\r\nRequestMessage: {httpResponseMessage?.RequestMessage}";
                }

             
                responseText = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                var req = requestText;
                var res = responseText;
                Task.Run(() => _log.Write(req, res, mtd, token, _apiName));
            }

            return responseText;
        }
    }
}