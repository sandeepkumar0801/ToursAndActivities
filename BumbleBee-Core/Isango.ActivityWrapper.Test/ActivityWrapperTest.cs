using ActivityWrapper;
using Autofac;
using Isango.Entities;
using Isango.Register;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Isango.ActivityWrapper.Test
{
    [TestFixture]
    public class ActivityWrapperTest : BaseTest
    {
        private ActivityWrapperService _activityWrapper;

        [OneTimeSetUp]
        public void TestInitialise()
        {

            using (var scope = _container.BeginLifetimeScope())
            {
                _activityWrapper = scope.Resolve<ActivityWrapperService>();
            }
        }

        [Test]
        public void GetWrapperActivities()
        {
            var stopWatch = new Stopwatch();

            var activityIds = new List<int> { 29162 };
            var clientInfo = new ClientInfo
            {
                AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                CountryIp = "gb",
                LanguageCode = "en"
            };
            stopWatch.Start();
            var activities = _activityWrapper.GetActivities(activityIds, clientInfo);
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);
            Assert.IsTrue(activityIds.Count == activities.Count());
            Assert.IsTrue(activities.Count(x => x.BaseMinPrice != 0M) != 0);
            Assert.IsTrue(activities.Count(x => x.GateBaseMinPrice != 0M) != 0);
        }

        [Test]
        public void GetWrapperCalendarActivities()
        {
            var datePriceAvailability = _activityWrapper.GetPriceAndAvailability(29162, "5beef089-3e4e-4f0f-9fbf-99bf1f350183");

            Assert.IsTrue(datePriceAvailability.DatePriceAvailability.Count > 0);
            Assert.IsTrue(datePriceAvailability.CurrencyIsoCode != null);
            Assert.IsTrue(datePriceAvailability.DatePriceAvailability.FirstOrDefault().Value != 0M);
        }
    }
}