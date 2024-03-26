using Logger.Contract;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;

namespace ServiceAdapters.GlobalTix.GlobalTix.Commands
{
    class CancelByBookingCommandHandler : CommandHandlerBase, ICancelByBookingCommandHandler
    {
        public CancelByBookingCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            CancelInputContext cancelContext = inputContext as CancelInputContext;
            return
                (cancelContext != null)
                    ? new Dictionary<string, string>() { { Constant.QueryParam_RefernceNumber, cancelContext.BookingReference } }
                    : null;
        }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }

        protected override object GetResults(object input, string authString)
        {
            IDictionary<string, string> cancelBookingQueryParams = input as IDictionary<string, string>;
            if (cancelBookingQueryParams == null)
            {
                return null;
            }

            AsyncClient Client = GetServiceClient(Constant.URL_CancelByBooking);
            return Client.ConsumeGetService(GetHttpRequestHeaders(), cancelBookingQueryParams);
        }

        protected override Task<object> GetResultsAsync(object input, string authString)
        {
            throw new NotImplementedException();
        }
    }
}
