using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Commands.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities;
using ServiceAdapters.Bokun.Bokun.Entities.GetActivity;
using ServiceAdapters.Bokun.Constants;

namespace ServiceAdapters.Bokun.Bokun.Commands
{
    public class GetActivityCommandHandler : CommandHandlerBase, IGetActivityCommandHandler
    {
        public GetActivityCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Return request required to hit API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest<T>(T inputContext)
        {
            var factsheetId = inputContext as string;
            var requestObject = new GetActivityRq
            {
                ActivityId = System.Convert.ToInt32(factsheetId)
            };
            return requestObject;
        }

        /// <summary>
        /// Call API to Get Activity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object BokunApiRequest<T>(T inputContext)
        {
            var input = inputContext as GetActivityRq;
            var methodPath = GenerateMethodPath(input);
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Get, methodPath);

            var result = httpClient.GetAsync(methodPath)?.GetAwaiter().GetResult();
            //result.Wait();
            return ValidateApiResponse(result);
        }

        /// <summary>
        /// Call API to Get Activity asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> BokunApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as GetActivityRq;
            var methodPath = GenerateMethodPath(input);
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Get, methodPath);

            var result = await httpClient.GetAsync(methodPath);
            return ValidateApiResponse(result);
        }

        private string GenerateMethodPath(GetActivityRq input)
        {
            return string.Format(UriConstants.GetActivity, input.ActivityId);
        }
    }
}