using Isango.Entities.TourCMSCriteria;
using Logger.Contract;
using ServiceAdapters.TourCMS.Constants;
using ServiceAdapters.TourCMS.TourCMS.Commands.Contracts;
using System.Text;
using Util;

namespace ServiceAdapters.TourCMS.TourCMS.Commands
{
    public class TourShowCommandHandler : CommandHandlerBase, ITourShowCommandHandler
    {
        public TourShowCommandHandler(ILogger log) : base(log)
        {
        }

     

        protected override object TourCMSApiRequest<T>(T inputContext, out string inputRequest, out string inputResponse)
        {
            var criteria = inputContext as TourCMSCriteria;
            int channelId = criteria.ChannelId;
            int tourId = criteria.TourId;
            int marketplaceId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountID));
            string path = FormUrlAvailability(tourId);
            inputRequest = path;
            string verb = "GET";


            string tourCMSMarketPlaceAccountIDTestMode =
               Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountIDTestMode));
            string privateKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSPrivateKey);
            if (tourCMSMarketPlaceAccountIDTestMode != "1")
            {
                marketplaceId = Convert.ToInt32(criteria.AccountId);
            }

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

        private string FormUrlAvailability(int tourId)
        {
            return UriConstant.ShowChannelC+UriConstant.Tour+UriConstant.ShowChannelXML+"?id="+tourId;
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