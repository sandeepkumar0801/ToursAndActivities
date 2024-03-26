using Isango.Entities;
using Isango.Entities.RedeamV12;
using Logger.Contract;
using ServiceAdapters.RedeamV12.Constants;
using ServiceAdapters.RedeamV12.RedeamV12.Commands.Contracts;

using System;
using System.Threading.Tasks;

namespace ServiceAdapters.RedeamV12.RedeamV12.Commands
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
            var input = inputContext as CanocalizationCriteria;
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
            var input = inputContext as CanocalizationCriteria;
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
        private string GenerateMethodPath(CanocalizationCriteria criteria)
        {
            var startDate = criteria?.CheckinDate.ToString(Constant.DateTimeStringFormat);
            return $"{BaseAddress}{string.Format(UriConstants.GetAvailability, criteria?.SupplierId, criteria?.ProductId, startDate, criteria?.Quantity)}";
        }

        #endregion Private Methods
    }
}