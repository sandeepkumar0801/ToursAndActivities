using Logger.Contract;
using ServiceAdapters.RedeamV12.Constants;
using ServiceAdapters.RedeamV12.RedeamV12.Commands.Contracts;

using System;
using System.Threading.Tasks;

namespace ServiceAdapters.RedeamV12.RedeamV12.Commands
{
    public class CancelBookingCommandHandler : CommandHandlerBase, ICancelBookingCommandHandler
    {
        public CancelBookingCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to Cancel Booking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object RedeamApiRequest<T>(T inputContext)
        {
            var input = inputContext.ToString();
            var methodPath = new Uri(GenerateMethodPath(input));

            var result = HttpClient.PutAsync(methodPath, null);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call to Cancel Booking asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> RedeamApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext.ToString();
            var methodPath = new Uri(GenerateMethodPath(input));

            var result = await HttpClient.PutAsync(methodPath, null);
            return ValidateApiResponse(result);
        }

        #region Private Methods

        /// <summary>
        /// Create the API endpoint url
        /// </summary>
        /// <param name="bookingReferenceNumber"></param>
        /// <returns></returns>
        private string GenerateMethodPath(string bookingReferenceNumber)
        {
            return $"{BaseAddress}{string.Format(UriConstants.CancelBooking, bookingReferenceNumber)}";
        }

        #endregion Private Methods
    }
}