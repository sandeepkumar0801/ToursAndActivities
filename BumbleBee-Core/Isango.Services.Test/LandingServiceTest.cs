using CacheManager.Contract;
using Isango.Entities;
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
    public class LandingServiceTest : BaseTest
    {
        private LandingService _landingServiceForMocking;
        private ILandingCacheManager _gatewayLandingCacheManagerForMocking;
        private ILandingPersistence _gatewayLandingPersistenceForMocking;
        private LandingService _landingServiceForMockingException;
        private ILandingCacheManager _gatewayLandingCacheManagerForMockingException;
        private ILandingPersistence _gatewayLandingPersistenceForMockingException;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _gatewayLandingCacheManagerForMocking = Substitute.For<ILandingCacheManager>();
            _gatewayLandingPersistenceForMocking = Substitute.For<ILandingPersistence>();
            _gatewayLandingCacheManagerForMockingException = Substitute.For<ILandingCacheManager>();
            _gatewayLandingPersistenceForMockingException = Substitute.For<ILandingPersistence>();
            var gatewayILogger = Substitute.For<ILogger>();

            _landingServiceForMocking = new LandingService(_gatewayLandingCacheManagerForMocking, _gatewayLandingPersistenceForMocking, gatewayILogger);
            _landingServiceForMockingException = new LandingService(_gatewayLandingCacheManagerForMockingException, _gatewayLandingPersistenceForMockingException, gatewayILogger);
        }

        [Test]
        public void TestGetPopularDestinationsList()
        {
            var testResult = new List<LocalizedMerchandising>() { new LocalizedMerchandising() { Id = 111, Language = "en", AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183", Name = "Test", SourceMarket = "gb", Type = "D" } };
            _gatewayLandingPersistenceForMocking.LoadLocalizedMerchandising().Returns(testResult);
            var result = _landingServiceForMocking.GetPopularDestinationsListAsync("gb", "en", "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183").Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void TestGetPopularDestinationsListException()
        {
            // Catch block scenario
            _landingServiceForMockingException.GetPopularDestinationsListAsync(null, null, null).ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _landingServiceForMockingException.GetPopularDestinationsListAsync(null, null, null));
        }

        [Test]
        public void TestGetPopularAttractionList()
        {
            var testResult = new List<LocalizedMerchandising>() { new LocalizedMerchandising() { Id = 111, Language = "en", AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183", Name = "Test", SourceMarket = "gb", Type = "A" } };
            _gatewayLandingPersistenceForMocking.LoadLocalizedMerchandising().ReturnsForAnyArgs(testResult);
            var result = _landingServiceForMocking.GetPopularAttractionListAsync("gb", "en", "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183").Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void TestGetPopularAttractionListException()
        {
            // Catch block scenario
            _landingServiceForMockingException.LoadLocalizedMerchandisingAsync().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _landingServiceForMockingException.GetPopularAttractionListAsync("gb", "en", "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183"));
        }

        [Test]
        public void TestLocalizedMerchandisingCache()
        {
            var localizedMerchandisings = new List<LocalizedMerchandising>() { new LocalizedMerchandising() };
            _gatewayLandingPersistenceForMocking.LoadLocalizedMerchandising().ReturnsForAnyArgs(localizedMerchandisings);
            var result = _landingServiceForMocking.LoadLocalizedMerchandisingAsync().Result;
            Assert.That(result, Is.EqualTo(localizedMerchandisings));
        }

        [Test]
        public void TestLocalizedMerchandising()
        {
            var testResult = new List<LocalizedMerchandising>() { new LocalizedMerchandising() { Id = 111, Language = "en", AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183", Name = "Test", SourceMarket = "gb", Type = "A" } };
            _gatewayLandingPersistenceForMocking.LoadLocalizedMerchandising().Returns(testResult);
            var result = _landingServiceForMocking.LoadLocalizedMerchandisingAsync().Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void TestLocalizedMerchandising_Exception()
        {
            // Catch block scenario
            _landingServiceForMockingException.LoadLocalizedMerchandisingAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _landingServiceForMockingException.LoadLocalizedMerchandisingAsync());
        }
    }
}