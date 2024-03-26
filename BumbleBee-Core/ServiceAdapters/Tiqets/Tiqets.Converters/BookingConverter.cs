using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Tiqets;
using Logger.Contract;
using ServiceAdapters.Tiqets.Tiqets.Converters.Contracts;
using ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels;

namespace ServiceAdapters.Tiqets.Tiqets.Converters
{
    public class BookingConverter : ConverterBase, IBookingConverter
    {
        public BookingConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert<T>(T objectResult, object input)
        {
            var bookingResponse = objectResult as GetTicketResponse;
            var availabilityList = ConvertBookingResult(bookingResponse);
            return availabilityList;
        }

        #region "Private Methods"

        /// <summary>
        /// This method maps the API response to iSango Contracts objects.
        /// </summary>
        /// <returns></returns>
        private object ConvertBookingResult(GetTicketResponse getTicketResponse)
        {
            if (getTicketResponse != null)
            {
                try
                {
                    var booking = new Booking
                    {
                        ReferenceNumber = getTicketResponse.OrderReferenceId,
                        SelectedProducts = new List<SelectedProduct>
                    {
                        new TiqetsSelectedProduct
                        {
                            OrderStatus = getTicketResponse.TiqetsOrderStatus,
                            Success = getTicketResponse.Success,
                            TicketPdfUrl = getTicketResponse.TicketPdfUrl,
                            OrderReferenceId = getTicketResponse.OrderReferenceId
                        }
                    }
                    };

                    return booking;
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "Tiqets.BookingConverter",
                        MethodName = "ConvertBookingResult"
                    };
                    _logger.Error(isangoErrorEntity, ex);
                    throw; //use throw as existing flow should not break bcoz of logging implementation.
                }
            }
            return null;
        }

        #endregion "Private Methods"
    }
}