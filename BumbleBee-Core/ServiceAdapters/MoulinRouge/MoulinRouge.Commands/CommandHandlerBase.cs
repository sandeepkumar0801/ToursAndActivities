using Logger.Contract;
using ServiceAdapters.MoulinRouge.Constants;
using ServiceAdapters.MoulinRouge.MoulinRouge.Entities;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Util;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Commands
{
    public abstract class CommandHandlerBase
    {
        #region "Properties"

        protected HttpClient HttpClient { get; set; }
        private readonly ILogger _log;

        #endregion "Properties"

        /// <summary>
        /// constructor
        /// </summary>
        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            HttpClient = new HttpClient();
            _log = log;
        }

        /// <summary>
        /// This method used to call the API asynchronously and Log the JSON Files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext">Input Parameter</param>
        /// <param name="methodType">Method Type</param>
        /// <param name="token"></param>
        public virtual async Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token)
        {
            AddRequestHeadersAndAddressToApi();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = await GetResultAsync(inputRequest);
            watch.Stop();
            _log.WriteTimer(methodType.ToString(), token, "MoulinRouge", watch.Elapsed.ToString());

            if (responseApi != null)
            {
                _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), Convert.ToString(responseApi), methodType.ToString(), token, "MoulinRouge");
                return responseApi;

            }
            else
            {
                // Handle the case when responseApi is null
                // For example, log an error or take appropriate action.
                _log.Write("inputRequest", "Response API is null", methodType.ToString(), token, "MoulinRouge");
            }
            return null;

        }

        /// <summary>
        /// This method used to call the API and Log the JSON Files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <param name="methodType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual object Execute<T>(T inputContext, MethodType methodType, string token)
        {
            AddRequestHeadersAndAddressToApi();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = GetResult(inputRequest);
            watch.Stop();
            _log.WriteTimer(methodType.ToString(), token, "MoulinRouge", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), responseApi.ToString(), methodType.ToString(), token, "MoulinRouge");
            return responseApi;
        }

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token, out string request, out string response)
        {
            request = "";
            response = "";
            AddRequestHeadersAndAddressToApi();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = GetResult(inputRequest);
            watch.Stop();
            request = SerializeXml(inputRequest);
            response = responseApi.ToString();
            _log.WriteTimer(methodType.ToString(), token, "MoulinRouge", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), responseApi.ToString(), methodType.ToString(), token, "MoulinRouge");
            return responseApi;
        }

        /// <summary>
        /// Get result async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected abstract Task<object> GetResultAsync<T>(T inputContext);

        /// <summary>
        /// Get result async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected abstract object GetResult<T>(T inputContext);

        /// <summary>
        /// Create input object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected abstract object CreateInputRequest<T>(T inputContext);

        #region Other Supporting Methods

        /// <summary>
        /// Convert object to xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        protected string SerializeXml<T>(T requestObject)
        {
            if (requestObject == null)
                throw new ArgumentNullException();
            var serializer = new XmlSerializer(requestObject.GetType());
            var memoryStream = new MemoryStream();
            var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            var utf8Encoding = new UTF8Encoding();

            var xmlnsEmpty = new XmlSerializerNamespaces();
            xmlnsEmpty.Add("", "");

            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.Indentation = 4;

            serializer.Serialize(xmlTextWriter, requestObject, xmlnsEmpty);
            var newMemStream = (MemoryStream)xmlTextWriter.BaseStream;
            var xmlString = utf8Encoding.GetString(newMemStream.ToArray());
            return xmlString;
        }

        /// <summary>
        /// This method adds the headers and Bsae address to Http Client.
        /// </summary>
        private void AddRequestHeadersAndAddressToApi()
        {
            if (HttpClient.BaseAddress != null) return;
            var serviceUrl = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.MoulinRougeServiceUrl));
            HttpClient.Timeout = TimeSpan.FromMinutes(3);
            HttpClient.BaseAddress = new Uri(serviceUrl);
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation(Constant.Token, Constant.TokenValue);
        }

        #endregion Other Supporting Methods
    }
}