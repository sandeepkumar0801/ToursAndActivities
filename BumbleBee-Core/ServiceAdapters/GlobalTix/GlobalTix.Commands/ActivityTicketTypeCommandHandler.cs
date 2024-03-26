using Logger.Contract;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;

namespace ServiceAdapters.GlobalTix.GlobalTix.Commands
{
    public class ActivityTicketTypeCommandHandler : CommandHandlerBase, IActivityTicketTypeCommandHandler
    {
        #region Class Constructors
        public ActivityTicketTypeCommandHandler(ILogger iLog) : base(iLog)
        {

        }
        #endregion
        
        protected override object CreateInputRequest(InputContext inputContext)
        {
			ActivityTicketTypeInputContext actTTInputCtx = inputContext as ActivityTicketTypeInputContext;
            if (actTTInputCtx == null)
            {
                return null;
            }

            IDictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add(Constant.QueryParam_Id, actTTInputCtx.TicketType);
            return queryParams;
        }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }

        protected override object GetResults(object inputContext, string token)
        {
            IDictionary<string, string> queryParams = inputContext as IDictionary<string, string>;
            if (queryParams == null)
            {
                return null;
            }

            var client = GetServiceClient(Constant.URL_ActivityTicketType);
            return client.ConsumeGetService(GetHttpRequestHeaders(), queryParams);
        }

        protected override Task<object> GetResultsAsync(object input, string authString)
        {
            throw new NotImplementedException();
        }

    }
}
