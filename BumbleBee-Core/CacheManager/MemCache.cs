using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.HotelBeds;
using Isango.Entities.Region;
using Isango.Entities.Wrapper;
using MongoDB.Driver;
using System.Collections.Generic;
using Util;

namespace CacheManager
{
    public class MemCache : IMemCache
    {
        #region Variable declaration

        private readonly CollectionDataFactory<CacheKey<RegionCategoryMapping>> _collectionDataFactoryRegionCategory;
        private readonly CollectionDataFactory<CacheKey<LocalizedMerchandising>> _collectionDataFactorylocalized;
        private readonly CollectionDataFactory<CacheKey<MappedRegion>> _collectionDataFactoryMappedRegion;
        private readonly CollectionDataFactory<CacheKey<MappedLanguage>> _collectionDataFactoryMappedLanguage;
        private readonly CollectionDataFactory<CacheKey<UrlPageIdMapping>> _collectionDataFactoryUrlPageId;

        #endregion Variable declaration

        public MemCache(CollectionDataFactory<CacheKey<RegionCategoryMapping>> cosmosHelperRegionCategory,
            CollectionDataFactory<CacheKey<LocalizedMerchandising>> cosmosHelperlocalized,
            CollectionDataFactory<CacheKey<MappedRegion>> cosmosHelperMappedRegion,
            CollectionDataFactory<CacheKey<MappedLanguage>> cosmosHelperMappedLanguage,
            CollectionDataFactory<CacheKey<UrlPageIdMapping>> cosmosHelperUrlPageId)
        {
            _collectionDataFactoryRegionCategory = cosmosHelperRegionCategory;
            _collectionDataFactorylocalized = cosmosHelperlocalized;
            _collectionDataFactoryMappedRegion = cosmosHelperMappedRegion;
            _collectionDataFactoryMappedLanguage = cosmosHelperMappedLanguage;
            _collectionDataFactoryUrlPageId = cosmosHelperUrlPageId;
        }

        /// <summary>
        /// Load Region Category Mapping
        /// </summary>
        public bool RegionCategoryMapping(CacheKey<RegionCategoryMapping> regionCategoryMappingList)
        {
            var configuraton = ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection");
            CheckIfMasterDataCollectionExists(_collectionDataFactoryRegionCategory);
            if (_collectionDataFactoryRegionCategory.GetCollectionDataHelper().CheckIfDocumentExist(configuraton, regionCategoryMappingList.Id).Result)
            {
                return _collectionDataFactoryRegionCategory.GetCollectionDataHelper().UpdateDocument(configuraton, regionCategoryMappingList).Result;
            }
            else
            {
                if (!_collectionDataFactoryRegionCategory.GetCollectionDataHelper().InsertDocument(configuraton, regionCategoryMappingList).Result)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Load Localized Merchandising
        /// </summary>
        public bool LocalizedMerchandising(CacheKey<LocalizedMerchandising> localizedMerchandisingList)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactoryRegionCategory);
            if (_collectionDataFactorylocalized.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), localizedMerchandisingList.Id).Result)
            {
                return _collectionDataFactorylocalized.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), localizedMerchandisingList).Result;
            }
            else
            {
                if (!_collectionDataFactorylocalized.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), localizedMerchandisingList).Result)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Load Region Destination Mapping
        /// </summary>
        public bool RegionDestinationMapping(CacheKey<MappedRegion> mappedRegionList)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactoryRegionCategory);
            if (_collectionDataFactoryMappedRegion.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), mappedRegionList.Id).Result)
            {
                return _collectionDataFactoryMappedRegion.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), mappedRegionList).Result;
            }
            else
            {
                if (!_collectionDataFactoryMappedRegion.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), mappedRegionList).Result)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Load Mapped Language list
        /// </summary>
        public bool LanguageCodeMapping(CacheKey<MappedLanguage> mappedLanguagesList)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactoryRegionCategory);
            if (_collectionDataFactoryMappedLanguage.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), mappedLanguagesList.Id).Result)
            {
                return _collectionDataFactoryMappedLanguage.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), mappedLanguagesList).Result;
            }
            else
            {
                if (!_collectionDataFactoryMappedLanguage.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), mappedLanguagesList).Result)
                {
                    return false;
                }
            }

            return true;
        }

        public bool LoadURLVsPageID(CacheKey<UrlPageIdMapping> UrlPageIdMappingList)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactoryRegionCategory);
            if (_collectionDataFactoryUrlPageId.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), UrlPageIdMappingList.Id).Result)
            {
                return _collectionDataFactoryUrlPageId.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), UrlPageIdMappingList).Result;
            }
            else
            {
                if (!_collectionDataFactoryUrlPageId.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), UrlPageIdMappingList).Result)
                {
                    return false;
                }
            }

            return true;
        }

        public List<MappedLanguage> GetMappedLanguage()
        {
            var query = "select * from M where M.id= 'MappedLanguage'";
            //for mongoDB
            var filter = Builders<CacheKey<MappedLanguage>>.Filter.Eq("_id", "MappedLanguage");
            //end mongo
            var result = _collectionDataFactoryMappedLanguage.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            return result.CacheValue;
        }

        public List<MappedRegion> GetRegionDestinationMapping()
        {
            var query = "select * from M where M.id= 'RegionVsDestination'";
            //for mongoDB
            var filter = Builders<CacheKey<MappedRegion>>.Filter.Eq("_id", "RegionVsDestination");
            //end mongo
            var result = _collectionDataFactoryMappedRegion.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            return result.CacheValue;
        }

        private void CheckIfMasterDataCollectionExists(CollectionDataFactory<CacheKey<RegionCategoryMapping>> _collectionDataFactory)
        {
            if (!_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }
        }
    }
}