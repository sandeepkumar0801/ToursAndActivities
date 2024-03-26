using Isango.Entities.NewCitySightSeeing;
using Logger.Contract;
using ServiceAdapters.NewCitySightSeeing.Constants;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands.Contract;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Product;
using Util;
namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands
{
    public class ProductsCmdHandler : CommandHandlerBase, IProductsCommandHandler
    {
        public ProductsCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var citySightCriteria = criteria as NewCitySightSeeingCriteria;
            var activityRq = GetProductRequest(citySightCriteria);
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
            var result = default(List<Products>);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<List<Products>>(responseText);
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
            var url = $"{_newCitySightSeeingServiceURL}{Constant.Products}";
            IDictionary<string, string> queryParams = input as IDictionary<string, string>;

            var client = new AsyncClient
            {
                ServiceURL = url
            };
            return client.ConsumeGetService(GetHttpRequestHeaders(), queryParams);
            
        }
    }
}