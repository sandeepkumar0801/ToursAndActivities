using Logger.Contract;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System.Text;
using Util;

namespace ServiceAdapters.GlobalTix.GlobalTix.Commands
{
    class CancelByTicketCommandHandler : CommandHandlerBase, ICancelByTicketCommandHandler
    {
        public CancelByTicketCommandHandler(ILogger iLog) : base(iLog)
        {

        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            CancelInputContext cancelContext = inputContext as CancelInputContext;
            return (cancelContext != null) 
                ? new CancelByTicketRQ() { TicketId = int.Parse(cancelContext.BookingNumber) } 
                : null;
        }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }

        protected override object GetResults(object input, string authString)
        {
            CancelByTicketRQ cancelRQ = input as CancelByTicketRQ;
            if (cancelRQ == null)
            {
                return null;
            }

            //Creating the post URL
            AsyncClient Client = GetServiceClient(Constant.URL_CancelByTicket);
            string response = Client.ConsumePostService(GetHttpRequestHeaders(), Constant.HttpHeader_ContentType_JSON, SerializeDeSerializeHelper.Serialize(cancelRQ), Encoding.UTF8);
            return response;
        }

        protected override Task<Object> GetResultsAsync(object input, string authString)
        {
            throw new NotImplementedException();
        }
    }
}
