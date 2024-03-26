using Isango.Entities;
using Isango.Entities.MyIsango;
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
    public class ProfileServiceTest : BaseTest
    {
        private IProfilePersistence _profilePersistenceMocking;
        private ProfileService _profileServiceMocking;
        private IProfilePersistence _profilePersistenceMockingException;
        private ProfileService _profileServiceMockingException;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _profilePersistenceMocking = Substitute.For<IProfilePersistence>();
            _profilePersistenceMockingException = Substitute.For<IProfilePersistence>();
            var gatewayILogger = Substitute.For<ILogger>();

            _profileServiceMocking = new ProfileService(_profilePersistenceMocking, gatewayILogger);
            _profileServiceMockingException = new ProfileService(_profilePersistenceMockingException, gatewayILogger);
        }

        [Test]
        public void FetchAgentBookingDetailsTest()
        {
            var affiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            var bookingRefenceNumber = "B001111";
            var listBookingSummary = new List<MyBookingSummary> {
                new MyBookingSummary {
                    BookingId=2001,
                    BookingRefenceNumber=bookingRefenceNumber,
                    BookedProducts=new List<MyBookedProduct>()
                }
            };

            _profileServiceMocking.FetchAgentBookingDetailsAsync(1001, affiliateId, true).Result.ReturnsForAnyArgs(listBookingSummary);
            var result = _profileServiceMocking.FetchAgentBookingDetailsAsync(1001, affiliateId, true).Result;
            Assert.That(bookingRefenceNumber.Equals(result[0].BookingRefenceNumber));
        }

        [Test]
        public void FetchAgentBookingDetailsExceptionTest()
        {
            //Catch block scenario
            _profileServiceMockingException.FetchAgentBookingDetailsAsync(0, null, false).Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => _profileServiceMockingException.FetchAgentBookingDetailsAsync(0, null, false));
        }

        [Test]
        public void FetchUserBookingDetailsTest()
        {
            var affiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            var bookingRefenceNumber = "B001111";
            var listBookingSummary = new List<MyBookingSummary> {
                new MyBookingSummary {
                    BookingId=2001,
                    BookingRefenceNumber=bookingRefenceNumber,
                    BookedProducts=new List<MyBookedProduct>()
                }
            };

            _profileServiceMocking.FetchUserBookingDetailsAsync("hsharma12@isango.com", affiliateId, true).Result.ReturnsForAnyArgs(listBookingSummary);
            var result = _profileServiceMocking.FetchUserBookingDetailsAsync("hsharma12@isango.com", affiliateId, true).Result;
            Assert.That(bookingRefenceNumber.Equals(result[0].BookingRefenceNumber));
        }

        [Test]
        public void FetchUserBookingDetailsExceptionTest()
        {
            //Catch block scenario
            _profileServiceMockingException.FetchUserBookingDetailsAsync(null, null, false).Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => _profileServiceMockingException.FetchUserBookingDetailsAsync(null, null, false));
        }

        [Test]
        public void FetchUserEmailPreferencesTest()
        {
            var affiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            var questionText = "Test Question Text";
            var listEmailPreferences = new List<MyUserEmailPreferences> {
                new MyUserEmailPreferences {
                     QuestionId=5001,
                     QuestionOrder=5,
                     QuestionText=questionText
                }
            };
            _profileServiceMocking.FetchUserEmailPreferencesAsync(1001, affiliateId, "en").Result.ReturnsForAnyArgs(listEmailPreferences);
            var result = _profileServiceMocking.FetchUserEmailPreferencesAsync(1001, affiliateId, "en").Result;
            Assert.That(questionText.Equals(result[0].QuestionText));
        }

        [Test]
        public void FetchUserEmailPreferencesExceptionTest()
        {
            //Catch block scenario
            _profileServiceMockingException.FetchUserEmailPreferencesAsync(1001, null, null).Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => _profileServiceMockingException.FetchUserEmailPreferencesAsync(1001, null, null));
        }

        [Test]
        public void FetchUserInfoTest()
        {
            var isangoUser = new ISangoUser
            {
                FirstName = "Test User",
                EmailAddress = "test@test.com",
                Age = 28,
                City = "Mumbai"
            };

            _profileServiceMocking.FetchUserInfoAsync(1001).Result.ReturnsForAnyArgs(isangoUser);
            var result = _profileServiceMocking.FetchUserInfoAsync(1001).Result;
            Assert.That(isangoUser.City.Equals(result.City));
        }

        [Test]
        public void FetchUserInfoExceptionTest()
        {
            //Catch block scenario
            _profileServiceMockingException.FetchUserInfoAsync(1001).Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => _profileServiceMockingException.FetchUserInfoAsync(1001));
        }
    }
}