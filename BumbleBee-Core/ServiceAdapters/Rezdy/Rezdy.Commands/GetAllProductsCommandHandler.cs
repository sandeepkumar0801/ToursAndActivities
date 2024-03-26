using Logger.Contract;
using ServiceAdapters.Rezdy.Rezdy.Commands.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities;
using ServiceAdapters.Rezdy.Rezdy.Entities.ProductDetails;

namespace ServiceAdapters.Rezdy.Rezdy.Commands
{
    public class GetAllProductsCommandHandler : CommandHandlerBase, IGetAllProductsCommandHandler
    {
        public GetAllProductsCommandHandler(ILogger log) : base(log)
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
            //var input = inputContext as GetProductReqeust;
            //var methodPath = GenerateMethodPath(input);
            //var result = await _httpClient.GetAsync(methodPath);
            //return ValidateApiResponse(result);

            
            var retries = 0;
            var input = inputContext as GetProductReqeust;
            var methodPath = GenerateMethodPath(input);
            var maxRetry = 3;//try 3 times
            var totalWaitTime = 60000;// 1 minute
            while (retries++ < maxRetry)
            {
                try
                {
                    var result =  _httpClient.GetAsync(methodPath);
                    result.Wait();
                    return ValidateApiResponse(await result);
                }
                catch (Exception ex)
                {
                    // To many request (more than 100 RQT in 1 minute) 
                    //1.) 406 Not Acceptable 2.) Too Many Requests response 
                    if (ex.Message.Contains("429") || ex.Message.Contains("406"))
                    {
                        await Task.Delay(totalWaitTime);
                        continue;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        private string GenerateMethodPath(GetProductReqeust input)
        {
            if (!string.IsNullOrEmpty(input.SupplierAlias))
            {
                return string.Format(UriConstants.AllProductsForSupplierAlias, input.SupplierAlias);
            }
            else if (input.SupplierId != 0)
            {
                return string.Format(UriConstants.AllProductsForSupplierId, input.SupplierId);
            }
            else
            {
                return string.Format(UriConstants.AllProducts,input.Limit,input.Offset);
            }
        }
    }
}