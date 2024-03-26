using Isango.Entities.PrioHub;
using Logger.Contract;
using Newtonsoft.Json;
using ServiceAdapters.PrioHub.Constants;
using ServiceAdapters.PrioHub.PrioHub.Commands.Contract;
using ServiceAdapters.PrioHub.PrioHub.Entities;
using ServiceAdapters.PrioHub.PrioHub.Entities.OAuthReq;
using ServiceAdapters.PrioHub.PrioHub.Entities.OAuthRes;
using System.Diagnostics;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using Util;
using System.Linq;

namespace ServiceAdapters.PrioHub.PrioHub.Commands
{
    public abstract class CommandHandlerBase : ICommandHandlerBase
    {
        private readonly ILogger _log;
        public static string _PrioHubServiceURL;
        //public static string _PrioHubServiceURLOnlyPrioProducts;

        //public static string _PrioHubApiUser;
        public static string _PrioHubApiKey;
        //public static string _PrioHubApiDistributorId;

        //public static string _PrioHubApiUserPrioOnly;
        //public static string _PrioHubApiKeyPrioOnly;
        //public static string _PrioHubApiDistributorIdPrioOnly;

        public static string _PrioHubApiScopeProducts;
        public static string _PrioHubApiScopeReservation;
        public static string _PrioHubApiScopeBooking;

        public static string _apiName;
        protected HttpClient _httpClientOAuth { get; set; }
        protected HttpClient _httpClientOAuthPrioOnly { get; set; }
        protected HttpClient _httpClient { get; set; }

        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _log = log;
            _httpClient = new HttpClient();
            _httpClientOAuth = new HttpClient();
            _httpClientOAuthPrioOnly = new HttpClient();
        }

        static CommandHandlerBase()
        {
            _PrioHubServiceURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioBaseAddress);
            //_PrioHubServiceURLOnlyPrioProducts = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.PrioHubBaseAddressOnlyPrioProducts);

