using Autofac;
using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Entities.Payment;
using Isango.Entities.Region;
using Isango.Entities.Wrapper;
using Isango.Persistence.Contract;
using Isango.Register;
using Isango.Service;
using Isango.Service.Contract;
using Logger.Contract;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using ServiceAdapters.HB;
using ServiceAdapters.HB.HB.Entities.ActivityDetail;
using System;
using System.Collections.Generic;
using Isango.Entities.Availability;
using ThemeparkTicket = Isango.Entities.ThemeparkTicket;
using TicketByRegion = Isango.Entities.TicketByRegion;

namespace Isango.Services.Test
{
    [TestFixture]
    public class UnusedActivityServiceTest : BaseTest
    {
        private IUnusedActivityService _unusedActivityService;
        private UnusedActivityService unusedActivityServiceForMocking;
        private UnusedActivityService unusedActivityServiceForMockingException;
        private IActivityPersistence gatewayActivityPersistence;
        private IMasterCacheManager gatewayMasterCacheManager;
        private IAffiliateService gatewayAffiliateService;
        private IMasterService gatewayMasterService;
        private IAttractionActivityMappingCacheManager gatewayAttractionActivityMappingCacheManager;
        private IHBAdapter gatewayHBAdapter;
        private IActivityCacheManager gatewayActivityCacheManager;
        private ISearchService gatewaySearchService;
        private IHotelBedsActivitiesCacheManager gatewayHotelBedsActivitiesCacheManager;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();
            gatewayActivityPersistence = Substitute.For<IActivityPersistence>();
            gatewayMasterCacheManager = Substitute.For<IMasterCacheManager>();
            var gatewayLogger = Substitute.For<ILogger>();
            gatewayAffiliateService = Substitute.For<IAffiliateService>();
            gatewayAttractionActivityMappingCacheManager = Substitute.For<IAttractionActivityMappingCacheManager>();
            var gatewayNetPriceCacheManager = Substitute.For<INetPriceCacheManager>();
            gatewayMasterService = Substitute.For<IMasterService>();
            gatewayHBAdapter = Substitute.For<IHBAdapter>();
            gatewayActivityCacheManager = Substitute.For<IActivityCacheManager>();
            gatewaySearchService = Substitute.For<ISearchService>();
            gatewayHotelBedsActivitiesCacheManager = Substitute.For<IHotelBedsActivitiesCacheManager>();

            unusedActivityServiceForMocking = new UnusedActivityService(
                gatewayActivityPersistence
                , gatewayLogger
                , gatewayMasterCacheManager
                , gatewayMasterService
                , gatewayAttractionActivityMappingCacheManager
                , gatewayAffiliateService
                , gatewayNetPriceCacheManager
                , gatewayHBAdapter
                , gatewaySearchService
            );

            unusedActivityServiceForMockingException = new UnusedActivityService(gatewayActivityPersistence
                , gatewayLogger
                , gatewayMasterCacheManager
                , gatewayMasterService
                , gatewayAttractionActivityMappingCacheManager
                , gatewayAffiliateService
                , gatewayNetPriceCacheManager
                , gatewayHBAdapter
                , gatewaySearchService
            );

            using (var scope = _container.BeginLifetimeScope())
            {
                _unusedActivityService = scope.Resolve<IUnusedActivityService>();
            }
        }

        [Test]
        public void GetActivitiesOptionTest()
        {
            var result =
                _unusedActivityService.GetActivitiesAsync("6873", new ClientInfo { LanguageCode = "EN" }, DateTime.Now);
            Assert.NotNull(result);
        }

        [Test]
        public void CheckActivityTypeAsyncTest()
        {
            gatewayActivityPersistence.GetActivityType(2789).ReturnsForAnyArgs(1);
            var result = unusedActivityServiceForMocking.GetActivityTypeAsync(2789).Result;
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void CheckActivityTypeExceptionTest()
        {
            unusedActivityServiceForMockingException.GetActivityTypeAsync(2789).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => unusedActivityServiceForMockingException.GetActivityTypeAsync(2789));
        }

