using Logger.Contract;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Entities;

namespace ServiceAdapters.FareHarbor.FareHarbor.Commands
{
    public class GetLodgingsCommandHandler : CommandHandlerBase, IGetLodgingsCommandHandler
    {
        public GetLodgingsCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object FareHarborApiRequest<T>(T inputContext)
        {
            var items = inputContext as FareHarborRequest;
            var result = HttpClient.GetAsync(FormUrlLodgings(items));
            result.Wait();
            return result.Result;
        }

        protected override async Task<object> FareHarborApiRequestAsync<T>(T inputContext)
        {
            var items = inputContext as FareHarborRequest;
            var result = await HttpClient.GetAsync(FormUrlLodgings(items));
            return result;
        }

        private string FormUrlLodgings(FareHarborRequest fareHarborRequest)
        {
            return $"{Constant.CompanyUrlConstant}/{fareHarborRequest.ShortName}/{Constant.LodgingsUrlConstant}/";
        }
    }
}