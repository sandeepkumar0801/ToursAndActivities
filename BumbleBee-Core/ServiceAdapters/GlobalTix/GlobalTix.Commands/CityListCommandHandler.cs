using Logger.Contract;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;

namespace ServiceAdapters.GlobalTix.GlobalTix.Commands
{
    public class CityListCommandHandler : CommandHandlerBase, ICityListCommandHandler
	{
		#region Class Constructors
		public CityListCommandHandler(ILogger iLog) : base(iLog)
		{
		}
		#endregion

		protected override object CreateInputRequest(InputContext inputContext)
		{
			return null;
		}

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }

        protected override object GetResults(object input, string token)
		{
			AsyncClient client = GetServiceClient(Constant.URL_CityList);
			return client.ConsumeGetService(GetHttpRequestHeaders(), null);
		}

		protected override Task<object> GetResultsAsync(object input, string authString)
		{
			throw new NotImplementedException();
		}
	}
}
