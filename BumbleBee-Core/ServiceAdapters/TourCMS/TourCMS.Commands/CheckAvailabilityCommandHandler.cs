using Isango.Entities.TourCMSCriteria;
using Logger.Contract;
using ServiceAdapters.TourCMS.Constants;
using ServiceAdapters.TourCMS.TourCMS.Commands.Contracts;
using System.Text;
using Util;

namespace ServiceAdapters.TourCMS.TourCMS.Commands
{
    public class CheckAvailabilityCommandHandler : CommandHandlerBase, ICheckAvailabilityCommandHandler
    {
        public CheckAvailabilityCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object TourCMSApiRequest<T>(T inputContext, out string inputRequest, out string inputResponse)
        {
            var criteria = inputContext as TourCMSCriteria;
            int channelId = criteria.ChannelId;
            int tourId = criteria.TourId;
            var checkInDate = criteria.CheckinDate;
            var checkOutDate = criteria.CheckoutDate;
            var paxString = string.Empty;
            if (criteria.NoOfPassengers != null)
            {
                foreach (var item in criteria?.NoOfPassengers)
                {
                    var ageGroupCode = criteria?.TourCMSMappings?.FirstOrDefault
                            (x => x.PassengerType == item.Key)?.AgeGroupCode;
                    if (!String.IsNullOrEmpty(ageGroupCode))
                    {
                        paxString += ageGroupCode + "=" + item.Value + "&";
                    }
                }
            }
            if (!String.IsNullOrEmpty(paxString))
            {
                paxString = paxString?.Remove(paxString.Length - 1, 1);
            }
            
            int marketplaceId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountID));
            string path = FormUrlAvailability(tourId, checkInDate, paxString);
            inputRequest = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Uri) + path;
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
                //load the file using;
                //var xDocument = XDocument.Load(@"C:\tourcms\1_9.xml");
                //string xml = xDocument.ToString();
                //return xml;
                var response = httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
                byte[] buf = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                string content = Encoding.UTF8.GetString(buf);
                inputResponse = content;
                return content;
            }
        }

        private string FormUrlAvailability(int tourId,DateTime checkInDate, 
            string paxString)
        {
            //return "/c/tour/datesprices/checkavail.xml?id=83" +
            //    "&date=2021-11-04&r1=2&r2=1&hdur=2";

            //c / tour / datesprices / checkavail.xml ? id = 114
            // & date = 2012 - 01 - 01 & ad = 2 & 
            // ch = 2 & inf = 0 & hdur = 2

            return UriConstant.ShowChannelC + UriConstant.Tour
                + UriConstant.DatesPrices
                + UriConstant.CheckAvail
                + "?id=" + tourId
                + "&date=" + checkInDate.ToString("yyyy-MM-dd") + "&"
                + paxString;
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