using CacheManager;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.Enums;
using Logger.Contract;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    public class AgeGroupsCacheManagerTest : BaseTest
    {
        private IAgeGroupsCacheManager _ageGroupsCacheManagerMock;
        private AgeGroupsCacheManager _ageGroupsCacheManager;
        private CollectionDataFactory<AgeGroup> _collectionDataFactory;
        private CollectionDataFactory<FareHarborAgeGroup> _collectionDataFactoryFH;
        private readonly ILogger _logger;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _collectionDataFactory = Substitute.For<CollectionDataFactory<AgeGroup>>();
            _collectionDataFactoryFH = Substitute.For<CollectionDataFactory<FareHarborAgeGroup>>();
            _ageGroupsCacheManagerMock = Substitute.For<IAgeGroupsCacheManager>();
            _ageGroupsCacheManager = new AgeGroupsCacheManager(_collectionDataFactory, _collectionDataFactoryFH, _logger);
            //_ageGroupsCacheManagerMock = new AgeGroupsCacheManager(_cosmosHelper, _cosmosHelperHF);
        }

        [Test]
        public void LoadAgeGroupByActivityTest()
        {
            var ageGroupList = new List<AgeGroup>
            {
                new AgeGroup()
                {
                    AgeGroupId = 100,
                    ActivityId = 853,
                    DisplayName="test"
                },
                 new AgeGroup()
                {
                    AgeGroupId = 101,
                    ActivityId = 853,
                    DisplayName="test1"
                }
            };

            _collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist("AgeGroupByActivityCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CreateCollection("AgeGroupByActivityCollection", "/ActivityId").ReturnsForAnyArgs(true);
            _ageGroupsCacheManagerMock.LoadAgeGroupByActivity(ageGroupList).ReturnsForAnyArgs("");
            var result = _ageGroupsCacheManager.LoadAgeGroupByActivity(ageGroupList);
            Assert.AreEqual("100,101", result);
        }

        [Test]
        public void LoadFhAgeGroupByActivity()
        {
            var ageGroupList = new List<FareHarborAgeGroup>
            {
                new FareHarborAgeGroup()
                {
                    AgeGroupId = 100,
                    ActivityId = 853,
                    DisplayName="test",
                    ApiType = Convert.ToInt32(APIType.Fareharbor)
                },
                 new FareHarborAgeGroup()
                {
                    AgeGroupId = 101,
                    ActivityId = 853,
                    DisplayName="test1",
                    ApiType = Convert.ToInt32(APIType.Fareharbor)
                }
            };

            _collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist("AgeGroupByActivityCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().DeleteCollection("AgeGroupByActivityCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CreateCollection("AgeGroupByActivityCollection", "/ActivityId").ReturnsForAnyArgs(true);
            _ageGroupsCacheManagerMock.LoadFhAgeGroupByActivity(ageGroupList).ReturnsForAnyArgs("100,101");
            var result = _ageGroupsCacheManager.LoadFhAgeGroupByActivity(ageGroupList);
            Assert.AreEqual("100,101", result);
        }

        [Test]
        public void GetAgeGroupTest()
        {
            var ageGroupList = new List<AgeGroup>
            {
                new AgeGroup()
                {
                    AgeGroupId = 100,
                    ActivityId = 853,
                    DisplayName="test",
                    ApiType = 2
                }
            };
            _collectionDataFactory.GetCollectionDataHelper().GetResultList("", "").ReturnsForAnyArgs(ageGroupList);
            _ageGroupsCacheManagerMock.GetAgeGroup(2, 14416).ReturnsForAnyArgs(ageGroupList);
            var result = _ageGroupsCacheManager.GetAgeGroup(2, 14416);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public void GetFareHarborAgeGroupTest()
        {
            var ageGroupList = new List<FareHarborAgeGroup>
            {
                new FareHarborAgeGroup()
                {
                    AgeGroupId = 100,
                    ActivityId = 853,
                    DisplayName="test",
                    ApiType = 2
                }
            };
            _collectionDataFactoryFH.GetCollectionDataHelper().GetResultList("", "").ReturnsForAnyArgs(ageGroupList);
            _ageGroupsCacheManagerMock.GetFareHarborAgeGroup(2, 14416).ReturnsForAnyArgs(ageGroupList);
            var result = _ageGroupsCacheManager.GetFareHarborAgeGroup(6, 14416);
            Assert.IsNotNull(result);
        }
    }
}