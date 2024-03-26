using Autofac;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Register;
using Isango.Service.Constants;
using Isango.Service.Contract;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Util;

namespace Isango.Services.Test
{
    public class PrioCriteriaServiceTest : BaseTest
    {
        private IPrioCriteriaService _prioCriteriaService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();
            using (var scope = _container.BeginLifetimeScope())
            {
                _prioCriteriaService = scope.Resolve<IPrioCriteriaService>();
            }
        }

        [Test]
        public void GetAvailabilityAndServiceDetailsTest()
        {
            var products = new List<IsangoHBProductMapping>
            {
                new IsangoHBProductMapping
                {
                    ApiType = APIType.Prio,
                    CountryId = 0,
                    MinAdultCount = 1,
                    FactSheetId = 0,
                    HotelBedsActivityCode = "3285",
                    IsangoHotelBedsActivityId = 5106,
                    IsangoRegionId = 7831,
                }
            };
            var criteria = CreateCriteria(products);
            var availability = _prioCriteriaService.GetAvailability(criteria);

            var serviceDetails = _prioCriteriaService.GetServiceDetails(availability, products);

            Assert.NotNull(availability);
            Assert.NotNull(serviceDetails);
        }

        [Test]
        public void GetAvailabilityAndServiceDetails_NegativeTest()
        {
            var products = new List<IsangoHBProductMapping>
            {
                new IsangoHBProductMapping
                {
                    ApiType = APIType.Prio,
                    CountryId = 0,
                    MinAdultCount = 1,
                    FactSheetId = 0,
                    HotelBedsActivityCode = "3285",
                    IsangoHotelBedsActivityId = 5106,
                    IsangoRegionId = 7831,
                }
            };
            var criteria = CreateCriteria(products);

            var availability = new List<Activity>
            {
                new Activity
                {
                    ID = 4567
                }
            };

            //Null Check Scenario
            var activities = new List<Activity> { null };
            var serviceDetails = _prioCriteriaService.GetServiceDetails(activities, products);
            Assert.IsNotNull(serviceDetails);

            //Null Check Scenario
            products.First().IsangoHotelBedsActivityId = 1234;
            serviceDetails = _prioCriteriaService.GetServiceDetails(availability, products);
            Assert.IsNotNull(serviceDetails);

            //Null Check Scenario
            criteria.MappedProducts = new List<IsangoHBProductMapping>();
            availability = _prioCriteriaService.GetAvailability(criteria);
            Assert.IsNull(availability);
        }

        #region Private Method

        /// <summary>
        /// Create criteria
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        private Entities.ConsoleApplication.ServiceAvailability.Criteria CreateCriteria(List<IsangoHBProductMapping> products)
        {
            var days = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Days2FetchForHeavyData));
            var months = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Months2FetchForHeavyData));

            //Set criteria
            var criteria = new Entities.ConsoleApplication.ServiceAvailability.Criteria
            {
                MappedProducts = products,
                Days2Fetch = days,
                Months2Fetch = months,
                Counter = (int)Math.Ceiling((double)(months * 30) / days),
                SameDay = false,
                Token = Guid.NewGuid().ToString()
            };

            return criteria;
        }

        #endregion Private Method
    }
}