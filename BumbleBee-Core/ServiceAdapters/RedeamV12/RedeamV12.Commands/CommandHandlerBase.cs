using Isango.Entities;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.RedeamV12.RedeamV12.Commands.Contracts;
using ServiceAdapters.RedeamV12.RedeamV12.Entities;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Util;

using Constant = ServiceAdapters.RedeamV12.Constants.Constant;

namespace ServiceAdapters.RedeamV12.RedeamV12.Commands
{
    public class CommandHandlerBase : ICommandHandlerBase
    {
        #region Properties

        private readonly ILogger _log;
        protected HttpClient HttpClient { get; set; }
        protected string BaseAddress { get; set; }

        #endregion Properties

        public CommandHandlerBase(ILogger log)
        {
            _log = log;
            BaseAddress = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.BaseAddressV12);
            HttpClient = new HttpClient();
            var redeamTimeout = Convert.ToDouble(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.RedeamTimeout));

            if (redeamTimeout == 0)
                redeamTimeout = 2;
            try
            {
                HttpClient.Timeout = TimeSpan.FromMinutes(redeamTimeout);
            }
            catch (Exception ex)
            {
                //ignore
            }
        }

        #region Public Methods

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token)
        {
            AddRequestHeadersAndBaseAddress();
            var watch = Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = (Tuple<string, string, HttpStatusCode>)RedeamApiRequest(inputRequest);
            if (inputRequest.GetType() == typeof(CanocalizationCriteria))
            {
                inputRequest = null;
            }
            var inputRequestObject = CreateInputRequestObject(responseApi, inputRequest);
            watch.Stop();
            var apiResponseMessage = responseApi.Item1;
            var logData = new LogData()
            {
                MethodType = methodType,
                Token = token,
                InputRequest = inputRequestObject,
                ResponseApi = apiResponseMessage,
                ApiType = Convert.ToString(APIType.RedeamV12),
                TimeElapsed = watch.Elapsed
            };
            //logging data here
            WriteLogDataAsync(logData);
            return CheckIfErrorOccurredWhileApiHit(apiResponseMessage) ? null : apiResponseMessage;
        }

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token,
            out string request, out string response, out HttpStatusCode httpStatusCode)
        {
            AddRequestHeadersAndBaseAddress();
            var watch = Stopwatch.StartNew();
            request = "";
            response = "";
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = (Tuple<string, string, HttpStatusCode>)RedeamApiRequest(inputRequest);
            var inputRequestObject = CreateInputRequestObject(responseApi, inputRequest);
            watch.Stop();
            var apiResponseMessage = responseApi.Item1;

            request = SerializeDeSerializeHelper.Serialize(inputRequest);
            response = apiResponseMessage.ToString();

            var logData = new LogData()
            {
                MethodType = methodType,
                Token = token,
                InputRequest = inputRequestObject,
                ResponseApi = apiResponseMessage,
                ApiType = Convert.ToString(APIType.RedeamV12),
                TimeElapsed = watch.Elapsed
            };
            //logging data here
            WriteLogDataAsync(logData);
            httpStatusCode = responseApi.Item3;
            
            if (!apiResponseMessage.Contains("error"))
            {
                return apiResponseMessage;
            }
            return null;
        }

        public virtual async Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token)
        {
            AddRequestHeadersAndBaseAddress();
            var watch = Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = (Tuple<string, string,HttpStatusCode>)await RedeamApiRequestAsync(inputRequest);
            var inputRequestObject = CreateInputRequestObject(responseApi, inputRequest);
            watch.Stop();
            var apiResponseMessage = responseApi.Item1;
            var logData = new LogData()
            {
                MethodType = methodType,
                Token = token,
                InputRequest = inputRequestObject,
                ResponseApi = apiResponseMessage,
                ApiType = Convert.ToString(APIType.RedeamV12),
                TimeElapsed = watch.Elapsed
            };
            //logging data here
            await WriteLogDataAsync(logData);

            return CheckIfErrorOccurredWhileApiHit(apiResponseMessage) ? null : apiResponseMessage;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        protected virtual object RedeamApiRequest<T>(T inputContext)
        {
            return null;
        }

        protected virtual async Task<object> RedeamApiRequestAsync<T>(T inputContext)
        {
            return await Task.FromResult<object>(null);
        }

        protected object ValidateApiResponse(HttpResponseMessage result)
        {
            // Adding the valid status codes that can be returned from the API response, add here if any new status code arrives
            var validStatusCodes = new List<HttpStatusCode>
            {
                HttpStatusCode.OK,
                HttpStatusCode.Accepted,
                HttpStatusCode.Created,
                HttpStatusCode.NoContent
            };

            var statusCode = result.StatusCode;
            var apiResponse = $"{result?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult()}";

            //if (!validStatusCodes.Contains(statusCode))
            //{
            //    apiResponse = $"{Constant.ErrorWhileAPIHit} : {result?.StatusCode.ToString()} \n-----------------------------------\n\n<Request Message> -----------------------------------\n{result?.RequestMessage} \n<Response Message> -----------------------------------\n {result?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult()}";
            //}
            return new Tuple<string, string,HttpStatusCode>(apiResponse, result?.RequestMessage?.ToString(), statusCode);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// This method adds base address and request headers to Http Client.
        /// </summary>
        private void AddRequestHeadersAndBaseAddress()
        {
            if (HttpClient.BaseAddress != null) return;

            var apiKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApiKeyV12);
            var apiSecretKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApiSecretKeyV12);

            HttpClient.BaseAddress = new Uri(BaseAddress);
            HttpClient.DefaultRequestHeaders.Add(Constant.ApiKeyHeader, apiKey);
            HttpClient.DefaultRequestHeaders.Add(Constant.ApiSecretKeyHeader, apiSecretKey);
        }

        private bool CheckIfErrorOccurredWhileApiHit(object responseApi)
        {
            return Convert.ToString(responseApi).Contains(Constant.ErrorWhileAPIHit);
        }

        private async Task WriteLogDataAsync(LogData logData)
        {
            WriteLogData(logData);

            //var task = Task.Run(() =>
            //{
            //    WriteLogData(logData);
            //});
            //await task;
        }

        private void WriteLogData(LogData logData)
        {
            _log.WriteTimer(logData.MethodType.ToString(), logData.Token, Convert.ToString(APIType.RedeamV12), logData.TimeElapsed.ToString());
            _log.Write(logData.InputRequest.ToString(), logData.ResponseApi.ToString(), logData.MethodType.ToString(), logData.Token, Convert.ToString(APIType.RedeamV12));
        }

        /// <summary>
        /// This method create input request containing API URL/Header and request object
        /// </summary>
        /// <param name="responseApi"></param>
        /// <param name="inputRequest"></param>
        /// <returns></returns>
        private string CreateInputRequestObject(Tuple<string, string, HttpStatusCode> responseApi, object inputRequest)
        {
            if (inputRequest == null)
            {
                var inputRequestObject = $"<API Url and Headers> ------------\n {responseApi.Item2}\n";
                return inputRequestObject;
            }
            else
            {
                var inputRequestObject = $"<API Url and Headers> ------------\n {responseApi.Item2}\n<Request Object> --------\n {SerializeDeSerializeHelper.Serialize(inputRequest)}";
                return inputRequestObject;
            }
        }

        #endregion Private Methods
    }
}