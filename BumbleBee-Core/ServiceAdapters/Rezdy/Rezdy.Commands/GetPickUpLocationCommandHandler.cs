using Logger.Contract;
using ServiceAdapters.Rezdy.Rezdy.Commands.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities;
using ServiceAdapters.Rezdy.Rezdy.Entities.PickUpLocation;

namespace ServiceAdapters.Rezdy.Rezdy.Commands
{
    public class GetPickUpLocationCommandHandler : CommandHandlerBase, IGetPickUpLocationCommandHandler
    {
        public GetPickUpLocationCommandHandler(ILogger log) : base(log)
        {

        }

        protected override object RezdyApiRequest<T>(T inputContext)
        {
            var input = inputContext as PickUpLocationRequest;
            var methodPath = GenerateMethodPath(input);
            var result = _httpClient.GetAsync(methodPath);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        protected override async Task<object> RezdyApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as PickUpLocationRequest;
            var methodPath = GenerateMethodPath(input);
            var result = await _httpClient.GetAsync(methodPath);
            return ValidateApiResponse(result);
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        private string GenerateMethodPath(PickUpLocationRequest input)
        {
            return string.Format(UriConstants.PickUpDetails, input.PickUpId);
        }


    }
}
