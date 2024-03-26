using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Commands.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities;
using ServiceAdapters.Bokun.Bokun.Entities.GetBooking;
using ServiceAdapters.Bokun.Constants;

namespace ServiceAdapters.Bokun.Bokun.Commands
{
    public class GetBookingCommandHandler : CommandHandlerBase, IGetBookingCommandHandler
    {
        public GetBookingCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call API to Get Booking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object BokunApiRequest<T>(T inputContext)
        {
            var input = inputContext as GetBookingRq;
            var methodPath = GenerateMethodPath(input);
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Get, methodPath);

            var result = httpClient.GetAsync(methodPath);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call API to Get Booking asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> BokunApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as GetBookingRq;
            var methodPath = GenerateMethodPath(input);
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Get, methodPath);

            var result = await httpClient.GetAsync(methodPath);
            return ValidateApiResponse(result);
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        private string GenerateMethodPath(GetBookingRq input)
        {
            return string.Format(UriConstants.GetConfirmedBooking, input.BookingId);
        }
    }
}