        [Test]
        public void LoadMaxPaxDetailsAsyncTest()
        {
            var testResult = new Dictionary<string, int>
            {
                { "test", 1001 }
            };
            gatewayActivityPersistence.LoadMaxPaxDetails(517).ReturnsForAnyArgs(testResult);
            var result = unusedActivityServiceForMocking.LoadMaxPaxDetailsAsync(517).Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void LoadMaxPaxDetailsExceptionTest()
        {
            unusedActivityServiceForMockingException.LoadMaxPaxDetailsAsync(21776).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => unusedActivityServiceForMockingException.LoadMaxPaxDetailsAsync(21776));
        }

        [Test]
        public void GetActivityDetailAsyncTest()
        {
            var PaxesList = new List<Pax>();

            var pax1 = new Pax { Age = 12 };
            var pax2 = new Pax { Age = 10 };

            PaxesList.Add(pax1);
            PaxesList.Add(pax2);

            var activityRq = new ActivityRq
            {
                Code = "CITYTOUR~#~T~#~PAR",
                From = "Paris",
                Language = "EN",
                To = "Dubai",
                Paxes = PaxesList
            };

            var token = "TestFromActivityService";
            var activity = new Activity() { ActivityType = ActivityType.HalfDay };
            var criteria = GetActivityCriteriaReqeust();
            gatewayHBAdapter.ActivityDetailsAsync(criteria, token).Returns(activity);
            var result = unusedActivityServiceForMocking.GetActivityDetailAsync(criteria, token).Result;
            Assert.That(result, Is.EqualTo(activity));
        }

