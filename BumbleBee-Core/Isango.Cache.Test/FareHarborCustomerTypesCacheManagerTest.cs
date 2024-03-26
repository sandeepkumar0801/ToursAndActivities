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
    public class FareHarborCustomerTypesCacheManagerTest : BaseTest
    {
        private IFareHarborCustomerTypesCacheManager _fareHarborCustomerTypesCacheManagerMock;
        private CollectionDataFactory<CacheKey<AgeGroup>> _collectionDataFactory;
        private FareHarborCustomerTypesCacheManager _fareHarborCustomerTypesCacheManager;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _fareHarborCustomerTypesCacheManagerMock = Substitute.For<IFareHarborCustomerTypesCacheManager>();
            _collectionDataFactory = Substitute.For<CollectionDataFactory<CacheKey<AgeGroup>>>();
            _fareHarborCustomerTypesCacheManager = new FareHarborCustomerTypesCacheManager(_collectionDataFactory);
        }

        [Test]
        public void SetFareHarborAgeGroupsByActivityToCacheTest()
        {
            var cacheResult = new CacheKey<AgeGroup>
            {
                Id = "FareHarborAgeGroup",
                CacheValue = new List<AgeGroup>
                {
                    new AgeGroup
                    {
                        AgeGroupId = 1,
                        ActivityId = 13526,
                        DisplayName = "Adult",
                        Description = "",
                        Name = "A"
                    }
                }
            };

            _collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist("").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CreateCollection("", "").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CheckIfDocumentExist("", "").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().UpdateDocument("", cacheResult).ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().InsertDocument("", cacheResult).ReturnsForAnyArgs(true);
            _fareHarborCustomerTypesCacheManagerMock.SetFareHarborAgeGroupsByActivityToCache(cacheResult).ReturnsForAnyArgs(true);
            var result = _fareHarborCustomerTypesCacheManager.SetFareHarborAgeGroupsByActivityToCache(cacheResult);
            Assert.IsTrue(result);
        }
    }
}