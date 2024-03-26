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
using Util;

namespace Isango.Services.Test
{
    public class MoulinRougeCriteriaServiceTest : BaseTest
    {
        private IMoulinRougeCriteriaService _moulinRougeCriteriaService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();
            using (var scope = _container.BeginLifetimeScope())
            {
                _moulinRougeCriteriaService = scope.Resolve<IMoulinRougeCriteriaService>();
            }
        }

        [Test]
        public void GetAvailabilityAndServiceDetailsTest()
        {
            var products = new List<IsangoHBProductMapping>
            {
                new IsangoHBProductMapping
                {
                    ApiType = APIType.Moulinrouge,
                    CountryId = 0,
                    MinAdultCount = 1,
                    FactSheetId = 82646,
                    HotelBedsActivityCode = "79675~21:00:00",
                    IsangoHotelBedsActivityId = 16628,
                    IsangoRegionId = 7129,
                }
            };
            var criteria = CreateCriteria(products);
            var availability = _moulinRougeCriteriaService.GetAvailability(criteria);

            var serviceDetails = _moulinRougeCriteriaService.GetServiceDetails(availability, products);

            Assert.NotNull(availability);
            Assert.NotNull(serviceDetails);
        }

        [Test]
        public void GetAvailabilityAndServiceDetails_NegativeTest()
        {
            //Null Check Scenario
            var activities = new List<Activity> { null };
            var serviceDetails = _moulinRougeCriteriaService.GetServiceDetails(activities, null);
            Assert.IsNotNull(serviceDetails);

            //Null Check Scenario
            var availability = _moulinRougeCriteriaService.GetAvailability(null);
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