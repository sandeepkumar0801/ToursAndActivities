using Logger.Contract;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;

namespace ServiceAdapters.GlobalTix.GlobalTix.Commands
{
    public class PackageInfoCommandHandler : CommandHandlerBase, IPackageInfoCommandHandler
    {
        #region Class Constructors
        public PackageInfoCommandHandler(ILogger iLog) : base(iLog)
        {
        }
        #endregion
        
        protected override object CreateInputRequest(InputContext inputContext)
        {
            PackageInfoInputContext pkgInfoInputCtx = inputContext as PackageInfoInputContext;
            return
                (pkgInfoInputCtx != null)
                    ? new Dictionary<string, string>() { { Constant.QueryParam_Id, pkgInfoInputCtx.PackageId.ToString() } }
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

            AsyncClient client = GetServiceClient(Constant.URL_PackageInfo);
            return client.ConsumeGetService(GetHttpRequestHeaders(), queryParams);
        }

        protected override Task<object> GetResultsAsync(object input, string authString)
        {
            throw new NotImplementedException();
        }

    }
}
