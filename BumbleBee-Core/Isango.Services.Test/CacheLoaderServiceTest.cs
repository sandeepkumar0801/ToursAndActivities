using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Availability;
using Isango.Entities.Enums;
using Isango.Entities.GoldenTours;
using Isango.Entities.HotelBeds;
using Isango.Entities.PricingRules;
using Isango.Entities.Region;
using Isango.Entities.Tiqets;
using Isango.Entities.Wrapper;
using Isango.Persistence.Contract;
using Isango.Service;
using Isango.Service.Contract;
using Logger.Contract;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using AgeGroup = Isango.Entities.AgeGroup;
using PassengerInfo = Isango.Entities.Booking.PassengerInfo;

namespace Isango.Services.Test
{
    [TestFixture]
    public class CacheLoaderServiceTest : BaseTest
    {
        #region Try block testing

        private CacheLoaderService _cacheLoaderService;
        private ICacheLoaderService _cacheLoaderServiceMocking;
        private IMasterPersistence _masterPersistenceMock;
        private IActivityPersistence _activityPersistenceMock;
        private ILandingPersistence _landingPersistenceMock;
        private IAffiliatePersistence _affiliatePersistenceMock;
        private IAgeGroupsCacheManager _ageGroupsCacheManagerMock;
        private IHbProductMappingCacheManager _hbProductMappingCacheManagerMock;
        private IHotelBedsActivitiesCacheManager _hotelBedsActivitiesCacheManagerMock;
        private IMemCache _memCacheMock;
        private INetPriceCacheManager _netPriceCacheManagerMock;
        private ISimilarProductsRegionAttractionCacheManager _similarProductsRegionAttractionCacheManagerMock;
        private IMasterCacheManager _masterCacheManagerMock;
        private IActivityCacheManager _activityCacheManagerMock;
        private IAffiliateCacheManager _affiliateCacheManagerMock;
        private IFareHarborCustomerPrototypesCacheManager _fareHarborCustomerPrototypesCacheManagerMock;
        private IFareHarborUserKeysCacheManager _fareHarborUserKeysCacheManagerMock;
        private IPriceRuleEnginePersistence _priceRuleEnginePersistenceMock;
        private IPricingRulesCacheManager _pricingRuleCacheManagerMock;
        private ICalendarAvailabilityCacheManager _calendarAvailabilityCacheManagerMock;
        private IPickupLocationsCacheManager _pickupLocationsCacheManagerMock;
        private ITiqetsPaxMappingCacheManager _tiqetsPaxMappingCacheManagerMock;
        private IGoogleMapsPersistence _googleMapsPersistenceMock;

        #endregion Try block testing

        #region Catch block testing

        private CacheLoaderService _cacheLoaderServiceException;
        private IMasterPersistence _masterPersistenceMockException;
        private IActivityPersistence _activityPersistenceMockException;
        private ILandingPersistence _landingPersistenceMockException;
        private IAffiliatePersistence _affiliatePersistenceMockException;
        private IAgeGroupsCacheManager _ageGroupsCacheManagerMockException;
        private IHbProductMappingCacheManager _hbProductMappingCacheManagerMockException;
        private IHotelBedsActivitiesCacheManager _hotelBedsActivitiesCacheManagerMockException;
        private IMemCache _memCacheMockException;
        private INetPriceCacheManager _netPriceCacheManagerMockException;
        private ISimilarProductsRegionAttractionCacheManager _similarProductsRegionAttractionCacheManagerMockException;
        private IMasterCacheManager _masterCacheManagerMockException;
        private IActivityCacheManager _activityCacheManagerMockException;
        private IAffiliateCacheManager _affiliateCacheManagerMockException;
        private IFareHarborCustomerPrototypesCacheManager _fareHarborCustomerPrototypesCacheManagerMockException;
        private IFareHarborUserKeysCacheManager _fareHarborUserKeysCacheManagerMockException;
        private IPriceRuleEnginePersistence _priceRuleEnginePersistenceMockException;
        private IPricingRulesCacheManager _pricingRulesCacheManagerMockException;
        private ICalendarAvailabilityCacheManager _calendarAvailabilityCacheManagerMockException;
        private IPickupLocationsCacheManager _pickupLocationsCacheManagerMockException;
        private ITiqetsPaxMappingCacheManager _tiqetsPaxMappingCacheManagerMockException;
        private IGoogleMapsPersistence _googleMapsPersistenceMockException;

        #endregion Catch block testing

        private ILogger _logger;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            #region Try block initialisation

            _cacheLoaderServiceMocking = Substitute.For<ICacheLoaderService>();
            _masterPersistenceMock = Substitute.For<IMasterPersistence>();
            _activityPersistenceMock = Substitute.For<IActivityPersistence>();
            _landingPersistenceMock = Substitute.For<ILandingPersistence>();
            _affiliatePersistenceMock = Substitute.For<IAffiliatePersistence>();
            _ageGroupsCacheManagerMock = Substitute.For<IAgeGroupsCacheManager>();
            _hbProductMappingCacheManagerMock = Substitute.For<IHbProductMappingCacheManager>();
            _hotelBedsActivitiesCacheManagerMock = Substitute.For<IHotelBedsActivitiesCacheManager>();
            _memCacheMock = Substitute.For<IMemCache>();
            _netPriceCacheManagerMock = Substitute.For<INetPriceCacheManager>();
            _similarProductsRegionAttractionCacheManagerMock = Substitute.For<ISimilarProductsRegionAttractionCacheManager>();
            _masterCacheManagerMock = Substitute.For<IMasterCacheManager>();
            _activityCacheManagerMock = Substitute.For<IActivityCacheManager>();
            _affiliateCacheManagerMock = Substitute.For<IAffiliateCacheManager>();
            _fareHarborCustomerPrototypesCacheManagerMock = Substitute.For<IFareHarborCustomerPrototypesCacheManager>();
            _fareHarborUserKeysCacheManagerMock = Substitute.For<IFareHarborUserKeysCacheManager>();
            _priceRuleEnginePersistenceMock = Substitute.For<IPriceRuleEnginePersistence>();
            _pricingRuleCacheManagerMock = Substitute.For<IPricingRulesCacheManager>();
            _calendarAvailabilityCacheManagerMock = Substitute.For<ICalendarAvailabilityCacheManager>();
            _pickupLocationsCacheManagerMock = Substitute.For<IPickupLocationsCacheManager>();
            _tiqetsPaxMappingCacheManagerMock = Substitute.For<ITiqetsPaxMappingCacheManager>();
            _googleMapsPersistenceMock = Substitute.For<IGoogleMapsPersistence>();

            #endregion Try block initialisation

