using CacheManager;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.Wrapper;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    [TestFixture]
    public class LandingCacheTest : BaseTest
    {
        private ILandingCacheManager _landingCacheManagerMock;
        private LandingCacheManager _landingCacheManager;
        private CollectionDataFactory<CacheKey<LocalizedMerchandising>> _collectionDataFactory;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _landingCacheManagerMock = Substitute.For<ILandingCacheManager>();
            _collectionDataFactory = Substitute.For<CollectionDataFactory<CacheKey<LocalizedMerchandising>>>();
            _landingCacheManager = new LandingCacheManager(_collectionDataFactory);
        }

        [Test]
        public void GetLoadLocalizedMerchandisingTest()
        {
            var lmList = new List<LocalizedMerchandising>()
            {
                new LocalizedMerchandising()
                {
                    Id = 10,
                    Name = "test"
                }
            };

            var cached = new CacheKey<LocalizedMerchandising>()
            {
                Id = "LocalizedMerchandising",
                CacheValue = lmList
            };
            _landingCacheManager.GetLoadLocalizedMerchandising("LocalizedMerchandising").ReturnsForAnyArgs(cached);
            var result = _landingCacheManager.GetLoadLocalizedMerchandising("LocalizedMerchandising");
            Assert.IsTrue(result?.CacheValue.Count > 0);
        }

        [Test]
        public void SetLoadLocalizedMerchandisingTest()
        {
            var lmList = new List<LocalizedMerchandising>()
            {
                new LocalizedMerchandising()
                {
                    Id = 10,
                    Name = "test"
                }
            };

            var cached = new CacheKey<LocalizedMerchandising>()
            {
                Id = "LocalizedMerchandising",
                CacheValue = lmList
            };

            _collectionDataFactory.GetCollectionDataHelper().InsertDocument("MasterDataCollection", cached).ReturnsForAnyArgs(true);
            _landingCacheManagerMock.SetLoadLocalizedMerchandising(cached).ReturnsForAnyArgs(true);
            var result = _landingCacheManager.SetLoadLocalizedMerchandising(cached);
            Assert.IsTrue(result);
        }
    }
}