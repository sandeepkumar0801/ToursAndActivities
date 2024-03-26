using Logger.Contract;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts;
using System.Security.Authentication;
using Util;

namespace ServiceAdapters.FareHarbor.FareHarbor.Commands
{
    public class DeleteBookingCommandHandler : CommandHandlerBase, IDeleteBookingCommandHandler
    {
        public DeleteBookingCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object FareHarborApiRequest<T>(T inputContext)
        {
            var items = inputContext as string[];
            var result = HttpClient.DeleteAsync(FormUrlDeleteBooking(items));
            result.Wait();
            return result?.Result;
        }

        protected override async Task<object> FareHarborApiRequestAsync<T>(T inputContext)
        {
            var items = inputContext as string[];
            var result = await HttpClient.DeleteAsync(FormUrlDeleteBooking(items));
            return result;
        }

        private string FormUrlDeleteBooking(string[] inputObjects)
        {
            return $"{Constant.CompanyUrlConstant}/{inputObjects[0]}/{Constant.BookingsUrlConstant}/{inputObjects[1]}/";
        }

        protected override void AddRequestHeadersAndAddressToApi<T>(T inputContext)
        {
            var input = inputContext as string[];
            var httpClientHandler = new HttpClientHandler();

            // Set TLS versions for .NET Core 6.0
            httpClientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            HttpClient = new HttpClient(httpClientHandler);
            // SET TLS version for Framework 4.5
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 |
            //                                       SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            if (HttpClient.BaseAddress == null)
            {
                HttpClient.Timeout = TimeSpan.FromMinutes(3);
                HttpClient.BaseAddress =
                    new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborUri));
            }
            else
            {
                HttpClient.DefaultRequestHeaders.Remove(Constant.XFareHarborApiApp);
                HttpClient.DefaultRequestHeaders.Remove(Constant.XFareHarborApiUser);
            }

            HttpClient.DefaultRequestHeaders.Add(Constant.XFareHarborApiApp,
                ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborAppKey));
            HttpClient.DefaultRequestHeaders.Add(Constant.XFareHarborApiUser,
                input?[2] ?? ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborUserKey));
        }
    }
}