            #region Catch Block initialisation

            _masterPersistenceMockException = Substitute.For<IMasterPersistence>();
            _activityPersistenceMockException = Substitute.For<IActivityPersistence>();
            _landingPersistenceMockException = Substitute.For<ILandingPersistence>();
            _affiliatePersistenceMockException = Substitute.For<IAffiliatePersistence>();
            _ageGroupsCacheManagerMockException = Substitute.For<IAgeGroupsCacheManager>();
            _hbProductMappingCacheManagerMockException = Substitute.For<IHbProductMappingCacheManager>();
            _hotelBedsActivitiesCacheManagerMockException = Substitute.For<IHotelBedsActivitiesCacheManager>();
            _memCacheMockException = Substitute.For<IMemCache>();
            _netPriceCacheManagerMockException = Substitute.For<INetPriceCacheManager>();
            _similarProductsRegionAttractionCacheManagerMockException = Substitute.For<ISimilarProductsRegionAttractionCacheManager>();
            _masterCacheManagerMockException = Substitute.For<IMasterCacheManager>();
            _activityCacheManagerMockException = Substitute.For<IActivityCacheManager>();
            _affiliateCacheManagerMockException = Substitute.For<IAffiliateCacheManager>();
            _fareHarborCustomerPrototypesCacheManagerMockException = Substitute.For<IFareHarborCustomerPrototypesCacheManager>();
            _fareHarborUserKeysCacheManagerMockException = Substitute.For<IFareHarborUserKeysCacheManager>();
            _priceRuleEnginePersistenceMockException = Substitute.For<IPriceRuleEnginePersistence>();
            _pricingRulesCacheManagerMockException = Substitute.For<IPricingRulesCacheManager>();
            _calendarAvailabilityCacheManagerMockException = Substitute.For<ICalendarAvailabilityCacheManager>();
            _pickupLocationsCacheManagerMockException = Substitute.For<IPickupLocationsCacheManager>();
            _tiqetsPaxMappingCacheManagerMockException = Substitute.For<ITiqetsPaxMappingCacheManager>();
            _googleMapsPersistenceMockException = Substitute.For<IGoogleMapsPersistence>();

            #endregion Catch Block initialisation

            _logger = Substitute.For<ILogger>();

            #region Try

            _cacheLoaderService = new CacheLoaderService(_masterPersistenceMock, _activityPersistenceMock, _affiliatePersistenceMock,
                     _ageGroupsCacheManagerMock, _hbProductMappingCacheManagerMock, _hotelBedsActivitiesCacheManagerMock, _memCacheMock,
                    _netPriceCacheManagerMock, _similarProductsRegionAttractionCacheManagerMock, _masterCacheManagerMock,
                    _activityCacheManagerMock, _affiliateCacheManagerMock, _fareHarborCustomerPrototypesCacheManagerMock,
                    _fareHarborUserKeysCacheManagerMock, _priceRuleEnginePersistenceMock, _pricingRuleCacheManagerMock, _calendarAvailabilityCacheManagerMock, _logger, _pickupLocationsCacheManagerMock, _tiqetsPaxMappingCacheManagerMock, _googleMapsPersistenceMock);

            #endregion Try

            #region Catch

            _cacheLoaderServiceException = new CacheLoaderService(_masterPersistenceMockException, _activityPersistenceMockException, _affiliatePersistenceMockException,
                    _ageGroupsCacheManagerMockException, _hbProductMappingCacheManagerMockException, _hotelBedsActivitiesCacheManagerMockException, _memCacheMockException,
                    _netPriceCacheManagerMockException, _similarProductsRegionAttractionCacheManagerMockException, _masterCacheManagerMockException,
                    _activityCacheManagerMockException, _affiliateCacheManagerMockException, _fareHarborCustomerPrototypesCacheManagerMockException,
                    _fareHarborUserKeysCacheManagerMockException, _priceRuleEnginePersistenceMockException, _pricingRulesCacheManagerMockException, _calendarAvailabilityCacheManagerMockException, _logger, _pickupLocationsCacheManagerMockException, _tiqetsPaxMappingCacheManagerMockException, _googleMapsPersistenceMockException);

            #endregion Catch
        }