            //_PrioHubApiUser = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioApiUser);
            _PrioHubApiKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioApiKey)?.Replace("&amp;", "&");
            //_PrioHubApiDistributorId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioApiDistributorId);

            //_PrioHubApiUserPrioOnly = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioApiUserPrioOnly);
            //_PrioHubApiKeyPrioOnly = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioApiKeyPrioOnly);
            //_PrioHubApiDistributorIdPrioOnly = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioApiDistributorIdPrioOnly);

            //_PrioHubApiScopeProducts = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioApiScopeProducts);
            _PrioHubApiScopeReservation = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioApiScopeReservation);
            _PrioHubApiScopeBooking = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewPrioApiScopeBooking);
            _apiName = Constant.NewPrio;
        }

        protected abstract object CreateInputRequest<T>(T inputContext);

        protected abstract object GetResultsAsync(object inputContext);

        public virtual object Execute<T>(T inputContext, string token, MethodType methodType, out string request, out string response)
        {
            var watch = Stopwatch.StartNew();
            request = response = string.Empty;

            var requestObject = default(object);
            var responseObject = default(object);
            var httpResponseMessage = default(HttpResponseMessage);
            var requestDetails = string.Empty;
            var requestBody = string.Empty;
            try
            {
                requestObject = CreateInputRequest(inputContext);
                request = SerializeDeSerializeHelper.Serialize(requestObject);
                httpResponseMessage = GetResultsAsync(requestObject) as HttpResponseMessage;
                responseObject = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult() ?? null;
                response = Convert.ToString(responseObject);
                var errorCheck = false;

                #region Data for request log

                try
                {
                    var intStatusCode = (httpResponseMessage != null) ? (int)httpResponseMessage.StatusCode : 0;
                    if (intStatusCode >= 400 || intStatusCode == 0)
                    {
                        errorCheck = true;
                    }

                    requestDetails = SerializeDeSerializeHelper.Serialize(httpResponseMessage?.RequestMessage);
                    if (!string.IsNullOrWhiteSpace(requestDetails))
                    {
                        requestBody = httpResponseMessage?.RequestMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult() ?? request;
                        if (!string.IsNullOrWhiteSpace(requestBody))
                        {
                            var requestDetailsCreated = @"{""RequestDetails"":" + requestDetails
                                + @",""RequestBody"":" + requestBody
                                + @",""ResponseStatusCode"":" + @"""" + (int)httpResponseMessage.StatusCode + "-" + httpResponseMessage.StatusCode.ToString() + @""","
                                + @"""ResponseBody"":" + response
                                + @"}";
                            requestDetails = requestDetailsCreated;
                        }

                        request = requestDetails;
                    }
                }
                catch (Exception ex)
                {
                    request = SerializeDeSerializeHelper.Serialize(inputContext);
                    var exception = $"{ex.Message}\n{ex.StackTrace}";
                    _log.Write(request, exception, methodType.ToString(), token, _apiName);
                    requestDetails = null;
                }

                #endregion Data for request log

                watch.Stop();

                //waiting not required
#pragma warning disable 4014
                var requestText = string.IsNullOrWhiteSpace(requestDetails) ? request : requestDetails;
                var returnData = inputContext as InputContext;
                var baseAddress = _PrioHubServiceURL;

                if (string.IsNullOrWhiteSpace(requestDetails))
                {
                    if (returnData?.MethodType == MethodType.Reservation)
                    {
                        requestText = baseAddress + Constant.Reservations + "\r\n" + requestText;
                    }
                    else if (returnData?.MethodType == MethodType.CreateBooking)
                    {
                        requestText = baseAddress + Constant.Booking + "\r\n" + requestText;
                    }
                }

                var responseText = response;
                //need error response in cancel booking so append one condition into it, old code no idea
                if (methodType != MethodType.CreateBooking && methodType != MethodType.Reservation && methodType != MethodType.CancelBooking && response?.Contains("{\"error\":") == true)
                {
                    response = null;
                }
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
        /// Get headers to be added to client.
        /// </summary>
        /// <returns></returns>
        public void GetDefaultHeaders()
        {
            try
            {
                if (_httpClient.BaseAddress == null)
                {
                    _httpClient.BaseAddress = new Uri(_PrioHubServiceURL);
                }
                if (!string.IsNullOrWhiteSpace(_PrioHubApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add(Constant.HeaderApiKey, _PrioHubApiKey);
                }
                _httpClient.DefaultRequestHeaders.Add(Constant.HeaderContentType, Constant.HeaderApplicationJson);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO add logging
            }
        }

        protected IDictionary<string, string> GetHttpRequestHeaders(string accessToken)
        {
            IDictionary<string, string> httpHeaders = new Dictionary<string, string>
            {
                { Constant.HeaderApiKey, "Bearer "+accessToken},
                { Constant.Accept,"application/json"}
            };
            return httpHeaders;
        }

        public string AddRequestHeadersAndAddressToApi(string scope, int actualDistributerId = 0)
        {
            //Cache 1 hour token expiry from API response
            //On our end set:55 min. in Cache
            var memCache = MemoryCache.Default;
            var key = "defaulttokenph" + actualDistributerId;
            if (scope == _PrioHubApiScopeReservation)
            {
                key = "reservationtokenph" + actualDistributerId;
            }
            else if (scope == _PrioHubApiScopeBooking)
            {
                key = "bookingtokenph" + actualDistributerId;
            }
            var res = memCache.Get(key);
            if (res != null)
            {
                return (string)res;
            }
            else
            {
                var OAuthReq = new OAuthReq
                {
                    GrantType = "client_credentials",
                    Scope = scope
                };
                var jsonOAuthReq = JsonConvert.SerializeObject(OAuthReq);
                var dataOAuthReq = new StringContent(jsonOAuthReq, Encoding.UTF8, "application/json");

                var actualBaseAddress = _PrioHubServiceURL;
                
                if (_httpClientOAuth.BaseAddress == null)
                {
                    _httpClientOAuth.BaseAddress = new Uri(actualBaseAddress);
                }

                var getData = GetUserKeysPasswordByDistributerID(Convert.ToString(actualDistributerId));
                var prioHubUser = getData?.User;
                var prioHubKey = getData?.Password;

                var authToken = Encoding.ASCII.GetBytes(prioHubUser + ":" + prioHubKey);

                var url = _PrioHubServiceURL + Constant.Oauth2;
                var response = new HttpResponseMessage();
               
                    _httpClientOAuth.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
                    url = _PrioHubServiceURL + Constant.Oauth2;
                    response = _httpClientOAuth.PostAsync(url, dataOAuthReq)?.GetAwaiter().GetResult();
                
                var result = response.Content.ReadAsStringAsync()?.Result;
                var resultFinal = default(OAuthRes);
                resultFinal = JsonConvert.DeserializeObject<OAuthRes>(result);
                var tokenData = resultFinal?.AccessToken;
                memCache.Add(key, tokenData, DateTimeOffset.UtcNow.AddMinutes(55));
                return tokenData;
            }
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
        /// GetUserKeysPasswordByDistributerID
        /// </summary>
        /// <param name="actualDistributerId"></param>
        /// <returns></returns>
        public PrioHubUserKey GetUserKeysPasswordByDistributerID(string actualDistributerId)
        {
            var baseUrl = ConfigurationManagerHelper.GetValuefromAppSettings("WebAPIBaseUrl");
            var newPrioUserKey = new List<PrioHubUserKey>();
            if (baseUrl.ToLower().Contains("prod"))
            {
                newPrioUserKey = new List<PrioHubUserKey>
               {
                new PrioHubUserKey { User = "isango-95KH5@prioapis.com", Password = "hU@p8goaPA",DistributerID="42865",IsTestEnvironment=false},
                new PrioHubUserKey { User = "api-support_isango@prioticket.com", Password = "Isango@12345",DistributerID="2425",IsTestEnvironment=false},
                new PrioHubUserKey { User = "isango-0003AWs@prioapis.com", Password = "AGo01@I5aNg)",DistributerID="49167",IsTestEnvironment=false},
                };
            }
            else
            {
                newPrioUserKey = new List<PrioHubUserKey>
               {
                new PrioHubUserKey { User = "isango-demo@prioapi.com", Password = "2JNKv_2Ezf29kw&",DistributerID="1070569",IsTestEnvironment=true}
               };

            }

            var prioUserKey = newPrioUserKey?.Where(x => x.DistributerID == actualDistributerId)?.FirstOrDefault();
            return prioUserKey;
        }
    }
}