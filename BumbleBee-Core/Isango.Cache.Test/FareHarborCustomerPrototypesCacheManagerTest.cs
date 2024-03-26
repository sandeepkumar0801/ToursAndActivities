using CacheManager.Contract;
using CacheManager.FareHarborCacheManagers;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.Wrapper;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    public class FareHarborCustomerPrototypesCacheManagerTest : BaseTest
    {
        private IFareHarborCustomerPrototypesCacheManager _fareHarborCustomerPrototypesCacheManagerMock;
        private CollectionDataFactory<CacheKey<CustomerPrototype>> _collectionDataFactory;
        private FareHarborCustomerPrototypesCacheManager _fareHarborCustomerPrototypesCacheManager;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _fareHarborCustomerPrototypesCacheManagerMock = Substitute.For<IFareHarborCustomerPrototypesCacheManager>();
            _collectionDataFactory = Substitute.For<CollectionDataFactory<CacheKey<CustomerPrototype>>>();
            _fareHarborCustomerPrototypesCacheManager = new FareHarborCustomerPrototypesCacheManager(_collectionDataFactory);
        }

        [Test]
        public void SetCustomerPrototypeByActivityToCache()
        {
            var cacheResult = new CacheKey<CustomerPrototype>
            {
                Id = "FareHarborCustomerPrototype",
                CacheValue = new List<CustomerPrototype>
                {
                    new CustomerPrototype
                    {
                        AgeGroupId = 3,
                        CustomerPrototypeId = 120571,
                        PassengerType = PassengerType.Child,
                        ServiceId = 13526,
                        ServiceOptionId = 53832,
                        StartAt = "10:00"
                    }
                }
            };
            _collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist("").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CreateCollection("", "").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CheckIfDocumentExist("", "").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().UpdateDocument("", cacheResult).ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().InsertDocument("", cacheResult).ReturnsForAnyArgs(true);
            _fareHarborCustomerPrototypesCacheManagerMock.SetCustomerPrototypeByActivityToCache(cacheResult).ReturnsForAnyArgs(true);
            var result = _fareHarborCustomerPrototypesCacheManager.SetCustomerPrototypeByActivityToCache(cacheResult);
            Assert.IsTrue(result);
        }
    }
}