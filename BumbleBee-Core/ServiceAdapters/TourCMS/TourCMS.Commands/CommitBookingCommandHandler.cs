using Isango.Entities.Activities;
using Isango.Entities.TourCMS;
using Logger.Contract;
using ServiceAdapters.TourCMS.Constants;
using ServiceAdapters.TourCMS.TourCMS.Commands.Contracts;
using ServiceAdapters.TourCMS.TourCMS.Entities.CommitBooking;
using System.Text;
using Util;

namespace ServiceAdapters.TourCMS.TourCMS.Commands
{
    public class CommitBookingCommandHandler : CommandHandlerBase, 
        ICommitBookingCommandHandler
    {
        public CommitBookingCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T bookingContext)
        {
            var prefixServiceCode = string.Empty;
            var bookingConfirmRq = new CommitBookingRequest();
            try
            {
                var inputContext = bookingContext as ServiceAdapters.TourCMS.TourCMS.Entities.InputContext;
                var selectedProducts = inputContext.SelectedProducts;
                var tourCMSSelectdProducts = (TourCMSSelectedProduct)selectedProducts;

                var selectedOptions = selectedProducts?.ProductOptions
                   ?.FindAll(f => f.IsSelected.Equals(true))
                   ?.Cast<ActivityOption>().ToList();

                //find TourCMs Channel Id and TourCMS Account Id
                prefixServiceCode = selectedOptions?.FirstOrDefault()?.PrefixServiceCode;

                if (!(tourCMSSelectdProducts!=null))
                {
                    return null;
                }
                bookingConfirmRq.BookingId = tourCMSSelectdProducts.BookingId;
            }
            catch (Exception)
            {
                throw;
            }
            return Tuple.Create(bookingConfirmRq, prefixServiceCode);
        }

        protected override object TourCMSApiRequest<T>(T inputContext, out string inputRequest, out string inputResponse)
        {
           
            try
            {
                var inputTuple = inputContext as Tuple<CommitBookingRequest, string>;
                var channelId = Convert.ToInt32(inputTuple?.Item2?.Split('_')[0]);
                var accountId = inputTuple?.Item2?.Split('_')[1];
                var requestXML = SerializeDeSerializeHelper.SerializeXml(inputTuple?.Item1);
                var result = new StringContent(requestXML, Encoding.UTF8, "application/xml");
                inputRequest = Convert.ToString(requestXML);
                int marketplaceId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountID));
                var path = UriConstant.CommitBooking;
                string verb = "POST";
                string tourCMSMarketPlaceAccountIDTestMode =
                Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountIDTestMode));
                string privateKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSPrivateKey);
                if (tourCMSMarketPlaceAccountIDTestMode != "1")
                {
                    marketplaceId = Convert.ToInt32(accountId);
                }
                using (var httpClient = AddRequestHeadersAndAddressToApi(channelId, marketplaceId,
                   path, verb, privateKey))
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, path);
                    var response = httpClient.PostAsync(path, result).GetAwaiter().GetResult();
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