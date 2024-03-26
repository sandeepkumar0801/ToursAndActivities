using Logger.Contract;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceAdapters.CitySightSeeing.Constants;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities.Products;
using Util;
using System.Net;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands
{
    public class ProductsCmdHandler : CommandHandlerBase, IProductsCommandHandler
    {
        public ProductsCmdHandler(ILogger iLog) : base(iLog)
        {
        }
        protected override object CssApiRequest<T>(T inputContext, string token)
        {
            var result = HttpClient.GetAsync(UriConstants.Products);
            result.Wait();
            return result.Result;
        }

        protected override async Task<object> CssApiRequestAsync<T>(T inputContext)
        {
            var result = await HttpClient.GetAsync(UriConstants.Products);
            return result;
        }
    }
}
