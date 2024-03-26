using ServiceAdapters.HB.HB.Converters.Contracts;
using ServiceAdapters.HB.HB.Entities;
using Booking = ServiceAdapters.HB.HB.Entities.Booking.BookingDetail;

namespace ServiceAdapters.HB.HB.Converters
{
    public class HBGetBookingDetailConverter : ConverterBase, IHbGetBookingDetailConverter
    {
        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>
        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            var result = (Booking.BookingDetailRs)apiResponse;
            return result;
        }
    }
}