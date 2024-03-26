using Logger.Contract;
using ServiceAdapters.PrioTicket.Constants;
using ServiceAdapters.PrioTicket.PrioTicket.Commands.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.PrioTicket.PrioTicket.Commands
{
    public class TicketDetailsCmdHandler : CommandHandlerBase, ITicketDetailsCmdHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly TicketDetailsRq _ticketDetails;

        public TicketDetailsCmdHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();
            _ticketDetails = new TicketDetailsRq();
        }

        /// <summary>
        /// Deserialize json into TicketDetailRS type
        /// </summary>
        /// <param name="jsonResult"></param>
        /// <returns></returns>
        protected override object GetResults(object jsonResult)
        {
            return SerializeDeSerializeHelper.DeSerialize<TicketDetailRs>(jsonResult.ToString());
        }

        /// <summary>
        /// Create input request
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest(InputContext inputContext)
        {
            _ticketDetails.RequestType = Constant.Details;
            _ticketDetails.Data = new DataRequest
            {
                DistributorId = inputContext.UserName,
                TicketId = inputContext.ActivityId
            };
            var jsonRequest = SerializeDeSerializeHelper.Serialize(_ticketDetails);
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