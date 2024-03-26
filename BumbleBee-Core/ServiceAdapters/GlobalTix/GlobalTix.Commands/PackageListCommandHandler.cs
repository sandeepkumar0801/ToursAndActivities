using Logger.Contract;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
namespace ServiceAdapters.GlobalTix.GlobalTix.Commands
{
    public class PackageListCommandHandler : CommandHandlerBase, IPackageListCommandHandler
	{
		#region Constructors
		public PackageListCommandHandler(ILogger iLog) : base(iLog)
		{
		}
		#endregion

		protected override object CreateInputRequest(InputContext inputContext)
		{
			PackageListInputContext pkgListCtx = inputContext as PackageListInputContext;
			if (pkgListCtx == null)
			{
				return null;
			}

			IDictionary<string, string> queryParams = new Dictionary<string, string>();
			queryParams.Add(Constant.QueryParam_Page, pkgListCtx.PageNumber.ToString());
			queryParams.Add(Constant.QueryParam_ResellerPackageOnly, Constant.QueryParam_ResellerPackageOnly_Val.ToString());
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

			AsyncClient client = GetServiceClient(Constant.URL_PackageList);
			return client.ConsumeGetService(GetHttpRequestHeaders(), queryParams);
		}

		protected override Task<object> GetResultsAsync(object input, string authString)
		{
			throw new NotImplementedException();
		}
	}
}
