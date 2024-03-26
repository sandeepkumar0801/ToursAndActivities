using Logger.Contract;
using ServiceAdapters.BigBus.BigBus.Entities;
using ServiceAdapters.BigBus.Constants;
using System.Text;
using Util;

namespace ServiceAdapters.BigBus.BigBus.Commands
{
    public abstract class CommandHandlerBase
    {
        #region "Properties"

        public MethodType Handles { get; set; }
        protected HttpClient HttpClient { get; set; }
        private readonly ILogger _log;

        #endregion "Properties"

        //Constructor
        public CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            HttpClient = new HttpClient();
            _log = log;
        }

        /// <summary>
        /// This method used to call the API and Log the JSON Files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext">Input Paramter</param>
        /// <param name="methodType">Method Type</param>
        /// <returns></returns>
        public virtual string Execute(InputContext inputContext, MethodType methodType, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            AddRequestHeadersAndAddressToApi();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = BigBusApiRequest(inputRequest);

            request = SerializeDeSerializeHelper.Serialize(inputRequest);
            response = SerializeDeSerializeHelper.Serialize(responseApi);
            watch.Stop();
            _log.WriteTimer(methodType.ToString(), token, "BigBus", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), responseApi, methodType.ToString(), token, "BigBus");
            return responseApi;
        }

        protected abstract string BigBusApiRequest<T>(T inputContext);

        protected abstract object CreateInputRequest(InputContext inputContext);

        /// <summary>
        /// This method adds the headers and Base address to Http Client.
        /// </summary>
        private void AddRequestHeadersAndAddressToApi()
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            if (HttpClient.BaseAddress == null)
            {
                HttpClient.BaseAddress = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.BigBusURI));

                var MyUserName = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.UserId));
                var MyPassword = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Password));

                var ByteArray = Encoding.ASCII.GetBytes(MyUserName + ":" + MyPassword);
                HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Constant.Basic, Convert.ToBase64String(ByteArray));
            }
        }
    }
}