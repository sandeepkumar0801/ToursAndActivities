using Logger.Contract;
using ServiceAdapters.GoldenTours.Constants;
using ServiceAdapters.GoldenTours.GoldenTours.Commands.Contracts;

namespace ServiceAdapters.GoldenTours.GoldenTours.Commands
{
    public class PickupPointsCommandHandler : CommandHandlerBase, IPickupPointsCommandHandler
    {
        public PickupPointsCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to get the pickup points
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object GoldenToursApiRequest<T>(T inputContext)
        {
            var input = inputContext as string;
            var methodPath = new Uri(GenerateMethodPath(input));

            var result = _httpClient.GetAsync(methodPath);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call to get the pickup points asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> GoldenToursApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as string;
            var methodPath = new Uri(GenerateMethodPath(input));

            var result = await _httpClient.GetAsync(methodPath);
            return ValidateApiResponse(result);
        }

        #region Private Methods

        /// <summary>
        /// Create the API endpoint url
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        private string GenerateMethodPath(string productId)
        {
            return $"{_baseAddress}{string.Format(UriConstants.PickupPoints, productId, _key)}";
        }

        #endregion Private Methods
    }
}