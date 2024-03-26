using Logger.Contract;
using ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters
{
	public class ReservationConverter : ConverterBase, IReservationConverter
	{
		public ReservationConverter(ILogger logger) : base(logger)
		{
		}
		public override object Convert(object objectResult)
		{
			return ConvertReservationResult(objectResult);
		}

        /// <summary>
        /// Convert Reservation Result
        /// </summary>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        public Tuple<string, string, string> ConvertReservationResult(object objectResult)
        {
            var reservationRs = (ReservationRs)objectResult;
            return Tuple.Create(reservationRs.Data.ReservationReference, reservationRs.Data.DistributorReference, reservationRs.Data.BookingStatus);
        }
    }
}