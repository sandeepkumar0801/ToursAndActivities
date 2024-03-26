using ServiceAdapters.HB.HB.Converters.Contracts;
using ServiceAdapters.HB.HB.Entities;
using Booking = ServiceAdapters.HB.HB.Entities.Booking.BookingDetail;

namespace ServiceAdapters.HB.HB.Converters
{
    public class HBBookingCancelConverter : ConverterBase, IHbBookingCancelConverter
    {
        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            var result = (Booking.BookingDetailRs)apiResponse;
            return result;
        }
    }
}