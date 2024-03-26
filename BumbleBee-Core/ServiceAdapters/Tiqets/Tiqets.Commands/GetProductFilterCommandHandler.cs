using Isango.Entities.Tiqets;
using Logger.Contract;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;

namespace ServiceAdapters.Tiqets.Tiqets.Commands
{
    public class GetProductFilterCommandHandler : CommandHandlerBase, IGetProductFilterCommandHandler
    {
        private readonly ILogger _log;

        public GetProductFilterCommandHandler(ILogger log) : base(log)
        {
            _log = log;
        }

        protected override object TiqetsApiRequest<T>(T inputContext)
        {
            var availabilityCriteria = inputContext as TiqetsCriteria;
            using (var httpClient = AddRequestHeadersAndAddressToApi(availabilityCriteria?.AffiliateId))
            {
                var result = httpClient.GetAsync(FormUrlAvailability(availabilityCriteria));
                try
                {
                    result.Wait();
                    return result.Result;
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
                    {
                        ClassName = "GetProductFilterCommandHandler",
                        MethodName = "TiqetsApiRequest",
                        Params = Util.SerializeDeSerializeHelper.Serialize(inputContext)
                    };
                    _log.Error(isangoErrorEntity, ex);
                     return null;
                }
            }
        }

        private string FormUrlAvailability(TiqetsCriteria availabilityCriteria)
        {
            //https://api-tiqt-test.steq.it/v2/products?page_size=100
            int pageNumber = availabilityCriteria.PageNumber;
            var url = UriConstant.ProductsFilter+"?page="+pageNumber+"&page_size=100";
            return url;
        }
    }
}
