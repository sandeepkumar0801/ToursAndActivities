using CacheManager;
using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities.Affiliate;
using Isango.Entities.Wrapper;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    [TestFixture]
    public class AffiliateCacheTest : BaseTest
    {
        private IAffiliateCacheManager _affiliateCacheManagerMock;
        private AffiliateCacheManager _affiliateCacheManager;

        private CollectionDataFactory<Affiliate> _collectionDataFactory;
        private CollectionDataFactory<CacheKey<AffiliateFilter>> _collectionDataCacheAffiliateFilter;
        private CollectionDataFactory<AffiliateFilter> _collectionDataAffiliateFilter;
        
        [OneTimeSetUp]
        public void TestInitialise()
        {
            _affiliateCacheManagerMock = Substitute.For<IAffiliateCacheManager>();
            _collectionDataCacheAffiliateFilter = Substitute.For<CollectionDataFactory<CacheKey<AffiliateFilter>>>();
            _collectionDataFactory = Substitute.For<CollectionDataFactory<Affiliate>>();
            _collectionDataAffiliateFilter = Substitute.For<CollectionDataFactory<AffiliateFilter>>();
            _affiliateCacheManager = new AffiliateCacheManager(_collectionDataCacheAffiliateFilter, _collectionDataAffiliateFilter, _collectionDataFactory);
        }

        [Test]
        public void GetAffiliateInfoTest()
        {
            const string affiliateKey = "www.isango.com";
            var affiliateInfo = new Affiliate { Id = "1001" };
            _collectionDataFactory.GetCollectionDataHelper().GetResult("test", "").ReturnsForAnyArgs(affiliateInfo);
            _affiliateCacheManagerMock.GetAffiliateInfo(affiliateKey).ReturnsForAnyArgs(affiliateInfo);
            var resultIsango = _affiliateCacheManager.GetAffiliateInfo(affiliateKey);
            Assert.That(resultIsango, Is.EqualTo(affiliateInfo));
        }

        [Test]
        public void SetAffiliateInfoToCacheTest()
        {
            var affiliateKey = "www.isango.in";
            var affiliateInfo = new Affiliate { Id = affiliateKey };

            _collectionDataFactory.GetCollectionDataHelper().InsertDocument("MasterDataCollection", affiliateInfo).ReturnsForAnyArgs(true);
            _affiliateCacheManagerMock.SetAffiliateInfoToCache(affiliateInfo).ReturnsForAnyArgs(true);
            var result = _affiliateCacheManager.SetAffiliateInfoToCache(affiliateInfo);
            Assert.IsTrue(result);
        }

        [Test]
        public void GetAffiliateFilterTest()
        {
            var affiliateFilters = new List<AffiliateFilter>
            {
                new AffiliateFilter
                {
                    Id = "1"
                }
            };

            var caheResult = new CacheKey<AffiliateFilter>()
            {
                CacheKeyName = "",
                CacheValue = affiliateFilters
            };

            var affiliateKey = "AffiliateFilter";
            _collectionDataCacheAffiliateFilter.GetCollectionDataHelper().GetResult("test", affiliateKey).ReturnsForAnyArgs(caheResult);
            _affiliateCacheManagerMock.GetAffiliateFilter(affiliateKey).ReturnsForAnyArgs(caheResult);
            var result = _affiliateCacheManagerMock.GetAffiliateFilter(affiliateKey);
            Assert.IsTrue(result.CacheValue.Count > 0);
        }

        [Test]
        public void SetAffiliateFilterToCacheTest()
        {
            var affiliateFilters = new List<AffiliateFilter>
            {
                new AffiliateFilter
                {
                    Id = "1"
                }
            };

            var cacheResult = new CacheKey<AffiliateFilter>()
            {
                CacheKeyName = "",
                CacheValue = affiliateFilters
            };
            _collectionDataCacheAffiliateFilter.GetCollectionDataHelper().CheckIfCollectionExist("test").ReturnsForAnyArgs(true);
            _collectionDataCacheAffiliateFilter.GetCollectionDataHelper().CreateCollection("test", "t1").ReturnsForAnyArgs(true);
            _collectionDataCacheAffiliateFilter.GetCollectionDataHelper().CheckIfDocumentExist("test", "sdad").ReturnsForAnyArgs(true);
            _collectionDataCacheAffiliateFilter.GetCollectionDataHelper().UpdateDocument("test", cacheResult).ReturnsForAnyArgs(true);
            _collectionDataCacheAffiliateFilter.GetCollectionDataHelper().InsertDocument("test", cacheResult).ReturnsForAnyArgs(true);
            _affiliateCacheManagerMock.SetAffiliateFilterToCache(cacheResult).ReturnsForAnyArgs(true);
            var result = _affiliateCacheManagerMock.SetAffiliateFilterToCache(cacheResult);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteAndCreateCollection()
        {
            _collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(Constant.AffiliateCollection).ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().DeleteCollection(Constant.AffiliateCollection).ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CreateCollection(Constant.AffiliateCollection, Constant.PartitionKeyAffiliateCollection).ReturnsForAnyArgs(true);
            _affiliateCacheManagerMock.DeleteAndCreateCollection().ReturnsForAnyArgs(true);
            var result = _affiliateCacheManagerMock.DeleteAndCreateCollection();
            Assert.IsTrue(result);
        }
    }
}