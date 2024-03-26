using Isango.Entities;
using Isango.Entities.RedeamV12;
using Logger.Contract;
using ServiceAdapters.RedeamV12.Constants;
using ServiceAdapters.RedeamV12.RedeamV12.Commands.Contracts;

using System;
using System.Threading.Tasks;

namespace ServiceAdapters.RedeamV12.RedeamV12.Commands
{
    public class GetRatesCommandHandler : CommandHandlerBase, IGetRatesCommandHandler
    {
        public GetRatesCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to fetch rates
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
        /// Call to fetch rates asynchronously
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
            return $"{BaseAddress}{string.Format(UriConstants.GetRates, criteria?.SupplierId, criteria?.ProductId)}";
        }

        #endregion Private Methods
    }
}