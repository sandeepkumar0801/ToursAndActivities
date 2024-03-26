using Logger.Contract;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;

namespace ServiceAdapters.GlobalTix.GlobalTix.Commands
{
    public class ActivityInfoCommandHandler : CommandHandlerBase, IActivityInfoCommandHandler
    {
        #region Class Constructors
        public ActivityInfoCommandHandler(ILogger iLog) : base(iLog)
        {
        }
        #endregion
        
        protected override object CreateInputRequest(InputContext inputContext)
        {
            //return inputContext;
            ActivityInfoInputContext actInfoInputCtx = inputContext as ActivityInfoInputContext;
            return
                (actInfoInputCtx != null)
                    ? new Dictionary<string, string>() { { Constant.QueryParam_Id, actInfoInputCtx.FactSheetId.ToString() } }
                    : null;
        }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }

        protected override object GetResults(object input, string token)
        {
            IDictionary<string, string> queryParams = input as IDictionary<string, string>;
            if (queryParams == null)
            {
                return null;
            }

            AsyncClient client = GetServiceClient(Constant.URL_ActivityInfo);
            return client.ConsumeGetService(GetHttpRequestHeaders(), queryParams);
        }

        protected override Task<object> GetResultsAsync(object input, string authString)
        {
            throw new NotImplementedException();
        }

    }
}
