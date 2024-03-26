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
    public class RedeamCriteriaServiceTest : BaseTest
    {
        private IRedeamCriteriaService _redeamCriteriaService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _redeamCriteriaService = scope.Resolve<IRedeamCriteriaService>();
            }
        }

        [Test]
        public void GetAvailabilityTest()
        {
            var products = new List<IsangoHBProductMapping>
            {
                new IsangoHBProductMapping
                {
                    ApiType = APIType.Redeam,
                    CountryId = 0,
                    MinAdultCount = 1,
                    FactSheetId = 972859,
                    HotelBedsActivityCode = "972859",
                    IsangoHotelBedsActivityId = 29136,
                    IsangoRegionId = 7377,
                    ServiceOptionInServiceid = 143598
                }
            };
            var criteria = CreateCriteria(products);
            var availability = _redeamCriteriaService.GetAvailability(criteria);

            var serviceDetails = _redeamCriteriaService.GetServiceDetails(availability, products);

            Assert.NotNull(availability);
            Assert.NotNull(serviceDetails);
        }

        #region Private Methods

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

        #endregion Private Methods
    }
}