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
    public class ReviewServiceTest : BaseTest
    {
        private ReviewService _reviewServiceForMocking;
        private IReviewPersistence _gatewayPersistenceMock;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _gatewayPersistenceMock = Substitute.For<IReviewPersistence>();
            var gatewayLog = Substitute.For<ILogger>();
            _reviewServiceForMocking = new ReviewService(_gatewayPersistenceMock, gatewayLog);
        }

        [Test]
        public void GetProductReviewsTest()
        {
            var listReview = new List<Review>() { new Review() { ServiceId = "5850", Title = "reviewtest" } };
            var tuple = Tuple.Create(listReview, 1001);
            _gatewayPersistenceMock.GetProductReviews(5850, 5, 1).ReturnsForAnyArgs(tuple);
            var result = _reviewServiceForMocking.GetProductReviewsAsync(5850, 5, 1).Result;
            Assert.That(result, Is.EqualTo(tuple));
        }

        [Test]
        public void GetProductReviewWithException()
        {
            //Catch Block Scenario
            _reviewServiceForMocking.GetProductReviewsAsync(0, 0, 0).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _reviewServiceForMocking.GetProductReviewsAsync(0, 0, 0));
        }

        [Test]
        public void AddReviewTest()
        {
            var testResult = 5001;
            var userReview = new UserReview
            {
                Status = ReviewStatus.UNDEFINED,
                ActivityId = 1001,
                BookingRef = "1001",
                Email = "test@test.com"
            };

            _gatewayPersistenceMock.AddReview(userReview, "UserAgent").ReturnsForAnyArgs(testResult);
            var result = _reviewServiceForMocking.AddReviewAsync(userReview, "agent").Result;
            Assert.That(result, Is.EqualTo(testResult));

            //Catch Block Scenario
            _reviewServiceForMocking.AddReviewAsync(userReview, "UserAgent").Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _reviewServiceForMocking.AddReviewAsync(userReview, "UserAgent"));
        }

        [Test]
        public void AddReviewImageTest()
        {
            _gatewayPersistenceMock.AddReviewImage(1001, "TestImagefile", 0, "AddImageReview").ReturnsForAnyArgs(true);
            var result = _reviewServiceForMocking.AddReviewImageAsync(1001, "TestImagefile", 0, "AddImageReview").Result;
            Assert.That(result, Is.EqualTo(true));

            //Catch Block Scenario
            _reviewServiceForMocking.AddReviewImageAsync(1001, "TestImagefile", 0, "AddImageReview").Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _reviewServiceForMocking.AddReviewImageAsync(1001, "TestImagefile", 0, "AddImageReview"));
        }

        [Test]
        public void GetReviewsTest()
        {
            var listReview = new List<Review>() { new Review { UserName = "TestUser1", Id = 1001, Country = "India", ImageName = "TestImage", Text = "Testing review functionality" } };
            var listReviewTuple = new Tuple<List<Review>, int>(listReview, 1);

            _gatewayPersistenceMock.GetReviews("312113EE-2C74-4352-B541-013C851C73A2", 5, 1).ReturnsForAnyArgs(listReviewTuple);
            var result = _reviewServiceForMocking.GetReviewsAsync("312113EE-2C74-4352-B541-013C851C73A2", 5, 1).Result;
            Assert.That(result, Is.EqualTo(listReviewTuple));

            //Catch Block Scenario
            _reviewServiceForMocking.GetReviewsAsync("312113EE-2C74-4352-B541-013C851C73A2", 5, 1).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _reviewServiceForMocking.GetReviewsAsync("312113EE-2C74-4352-B541-013C851C73A2", 5, 1));
        }

        [Test]
        public void GetAllProductReviewsDataTest()
        {
            var listReview = new List<RegionReviews> { new RegionReviews {  Id = 1001, Name = "India", Reviews = new List<Review> { new Review { UserName = "TestUser1", Id = 1001, Country = "India", ImageName = "TestImage", Text = "Testing review functionality" } },
            Url = "Testurl"} };

            _gatewayPersistenceMock.GetAllProductReviewsData().ReturnsForAnyArgs(listReview);
            var result = _reviewServiceForMocking.GetAllProductReviewsDataAsync().Result;
            Assert.That(result, Is.EqualTo(listReview));

            //Catch Block Scenario
            _reviewServiceForMocking.GetAllProductReviewsDataAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _reviewServiceForMocking.GetAllProductReviewsDataAsync());
        }
    }
}