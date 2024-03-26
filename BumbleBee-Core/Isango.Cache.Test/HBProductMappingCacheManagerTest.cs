using CacheManager;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    public class HbProductMappingCacheManagerTest : BaseTest
    {
        private IHbProductMappingCacheManager _hbProductMappingCacheManagerMock;
        private HbProductMappingCacheManager _hbProductMappingCacheManager;
        private CollectionDataFactory<IsangoHBProductMapping> _collectionDataFactory;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _hbProductMappingCacheManagerMock = Substitute.For<IHbProductMappingCacheManager>();
            _collectionDataFactory = Substitute.For<CollectionDataFactory<IsangoHBProductMapping>>();
            _hbProductMappingCacheManager = new HbProductMappingCacheManager(_collectionDataFactory);
        }

        [Test]
        public void LoadCacheMappingTest()
        {
            var hbProductMapping = new List<IsangoHBProductMapping>()
            {
                new IsangoHBProductMapping()
                {
                    ApiType = Entities.Enums.APIType.Aot,
                    Language = "en",
                    FactSheetId = 123,
                    IsangoRegionId = 10
                },
                new IsangoHBProductMapping()
                {
                    ApiType = Entities.Enums.APIType.Bokun,
                    Language = "en",
                    FactSheetId = 32,
                    IsangoRegionId = 11
                }
            };

            _collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist("HBProductMappingCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().DeleteCollection("HBProductMappingCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CreateCollection("HBProductMappingCollection", "/ApiType").ReturnsForAnyArgs(true);
            _hbProductMappingCacheManagerMock.LoadCacheMapping(hbProductMapping).Returns("10,11");
            var result = _hbProductMappingCacheManager.LoadCacheMapping(hbProductMapping);
            Assert.AreEqual("10,11", result);
        }
    }
}