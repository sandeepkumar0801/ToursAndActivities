using Logger.Contract;
using ServiceAdapters.PrioHub.PrioHub.Commands.Contract;
using ServiceAdapters.PrioHub.PrioHub.Entities.ProductDetailResponse;
using Util;
namespace ServiceAdapters.PrioHub.PrioHub.Commands
{
    public class ProductDetailCmdHandler : CommandHandlerBase, IProductDetailCommandHandler
    {
        public ProductDetailCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var newPrioCriteria = criteria as Tuple<string,int>;
            return newPrioCriteria;
        }
        protected override object GetResponseObject(string responseText)
        {
            var result = default(ProductDetailResponse);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<ProductDetailResponse>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }
        protected override object GetResultsAsync(object input)
        {
            var newPrioCriteria = input as Tuple<string,int>;
            var scope = _PrioHubApiScopeProducts;
            //1. Using basic Auth Get AccessToken 
            var accessToken = AddRequestHeadersAndAddressToApi(scope, newPrioCriteria.Item2);
            //products/2433?distributor_id=501&cache=false
            var url = newPrioCriteria?.Item1;


            IDictionary<string, string> queryParams = input as IDictionary<string, string>;
            var client = new AsyncClient
            {
                ServiceURL = url
            };
            //2. Using Bearer"
            return client.ConsumeGetServicePrioHub(GetHttpRequestHeaders(accessToken), queryParams);
        }
      }
}