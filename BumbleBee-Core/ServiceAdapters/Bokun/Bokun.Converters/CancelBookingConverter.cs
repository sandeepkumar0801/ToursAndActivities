using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Converters.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities.CancelBooking;
using Util;

namespace ServiceAdapters.Bokun.Bokun.Converters
{
    public class CancelBookingConverter : ConverterBase, ICancelBookingConverter
    {
        public CancelBookingConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">Generic model for Cancel Booking Call</param>
        /// <param name="criteria">Generic request model</param>
        /// <returns></returns>
        public object Convert<T>(T objectResult, T criteria)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<CancelBookingRs>(objectResult.ToString());
            return result;
        }
    }
}