using Isango.Entities.TourCMSCriteria;
using Logger.Contract;
using ServiceAdapters.TourCMS.Constants;
using ServiceAdapters.TourCMS.TourCMS.Commands.Contracts;
using System.Text;
using Util;

namespace ServiceAdapters.TourCMS.TourCMS.Commands
{
    public class ChannelShowCommandHandler : CommandHandlerBase, IChannelShowCommandHandler
    {
        public ChannelShowCommandHandler(ILogger log) : base(log)
        {
        }

        

        protected override object TourCMSApiRequest<T>(T inputContext, out string inputRequest, out string inputResponse)
        {
            int marketplaceId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountID));
            var criteria = inputContext as TourCMSCriteria;
            string tourCMSMarketPlaceAccountIDTestMode =
            Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountIDTestMode));
            string privateKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSPrivateKey);
            if (tourCMSMarketPlaceAccountIDTestMode != "1")
            {
                marketplaceId = Convert.ToInt32(criteria.AccountId);
            }

            int channelId = criteria.ChannelId;
           
            string path = FormUrlAvailability();
            inputRequest = path;
            string verb = "GET";
            

            using (var httpClient = AddRequestHeadersAndAddressToApi(channelId, marketplaceId,
                path, verb, privateKey))
            {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get,path);
            var response =  httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
                
            byte[] buf = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            string content = Encoding.UTF8.GetString(buf);
            inputResponse = content;
            return content;
           }
        }

        private string FormUrlAvailability()
        {
            return $"{UriConstant.ShowChannelC}{UriConstant.ShowChannelText}{UriConstant.ShowChannelXML}";
        }
        protected override object GetResponseObject(string responseText)
        {
            throw new NotImplementedException();
        }

        protected override object GetResultsAsync(object inputContext)
        {
            throw new NotImplementedException();
        }
    }
}