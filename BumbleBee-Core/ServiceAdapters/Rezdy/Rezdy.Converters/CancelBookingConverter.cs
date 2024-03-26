using ServiceAdapters.Rezdy.Rezdy.Converters.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities.CancelBooking;

using Util;

namespace ServiceAdapters.Rezdy.Rezdy.Converters
{
    public class CancelBookingConverter : ConverterBase, ICancelBookingConverter
    {
        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">Generic model for Cancel Booking Call</param>
        /// <param name="criteria">Generic request model</param>
        /// <returns></returns>
        public override object Convert(string objectResult)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<CancelBookingResponse>(objectResult.ToString());
            return result;
        }
    }
}