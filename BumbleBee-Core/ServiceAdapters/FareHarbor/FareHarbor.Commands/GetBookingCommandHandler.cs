using Logger.Contract;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts;

namespace ServiceAdapters.FareHarbor.FareHarbor.Commands
{
    public class GetBookingCommandHandler : CommandHandlerBase, IGetBookingCommandHandler
    {
        public GetBookingCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object FareHarborApiRequest<T>(T inputContext)
        {
            var items = inputContext as string[];
            var url = FormUrlGetBooking(items);
            var result = HttpClient.GetAsync(url);
            result.Wait();
            return result.Result;
        }

        protected override async Task<object> FareHarborApiRequestAsync<T>(T inputContext)
        {
            var items = inputContext as string[];
            var result = await HttpClient.GetAsync(FormUrlGetBooking(items));
            return result;
        }

        private string FormUrlGetBooking(string[] inputObjects)
        {
            return $"{Constant.CompanyUrlConstant}/{inputObjects[0]}/{Constant.BookingsUrlConstant}/{inputObjects[1]}/";
        }
    }
}