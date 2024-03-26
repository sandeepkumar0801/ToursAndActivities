using Logger.Contract;
using ServiceAdapters.PrioHub.PrioHub.Converters.Contracts;
using ServiceAdapters.PrioHub.PrioHub.Entities.CancelBookingResponse;

namespace ServiceAdapters.PrioHub.PrioHub.Converters
{
    public class CancelBookingConverter : ConverterBase, ICancelBookingConverter
	{
		public CancelBookingConverter(ILogger logger) : base(logger)
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
        public Tuple<string, string, string,string,string, DateTime> ConvertCancelResult(object objectResult)
        {
            var cancelBookingRs = (CancelBookingResponse)objectResult;
            var statusOuter = cancelBookingRs?.Data?.Order?.OrderStatus?.ToUpper();
            var statusInner = cancelBookingRs?.Data?.Order?.Order_bookings[0]?.Booking_status?.ToUpper();
            var orderRef = cancelBookingRs?.Data?.Order?.OrderReference;
            var orderExtRef = cancelBookingRs?.Data?.Order?.OrderExternalReference;
            var orderCancelDateTime = cancelBookingRs?.Data?.Order?.OrderCancellationDateTime;
            var error = cancelBookingRs?.Error;
            var errorMessage = cancelBookingRs?.ErrorMessage;
            var finalStatus = statusOuter;
            if (!string.IsNullOrEmpty(statusInner))
            {
                if (statusInner == Entities.ConstantPrioHub.BOOKINGPROCESSINGCANCELLATION && statusOuter == Entities.ConstantPrioHub.ORDERCONFIRMED)
                {
                    finalStatus = Entities.ConstantPrioHub.BOOKINGPROCESSINGCANCELLATION;
                }
                
            }
             var returnfinalStatus = (finalStatus== Entities.ConstantPrioHub.ORDERCANCELLED) || (finalStatus== Entities.ConstantPrioHub.BOOKINGPROCESSINGCANCELLATION)
                ? Tuple.Create
                (orderRef, orderRef, finalStatus, error, errorMessage, System.Convert.ToDateTime(orderCancelDateTime))
                : Tuple.Create
                (string.Empty, string.Empty, string.Empty, error, errorMessage, DateTime.Now);

            return returnfinalStatus;
        }
    }
}