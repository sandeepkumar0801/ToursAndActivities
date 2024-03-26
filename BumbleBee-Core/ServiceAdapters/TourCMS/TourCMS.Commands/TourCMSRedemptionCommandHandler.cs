using Isango.Entities.TourCMS;
using Logger.Contract;
using ServiceAdapters.TourCMS.Constants;
using ServiceAdapters.TourCMS.TourCMS.Commands.Contracts;
using System.Text;
using Util;

namespace ServiceAdapters.TourCMS.TourCMS.Commands
{
    public class TourCMSRedemptionCommandHandler : CommandHandlerBase, ITourCMSRedemptionCommandHandler
    {
        public TourCMSRedemptionCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object TourCMSApiRequest<T>(T inputContext, out string inputRequest, out string inputResponse)
        {
            int marketplaceId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountID));
            var criteria = inputContext as TourCMSRedemptionCriteria;
            string tourCMSMarketPlaceAccountIDTestMode =
            Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountIDTestMode));
            string privateKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSPrivateKey);
            if (tourCMSMarketPlaceAccountIDTestMode != "1")
            {
                marketplaceId = Convert.ToInt32(criteria.AccountId);
            }

            int channelId = criteria.ChannelId;
            var bookingStartDate = criteria.CheckinDate;
            var bookingEndDate = criteria.CheckoutDate;
            int Page = criteria.page;

            string path = FormUrlAvailability(bookingStartDate, bookingEndDate, Page);
            inputRequest = path;
            string verb = "GET";


            using (var httpClient = AddRequestHeadersAndAddressToApi(channelId, marketplaceId,
                path, verb, privateKey))
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, path);
                var response = httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();

                byte[] buf = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                string content = Encoding.UTF8.GetString(buf);
                inputResponse = content;
                return content;
            }
        }
        //Booking Start Date - this is the date the booking was created not the travel date
        private string FormUrlAvailability(DateTime BookingStartDate, DateTime BookingEndDate,int Page)
        {
            var startDate = BookingStartDate.ToString("yyyy-MM-dd");
            var endDate = BookingEndDate.ToString("yyyy-MM-dd");
            return UriConstant.RedemptionBooking + "?per_page=" + 250
                 + "&page=" + Page
                 + "&made_date_start=" + startDate
                 + "&made_date_end=" + endDate;
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
