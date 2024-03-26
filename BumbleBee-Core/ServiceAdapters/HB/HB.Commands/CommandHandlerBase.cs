using Isango.Entities.HotelBeds;
using Logger.Contract;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Entities;
using ServiceAdapters.HB.HB.Entities.ActivityDetail;
using ServiceAdapters.HB.HB.Entities.Calendar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.HB.HB.Commands
{
    public abstract class CommandHandlerBase
    {
        private readonly ILogger _log;
        public static string _hotelBedsApitudeServiceURL;
        public static string _hotelBedsApiKey;
        public static string _hotelBedsApiSecret;
        public static string _apiName;
        private readonly bool _isMock;
        private readonly string _path;
        private readonly string _mockingPath;

        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _log = log;
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
            _hotelBedsApitudeServiceURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.HBApitudeServiceURL);
            _hotelBedsApiKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.HBApitudeApiKey);
            _hotelBedsApiSecret = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.HBApitudeApiSecret);
            _apiName = Constant.HBApitudeApi;
        }

        protected abstract object CreateInputRequest<T>(T inputContext);

        protected abstract Task<object> GetResultsAsync(object inputContext);

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

        public string ExternalIpAddress()
        {
            string externalip;

            //Didn't remove try-catch as it returns value
            try
            {
                externalip = new WebClient().DownloadString(Constant.Icanhazip);
            }
            catch (Exception)
            {
                return Constant.Localhost;
            }
            return externalip.Replace("\n", "");
        }

        /// <summary>
        /// Get signature for the request
        /// </summary>
        /// <param name="inputContext"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private string GetSignautre()
        {
            var signature = string.Empty;
            using (var sha = SHA256.Create())
            {
                try
                {
                    long ts = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000;
                    var computedHash = sha.ComputeHash(Encoding.UTF8.GetBytes(_hotelBedsApiKey + _hotelBedsApiSecret + ts));
                    signature = BitConverter.ToString(computedHash).Replace("-", "");
                }
                catch (Exception ex)
                {
                    //ignored
                    //#TODO add logging
                }
            }
            return signature;
        }

        /// <summary>
        /// Get headers to be added to client.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetDefaultHeaders()
        {
            var headers = new Dictionary<string, string>();
            try
            {
                var signature = GetSignautre();
                if (!string.IsNullOrWhiteSpace(signature))
                {
                    headers.Add(Constant.HeaderXSignature, signature);
                }
                if (!string.IsNullOrWhiteSpace(_hotelBedsApiKey))
                {
                    headers.Add(Constant.HeaderApiKey, _hotelBedsApiKey);
                }
                //headers.Add(Constant.HeaderContentType, Constant.HeaderApplicationJson);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO add logging
            }
            return headers;
        }

        /// <summary>
        /// Get Api response object to be passed to converter or dumping.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responseText"></param>
        /// <returns></returns>
        protected abstract object GetResponseObject(string responseText);

        /// <summary>
        /// Create Activity Detail(Simple and Full) api request object
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        public ActivityRq GetActivityDetailReqeust(HotelbedCriteriaApitude inputContext)
        {
            try
            {
                var activityRq = new ActivityRq
                {
                    Code = inputContext.ActivityCode,
                    From = inputContext.CheckinDate.ToString(Constants.Constant.DateInyyyyMMdd),
                    To = inputContext.CheckoutDate.ToString(Constants.Constant.DateInyyyyMMdd),
                    Language = inputContext.Language,
                    Paxes = new List<ServiceAdapters.HB.HB.Entities.ActivityDetail.Pax>()
                };

                foreach (var passenger in inputContext?.Ages)
                {
                    var paxCount = inputContext?.NoOfPassengers[passenger.Key];
                    for (int i = 0; i < paxCount; i++)
                    {
                        var pax = new ServiceAdapters.HB.HB.Entities.ActivityDetail.Pax
                        {
                            Age = passenger.Value,
                        };
                        activityRq.Paxes.Add(pax);
                    }
                }

                return activityRq;
            }
            catch (Exception)
            {
                //ignored
                //#TODO add logging here
            }
            return null;
        }

        /// <summary>
        /// Create Calendar api request object
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        public CalendarRq GetCriteriaCalendarRequest(HotelbedCriteriaApitudeFilter inputContext)
        {
            try
            {
                var calendarRq = new CalendarRq
                {
                    From = inputContext.CheckinDate.ToString(Constants.Constant.DateInyyyyMMdd),
                    To = inputContext.CheckoutDate.ToString(Constants.Constant.DateInyyyyMMdd),
                    Language = inputContext.Language,
                };
                calendarRq.Paxes = new List<Entities.Calendar.Pax>();

                foreach (var passenger in inputContext?.Ages)
                {
                    var paxCount = inputContext?.NoOfPassengers[passenger.Key];
                    for (int i = 0; i < paxCount; i++)
                    {
                        var pax = new ServiceAdapters.HB.HB.Entities.Calendar.Pax
                        {
                            Age = passenger.Value,
                        };
                        calendarRq.Paxes.Add(pax);
                    }
                }

                calendarRq.Filters = new List<Filter>();

                foreach (var itemFilter in inputContext.Filters)
                {
                    var searchFilterItems = new List<Searchfilteritem>();
                    foreach (var itemSearchItem in itemFilter.SearchFilterItems)
                    {
                        var searchfilteritem = new Searchfilteritem();
                        searchfilteritem.Type = itemSearchItem.Type;
                        searchfilteritem.Value = itemSearchItem.Value;
                        searchFilterItems.Add(searchfilteritem);
                    }
                    var filter = new Filter();
                    filter.SearchFilterItems = searchFilterItems;
                    calendarRq.Filters.Add(filter);
                }
                return calendarRq;
            }
            catch (Exception)
            {
                //ignored
                //#TODO add logging here
            }
            return null;
        }

        /// <summary>
        /// This methods posts the passed object to hotelbeds' end point and also sets required headers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiRequestObject">Request object for api end point</param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetResponseFromAPIEndPoint<T>(T apiRequestObject, string url, string methodType = "POST")
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
                    var header = GetDefaultHeaders();
                    var client = new AsyncClient
                    {
                        ServiceURL = url
                    };

                    response = await client.PostJsonWithHeadersAsync<T>(apiRequestObject, header, methodType);
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
            var httpResponseMessage = default(HttpResponseMessage);

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
                            if (methodType != MethodType.Bookingconfirm)
                            {
                                responseObject = GetResultsAsync(requestObject).GetAwaiter().GetResult();
                                httpResponseMessage = (HttpResponseMessage)responseObject;
                                response = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    responseObject = GetResultsAsync(requestObject).GetAwaiter().GetResult();
                    httpResponseMessage = (HttpResponseMessage)responseObject;
                    response = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                }

                if (httpResponseMessage?.IsSuccessStatusCode == true || _isMock)
                {
                    responseObject = GetResponseObject(response);
                }

                //var rq = httpResponseMessage.RequestMessage.ToString();
                //var rq1 = httpResponseMessage.ToString();

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

                do
                {
                    var watch = Stopwatch.StartNew();
                    retryCount++;

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
                                if (methodType != MethodType.Bookingconfirm)
                                {
                                    responseObject = GetResultsAsync(requestObject).GetAwaiter().GetResult();
                                    httpResponseMessage = (HttpResponseMessage)responseObject;
                                    response = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        responseObject = GetResultsAsync(requestObject).GetAwaiter().GetResult();
                        httpResponseMessage = (HttpResponseMessage)responseObject;
                        response = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                    }

                    if (httpResponseMessage.IsSuccessStatusCode || _isMock)
                    {
                        responseObject = GetResponseObject(response);
                    }

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
                } while (errorWhileSendingRequest == response.ToLower() && retryCount <= 3);
#pragma warning restore 4014
            }
            catch (Exception ex)
            {
                var exception = $"{ex.Message}\n{ex.StackTrace}";
                _log.Write(request, exception, methodType.ToString(), token, _apiName);
                responseObject = null;
            }
            return Task.FromResult<object>(responseObject);
        }
    }
}