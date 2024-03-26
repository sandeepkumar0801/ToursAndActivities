using CacheManager;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    public class CalendarAvailabilityCacheManagerTest : BaseTest
    {
        private ICalendarAvailabilityCacheManager _calendarAvailabilityCacheManagerMock;
        private CalendarAvailabilityCacheManager calendarAvailabilityCacheManager;
        private CollectionDataFactory<CalendarAvailability> _collectionDataFactory;
        [OneTimeSetUp]
        public void TestInitialise()
        {
            _calendarAvailabilityCacheManagerMock = Substitute.For<ICalendarAvailabilityCacheManager>();
             calendarAvailabilityCacheManager = new CalendarAvailabilityCacheManager(_collectionDataFactory);
        }

        [Test]
        public void LoadCalendarAvailabilityTest()
        {
            var calendarAvailabilityList = CreateCalendarData();

            _collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist("CalendarAvailabilityCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().DeleteCollection("CalendarAvailabilityCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CreateCollection("CalendarAvailabilityCollection", "/ActivityId").ReturnsForAnyArgs(true);
            _calendarAvailabilityCacheManagerMock.LoadCalendarAvailability().ReturnsForAnyArgs(true);
            var result = calendarAvailabilityCacheManager.LoadCalendarAvailability();
            Assert.IsTrue(result);
        }

        [Test]
        public void GetCalendarAvailability()
        {
            var calendarAvailabilityList = CreateCalendarData();

            _collectionDataFactory.GetCollectionDataHelper().GetResultList("CalendarAvailabilityCollection", "Select * from C WHERE c.ActivityId= 5972 and c.AffiliateId = 'FC0FF579 - 32E6 - 4941 - 9D96 - 287AFC367B9E'").ReturnsForAnyArgs(calendarAvailabilityList);
            _calendarAvailabilityCacheManagerMock.GetCalendarAvailability(853, "5beef089-3e4e-4f0f-9fbf-99bf1f350183").ReturnsForAnyArgs(calendarAvailabilityList);
        }

        private List<CalendarAvailability> CreateCalendarData()
        {
            var calendarAvailabilityList = new List<CalendarAvailability>
            {
                new CalendarAvailability
                {
                    ActivityId = 853,
                    RegionId = 6167,
                    Currency ="USD",
                    AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                    StartDate = DateTime.Now.AddDays(1),
                    B2BBasePrice = 1254.1M,
                    B2CBasePrice = 1150M
                },
                new CalendarAvailability
                {
                    ActivityId = 853,
                    RegionId = 6167,
                    Currency ="USD",
                    AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                    StartDate = DateTime.Now.AddDays(2),
                    B2BBasePrice = 1254.1M,
                    B2CBasePrice = 1150M
                }
            };
            return calendarAvailabilityList;
        }
    }
}