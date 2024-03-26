using Logger.Contract;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands
{
    public abstract class CommandHandlerBase
    {
        private readonly ILogger _log;

        public CommandHandlerBase(ILogger log)
        {
            _log = log;
        }

        protected abstract object CreateInputRequest<T>(T inputContext, object requestExtraData, string token);

        public virtual object Execute(object inputContext, object requestExtraData, string endPoint, string token, MethodType methodType)
        {
            var watch = Stopwatch.StartNew();
            var request = string.Empty;
            var response = string.Empty;
            var requestObject = default(object);
            var responseObject = default(object);
            var httpResponseMessage = default(HttpResponseMessage);
            try
            {
                requestObject = CreateInputRequest(inputContext, requestExtraData, token);
                request = SerializeDeSerializeHelper.Serialize(requestObject);

                responseObject = RiskifiedPaymentApiRequest(requestObject, endPoint);
                if (responseObject != null)
                {
                    httpResponseMessage = (HttpResponseMessage)responseObject;
                    response = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                }

                watch.Stop();
                Task.Run(() =>
                LoggingReqResAsync
                    (
                        token, "RiskifiedPayment", methodType.ToString()
                        , request
                        , response
                        , watch.Elapsed.ToString()
                    )
                    .ConfigureAwait(false)
                );
            }
            catch (Exception ex)
            {
                var exception = $"{ex.Message}\n{ex.StackTrace}";
                Task.Run(() =>
                _log.Write(request, exception, methodType.ToString(), token, endPoint)
                );
            }
            return response;
        }

        public virtual async Task<object> ExecuteAsync(object inputContext, object requestExtraData, string endPoint, string token, MethodType methodType)
        {
            var watch = Stopwatch.StartNew();
            var request = string.Empty;
            var response = string.Empty;
            var requestObject = default(object);
            var responseObject = default(object);
            var httpResponseMessage = default(HttpResponseMessage);
            try
            {
                requestObject = CreateInputRequest(inputContext, requestExtraData, token);
                request = SerializeDeSerializeHelper.Serialize(requestObject);

                responseObject = await RiskifiedPaymentApiRequestAsync(requestObject, endPoint);
                httpResponseMessage = (HttpResponseMessage)responseObject;

                response = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();

                watch.Stop();
                Task.Run(() =>
                LoggingReqResAsync
                    (
                        token, "RiskifiedPayment", methodType.ToString()
                        , request
                        , response
                        , watch.Elapsed.ToString()
                    )
                    .ConfigureAwait(false)
                );
            }
            catch (Exception ex)
            {
                var exception = $"{ex.Message}\n{ex.StackTrace}";
                Task.Run(() =>
                _log.Write(request, exception, methodType.ToString(), token, endPoint)
                );
            }
            return response;
        }

        private async Task<object> LoggingReqResAsync(string token, string apiName, string methodType, string request, string response
            , string elapsedTime
            )
        {
            //Need to run logging async
            Task.Run(() => _log.WriteTimer(methodType, token, apiName, elapsedTime));
            Task.Run(() => _log.Write(request, response, methodType, token, apiName));

            return Task.FromResult<object>(null);
        }

        public HttpResponseMessage RiskifiedPaymentApiRequest<T>(T inputContext, string endpoint, string methodType = "POST")
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(Constant.Constant.RequestPrepare),
            };
            var request = SerializeDeSerializeHelper.Serialize(inputContext);
            var reuestHeaders = GetRequestHeaders(request);

            try
            {
                var url = ConfigurationManagerHelper.GetValuefromAppSettings("RiskifiedServerURL") + endpoint;
                var client = new AsyncClient
                {
                    ServiceURL = url
                };
                var httpResponse = client.PostJsonWithHeadersAsync<T>(inputContext, reuestHeaders, methodType);
                if (httpResponse != null)
                {
                    response = httpResponse.Result;
                    var data = httpResponse.GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.Message),
                };
            }
            return response;
        }

        public async Task<HttpResponseMessage> RiskifiedPaymentApiRequestAsync<T>(T inputContext, string endpoint, string methodType = "POST")
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(Constant.Constant.RequestPrepare),
            };
            var request = SerializeDeSerializeHelper.Serialize(inputContext);
            var reuestHeaders = GetRequestHeaders(request);

            try
            {
                var url = ConfigurationManagerHelper.GetValuefromAppSettings("RiskifiedServerURL") + endpoint;
                var client = new AsyncClient
                {
                    ServiceURL = url
                };
                response = await client.PostJsonWithHeadersAsync<T>(inputContext, reuestHeaders, methodType);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.Message),
                };
            }
            return response;
        }

        private Dictionary<string, string> GetRequestHeaders(string request)
        {
            Dictionary<string, string> RequestHeaders = new Dictionary<string, string>();
            RequestHeaders.Add("Accept", "application/vnd.riskified.com; version=2");
            RequestHeaders.Add("X-RISKIFIED-SHOP-DOMAIN", ConfigurationManagerHelper.GetValuefromAppSettings("RiskifiedShopDomain"));
            var shaHmac = SHA256_HMAC(request);
            RequestHeaders.Add("X-RISKIFIED-HMAC-SHA256", shaHmac);
            RequestHeaders.Add("Content-Type", "application/json");
            return RequestHeaders;
        }

        private string SHA256_HMAC(string request)
        {
            var key = Encoding.ASCII.GetBytes(ConfigurationManagerHelper.GetValuefromAppSettings("RiskifiedAuthToken"));
            var myhmacsha256 = new HMACSHA256(key);
            var byteArray = Encoding.UTF8.GetBytes(request);
            var stream = new MemoryStream(byteArray);
            var result = myhmacsha256.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
            return result;
        }

        protected string MaskCreditCard(string data)
        {
            var maskedData = string.Empty;
            if (data?.Length == 16)
            {
                var stringNotToMask = data.Substring(12);
                maskedData = $"XXXX-XXXX-XXXX-{stringNotToMask}";
            }
            return maskedData;
        }
    }
}