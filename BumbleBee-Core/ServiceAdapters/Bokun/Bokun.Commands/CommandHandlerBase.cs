using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Entities;
using ServiceAdapters.Bokun.Constants;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.Bokun.Bokun.Commands
{
    public abstract class CommandHandlerBase
    {
        #region "Properties"

        protected string BokunUri;
        protected string AccessKey;
        protected string SecretKey;
        protected readonly ILogger _log;

        #endregion "Properties"

        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            var apiConfig = BokunAPIConfig.GetInstance();
            BokunUri = apiConfig.BaseUri;
            AccessKey = apiConfig.AccessKey;
            SecretKey = apiConfig.SecretKey;
            _log = log;
        }

        /// <summary>
        /// This method creates HttpClient object, adds the headers and Base address to Http Client.
        /// </summary>

        protected HttpClient AddRequestHeadersAndAddressToApi(string type, string methodPath)
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(3),
                BaseAddress = new Uri(BokunUri)
            };
            httpClient.DefaultRequestHeaders.Add(Constant.XBokunAccessKey, AccessKey);

            var xBokunDate = DateTime.Now.ToString(Constant.DateInyyyyMMddHHmmss);
            httpClient.DefaultRequestHeaders.Add(Constant.XBokunDate, xBokunDate);

            var xBokunSignature = GenerateSignature(type, methodPath, xBokunDate);
            httpClient.DefaultRequestHeaders.Add(Constant.XBokunSignature, xBokunSignature);
            return httpClient;
        }

        /// <summary>
        /// Generates X-Bokun-Signature from X-Bokun-Date, X-Bokun-AccessKey and the method path
        /// </summary>
        /// <param name="type"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        protected string GenerateSignature(string type, string url, string bokunDate)
        {
            var concatedString = $"{bokunDate}{AccessKey}{type}{url}";
            var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(SecretKey));
            var hashValue = hmac.ComputeHash(Encoding.ASCII.GetBytes(concatedString));
            var result = Convert.ToBase64String(hashValue);
            return result;
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
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = BokunApiRequest(inputRequest);
            watch.Stop();
            var response = responseApi;

            try
            {
                var errorTooManyRequests = Convert.ToString(responseApi).Contains("ErrorWhileAPIHit : 429");
                if (errorTooManyRequests)
                {
                    watch = System.Diagnostics.Stopwatch.StartNew();
                    Thread.Sleep((int)TimeSpan.FromSeconds(65).TotalMilliseconds);
                    responseApi = BokunApiRequest(inputRequest);
                    watch.Stop();
                    response = responseApi;

                }
            }
            catch (Exception ex)
            {
                //ignore
            }

            var errorOccurredWhileAPIHit =
                Convert.ToString(responseApi).Contains(APIHitErrors.ErrorWhileAPIHit.ToString());

            //if (methodType == MethodType.Submitcheckout && responseApi != null && !errorOccurredWhileAPIHit)
            //{
            //    var obj = JObject.Parse(responseApi.ToString());
            //    responseApi = obj["booking"]["confirmationCode"];
            //}

            _log.WriteTimer(methodType.ToString(), token, "Bokun", watch.Elapsed.ToString());
            //if (!ignoreLogMethodTypes.Contains(methodType.ToString()))
            try
            {
                _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), SerializeDeSerializeHelper.Serialize(responseApi), methodType.ToString(), token, "Bokun");
            }
            catch (Exception ex)
            {
                var errorResponse = errorOccurredWhileAPIHit ? responseApi?.ToString() : ex.Message + "\n" + ex.StackTrace;
                _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), errorResponse, methodType.ToString(), token, "Bokun");
            }
            return errorOccurredWhileAPIHit ? null : response;
        }

        /// <summary>
        /// This method used to call the API and Log the JSON Files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext">Input Parameter</param>
        /// <param name="methodType">Method Type</param>
        /// <param name="token"></param>
        /// <param name="apiRequestText"></param>
        /// <param name="apiResponseText"></param>
        /// <returns></returns>
        public virtual object Execute<T>(T inputContext, MethodType methodType, string token, out string apiRequestText, out string apiResponseText)
        {
            apiRequestText = string.Empty;
            apiResponseText = string.Empty;
            var responseApiObject = default(object);
            var isErrorOccurredWhileApiHit = true;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);

            if (inputRequest != null)
            {
                responseApiObject = BokunApiRequest(inputRequest);
                apiRequestText = SerializeDeSerializeHelper.Serialize(inputContext);
                apiResponseText = responseApiObject != null ? responseApiObject.ToString() : Constant.ResponseNotFound;

                if (responseApiObject == null
                        || apiResponseText.Contains(APIHitErrors.ErrorWhileAPIHit.ToString())
                        || apiResponseText == Constant.ResponseNotFound
                        || string.IsNullOrWhiteSpace(apiResponseText)
                )
                {
                    isErrorOccurredWhileApiHit = true;
                }
                else
                {
                    isErrorOccurredWhileApiHit = false;
                }
            }
            else
            {
                apiRequestText = "Request object is null.";
                apiResponseText = "No need to hit api endpoint.";
            }
            watch.Stop();
            if (methodType != MethodType.Submitcheckout)
            {
                var requestText = apiRequestText;
                var responseText = apiResponseText;
                 _log.WriteTimer(methodType.ToString(), token, "Bokun", watch.Elapsed.ToString());
                _log.Write(requestText, responseText, methodType.ToString(), token, "Bokun");
            }

            return isErrorOccurredWhileApiHit ? null : responseApiObject;
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
            var responseApi = await BokunApiRequestAsync(inputRequest);
            watch.Stop();
            var response = responseApi;
            var errorOccurredWhileAPIHit =
                Convert.ToString(responseApi).Contains(APIHitErrors.ErrorWhileAPIHit.ToString());
            //if (methodType == MethodType.Submitcheckout && responseApi != null && !errorOccurredWhileAPIHit)
            //{
            //    var obj = JObject.Parse(responseApi.ToString());
            //    responseApi = obj["booking"]["confirmationCode"];
            //}

            _log.WriteTimer(methodType.ToString(), token, "Bokun", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), SerializeDeSerializeHelper.Serialize(responseApi), methodType.ToString(), token, "Bokun");
            return errorOccurredWhileAPIHit ? null : response;
        }

        /// <summary>
        /// Create an InputRequest object
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected abstract object CreateInputRequest<T>(T inputContext);

        protected abstract object BokunApiRequest<T>(T inputContext);

        protected abstract Task<object> BokunApiRequestAsync<T>(T inputContext);

        protected object ValidateApiResponse(HttpResponseMessage httpResponseMessage)
        {
            var returnValue = default(string);
            var content = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
            if (httpResponseMessage?.StatusCode == HttpStatusCode.OK)
            {
                returnValue = content;
            }
            else
            {
                try
                {
                    var headers = httpResponseMessage?.Headers;
                    IEnumerable<string> values;
                    string session = "Not Found";
                    if (headers.TryGetValues("X-RateLimit-Limit", out values))
                    {
                        session = values.ToString();
                    }
                    returnValue = $"{APIHitErrors.ErrorWhileAPIHit.ToString()} : {httpResponseMessage.StatusCode.ToString()} \n-----------------------------------\n\n<Request Message> -----------------------------------\n{httpResponseMessage.RequestMessage} \n<Response Message> -----------------------------------\n {content}{session}";
                }
                catch (Exception ex)
                {
                    returnValue = ex.Message + "\n" + ex.StackTrace;
                }
            }
            return returnValue;
        }
    }
}