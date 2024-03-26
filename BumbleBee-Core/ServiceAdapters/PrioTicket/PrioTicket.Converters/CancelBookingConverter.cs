using Logger.Contract;
using ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters
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
        public Tuple<string, string, string, DateTime> ConvertCancelResult(object objectResult)
        {
            var cancelBookingRs = (CancelBookingRs)objectResult;
            return cancelBookingRs?.Data?.BookingStatus.ToLower() == PrioApiStatus.Cancelled.ToLower()
                ? Tuple.Create(cancelBookingRs?.Data.BookingReference, cancelBookingRs?.Data.DistributorReference,
                    cancelBookingRs?.Data.BookingStatus, cancelBookingRs.Data.CancellationDateTime)
                : Tuple.Create(string.Empty, string.Empty, string.Empty, DateTime.Now);
        }
    }
}