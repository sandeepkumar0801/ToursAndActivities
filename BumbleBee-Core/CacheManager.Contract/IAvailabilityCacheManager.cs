using Isango.Entities.Availability;
using Isango.Entities.Wrapper;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IAvailabilityCacheManager
    {
        List<Availability> GetRegionAvailabilities(string regionId);
        List<Availability> LoadAvailabilityCache(string regionId, string cityId);
        void SetIsangoAvailability(CacheKey<Availability> availabilities);
    }
}
