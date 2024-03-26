using Isango.Entities.Tiqets;
using Logger.Contract;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;

namespace ServiceAdapters.Tiqets.Tiqets.Commands
{
    public class GetProductDetailsCommandHandler : CommandHandlerBase, IGetProductDetailsCommandHandler
    {
        public GetProductDetailsCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object TiqetsApiRequest<T>(T inputContext)
        {
            var criteria = inputContext as TiqetsCriteria;
            using (var httpClient = AddRequestHeadersAndAddressToApi(criteria?.AffiliateId))
            {
                var result = httpClient.GetAsync(FormUrlAvailability(criteria));
                result.Wait();
                return result.Result;
            }
        }

        private string FormUrlAvailability(TiqetsCriteria criteria)
        {
            return $"{UriConstant.Products}{criteria.ProductId}{UriConstant.Language}{criteria.Language}";
        }
    }
}