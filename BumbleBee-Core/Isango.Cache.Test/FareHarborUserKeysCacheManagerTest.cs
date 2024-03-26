using CacheManager.Contract;
using CacheManager.FareHarborCacheManagers;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.Wrapper;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    public class FareHarborUserKeysCacheManagerTest : BaseTest
    {
        private IFareHarborUserKeysCacheManager _fareHarborUserKeysCacheManagerMock;
        private FareHarborUserKeysCacheManager _fareHarborUserKeysCacheManager;
        private CollectionDataFactory<CacheKey<FareHarborUserKey>> _collectionDataFactory;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _fareHarborUserKeysCacheManagerMock = Substitute.For<IFareHarborUserKeysCacheManager>();
            _collectionDataFactory = Substitute.For<CollectionDataFactory<CacheKey<FareHarborUserKey>>>();
            _fareHarborUserKeysCacheManager = new FareHarborUserKeysCacheManager(_collectionDataFactory);
        }

        [Test]
        public void SetAllFareHarborUserKeysToCacheTest()
        {
            var cacheResult = new CacheKey<FareHarborUserKey>
            {
                Id = "FareHarborUserKey",
                CacheValue = new List<FareHarborUserKey>
                {
                    new FareHarborUserKey
                    {
                        CompanyShortName = "onlocationtoursnewyork",
                        Currency = "USD",
                        UserKey = "5d9531fc-a1e4-4355-b364-1f0b2ac37d04"
                    }
                }
            };
            _collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist("").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CreateCollection("", "").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CheckIfDocumentExist("", "").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().UpdateDocument("", cacheResult).ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().InsertDocument("", cacheResult).ReturnsForAnyArgs(true);
            _fareHarborUserKeysCacheManagerMock.SetAllFareHarborUserKeysToCache(cacheResult).ReturnsForAnyArgs(true);
            var result = _fareHarborUserKeysCacheManager.SetAllFareHarborUserKeysToCache(cacheResult);
            Assert.IsTrue(result);
        }
    }
}