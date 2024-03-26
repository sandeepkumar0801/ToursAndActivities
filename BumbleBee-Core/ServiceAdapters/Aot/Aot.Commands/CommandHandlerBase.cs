using Logger.Contract;
using ServiceAdapters.Aot.Aot.Commands.Contracts;
using ServiceAdapters.Aot.Aot.Entities;
using System.Diagnostics;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Util;
using Constant = ServiceAdapters.Aot.Constants.Constant;

namespace ServiceAdapters.Aot.Aot.Commands
{
    public class CommandHandlerBase : ICommandHandlerBase
    {
        #region Properties

        public MethodType Handles { get; set; }
        protected HttpClient HttpClient { get; set; }
        public OperationContextScope OperationContextScope { get; set; }
        public string AgentId { get; set; }
        public string Password { get; set; }
        private readonly ILogger _log;

        #endregion Properties

        /// <summary>
        /// constructor
        /// </summary>
        public CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            HttpClient = new HttpClient();
            _log = log;
        }

        #region Implementation Methods

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token)
        {
            var watch = Stopwatch.StartNew();
            AddRequestHeadersAndAddressToApi();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = AotApiRequest(inputRequest);
            watch.Stop();
            _log.WriteTimer(methodType.ToString(), token, "AOT", watch.Elapsed.ToString());
            if (responseApi != null)
            {
                _log.Write(SerializeXml(inputRequest), responseApi?.ToString(), methodType.ToString(), token, "AOT");
            }
            return responseApi;
        }

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token, out string request, out string response)
        {
            var watch = Stopwatch.StartNew();
            request = "";
            response = "";
            AddRequestHeadersAndAddressToApi();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = AotApiRequest(inputRequest);
            watch.Stop();

            request = SerializeDeSerializeHelper.Serialize(inputRequest);
            response = SerializeDeSerializeHelper.Serialize(responseApi);
            _log.WriteTimer(methodType.ToString(), token, "AOT", watch.Elapsed.ToString());
            _log.Write(SerializeXml(inputRequest).Replace(",\"Age\":0", string.Empty).Replace("<Age>0</Age>", string.Empty), responseApi?.ToString(), methodType.ToString(), token, "AOT");
            return responseApi;
        }

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token, string referenceNumber, out string request, out string response)
        {
            var watch = Stopwatch.StartNew();
            request = "";
            response = "";
            AddRequestHeadersAndAddressToApi();
            var inputRequest = CreateInputRequest(inputContext, referenceNumber);
            var responseApi = AotApiRequest(inputRequest);
            watch.Stop();
            request = SerializeDeSerializeHelper.Serialize(inputRequest);
            response = SerializeDeSerializeHelper.Serialize(responseApi);

            _log.WriteTimer(methodType.ToString(), token, "AOT", watch.Elapsed.ToString());
            _log.Write(SerializeXml(inputRequest).Replace(",\"Age\":0", string.Empty).Replace("<Age>0</Age>", string.Empty), responseApi?.ToString(), methodType.ToString(), token, "AOT");
            return responseApi;
        }

        /// <summary>
        /// This method used to call the API asynchronously and Log the JSON Files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext">Input Paramter</param>
        /// <param name="methodType">Method Type</param>
        public virtual async Task<object> ExecuteAsync<T>(T inputContext, MethodType methodType, string token)
        {
            AddRequestHeadersAndAddressToApi();
            var watch = Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = await AotApiRequestAsync(inputRequest);
            watch.Stop();
            _log.WriteTimer(methodType.ToString(), token, "AOT", watch.Elapsed.ToString());
            _log.Write(SerializeXml(inputRequest), responseApi?.ToString(), methodType.ToString(), token, "AOT");
            return responseApi;
        }

        public string SerializeXml<T>(T requestObject)
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

        #endregion Implementation Methods

        #region Private Methods

        /// <summary>
        /// This method adds the headers and Bsae address to Http Client.
        /// </summary>
        private void AddRequestHeadersAndAddressToApi()
        {
            var httpClientHandler = new HttpClientHandler();

            // Set TLS versions for .NET Core 6.0
            httpClientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            HttpClient = new HttpClient(httpClientHandler);

            if (HttpClient.BaseAddress == null)
            {
                HttpClient.Timeout = TimeSpan.FromMinutes(3);
                HttpClient.BaseAddress = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AotUri));
            }
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected virtual object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected virtual object CreateInputRequest<T>(T inputContext, string referenceNumber)
        {
            return inputContext;
        }

        protected virtual object AotApiRequest<T>(T inputContext)
        {
            return null;
        }

        protected virtual async Task<object> AotApiRequestAsync<T>(T inputContext)
        {
            return await Task.FromResult<object>(null);
        }

        #endregion Protected Methods
    }
}