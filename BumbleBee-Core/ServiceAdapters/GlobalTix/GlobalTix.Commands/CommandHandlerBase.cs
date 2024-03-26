using Logger.Contract;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using System.Diagnostics;
using System.Text;
using Util;

namespace ServiceAdapters.GlobalTix.GlobalTix.Commands
{
    public abstract class CommandHandlerBase
    {
        private readonly ILogger _log;
        private readonly string _saveInStorage;
        private readonly string _apiName;
        private readonly bool _isSaveInStorage;

        #region Constructor

        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _log = log;
            _saveInStorage = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SaveInStorage);
            _isSaveInStorage = _saveInStorage.Equals(Constant.SaveInStorageValue);
            _apiName = Constant.APINameGlobalTix;
        }

        #endregion Constructor

        public virtual async Task<object> ExecuteAsync(InputContext inputContext, string token)
        {
            var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext);
            var res = await GetResultsAsync(input, inputContext.AuthToken);
            var response = (HttpResponseMessage)res;
            watch.Stop();
            if (response.IsSuccessStatusCode)
            {
                if (_isSaveInStorage)
                {
                    var hdr = new StringBuilder();
                    hdr.Append("<!--\n");
                    foreach (var header in response.Headers)
                    {
                        hdr.Append(header.Key);
                        hdr.Append(" ~ ");
                        hdr.Append("\t\t");
                        hdr.Append(((string[])(header.Value))[0]);
                        hdr.Append("\n");
                    }

                    _log.WriteTimer(inputContext.MethodType.ToString(), token, _apiName, watch.Elapsed.ToString());
                    _log.Write(SerializeDeSerializeHelper.Serialize(input), res.ToString(), inputContext.MethodType.ToString(), token, _apiName);
                }

                return response;
            }

            return null;
        }

        //public virtual async Task<object> ExecuteCancelAsync(InputContext inputContext, string token)
        //{
        //    var watch = Stopwatch.StartNew();
        //    var input = CreateInputRequest(inputContext);
        //    var res = await GetResultsAsync(input, inputContext.AuthToken);
        //    watch.Stop();
        //    var response = SerializeDeSerializeHelper.DeSerialize<CancelBookingRS>(res.ToString());
        //
        //    var saveInStorage = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SaveInStorage);
        //    if (saveInStorage.Equals(Constant.SaveInStorageValue))
        //    {
        //        _log.WriteTimer(inputContext.MethodType.ToString(), token, "GrayLineIceLand", watch.Elapsed.ToString());
        //        _log.Write(SerializeDeSerializeHelper.Serialize(input), res.ToString(), inputContext.MethodType.ToString(), token, "GrayLineIceLand");
        //    }
        //
        //    return response;
        //}

        //public virtual object ExecuteCancel(InputContext inputContext, string token)
        //{
        //    var watch = Stopwatch.StartNew();
        //    var input = CreateInputRequest(inputContext);
        //    var res = GetResults(input, inputContext.AuthToken);
        //    watch.Stop();
        //    var response = SerializeDeSerializeHelper.DeSerialize<CancelBookingRS>(res.ToString());
        //
        //    var saveInStorage = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SaveInStorage);
        //    if (saveInStorage.Equals(Constant.SaveInStorageValue))
        //    {
        //        _log.WriteTimer(inputContext.MethodType.ToString(), token, "GrayLineIceLand", watch.Elapsed.ToString());
        //        _log.Write(SerializeDeSerializeHelper.Serialize(input), res.ToString(), inputContext.MethodType.ToString(), token, "GrayLineIceLand");
        //    }
        //
        //    return response;
        //}

        //public virtual async Task<string> ExecuteStringAsync(InputContext inputContext, string token)
        //{
        //    var watch = Stopwatch.StartNew();
        //    var input = CreateInputRequest(inputContext);
        //    var res = await GetStringResultsAsync(input);
        //    watch.Stop();
        //    var saveInStorage = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SaveInStorage);
        //    if (saveInStorage.Equals(Constant.SaveInStorageValue))
        //    {
        //        _log.WriteTimer(inputContext.MethodType.ToString(), token, "GrayLineIceLand", watch.Elapsed.ToString());
        //        _log.Write(SerializeDeSerializeHelper.Serialize(input), res.ToString(), inputContext.MethodType.ToString(), token, "GrayLineIceLand");
        //    }
        //
        //    return res;
        //}

        protected abstract Task<object> GetResultsAsync(object input, string authString);

        //protected abstract Task<string> GetStringResultsAsync(object input);

        #region Synchronous methods

        public virtual object Execute(InputContext inputContext, string token, out string request, out string response)
        {
            var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext);
            //var res = GetResults(input, inputContext.AuthToken, out requestXml, out responseXml);
            request = SerializeDeSerializeHelper.Serialize(input);

            var res = GetResults(input, inputContext.AuthToken);
            watch.Stop();
            response = res.ToString();
            if (_isSaveInStorage)
            {
                _log.WriteTimer(inputContext.MethodType.ToString(), token, _apiName, watch.Elapsed.ToString());
                _log.Write(SerializeDeSerializeHelper.Serialize(input), res.ToString(), inputContext.MethodType.ToString(), token, _apiName);
            }

            return response;
        }

        public virtual object Execute(InputContext inputContext, string token)
        {
            var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext);
            //var res = GetResults(input, inputContext.AuthToken);
            var res = GetResults(input, (inputContext != null) ? inputContext.AuthToken : string.Empty);
            watch.Stop();
            if (res != null)
            {
                var response = res.ToString();

                if (_isSaveInStorage)
                {
                    if (inputContext != null)
                    {
                        if (_log != null)
                        {
                            try
                            {
                                var inputRequest = SerializeDeSerializeHelper.Serialize(inputContext);
                                var methodType = inputContext.MethodType.ToString();
                                var watchElapsedTime = watch.Elapsed.ToString();
                                Task.Run(() => _log.WriteTimer(methodType, token, _apiName, watchElapsedTime));
                                Task.Run(() => _log.Write(inputRequest, response, methodType, token, _apiName));
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }

                return response;
            }
            return null;
        }

        public virtual object Execute(InputContext inputContext, string token, bool isNonThailandProduct)
        {
            var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext, isNonThailandProduct);
            //var res = GetResults(input, inputContext.AuthToken);
            var res = GetResults(input, (inputContext != null) ? inputContext.AuthToken : string.Empty);
            watch.Stop();
            if (res != null)
            {
                var response = res.ToString();
                if (_isSaveInStorage)
                {
                    if (inputContext != null)
                    {
                        if (_log != null)
                        {
                            try
                            {
                                _log.WriteTimer(inputContext.MethodType.ToString(), token, _apiName, watch.Elapsed.ToString());
                                _log.Write(SerializeDeSerializeHelper.Serialize(inputContext), res.ToString(), inputContext.MethodType.ToString(), token, _apiName);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }

                return response;
            }
            return null;
        }

        protected abstract object CreateInputRequest(InputContext inputContext);

        protected abstract object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct);

        //protected abstract object GetResults(InputContext input, string authString);
        protected abstract object GetResults(object input, string authString);

        //protected abstract object GetResults(object input, string authString, out string requestXml,
        //    out string responseXml);

        protected IDictionary<string, string> GetHttpRequestHeaders()
        {
            IDictionary<string, string> httpHeaders = new Dictionary<string, string>();
            httpHeaders.Add(Constant.HttpHeader_AcceptVersion, Constant.HttpHeader_AcceptVersion_Val);
            //Temp code starts
            httpHeaders.Add(Constant.HttpHeader_Authorization, AgentAuthenticateDetails.Instance(true).GetHttpAuthorizationHeader());
            //Temp code ends
            return httpHeaders;
        }

        protected AsyncClient GetServiceClient(string opRelPath)
        {
            return
                new AsyncClient
                {
                    ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{opRelPath}"
                };
        }

        /*
        protected string SerializeGetQueryParameters(IDictionary<string, string> queryParams)
        {
            StringBuilder urlBuilder = new StringBuilder();
            if (queryParams?.Count > 0)
            {
                urlBuilder.Append("?");
                foreach (KeyValuePair<string, string> queryParam in queryParams)
                {
                    urlBuilder.Append(queryParam.Key + "=" + queryParam.Value + "&");
                }

                urlBuilder.Length = urlBuilder.Length - 1;
            }

            return urlBuilder.ToString();
        }
        */

        #endregion Synchronous methods
    }
}