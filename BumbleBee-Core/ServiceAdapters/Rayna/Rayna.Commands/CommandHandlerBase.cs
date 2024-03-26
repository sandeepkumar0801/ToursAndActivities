using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contracts;
using ServiceAdapters.Rayna.Rayna.Entities;
using System.Diagnostics;
using System.Net;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public abstract class CommandHandlerBase : ICommandHandlerBase
    {
        public static string _apiName;
        private readonly ILogger _log;
        private readonly bool _isMock;
        private readonly string _path;
        private readonly string _mockingPath;
        public static string _raynaURL;
        public static string _raynaToken;
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
            _raynaURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constants.Constants.RaynaURL);
            _raynaToken = ConfigurationManagerHelper.GetValuefromAppSettings(Constants.Constants.RaynaToken);
            _apiName = Constants.Constants.RaynaApi;
        }

        public virtual object Execute<T>(T inputContext, string token, MethodType methodType, out string request, out string response)
        {
            request = response = string.Empty;
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
                            if (methodType != MethodType.Booking)
                            {
                                responseObject = GetResults(requestObject);
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
                    responseObject = GetResults(requestObject);
                    httpResponseMessage = (HttpResponseMessage)responseObject;
                    response = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                }

                if (httpResponseMessage?.IsSuccessStatusCode == true || _isMock)
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


        public virtual object Execute<T>(T inputContext, string token, MethodType methodType)
        {
            string request = "";
            string response = "";
            //out string request, out string response

            request = response = string.Empty;
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
                            if (methodType != MethodType.Booking)
                            {
                                responseObject = GetResults(requestObject);
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
                    responseObject = GetResults(requestObject);
                    httpResponseMessage = (HttpResponseMessage)responseObject;
                    response = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                }

                if (httpResponseMessage?.IsSuccessStatusCode == true || _isMock)
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

        private async Task<object> LoggingReqResAsync(string token, string apiName, string methodType, string request, string response
           , string elapsedTime
           )
        {
            try
            {
                _log.WriteTimer(methodType, token, _apiName, elapsedTime);
                _log.Write(request, response, methodType, token, _apiName);
                //Need to run logging async
                //Task.Run(() => _log.WriteTimer(methodType, token, _apiName, elapsedTime));
                //Task.Run(() => _log.Write(request, response, methodType, token, _apiName));
            }
            catch (Exception ex)
            {
                //ignored
                //Add errror Logging
            }
            return Task.FromResult<object>(null);
        }
        public HttpResponseMessage GetResponseFromAPIEndPoint<T>(T apiRequestObject, string url, string methodType = "POST")
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(Constants.Constants.RequestPrepare),
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

                    response =  client.PostJsonWithHeadersAsync<T>(apiRequestObject, header, methodType)?.GetAwaiter().GetResult();
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
        public Dictionary<string, string> GetDefaultHeaders()
        {
            var headers = new Dictionary<string, string>();
            try
            {
                if (!string.IsNullOrWhiteSpace(_raynaToken))
                {
                    headers.Add(Constants.Constants.Authorization, "Bearer "+ _raynaToken);
                }

            }
            catch (Exception ex)
            {
                //ignored
                //#TODO add logging
            }
            return headers;
        }
        protected abstract object CreateInputRequest<T>(T inputContext);
        protected abstract object GetResponseObject(string responseText);
        protected abstract object GetResults(object inputContext);

    }
}