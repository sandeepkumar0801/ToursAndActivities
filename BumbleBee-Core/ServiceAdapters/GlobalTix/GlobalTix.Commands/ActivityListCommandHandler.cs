using Logger.Contract;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;

namespace ServiceAdapters.GlobalTix.GlobalTix.Commands
{
    public class ActivityListCommandHandler : CommandHandlerBase, IActivityListCommandHandler
    {
        #region Constructors
        public ActivityListCommandHandler(ILogger iLog) : base(iLog)
        {
        }
        #endregion

        protected override object CreateInputRequest(InputContext inputContext)
        {
            AvailabilityInputContext availCtx = inputContext as AvailabilityInputContext;
            if (availCtx == null)
            {
                return null;
            }

            IDictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add(Constant.QueryParam_CountryId, availCtx.CountryId);
            queryParams.Add(Constant.QueryParam_CityId, availCtx.CityId);
            queryParams.Add(Constant.QueryParam_Page, availCtx.PageNumber.ToString());
            return queryParams;
        }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }

        protected override object GetResults(object input, string authString)
        {
            IDictionary<string, string> queryParams = input as IDictionary<string, string>;
            if (queryParams == null)
            {
                return null;
            }

            AsyncClient client = GetServiceClient(Constant.URL_ActivityList);
            return client.ConsumeGetService(GetHttpRequestHeaders(), queryParams);
        }

        protected override Task<object> GetResultsAsync(object input, string authString)
        {
            if (input == null) return null;

            var client = GetServiceClient(Constant.URL_ActivityList);
            IDictionary<string, string> queryParams = new Dictionary<string, string>();

            return client.ConsumeGetServiceAsync(GetHttpRequestHeaders(), queryParams);
        }

   }
}
