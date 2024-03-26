using Isango.Entities;
using Isango.Persistence.Contract;
using Isango.Service;
using Logger.Contract;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;

namespace Isango.Services.Test
{
    [TestFixture]
    public class ManageIdentityServiceTest : BaseTest
    {
        private ManageIdentityService _manageIdentityServiceForMocking;
        private IManageIdentityPersistence _gatewayManageIdentityPersistence;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _gatewayManageIdentityPersistence = Substitute.For<IManageIdentityPersistence>();
            var gatewayILogger = Substitute.For<ILogger>();

            _manageIdentityServiceForMocking = new ManageIdentityService(_gatewayManageIdentityPersistence, gatewayILogger);
        }

        [Test]
        public void TestSubscribeUser_Success()
        {
            var citeria = PrepareNewsLetterData();
            _gatewayManageIdentityPersistence.LogForConsentUser(citeria);
            var result = _manageIdentityServiceForMocking.SubscribeNewsLetterAsync(PrepareNewsLetterData()).Result;
            Assert.That(result.Equals("subscribed"));
        }

        [Test]
        public void TestSubscribeUser_Fail()
        {
            _gatewayManageIdentityPersistence.SubscribeToNewsLetter(PrepareNewsLetterCriteria()).ReturnsForAnyArgs("test");
            var result = _manageIdentityServiceForMocking.SubscribeNewsLetterAsync(PrepareNewsLetterData()).Result;
            Assert.That(!result.Equals("subscribed"));
        }

        [Test]
        public void TestSubscribeUserException()
        {
            _gatewayManageIdentityPersistence.SubscribeToNewsLetter(null).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _manageIdentityServiceForMocking.SubscribeNewsLetterAsync(null));
        }

        [Test]
        public void TestIsValidNewsletterSubscriberAsync()
        {
            _gatewayManageIdentityPersistence.IsValidNewsletterSubscriber("test@test.com").ReturnsForAnyArgs(true);
            var result = _manageIdentityServiceForMocking.IsValidNewsletterSubscriberAsync("test@test.com").Result;
            Assert.IsTrue(result);
        }

        [Test]
        public void TestIsValidNewsletterSubscriberAsyncException()
        {
            _manageIdentityServiceForMocking.IsValidNewsletterSubscriberAsync(string.Empty).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _manageIdentityServiceForMocking.IsValidNewsletterSubscriberAsync(string.Empty));
        }

        private NewsLetterCriteria PrepareNewsLetterCriteria()
        {
            return new NewsLetterCriteria
            {
                EmailId = "test@test.com",
                LanguageCode = "en",
                AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183",
                CountryId = 0,
                CountryName = "India",
                IsNbVerified = true,
                ConsentUser = true
            };
        }

        private NewsLetterData PrepareNewsLetterData()
        {
            return new NewsLetterData
            {
                EmailId = "test@test.com",
                LanguageCode = "en",
                AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183",
                Name = "test",
                CustomerOrigin = "GBP"
            };
        }
    }
}