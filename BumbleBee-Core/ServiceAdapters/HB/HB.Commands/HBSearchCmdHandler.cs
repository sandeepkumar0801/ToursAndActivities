using Logger.Contract;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Commands.Contract;
using ServiceAdapters.HB.HB.Entities;
using System;
using System.Threading.Tasks;
using Util;
using Search = ServiceAdapters.HB.HB.Entities.Search;

namespace ServiceAdapters.HB.HB.Commands
{
    public class HBSearchCmdHandler : CommandHandlerBase, IHbSearchCmdHandler
    {
        public HBSearchCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T hotelbedsApitudeCriteria)
        {
            var inputContext = hotelbedsApitudeCriteria as InputContext;
            var searchRq = new Search.SearchRq(inputContext.Filters, inputContext.FromDate, inputContext.ToDate, inputContext.Language, inputContext.Pagination, inputContext.Order);
            return searchRq;
        }

        protected override object GetResponseObject(string responseText)
        {
            throw new NotImplementedException();
        }

        protected override async Task<object> GetResultsAsync(object input)
        {
            if (input != null)
            {
                var client = new AsyncClient
                {
                    ServiceURL = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.EndpointSearch))
                };
                return await client.PostJsonAsyncWithoutAuth((Search.SearchRq)input);
            }
            return null;
        }
    }
}