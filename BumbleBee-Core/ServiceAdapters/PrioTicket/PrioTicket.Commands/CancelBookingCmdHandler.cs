using Logger.Contract;
using ServiceAdapters.PrioTicket.Constants;
using ServiceAdapters.PrioTicket.PrioTicket.Commands.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.PrioTicket.PrioTicket.Commands
{
    public class CancelBookingCmdHandler : CommandHandlerBase, ICancelBookingCmdHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly CancelBookingRq _cancelBookingRq;

        public CancelBookingCmdHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();
            _cancelBookingRq = new CancelBookingRq();
        }

        /// <summary>
        /// CreateInputRequest
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest(InputContext inputContext)
        {
            _cancelBookingRq.RequestType = Constant.CancleBooking;
            _cancelBookingRq.Data = new DataCancelBooking
            {
                DistributorId = inputContext.UserName,
                BookingReference = inputContext.BookingReference,
                DistributorReference = inputContext.DistributorReference
            };
            var jsonRequest = SerializeDeSerializeHelper.Serialize(_cancelBookingRq);
            return jsonRequest;
        }

        /// <summary>
        /// Get Json Results
        /// </summary>
        /// <param name="inputJson"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected override object GetJsonResults(object inputJson, string token)
        {
            return _asyncClient.PostPrioJsonAsync(inputJson, token);
        }

        /// <summary>
        /// Get Results
        /// </summary>
        /// <param name="jsonResult"></param>
        /// <returns></returns>
        protected override object GetResults(object jsonResult)
        {
            // DeserializeObject to the Response class
            return SerializeDeSerializeHelper.DeSerialize<CancelBookingRs>(jsonResult.ToString());
        }

        protected override Task<object> GetJsonResultsV2(object inputJson, string token)
        {
            return _asyncClient.PostPrioJsonAsyncV2(inputJson, token);
        }
    }
}