using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Util;
using ServiceAdapters.CitySightSeeing.Constants;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands
{
    public abstract class CommandHandlerBase : ICommandHandlerBase
    {
        private readonly ILogger _log;
        public static string _cssServiceURL;
        public static string _cssApiUserName;
        public static string _cssApiPassword;
        private static string _path;
        public static string _apiName;
        protected HttpClient HttpClient { get; set; }
        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            HttpClient = new HttpClient();
            _log = log;
            //_apiName = Constant.ApiNameFareHarbor;
        }



        protected HttpClient _httpClient { get; set; }


        static CommandHandlerBase()
        {
            _cssServiceURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CssBaseAddress);
            _cssApiUserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CssApiUserName);
            _cssApiPassword = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CssApiPassword);
            _apiName = "CSS";

            try
            {
                _path = AppDomain.CurrentDomain.BaseDirectory;
            }
            catch
            {
            }
        }
        public virtual object Execute<T>(T inputContext, MethodType methodType, string token, out string requestText, out string responseText)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            AddRequestHeadersAndAddressToApi();
            var responseApi = CssApiRequest(inputRequest, token);
            responseText = string.Empty;
            requestText = string.Empty;
            watch.Stop();
            if (methodType != MethodType.CancelBooking && methodType != MethodType.redemption)
            {
                responseApi = RetryOnConflict(inputRequest, responseApi, methodType, token, out requestText, out responseText);
            }
            _log.WriteTimer(methodType.ToString(), token, _apiName, watch.Elapsed.ToString());
            return responseApi;
        }
        protected virtual object CssApiRequest<T>(T inputContext, string token)
        {
            return null;
        }

        protected virtual async Task<object> CssApiRequestAsync<T>(T inputContext)
        {
            return await Task.FromResult<object>(null);
        }


        protected virtual object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token)
        {
            var responseText = string.Empty;
            var requestText = string.Empty;
            var responseApi = Execute(inputContext, methodType, token, out requestText, out responseText);
            return responseApi;
        }
        public void AddRequestHeadersAndAddressToApi()
        {
            if (HttpClient.BaseAddress == null)
            {
                HttpClient.BaseAddress = new Uri(_cssServiceURL);

                var MyUserName = _cssApiUserName;
                var MyPassword = _cssApiPassword;

                var ByteArray = Encoding.ASCII.GetBytes(MyUserName + ":" + MyPassword);
                HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(ByteArray));
            }
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
                        responseApi = CssApiRequest(inputRequest, token);
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
                requestText = $"StatusCode:{statusCode}\r\nRequestMessage: {httpResponseMessage?.RequestMessage}\r\nRequestBody: {requestText}";
                responseText = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                var req = requestText;
                var res = responseText;
                Task.Run(() => _log.Write(req, res, mtd, token, _apiName));
            }

            return responseText;
        }

        public void AddIdempotentHeader(string idempotentKey)
        {
            //Guid idempotentGuid = Guid.NewGuid(); // Generate a new UUID
            //string idempotentKey = idempotentGuid.ToString();
            // Add "Idempotent-Key" header to the HttpClient headers
            HttpClient.DefaultRequestHeaders.Add("Idempotency-Key", idempotentKey);
        }

    }
}
