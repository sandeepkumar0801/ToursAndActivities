using Isango.Entities.Redeam;
using Logger.Contract;
using ServiceAdapters.Redeam.Constants;
using ServiceAdapters.Redeam.Redeam.Commands.Contracts;

using System;
using System.Threading.Tasks;

namespace ServiceAdapters.Redeam.Redeam.Commands
{
    public class GetRateCommandHandler : CommandHandlerBase, IGetRateCommandHandler
    {
        public GetRateCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to fetch a single rate
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
        /// Call to fetch a single rate asynchronously
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
            return $"{BaseAddress}{string.Format(UriConstants.GetRate, criteria?.SupplierId, criteria?.ProductId, criteria?.RateId)}";
        }

        #endregion Private Methods
    }
}