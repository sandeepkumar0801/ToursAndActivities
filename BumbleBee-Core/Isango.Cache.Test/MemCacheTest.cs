using CacheManager;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.HotelBeds;
using Isango.Entities.Region;
using Isango.Entities.Wrapper;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    public class MemCacheTest : BaseTest
    {
        private CollectionDataFactory<CacheKey<RegionCategoryMapping>>_collectionDataFactoryRegionCategory;
        private CollectionDataFactory<CacheKey<LocalizedMerchandising>>_collectionDataFactorylocalized;
        private CollectionDataFactory<CacheKey<MappedRegion>>_collectionDataFactoryMappedRegion;
        private CollectionDataFactory<CacheKey<MappedLanguage>>_collectionDataFactoryMappedLanguage;
        private CollectionDataFactory<CacheKey<UrlPageIdMapping>>_collectionDataFactoryUrlPageId;

        private IMemCache _memCacheMock;
        private MemCache _memCache;

        [OneTimeSetUp]
        public void TestInitialise()
        {
           _collectionDataFactoryRegionCategory = Substitute.For<CollectionDataFactory<CacheKey<RegionCategoryMapping>>>();
           _collectionDataFactorylocalized = Substitute.For<CollectionDataFactory<CacheKey<LocalizedMerchandising>>>();
           _collectionDataFactoryMappedRegion = Substitute.For<CollectionDataFactory<CacheKey<MappedRegion>>>();
           _collectionDataFactoryMappedLanguage = Substitute.For<CollectionDataFactory<CacheKey<MappedLanguage>>>();
           _collectionDataFactoryUrlPageId = Substitute.For<CollectionDataFactory<CacheKey<UrlPageIdMapping>>>();

            _memCacheMock = Substitute.For<IMemCache>();
            _memCache = new MemCache(_collectionDataFactoryRegionCategory, _collectionDataFactorylocalized,_collectionDataFactoryMappedRegion,
               _collectionDataFactoryMappedLanguage,_collectionDataFactoryUrlPageId);
        }

        [Test]
        public void RegionCategoryMappingTest()
        {
            var list = new List<RegionCategoryMapping>()
            {
                new RegionCategoryMapping()
                {
                    CategoryId = 1,
                    CategoryName = "test",
                    CategoryType = "test"
                }
            };

            var regionCategoryMappingList = new CacheKey<RegionCategoryMapping>()
            {
                Id = "regionCategoryMappingList",
                CacheValue = list
            };
           _collectionDataFactoryRegionCategory.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "regionCategoryMappingList").ReturnsForAnyArgs(true);
           _collectionDataFactoryRegionCategory.GetCollectionDataHelper().UpdateDocument("MasterDataCollection", regionCategoryMappingList).ReturnsForAnyArgs(true);
            _memCacheMock.RegionCategoryMapping(regionCategoryMappingList).ReturnsForAnyArgs(true);
            var result = _memCache.RegionCategoryMapping(regionCategoryMappingList);
            Assert.IsTrue(result);

           _collectionDataFactoryRegionCategory.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "regionCategoryMappingList").ReturnsForAnyArgs(false);
           _collectionDataFactoryRegionCategory.GetCollectionDataHelper().InsertDocument("MasterDataCollection", regionCategoryMappingList).ReturnsForAnyArgs(true);
            _memCacheMock.RegionCategoryMapping(regionCategoryMappingList).ReturnsForAnyArgs(true);
            var result2 = _memCache.RegionCategoryMapping(regionCategoryMappingList);
            Assert.IsTrue(result2);

           _collectionDataFactoryRegionCategory.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "regionCategoryMappingList").ReturnsForAnyArgs(false);
           _collectionDataFactoryRegionCategory.GetCollectionDataHelper().InsertDocument("MasterDataCollection", regionCategoryMappingList).ReturnsForAnyArgs(false);
            _memCacheMock.RegionCategoryMapping(regionCategoryMappingList).ReturnsForAnyArgs(true);
            var result3 = _memCache.RegionCategoryMapping(regionCategoryMappingList);
            Assert.IsFalse(result3);
        }

        [Test]
        public void LocalizedMerchandisingTest()
        {
            var list = new List<LocalizedMerchandising>()
            {
                new LocalizedMerchandising()
                {
                   AffiliateId =  "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                   Id = 12,
                    Name ="test"
                }
            };

            var localizedMerchandisingList = new CacheKey<LocalizedMerchandising>()
            {
                Id = "localizedMerchandisingList",
                CacheValue = list
            };
           _collectionDataFactorylocalized.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "localizedMerchandisingList").ReturnsForAnyArgs(true);
           _collectionDataFactorylocalized.GetCollectionDataHelper().UpdateDocument("MasterDataCollection", localizedMerchandisingList).ReturnsForAnyArgs(true);
            _memCacheMock.LocalizedMerchandising(localizedMerchandisingList).ReturnsForAnyArgs(true);
            var result = _memCache.LocalizedMerchandising(localizedMerchandisingList);
            Assert.IsTrue(result);

           _collectionDataFactorylocalized.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "localizedMerchandisingList").ReturnsForAnyArgs(false);
           _collectionDataFactorylocalized.GetCollectionDataHelper().InsertDocument("MasterDataCollection", localizedMerchandisingList).ReturnsForAnyArgs(true);
            _memCacheMock.LocalizedMerchandising(localizedMerchandisingList).ReturnsForAnyArgs(true);
            var result2 = _memCache.LocalizedMerchandising(localizedMerchandisingList);
            Assert.IsTrue(result2);

           _collectionDataFactorylocalized.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "localizedMerchandisingList").ReturnsForAnyArgs(false);
           _collectionDataFactorylocalized.GetCollectionDataHelper().InsertDocument("MasterDataCollection", localizedMerchandisingList).ReturnsForAnyArgs(false);
            _memCacheMock.LocalizedMerchandising(localizedMerchandisingList).ReturnsForAnyArgs(true);
            var result3 = _memCache.LocalizedMerchandising(localizedMerchandisingList);
            Assert.IsFalse(result3);
        }

        [Test]
        public void RegionDestinationMappingTest()
        {
            var list = new List<MappedRegion>()
            {
                new MappedRegion()
                {
                    DestinationCode = "ind",
                    RegionId = 7128,
                    RegionName = "india"
                }
            };

            var mappedRegionList = new CacheKey<MappedRegion>()
            {
                Id = "mappedRegionList",
                CacheValue = list
            };

           _collectionDataFactoryMappedRegion.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "mappedRegionList").ReturnsForAnyArgs(true);
           _collectionDataFactoryMappedRegion.GetCollectionDataHelper().UpdateDocument("MasterDataCollection", mappedRegionList).ReturnsForAnyArgs(true);
            _memCacheMock.RegionDestinationMapping(mappedRegionList).ReturnsForAnyArgs(true);
            var result = _memCache.RegionDestinationMapping(mappedRegionList);
            Assert.IsTrue(result);

           _collectionDataFactoryMappedRegion.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "mappedRegionList").ReturnsForAnyArgs(false);
           _collectionDataFactoryMappedRegion.GetCollectionDataHelper().InsertDocument("MasterDataCollection", mappedRegionList).ReturnsForAnyArgs(true);
            _memCacheMock.RegionDestinationMapping(mappedRegionList).ReturnsForAnyArgs(true);
            var result2 = _memCache.RegionDestinationMapping(mappedRegionList);
            Assert.IsTrue(result2);

           _collectionDataFactoryMappedRegion.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "mappedRegionList").ReturnsForAnyArgs(false);
           _collectionDataFactoryMappedRegion.GetCollectionDataHelper().InsertDocument("MasterDataCollection", mappedRegionList).ReturnsForAnyArgs(false);
            _memCacheMock.RegionDestinationMapping(mappedRegionList).ReturnsForAnyArgs(true);
            var result3 = _memCache.RegionDestinationMapping(mappedRegionList);
            Assert.IsFalse(result3);
        }

        [Test]
        public void LanguageCodeMappingTest()
        {
            var list = new List<MappedLanguage>()
            {
                new MappedLanguage()
                {
                    AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                    GliLanguageCode = 1,
                    IsangoLanguageCode = "en",
                     SupplierLanguageCode ="en"
                }
            };

            var mappedLanguagesList = new CacheKey<MappedLanguage>()
            {
                Id = "mappedLanguagesList",
                CacheValue = list
            };
           _collectionDataFactoryMappedLanguage.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "mappedLanguagesList").ReturnsForAnyArgs(true);
           _collectionDataFactoryMappedLanguage.GetCollectionDataHelper().UpdateDocument("MasterDataCollection", mappedLanguagesList).ReturnsForAnyArgs(true);
            _memCacheMock.LanguageCodeMapping(mappedLanguagesList).ReturnsForAnyArgs(true);
            var result = _memCache.LanguageCodeMapping(mappedLanguagesList);
            Assert.IsTrue(result);

           _collectionDataFactoryMappedLanguage.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "mappedLanguagesList").ReturnsForAnyArgs(false);
           _collectionDataFactoryMappedLanguage.GetCollectionDataHelper().InsertDocument("MasterDataCollection", mappedLanguagesList).ReturnsForAnyArgs(true);
            _memCacheMock.LanguageCodeMapping(mappedLanguagesList).ReturnsForAnyArgs(true);
            var result2 = _memCache.LanguageCodeMapping(mappedLanguagesList);
            Assert.IsTrue(result2);

           _collectionDataFactoryMappedLanguage.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "mappedLanguagesList").ReturnsForAnyArgs(false);
           _collectionDataFactoryMappedLanguage.GetCollectionDataHelper().InsertDocument("MasterDataCollection", mappedLanguagesList).ReturnsForAnyArgs(false);
            _memCacheMock.LanguageCodeMapping(mappedLanguagesList).ReturnsForAnyArgs(true);
            var result3 = _memCache.LanguageCodeMapping(mappedLanguagesList);
            Assert.IsFalse(result3);
        }

        [Test]
        public void LoadURLVsPageIDTest()
        {
            var list = new List<UrlPageIdMapping>()
            {
                new UrlPageIdMapping()
                {
                    AffiliateId =  "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                    CategoryId = 214,
                    LanguageCode="en",
                    PageId = 3,
                    PageName = "test",
                    RegionId = 51
                }
            };

            var UrlPageIdMappingList = new CacheKey<UrlPageIdMapping>()
            {
                Id = "UrlPageIdMappingList",
                CacheValue = list
            };
           _collectionDataFactoryUrlPageId.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "UrlPageIdMappingList").ReturnsForAnyArgs(true);
           _collectionDataFactoryUrlPageId.GetCollectionDataHelper().UpdateDocument("MasterDataCollection", UrlPageIdMappingList).ReturnsForAnyArgs(true);
            _memCacheMock.LoadURLVsPageID(UrlPageIdMappingList).ReturnsForAnyArgs(true);
            var result = _memCache.LoadURLVsPageID(UrlPageIdMappingList);
            Assert.IsTrue(result);

           _collectionDataFactoryUrlPageId.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "UrlPageIdMappingList").ReturnsForAnyArgs(false);
           _collectionDataFactoryUrlPageId.GetCollectionDataHelper().InsertDocument("MasterDataCollection", UrlPageIdMappingList).ReturnsForAnyArgs(true);
            _memCacheMock.LoadURLVsPageID(UrlPageIdMappingList).ReturnsForAnyArgs(true);
            var result2 = _memCache.LoadURLVsPageID(UrlPageIdMappingList);
            Assert.IsTrue(result2);

           _collectionDataFactoryUrlPageId.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "UrlPageIdMappingList").ReturnsForAnyArgs(false);
           _collectionDataFactoryUrlPageId.GetCollectionDataHelper().InsertDocument("MasterDataCollection", UrlPageIdMappingList).ReturnsForAnyArgs(false);
            _memCacheMock.LoadURLVsPageID(UrlPageIdMappingList).ReturnsForAnyArgs(true);
            var result3 = _memCache.LoadURLVsPageID(UrlPageIdMappingList);
            Assert.IsFalse(result3);
        }
    }
}