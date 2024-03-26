using Autofac;
using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Bokun;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Entities.Payment;
using Isango.Entities.Region;
using Isango.Entities.SiteMap;
using Isango.Entities.Wrapper;
using Isango.Persistence.Contract;
using Isango.Register;
using Isango.Service;
using Isango.Service.Contract;
using Logger.Contract;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Isango.Services.Test
{
    [TestFixture]
    public class MasterServiceTest : BaseTest
    {
        private IMasterService _masterService;
        private MasterService _masterServiceForMocking;
        private IMasterPersistence gatewayPersistenceMocking;
        private IMasterCacheManager gatewayCacheManagerMocking;
        private IAgeGroupsCacheManager gatewayAgeGroupsCacheManagerMocking;
        private IPickupLocationsCacheManager pickupLocationsCacheManagerMocking;

        private MasterService _masterServiceForMockingException;
        private IMasterPersistence gatewayPersistenceMockingException;
        private IMasterCacheManager gatewayCacheManagerMockingException;
        private IAgeGroupsCacheManager gatewayAgeGroupsCacheManagerMockingException;
        private IPickupLocationsCacheManager pickupLocationsCacheManagerMockingException;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();
            gatewayPersistenceMocking = Substitute.For<IMasterPersistence>();
            gatewayCacheManagerMocking = Substitute.For<IMasterCacheManager>();
            gatewayAgeGroupsCacheManagerMocking = Substitute.For<IAgeGroupsCacheManager>();
            pickupLocationsCacheManagerMocking = Substitute.For<IPickupLocationsCacheManager>();

            gatewayPersistenceMockingException = Substitute.For<IMasterPersistence>();
            gatewayCacheManagerMockingException = Substitute.For<IMasterCacheManager>();
            gatewayAgeGroupsCacheManagerMockingException = Substitute.For<IAgeGroupsCacheManager>();
            pickupLocationsCacheManagerMockingException = Substitute.For<IPickupLocationsCacheManager>();

            var gatewayILogger = Substitute.For<ILogger>();

            _masterServiceForMocking = new MasterService(gatewayPersistenceMocking, gatewayCacheManagerMocking, gatewayILogger, gatewayAgeGroupsCacheManagerMocking, pickupLocationsCacheManagerMocking,null);

            _masterServiceForMockingException = new MasterService(gatewayPersistenceMockingException, gatewayCacheManagerMockingException, gatewayILogger, gatewayAgeGroupsCacheManagerMockingException, pickupLocationsCacheManagerMockingException,null);

            using (var scope = _container.BeginLifetimeScope())
            {
                _masterService = scope.Resolve<IMasterService>();
            }
        }

        [Test]
        public void GetCurrenciesFromDbTest()
        {
            var affiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            var listCurrency = new List<Currency> { new Currency { Name = "INR", IsoCode = "rupees" } };
            gatewayPersistenceMocking.GetCurrencies(affiliateId).ReturnsForAnyArgs(listCurrency);
            var result = _masterServiceForMocking.GetCurrenciesAsync(affiliateId).Result;
            Assert.That(result, Is.EqualTo(listCurrency));
        }

        [Test]
        public void GetCurrencyTest()
        {
            var testResult = new Currency { Name = "INR", IsoCode = "rupees" };
            gatewayPersistenceMocking.GetCurrency("INR").ReturnsForAnyArgs(testResult);
            var result = _masterServiceForMocking.GetCurrencyAsync("INR").Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void GetCurrencyExceptionTest()
        {
            //Catch block scenario
            _masterServiceForMocking.GetCurrencyAsync("test").Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMocking.GetCurrencyAsync("test"));
        }

        [Test]
        public void GetCurrencyCodeForCountryTest()
        {
            var currencyCode = "AUD";
            gatewayPersistenceMocking.GetCurrencyCodeForCountry("AU").ReturnsForAnyArgs(currencyCode);
            var result = _masterServiceForMocking.GetCurrencyCodeForCountryAsync("AU").Result;
            Assert.That(result, Is.EqualTo(currencyCode));
        }

        [Test]
        public void GetCurrencyCodeForCountryExceptionTest()
        {
            //Catch block scenario
            _masterServiceForMocking.GetCurrencyCodeForCountryAsync("test").Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMocking.GetCurrencyCodeForCountryAsync("test"));
        }

        [Test]
        public void GetRegionTest()
        {
            var testResult = new List<LatLongVsurlMapping>() { new LatLongVsurlMapping() { CityName = "testCity", CountryName = "Testcountry", LanguageCode = "en", Latitude = 48.2, Longitude = 56.23, RegionId = 123 } };
            gatewayPersistenceMocking.GetRegionData().ReturnsForAnyArgs(testResult);
            var result = _masterServiceForMocking.GetRegionAsync().Result;
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public void GetRegionExceptionTest()
        {
            //Catch block scenario
            _masterServiceForMockingException.GetRegionAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMockingException.GetRegionAsync());
        }

        [Test]
        public void GetCountriesTest()
        {
            const string langCode = "En";
            var listRegion = new List<Region> { new Region { Name = "Paris", Id = 20001, Type = RegionType.City } };
            gatewayPersistenceMocking.GetCountries(langCode).ReturnsForAnyArgs(listRegion);
            var result = _masterServiceForMocking.GetCountriesAsync(langCode).Result;
            Assert.That(result, Is.EqualTo(listRegion));
        }

        [Test]
        public void GetCountriesExceptionTest()
        {
            //Catch block scenario
            _masterServiceForMocking.GetCountriesAsync("ex").Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMocking.GetCountriesAsync("ex"));
        }

        [Test]
        public void GetSupportPhonesWithCountryCodeTest()
        {
            const string affiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            const string language = "en";
            var dictionary = new Dictionary<string, string> { { "TestKey", "TestValue" } };
            gatewayPersistenceMocking.GetSupportPhonesWithCountryCode(affiliateId, language).ReturnsForAnyArgs(dictionary);
            var result = _masterServiceForMocking.GetSupportPhonesWithCountryCodeAsync(affiliateId, language).Result;
            Assert.That(result, Is.EqualTo(dictionary));
        }

        [Test]
        public void GetSupportPhonesWithCountryCodeExceptionTest()
        {
            const string affiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            const string language = "en";
            //Catch block scenario
            _masterServiceForMockingException.GetSupportPhonesWithCountryCodeAsync(affiliateId, language).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMockingException.GetSupportPhonesWithCountryCodeAsync(affiliateId, language));
        }

        [Test]
        public void GetAllCrossSellProductsTest()
        {
            var dictionary = new Dictionary<int, List<CrossSellProduct>>
            {
                { 1, new List<CrossSellProduct>() { new CrossSellProduct() { Id = 2, Priority = 3, AttractionIDs = new List<int>() { 1, 2, 3 } } } }
            };
            gatewayPersistenceMocking.GetAllCrossSellProducts().ReturnsForAnyArgs(dictionary);
            var result = _masterServiceForMocking.GetAllCrossSellProductsAsync().Result;
            Assert.That(result, Is.EqualTo(dictionary));
        }

        [Test]
        public void GetAllCrossSellProductsExceptionTest()
        {
            //Catch block scenario
            _masterServiceForMockingException.GetAllCrossSellProductsAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMockingException.GetAllCrossSellProductsAsync());
        }

        [Test]
        public void GetSiteMapDataTest()
        {
            var AffiliateID = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            var Language = "en";
            var siteMapData = new List<SiteMapData>() { new SiteMapData() { Order = 2, ParentId = 12, RegionId = 670, RegionName = "test", RegionType = "type", Url = "testUrl" } };
            var tuple = Tuple.Create(siteMapData, 56);
            gatewayPersistenceMocking.GetSiteMapData(AffiliateID, Language).ReturnsForAnyArgs(tuple);
            var result = _masterServiceForMocking.GetSiteMapDataAsync(AffiliateID, Language).GetAwaiter().GetResult();
            Assert.That(result, Is.EqualTo(tuple));
        }

        [Test]
        public void GetSiteMapDataExceptionTest()
        {
            //Catch block scenario
            var AffiliateID = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            var Language = "en";
            _masterServiceForMocking.GetSiteMapDataAsync(AffiliateID, Language).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMocking.GetSiteMapDataAsync(AffiliateID, Language));
        }

        [Test]
        public void LoadIndexedAttractionToRegionUrlsTest()
        {
            var dictionary = new Dictionary<string, string>();
            gatewayPersistenceMocking.LoadIndexedAttractionToRegionUrls().ReturnsForAnyArgs(dictionary);
            var result = _masterServiceForMocking.LoadIndexedAttractionToRegionUrlsAsync().Result;
            Assert.That(result, Is.EqualTo(dictionary));
        }

        [Test]
        public void LoadIndexedAttractionToRegionUrlsExceptionTest()
        {
            //Catch block scenario
            _masterServiceForMockingException.LoadIndexedAttractionToRegionUrlsAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMockingException.LoadIndexedAttractionToRegionUrlsAsync());
        }

        [Test]
        public void GetRegionIdFromGeotreeTest()
        {
            var testResult = 156;
            gatewayPersistenceMocking.GetRegionIdFromGeotree(110).ReturnsForAnyArgs(testResult);
            var result = _masterServiceForMocking.GetRegionIdFromGeotreeAsync(110).Result;
            Assert.AreEqual(testResult, result);
        }

        [Test]
        public void GetRegionIdFromGeotreeExceptionTest()
        {
            //Catch block scenario
            _masterServiceForMocking.GetRegionIdFromGeotreeAsync(0).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMocking.GetRegionIdFromGeotreeAsync(0));
        }

        [Test]
        public void GetFilteredThemeparkTicketsTest()
        {
            var testResult = new CacheKey<Entities.TicketByRegion>() { CacheKeyName = "test", CacheValue = new List<Entities.TicketByRegion>() { new Entities.TicketByRegion() { CountryCode = "NZ", ThemeparkTicket = new Entities.ThemeparkTicket() { City = 5 } } } };
            gatewayCacheManagerMocking.GetFilteredTickets("testFilteredTickets").ReturnsForAnyArgs(testResult);
            var result = _masterServiceForMocking.GetFilteredThemeparkTicketsAsync().Result;
            Assert.IsTrue(result?.Count > 0);
        }

        [Test]
        public void GetFilteredThemeparkTicketsExceptionTest()
        {
            //Catch block scenario
            gatewayCacheManagerMockingException.GetFilteredTickets("test").ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMockingException.GetFilteredThemeparkTicketsAsync());
        }

        [Test]
        public void GetSupportedLanguagesTest()
        {
            var listLanguage = new List<Language>() { new Language() { Code = "en", Description = "discription" } };
            gatewayPersistenceMocking.GetSupportedLanguages().ReturnsForAnyArgs(listLanguage);
            var result = _masterServiceForMocking.GetSupportedLanguagesAsync().Result;
            Assert.That(result, Is.EqualTo(listLanguage));
        }

        [Test]
        public void GetSupportedLanguagesExceptionTest()
        {
            //Catch block scenario
            _masterServiceForMockingException.GetSupportedLanguagesAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMockingException.GetSupportedLanguagesAsync());
        }

        [Test]
        public void LoadExchangeRatesTest()
        {
            var testResult = new List<CurrencyExchangeRates>() { new CurrencyExchangeRates() { ExchangeRate = 1, FromCurrencyCode = "en", ToCurrencyCode = "sp" } };
            gatewayPersistenceMocking.LoadExchangeRates().ReturnsForAnyArgs(testResult);
            var result = _masterServiceForMocking.LoadCurrencyExchangeRatesAsync().Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void LoadExchangeRatesExceptionTest()
        {
            //Catch block scenario
            _masterServiceForMockingException.LoadCurrencyExchangeRatesAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMockingException.LoadCurrencyExchangeRatesAsync());
        }

        [Test]
        public void RegionVsDestinationTest()
        {
            var testResult = new List<MappedRegion>() { new MappedRegion() { DestinationCode = string.Empty, RegionId = 6710, RegionName = "Newzeland" } };
            gatewayPersistenceMocking.RegionVsDestination().ReturnsForAnyArgs(testResult);
            var result = _masterServiceForMocking.LoadRegionVsDestinationAsync().Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void RegionVsDestinationExceptionTest()
        {
            //Catch block scenario
            gatewayPersistenceMockingException.RegionVsDestination().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMockingException.LoadRegionVsDestinationAsync());
        }

        [Test]
        public void LoadMappedLanguageTest()
        {
            var testResult = new List<MappedLanguage>() { new MappedLanguage() { AffiliateId = string.Empty, GliLanguageCode = 2, IsangoLanguageCode = "en", SupplierLanguageCode = "eur" } };
            gatewayPersistenceMocking.LoadMappedLanguage().ReturnsForAnyArgs(testResult);
            var result = _masterServiceForMocking.LoadMappedLanguageAsync().Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void LoadMappedLanguageExceptionTest()
        {
            //Catch block scenario
            _masterServiceForMockingException.LoadMappedLanguageAsync().Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => _masterServiceForMockingException.LoadMappedLanguageAsync());
        }

        /// <summary>
        /// Test case to get conversion factor
        /// </summary>
        [Test]
        public void GetConversionFactorTest()
        {
            var clientInfo = new ClientInfo()
            {
                Currency = new Currency()
                {
                    IsoCode = "NZD"
                }
            };

            //First Scenario
            var testResult = new List<CurrencyExchangeRates>() { new CurrencyExchangeRates() { ExchangeRate = 1, FromCurrencyCode = "en", ToCurrencyCode = "sp" } };
            gatewayPersistenceMocking.LoadExchangeRates().ReturnsForAnyArgs(testResult);
            var result = _masterServiceForMocking.GetConversionFactorAsync("EUR", "NZD").Result;
            Assert.IsNotNull(result);

            //Second Scenario
            result = _masterService.GetConversionFactorAsync("EUR", "EUR").Result;
            Assert.IsNotNull(result);

            //Third Scenario
            result = _masterService.GetConversionFactorAsync("baseCurrencyCode", "currencyCode").Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetLocalizedDestinationsAsyncTest()
        {
            var testResult = new List<LocalizedDestinations>() { new LocalizedDestinations() { Language = "en", Destinations = new List<Destination>() { new Destination() { Id = 14, Name = "Australia" } } } };
            gatewayPersistenceMocking.GetLocalizedDestinations().Returns(testResult);
            var result = _masterServiceForMocking.GetLocalizedDestinationsAsync().Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void GetLocalizedCategoriesAsync()
        {
            var testResult = new List<LocalizedCategories>() { new LocalizedCategories() { Language = "en", DestinationCategories = new Dictionary<int, Dictionary<int, string>>() } };
            gatewayPersistenceMocking.GetLocalizedCategories().ReturnsForAnyArgs(testResult);
            var result = _masterServiceForMocking.GetLocalizedCategoriesAsync().Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void LoadNetPriceMasterDataAsync()
        {
            var testResult = new List<NetPriceMasterData>
            {
                new NetPriceMasterData
                {
                   AffiliateId="test",
                   ApiType=APIType.Bokun,
                   CommisionPercentage=10,
                   CostPrice=20,
                   CurrencyCode="USD",
                   ProductId=1503
                }
            };
            gatewayPersistenceMocking.LoadNetPriceMasterData().ReturnsForAnyArgs(testResult);
            var result = _masterServiceForMocking.LoadNetPriceMasterDataAsync().Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void GetBokunPriceCategoryByActivityAsync()
        {
            var testResult = new List<PriceCategory>
            {
                new PriceCategory
                {
                    AgeGroupId=1,
                    OptionId=1101,
                    PriceCategoryId=11,
                    ServiceId=1503,
                    ServiceOptionCode=1001,
                    Title="Adult"
                }
            };
            gatewayPersistenceMocking.GetBokunPriceCategoryByActivity().ReturnsForAnyArgs(testResult);
            var result = _masterServiceForMocking.GetBokunPriceCategoryByActivityAsync().Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void GetPassengerMappingTest()
        {
            var result = _masterService.GetPassengerMapping(APIType.Goldentours).GetAwaiter().GetResult();
            Assert.IsNotNull(result);
        }

        #region PrivateMethods

        private ClientInfo PrepareClientInfo()
        {
            var clientInfo = new ClientInfo
            {
                AffiliateDisplayName = "Isango",
                AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                AffiliateName = "Isango",
                AffiliatePartner = null,
                B2BAffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                CityCode = null,
                CompanyAlias = "ien",
                CountryIp = "GB",
                DiscountCode = "",
                DiscountCodePercentage = 0M,
                FacebookAppId = "656660554485822",
                FacebookAppSecret = "af34c66444b9c19d38bc4e55cf2d54cf",
                GtmIdentifier = "GTM-PSQPTWZ",
                IsB2BAffiliate = false,
                IsB2BNetPriceAffiliate = false,
                IsSupplementOffer = true,
                LanguageCode = "en",
                LineOfBusiness = "TOURS & ACTIVITIES - isango!",
                PaymentMethodType = PaymentMethodType.Transaction,
                WidgetDate = DateTime.Now, // This date value is not valid
                Currency = new Currency
                {
                    IsPostFix = false,
                    IsoCode = "GBP",
                    Name = "GBP",
                    Symbol = "£"
                }
            };

            return clientInfo;
        }

        #endregion PrivateMethods
    }
}