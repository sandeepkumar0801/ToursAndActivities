using Logger.Contract;
using ServiceAdapters.Rezdy.Rezdy.Commands.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities;
using ServiceAdapters.Rezdy.Rezdy.Entities.ProductDetails;

namespace ServiceAdapters.Rezdy.Rezdy.Commands
{
    public class GetProductCommandHandler : CommandHandlerBase, IGetProductCommandHandler
    {
        public GetProductCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object RezdyApiRequest<T>(T inputContext)
        {
            var input = inputContext as GetProductReqeust;
            var methodPath = GenerateMethodPath(input);
            var result = _httpClient.GetAsync(methodPath);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        protected override async Task<object> RezdyApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as GetProductReqeust;
            var methodPath = GenerateMethodPath(input);
            var result = await _httpClient.GetAsync(methodPath);
            return ValidateApiResponse(result);
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        private string GenerateMethodPath(GetProductReqeust input)
        {
            return string.Format(UriConstants.Products, input.ProductCode);
        }
    }
}