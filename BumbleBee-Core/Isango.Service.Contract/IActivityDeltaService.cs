using Isango.Entities.Master;
using Isango.Entities.Review;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IActivityDeltaService
    {
        Task<List<Review>> GetDeltaReviewAsync();

        Task<List<Entities.Booking.PassengerInfo>> GetDeltaPassengerInfoAsync();

        Task<List<ActivityIds>> GetDeltaActivityAsync();

        Task<List<ActivityMinPrice>> GetDeltaActivityPriceAsync();

        Task<List<ActivityAvailableDays>> GetDeltaActivityAvailableAsync();
    }
}
