using Isango.Entities.Master;
using Isango.Entities.Review;
using Isango.Persistence.Contract;
using Isango.Service;
using Logger.Contract;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Isango.Services.Test
{
    [TestFixture]
    public class ActivityDeltaServiceTest : BaseTest
    {
        private IActivityDeltaPersistence _gatewayActivityDeltaPersistence;
        private ActivityDeltaService _activityDeltaServiceForMocking;
        private ActivityDeltaService _activityDeltaServiceForMockingException;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _gatewayActivityDeltaPersistence = Substitute.For<IActivityDeltaPersistence>();
            var gatewayLogger = Substitute.For<ILogger>();
            _activityDeltaServiceForMocking = new ActivityDeltaService(_gatewayActivityDeltaPersistence, gatewayLogger);
            _activityDeltaServiceForMockingException =
                new ActivityDeltaService(_gatewayActivityDeltaPersistence, gatewayLogger);
        }

        [Test]
        [Ignore("Ignored")]
        public void GetDeltaReviewAsyncTest()
        {
            var reviews = new List<Review>
            {
                new Review
                {
                    Id = 123,
                    Rating = "good",
                }
            };

            _gatewayActivityDeltaPersistence.GetDeltaActivityReview().ReturnsForAnyArgs(reviews);
            var result = _activityDeltaServiceForMocking.GetDeltaActivityAsync().Result;
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignored")]
        public void GetDeltaReviewAsyncExceptionTest()
        {
            _activityDeltaServiceForMockingException.GetDeltaReviewAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _activityDeltaServiceForMockingException.GetDeltaReviewAsync());
        }

        [Test]
        [Ignore("Ignored")]
        public void GetDeltaPassengerInfoAsyncTest()
        {
            var passengerInfos = new List<Entities.Booking.PassengerInfo>
            {
                new Entities.Booking.PassengerInfo
                {
                    PassengerTypeId = 1,
                    Label = "Adult"
                }
            };

            _gatewayActivityDeltaPersistence.GetDeltaActivityPassengerInfo().ReturnsForAnyArgs(passengerInfos);
            var result = _activityDeltaServiceForMocking.GetDeltaPassengerInfoAsync().Result;
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignored")]
        public void GetDeltaPassengerInfoAsyncExceptionTest()
        {
            _activityDeltaServiceForMockingException.GetDeltaPassengerInfoAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _activityDeltaServiceForMockingException.GetDeltaPassengerInfoAsync());
        }

        [Test]
        [Ignore("Ignored")]
        public void GetDeltaActivityAsyncTest()
        {
            var activityIds = new List<ActivityIds>
            {
                new ActivityIds
                {
                   LanguageCode =  "en",
                    ServiceStatusID = true,
                    Serviceid = 853
                }
            };

            _gatewayActivityDeltaPersistence.GetDeltaActivity().ReturnsForAnyArgs(activityIds);
            var result = _activityDeltaServiceForMocking.GetDeltaActivityAsync().Result;
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignored")]
        public void GetDeltaActivityAsyncExceptionTest()
        {
            _activityDeltaServiceForMockingException.GetDeltaActivityAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _activityDeltaServiceForMockingException.GetDeltaActivityAsync());
        }

        [Test]
        [Ignore("Ignored")]
        public void GetDeltaActivityPriceAsyncTest()
        {
            var activityMinPrices = new List<ActivityMinPrice>
            {
                new ActivityMinPrice
                {
                    Serviceid = 853,
                    BasePrice =  10.0M,
                    AffiliateID = ""
                }
            };

            _gatewayActivityDeltaPersistence.GetDeltaActivityPrice().ReturnsForAnyArgs(activityMinPrices);
            var result = _activityDeltaServiceForMocking.GetDeltaActivityPriceAsync().Result;
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignored")]
        public void GetDeltaActivityPriceAsyncExceptionTest()
        {
            _activityDeltaServiceForMockingException.GetDeltaActivityPriceAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _activityDeltaServiceForMockingException.GetDeltaActivityPriceAsync());
        }

        [Test]
        [Ignore("Ignored")]
        public void GetDeltaActivityAvailableAsyncTest()
        {
            var activityAvailableDays = new List<ActivityAvailableDays>
            {
                new ActivityAvailableDays
                {
                    Serviceid = 853,
                    AvailableDays = "10"
                }
            };

            _gatewayActivityDeltaPersistence.GetDeltaActivityAvailability().ReturnsForAnyArgs(activityAvailableDays);
            var result = _activityDeltaServiceForMocking.GetDeltaActivityAvailableAsync().Result;
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignored")]
        public void GetDeltaActivityAvailableAsyncExceptionTest()
        {
            _activityDeltaServiceForMockingException.GetDeltaActivityAvailableAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _activityDeltaServiceForMockingException.GetDeltaActivityAvailableAsync());
        }
    }
}