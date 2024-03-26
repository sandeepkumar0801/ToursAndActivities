using Logger.Contract;
using ServiceAdapters.Rezdy.Constants;
using ServiceAdapters.Rezdy.Rezdy.Commands.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities;
using ServiceAdapters.Rezdy.Rezdy.Entities.CancelBooking;
using System.Text;

using Util;

namespace ServiceAdapters.Rezdy.Rezdy.Commands
{
    public class CancelBookingCommandHandler : CommandHandlerBase, ICancelBookingCommandHandler
    {
        public CancelBookingCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object RezdyApiRequest<T>(T inputContext)
        {
            var input = inputContext as CancelBookingRequest;
            var methodPath = GenerateMethodPath(input);
            var content = new StringContent(SerializeDeSerializeHelper.Serialize(inputContext), Encoding.UTF8, Constant.ApplicationJson);
            var result = _httpClient.DeleteAsync(methodPath);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        protected override async Task<object> RezdyApiRequestAsync<T>(T inputContext)
        {
            return await Task.FromResult<object>(null);
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        private string GenerateMethodPath(CancelBookingRequest inputContext)
        {
            return string.Format(UriConstants.CancelBooking, inputContext.OrderNumber);
        }
    }
}