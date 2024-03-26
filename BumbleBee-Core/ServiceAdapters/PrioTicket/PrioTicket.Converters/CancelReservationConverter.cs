using Logger.Contract;
using ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters
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
            var cancelReservationRs = (CancelReservationRs)objectResult;
            if (cancelReservationRs?.Data == null) return Tuple.Create(string.Empty, string.Empty, string.Empty, DateTime.Now);

            if (string.Equals(cancelReservationRs?.Data?.BookingStatus, PrioApiStatus.Cancelled, StringComparison.CurrentCultureIgnoreCase))
            {
                return Tuple.Create(cancelReservationRs.Data.ReservationReference, cancelReservationRs.Data.ReservationReference, cancelReservationRs.Data.BookingStatus, cancelReservationRs.Data.CancellationDateTime);
            }
            return Tuple.Create(string.Empty, string.Empty, string.Empty, DateTime.Now);
        }
    }
}