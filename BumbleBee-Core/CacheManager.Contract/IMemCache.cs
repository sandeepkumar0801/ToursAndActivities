using Isango.Entities;
using Isango.Entities.HotelBeds;
using Isango.Entities.Region;
using Isango.Entities.Wrapper;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IMemCache
    {
        bool RegionCategoryMapping(CacheKey<RegionCategoryMapping> regionCategoryMappingList);

        bool LocalizedMerchandising(CacheKey<LocalizedMerchandising> localizedMerchandisingList);

        bool RegionDestinationMapping(CacheKey<MappedRegion> mappedRegionList);

        bool LanguageCodeMapping(CacheKey<MappedLanguage> mappedLanguagesList);

        bool LoadURLVsPageID(CacheKey<UrlPageIdMapping> UrlPageIdMappingList);

        List<MappedLanguage> GetMappedLanguage();

        List<MappedRegion> GetRegionDestinationMapping();
    }
}