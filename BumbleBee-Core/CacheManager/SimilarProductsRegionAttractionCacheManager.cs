using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using System.Collections.Generic;
using System.Text;
using Util;

namespace CacheManager
{
    public class SimilarProductsRegionAttractionCacheManager : ISimilarProductsRegionAttractionCacheManager
    {
        private readonly CollectionDataFactory<RegionCategorySimilarProducts> _collectionData;

        public SimilarProductsRegionAttractionCacheManager(CollectionDataFactory<RegionCategorySimilarProducts> cosmosHelper)
        {
            _collectionData = cosmosHelper;
        }

        public string RegionCategoryMappingProducts(List<RegionCategorySimilarProducts> regionCategoryList)
        {
            if (_collectionData.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("RegionCategoryMappingCollection")).Result)
            {
                _collectionData.GetCollectionDataHelper().DeleteCollection(ConfigurationManagerHelper.GetValuefromAppSettings("RegionCategoryMappingCollection")).Wait();
            }

            if (_collectionData.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("RegionCategoryMappingCollection"), Constant.RegionCategoryMappingCollectionPartitionKey).Result)
            {
                return InsertDocuments(regionCategoryList);
            }

            return string.Empty;
        }

        private string InsertDocuments(List<RegionCategorySimilarProducts> regionCategoryList)
        {
            var sb = new StringBuilder();
            foreach (var regionCategory in regionCategoryList)
            {
                if (!_collectionData.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("RegionCategoryMappingCollection"), regionCategory).Result)
                {
                    sb.Append(sb.Length == 0 ? regionCategory.RegionId.ToString() : "," + regionCategory.RegionId);
                }
            }
            return sb.ToString();
        }
    }
}