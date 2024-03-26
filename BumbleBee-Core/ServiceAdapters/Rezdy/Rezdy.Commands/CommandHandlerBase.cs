using Isango.Entities.Rezdy;
using Logger.Contract;
using ServiceAdapters.Rezdy.Constants;
using ServiceAdapters.Rezdy.Rezdy.Commands.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities;
using System.Diagnostics;
using System.Net;

using Util;

namespace ServiceAdapters.Rezdy.Rezdy.Commands
{
    public class CommandHandlerBase : ICommandHandlerBase
    {
        #region Properties

        private readonly ILogger _log;
        protected HttpClient _httpClient { get; set; }
        protected string _baseAddress { get; set; }
        protected string _apiKey { get; set; }

        #endregion Properties

        public CommandHandlerBase(ILogger log)
        {
            _log = log;
            _baseAddress = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.RezdyBaseAddress);
            _httpClient = new HttpClient();
            _apiKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.RezdyApiKeyValue);
        }

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token)
        {
            AddBaseAddress();
            var watch = Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = (Tuple<string, string>)RezdyApiRequest(inputRequest);
            var inputRequestObject = CreateInputRequestObject(responseApi, inputRequest);
            watch.Stop();
            var apiResponseMessage = responseApi?.Item1;

            var saveInStorage = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SaveInStorage);
            if (saveInStorage.Equals(Constant.SaveInStorageValue))
            {
                _log.WriteTimer(methodType.ToString(), token, "Rezdy", watch.Elapsed.ToString());
                _log.Write(inputRequestObject.ToString(), apiResponseMessage?.ToString(), methodType.ToString(), token, "Rezdy");
            }
            return apiResponseMessage;
        }

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token, out string request, out string response)
        {
            AddBaseAddress();
            var watch = Stopwatch.StartNew();
            request = "";
            response = "";
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = (Tuple<string, string>)RezdyApiRequest(inputRequest);
            var inputRequestObject = CreateInputRequestObject(responseApi, inputRequest);
            watch.Stop();
            var apiResponseMessage = responseApi?.Item1;

            request = SerializeDeSerializeHelper.Serialize(inputRequest);
            response = responseApi?.ToString();

            var saveInStorage = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SaveInStorage);
            if (saveInStorage.Equals(Constant.SaveInStorageValue))
            {
                _log.WriteTimer(methodType.ToString(), token, "Rezdy", watch.Elapsed.ToString());
                _log.Write(inputRequestObject.ToString(), apiResponseMessage?.ToString(), methodType.ToString(), token, "Rezdy");
            }
            //return apiResponseMessage;
            return CheckIfErrorOccurredWhileApiHit(apiResponseMessage) ? null : apiResponseMessage;
        }

        private bool CheckIfErrorOccurredWhileApiHit(object responseApi)
        {
            return Convert.ToString(responseApi).Contains(Constant.ErrorWhileAPIHit);
        }

        public virtual async Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token)
        {
            AddBaseAddress();
            var watch = Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi =  (Tuple<string, string>) await RezdyApiRequestAsync(inputRequest);
            var inputRequestObject = CreateInputRequestObject(responseApi, inputRequest);
            watch.Stop();
            var apiResponseMessage = responseApi?.Item1;

            //Check if logging is on Or off.
            var saveInStorage = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SaveInStorage);
            if (saveInStorage.Equals(Constant.SaveInStorageValue))
            {
                _log.WriteTimer(methodType.ToString(), token, "Rezdy", watch.Elapsed.ToString());
                _log.Write(inputRequestObject.ToString(), apiResponseMessage?.ToString(), methodType.ToString(), token, "Rezdy");
            }
            return apiResponseMessage;
        }

        private void AddBaseAddress()
        {
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(_baseAddress);
            }
            if (!_httpClient.DefaultRequestHeaders.Contains(Constant.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add(Constant.ApiKey, _apiKey);
            }
        }

        protected virtual object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        protected virtual object CreateInputRequest<T>(T inputContext, List<RezdyProduct> rezdyProducts)
        {
            return inputContext;
        }

        protected virtual object RezdyApiRequest<T>(T inputContext)
        {
            return null;
        }

        protected virtual async Task<object> RezdyApiRequestAsync<T>(T inputContext)
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

            if (!validStatusCodes.Contains(statusCode))
            {
                apiResponse = $"{Constant.ErrorWhileAPIHit} : {result?.StatusCode.ToString()} \n-----------------------------------\n\n<Request Message> -----------------------------------\n{result?.RequestMessage} \n<Response Message> -----------------------------------\n {result?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult()}";
            }
            return new Tuple<string, string>(apiResponse, result?.RequestMessage?.ToString());
        }

        //protected object ValidateApiResponse(HttpResponseMessage result)
        //{
        //    var errorResult = string.Empty;
        //    if (result.StatusCode == HttpStatusCode.OK)
        //    {
        //        var apiResponse = $"{result.Content.ReadAsStringAsync().Result}";

        //        return new Tuple<string, string>(apiResponse, result.RequestMessage.ToString());
        //    }
        //    else
        //    {
        //        try
        //        {
        //            errorResult = $"{result.StatusCode.ToString()} \n-----------------------------------\n\n<Request Message> -----------------------------------\n{result.RequestMessage.ToString()} \n<Response Message> -----------------------------------\n {result.Content.ReadAsStringAsync().Result}";
        //        }
        //        catch
        //        {
        //            errorResult = null;
        //        }
        //        return new Tuple<string, string>(errorResult, result.RequestMessage.ToString());
        //    }
        //}

        //creating input request object with URL/Headers and request object
        private string CreateInputRequestObject(Tuple<string, string> responseApi, object inputRequest)
        {
            var inputRequestObject = $"<API Url and Headers> -----------------------------------\n {responseApi?.Item2}\n<Request Object> -----------------------------------\n {SerializeDeSerializeHelper.Serialize(inputRequest)}";

            return inputRequestObject;
        }
    }
}