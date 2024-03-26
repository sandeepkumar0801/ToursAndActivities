using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Logger.Contract;
using ServiceAdapters.GlobalTixV3.Constants;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using Util;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands
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
            var res = await GetResultsAsync(input, inputContext.AuthToken, ((ActivityInfoInputContext)inputContext).isNonThailandProduct);
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

        

       

       

        protected abstract Task<object> GetResultsAsync(object input, string authString, bool isNonThailandProduct);

       

        #region Synchronous methods

        public virtual object Execute(InputContext inputContext, string token, out string request,
            out string response, out HttpStatusCode httpStatusCode)
        {
            var isNonThailandProduct= inputContext==null?true:((BookInputContextV3)inputContext).isNonThailandProduct;
             var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext);
            var requestData = SerializeDeSerializeHelper.Serialize(input);

            var resHttpResponse = GetResults(input, inputContext.AuthToken, isNonThailandProduct);
            var resHttp = resHttpResponse as HttpResponseMessage;
            var res= resHttp.Content.ReadAsStringAsync()?.GetAwaiter().GetResult() ?? null;
            request = "isNonThailandProduct:" + isNonThailandProduct + " RequestDetails=" +SerializeDeSerializeHelper.Serialize(resHttp?.RequestMessage) 
                     + " RequestBody="+ requestData;
            watch.Stop();
            response = res.ToString();
            if (_isSaveInStorage)
            {
                //GetHttpRequestHeaders(isNonThailandProduct);
                _log.WriteTimer(inputContext.MethodType.ToString(), token, _apiName, watch.Elapsed.ToString());
                _log.Write(SerializeDeSerializeHelper.Serialize(request), res.ToString(), inputContext.MethodType.ToString(), token, _apiName);
            }
            httpStatusCode = resHttp.StatusCode;
            return response;
        }

        public virtual object Execute(InputContext inputContext, string token)
        {
            var isNonThailandProduct = inputContext==null?true:((ActivityInfoInputContext)inputContext).isNonThailandProduct;
            var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext);
            var requestData = SerializeDeSerializeHelper.Serialize(input);
            var resHttpResponse =  GetResults(input, (inputContext != null) ? inputContext.AuthToken : string.Empty,
                isNonThailandProduct);
            var resHttp = resHttpResponse as HttpResponseMessage;
            var res = resHttp.Content.ReadAsStringAsync()?.GetAwaiter().GetResult() ?? null;
            var request = "isNonThailandProduct:" + isNonThailandProduct + " RequestDetails=" + SerializeDeSerializeHelper.Serialize(resHttp?.RequestMessage)
                     + " RequestBody=" + requestData;
            
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
                                
                                var methodType = inputContext.MethodType.ToString();
                                var watchElapsedTime = watch.Elapsed.ToString();
                                Task.Run(() => _log.WriteTimer(methodType, token, _apiName, watchElapsedTime));
                                Task.Run(() => _log.Write(request, response, methodType, token, _apiName));
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
            var res = GetResults(input, (inputContext != null) ? inputContext.AuthToken : string.Empty,
                isNonThailandProduct);
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
                                _log.Write(SerializeDeSerializeHelper.Serialize(input), res.ToString(), inputContext.MethodType.ToString(), token, _apiName);
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

        
        protected abstract object GetResults(object input, string authString,bool isNonThailandProduct);

       

        protected IDictionary<string, string> GetHttpRequestHeaders(bool isNonThailandProduct)
        {
            IDictionary<string, string> httpHeaders = new Dictionary<string, string>();
            httpHeaders.Add(Constant.HttpHeader_AcceptVersion, Constant.HttpHeader_AcceptVersion_Val);
           
            httpHeaders.Add(Constant.HttpHeader_Authorization, AgentAuthenticateDetails.Instance(isNonThailandProduct).GetHttpAuthorizationHeader());
           
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

        

        #endregion Synchronous methods
    }
}

