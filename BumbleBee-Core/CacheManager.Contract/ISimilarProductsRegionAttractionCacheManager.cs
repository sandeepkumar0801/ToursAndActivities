using Isango.Entities;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface ISimilarProductsRegionAttractionCacheManager
    {
        string RegionCategoryMappingProducts(List<RegionCategorySimilarProducts> regionCategoryList);
    }
}