using Isango.Entities.GoldenTours;
using Logger.Contract;
using ServiceAdapters.GoldenTours.Constants;
using ServiceAdapters.GoldenTours.GoldenTours.Commands.Contracts;
using System.Globalization;

namespace ServiceAdapters.GoldenTours.GoldenTours.Commands
{
    public class GetBookingDatesCommandHandler : CommandHandlerBase, IGetBookingDatesCommandHandler
    {
        public GetBookingDatesCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to get the booking dates
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
        /// Call to get the booking dates asynchronously
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
            var checkinDate = criteria?.CheckinDate.ToString(Constant.DDMMYYYY, CultureInfo.InvariantCulture);
            var checkoutDate = criteria?.CheckoutDate.ToString(Constant.DDMMYYYY, CultureInfo.InvariantCulture);

            return $"{_baseAddress}{string.Format(UriConstants.GetBookingDates, criteria?.SupplierOptionCode, _key, criteria?.Status, checkinDate, checkoutDate)}";
        }

        #endregion Private Methods
    }
}