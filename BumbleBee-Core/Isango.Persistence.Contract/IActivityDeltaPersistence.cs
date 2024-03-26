using Isango.Entities.Master;
using Isango.Entities.Review;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IActivityDeltaPersistence
    {
        List<Review> GetDeltaActivityReview();

        List<Entities.Booking.PassengerInfo> GetDeltaActivityPassengerInfo();

        List<ActivityIds> GetDeltaActivity();

        List<ActivityMinPrice> GetDeltaActivityPrice();

        List<ActivityAvailableDays> GetDeltaActivityAvailability();
    }
}