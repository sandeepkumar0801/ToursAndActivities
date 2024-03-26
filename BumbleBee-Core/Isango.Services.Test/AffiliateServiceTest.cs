using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Affiliate;
using Isango.Entities.Wrapper;
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
    public class AffiliateServiceTest : BaseTest
    {
        private AffiliateService _affiliateServiceForMocking;
        private IAffiliatePersistence _gatewayAffiliatePersistence;
        private IAffiliateCacheManager _gatewayAffiliateCacheManager;
        private AffiliateService _affiliateServiceForException;
        private IAffiliatePersistence _gatewayAffiliatePersistenceException;
        private IAffiliateCacheManager _gatewayAffiliateCacheManagerException;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _gatewayAffiliatePersistence = Substitute.For<IAffiliatePersistence>();
            _gatewayAffiliateCacheManager = Substitute.For<IAffiliateCacheManager>();

            _gatewayAffiliatePersistenceException = Substitute.For<IAffiliatePersistence>();
            _gatewayAffiliateCacheManagerException = Substitute.For<IAffiliateCacheManager>();
            var gatewayILogger = Substitute.For<ILogger>();

            _affiliateServiceForMocking = new AffiliateService(_gatewayAffiliatePersistence, _gatewayAffiliateCacheManager, gatewayILogger);
            _affiliateServiceForException = new AffiliateService(_gatewayAffiliatePersistenceException, _gatewayAffiliateCacheManagerException, gatewayILogger);
        }

        /// <summary>
        /// Test method to search by region
        /// </summary>
        [Test]
        [Ignore("Ignored")]
        public void GetAffliateInfoTest()
        {
            var domain = "www.isango.com";
            var alias = string.Empty;
            var widgetDate = string.Empty;
            var testResult =
                new Affiliate { Id = "5001" };
            _affiliateServiceForMocking.GetAffiliateInfoAsync(domain, alias, widgetDate).Result.ReturnsForAnyArgs(testResult);
            var result = _affiliateServiceForMocking.GetAffiliateInfoAsync(domain, alias, widgetDate).Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffliateInfoExceptionTest()
        {
            //Catch Block Scenario
            _affiliateServiceForMocking.GetAffiliateInfoAsync("1234", "1234", "1234").Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _affiliateServiceForMocking.GetAffiliateInfoAsync("1234", "1234", "1234"));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateFiltersAsyncTest()
        {
            var testResult = new CacheKey<AffiliateFilter>()
            {
                CacheKeyName = "test",
                CacheValue = new List<AffiliateFilter>
            {
                new AffiliateFilter
                {
                    Id = "1001",
                    AffiliateId = Guid.NewGuid().ToString(),
                    Activities = new List<int> {1001, 1002, 1003},
                    Regions = new List<int> {2001, 2002, 2003}
                }
            },
                Id = "1001"
            };

            _gatewayAffiliateCacheManager.GetAffiliateFilter("test").ReturnsForAnyArgs(testResult);
            var result = _affiliateServiceForMocking.GetAffiliateFiltersAsync().Result;
            Assert.That(result, Is.EqualTo(testResult.CacheValue));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateFiltersWithExceptionTest()
        {
            //Catch Block Scenario
            _affiliateServiceForMocking.GetAffiliateFiltersAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _affiliateServiceForMocking.GetAffiliateFiltersAsync());
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateFilterByIdTest()
        {
            var affiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            var listAffliate = new List<AffiliateFilter>() { new AffiliateFilter() { AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183" } };
            _gatewayAffiliatePersistence.GetAffiliateFilter().ReturnsForAnyArgs(listAffliate);
            var result = _affiliateServiceForMocking.GetAffiliateFilterByIdAsync(affiliateId).Result;
            Assert.AreEqual(result.AffiliateId, affiliateId);
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateFilterByIdWithExceptionTest()
        {
            //Catch Block Scenario
            _affiliateServiceForMocking.GetAffiliateFilterByIdAsync("5beef089-3e4e-4f0f-9fbf-99bf1f350183").Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _affiliateServiceForMocking.GetAffiliateFilterByIdAsync("5beef089-3e4e-4f0f-9fbf-99bf1f350183"));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateInformationest()
        {
            var affiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            var affiliate = new Affiliate() { Id = "5beef089-3e4e-4f0f-9fbf-99bf1f350183" };
            _gatewayAffiliatePersistence.GetAffiliateInformation(affiliateId, "en").ReturnsForAnyArgs(affiliate);
            var result = _affiliateServiceForMocking.GetAffiliateInformationAsync(affiliateId);
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateInformationExceptionTest()
        {
            var affiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            _affiliateServiceForMocking.GetAffiliateInformationAsync(affiliateId).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _affiliateServiceForMocking.GetAffiliateInformationAsync(affiliateId));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetWidgetPartnersTest()
        {
            var partner = new List<Partner> { new Partner { ID = "1001", HomePage = "test", Name = "partner", UrlPrefix = "isangotest", SiteRoot = "test" } };
            _gatewayAffiliatePersistence.GetWidgetPartners().ReturnsForAnyArgs(partner);
            var result = _affiliateServiceForMocking.GetWidgetPartnersAsync();
            Assert.That(result, Is.EqualTo(partner));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetWidgetPartnersExceptionTest()
        {
            _affiliateServiceForException.GetWidgetPartnersAsync().Throws(new Exception());
            Assert.Throws<Exception>(() => _affiliateServiceForException.GetWidgetPartnersAsync());
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateReleaseTagsTest()
        {
            var testResult = new List<AffiliateReleaseTag> { new AffiliateReleaseTag { AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183", AffiliateName = "testName", CompanyWebsite = "test.com", ReleaseTag = "tag" } };
            _gatewayAffiliatePersistence.GetAffiliateReleaseTags().ReturnsForAnyArgs(testResult);
            var result = _affiliateServiceForMocking.GetAffiliateReleaseTagsAsync().Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateReleaseTagsAsyncExceptionTest()
        {
            _affiliateServiceForException.GetAffiliateReleaseTagsAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _affiliateServiceForException.GetAffiliateReleaseTagsAsync());
        }

        [Test]
        [Ignore("Ignored")]
        public void UpdateAffiliateReleaseTagsTest()
        {
            var testResult = "success";
            _gatewayAffiliatePersistence.UpdateAffiliateReleaseTags("5beef089-3e4e-4f0f-9fbf-99bf1f350183", "test", false).ReturnsForAnyArgs(testResult);
            var result = _affiliateServiceForMocking.UpdateAffiliateReleaseTagsAsync("5beef089-3e4e-4f0f-9fbf-99bf1f350183", "test", false).Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        [Ignore("Ignored")]
        public void UpdateAffiliateReleaseTagsExceptionTest()
        {
            _affiliateServiceForMocking.UpdateAffiliateReleaseTagsAsync("5beef089-3e4e-4f0f-9fbf-99bf1f350183", string.Empty, false).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _affiliateServiceForMocking.UpdateAffiliateReleaseTagsAsync("5beef089-3e4e-4f0f-9fbf-99bf1f350183", string.Empty, false));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetWLAffiliateInfoTest()
        {
            var testResult = new List<Affiliate>
            {
                new Affiliate
                {
                    Id = "123", Name = "Test",
                    AffiliateConfiguration = new AffiliateConfiguration {IsB2BAffiliate = true}
                }
            };
            _gatewayAffiliatePersistence.GetWLAffiliateInfo().ReturnsForAnyArgs(testResult);
            var result = _affiliateServiceForMocking.GetWLAffiliateInfoAsync().Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetWLAffiliateInfoExceptionTest()
        {
            _affiliateServiceForException.GetWLAffiliateInfoAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _affiliateServiceForException.GetWLAffiliateInfoAsync());
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateIdByUserIdTest()
        {
            var testResult = "success";
            _gatewayAffiliatePersistence.GetAffiliateIdByUserId("test").ReturnsForAnyArgs(testResult);
            var result = _affiliateServiceForMocking.GetAffiliateIdByUserIdAsync("test").Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateIdByUserIdExceptionTest()
        {
            _affiliateServiceForMocking.GetAffiliateIdByUserIdAsync("").Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _affiliateServiceForMocking.GetAffiliateIdByUserIdAsync(""));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateInfo_NegativeTest()
        {
            var domain = string.Empty;
            var alias = string.Empty;
            var widgetDate = "1234";
            var testResult =
                new Affiliate { Id = "5001" };
            _affiliateServiceForMocking.GetAffiliateInfoAsync(domain, alias, widgetDate).Result.ReturnsForAnyArgs(testResult);
            var result = _affiliateServiceForMocking.GetAffiliateInfoAsync(domain, alias, widgetDate).Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        [Ignore("Ignored")]
        public void GetAffiliateFiltersAsync_NegativeTest()
        {
            var affiliateFilterList = new List<AffiliateFilter>
            {
                new AffiliateFilter
                {
                    Id = "1001",
                    AffiliateId = Guid.NewGuid().ToString(),
                    Activities = new List<int> {1001, 1002, 1003},
                    Regions = new List<int> {2001, 2002, 2003}
                }
            };
            var testResult = new CacheKey<AffiliateFilter>()
            {
                CacheKeyName = "test",
                CacheValue = affiliateFilterList,
                Id = "1001"
            };

            _gatewayAffiliateCacheManagerException.GetAffiliateFilter("test").ReturnsForAnyArgs(new CacheKey<AffiliateFilter> { CacheValue = new List<AffiliateFilter>() });
            _gatewayAffiliatePersistenceException.GetAffiliateFilter().ReturnsForAnyArgs(affiliateFilterList);
            var result = _affiliateServiceForException.GetAffiliateFiltersAsync().Result;
            Assert.That(result, Is.EqualTo(testResult.CacheValue));
        }
    }
}