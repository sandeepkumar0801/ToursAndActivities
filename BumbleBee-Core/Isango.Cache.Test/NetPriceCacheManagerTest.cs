using CacheManager;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    [TestFixture]
    public class NetPriceCacheManagerTest : BaseTest
    {
        private INetPriceCacheManager _cachedNetPriceCacheManagerMock;
        private NetPriceCacheManager _cachedNetPriceCacheManager;

        private CollectionDataFactory<NetPriceMasterData> _collectionDataFactory;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _cachedNetPriceCacheManagerMock = Substitute.For<INetPriceCacheManager>();
            _collectionDataFactory = Substitute.For<CollectionDataFactory<NetPriceMasterData>>();

            _cachedNetPriceCacheManager = new NetPriceCacheManager(_collectionDataFactory);
        }

        [Test]
        public void LoadNetPriceMasterDataTest()
        {
            var netPriceDataList = new List<NetPriceMasterData>()
            {
                new  NetPriceMasterData()
                {
                    ProductId = 10
                },
                new NetPriceMasterData()
                {
                    ProductId  =11
                }
            };
            _collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist("NetPriceDataCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().DeleteCollection("NetPriceDataCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CreateCollection("NetPriceDataCollection", "/AffiliateId").ReturnsForAnyArgs(true);
            _cachedNetPriceCacheManagerMock.LoadNetPriceMasterData(netPriceDataList).ReturnsForAnyArgs("10,11");
            var result = _cachedNetPriceCacheManager.LoadNetPriceMasterData(netPriceDataList);
            Assert.AreEqual("10,11", result);
        }

        [Test]
        public void GetNetPriceMasterDataTest()
        {
            var netPriceDataList = new List<NetPriceMasterData>()
            {
                new  NetPriceMasterData()
                {
                    ProductId = 10
                },
                new NetPriceMasterData()
                {
                    ProductId  =11
                }
            };

            _collectionDataFactory.GetCollectionDataHelper().GetResultList("NetPriceDataCollection", string.Empty).ReturnsForAnyArgs(netPriceDataList);
            var result = _cachedNetPriceCacheManager.GetNetPriceMasterData();
            Assert.IsNotNull(result);
        }
    }
}