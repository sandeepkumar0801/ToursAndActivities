using Isango.Entities.Redeam;
using Logger.Contract;
using ServiceAdapters.Redeam.Constants;
using ServiceAdapters.Redeam.Redeam.Commands.Contracts;

using System;
using System.Threading.Tasks;

namespace ServiceAdapters.Redeam.Redeam.Commands
{
    public class GetAvailabilityCommandHandler : CommandHandlerBase, IGetAvailabilityCommandHandler
    {
        public GetAvailabilityCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to fetch single availability
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object RedeamApiRequest<T>(T inputContext)
        {
            var input = inputContext as RedeamCriteria;
            var methodPath = new Uri(GenerateMethodPath(input));

            var result = HttpClient.GetAsync(methodPath);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call to fetch single availability asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> RedeamApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as RedeamCriteria;
            var methodPath = new Uri(GenerateMethodPath(input));

            var result = await HttpClient.GetAsync(methodPath);
            return ValidateApiResponse(result);
        }

        #region Private Methods

        /// <summary>
        /// Create the API endpoint url
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private string GenerateMethodPath(RedeamCriteria criteria)
        {
            var startDate = criteria?.CheckinDate.ToString(Constant.DateTimeStringFormat);
            return $"{BaseAddress}{string.Format(UriConstants.GetAvailability, criteria?.SupplierId, criteria?.ProductId, startDate, criteria?.Quantity)}";
        }

        #endregion Private Methods
    }
}