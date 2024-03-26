using Logger.Contract;
using ServiceAdapters.PrioHub.PrioHub.Converters.Contracts;
using ServiceAdapters.PrioHub.PrioHub.Entities.CancelReservationResponse;

namespace ServiceAdapters.PrioHub.PrioHub.Converters
{
    public class CancelReservationConverter : ConverterBase, ICancelReservationConverter
	{
		public CancelReservationConverter(ILogger logger) : base(logger)
		{
		}
		/// <summary>
		/// Convert
		/// </summary>
		/// <param name="objectResult"></param>
		/// <returns></returns>
		public override object Convert(object objectResult)
		{
			return ConvertCancelResult(objectResult);
		}

        /// <summary>
        /// Convert Cancel Result
        /// </summary>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        public Tuple<string, string, string, DateTime> ConvertCancelResult(object objectResult)
        {
            var cancelReservationRs = (CancelReservationResponse)objectResult;
            if (cancelReservationRs?.Data == null) return Tuple.Create(string.Empty, string.Empty, string.Empty, DateTime.Now);

            var status = cancelReservationRs?.Data?.Reservation?.ReservationDetails?.FirstOrDefault()?.Booking_status?.ToUpper();
            if (string.Equals(status, "BOOKING_RESERVATION_CANCELLED", StringComparison.CurrentCultureIgnoreCase))
            {
                var reservationRef = cancelReservationRs.Data?.Reservation?.ReservationReference;
                var bookingReservationRef = cancelReservationRs.Data?.Reservation?.ReservationDetails.FirstOrDefault().BookingReservationReference;
                var bookingCancelledTime = cancelReservationRs.Data?.Reservation?.ReservationDetails?.FirstOrDefault()?.Booking_cancelled;
                return Tuple.Create(reservationRef, bookingReservationRef, status,System.Convert.ToDateTime(bookingCancelledTime));
            }
            return Tuple.Create(string.Empty, string.Empty, string.Empty, DateTime.Now);
        }
    }
}