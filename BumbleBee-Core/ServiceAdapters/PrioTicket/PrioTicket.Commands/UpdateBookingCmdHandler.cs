using Logger.Contract;
using ServiceAdapters.PrioTicket.PrioTicket.Commands.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.PrioTicket.PrioTicket.Commands
{
    public class UpdateBookingCmdHandler : CommandHandlerBase, IUpdateBookingCmdHandler
    {
        private readonly AsyncClient _asyncClient;

        public UpdateBookingCmdHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            return null;
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
        /// Get result
        /// </summary>
        /// <param name="jsonResult"></param>
        /// <returns></returns>
        protected override object GetResults(object jsonResult)
        {
            // json deserialize in Dto
            //return SerializeDeSerializeHelper.DeSerialize<cre>
            return jsonResult;
        }
        protected override Task<object> GetJsonResultsV2(object inputJson, string token)
        {
            return _asyncClient.PostPrioJsonAsyncV2(inputJson, token);
        }
    }
}