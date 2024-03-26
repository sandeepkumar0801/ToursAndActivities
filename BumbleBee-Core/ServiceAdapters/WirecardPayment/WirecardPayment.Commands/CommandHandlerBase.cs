using Logger.Contract;
using ServiceAdapters.WirecardPayment.Constant;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Commands
{
    public abstract class CommandHandlerBase
    {
        #region Declaration Section

        private readonly bool _isTestMode;

        private readonly ILogger _log;

        protected HttpClient HttpClient { get; set; }

        #endregion Declaration Section

        #region Constructor

        protected CommandHandlerBase(ILogger log)
        {
            _isTestMode = Convert.ToBoolean(ConfigurationManager.AppSettings["IsWireCardInTestMode"]);
            HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
            _log = log;
        }

        #endregion Constructor

        /// <summary>
        /// This method used to call the API and Log the JSON Files.
        /// </summary>
        /// <param name="paymentCardCriteria">Input Parameter</param>
        /// <param name="is3D">Payment Type</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual Tuple<string, string> Execute(PaymentCardCriteria paymentCardCriteria, bool is3D, string token)
        {
            var watch = Stopwatch.StartNew();
            var request = CreateInputRequest(paymentCardCriteria);
            var response = WirecardPaymentApiRequest(request, is3D);
            watch.Stop();
            var requestText = SerializeDeSerializeHelper.Serialize(request);
            requestText = RemoveCardDetails(requestText);
            var responseText = RemoveCardDetails(response);
            _log.WriteTimer(paymentCardCriteria.MethodType.ToString(), token, "WirecardPayment", watch.Elapsed.ToString());
            _log.Write(requestText, responseText,
                paymentCardCriteria.MethodType.ToString(), token, "WirecardPayment");
            return Tuple.Create(request, response);
        }

        public virtual async Task<string> ExecuteAsync(PaymentCardCriteria paymentCardCriteria, bool is3D, string token)
        {
            var watch = Stopwatch.StartNew();
            var request = CreateInputRequest(paymentCardCriteria);
            var response = await WirecardPaymentApiRequestAsync(request, is3D);
            watch.Stop();
            _log.WriteTimer(paymentCardCriteria.MethodType.ToString(), token, "WirecardPayment", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(request), response,
                paymentCardCriteria.MethodType.ToString(), token, "WirecardPayment");
            return response;
        }

        private Uri GetUrl()
        {
            var serverUrl = !_isTestMode ? ConfigurationManagerHelper.GetValuefromAppSettings("LiveServerURL_WireCard") : ConfigurationManagerHelper.GetValuefromAppSettings("TestServerURL_WireCard");
            var url = new Uri(serverUrl);
            return url;
        }

        private string WirecardPaymentApiRequest(string requestXml, bool is3D)
        {
            var authenticateString = $"{GetUserName(is3D)}:{GetPassword(is3D)}";
            var encoding = new ASCIIEncoding();
            var authenticateBytes = encoding.GetBytes(authenticateString);
            var authenticateStringBase64 = Convert.ToBase64String(authenticateBytes);
            if(HttpClient.DefaultRequestHeaders?.Authorization != null)
            {
                HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
            }
            HttpClient.DefaultRequestHeaders.Add("Authorization", "basic " + authenticateStringBase64);
            var content = new StringContent(requestXml, Encoding.UTF8, "application/xml");
            var result = HttpClient.PostAsync(GetUrl(), content);
            result.Wait();
            return result.Result.Content.ReadAsStringAsync().Result;
        }

        private async Task<string> WirecardPaymentApiRequestAsync(string requestXml, bool is3D)
        {
            var authenticateString = $"{GetUserName(is3D)}:{GetPassword(is3D)}";
            var encoding = new ASCIIEncoding();
            var authenticateBytes = encoding.GetBytes(authenticateString);
            var authenticateStringBase64 = Convert.ToBase64String(authenticateBytes);
            HttpClient.DefaultRequestHeaders.Add("Authorization", $"basic {authenticateStringBase64}");
            var content = new StringContent(requestXml, Encoding.UTF8, "application/xml");
            var result = await HttpClient.PostAsync(GetUrl(), content);
            return result.Content.ReadAsStringAsync().Result;
        }

        private string GetUserName(bool is3D)
        {
            var liveUserNameAndPassword =
                GetUserNameOrPassword(is3D ? "LiveUserName_3DWirecard" : "LiveUserName_Wirecard");
            var testUsernameAndPassword = GetUserNameOrPassword(is3D ? "TestUserName_3DWirecard" : "TestUserName_Wirecard");
            var userName = !_isTestMode ? liveUserNameAndPassword : testUsernameAndPassword;
            return userName;
        }

        private string GetPassword(bool is3D)
        {
            string password;
            if (!_isTestMode)
            {
                password = is3D ? GetUserNameOrPassword("LivePassword_3DWirecard") : GetUserNameOrPassword("LivePassword_Wirecard");
            }
            else
            {
                password = GetUserNameOrPassword(is3D ? "TestPassword_Wirecard" : "TestPassword_Wirecard");
            }
            return password;
        }

        private string GetUserNameOrPassword(string key)
        {
            return ConfigurationManagerHelper.GetValuefromAppSettings(key);
        }

        private string RemoveCardDetails(string text)
        {
            var updatedText = text;
            if (!string.IsNullOrWhiteSpace(updatedText))
            {
                try
                {
                    var regex = new Regex(@"<CREDIT_CARD_DATA>(.*?)</CREDIT_CARD_DATA>");
                    updatedText = regex.Replace(text, "");
                }
                catch (Exception ex)
                {
                    updatedText += ex.Message;
                }
            }
            return updatedText;
        }

        protected abstract string CreateInputRequest(PaymentCardCriteria paymentCardCriteria);
    }
}