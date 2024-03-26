using Logger.Contract;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Entities;
using System.Security.Authentication;
using Util;

namespace ServiceAdapters.FareHarbor.FareHarbor.Commands
{
    public class GetLodgingsAvailabilityCommandHandler : CommandHandlerBase, IGetLodgingsAvailabilityCommandHandler
    {
        public GetLodgingsAvailabilityCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object FareHarborApiRequest<T>(T inputContext)
        {
            var items = inputContext as FareHarborRequest;
            var result = HttpClient.GetAsync(FormUrlLodgingsAvailability(items));
            result.Wait();
            return result.Result;
        }

        protected override async Task<object> FareHarborApiRequestAsync<T>(T inputContext)
        {
            var items = inputContext as FareHarborRequest;
            var result = await HttpClient.GetAsync(FormUrlLodgingsAvailability(items));
            return result;
        }

        private string FormUrlLodgingsAvailability(FareHarborRequest fareHarborRequest)
        {
            return $"{Constant.CompanyUrlConstant}/{fareHarborRequest.ShortName}/{Constant.AvailabilitiesUrlConstant}/{fareHarborRequest.Availability}/{Constant.LodgingsUrlConstant}/";
        }

        protected override void AddRequestHeadersAndAddressToApi<T>(T inputContext)
        {
            var userKey = inputContext as FareHarborRequest;
            // SET TLS version for Framework 4.5
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var httpClientHandler = new HttpClientHandler();

            // Set TLS versions for .NET Core 6.0
            httpClientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            HttpClient = new HttpClient(httpClientHandler);
            if (HttpClient.BaseAddress == null)
            {
                HttpClient.Timeout = TimeSpan.FromMinutes(3);
                HttpClient.BaseAddress = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborUri));
            }
            else
            {
                HttpClient.DefaultRequestHeaders.Remove(Constant.XFareHarborApiApp);
                HttpClient.DefaultRequestHeaders.Remove(Constant.XFareHarborApiUser);
            }

            HttpClient.DefaultRequestHeaders.Add(Constant.XFareHarborApiApp, ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborAppKey));
            HttpClient.DefaultRequestHeaders.Add(Constant.XFareHarborApiUser, userKey.Uuid);
        }
    }
}