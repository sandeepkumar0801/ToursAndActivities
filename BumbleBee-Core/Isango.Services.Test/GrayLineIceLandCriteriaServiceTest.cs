using Autofac;
using Isango.Entities;
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
    public class GrayLineIceLandCriteriaServiceTest : BaseTest
    {
        private IGrayLineIceLandCriteriaService _grayLineIceLandCriteriaService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();
            using (var scope = _container.BeginLifetimeScope())
            {
                _grayLineIceLandCriteriaService = scope.Resolve<IGrayLineIceLandCriteriaService>();
            }
        }

        [Test]
        public void GetAvailabilityAndServiceDetailsTest()
        {
            var products = new List<IsangoHBProductMapping>
            {
                new IsangoHBProductMapping
                {
                    ApiType = APIType.Graylineiceland,
                    CountryId = 0,
                    MinAdultCount = 1,
                    FactSheetId = 0,
                    HotelBedsActivityCode = "AH74",
                    IsangoHotelBedsActivityId = 6568,
                    IsangoRegionId = 7518,
                }
            };
            var criteria = CreateCriteria(products);
            var availability = _grayLineIceLandCriteriaService.GetAvailability(criteria);

            var serviceDetails = _grayLineIceLandCriteriaService.GetServiceDetails(availability, products);

            Assert.NotNull(availability);
            Assert.NotNull(serviceDetails);
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