using Isango.Entities.NewCitySightSeeing;
using Logger.Contract;
using ServiceAdapters.NewCitySightSeeing.Constants;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands.Contract;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities;
using System.Diagnostics;
using System.Net;
using Util;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands
{
    public abstract class CommandHandlerBase : ICommandHandlerBase
    {
        private readonly ILogger _log;
        public static string _newCitySightSeeingServiceURL;
        public static string _newCitySightSeeingApiKey;
        public static string _apiName;
        protected HttpClient _httpClient { get; set; }

        private readonly bool _isMock;
        private readonly string _path;
        private readonly string _mockingPath;

        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _log = log;
            _httpClient = new HttpClient();
            _isMock = false;
            try
            {
                _path = AppDomain.CurrentDomain.BaseDirectory;
                _mockingPath = $"{_path}Mock-Samples\\{_apiName}";
                _isMock = Util.ConfigurationManagerHelper.GetValuefromAppSettings("isMock_MR") == "1";
            }
            catch
            {
            }
        }

        static CommandHandlerBase()
        {
            _newCitySightSeeingServiceURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewCitySightSeeingBaseAddress);
            _newCitySightSeeingApiKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewCitySightSeeingApiKey);
            _apiName = Constant.NewCitySightSeeing;
        }

        protected abstract object CreateInputRequest<T>(T inputContext);

        protected abstract object GetResultsAsync(object inputContext);

        public virtual async Task<object> ExecuteAsync<T>(T inputContext, string token, MethodType methodType)
        {
            var result = Task.Run(() => ProcessAsync(inputContext, token, methodType)).GetAwaiter().GetResult();
            return result;
        }

        public virtual object Execute<T>(T inputContext, string token, MethodType methodType, out string request, out string response)
        {
            request = response = string.Empty;
            var result = Process(inputContext, token, methodType, out request, out response);
            return result;
        }

        /// <summary>
        /// Get headers to be added to client.
        /// </summary>
        /// <returns></returns>
        public void GetDefaultHeaders()
        {
            try
            {
                if (_httpClient.BaseAddress == null)
                {
                    _httpClient.BaseAddress = new Uri(_newCitySightSeeingServiceURL);
                }
                if (!string.IsNullOrWhiteSpace(_newCitySightSeeingApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add(Constant.HeaderApiKey, _newCitySightSeeingApiKey);
                }
                _httpClient.DefaultRequestHeaders.Add(Constant.HeaderContentType, Constant.HeaderApplicationJson);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO add logging
            }
        }

        protected IDictionary<string, string> GetHttpRequestHeaders()
        {
            IDictionary<string, string> httpHeaders = new Dictionary<string, string>();
            httpHeaders.Add(Constant.HeaderApiKey, _newCitySightSeeingApiKey);
            return httpHeaders;
        }

        /// <summary>
        /// Get Api response object to be passed to converter or dumping.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responseText"></param>
        /// <returns></returns>
        protected abstract object GetResponseObject(string responseText);

        /// <summary>
        /// This methods posts the passed object to hotelbeds' end point and also sets required headers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiRequestObject">Request object for api end point</param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetResponseFromAPIEndPoint<T>(T apiRequestObject, string url)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(Constant.RequestPrepare),
            };

            try
            {
                if (apiRequestObject != null)
                {
                    GetDefaultHeaders();
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
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

        private async Task<object> LoggingReqResAsync(string token, string apiName, string methodType, string request, string response
            , string elapsedTime
            )
        {
            try
            {
                //Need to run logging async
                Task.Run(() => _log.WriteTimer(methodType, token, _apiName, elapsedTime));
                Task.Run(() => _log.Write(request, response, methodType, token, _apiName));
            }
            catch (Exception ex)
            {
                //ignored
                //Add errror Logging
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// This executes processing for execute and executeAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <param name="token"></param>
        /// <param name="methodType"></param>
        /// <returns></returns>
        private object Process<T>(T inputContext, string token, MethodType methodType, out string request, out string response)
        {
            var watch = Stopwatch.StartNew();
            request = response = string.Empty;

            var requestObject = default(object);
            var responseObject = default(object);
            //var httpResponseMessage = default(HttpResponseMessage);

            try
            {
                requestObject = CreateInputRequest(inputContext);
                request = SerializeDeSerializeHelper.Serialize(requestObject);

                if (_isMock)
                {
                    try
                    {
                        var resFilePath = $"{_mockingPath}\\{methodType.ToString()}_res.json";
                        if (System.IO.File.Exists(resFilePath))
                        {
                            responseObject = File.ReadAllText(resFilePath);
                            response = responseObject?.ToString();
                        }
                        else
                        {
                            if (methodType != MethodType.CreateBooking)
                            {
                                responseObject = GetResultsAsync(requestObject);
                                response = responseObject?.ToString();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    responseObject = GetResultsAsync(requestObject);
                    response = responseObject?.ToString();
                }
                //try
                //{
                //responseObject = GetResultsAsync(requestObject);
                //httpResponseMessage = (HttpResponseMessage)responseObject;

                //var responseBody = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                //response = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                //}
                //catch (Exception ex)
                //{
                //}
                //var pathSet = "";
                //var test1 = "1";
                //var test = "1";
                //if (test1 == "1")
                //{
                //    pathSet = @"c:\test0.json";
                //}
                //else if (test1 == "21")
                //{
                //    pathSet = @"c:\2_ProductAvailability_2.json";
                //}
                //else if (test1 == "22")
                //{
                //    pathSet = @"c:\2_ProductAvailability_3.json";
                //}
                //else if (test1 == "2")
                //{
                //    pathSet = @"c:\3_Reservation.json";
                //}
                //else if (test1 == "3")
                //{
                //    pathSet = @"c:\4_BookReservation.json";
                //}
                //else if (test1 == "4")
                //{
                //    pathSet = @"c:\5_Cancellation.json";
                //}
                ////var
                //JObject o1 = JObject.Parse(File.ReadAllText(pathSet));
                ////JObject o1 = JObject.Parse(File.ReadAllText(@"c:\2_ProductAvailability.json"));
                //// read JSON directly from a file
                //var test = string.Empty;
                //using (StreamReader file = File.OpenText(pathSet))

                //using (JsonTextReader reader = new JsonTextReader(file))
                //{
                //    JObject o2 = (JObject)JToken.ReadFrom(reader);
                //    test = o2.ToString();
                //}

                //response = test;
                watch.Stop();

                //waiting not required
#pragma warning disable 4014
                var requestText = request;
                var responseText = response;
                Task.Run(() =>
                LoggingReqResAsync
                    (
                        token, _apiName, methodType.ToString()
                        , requestText
                        , responseText
                        , watch.Elapsed.ToString()
                    )
                    .ConfigureAwait(false)
                );

#pragma warning restore 4014

                responseObject = GetResponseObject(response);
            }
            catch (Exception ex)
            {
                request = SerializeDeSerializeHelper.Serialize(inputContext);
                var exception = $"{ex.Message}\n{ex.StackTrace}";
                _log.Write(request, exception, methodType.ToString(), token, _apiName);
                responseObject = null;
            }
            return responseObject;
        }

        /// <summary>
        /// This executes processing for execute and executeAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <param name="token"></param>
        /// <param name="methodType"></param>
        /// <returns></returns>
        private Task<object> ProcessAsync<T>(T inputContext, string token, MethodType methodType)
        {
            var request = string.Empty;
            var response = string.Empty;

            var requestObject = default(object);
            var responseObject = default(object);
            var httpResponseMessage = default(HttpResponseMessage);
            var errorWhileSendingRequest = "An error occurred while sending the request.".ToLower();
            var retryCount = 0;
            try
            {
                requestObject = CreateInputRequest(inputContext);
                request = SerializeDeSerializeHelper.Serialize(requestObject);

                var watch = Stopwatch.StartNew();

                responseObject = GetResultsAsync(requestObject);

                httpResponseMessage = (HttpResponseMessage)responseObject;
                response = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                watch.Stop();

                //waiting not required
#pragma warning disable 4014
                var requestText = request;
                var responseText = response;
                Task.Run(() =>
                LoggingReqResAsync
                    (
                        token, _apiName, methodType.ToString() + " " + retryCount
                        , requestText
                        , responseText
                        , watch.Elapsed.ToString()
                    )
                    .ConfigureAwait(false)
                );

#pragma warning restore 4014

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    responseObject = GetResponseObject(response);
                }
            }
            catch (Exception ex)
            {
                var exception = $"{ex.Message}\n{ex.StackTrace}";
                _log.Write(request, exception, methodType.ToString(), token, _apiName);
                responseObject = null;
            }
            return Task.FromResult<object>(responseObject);
        }

        /// <summary>
        /// Create Activity Detail(Simple and Full) api request object
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        public NewCitySightSeeingCriteria GetProductRequest(NewCitySightSeeingCriteria inputContext)
        {
            try
            {
                var newCitySightSeeingCriteria = new NewCitySightSeeingCriteria
                {
                };
                return newCitySightSeeingCriteria;
            }
            catch (Exception)
            {
                //ignored
                //#TODO add logging here
            }
            return null;
        }
    }
}