using CacheManager;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    public class AttractionActivityMappingCacheManagerTest : BaseTest
    {
        private IAttractionActivityMappingCacheManager _attractionActivityMappingCacheManagerMock;
        private AttractionActivityMappingCacheManager _attractionActivityMappingCacheManager;
        private CollectionDataFactory<AttractionActivityMapping> _collectionDataFactoryFilter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _collectionDataFactoryFilter = Substitute.For<CollectionDataFactory<AttractionActivityMapping>>();
            _attractionActivityMappingCacheManagerMock = Substitute.For<IAttractionActivityMappingCacheManager>();
            _attractionActivityMappingCacheManager = new AttractionActivityMappingCacheManager(_collectionDataFactoryFilter);
        }

        [Test]
        public void LoadCachedAttractionActivitytest()
        {
            var attractionActivityMappingList = new List<AttractionActivityMapping>()
            {
                new AttractionActivityMapping()
                {
                    ActivityId = "15",
                    AttractionId = "10"
                },
                new AttractionActivityMapping()
                {
                    ActivityId = "16",
                    AttractionId = "11"
                }
            };

            _collectionDataFactoryFilter.GetCollectionDataHelper().CheckIfCollectionExist("AttractonActivityMappingCollection").ReturnsForAnyArgs(true);
            _collectionDataFactoryFilter.GetCollectionDataHelper().DeleteCollection("AttractonActivityMappingCollection").ReturnsForAnyArgs(true);
            _collectionDataFactoryFilter.GetCollectionDataHelper().CreateCollection("AttractonActivityMappingCollection", "/AttractionId").ReturnsForAnyArgs(true);
            _attractionActivityMappingCacheManagerMock.LoadCachedAttractionActivity(attractionActivityMappingList).ReturnsForAnyArgs("10,11");
            var result = _attractionActivityMappingCacheManager.LoadCachedAttractionActivity(attractionActivityMappingList);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetAttractionActivityListTest()
        {
            var attractionActivityMappingList = new List<AttractionActivityMapping>()
            {
                new AttractionActivityMapping()
                {
                    ActivityId = "15",
                    AttractionId = "10"
                }
            };

            _collectionDataFactoryFilter.GetCollectionDataHelper().GetResultList("AttractonActivityMappingCollection", "").ReturnsForAnyArgs(attractionActivityMappingList);
            var result = _attractionActivityMappingCacheManager.GetAttractionActivityList(10);
            Assert.IsNotNull(result);
        }

        [Test]
        public void InsertDocumentsTest()
        {
            var attractionActivityMappingList = new List<AttractionActivityMapping>()
            {
                new AttractionActivityMapping()
                {
                    ActivityId = "15",
                    AttractionId = "10"
                }
            };

            var attractionDocument = new AttractionActivityMapping()
            {
                ActivityId = "15",
                AttractionId = "10"
            };
            _collectionDataFactoryFilter.GetCollectionDataHelper().InsertDocument("AttractonActivityMappingCollection", attractionDocument).ReturnsForAnyArgs(true);
            _attractionActivityMappingCacheManagerMock.InsertDocuments(attractionActivityMappingList).ReturnsForAnyArgs("");
            var result = _attractionActivityMappingCacheManager.InsertDocuments(attractionActivityMappingList);
            Assert.AreEqual("", result);
        }
    }
}