using Logger.Contract;
using ServiceAdapters.TourCMS.Constants;
using ServiceAdapters.TourCMS.TourCMS.Commands.Contracts;
using ServiceAdapters.TourCMS.TourCMS.Entities.DeleteBookingRequest;
using System.Text;
using Util;


namespace ServiceAdapters.TourCMS.TourCMS.Commands
{
    public class DeleteBookingCommandHandler : CommandHandlerBase, 
        IDeleteBookingCommandHandler
    {
        public DeleteBookingCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T cancelBookingContext)
        {
            var canelBookingRq = new DeleteBookingRequest();
            var CancelBookingPassRequest = new DeleteBookingPassRequest();
            try
            {
                CancelBookingPassRequest = cancelBookingContext as DeleteBookingPassRequest;
                canelBookingRq.BookingId = CancelBookingPassRequest.BookingId;
            }
            catch (Exception)
            {
                throw;
            }
            return Tuple.Create(canelBookingRq, CancelBookingPassRequest.PrefixServiceCode);
           
        }

        protected override object TourCMSApiRequest<T>(T inputContext, out string inputRequest, out string inputResponse)
        {
           
            try
            {
                var inputTuple = inputContext as Tuple<DeleteBookingRequest, string>;
                var channelId = Convert.ToInt32(inputTuple?.Item2?.Split('_')[0]);
                var accountId = inputTuple?.Item2?.Split('_')[1];

                int marketplaceId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountID));
                string tourCMSMarketPlaceAccountIDTestMode =
                Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountIDTestMode));
                string privateKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSPrivateKey);
                //var serializeRqt = SerializeDeSerializeHelper.SerializeXml(inputTuple?.Item1);
                //var result = new StringContent(serializeRqt, Encoding.UTF8, "application/xml");
                //inputRequest = Convert.ToString(serializeRqt);
                inputRequest = "";
                if (tourCMSMarketPlaceAccountIDTestMode != "1")
                {
                    marketplaceId = Convert.ToInt32(accountId);//set dynamic id
                }
                var path = UriConstant.DeleteBooking +"?booking_id="+ inputTuple?.Item1?.BookingId;
                string verb = "POST";
               
                using (var httpClient = AddRequestHeadersAndAddressToApi(channelId, marketplaceId,
                   path, verb, privateKey))
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, path);
                    var response = httpClient.PostAsync(path,null).GetAwaiter().GetResult();
                    byte[] buf = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                    string content = Encoding.UTF8.GetString(buf);
                    inputResponse = content;
                    return content;
                }
            }
            catch (Exception ex)
            {
                inputRequest = string.Empty;
                inputResponse = string.Empty;
                //ignored
                //#TODO Add logging here
                return null;
            }
            
        }

        protected override object GetResponseObject(string responseText)
        {
           return null;
        }

        protected override object GetResultsAsync(object input)
        {

            return null;
        }
    }
}