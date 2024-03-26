using Logger.Contract;
using ServiceAdapters.PrioTicket.Constants;
using ServiceAdapters.PrioTicket.PrioTicket.Commands.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.PrioTicket.PrioTicket.Commands
{
    public class CancelReservationCmdHandler : CommandHandlerBase, ICancelReservationCmdHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly CancelReservationRq _cancelReservationRq;

        public CancelReservationCmdHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();
            _cancelReservationRq = new CancelReservationRq();
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            _cancelReservationRq.RequestType = Constant.CancelReserve;
            _cancelReservationRq.Data = new CancelReservationRqData
            {
                DistributorId = inputContext.UserName,
                ReservationReference = inputContext.ReservationReference,
                DistributorReference = inputContext.DistributorReference
            };
            var jsonRequest = SerializeDeSerializeHelper.Serialize(_cancelReservationRq);
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
            return SerializeDeSerializeHelper.DeSerialize<CancelReservationRs>(jsonResult.ToString());
        }

        protected override Task<object> GetJsonResultsV2(object inputJson, string token)
        {
            return _asyncClient.PostPrioJsonAsyncV2(inputJson, token);
        }
    }
}