using Isango.Entities.GoCity;
using Logger.Contract;
using ServiceAdapters.GoCity.Constants;
using ServiceAdapters.GoCity.GoCity.Commands.Contract;
using ServiceAdapters.GoCity.GoCity.Entities.Product;
using Util;
namespace ServiceAdapters.GoCity.GoCity.Commands
{
    public class ProductsCmdHandler : CommandHandlerBase, IProductsCommandHandler
    {
        public ProductsCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var goCityCriteria = criteria as GoCityCriteria;
            var activityRq = GetProductRequest(goCityCriteria);
            return activityRq;
        }

        /// <summary>
        /// Get Api response object to be passed to converter or dumping.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responseText"></param>
        /// <returns></returns>

        protected override object GetResponseObject(string responseText)
        {
            var result = default(ProductResponse);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<ProductResponse>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override  object GetResultsAsync(object input)
        {
            AddRequestHeadersAndAddressToApi();
            var url = Constant.Products+"?userId="+_goCityApiUserName;
            var result = _httpClient.GetAsync(url);
            result.Wait();
            return result.Result.Content.ReadAsStringAsync().Result;
        }
    }
}