        [Test]
        [Ignore("")]
        public void LoadGliAgeGroupByActivityTest()
        {
            var ageGroupList = new List<AgeGroup>
            {
                new AgeGroup()
                {
                    AgeGroupId = 100,
                    ActivityId = 853,
                    DisplayName="test"
                },
                 new AgeGroup()
                {
                    AgeGroupId = 101,
                    ActivityId = 853,
                    DisplayName="test1"
                },
                  new AgeGroup()
                {
                    AgeGroupId = 102,
                    ActivityId = 853,
                    DisplayName="test2"
                }
            };

            _masterPersistenceMock.GetGliAgeGroupsByActivity().Returns(ageGroupList);
            _ageGroupsCacheManagerMock.LoadAgeGroupByActivity(ageGroupList).ReturnsForAnyArgs("102");
            var result = _cacheLoaderService.LoadGliAgeGroupByActivityAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadAgeGroupByActivityExceptionTest()
        {
            _masterPersistenceMockException.GetGliAgeGroupsByActivity().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadGliAgeGroupByActivityAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadPrioAgeGroupByActivityTest()
        {
            var ageGroupList = new List<AgeGroup>
            {
                new AgeGroup()
                {
                    AgeGroupId = 100,
                    ActivityId = 853,
                    DisplayName="test"
                },
                 new AgeGroup()
                {
                    AgeGroupId = 101,
                    ActivityId = 853,
                    DisplayName="test1"
                },
                  new AgeGroup()
                {
                    AgeGroupId = 102,
                    ActivityId = 853,
                    DisplayName="test2"
                }
            };

            _masterPersistenceMock.GetPrioAgeGroupsByActivity().Returns(ageGroupList);
            _ageGroupsCacheManagerMock.LoadAgeGroupByActivity(ageGroupList).ReturnsForAnyArgs("102");
            var result = _cacheLoaderService.LoadPrioAgeGroupByActivityAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadPrioAgeGroupByActivityExceptionTest()
        {
            _masterPersistenceMockException.GetPrioAgeGroupsByActivity().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadPrioAgeGroupByActivityAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadAotAgeGroupByActivityTest()
        {
            var ageGroupList = new List<AgeGroup>
            {
                new AgeGroup()
                {
                    AgeGroupId = 100,
                    ActivityId = 853,
                    DisplayName="test"
                },
                 new AgeGroup()
                {
                    AgeGroupId = 101,
                    ActivityId = 853,
                    DisplayName="test1"
                },
                  new AgeGroup()
                {
                    AgeGroupId = 102,
                    ActivityId = 853,
                    DisplayName="test2"
                }
            };

            _masterPersistenceMock.GetAotAgeGroupsByActivity().Returns(ageGroupList);
            _ageGroupsCacheManagerMock.LoadAgeGroupByActivity(ageGroupList).ReturnsForAnyArgs("102");
            var result = _cacheLoaderService.LoadAotAgeGroupByActivityAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadAotAgeGroupByActivityExceptionTest()
        {
            _masterPersistenceMockException.GetAotAgeGroupsByActivity().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadAotAgeGroupByActivityAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadCacheMappingTest()
        {
            var hbProductMapping = new List<IsangoHBProductMapping>()
            {
                new IsangoHBProductMapping()
                {
                    ApiType = APIType.Undefined,
                    IsangoRegionId = 56,
                    IsangoHotelBedsActivityId = 26
                },
                new IsangoHBProductMapping()
                {
                    ApiType = APIType.Undefined,
                    IsangoRegionId = 57,
                    IsangoHotelBedsActivityId = 26
                },
                new IsangoHBProductMapping()
                {
                    ApiType = APIType.Undefined,
                    IsangoRegionId = 58,
                    IsangoHotelBedsActivityId = 26
                }
            };

            _masterPersistenceMock.LoadFactSheetMapping().ReturnsForAnyArgs(hbProductMapping);
            _hbProductMappingCacheManagerMock.LoadCacheMapping(hbProductMapping).ReturnsForAnyArgs("56");
            var result = _cacheLoaderService.LoadCacheMappingAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadCacheMappingExceptionTest()
        {
            _masterPersistenceMockException.LoadFactSheetMapping().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadCacheMappingAsync());
        }

        [Test]
        [Ignore("")]
        public void RegionCategoryMappingTest()
        {
            var regionCategoryMapping = new List<RegionCategoryMapping>()
            {
                new RegionCategoryMapping()
                {
                     CategoryId = 123,
                     RegionId = 456
                },
                new RegionCategoryMapping()
                {
                     CategoryId = 123,
                     RegionId = 456
                },
                new RegionCategoryMapping()
                {
                     CategoryId = 123,
                     RegionId = 456
                }
            };

            var cachedObject = new CacheKey<RegionCategoryMapping>()
            {
                Id = "RegionCategoryMapping",
                CacheValue = regionCategoryMapping
            };

            _activityPersistenceMock.LoadRegionCategoryMapping().ReturnsForAnyArgs(regionCategoryMapping);
            _memCacheMock.RegionCategoryMapping(cachedObject).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.RegionCategoryMappingAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void RegionCategoryMappingExceptionTest()
        {
            _activityPersistenceMockException.LoadRegionCategoryMapping().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.RegionCategoryMappingAsync());
        }

        [Test]
        [Ignore("")]
        public void RegionDestinationMappingTest()
        {
            var mappedRegionList = new List<MappedRegion>()
            {
                new MappedRegion()
                {
                    DestinationCode = "Ind",
                    RegionId = 45,
                    RegionName = "India"
                }
            };

            var cachedObject = new CacheKey<MappedRegion>()
            {
                Id = "RegionVsDestination",
                CacheValue = mappedRegionList
            };

            _masterPersistenceMock.RegionVsDestination().ReturnsForAnyArgs(mappedRegionList);
            _memCacheMock.RegionDestinationMapping(cachedObject).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.RegionDestinationMappingAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void RegionDestinationMappingExceptionTest()
        {
            _masterPersistenceMockException.RegionVsDestination().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.RegionDestinationMappingAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadMappedLanguageTest()
        {
            var mappedLanguageList = new List<MappedLanguage>()
            {
                new MappedLanguage()
                {
                    AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183",
                    GliLanguageCode = 1,
                    IsangoLanguageCode="en",
                    SupplierLanguageCode ="en"
                }
            };

            var cachedObject = new CacheKey<MappedLanguage>()
            {
                Id = "MappedLanguage",
                CacheValue = mappedLanguageList
            };
            _masterPersistenceMock.LoadMappedLanguage().ReturnsForAnyArgs(mappedLanguageList);
            _memCacheMock.LanguageCodeMapping(cachedObject).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.LoadMappedLanguageAsync().Result;
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("")]
        public void LoadMappedLanguageExceptionTest()
        {
            _masterPersistenceMockException.LoadMappedLanguage().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadMappedLanguageAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadNetPriceMasterDataTest()
        {
            var netPriceMasterList = new List<NetPriceMasterData>()
            {
                new NetPriceMasterData()
                {
                    AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183",
                    ApiType = APIType.Undefined,
                    ProductId = 100
                },
                   new NetPriceMasterData()
                {
                    AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183",
                    ApiType = APIType.Undefined,
                    ProductId = 101
                },
                      new NetPriceMasterData()
                {
                    AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183",
                    ApiType = APIType.Undefined,
                    ProductId = 102
                }
            };

            _masterPersistenceMock.LoadNetPriceMasterData().ReturnsForAnyArgs(netPriceMasterList);
            _netPriceCacheManagerMock.LoadNetPriceMasterData(netPriceMasterList).ReturnsForAnyArgs("100");
            var result = _cacheLoaderService.LoadNetPriceMasterDataAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadNetPriceMasterDataExceptionTest()
        {
            _masterPersistenceMockException.LoadNetPriceMasterData().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadNetPriceMasterDataAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadRegionCategoryMappingProductsTest()
        {
            var similarProductList = new List<RegionCategorySimilarProducts>()
            {
                new RegionCategorySimilarProducts()
                {
                    AttractionId = 100,
                    Priority = 5,
                    RegionId = 7128
                },
                new RegionCategorySimilarProducts()
                {
                    AttractionId = 100,
                    Priority = 5,
                    RegionId = 7129
                },
                new RegionCategorySimilarProducts()
                {
                    AttractionId = 100,
                    Priority = 5,
                    RegionId = 7126
                }
            };

            _masterPersistenceMock.GetRegionCategoryMapping().ReturnsForAnyArgs(similarProductList);
            _similarProductsRegionAttractionCacheManagerMock.RegionCategoryMappingProducts(similarProductList).ReturnsForAnyArgs("7128");
            var result = _cacheLoaderService.LoadRegionCategoryMappingProductsAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadRegionCategoryMappingProductsExceptionTest()
        {
            _masterPersistenceMockException.GetRegionCategoryMapping().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadRegionCategoryMappingProductsAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadAvailabilityCacheTest()
        {
            var availabilityTable = PrepareIsangoAvailabilities();
            var latLongList = new List<Availability>
            {
                new Availability()
                {
                    ServiceId = 6523,
                    RegionId = 7128,
                    Currency = "AUD",
                },
                new Availability()
                {
                    ServiceId = 4578,
                    RegionId = 7128,
                    Currency = "AUD",
                },
                new Availability()
                {
                    ServiceId = 6524,
                    RegionId = 7128,
                    Currency = "AUD",
                }
            };

            _activityPersistenceMock.GetOptionAvailability("7128").ReturnsForAnyArgs(availabilityTable);
            _hotelBedsActivitiesCacheManagerMock.LoadAvailabilityCache(latLongList).ReturnsForAnyArgs("");
            var result = _cacheLoaderService.LoadAvailabilityCacheAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadAvailabilityCacheExceptionTest()
        {
            _activityPersistenceMockException.GetOptionAvailability().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadAvailabilityCacheAsync());
        }

        //[Test]
        //public void LoadHotelCacheExceptionTest()
        //{
        //    _masterPersistenceMockException.GetSupportedLanguages().ThrowsForAnyArgs(new Exception());
        //    Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadHotelCacheAsync());
        //}

        [Test]
        [Ignore("")]
        public void LoadCurrencyExchangeRatesTest()
        {
            var currencyExchangeRateList = new List<CurrencyExchangeRates>()
            {
                new CurrencyExchangeRates()
                {
                    ExchangeRate = 17.35M,
                    FromCurrencyCode ="inr",
                    ToCurrencyCode = "usd"
                }
            };

            var prepareCacheData = new CacheKey<CurrencyExchangeRates>
            {
                Id = "CurrencyExchangeRate",
                CacheValue = currencyExchangeRateList
            };

            _masterPersistenceMock.LoadExchangeRates().ReturnsForAnyArgs(currencyExchangeRateList);
            _masterCacheManagerMock.LoadCurrencyExchangeRate(prepareCacheData).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.LoadCurrencyExchangeRatesAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadCurrencyExchangeRatesExceptionTest()
        {
            _masterPersistenceMockException.LoadExchangeRates().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadCurrencyExchangeRatesAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadActivitiesCollectionTest()
        {
            var availabilityTable = PrepareIsangoAvailabilities();
            var latLongList = new List<Availability>
            {
                new Availability()
                {
                    ServiceId = 6523,
                    RegionId = 7128,
                    Currency = "AUD",
                },
                new Availability()
                {
                    ServiceId = 4578,
                    RegionId = 7128,
                    Currency = "AUD",
                },
                new Availability()
                {
                    ServiceId = 6524,
                    RegionId = 7128,
                    Currency = "AUD",
                }
            };
            var languages = new List<Language>()
            {
                new Language()
                {
                    Code= "en"
                }
            };

            var serviceIds = new int[]
            {
                860,
                866
            };

            var activityList = new List<Activity>()
            {
                new Activity()
                {
                    ID = 860,
                    ProductOptions = new List<ProductOption>()
                },
                new Activity()
                {
                    ID = 866,
                    ProductOptions = new List<ProductOption>()
                }
            };

            var liveActivityList = new List<Activity>()
            {
                new Activity()
                {
                    ID = 74,
                    ProductOptions = new List<ProductOption>
                    {
                        new ProductOption()
                    },
                    ActivityType = ActivityType.Bundle
                },
                new Activity()
                {
                    ID = 474,
                    ProductOptions = new List<ProductOption>()
                }
            };
            var passengerInfo = new List<PassengerInfo>
            {
                new PassengerInfo()
            };

            var activityIdsList = string.Join(",", serviceIds);
            _activityCacheManagerMock.DeleteAndCreateCollection().ReturnsForAnyArgs(true);
            _masterPersistenceMock.GetSupportedLanguages().ReturnsForAnyArgs(languages);
            _activityPersistenceMock.GetLiveActivityIds("en").Returns(serviceIds);
            _activityPersistenceMock.GetActivitiesByActivityIds(activityIdsList, "en").ReturnsForAnyArgs(activityList);
            _activityPersistenceMock.LoadLiveHbActivities(0, "en").ReturnsForAnyArgs(liveActivityList);
            _activityPersistenceMock.GetOptionAvailability("7128").ReturnsForAnyArgs(availabilityTable);
            _hotelBedsActivitiesCacheManagerMock.LoadAvailabilityCache(latLongList).ReturnsForAnyArgs("");
            _activityPersistenceMock.GetPassengerInfoDetails().ReturnsForAnyArgs(passengerInfo);
            _activityCacheManagerMock.InsertActivity(new Activity()).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.LoadActivitiesCollectionAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadActivitiesCollectionExceptionTest()
        {
            _masterPersistenceMockException.GetSupportedLanguages().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadActivitiesCollectionAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadSelectedeActivitiesTest()
        {
            var availabilityTable = PrepareIsangoAvailabilities();
            var latLongList = new List<Availability>
            {
                new Availability()
                {
                    ServiceId = 6523,
                    RegionId = 7128,
                    Currency = "AUD",
                },
                new Availability()
                {
                    ServiceId = 4578,
                    RegionId = 7128,
                    Currency = "AUD",
                },
                new Availability()
                {
                    ServiceId = 6524,
                    RegionId = 7128,
                    Currency = "AUD",
                }
            };
            var languages = new List<Language>()
            {
                new Language()
                {
                    Code= "en"
                }
            };

            var serviceIds = new int[]
            {
                860,
                866
            };

            var activityList = new List<Activity>()
            {
                new Activity()
                {
                    ID = 860,
                    ProductOptions = new List<ProductOption>()
                },
                new Activity()
                {
                    ID = 866,
                    ProductOptions = new List<ProductOption>()
                }
            };

            var liveActivityList = new List<Activity>()
            {
                new Activity()
                {
                    ID = 74,
                    ProductOptions = new List<ProductOption>
                    {
                        new ProductOption()
                    },
                    ActivityType = ActivityType.Bundle
                },
                new Activity()
                {
                    ID = 474,
                    ProductOptions = new List<ProductOption>()
                }
            };
            var passengerInfo = new List<PassengerInfo>
            {
                new PassengerInfo()
            };

            var activityIdsList = string.Join(",", serviceIds);
            _masterPersistenceMock.GetSupportedLanguages().ReturnsForAnyArgs(languages);
            _activityPersistenceMock.GetLiveActivityIds("en").Returns(serviceIds);
            _activityPersistenceMock.GetActivitiesByActivityIds(activityIdsList, "en").ReturnsForAnyArgs(activityList);
            _activityPersistenceMock.LoadLiveHbActivities(0, "en").ReturnsForAnyArgs(liveActivityList);
            _activityPersistenceMock.GetOptionAvailability("866,860,74,474").ReturnsForAnyArgs(availabilityTable);
            _hotelBedsActivitiesCacheManagerMock.LoadAvailabilityCache(latLongList).ReturnsForAnyArgs("");
            _activityPersistenceMock.GetPassengerInfoDetails("866,860,74,474").ReturnsForAnyArgs(passengerInfo);
            _activityCacheManagerMock.InsertActivity(new Activity()).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.LoadSelectedActivitiesAsync("866,860,74,474").Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadSelectedActivitiesAsyncExceptionTest()
        {
            _masterPersistenceMockException.GetSupportedLanguages().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadSelectedActivitiesAsync("853"));
        }

        [Test]
        [Ignore("")]
        public void SetRegionTest()
        {
            var latLongUrlMappingList = new List<LatLongVsurlMapping>()
            {
                new LatLongVsurlMapping()
                {
                    RegionId = 7128,
                    LanguageCode = "en"
                },
                new LatLongVsurlMapping()
                {
                    RegionId = 7128,
                    LanguageCode = "en"
                }
            };

            var cacheResult = new CacheKey<LatLongVsurlMapping>
            {
                Id = "GeoCoordinateMasterMappingKey",
                CacheValue = latLongUrlMappingList
            };

            _masterPersistenceMock.GetRegionData().ReturnsForAnyArgs(latLongUrlMappingList);
            _masterCacheManagerMock.SetRegionData(cacheResult).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.SetRegionAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void SetRegionExceptionTest()
        {
            _masterPersistenceMockException.GetRegionData().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.SetRegionAsync());
        }

        [Test]
        [Ignore("")]
        public void SetAffiliateFiltersTest()
        {
            var affiliateFilterList = new List<AffiliateFilter>()
            {
                new AffiliateFilter()
                {
                    AffiliateId="ad88ac6e-72d8-4fcb-a0ec-00574699c53b",
                    ActivityFilter = true,
                    AffiliateMargin = 0
                },
                 new AffiliateFilter()
                {
                    AffiliateId="ad88ac6e-72d8-4fcb-a0ec-00574699c53b",
                    ActivityFilter = false,
                    AffiliateMargin = 1
                }
            };

            var cacheResult = new CacheKey<AffiliateFilter>
            {
                Id = "AffiliateFilterCacheKey",
                CacheValue = affiliateFilterList
            };

            _affiliatePersistenceMock.GetAffiliateFilter().ReturnsForAnyArgs(affiliateFilterList);
            _affiliateCacheManagerMock.SetAffiliateFilterToCache(cacheResult).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.SetAffiliateFiltersAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void SetAffiliateFiltersExceptionTest()
        {
            _affiliatePersistenceMockException.GetAffiliateFilter().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.SetAffiliateFiltersAsync());
        }

        [Test]
        [Ignore("")]
        public void GetCustomerPrototypeByActivityTest()
        {
            var customerPrototype = new List<CustomerPrototype>()
            {
                new CustomerPrototype()
                {
                    CustomerPrototypeId = 12,
                    AgeGroupId = 3,
                    ServiceId = 860
                },
                new CustomerPrototype()
                {
                    CustomerPrototypeId = 13,
                    AgeGroupId = 1,
                    ServiceId = 866
                },
                new CustomerPrototype()
                {
                    CustomerPrototypeId = 15,
                    AgeGroupId = 2,
                    ServiceId = 74
                }
            };

            var cacheResult = new CacheKey<CustomerPrototype>
            {
                Id = "FareHarborCustomerPrototype",
                CacheValue = customerPrototype
            };

            _masterPersistenceMock.GetCustomerPrototypeByActivity().ReturnsForAnyArgs(customerPrototype);
            _fareHarborCustomerPrototypesCacheManagerMock.SetCustomerPrototypeByActivityToCache(cacheResult).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.GetCustomerPrototypeByActivityAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void GetCustomerPrototypeByActivityExceptionTest()
        {
            _masterPersistenceMockException.GetCustomerPrototypeByActivity().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.GetCustomerPrototypeByActivityAsync());
        }

        [Test]
        [Ignore("")]
        public void GetAllFareHarborUserKeysTest()
        {
            var fareHarborUserKey = new List<FareHarborUserKey>()
            {
                new FareHarborUserKey()
                {
                    Id = 21,
                    Currency ="AUD",
                    CompanyShortName = "testing"
                },
                new FareHarborUserKey()
                {
                    Id = 53,
                    Currency ="INR",
                    CompanyShortName = "test"
                }
            };

            var cacheResult = new CacheKey<FareHarborUserKey>
            {
                Id = "FareHarborUserKey",
                CacheValue = fareHarborUserKey
            };

            _masterPersistenceMock.GetAllFareHarborUserKeys().ReturnsForAnyArgs(fareHarborUserKey);
            _fareHarborUserKeysCacheManagerMock.SetAllFareHarborUserKeysToCache(cacheResult).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.GetAllFareHarborUserKeysAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void GetAllFareHarborUserKeysExceptionTest()
        {
            _masterPersistenceMockException.GetAllFareHarborUserKeys().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.GetAllFareHarborUserKeysAsync());
        }

        [Test]
        [Ignore("")]
        public void SetUrlPageIdMappingMappingTest()
        {
            var urlPageIdMappingList = new List<UrlPageIdMapping>()
            {
                new UrlPageIdMapping()
                {
                    AffiliateId = "ad88ac6e-72d8-4fcb-a0ec-00574699c53b",
                    LanguageCode = "en",
                    PageId = 17,
                    CategoryId = 26,
                    RegionId = 7126
                },
                new UrlPageIdMapping()
                {
                    AffiliateId = "ad88ac6e-72d8-4fcb-a0ec-00574699c53b",
                    LanguageCode = "en",
                    PageId = 36,
                    CategoryId = 21,
                    RegionId = 7125
                },
            };

            var cacheResult = new CacheKey<UrlPageIdMapping>
            {
                Id = "UrlPageIdMapping",
                CacheValue = urlPageIdMappingList
            };

            _masterPersistenceMock.UrlPageIdMappingList().ReturnsForAnyArgs(urlPageIdMappingList);
            _memCacheMock.LoadURLVsPageID(cacheResult).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.SetUrlPageIdMappingMappingAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void SetUrlPageIdMappingMappingExceptionTest()
        {
            _masterPersistenceMockException.UrlPageIdMappingList().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.SetUrlPageIdMappingMappingAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadPricingRulesTest()
        {
            var productSaleRules = GetMockProductSaleRules();
            var supplierSaleRules = GetMockSupplierSaleRule();
            var b2BSaleRules = GetMockB2BSaleData();
            var b2BNetRateRules = GetMockB2BNetRateData();

            _pricingRuleCacheManagerMock.DeleteAndCreateCollection().ReturnsForAnyArgs(true);
            _pricingRuleCacheManagerMock.CreateCollectionIfNotExist().ReturnsForAnyArgs(true);

            // InsertDocument Mocking Starts
            var productSaleRulesByActivityData = new CacheKey<ProductSaleRuleByActivity>()
            {
                Id = "ProductSaleRuleByActivity",
                CacheKeyName = "ProductSaleRuleByActivity",
                CacheValue = productSaleRules.ProductSaleRulesByActivity
            };
            _pricingRuleCacheManagerMock.InsertDocuments(productSaleRulesByActivityData).ReturnsForAnyArgs(true);

            var productSaleRulesByOptionData = new CacheKey<ProductSaleRuleByOption>()
            {
                Id = "ProductSaleRuleByOption",
                CacheKeyName = "ProductSaleRuleByOption",
                CacheValue = productSaleRules.ProductSaleRulesByOption
            };
            _pricingRuleCacheManagerMock.InsertDocuments(productSaleRulesByOptionData).ReturnsForAnyArgs(true);

            var productSaleRulesByAffiliateData = new CacheKey<ProductSaleRuleByAffiliate>()
            {
                Id = "ProductSaleRuleByAffiliate",
                CacheKeyName = "ProductSaleRuleByAffiliate",
                CacheValue = productSaleRules.ProductSaleRulesByAffiliate
            };
            _pricingRuleCacheManagerMock.InsertDocuments(productSaleRulesByAffiliateData).ReturnsForAnyArgs(true);

            var productSaleRulesByCountryData = new CacheKey<ProductSaleRuleByCountry>()
            {
                Id = "ProductSaleRuleByCountry",
                CacheKeyName = "ProductSaleRuleByCountry",
                CacheValue = productSaleRules.ProductSaleRulesByCountry
            };
            _pricingRuleCacheManagerMock.InsertDocuments(productSaleRulesByCountryData).ReturnsForAnyArgs(true);

            var supplierSaleRulesByActivityData = new CacheKey<SupplierSaleRuleByActivity>()
            {
                Id = "SupplierSaleRuleByActivity",
                CacheKeyName = "SupplierSaleRuleByActivity",
                CacheValue = supplierSaleRules.SupplierSaleRulesByActivity
            };
            _pricingRuleCacheManagerMock.InsertDocuments(supplierSaleRulesByActivityData).ReturnsForAnyArgs(true);

            var supplierSaleRulesByOptionData = new CacheKey<SupplierSaleRuleByOption>()
            {
                Id = "SupplierSaleRuleByOption",
                CacheKeyName = "SupplierSaleRuleByOption",
                CacheValue = supplierSaleRules.SupplierSaleRulesByOption
            };
            _pricingRuleCacheManagerMock.InsertDocuments(supplierSaleRulesByOptionData).ReturnsForAnyArgs(true);

            var b2BSaleData = new CacheKey<B2BSaleRule>()
            {
                Id = "B2BSaleRules",
                CacheKeyName = "B2BSaleRules",
                CacheValue = b2BSaleRules
            };
            _pricingRuleCacheManagerMock.InsertDocuments(b2BSaleData).ReturnsForAnyArgs(true);

            var b2BNetRateData = new CacheKey<B2BNetRateRule>()
            {
                Id = "B2BNetRateRules",
                CacheKeyName = "B2BNetRateRules",
                CacheValue = b2BNetRateRules
            };
            _pricingRuleCacheManagerMock.InsertDocuments(b2BNetRateData).ReturnsForAnyArgs(true);

            // InsertDocument Mocking Ends

            // Peristence methods mocking starts

            _priceRuleEnginePersistenceMock.GetProductSaleRule().ReturnsForAnyArgs(productSaleRules);
            _priceRuleEnginePersistenceMock.GetB2BSaleRules().ReturnsForAnyArgs(b2BSaleRules);
            _priceRuleEnginePersistenceMock.GetB2BNetRateRules().ReturnsForAnyArgs(b2BNetRateRules);
            _priceRuleEnginePersistenceMock.GetSupplierSaleRule().ReturnsForAnyArgs(supplierSaleRules);

            // Peristence methods mocking ends

            var result = _cacheLoaderService.LoadPricingRulesAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadPricingRulesExceptionTest()
        {
            _pricingRulesCacheManagerMockException.DeleteAndCreateCollection().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadPricingRulesAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadCalendarAvailabilityTest()
        {
            var calendarAvailabilityList = new List<CalendarAvailability>
            {
                new CalendarAvailability
                {
                    ActivityId = 853,
                    RegionId = 6167,
                    Currency ="USD",
                    AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                    StartDate = DateTime.Now.AddDays(1),
                    B2BBasePrice = 1254.1M,
                    B2CBasePrice = 1150M
                },
                new CalendarAvailability
                {
                    ActivityId = 853,
                    RegionId = 6167,
                    Currency ="USD",
                    AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                    StartDate = DateTime.Now.AddDays(2),
                    B2BBasePrice = 1254.1M,
                    B2CBasePrice = 1150M
                }
            };
            _activityPersistenceMock.GetCalendarAvailability().ReturnsForAnyArgs(calendarAvailabilityList);
            _calendarAvailabilityCacheManagerMock.LoadCalendarAvailability().ReturnsForAnyArgs(true);
            _calendarAvailabilityCacheManagerMock.InsertCalendarDocuments(null).ReturnsForAnyArgs(false);
            var result = _cacheLoaderService.LoadCalendarAvailability().Result;
            Assert.IsNotEmpty(result);
        }

        #region Private Methods

        private ProductSaleRule GetMockProductSaleRules()
        {
            var productSaleRulesByActivity = new List<ProductSaleRuleByActivity>
            {
                new ProductSaleRuleByActivity
                {
                    AppliedRuleId = 1,
                    ServiceId = 1,
                    SaleRuleOfferPercent = 10,
                    BookingFromDate = DateTime.Now,
                    BookingToDate = DateTime.Now,
                    RuleName = "Dummy",
                    ShowSale = true,
                    TravelFromDate = DateTime.Now,
                    TravelToDate = DateTime.Now,
                    SupplementRuleArriveOnMonday = false,
                    SupplementRuleArriveOnFriday = false,
                    SupplementRuleArriveOnSaturday = false,
                    SupplementRuleArriveOnSunday = false,
                    SupplementRuleArriveOnThursday = false,
                    SupplementRuleArriveOnTuesday = false,
                    SupplementRuleArriveOnWednesday = false
                }
            };
            var productSaleRulesByOption = new List<ProductSaleRuleByOption>
            {
                new ProductSaleRuleByOption
                {
                    AppliedRuleId = 1,
                    PriorityOrder = 1,
                    ServiceOptionInServiceId = 1
                }
            };
            var productSaleRulesByAffiliate = new List<ProductSaleRuleByAffiliate>
            {
                new ProductSaleRuleByAffiliate
                {
                    AppliedRuleId = 1
                }
            };
            var productSaleRulesByCountry = new List<ProductSaleRuleByCountry>
            {
                new ProductSaleRuleByCountry
                {
                    AppliedRuleId = 1
                }
            };

            var productSaleRules = new ProductSaleRule
            {
                ProductSaleRulesByActivity = productSaleRulesByActivity,
                ProductSaleRulesByOption = productSaleRulesByOption,
                ProductSaleRulesByAffiliate = productSaleRulesByAffiliate,
                ProductSaleRulesByCountry = productSaleRulesByCountry
            };
            return productSaleRules;
        }

        private List<B2BSaleRule> GetMockB2BSaleData()
        {
            var b2BSaleRules = new List<B2BSaleRule>
            {
                new B2BSaleRule
                {
                    AffiliateId = "F2DE3C6A-048D-4C7E-AAF0-86CF0D51DDEF"
                }
            };
            return b2BSaleRules;
        }

        private List<B2BNetRateRule> GetMockB2BNetRateData()
        {
            var b2BNetRateRules = new List<B2BNetRateRule>
            {
                new B2BNetRateRule
                {
                    AffiliateId = "F2DE3C6A-048D-4C7E-AAF0-86CF0D51DDEF"
                }
            };

            return b2BNetRateRules;
        }

        private SupplierSaleRule GetMockSupplierSaleRule()
        {
            var supplierSaleRuleByActivity = new List<SupplierSaleRuleByActivity>
            {
                new SupplierSaleRuleByActivity
                {
                    AppliedRuleId = 1,
                    ServiceId = 1,
                    SaleRuleOfferPercent = 10,
                    BookingFromDate = DateTime.Now,
                    BookingToDate = DateTime.Now,
                    RuleName = "Dummy",
                    ShowSale = true,
                    TravelFromDate = DateTime.Now,
                    TravelToDate = DateTime.Now
                }
            };

            var supplierSaleRuleByOption = new List<SupplierSaleRuleByOption>
            {
                new SupplierSaleRuleByOption
                {
                    AppliedRuleId = 1,
                }
            };

            var supplierSaleRule = new SupplierSaleRule
            {
                SupplierSaleRulesByActivity = supplierSaleRuleByActivity,
                SupplierSaleRulesByOption = supplierSaleRuleByOption
            };
            return supplierSaleRule;
        }

        #endregion Private Methods

        [Test]
        [Ignore("")]
        public void LoadAffiliateDataByDomainTest()
        {
            var affiliateIds = new List<string>
            {
                "D3696D33-1682-4537-897E-4D0870BD4C56"
            };
            var affiliate = new Affiliate
            {
                AffiliateCompanyDetail = new AffiliateCompanyDetail
                {
                    CompanyWebSite = "m.isango.com"
                },
                Alias = "MOGO"
            };

            _affiliateCacheManagerMock.DeleteAndCreateCollection().ReturnsForAnyArgs(true);
            _affiliatePersistenceMock.GetModifiedAffiliates().ReturnsForAnyArgs(affiliateIds);
            _affiliatePersistenceMock.GetAffiliateInfo("", "", "D3696D33-1682-4537-897E-4D0870BD4C56").ReturnsForAnyArgs(affiliate);
            _affiliateCacheManagerMock.SetAffiliateInfoToCache(affiliate).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.LoadAffiliateDataByDomainAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadAffiliateDataByDomainExceptionTest()
        {
            _affiliateCacheManagerMockException.DeleteAndCreateCollection().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadAffiliateDataByDomainAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadHBAuthorizationDataTest()
        {
            var hotelBedsCredentials = new List<HotelBedsCredentials>
            {
                new HotelBedsCredentials
                {
                    AffiliateId="test",
                    LanguageCode="ENG",
                 Authentication="uname+pwd"
                }
            };

            var cacheResult = new CacheKey<HotelBedsCredentials>
            {
                Id = "HBAuthData",
                CacheValue = hotelBedsCredentials
            };

            _masterPersistenceMock.LoadHBauthData().ReturnsForAnyArgs(hotelBedsCredentials);
            _masterCacheManagerMock.SetHBAuthorizationData(cacheResult).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.LoadHBAuthorizationDataAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void LoadHBAuthorizationDataExceptionTest()
        {
            _masterPersistenceMockException.LoadHBauthData().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.LoadHBAuthorizationDataAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadPickupLocationsDataTest()
        {
            var pickupLocations = new List<PickupLocation>
            {
                new PickupLocation
                {
                    Id = "0",
                    Name = "",
                    ActivityId = 0,
                    Description = "",
                    LocationId = 0
                }
            };
            _masterPersistenceMock.GetPickupLocationsByActivity().ReturnsForAnyArgs(pickupLocations);
            _pickupLocationsCacheManagerMock.InsertDocuments(null).ReturnsForAnyArgs(true);
            _pickupLocationsCacheManagerMock.CreateCollection().ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.LoadPickupLocationsDataAsync().GetAwaiter().GetResult();
            Assert.IsEmpty(result);
        }

        [Test]
        [Ignore("")]
        public void LoadPickupLocationsDataExceptionTest()
        {
            var pickupLocations = new List<PickupLocation>
            {
                new PickupLocation
                {
                    Id = "0",
                    Name = "",
                    ActivityId = 0,
                    Description = "",
                    LocationId = 0
                }
            };
            _masterPersistenceMockException.GetPickupLocationsByActivity().ReturnsForAnyArgs(pickupLocations);
            _pickupLocationsCacheManagerMockException.InsertDocuments(null).ThrowsForAnyArgs(new Exception());
            _cacheLoaderServiceMocking.LoadPickupLocationsDataAsync().GetAwaiter().GetResult().ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceMocking.LoadPickupLocationsDataAsync());
        }

        [Test]
        [Ignore("")]
        public void SetFilteredTicketAsyncTest()
        {
            var ticketsByRegion = new List<Entities.TicketByRegion>
            {
                new Entities.TicketByRegion
                {
                    CountryCode = "",
                    ThemeparkTicket = new Entities.ThemeparkTicket
                    {
                        ProductId = 0,
                        City = 0,
                        Country = 0,
                        Region = 0
                    }
                }
            };
            _masterPersistenceMock.GetFilteredThemeparkTickets().ReturnsForAnyArgs(ticketsByRegion);
            _masterCacheManagerMock.SetFilteredTicketsToCache(null).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.SetFilteredTicketAsync().Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void SetFilteredTicketAsyncExceptionTest()
        {
            var ticketsByRegion = new List<Entities.TicketByRegion>
            {
                new Entities.TicketByRegion
                {
                    CountryCode = "",
                    ThemeparkTicket = new Entities.ThemeparkTicket
                    {
                        ProductId = 0,
                        City = 0,
                        Country = 0,
                        Region = 0
                    }
                }
            };
            _masterPersistenceMockException.GetFilteredThemeparkTickets().ReturnsForAnyArgs(ticketsByRegion);
            _masterCacheManagerMockException.SetFilteredTicketsToCache(null).ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.SetFilteredTicketAsync());
        }

        [Test]
        [Ignore("")]
        public void GetAllActivities()
        {
            _activityCacheManagerMock.GetAllActivities().ReturnsForAnyArgs(new List<Activity>());
            var result = _cacheLoaderService.GetAllActivities().Result;
            Assert.NotNull(result);
        }

        [Test]
        [Ignore("")]
        public void GetSingleActivities()
        {
            _activityCacheManagerMock.GetActivity("853").ReturnsForAnyArgs(new Activity());
            var result = _cacheLoaderService.GetSingleActivity("853").Result;
            Assert.NotNull(result);
        }

        [Test]
        [Ignore("")]
        public void GetPriceAndAvailability()
        {
            _hotelBedsActivitiesCacheManagerMock.GetAllPriceAndAvailability().ReturnsForAnyArgs(new List<Availability>());
            var result = _cacheLoaderService.GetPriceAndAvailability().Result;
            Assert.NotNull(result);
        }

        [Test, Order(31)]
        [Ignore("")]
        public void SetAffiliateFilterTest()
        {
            _affiliateCacheManagerMock.DeleteAndCreateAffiliateFilterCollection().ReturnsForAnyArgs(true);
            _affiliatePersistenceMock.GetAffiliateFilter().ReturnsForAnyArgs(new List<AffiliateFilter>());
            _affiliateCacheManagerMock.SetAffiliateToCache(new List<AffiliateFilter>()).ReturnsForAnyArgs(true);

            var result = _cacheLoaderService.SetAffiliateFilterAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("")]
        public void SetAffiliateFilterExceptionTest()
        {
            _affiliateCacheManagerMockException.DeleteAndCreateAffiliateFilterCollection().ThrowsForAnyArgs(new Exception());
            //_affiliatePersistenceMock.GetAffiliateFilter().ReturnsForAnyArgs(new List<AffiliateFilter>());
            //_affiliateCacheManagerMock.SetAffiliateToCache(new List<AffiliateFilter>()).ReturnsForAnyArgs(true);
            Assert.ThrowsAsync<Exception>(() => _cacheLoaderServiceException.SetAffiliateFilterAsync());
        }

        [Test]
        [Ignore("")]
        public void LoadGoldenToursPaxMappingsTest()
        {
            var passengerMappings = new List<PassengerMapping>()
            {
                new PassengerMapping()
                {
                    PassengerTypeId = 1,
                    SupplierPassengerTypeId = 1
                },
                new PassengerMapping()
                {
                    PassengerTypeId = 2,
                    SupplierPassengerTypeId = 2
                }
            };

            var cacheResult = new CacheKey<PassengerMapping>
            {
                Id = "GoldenToursPaxMapping",
                CacheValue = passengerMappings
            };

            _masterPersistenceMock.GetPassengerMapping().ReturnsForAnyArgs(passengerMappings);
            _masterCacheManagerMock.LoadGoldenToursPassengerMappings(cacheResult).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.LoadGoldenToursPaxMappingsAsync().Result;
            Assert.IsTrue(result);
        }

        #region Private Methods

        private DataTable PrepareIsangoAvailabilities()
        {
            var availabilityTable = new DataTable();
            availabilityTable.Columns.Add("serviceid");
            availabilityTable.Columns.Add("regionid");
            availabilityTable.Columns.Add("vcurrencyISOCode");
            availabilityTable.Columns.Add("serviceoptionid");
            availabilityTable.Columns.Add("startDate");
            availabilityTable.Columns.Add("endDate");
            availabilityTable.Columns.Add("GatePrice");
            availabilityTable.Columns.Add("CostPrice");
            var dataRow = availabilityTable.NewRow();
            dataRow[0] = 6523;
            dataRow[1] = 7128;
            dataRow[2] = "AUD";
            dataRow[3] = 0;
            dataRow[4] = DateTime.Today;
            dataRow[5] = DateTime.Today;
            dataRow[6] = 10;
            dataRow[7] = 10;
            availabilityTable.Rows.Add(dataRow);

            var dataRow1 = availabilityTable.NewRow();
            dataRow1[0] = 6522;
            dataRow1[1] = 7128;
            dataRow1[2] = "AUD";
            dataRow1[3] = 0;
            dataRow1[4] = DateTime.Today;
            dataRow1[5] = DateTime.Today;
            dataRow1[6] = 10;
            dataRow1[7] = 10;
            availabilityTable.Rows.Add(dataRow1);
            return availabilityTable;
        }

        #endregion Private Methods

        [Test]
        [Ignore("")]
        public void LoadTiqetsPaxMappingsAsyncTest()
        {
            //Note: Need to test it (data is dummy)
            var tiqetsPaxMappings = new List<TiqetsPaxMapping>()
            {
                new TiqetsPaxMapping()
                {
                    ServiceOptionId = 1234,
                    AgeGroupId = 2345,
                    AgeGroupCode = "test",
                    PassengerType = PassengerType.Adult,
                    APIType = APIType.Tiqets
                },
                new TiqetsPaxMapping()
                {
                    ServiceOptionId = 5678,
                    AgeGroupId = 5678,
                    AgeGroupCode = "test1",
                    PassengerType = PassengerType.Adult,
                    APIType = APIType.Tiqets
                }
            };

            var cacheResult = new CacheKey<TiqetsPaxMapping>
            {
                Id = "TiqetsPaxMapping",
                CacheValue = tiqetsPaxMappings
            };

            _masterPersistenceMock.LoadTiqetsPaxMappings().ReturnsForAnyArgs(tiqetsPaxMappings);
            _tiqetsPaxMappingCacheManagerMock.SetPaxMappingToCache(cacheResult).ReturnsForAnyArgs(true);
            var result = _cacheLoaderService.LoadTiqetsPaxMappingsAsync().Result;
            Assert.IsTrue(result);
        }
    }
}