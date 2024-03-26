using Isango.Entities.Tiqets;
using Logger.Contract;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;
using BookingRequest = Isango.Entities.Tiqets.BookingRequest;

namespace ServiceAdapters.Tiqets.Tiqets.Commands
{
    public class OrderInfoCommandHandler : CommandHandlerBase, IOrderInformationCmdHandler
    {
        public OrderInfoCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object TiqetsBookingApiRequest<T>(T inputContext)
        {
            var bookingRequest = inputContext as BookingRequest;
            using (var httpClient = AddRequestHeadersAndAddressToApi(bookingRequest?.AffiliateId))
            {
                var result = httpClient.GetAsync(FormUrl(bookingRequest));
                result.Wait();
                return result.GetAwaiter().GetResult();
            }
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            var bookingRequest = inputContext as BookingRequest;
            return bookingRequest;
        }

        private string FormUrl(BookingRequest bookingRequest)
        {
            var tiqetsProduct = bookingRequest.RequestObject as TiqetsSelectedProduct;
            return $"{UriConstant.GetOrderInfo}{tiqetsProduct.OrderReferenceId}{UriConstant.BookingLanguage}{bookingRequest.LanguageCode}";
        }
    }
}
