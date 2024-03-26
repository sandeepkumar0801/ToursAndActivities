using Isango.Entities.GoldenTours;
using Logger.Contract;
using ServiceAdapters.GoldenTours.Constants;
using ServiceAdapters.GoldenTours.GoldenTours.Commands.Contracts;

namespace ServiceAdapters.GoldenTours.GoldenTours.Commands
{
    public class AvailabilityCommandHandler : CommandHandlerBase, IAvailabilityCommandHandler
    {
        public AvailabilityCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to get the availability
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object GoldenToursApiRequest<T>(T inputContext)
        {
            var input = inputContext as GoldenToursCriteria;
            var methodPath = new Uri(GenerateMethodPath(input));

            var result = _httpClient.GetAsync(methodPath);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call to get the availability asynchronously
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
            var checkInDate = criteria?.CheckinDate;
            return $"{_baseAddress}{string.Format(UriConstants.Availability, criteria?.SupplierOptionCode, _key, checkInDate?.Day, checkInDate?.Month, checkInDate?.Year)}";
        }

        #endregion Private Methods
    }
}