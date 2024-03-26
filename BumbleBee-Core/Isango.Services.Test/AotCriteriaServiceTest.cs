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
    public class AotCriteriaServiceTest : BaseTest
    {
        private IAotCriteriaService _aotCriteriaService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            var builder = new ContainerBuilder(); 
            builder.RegisterModule<StartupModule>();
           var container = builder.Build(); // Build the container
            using (var scope = container.BeginLifetimeScope())
            {
                _aotCriteriaService = scope.Resolve<IAotCriteriaService>();
            }
        }

        [Test]
        [Ignore("")]
        public void GetAvailabilityAndServiceDetails_Australia_Test()
        {
            var products = new List<IsangoHBProductMapping>
            {
                new IsangoHBProductMapping
                {
                    ApiType = APIType.Aot,
                    CountryId = 6169,
                    MinAdultCount = 1,
                    FactSheetId = 0,
                    HotelBedsActivityCode = "DRWTDAATAUSD5",
                    IsangoHotelBedsActivityId = 79,
                    IsangoRegionId = 6187,
                }
            };
            var criteria = CreateCriteria(products);
            var availability = _aotCriteriaService.GetAvailability(criteria);
            var serviceDetails = _aotCriteriaService.GetServiceDetails(availability, products);

            Assert.NotNull(availability);
            Assert.NotNull(serviceDetails);
        }

        [Test]
        [Ignore("")]
        public void GetAvailabilityAndServiceDetails_NegativeTest()
        {
            var products = new List<IsangoHBProductMapping>
            {
                new IsangoHBProductMapping
                {
                    ApiType = APIType.Aot,
                    CountryId = 6169,
                    MinAdultCount = 1,
                    FactSheetId = 0,
                    HotelBedsActivityCode = "DRWTDAATAUSD5",
                    IsangoHotelBedsActivityId = 79,
                    IsangoRegionId = 6187,
                }
            };
            var criteria = CreateCriteria(products);

            //Null Check Scenario
            criteria.MappedProducts.First().HotelBedsActivityCode = null;
            var availability = _aotCriteriaService.GetAvailability(criteria);
            Assert.IsNotNull(availability);

            //Null Check Scenario
            var activities = new List<Activity> { null };
            var serviceDetails = _aotCriteriaService.GetServiceDetails(activities, products);
            Assert.IsNotNull(serviceDetails);

            //Null Check Scenario
            products.First().IsangoHotelBedsActivityId = 1234;
            serviceDetails = _aotCriteriaService.GetServiceDetails(availability, products);
            Assert.IsNotNull(serviceDetails);

            //Null Check Scenario
            criteria.MappedProducts = null;
            availability = _aotCriteriaService.GetAvailability(criteria);
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