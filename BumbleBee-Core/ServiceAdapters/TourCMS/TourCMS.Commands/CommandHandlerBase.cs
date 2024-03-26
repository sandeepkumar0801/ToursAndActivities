using Logger.Contract;
using ServiceAdapters.TourCMS.Constants;
using ServiceAdapters.TourCMS.TourCMS.Entities;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Util;

namespace ServiceAdapters.TourCMS.TourCMS.Commands
{
    public abstract class CommandHandlerBase
    {
        private readonly ILogger _log;

        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _log = log;
        }

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token,out string apiRequestAPI, out string apiResponseAPI)
        {
            var watch = Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = TourCMSApiRequest(inputRequest, out  apiRequestAPI, out  apiResponseAPI);
            watch.Stop();


            _log.WriteTimer(methodType.ToString(), token, Constant.TourCMS, watch.Elapsed.ToString());
            if (responseApi != null)
            {
                _log.Write(apiRequestAPI, responseApi.ToString(), methodType.ToString(), token, Constant.TourCMS);
            }
            return responseApi;

            
        }

        public virtual object Execute<T>(T inputContext, string token, MethodType methodType, out string apiRequestAPI, out string apiResponseAPI)
        {
            var watch = Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var responseApi = TourCMSApiRequest(inputRequest, out  apiRequestAPI, out  apiResponseAPI);
            watch.Stop();
            var apiResponse = responseApi.ToString();
            _log.WriteTimer(methodType.ToString(), token, Constant.TourCMS, watch.Elapsed.ToString());
            _log.Write(apiRequestAPI, apiResponse.ToString(), methodType.ToString(), token, Constant.TourCMS);
            return responseApi;

        }

        ///// <summary>
        ///// This method adds the headers and Base address to Http Client.
        ///// </summary>
        protected HttpClient AddRequestHeadersAndAddressToApi(int channelId,int marketplaceId,string path, string verb, string privateKey)
        {
            DateTime outboundTime = DateTime.Now.ToUniversalTime();
            string signature = GenerateSignature(path, verb, channelId, outboundTime,  marketplaceId,  privateKey);
            var httpClient = new HttpClient();
            if (httpClient.BaseAddress == null)
                httpClient.Timeout = TimeSpan.FromMinutes(3);
            httpClient.BaseAddress = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Uri));

            if (httpClient.DefaultRequestHeaders.Any())
                httpClient.DefaultRequestHeaders.Clear();
           
            httpClient.DefaultRequestHeaders.Add(Constant.TourCMSAuthorizationKey, Constant.TourCMSAuthorize + channelId + ":" + marketplaceId + ":" + signature);
            httpClient.DefaultRequestHeaders.Add(Constant.TourCMSDate, outboundTime.ToString("r"));
            

            return httpClient;
        }
        protected string GenerateSignature(string path, string verb, int channelId, DateTime outboundTime, int marketplaceId,string privateKey)
        {
            string stringToSign = "";
            string signature = "";
            string outboundStamp = DateTimeToStamp(outboundTime).ToString();

            // Build the basic string that gets signed
            stringToSign = channelId + "/" + marketplaceId + "/" + verb + "/" + outboundStamp + path;
            stringToSign = stringToSign.Trim();

            // Sign with private API Key
            var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(privateKey));
            hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            signature = Convert.ToBase64String(hmacsha256.Hash);
            signature = HttpUtility.UrlEncode(signature);

            return signature;
        }
        /// <summary>
        /// Convert a DateTime object (in UTC) to a Unix Timestapm, generally you won't neeed to call this directly
        /// </summary>
        protected int DateTimeToStamp(DateTime value)
        {
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (int)span.TotalSeconds;
        }
        ///// <summary>
        ///// Create Input Request
        ///// </summary>
        ///// <param name="inputContext"></param>
        ///// <returns></returns>
        protected virtual object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }
        protected abstract object GetResponseObject(string responseText);
        protected abstract object GetResultsAsync(object inputContext);
        protected virtual object TourCMSApiRequest<T>(T inputContext, out string inputRequest, out string inputResponse)
        {
            inputRequest="";
            inputResponse = "";
            return null;
        }
        
     }
}