using Isango.Entities;
using Logger.Contract;
using ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts;
using System;
using PrioResponse = ServiceAdapters.PrioTicket.PrioTicket.Entities.Response;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters
{
	public class CreateBookingConverter : ConverterBase, ICreateBookingConverter
	{
		public CreateBookingConverter(ILogger logger) : base(logger)
		{
		}
		// Converter is used to convert Object to the Activity class
		public override object Convert(object objectResult)
		{
			return ConverBookingDetaislResult(objectResult);
		}

		/// <summary>
		/// Convert Booking Details Result
		/// </summary>
		/// <param name="objectResult"></param>
		/// <returns></returns>
		private object ConverBookingDetaislResult(object objectResult)
		{
			var prioApiConfirmedBooking = new PrioApi();
			var createBookingRs = (PrioResponse.CreateBookingRs)objectResult;
			try
			{
				if (createBookingRs.Data != null && string.IsNullOrEmpty(createBookingRs.Data?.ErrorCode))
				{
					prioApiConfirmedBooking.DistributorReference = createBookingRs.Data.DistributorReference;
					prioApiConfirmedBooking.BookingReference = createBookingRs.Data.BookingReference;
					prioApiConfirmedBooking.BookingStatus = createBookingRs.Data.BookingStatus;
					prioApiConfirmedBooking.BookingDetails = new BookingDetails[1];
					if (createBookingRs.Data?.BookingDetails != null && createBookingRs.Data.BookingDetails.Length > 0)
					{
						var bookingDetail = createBookingRs.Data.BookingDetails[0];
						prioApiConfirmedBooking.BookingDetails[0] = new BookingDetails
						{
							VenueName = bookingDetail.VenueName,
							CodeType = bookingDetail.CodeType,
							GroupCode = bookingDetail.GroupCode
						};
                        // QR Code Per Order
                        if (!string.IsNullOrEmpty(prioApiConfirmedBooking.BookingDetails[0].GroupCode))
                        {
                            return prioApiConfirmedBooking;
                        }
                        else// QR Code Per Pax
                        {
                            var ticketLength = bookingDetail.TicketDetails.Length;
                            prioApiConfirmedBooking.BookingDetails[0].TicketDetails = new TicketDetails[ticketLength];
                            for (var ticket = ticketLength - 1; ticket >= 0; ticket--)
                            {
                                prioApiConfirmedBooking.BookingDetails[0].TicketDetails[ticket] = new TicketDetails
                                {
                                    TicketCode = bookingDetail.TicketDetails[ticket].TicketCode,
                                    TicketName = bookingDetail.TicketDetails[ticket].TicketName,
                                    TicketType = bookingDetail.TicketDetails[ticket].TicketType
                                };
                            }
                        }
					}
				}
				else
				{
					if (createBookingRs.Data == null) return prioApiConfirmedBooking;
					prioApiConfirmedBooking.ErrorCode = createBookingRs.Data.ErrorCode;
					prioApiConfirmedBooking.ErrorMessage = createBookingRs.Data.ErrorMessage;
				}
				return prioApiConfirmedBooking;
			}
			catch(Exception ex)
			{
				var isangoErrorEntity = new IsangoErrorEntity
				{
					ClassName = "Prio.CreateBookingConverter",
					MethodName = "ConverBookingDetailsResult"
				};
				_logger.Error(isangoErrorEntity, ex);
				throw; //use throw as existing flow should not break bcoz of logging implementation.
			}
		}
	}
}