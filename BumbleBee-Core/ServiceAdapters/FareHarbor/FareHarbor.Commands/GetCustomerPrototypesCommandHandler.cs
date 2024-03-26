using Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor;
using Logger.Contract;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts;
using System.Net;
using Util;

namespace ServiceAdapters.FareHarbor.FareHarbor.Commands
{
    public class GetCustomerPrototypesCommandHandler : CommandHandlerBase, IGetCustomerPrototypesCommandHandler
    {
        private readonly ILogger _log;

        public GetCustomerPrototypesCommandHandler(ILogger log) : base(log)
        {
            _log = log;
        }

        /// <summary>
        /// This method adds the headers and Base address to Http Client.
        /// </summary>
        private HttpClient AddRequestHeadersAndAddressToApi2<T>(T inputContext)
        {
            var product = inputContext as Product;
            var httpClient = new HttpClient();
            //{
            //    Timeout = TimeSpan.FromMinutes(5),
            //    BaseAddress = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborUri))
            //};

            if (httpClient.BaseAddress == null)
            {
                httpClient.Timeout = TimeSpan.FromMinutes(3);
                httpClient.BaseAddress = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborUri));
                httpClient.DefaultRequestHeaders.Add(Constant.XFareHarborApiApp, ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FareHarborAppKey));
                httpClient.DefaultRequestHeaders.Add(Constant.XFareHarborApiUser, product?.UserKey);
            }
            return httpClient;
        }

        protected override object FareHarborApiRequest<T>(T inputContext)
        {
            var product = inputContext as Product;
            var url = FormUrlCreateBooking(product);
            var httpClient = AddRequestHeadersAndAddressToApi2(inputContext);
            var result = httpClient.GetAsync(url);
            try
            {
                result.Wait();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = "GetCustomerPrototypesCommandHandler",
                    MethodName = "FareHarborApiRequest",
                    Params = Util.SerializeDeSerializeHelper.Serialize(inputContext)
                };
                _log.Error(isangoErrorEntity, ex);
                //timeout probably - check logs;
                return null;
            }
            if (result.Result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            var res = result.Result;
            return res;
        }

        private string FormUrlCreateBooking(Product product)
        {
            return $"{Constant.CompanyUrlConstant}/{product.SupplierName}/{Constant.ItemsUrlConstant}/{product.FactsheetId}/{Constant.MinimalUrlConstant}/{Constant.AvailabilitiesUrlConstant}/{Constant.DateRangeUrlConstant}/{product.CheckinDate:yyyy-MM-dd}/{product.CheckoutDate:yyyy-MM-dd}/";
        }
    }
}