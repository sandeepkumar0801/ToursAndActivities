using Isango.Entities.Tiqets;
using Logger.Contract;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;

namespace ServiceAdapters.Tiqets.Tiqets.Commands
{
    public class GetTicketCommandHandler : CommandHandlerBase, IGetTicketCommandHandler
    {
        public GetTicketCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object TiqetsBookingApiRequest<T>(T inputContext)
        {
            var bookingRequest = inputContext as BookingRequest;
            var confirmOrderResponse = bookingRequest?.RequestObject as ConfirmOrderResponse;
            var url = FormUrl(confirmOrderResponse?.OrderReferenceId, bookingRequest?.LanguageCode);
            using (var httpClient = AddRequestHeadersAndAddressToApi(bookingRequest?.AffiliateId))
            {
                var result = httpClient.GetAsync(url);
                result.Wait();
                return result.GetAwaiter().GetResult();
            }
        }

        private string FormUrl(string orderReferenceId, string languageCode)
        {
            return $"{UriConstant.ConfirmOrder}{orderReferenceId}{UriConstant.Tickets}{UriConstant.BookingLanguage}{languageCode}";
        }
    }
}