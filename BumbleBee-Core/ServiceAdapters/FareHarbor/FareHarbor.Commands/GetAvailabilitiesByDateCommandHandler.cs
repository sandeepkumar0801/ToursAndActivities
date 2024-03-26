using Isango.Entities.FareHarbor;
using Logger.Contract;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts;

namespace ServiceAdapters.FareHarbor.FareHarbor.Commands
{
    public class GetAvailabilitiesByDateCommandHandler : CommandHandlerBase, IGetAvailabilitiesByDateCommandHandler
    {
        public GetAvailabilitiesByDateCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object FareHarborApiRequest<T>(T inputContext)
        {
            var criteria = inputContext as FareHarborCriteria;
            var result = HttpClient.GetAsync(FormUrlAvailabilities(criteria));
            result.Wait();
            return result;
        }

        protected override async Task<object> FareHarborApiRequestAsync<T>(T inputContext)
        {
            var criteria = inputContext as FareHarborCriteria;
            var result = await HttpClient.GetAsync(FormUrlAvailabilities(criteria));
            return result;
        }

        private string FormUrlAvailabilities(FareHarborCriteria criteria)
        {
            return $"{Constant.CompanyUrlConstant}/{criteria.CompanyName}/{Constant.ItemsUrlConstant}/{criteria.ActivityCode}/{Constant.MinimalUrlConstant}/{Constant.AvailabilitiesUrlConstant}/{Constant.DateUrlConstant}/{criteria.CheckinDate.ToString(Constant.DateFormat)}/";
        }
    }
}