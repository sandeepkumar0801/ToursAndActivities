using Logger.Contract;
using ServiceAdapters.PrioTicket.Constants;
using ServiceAdapters.PrioTicket.PrioTicket.Commands.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using ServiceAdapters.PrioTicket.PrioTicket.Entities.ReservationRQ;
using System;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.PrioTicket.PrioTicket.Commands
{
    public class ReservationCmdHandler : CommandHandlerBase, IReservationCmdHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly ReservationRq _reservationRq;

        public ReservationCmdHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();
            _reservationRq = new ReservationRq();
        }

        /// <summary>
        /// Create Input Request
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest(InputContext inputContext)
        {
            _reservationRq.RequestType = Constant.Reserve;
            _reservationRq.Data = new ReservationRqData()
            {
                DistributorId = Convert.ToString(inputContext.UserName),
                TicketId = Convert.ToString(inputContext.ActivityId),
                PickupPointId = Convert.ToString(inputContext.PickupPointId),
                FromDateTime = inputContext.CheckInDate,
                ToDateTime = inputContext.CheckOutDate,
                BookingDetails = new BookingDetails[inputContext.TicketType.Count]
            };

            for (int i = 0; i <= inputContext.TicketType.Count - 1; i++)
            {
                _reservationRq.Data.BookingDetails[i] = new BookingDetails
                {
                    TicketType = inputContext.TicketType[i],
                    Count = inputContext.Count[i]
                };
            }
            _reservationRq.Data.DistributorReference = Convert.ToString(inputContext.DistributorReference) + "_" + Convert.ToString(inputContext.ActivityId);
            var jsonRequest = SerializeDeSerializeHelper.Serialize(_reservationRq);
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
            return SerializeDeSerializeHelper.DeSerialize<ReservationRs>(jsonResult.ToString());
        }
        protected override Task<object> GetJsonResultsV2(object inputJson, string token)
        {
            return _asyncClient.PostPrioJsonAsyncV2(inputJson, token);
        }
    }
}