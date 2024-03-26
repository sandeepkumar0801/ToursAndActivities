using Logger.Contract;
using ServiceAdapters.GoCity.Constants;
using ServiceAdapters.GoCity.GoCity.Commands.Contract;
using ServiceAdapters.GoCity.GoCity.Entities.Booking;
using ServiceAdapters.GoCity.GoCity.Entities.CancelBooking;
using System.Text;
using Util;

namespace ServiceAdapters.GoCity.GoCity.Commands
{
    public class CancellationCmdHandler : CommandHandlerBase, ICancellationCommandHandler
    {
        public CancellationCmdHandler(ILogger iLog) : base(iLog)
        {
        }

       
        protected override object CreateInputRequest<T>(T cancelBookingContext)
        {
            var canelBookingRq = new CancelBookingRequest();
           
            try
            {
                canelBookingRq = cancelBookingContext as CancelBookingRequest;
            }
            catch (Exception)
            {
                throw;
            }
            return canelBookingRq;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(CancelBookingResponse);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<CancelBookingResponse>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override  object GetResultsAsync(object input)
        {
            AddRequestHeadersAndAddressToApi();
            var url = Constant.Cancellation;
            var cancelRQ = input as CancelBookingRequest;
            if (cancelRQ == null)
            {
                return null;
            }
            var createBooking = SerializeDeSerializeHelper.Serialize(cancelRQ);
            var Content = new StringContent(createBooking, Encoding.UTF8, "application/json");
            var Result = _httpClient.PostAsync(url, Content);
            Result.Wait();
            return Result.Result.Content.ReadAsStringAsync().Result;
        }

    }
}