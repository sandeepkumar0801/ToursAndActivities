using Logger.Contract;
using ServiceAdapters.PrioHub.PrioHub.Converters.Contracts;
using ServiceAdapters.PrioHub.PrioHub.Entities.ReservationResponse;
using Util;

namespace ServiceAdapters.PrioHub.PrioHub.Converters
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
        public Tuple<string, string, string,string> ConvertReservationResult(object objectResult)
        {
            
            var reservationRs = SerializeDeSerializeHelper.DeSerialize<ReservationResponse>(objectResult.ToString());
            if (reservationRs != null && reservationRs.Data!=null)
            {
                return Tuple.Create(reservationRs?.Data.Reservation.ReservationReference,
                    reservationRs?.Data?.Reservation?.ReservationDetails?.FirstOrDefault()?.BookingReservationReference,
                    reservationRs?.Data?.Reservation?.ReservationDetails?.FirstOrDefault()?.BookingStatus,
                    reservationRs?.Data?.Reservation?.ReservationDistributorId);
            }
            else
            {
                return null;
            }

        }
    }
}