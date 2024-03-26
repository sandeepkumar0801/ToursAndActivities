using Isango.Entities;
using Logger.Contract;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Entities;
using System.Security.Authentication;
using Util;

namespace ServiceAdapters.FareHarbor.FareHarbor.Commands
{
    public class GetCompaniesCommandHandler : CommandHandlerBase, IGetCompaniesCommandHandler
    {
        public GetCompaniesCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object FareHarborApiRequest<T>(T inputContext)
        {
            var result = HttpClient.GetAsync(UriConstants.Companies);
            result.Wait();
            return result.Result;
        }

        protected override async Task<object> FareHarborApiRequestAsync<T>(T inputContext)
        {
            var result = await HttpClient.GetAsync(UriConstants.Companies);
            return result;
        }

        protected override void AddRequestHeadersAndAddressToApi<T>(T inputContext)
        {
            var userKey = inputContext as FareHarborUserKey;
            var httpClientHandler = new HttpClientHandler();

            // Set TLS versions for .NET Core 6.0
            httpClientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            HttpClient = new HttpClient(httpClientHandler);
            // SET TLS version for Framework 4.5
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
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

            HttpClient.DefaultRequestHeaders.Add(Constant.XFareHarborApiUser, userKey?.UserKey ?? ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborUserKey));
        }
    }
}