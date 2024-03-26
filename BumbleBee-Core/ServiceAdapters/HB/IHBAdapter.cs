using Isango.Entities.HotelBeds;
using ServiceAdapters.HB.HB.Entities.ActivityDetailFull;
using ServiceAdapters.HB.HB.Entities.Booking;
using ServiceAdapters.HB.HB.Entities.Calendar;
using ServiceAdapters.HB.HB.Entities.Cancellation;
using ServiceAdapters.HB.HB.Entities.ContentMulti;

namespace ServiceAdapters.HB
{
    public interface IHBAdapter
    {
        Task<object> SearchAsync(HotelbedCriteriaApitude hotelbedCriteria, string token);

        Task<Isango.Entities.Activities.Activity> ActivityDetailsAsync(HotelbedCriteriaApitude hotelbedCriteria, string token);

        Task<ActivityDetailFullRS> ActivityDetailsFullAsync(HotelbedCriteriaApitude hotelbedCriteria, string token);

        List<HotelBedsSelectedProduct> BookingConfirm(Isango.Entities.Booking.Booking booking, string token, out string request, out string response);

        List<HotelBedsSelectedProduct> GetBookingDetailAsync(BookingRq getBookingRq, string token, out string request, out string response);

        CancellationRS BookingCancel(string referenceNumber, string language, string token, out string request, out string response);

        CancellationRS BookingCancelSimulation(string referenceNumber, string language, string token, out string request, out string response);

        Task<Tuple<List<Isango.Entities.Activities.Activity>, CalendarRs>> CalendarAsync(HotelbedCriteriaApitudeFilter hotelbedCriteriaApitude, string token);

        Task<ContentMultiRS> ContentMultiAsync(ContentMultiRq hotelbedCriteriaApitude, string token);
    }
}