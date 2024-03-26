using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Entities;
using Util;
using ConstantsForVentrata = ServiceAdapters.Ventrata.Constants.Constants;

namespace ServiceAdapters.Ventrata.Ventrata.Commands
{
    public abstract class CommandHandlerBase
    {
        private readonly ILogger _log;
        private readonly string  _className;

        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _log = log;
            _className = nameof(CommandHandlerBase);
        }

        public object Execute(InputContext inputContext, string apiLoggingToken)
        {
            var requestText = string.Empty;
            var responseText = string.Empty;
            // Create Request and call Api
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var methodType = inputContext.MethodType.ToString();
                var tupleOfJsonStringAndHeaderDictTask = new Tuple<string, object,string>(string.Empty, default(object),string.Empty);


                var objectInputRequest = CreateInputRequest(inputContext);
                if (objectInputRequest != null) tupleOfJsonStringAndHeaderDictTask = objectInputRequest as Tuple<string, object,string>;

                //var jsonObject = tupleOfJsonStringAndHeaderDictTask.Item1;

                var jsonResponseTask = GetJsonResults(tupleOfJsonStringAndHeaderDictTask, apiLoggingToken, inputContext.Uuid);
                var response = new HttpResponseMessage();
                if (jsonResponseTask != null)
                {
                    response = (jsonResponseTask) as HttpResponseMessage;
                }
                requestText = (response?.RequestMessage?.ToString()+"\r\n" ?? string.Empty) + (tupleOfJsonStringAndHeaderDictTask?.Item1 ?? string.Empty);
                responseText = response?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                watch.Stop();
                _log.WriteTimer(methodType, apiLoggingToken, ConstantsForVentrata.APIName, watch.Elapsed.ToString());
                _log.Write(requestText, responseText, methodType, apiLoggingToken, ConstantsForVentrata.APIName);

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
                    ClassName = _className,
                    MethodName = nameof(Execute),
                    Params = $"inputContext : \n{inputContext}\n Request : \n{requestText}\n Response : \n{responseText}",
                }, ex);

            }

            return responseText;
        }

        public object Execute(InputContext inputContext, string apiLoggingToken, out string requestString, out string responseString)
        {
            // Create Request and call Api
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var jsonObject = CreateInputRequest(inputContext);
            var jsonResponse = GetJsonResults(jsonObject, apiLoggingToken, inputContext.Uuid);

            //requestString = jsonObject.ToString();
            if (MethodType.CancelReservationAndBooking == inputContext.MethodType)
            {
                requestString = Convert.ToString(((Tuple<string, object, string>)jsonObject).Item3 + ' ' + ((Tuple<string, object, string>)jsonObject).Item1) + SerializeDeSerializeHelper.Serialize(((Tuple<string, object, string>)jsonObject).Item2);
            }
            else
            {
                requestString = Convert.ToString(jsonObject);
            }
            var response = (HttpResponseMessage)jsonResponse;
            var responseText = response.Content.ReadAsStringAsync().Result;
            responseString = responseText;
            watch.Stop();
            Task.Run(() => _log.WriteTimer(inputContext.MethodType.ToString(), apiLoggingToken, ConstantsForVentrata.APIName, watch.Elapsed.ToString()));
            if (MethodType.CancelReservationAndBooking == inputContext.MethodType)
            {
                jsonObject = requestString.ToString();
            }
            Task.Run(() => _log.Write(SerializeDeSerializeHelper.Serialize(jsonObject), responseText, inputContext.MethodType.ToString(), apiLoggingToken, ConstantsForVentrata.APIName));
            if (response.IsSuccessStatusCode)
            {
                return GetResults(responseText);
            }

            return null;
        }

        protected abstract object GetResults(object jsonResult);

        //protected abstract Tuple<string, object> CreateInputRequest(InputContext inputContext);
        protected abstract object CreateInputRequest(InputContext inputContext);
        protected abstract object GetJsonResults(object inputTupleOfJsonStringAndHeaders, string token, string uuid);

        protected HttpClient AddRequestHeadersAndAddressToApi(string path, Dictionary<string, string> headers)
        {
            var httpClient = new HttpClient();
            if (httpClient.BaseAddress == null)
                httpClient.Timeout = TimeSpan.FromMinutes(3);
            httpClient.BaseAddress = new Uri(path);

            if (httpClient.DefaultRequestHeaders.Any())
                httpClient.DefaultRequestHeaders.Clear();

            foreach (var item in headers)
            {
                httpClient.DefaultRequestHeaders.Add(item.Key,item.Value);
            }
            return httpClient;
        }
    }
}
