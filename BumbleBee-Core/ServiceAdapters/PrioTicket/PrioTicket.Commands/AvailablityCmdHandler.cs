using Logger.Contract;
using ServiceAdapters.PrioTicket.Constants;
using ServiceAdapters.PrioTicket.PrioTicket.Commands.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.PrioTicket.PrioTicket.Commands
{
    public class AvailablityCmdHandler : CommandHandlerBase, IAvailablityCmdHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly AvailablityRq _availablityRq;

        public AvailablityCmdHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();
            _availablityRq = new AvailablityRq();
        }

        /// <summary>
        /// Create Input Request
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest(InputContext inputContext)
        {
            _availablityRq.RequestType = Constant.Availabilities;
            _availablityRq.Data = new AvailablityData
            {
                DistributorId = inputContext.UserName,
                TicketId = inputContext.ActivityId,
                FromDate = DateTime.Parse(inputContext.CheckInDate).ToString(Constant.DateFormat),
                ToDate = DateTime.Parse(inputContext.CheckOutDate).ToString(Constant.DateFormat)
            };
            var jsonRequest = SerializeDeSerializeHelper.Serialize(_availablityRq);
            return jsonRequest;
        }

        /// <summary>
        /// GetJsonResults
        /// </summary>
        /// <param name="inputJson"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected override object GetJsonResults(object inputJson, string token)
        {
            return _asyncClient.PostPrioJsonAsync(inputJson, token);
        }
        protected override Task<object> GetJsonResultsV2(object inputJson, string token)
        {
            return _asyncClient.PostPrioJsonAsyncV2(inputJson, token);
        }
        /// <summary>
        /// GetResults
        /// </summary>
        /// <param name="jsonResult"></param>
        /// <returns></returns>
        protected override object GetResults(object jsonResult)
        {
            return SerializeDeSerializeHelper.DeSerialize<AvailablityRs>(jsonResult.ToString());
        }
    }
}