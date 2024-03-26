using Isango.Entities;
using Isango.Entities.PrioHub;
using Logger.Contract;
using ServiceAdapters.PrioHub.PrioHub.Converters.Contracts;
using ServiceAdapters.PrioHub.PrioHub.Entities.CreateBookingResponse;
using ServiceAdapters.PrioHub.PrioHub.Entities.ErrorRes;
using Util;

namespace ServiceAdapters.PrioHub.PrioHub.Converters
{
    public class CreateBookingConverter : ConverterBase, ICreateBookingConverter
    {
        public CreateBookingConverter(ILogger logger) : base(logger)
        {
        }

        // Converter is used to convert Object to the Activity class
        public override object Convert(object objectResult)
        {
            if (objectResult == null)
            {
                return null;
            }

            return ConverBookingDetaislResult(objectResult);
        }

        /// <summary>
        /// Convert Booking Details Result
        /// </summary>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        private object ConverBookingDetaislResult(object objectResult)
        {
            var prioApiConfirmedBooking = new PrioHubAPITicket();
            var createBookingRs = new CreateBookingResponse();
            var createBookingRsError = new ErrorRes();
            string checkSuccessorError = (string)objectResult;

            if (checkSuccessorError.Contains("error"))
            {
                createBookingRsError = SerializeDeSerializeHelper.DeSerialize<ErrorRes>(objectResult?.ToString());
            }
            else
            {
                createBookingRs = SerializeDeSerializeHelper.DeSerialize<CreateBookingResponse>(objectResult?.ToString());
            }

            try
            {
                if (createBookingRs != null && createBookingRs.Data != null)
                {
                    prioApiConfirmedBooking.DistributorReference = createBookingRs?.Data?.Order?.OrderBookings?.FirstOrDefault()?.BookingReference;
                    prioApiConfirmedBooking.BookingReference = createBookingRs?.Data?.Order?.OrderReference;
                    prioApiConfirmedBooking.DistributorId = createBookingRs?.Data?.Order?.OrderDistributorId;
                    prioApiConfirmedBooking.BookingStatus = createBookingRs?.Data?.Order?.OrderBookings?.FirstOrDefault()?.BookingStatus;
                    prioApiConfirmedBooking.BookingDetails = new Isango.Entities.PrioHub.BookingDetails[1];

                    if (createBookingRs.Data?.Order?.OrderBookings != null && createBookingRs.Data?.Order?.OrderBookings?.Count > 0)
                    {
                        var bookingDetail = createBookingRs.Data?.Order?.OrderBookings[0];
                        prioApiConfirmedBooking.BookingDetails[0] = new Isango.Entities.PrioHub.BookingDetails
                        {
                            VenueName = bookingDetail?.ProductTitle,
                            CodeType = bookingDetail?.ProductCodeSettings?.ProductCodeFormat,
                            GroupCode = bookingDetail?.BookingGroupCode
                        };
                        // QR Code Per Order
                        if (!string.IsNullOrEmpty(prioApiConfirmedBooking.BookingDetails[0].GroupCode))
                        {
                            return prioApiConfirmedBooking;
                        }
                        else// QR Code Per Pax
                        {
                            var ticketLength = bookingDetail.ProductTypeDetails.Count;
                            prioApiConfirmedBooking.BookingDetails[0].TicketDetails = new Isango.Entities.PrioHub.TicketDetails[ticketLength];
                            for (var ticket = ticketLength - 1; ticket >= 0; ticket--)
                            {
                                prioApiConfirmedBooking.BookingDetails[0].TicketDetails[ticket] = new Isango.Entities.PrioHub.TicketDetails
                                {
                                    TicketCode = bookingDetail?.ProductTypeDetails[ticket]?.ProductTypeCode,
                                    TicketName = bookingDetail?.ProductTitle,
                                    TicketType = bookingDetail?.ProductTypeDetails[ticket]?.ProductType
                                };
                            }
                        }
                    }
                }
                else
                {
                    prioApiConfirmedBooking.ErrorCode = createBookingRsError?.Error;
                    prioApiConfirmedBooking.ErrorMessage = createBookingRsError?.ErrorDescription;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "PrioHub.CreateBookingConverter",
                    MethodName = "ConverBookingDetailsResult"
                };
                _logger.Error(isangoErrorEntity, ex);
                throw; //use throw as existing flow should not break bcoz of logging implementation.
            }
            return prioApiConfirmedBooking;
        }
    }
}