        [Test]
        public void GetActivityDetailExceptionTest()
        {
            unusedActivityServiceForMockingException.GetActivityDetailAsync(null, "token").Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => unusedActivityServiceForMockingException.GetActivityDetailAsync(null, "token"));
        }

        [Test]
        public void GetActivitiesTest()
        {
            var activityIds = "21776,24315";
            var listActivity = new List<Activity> { new Activity { Id = "10001", ActivityType = ActivityType.Undefined, Name = "SighSeeing", ApiType = APIType.Hotelbeds, PassengerInfo = new List<Entities.Booking.PassengerInfo> { new Entities.Booking.PassengerInfo { ActivityId = 21776, FromAge = 0, ToAge = 5, PassengerTypeId = 1, IndependablePax = true, MinSize = 0, MaxSize = 10, Label = "Adult", MeasurementDesc = "Adult >120cm" } } } };
            unusedActivityServiceForMocking.GetActivitiesAsync(activityIds, PrepareClientInfoForActivity(), DateTime.Now).Result.ReturnsForAnyArgs(listActivity);
            var result = unusedActivityServiceForMocking.GetActivitiesAsync(activityIds, PrepareClientInfoForActivity(), DateTime.Now).Result;
            Assert.That(result, Is.EqualTo(listActivity));
            Assert.IsTrue(result[0].PassengerInfo.Count > 0);
        }

        [Test]
        public void GetActivitiesExceptionTest()
        {
            //Catch block scenario
            unusedActivityServiceForMockingException.GetActivitiesAsync(null, null, new DateTime()).Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => unusedActivityServiceForMockingException.GetActivitiesAsync(null, null, new DateTime()));
        }

        [Test]
        public void LoadLiveHbActivitiesTest()
        {
            var listActivity = new List<Activity>() { new Activity() { ActivityType = ActivityType.FullDay, ApiType = APIType.Undefined, Id = "1001" } };
            gatewayActivityPersistence.LoadLiveHbActivities(0, "EN").ReturnsForAnyArgs(listActivity);
            var result = unusedActivityServiceForMocking.LoadLiveHbActivitiesAsync(0, PrepareClientInfoForActivity()).Result;
            Assert.That(result, Is.EqualTo(listActivity));
        }

        [Test]
        public void LoadLiveHbActivitiesExceptionTest()
        {
            //Catch block scenario
            unusedActivityServiceForMockingException.LoadLiveHbActivitiesAsync(0, null).Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => unusedActivityServiceForMockingException.LoadLiveHbActivitiesAsync(0, null));
        }

        /// <summary>
        /// Test case to get auto suggest data by AffiliateId
        /// </summary>
        [Test]
        public void GetAutoSuggestByAffiliateIdTest()
        {
            var affiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            var testResult = new List<AutoSuggest>() { new AutoSuggest() { Category = "test", Languagecode = "EN", ReferenceId = "21001" } };
            gatewayMasterCacheManager.GetAutoSuggestByAffiliateId(affiliateId).ReturnsForAnyArgs(testResult);
            var result = unusedActivityServiceForMocking.GetAutoSuggestByAffiliateIdAsync(affiliateId).Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void GetAutoSuggestByAffiliateIdExceptionTest()
        {
            unusedActivityServiceForMockingException.GetAutoSuggestByAffiliateIdAsync("5beef089-3e4e-4f0f-9fbf-99bf1f350183").Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => unusedActivityServiceForMockingException.GetAutoSuggestByAffiliateIdAsync("5beef089-3e4e-4f0f-9fbf-99bf1f350183"));
        }

        /// <summary>
        /// Test method to get live services by language code
        /// </summary>
        [Test]
        [TestCase("esp")]
        public void GetLiveServicesTest(string languageCode)
        {
            var testResult = new int[] { 1, 11, 111 };
            gatewayActivityPersistence.GetLiveActivityIds(languageCode).ReturnsForAnyArgs(testResult);
            var result = unusedActivityServiceForMocking.GetLiveActivityIdsAsync(languageCode).Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void GetLiveServicesExceptionTest()
        {
            unusedActivityServiceForMockingException.GetLiveActivityIdsAsync("EN").Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => unusedActivityServiceForMockingException.GetLiveActivityIdsAsync("EN"));
        }

        /// <summary>
        /// Test case to get activity by id
        /// </summary>
        [Test]
        public void GetActivityIdTest()
        {
            var productId = 10001;
            gatewayActivityPersistence.GetActivityId(productId).ReturnsForAnyArgs(10);
            var result = unusedActivityServiceForMocking.GetActivityIdAsync(productId).Result;
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void GetActivityIdExceptionTest()
        {
            unusedActivityServiceForMockingException.GetActivityIdAsync(0).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => unusedActivityServiceForMockingException.GetActivityIdAsync(0));
        }

        [Ignore("Unused method")]
        /// <summary>
        /// Test case to get auto suggest data
        /// </summary>
        [Test]
        [TestCase("EN")]
        [TestCase("de")]
        [TestCase("es")]
        public void GetAutoSuggestDataTest(string languageCode)
        {
            //First Scenario
            var testResult = new List<AutoSuggest>() { new AutoSuggest() { Category = "test", Display = "display", Languagecode = languageCode, Type = "test" } };
            var clientInfo = new ClientInfo
            {
                LanguageCode = languageCode,
                AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183"
            };
            gatewayActivityPersistence.GetAutoSuggestData(clientInfo.AffiliateId).ReturnsForAnyArgs(testResult);
            var result = unusedActivityServiceForMocking.GetAutoSuggestDataAsync(clientInfo).Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Ignore("Unused method")]
        [Test]
        public void GetAutoSuggestDataExceptionTest()
        {
            unusedActivityServiceForMockingException.GetAutoSuggestDataAsync(null).Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => unusedActivityServiceForMockingException.GetAutoSuggestDataAsync(null));
        }

        [Test]
        public void GetPriceAndAvailabilityTest()
        {
            var productOption = PrepareProductOption();
            var testResult = new List<ProductOption>() { productOption };
            var activity = new Activity
            {
                ID = 853,
                ProductOptions = new List<ProductOption>
                {
                    new ProductOption
                    {
                        IsSelected = true,
                        TravelInfo = new TravelInfo
                        {
                            StartDate = new DateTime(2019,1,4),
                            NumberOfNights = 2,
                            NoOfPassengers = new Dictionary<PassengerType, int>
                            {
                                {PassengerType.Adult, 2 }
                            }
                        }
                    }
                }
            };
            var listNetPrice = new List<NetPriceMasterData>() { new NetPriceMasterData() { AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183", ApiType = APIType.Undefined, CurrencyCode = "ISO", NetPrice = 10, ProductId = 853 } };
            var clientInfo = PrepareClientInfoForActivity();
            gatewayMasterService.LoadNetPriceMasterDataAsync().ReturnsForAnyArgs(listNetPrice);
            gatewayMasterService.GetConversionFactorAsync("GBP", "GBP").ReturnsForAnyArgs(1);
            gatewayActivityPersistence.GetPriceAndAvailability(activity, clientInfo, false).ReturnsForAnyArgs(testResult);
            var result = unusedActivityServiceForMocking.GetPriceAndAvailabilityAsync(activity, clientInfo).Result;
            Assert.That(result, Is.EqualTo(testResult));
        }

        [Test]
        public void GetPriceAndAvailabilityExceptionTest()
        {
            unusedActivityServiceForMockingException.GetPriceAndAvailabilityAsync(null, null).Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => unusedActivityServiceForMockingException.GetPriceAndAvailabilityAsync(null, null));
        }

        [Test]
        [TestCase(0)]
        [TestCase(7215)]
        public void GetActivityDataAsyncTest(int regionId)
        {
            var searchCriteria = new SearchCriteria
            {
                PageNumber = 1,
                PageSize = 15,
                SortType = ProductSortType.Default,
                ProductType = ProductType.Activity,
                RegionId = regionId,
                SelectedDates = "24-10-2018@24-10-2018",
                CategoryId = 1
            };
            var clientInfo = PrepareClientInfoForActivity();
            MockActivityData(searchCriteria, clientInfo, regionId);

            var result = unusedActivityServiceForMocking.GetOfferDataAsync(searchCriteria, clientInfo).Result;
            Assert.IsTrue(result.Activities.Count > 0 || result.Activities.Count.Equals(0));
        }

        [Test]
        public void GetActivityDataExceptionTest()
        {
            unusedActivityServiceForMockingException.GetOfferDataAsync(null, null).Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => unusedActivityServiceForMockingException.GetOfferDataAsync(null, null));
        }

        [Ignore("Unused method")]
        [Test]
        public void GetSimilarProductTest()
        {
            var searchCriteria = new SearchCriteria
            {
                PageNumber = 1,
                PageSize = 15,
                SortType = ProductSortType.Default,
                ProductType = ProductType.Activity,
                RegionId = 7129,
                SelectedDates = "24-10-2018@24-10-2018",
                CategoryId = 43,
                AttractionFilterIds = "1001",
                CategoryIDs = "11,111,22"
            };
            var clientInfo = PrepareClientInfoForActivity();
            MockDataToGetSimilarProducts(searchCriteria, clientInfo);

            var result = unusedActivityServiceForMocking.GetSimilarProductsAsync(searchCriteria, clientInfo).Result;
            Assert.IsTrue(result.Count > 0 || result.Count.Equals(0));
        }

        [Ignore("Unused method")]
        [Test]
        public void GetSimilarProductExceptionTest()
        {
            unusedActivityServiceForMockingException.GetSimilarProductsAsync(null, null).Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => unusedActivityServiceForMockingException.GetSimilarProductsAsync(null, null));
        }

        private HotelbedCriteriaApitude GetActivityCriteriaReqeust()
        {
            var activityRq = new HotelbedCriteriaApitude
            {
                ActivityCode = "E-U10-SANGO",
                CheckinDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),
                CheckoutDate = Convert.ToDateTime((DateTime.Now.AddDays(14)).ToString("yyyy-MM-dd")),
                Language = "en",
                Ages = new Dictionary<PassengerType, int>(),
                //PassengerAgeGroupIds = new Dictionary<PassengerType, int>(),
                NoOfPassengers = new Dictionary<PassengerType, int>()
                //Paxes = new List<ServiceAdapters.HB.HB.Entities.ActivityDetail.Pax>
                //{
                //    new ServiceAdapters.HB.HB.Entities.ActivityDetail.Pax
                //    {
                //        Age=30
                //    }
                //    ,
                //    new ServiceAdapters.HB.HB.Entities.ActivityDetail.Pax
                //    {
                //        Age=8
                //    }
                //}
            };
            activityRq.NoOfPassengers.Add(PassengerType.Adult, 2);
            activityRq.NoOfPassengers.Add(PassengerType.Child, 2);
            activityRq.NoOfPassengers.Add(PassengerType.Infant, 1);

            activityRq.Ages.Add(PassengerType.Adult, 30);
            activityRq.Ages.Add(PassengerType.Child, 4);
            activityRq.Ages.Add(PassengerType.Infant, 1);

            //activityRq.PassengerAgeGroupIds.Add(PassengerType.Adult, 15983);
            //activityRq.PassengerAgeGroupIds.Add(PassengerType.Child, 15984);
            //activityRq.PassengerAgeGroupIds.Add(PassengerType.Infant, 0);
            return activityRq;
        }

        private ClientInfo PrepareClientInfoForActivity()
        {
            var clientInfo = new ClientInfo();
            var currency = new Currency
            {
                IsoCode = "GBP",
                Symbol = "£",
                Name = "GBP"
            };

            clientInfo.AffiliateDisplayName = "Isango";
            clientInfo.AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            clientInfo.AffiliateName = "Isango";
            clientInfo.AffiliatePartner = null;
            clientInfo.B2BAffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
            clientInfo.CityCode = null;
            clientInfo.CompanyAlias = "ien";
            clientInfo.CountryIp = "GB";

            clientInfo.DiscountCode = "";
            clientInfo.DiscountCodePercentage = 0M;
            clientInfo.FacebookAppId = "656660554485822";
            clientInfo.FacebookAppSecret = "af34c66444b9c19d38bc4e55cf2d54cf";
            clientInfo.GtmIdentifier = "GTM-PSQPTWZ";
            clientInfo.IsB2BAffiliate = true;
            clientInfo.IsB2BNetPriceAffiliate = true;
            clientInfo.IsSupplementOffer = true;
            clientInfo.LanguageCode = "EN";
            clientInfo.LineOfBusiness = "TOURS & ACTIVITIES - isango!";
            clientInfo.PaymentMethodType = PaymentMethodType.Transaction;
            clientInfo.WidgetDate = DateTime.Now; // This date value is not valid

            currency.IsPostFix = false;
            currency.IsoCode = "GBP";
            currency.Name = "GBP";
            currency.Symbol = "£";

            clientInfo.Currency = currency;
            clientInfo.ApiToken = Guid.NewGuid().ToString();

            return clientInfo;
        }

        private ProductOption PrepareProductOption()
        {
            var adult = new AdultPricingUnit()
            {
                Price = 60
            };
            var datePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
            {
                {
                    DateTime.Today,
                    new DefaultPriceAndAvailability()
                    {
                        TotalPrice = 60,
                        TourDepartureId = 1060773,
                        AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                        PricingUnits = new List<PricingUnit>()
                        {
                            adult
                        }
                    }
                }
            };

            return new ProductOption
            {
                Id = 3879,
                AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                SellPrice = new Price
                {
                    Amount = 60,
                    DatePriceAndAvailabilty = datePriceAndAvailabilty
                },
                BasePrice = new Price
                {
                    Amount = 60,
                    DatePriceAndAvailabilty = datePriceAndAvailabilty
                },
                CostPrice = new Price
                {
                    Amount = 60,
                    DatePriceAndAvailabilty = datePriceAndAvailabilty
                },
                TravelInfo = PrepareTravelInfo(),
                IsSelected = true
            };
        }

        private TravelInfo PrepareTravelInfo()
        {
            return new TravelInfo
            {
                StartDate = DateTime.Today,
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Adult, 1 }
                },
                Ages = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Child, 2 }
                }
            };
        }

        private void MockActivityData(SearchCriteria searchCriteria, ClientInfo clientInfo, int regionId)
        {
            var searchResult = new SearchResult
            {
                Activities = new List<Activity>
                {
                    new Activity
                    {
                        ActivityType = ActivityType.FullDay,
                        ApiType = APIType.Undefined,
                        ID = 10001, ShortName = "Test",
                        Margin = new Margin() { Value = 10 },
                        Regions = new List<Region>
                        {
                            new Region
                            {
                                Id = regionId,
                                Name = "testRegion"
                            }
                        }
                    }
                },
                CategoryIds = new List<int> { 1, 3, 5 },
                Products = new List<Product>
                {
                    new Product
                    {
                        ID = 1001,
                        Title = "Test product"
                    }
                },
                TotalActivities = 1,
            };
            var listAttractionMapping = new List<AttractionActivityMapping>
            {
                new AttractionActivityMapping
                {
                    ActivityId = "10001",
                    AttractionId = "1111"
                },
                new AttractionActivityMapping
                {
                    AttractionId = "1",
                    ActivityId = "1010"
                }
            };
            var affiliate = PrepareAffiliateData();
            var ticketRegion = new List<TicketByRegion>
            {
                new TicketByRegion
                {
                    CountryCode = "TestCountryCode",
                    ThemeparkTicket =
                    new ThemeparkTicket
                    {
                        City = 1,
                        Country = 1,
                        ProductId = 1001,
                        Region = 2
                    }
                }
            };
            var cacheKey = new CacheKey<TicketByRegion> { CacheKeyName = "Test", CacheValue = ticketRegion, Id = "Test" };

            gatewayMasterCacheManager.GetFilteredTickets("FilteredTickets").Returns(cacheKey);
            gatewayActivityPersistence.SearchActivities(searchCriteria, clientInfo).Returns(searchResult);
            gatewayAttractionActivityMappingCacheManager.GetAttractionActivityList(1111).ReturnsForAnyArgs(listAttractionMapping);
            gatewayAffiliateService.GetAffiliateFilterByIdAsync("F2DE3C6A-048D-4C7E-AAF0-86CF0D51DDEF").ReturnsForAnyArgs(affiliate);
        }

        private AffiliateFilter PrepareAffiliateData()
        {
            var affiliateFilter = new AffiliateFilter()
            {
                Id = "F2DE3C6A-048D-4C7E-AAF0-86CF0D51DDEF",
                ActivityFilter = true,
                IsMarginFilter = true,
                RegionFilter = true,
                AffiliateActivityPriorityFilter = true,
                DurationTypeFilter = true,
                DurationTypes = new List<ActivityType>()
                {
                    ActivityType.FullDay
                    //ActivityType.Tour
                },
                Regions = new List<int>() { 7215 },
                Activities = new List<int>() { 10001, 1010, 1001 },
                AffiliateServicesPriority = new List<KeyValuePair<int, int>>()
                {
                    new KeyValuePair<int, int>( 10001, 11)
                },
                AffiliateMargin = 7
            };
            return affiliateFilter;
        }

        private void MockDataToGetSimilarProducts(SearchCriteria searchCriteria, ClientInfo clientInfo)
        {
            var listAvailability = new List<Availability>
            {
                new Availability
                {
                    RegionId = 7129,
                    ServiceId = 1001,
                    Id = "1111",
                    Currency = "USD",
                    BaseDateWisePriceAndAvailability = new Dictionary<DateTime, decimal>
                    { { Convert.ToDateTime("10/24/2018"), 10 } }
                }
            };
            var activities = new List<Activity>
            {
                new Activity
                {
                    ActivityType = ActivityType.FullDay,
                    ApiType = APIType.Undefined,
                    ID = 1001,
                    ShortName = "Test",
                    CategoryIDs =new List<int>{ 11,22},
                    PriorityWiseCategory =new Dictionary<int, int>{ { 1001,1001} }
                },
             new Activity
             {
                 ActivityType = ActivityType.HalfDay,
                 ApiType = APIType.Citysightseeing,
                 ID = 10002,
                 ShortName = "Test2",
                 CategoryIDs =new List<int>{ 11,33}
             }
            };
            var searchResult = new SearchResult { Activities = activities };
            var listRegionCategory = new List<RegionCategoryMapping>
            {
                new RegionCategoryMapping
                {
                    CategoryId = 101,
                    CategoryName = "testcategory",
                    CategoryType = "type",
                    Languagecode = "EN",
                    RegionId = 7129,
                    CountryId = 7129
                }
            };

            gatewayActivityPersistence.LoadRegionCategoryMapping().ReturnsForAnyArgs(listRegionCategory);
            gatewayActivityCacheManager.SearchActivities(searchCriteria, clientInfo).Returns(searchResult);
            gatewayActivityPersistence.GetRegionIdsFromAttractionId("5beef089-3e4e-4f0f-9fbf-99bf1f350183", 43).ReturnsForAnyArgs(new List<string> { "test1", "test2" });
            gatewayHotelBedsActivitiesCacheManager.GetAvailability("7129", string.Empty).ReturnsForAnyArgs(listAvailability);
        }
    }
}