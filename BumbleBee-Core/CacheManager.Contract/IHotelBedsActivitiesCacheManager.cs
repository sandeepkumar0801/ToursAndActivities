using Isango.Entities.Availability;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IHotelBedsActivitiesCacheManager
    {
        string LoadAvailabilityCache(List<Availability> availabilityList);

        List<Availability> GetAvailability(string regionId, string productId);

        List<Availability> GetAllPriceAndAvailability();
    }
}