using Logger.Contract;
using ServiceAdapters.NewCitySightSeeing.Constants;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands.Contract;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Booking;
using System.Text;
using Util;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands
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
            var result = default(string);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<string>(responseText);
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
            var url = $"{_newCitySightSeeingServiceURL}{Constant.Cancellation}";
            var cancelRQ = input as CancelBookingRequest;
            if (cancelRQ == null)
            {
                return null;
            }
            var client = new AsyncClient
            {
                ServiceURL = url
            };
            return client.ConsumePostService(GetHttpRequestHeaders(), Constant.HttpHeader_ContentType_JSON, SerializeDeSerializeHelper.Serialize(cancelRQ), Encoding.UTF8);
        }

    }
}