using CacheManager;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities.Availability;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    public class HotelBedsActivitiesCacheManagerTest : BaseTest
    {
        private IHotelBedsActivitiesCacheManager _hotelBedsActivitiesCacheManagerMock;
        private HotelBedsAvailabilityCacheManager _hotelBedsActivitiesCacheManager;
        private CollectionDataFactory<Availability> _collectionDataFactory;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _hotelBedsActivitiesCacheManagerMock = Substitute.For<IHotelBedsActivitiesCacheManager>();
            _collectionDataFactory = Substitute.For<CollectionDataFactory<Availability>>();

            _hotelBedsActivitiesCacheManager = new HotelBedsAvailabilityCacheManager(_collectionDataFactory);
        }

        [Test]
        public void LoadAvailabilityCacheTest()
        {
            var availabilityList = new List<Availability>()
            {
                new Availability()
                {
                    RegionId = 10,
                    ServiceId = 12
                },
                new Availability()
                {
                    RegionId = 11,
                    ServiceId = 13
                }
            };
            _collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist("HotelBedAvailabilityCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().DeleteCollection("HotelBedAvailabilityCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CreateCollection("HotelBedAvailabilityCollection", "/RegionId").ReturnsForAnyArgs(true);
            _hotelBedsActivitiesCacheManagerMock.LoadAvailabilityCache(availabilityList).ReturnsForAnyArgs("10-12,11-13");
            var result = _hotelBedsActivitiesCacheManager.LoadAvailabilityCache(availabilityList);
            Assert.AreEqual("10-12,11-13", result);
        }

        [Test]
        public void GetAvailabilityTest()
        {
            var availabilityList = new List<Availability>()
            {
                new Availability()
                {
                    RegionId = 10,
                    ServiceId = 12
                }
            };

            _collectionDataFactory.GetCollectionDataHelper().GetResultList("", "").ReturnsForAnyArgs(availabilityList);
            var result = _hotelBedsActivitiesCacheManager.GetAvailability("10", "12");
            Assert.IsTrue(result.Count > 0);
        }
    }
}