using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger.Contract;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.Constants;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands
{
    public class ProductListCommandHandler : CommandHandlerBase, IProductListCommandHandler
    {
        #region Constructors
        public ProductListCommandHandler(ILogger iLog) : base(iLog)
        {
        }
        #endregion

        protected override object CreateInputRequest(InputContext inputContext)
        {
            AvailabilityInputContext availCtx = new AvailabilityInputContext();
            if (availCtx == null)
            {
                return null;
            }

            IDictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add(Constant.QueryParam_CountryId, inputContext.countryid.ToString());
			queryParams.Add(Constant.QueryParam_Page, inputContext.PageNumber.ToString());
			return queryParams;
        }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }

        protected override object GetResults(object input, string authString,bool isNonThailandProduct)
        {
            IDictionary<string, string> queryParams = input as IDictionary<string, string>;
            if (queryParams == null)
            {
                return null;
            }

            AsyncClient client = GetServiceClient(Constant.URL_ProductList);
            return client.ConsumeGetServicePrioHub(GetHttpRequestHeaders(isNonThailandProduct), queryParams);
        }

        protected override Task<object> GetResultsAsync(object input, string authString,bool isNonThailandProduct)
        {
            if (input == null) return null;

            var client = GetServiceClient(Constant.URL_ProductList);
            IDictionary<string, string> queryParams = new Dictionary<string, string>();

            return client.ConsumeGetServiceAsync(GetHttpRequestHeaders(isNonThailandProduct), queryParams);
        }

    }
}
