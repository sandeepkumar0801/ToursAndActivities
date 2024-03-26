using Isango.Entities.NewCitySightSeeing;
using Logger.Contract;
using ServiceAdapters.NewCitySightSeeing.Constants;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands.Contract;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Booking;
using System.Text;
using Util;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands
{
    public class BookingCmdHandler : CommandHandlerBase, IBookingCommandHandler
    {
        public BookingCmdHandler(ILogger iLog) : base(iLog)
        {
        }
        protected override object CreateInputRequest<T>(T bookingContext)
        {
            var bookReservationRequest = new BookingRequest();
            try
            {
                var inputContext = bookingContext as ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.InputContext;
                var selectedProducts = inputContext.SelectedProducts;
                var IsangoRefNumber = inputContext?.BookingReference;
                var newCitySightSeeingSelectdProducts = (NewCitySightSeeingSelectedProduct)selectedProducts;

                bookReservationRequest.ExternalServiceRefCode = IsangoRefNumber;
                bookReservationRequest.ReservationId =Convert.ToString(newCitySightSeeingSelectdProducts?.NewCitySightSeeingReservationId);
            }
            catch (Exception)
            {
                throw;
            }
            return bookReservationRequest;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(BookingResponse);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override object GetResultsAsync(object input)
        {
            var url = $"{_newCitySightSeeingServiceURL}{Constant.BookReservation}";
            var bookReservationRQ = input as BookingRequest;
            if (bookReservationRQ == null)
            {
                return null;
            }
            var client = new AsyncClient
            {
                ServiceURL = url
            };
            return client.ConsumePostService(GetHttpRequestHeaders(), Constant.HttpHeader_ContentType_JSON, SerializeDeSerializeHelper.Serialize(bookReservationRQ), Encoding.UTF8);
        }

    }
}