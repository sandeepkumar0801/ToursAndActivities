using Logger.Contract;
using ServiceAdapters.RedeamV12.Constants;
using ServiceAdapters.RedeamV12.RedeamV12.Commands.Contracts;

using System;
using System.Threading.Tasks;

namespace ServiceAdapters.RedeamV12.RedeamV12.Commands
{
    public class DeleteHoldCommandHandler : CommandHandlerBase, IDeleteHoldCommandHandler
    {
        public DeleteHoldCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to delete a hold
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object RedeamApiRequest<T>(T inputContext)
        {
            var holdId = inputContext as string;
            var methodPath = new Uri(GenerateMethodPath(holdId));

            var result = HttpClient.DeleteAsync(methodPath);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call to delete a hold asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> RedeamApiRequestAsync<T>(T inputContext)
        {
            var holdId = inputContext as string;
            var methodPath = new Uri(GenerateMethodPath(holdId));

            var result = await HttpClient.DeleteAsync(methodPath);
            return ValidateApiResponse(result);
        }

        #region Private Methods

        /// <summary>
        /// Create the API endpoint url
        /// </summary>
        /// <param name="holdId"></param>
        /// <returns></returns>
        private string GenerateMethodPath(string holdId)
        {
            return $"{BaseAddress}{string.Format(UriConstants.DeleteHold, holdId)}";
        }

        #endregion Private Methods
    }
}