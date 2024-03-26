using Logger.Contract;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System.Net.Http;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.PrioTicket.PrioTicket.Commands
{
    public abstract class CommandHandlerBase
    {
        private readonly ILogger _log;

        protected CommandHandlerBase(ILogger log)
        {
            _log = log;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="inputContext"></param>
        /// <param name="token"></param>
        /// <param name="apiLoggingToken"></param>
        /// <returns></returns>
        public virtual object Execute(InputContext inputContext, string token, string apiLoggingToken)
        {
            var requestText = string.Empty;
            var responseText = string.Empty;
            // Create Request and call Api
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var methodType = inputContext.MethodType.ToString();
                var jsonObjectTask = Task.Run(() => CreateInputRequest(inputContext)); jsonObjectTask.Wait();
                var jsonObject = jsonObjectTask.GetAwaiter().GetResult();
                var jsonResponseTask = Task.Run(() => GetJsonResults(jsonObject, token)); jsonResponseTask.Wait();
                var response = (jsonResponseTask?.GetAwaiter().GetResult()) as HttpResponseMessage;
                requestText = SerializeDeSerializeHelper.Serialize(jsonObject);
                responseText = response?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                watch.Stop();

                Task.Run(() =>
                    _log.WriteTimer(methodType, apiLoggingToken, "Prio", watch.Elapsed.ToString())
                 );

                Task.Run(() =>
                    _log.Write(requestText, responseText, methodType, apiLoggingToken, "Prio")
                );
                if (response.IsSuccessStatusCode)
                {
                    return GetResults(responseText);
                }
            }
            catch (System.Exception ex)
            {
                Task.Run(() =>
                    _log.Error(new Isango.Entities.IsangoErrorEntity
                    {
                        ClassName = "CommandHandlerBase",
                        MethodName = "Execute",
                        Params = $"inputContext : \n{inputContext}\n Request : \n{requestText}\n Response : \n{responseText}",
                    }, ex)
                );
            }

            return Task.FromResult<object>(null);
        }

        public virtual async Task<object> ExecuteAsync(InputContext inputContext, string token, string apiLoggingToken)
        {
            var requestText = string.Empty;
            var responseText = string.Empty;
            // Create Request and call Api
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var methodType = inputContext.MethodType.ToString();
                var jsonObjectTask = CreateInputRequest(inputContext);
                var jsonObject = jsonObjectTask;
                //var jsonResponseTask = await GetJsonResultsV2(jsonObject, token);
                var jsonResponseTask =  GetJsonResults(jsonObject, token);
                var response = new HttpResponseMessage();
                if (jsonResponseTask != null)
                {
                    response = (jsonResponseTask) as HttpResponseMessage;
                }
                requestText = SerializeDeSerializeHelper.Serialize(jsonObject);
                responseText = response?.Content?.ReadAsStringAsync()?.Result;
                watch.Stop();
                _log.WriteTimer(methodType, apiLoggingToken, "Prio", watch.Elapsed.ToString());
                _log.Write(requestText, responseText, methodType, apiLoggingToken, "Prio");

                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        return GetResults(responseText);
                    }
                    else
                    {
                        return responseText;

                    }
                }
            }
            catch (System.Exception ex)
            {
                _log.Error(new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = "CommandHandlerBase",
                    MethodName = "Execute",
                    Params = $"inputContext : \n{inputContext}\n Request : \n{requestText}\n Response : \n{responseText}",
                }, ex);

            }

            return responseText;
        }


        public virtual object Execute(InputContext inputContext, string token, string apiLoggingToken, out string requestXml, out string responseXml)
        {
            // Create Request and call Api
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var jsonObject = CreateInputRequest(inputContext);
            var jsonResponse = GetJsonResults(jsonObject, token);

            requestXml = jsonObject.ToString();
            var response = (HttpResponseMessage)jsonResponse;
            var responseText = response.Content.ReadAsStringAsync().Result;
            responseXml = responseText;
            var reqDetails = new { data = jsonObject, reqestDetails = response?.RequestMessage?.ToString() };
            var reqDetailsJson = jsonObject + response?.RequestMessage?.ToString();
            watch.Stop();
            Task.Run(() => _log.WriteTimer(inputContext.MethodType.ToString(), apiLoggingToken, "Prio", watch.Elapsed.ToString()));
            Task.Run(() => _log.Write(reqDetailsJson, responseText, inputContext.MethodType.ToString(), apiLoggingToken, "Prio"));
            if (response.IsSuccessStatusCode)
            {
                return GetResults(responseText);
            }

            return null;
        }

        /// <summary>
        /// Create Input Request
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected abstract object CreateInputRequest(InputContext inputContext);

        /// <summary>
        /// Get Json Results
        /// </summary>
        /// <param name="inputJson"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected abstract object GetJsonResults(object inputJson, string token);
        protected abstract Task<object> GetJsonResultsV2(object inputJson, string token);
        /// <summary>
        /// Get Results
        /// </summary>
        /// <param name="jsonResult"></param>
        /// <returns></returns>
        protected abstract object GetResults(object jsonResult);
    }
}