using Logger.Contract;
using ServiceAdapters.PrioTicket.Constants;
using ServiceAdapters.PrioTicket.PrioTicket.Commands.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.PrioTicket.PrioTicket.Commands
{
    public class TicketListCmdHandler : CommandHandlerBase, ITicketListCmdHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly TicketListRq _ticketList;

        public TicketListCmdHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();
            _ticketList = new TicketListRq();
        }

        /// <summary>
        /// Deserialize json into TicketDetailRS type
        /// </summary>
        /// <param name="jsonResult"></param>
        /// <returns></returns>
        protected override object GetResults(object jsonResult)
        {
            return SerializeDeSerializeHelper.DeSerialize<TicketListRs>(jsonResult.ToString());
        }

        /// <summary>
        /// Create input request
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest(InputContext inputContext)
        {
            _ticketList.RequestType = Constant.PrioList;
            _ticketList.Data = new DataListRequest
            {
                DistributorId = inputContext.UserName,
            };
            var jsonRequest = SerializeDeSerializeHelper.Serialize(_ticketList);
            return jsonRequest;
        }

        /// <summary>
        /// Get Json Results
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected override object GetJsonResults(object jsonObject, string token)
        {
            return _asyncClient.PostPrioJsonAsync(jsonObject, token);
        }
        protected override Task<object> GetJsonResultsV2(object inputJson, string token)
        {
            return _asyncClient.PostPrioJsonAsyncV2(inputJson, token);
        }
    }
}