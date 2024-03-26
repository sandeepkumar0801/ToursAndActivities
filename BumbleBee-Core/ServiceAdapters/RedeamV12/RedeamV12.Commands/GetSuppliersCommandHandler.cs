using Logger.Contract;
using ServiceAdapters.RedeamV12.Constants;
using ServiceAdapters.RedeamV12.RedeamV12.Commands.Contracts;

using System;
using System.Threading.Tasks;

namespace ServiceAdapters.RedeamV12.RedeamV12.Commands
{
    public class GetSuppliersCommandHandler : CommandHandlerBase, IGetSuppliersCommandHandler
    {
        public GetSuppliersCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to get the suppliers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object RedeamApiRequest<T>(T inputContext)
        {
            var methodPath = new Uri(GenerateMethodPath());

            var result = HttpClient.GetAsync(methodPath);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call to get the suppliers asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> RedeamApiRequestAsync<T>(T inputContext)
        {
            var methodPath = new Uri(GenerateMethodPath());

            var result = await HttpClient.GetAsync(methodPath);
            return ValidateApiResponse(result);
        }

        #region Private Methods

        /// <summary>
        /// Create the API endpoint url
        /// </summary>
        /// <returns></returns>
        private string GenerateMethodPath()
        {
            return $"{BaseAddress}{UriConstants.GetAllSuppliers}";
        }

        #endregion Private Methods
    }
}