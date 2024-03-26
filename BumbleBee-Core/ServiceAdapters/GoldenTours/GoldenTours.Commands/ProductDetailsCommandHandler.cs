using Isango.Entities.GoldenTours;
using Logger.Contract;
using ServiceAdapters.GoldenTours.Constants;
using ServiceAdapters.GoldenTours.GoldenTours.Commands.Contracts;

namespace ServiceAdapters.GoldenTours.GoldenTours.Commands
{
    public class ProductDetailsCommandHandler : CommandHandlerBase, IProductDetailsCommandHandler
    {
        private readonly ILogger _log;
        public ProductDetailsCommandHandler(ILogger log) : base(log)
        {
            _log = log;
        }

        /// <summary>
        /// Call to get the product detail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object GoldenToursApiRequest<T>(T inputContext)
        {
            var input = inputContext as GoldenToursCriteria;
            var methodPath = new Uri(GenerateMethodPath(input));
            try
            {
                var result = _httpClient.GetAsync(methodPath);
                result.Wait();
                return ValidateApiResponse(result.Result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = "ProductDetailsCommandHandler",
                    MethodName = "GoldenToursApiRequest",
                    Params = Util.SerializeDeSerializeHelper.Serialize(inputContext)
                };
                _log.Error(isangoErrorEntity, ex);
                //timeout probably - check logs;
                return null;
            }
            
        }

        /// <summary>
        /// Call to get the product detail asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> GoldenToursApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as GoldenToursCriteria;
            var methodPath = new Uri(GenerateMethodPath(input));

            var result = await _httpClient.GetAsync(methodPath);
            return ValidateApiResponse(result);
        }

        #region Private Methods

        /// <summary>
        /// Create the API endpoint url
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private string GenerateMethodPath(GoldenToursCriteria criteria)
        {
            return $"{_baseAddress}{string.Format(UriConstants.ProductDetails, criteria?.SupplierOptionCode, _key, criteria?.CurrencyCode, criteria?.LanguageId)}";
        }

        #endregion Private Methods
    }
}