using Autofac;
using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Availability;
using Isango.Entities.Bokun;
using Isango.Entities.Enums;
using Isango.Entities.GrayLineIceLand;
using Isango.Entities.Payment;
using Isango.Entities.Prio;
using Isango.Entities.Region;
using Isango.Entities.Ticket;
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ThemeparkTicket = Isango.Entities.ThemeparkTicket;
using TicketByRegion = Isango.Entities.TicketByRegion;

namespace Isango.Services.Test
{
    [TestFixture]
    public class SearchServiceTest : BaseTest
    {
        private ISearchService _searchService;
        private SearchService searchServiceForMocking;
        private SearchService searchServiceForMockingException;
        private IActivityPersistence gatewayActivityPersistence;
        private IMasterCacheManager gatewayMasterCacheManager;
        private IAffiliateService gatewayAffiliateService;
        private IActivityCacheManager gatewayActivityCacheManager;
        private IMasterService gatewayMasterService;
        private IHotelBedsActivitiesCacheManager gatewayHotelBedsActivitiesCacheManager;
        public IHBAdapter gatewayHBAdapter;
        private IActivityService gatewayActivityService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();
            gatewayActivityPersistence = Substitute.For<IActivityPersistence>();
            gatewayActivityCacheManager = Substitute.For<IActivityCacheManager>();
            gatewayMasterCacheManager = Substitute.For<IMasterCacheManager>();
            var gatewayLogger = Substitute.For<ILogger>();
            gatewayAffiliateService = Substitute.For<IAffiliateService>();
            gatewayMasterService = Substitute.For<IMasterService>();
            gatewayHotelBedsActivitiesCacheManager = Substitute.For<IHotelBedsActivitiesCacheManager>();
            gatewayActivityService = Substitute.For<IActivityService>();

            searchServiceForMocking = new SearchService(
                gatewayAffiliateService
                , gatewayActivityPersistence
                , gatewayActivityCacheManager
                , gatewayMasterCacheManager
                , gatewayMasterService
                , gatewayActivityService
                , gatewayHotelBedsActivitiesCacheManager
                , gatewayLogger
            );

            searchServiceForMockingException = new SearchService(gatewayAffiliateService
                , gatewayActivityPersistence
                , gatewayActivityCacheManager
                , gatewayMasterCacheManager
                , gatewayMasterService
                , gatewayActivityService
                , gatewayHotelBedsActivitiesCacheManager
                , gatewayLogger
            );

            using (var scope = _container.BeginLifetimeScope())
            {
                _searchService = scope.Resolve<ISearchService>();
            }
        }

        [Test]
        //[TestCase(false, 0, APIType.Undefined)]
        [TestCase(true, 29058, APIType.Bokun)]
        [TestCase(true, 6568, APIType.Graylineiceland)]
        [TestCase(true, 3600, APIType.Hotelbeds)]
        [TestCase(true, 22645, APIType.Moulinrouge)]
        [TestCase(true, 26269, APIType.Prio)]
        public void GetSearchDataTest(bool isLive, int regionId, APIType aPIType)
        {
            var clientInfo = PrepareClientInfoForActivity();
            var searchInput = PrepareSearchDataForActivity(aPIType, regionId);
            var listAffiliate = PrepareAffiliateData();
            var criteria = new Criteria
            {
                CheckinDate = DateTime.Today,
                CheckoutDate = DateTime.Today,
                NoOfPassengers = new Dictionary<PassengerType, int>()
                {
                    {PassengerType.Adult, 1 }
                }
            };
            var searchCriteria = PrepareCriteriaForActivity("Region");
            searchCriteria.RegionId = regionId;
            searchCriteria.IsOffer = true;

            var searchResult = new SearchResult
            {
                Activities = new List<Activity>
                {
                    new Activity
                    {
                        ProductOptions = new List<ProductOption>
                        {
                            new ProductOption
                            {
                                BasePrice = new Price
                                {
                                     Amount = 10
                                },
                                CostPrice = new Price
                                {
                                    Amount = 10
                                }
                            }
                        }
                    }
                },
            };

            gatewayActivityCacheManager.SearchActivities(searchCriteria, clientInfo).ReturnsForAnyArgs(searchResult);
            gatewayAffiliateService.GetAffiliateFilterByIdAsync("F2DE3C6A-048D-4C7E-AAF0-86CF0D51DDEF").ReturnsForAnyArgs(listAffiliate);
            gatewayMasterCacheManager.GetFilteredTickets("filteredTicketTest").ReturnsForAnyArgs(searchInput.Item3);
            gatewayActivityPersistence.SearchActivities(searchCriteria, clientInfo).Returns(searchInput.Item1);
            gatewayActivityService.GetActivitiesWithLivePrice(clientInfo, criteria, searchResult.Activities)
                .ReturnsForAnyArgs(searchResult.Activities);
            MockOutputDataForApiTypes(aPIType);

            var result = searchServiceForMocking.GetSearchDataAsync(searchCriteria, clientInfo, criteria).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetSearchDataAsyncExceptionTest()
        {
            //Catch block scenario
            searchServiceForMockingException.GetSearchDataAsync(null, null, null).Throws(new NullReferenceException());
            Assert.ThrowsAsync<NullReferenceException>(() => searchServiceForMockingException.GetSearchDataAsync(null, null, null));
        }

        /// <summary>
        /// Test case to get searched data by region for live pricing
        /// </summary>
        [Test]
        [Ignore("Direct call to database")]
        public void GetSearchDataByRegionTestForLivePricing()
        {
            var clientInfo = PrepareClientInfoForActivity();
            var currency = new Currency
            {
                IsoCode = "GBP",
                Symbol = "£",
                Name = "GBP"
            };
            clientInfo.Currency = currency;
            var searchCriteria = PrepareCriteriaForActivity("Region");
            searchCriteria.RegionId = 7815;
            var criteria = new Criteria
            {
                CheckinDate = DateTime.Today,
                CheckoutDate = DateTime.Today,
                NoOfPassengers = new Dictionary<PassengerType, int>()
                {
                    {PassengerType.Adult, 1 }
                }
            };

            var result = _searchService.GetSearchDataAsync(searchCriteria, clientInfo, criteria).Result;
            Assert.IsTrue(result?.Activities.Count > 0);
        }

        /// <summary>
        /// Test method to search by category.
        /// This Test will always Pass.
        /// </summary>
        [Test]
        [Ignore("Direct call to database")]
        public void GetSearchDataByCategoryTest()
        {
            var searchCriteria = PrepareCriteriaForActivity("Category");
            searchCriteria.CategoryId = 3;
            var clientInfo = new ClientInfo { AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183", CountryIp = "GB", B2BAffiliateId = "278B6450-0A6B-49D5-8A54-47B3BF6A57B5", LanguageCode = "EN", ApiToken = Guid.NewGuid().ToString() };
            var currency = new Currency
            {
                IsoCode = "GBP",
                Symbol = "£",
                Name = "GBP"
            };
            clientInfo.Currency = currency;
            var criteria = new Criteria
            {
                CheckinDate = DateTime.Today,
                CheckoutDate = DateTime.Today,
                NoOfPassengers = new Dictionary<PassengerType, int>()
                {
                    {PassengerType.Adult, 1 }
                }
            };
            var result = _searchService.GetSearchDataAsync(searchCriteria, clientInfo, criteria).Result;
            Assert.IsTrue(result?.Activities.Count > 0);
        }

        /// <summary>
        /// Test method to search by keyword
        /// </summary>
        [Test]
        [Ignore("Direct call to database")]
        public void GetSearchDataByKeywordTest()
        {
            var searchCriteria = PrepareCriteriaForActivity("Keyword");
            var clientInfo = new ClientInfo() { CountryIp = "AU", AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183", LanguageCode = "EN", ApiToken = Guid.NewGuid().ToString() };
            var currency = new Currency
            {
                IsoCode = "GBP",
                Symbol = "£",
                Name = "GBP"
            };
            clientInfo.Currency = currency;
            var criteria = new Criteria
            {
                CheckinDate = DateTime.Today,
                CheckoutDate = DateTime.Today,
                NoOfPassengers = new Dictionary<PassengerType, int>()
                {
                    {PassengerType.Adult, 1 }
                }
            };

            var result = _searchService.GetSearchDataAsync(searchCriteria, clientInfo, criteria).Result;
            Assert.IsTrue(result?.Activities.Count > 0);

            //Catch block scenario
            searchServiceForMockingException.GetSearchDataAsync(null, null, null).Throws(new Exception());
            Assert.ThrowsAsync<Exception>(() => searchServiceForMockingException.GetSearchDataAsync(null, null, null));
        }

        /// <summary>
        /// Test method to search with attraction filter
        /// </summary>
        [Test]
        [TestCase("4066")]
        [Ignore("Direct call to database")]
        //[TestCase("4428 ")]
        public void GetSearchDataWithAttractionFilter(string attractionFilterIds)
        {
            var clientInfo = PrepareClientInfoForActivity();
            var searchCriteria = PrepareCriteriaForActivity("Region");
            searchCriteria.AttractionFilterIds = attractionFilterIds;
            var criteria = new Criteria
            {
                CheckinDate = DateTime.Today,
                CheckoutDate = DateTime.Today,
                NoOfPassengers = new Dictionary<PassengerType, int>()
                {
                    {PassengerType.Adult, 1 }
                }
            };

            var result = _searchService.GetSearchDataAsync(searchCriteria, clientInfo, criteria).Result;
            Assert.IsTrue(result?.Activities.Count > 0);
        }

        /// <summary>
        /// Test method to search with smart phone filter
        /// </summary>
        [Test]
        public void GetSearchDataWithSmartphoneFilter()
        {
            var clientInfo = PrepareClientInfoForActivity();
            var searchCriteria = PrepareCriteriaForActivity("Region");
            searchCriteria.IsSmartphoneFilter = true;
            searchCriteria.CategoryId = 0;
            var criteria = new Criteria
            {
                CheckinDate = DateTime.Today,
                CheckoutDate = DateTime.Today,
                NoOfPassengers = new Dictionary<PassengerType, int>()
                {
                    {PassengerType.Adult, 1 }
                }
            };

            var result = _searchService.GetSearchDataAsync(searchCriteria, clientInfo, criteria);
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Test method to search with date filter
        /// </summary>
        [TestCase("Region")]
        [TestCase("Keyword")]
        [Ignore("Direct call to database")]
        public void GetSearchDataWithDateFilter(string criteriaType)
        {
            var clientInfo = PrepareClientInfoForActivity();
            var searchCriteria = PrepareCriteriaForActivity(criteriaType);
            searchCriteria.SelectedDates = DateTime.Today.Date.ToString("dd-MM-yyyy");
            searchCriteria.CategoryId = 0;

            if (criteriaType == "Keyword")
            {
                searchCriteria.RegionIDs = "6543";
                searchCriteria.RegionId = 0;
            }
            var criteria = new Criteria
            {
                CheckinDate = DateTime.Today,
                CheckoutDate = DateTime.Today,
                NoOfPassengers = new Dictionary<PassengerType, int>()
                {
                    {PassengerType.Adult, 1 }
                }
            };
            var activities = new List<Activity>
            {
                new Activity
                {
                    Id = "1",
                    ProductOptions = new List<ProductOption>
                    {
                        new ProductOption
                        {
                            BasePrice = new Price
                            {
                                Amount = 10
                            },
                            CostPrice = new Price
                            {
                                Amount = 10
                            }
                        }
                    },
                    CategoryIDs = new List<int>(),
                    Regions = new List<Region>
                    {
                        new Region
                        {
                            Id = 6543,
                            Type = RegionType.City
                        }
                    }
                }
            };
            var searchResult = new SearchResult
            {
                Activities = activities
            };
            var availabilities = new List<Availability>
            {
                new Availability
                {
                    Id = "1",
                    Currency = "",
                    ServiceId = 0,
                    RegionId = 6543,
                    BaseDateWisePriceAndAvailability = new Dictionary<DateTime, decimal>
                    {
                        {DateTime.Today, 10}
                    },
                    CostDateWisePriceAndAvailability = new Dictionary<DateTime, decimal>
                    {
                        {DateTime.Today, 10}
                    },
                    ServiceOptionId = 1
                }
            };
            var cacheKey = new CacheKey<TicketByRegion>
            {
                CacheValue = new List<TicketByRegion>
                {
                    new TicketByRegion
                    {
                        CountryCode = "gb",
                        ThemeparkTicket = new ThemeparkTicket
                        {
                            ProductId = 1,
                            City = 0,
                            Country = 0,
                            Region = 6543
                        }
                    }
                }
            };
            var regionActivityMappings = new List<RegionActivityMapping>
            {
                new RegionActivityMapping
                {
                    ServiceId = 0,
                    RegionId = 6180,
                    IsHBService = false
                }
            };

            gatewayActivityCacheManager.SearchActivities(searchCriteria, clientInfo).ReturnsForAnyArgs(searchResult);
            gatewayHotelBedsActivitiesCacheManager.GetAvailability("6543", "").ReturnsForAnyArgs(availabilities);
            gatewayMasterCacheManager.GetFilteredTickets("FilteredTickets").ReturnsForAnyArgs(cacheKey);
            gatewayActivityPersistence
                .GetFullTextSearchActivitiyIdMapping(string.Empty, searchCriteria.Keyword, clientInfo).ReturnsForAnyArgs(regionActivityMappings);

            var result = searchServiceForMocking.GetSearchDataAsync(searchCriteria, clientInfo, criteria).Result;
            Assert.IsTrue(result?.Activities.Count > 0);
        }

        /// <summary>
        /// Test method to search by offer
        /// </summary>
        [Test]
        [Ignore("Direct call to database")]
        public void GetSearchDataByOfferTest()
        {
            var clientInfo = PrepareClientInfoForActivity();
            var currency = new Currency
            {
                IsoCode = "GBP",
                Symbol = "£",
                Name = "GBP"
            };
            clientInfo.Currency = currency;
            var searchCriteria = PrepareCriteriaForActivity("Region");
            searchCriteria.IsOffer = true;
            var criteria = new Criteria
            {
                CheckinDate = DateTime.Today,
                CheckoutDate = DateTime.Today,
                NoOfPassengers = new Dictionary<PassengerType, int>()
                {
                    {PassengerType.Adult, 1 }
                }
            };

            var result = _searchService.GetSearchDataAsync(searchCriteria, clientInfo, criteria).Result;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Activities.Count > 0 || result.Activities.Count.Equals(0));
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

        private Tuple<SearchResult, SearchStack, CacheKey<TicketByRegion>> PrepareSearchDataForActivity(APIType aPIType, int regionId)
        {
            var regions = new List<Region> { new Region { Id = regionId, Name = "test", Type = RegionType.Region } };
            var productOption = PrepareActivityData(aPIType).ProductOptions;
            var activityLite = new List<Activity>
            {
                new Activity() { ActivityType = ActivityType.FullDay, ApiType = aPIType,ID=1001,CategoryIDs=new List<int>(){ 10},Regions=regions, ProductOptions=productOption }
                //new Activity() { ActivityType = ActivityType.Tour, ApiType = aPIType,ID=1002, ProductOptions=productOption }
            };
            var searchResult = new SearchResult { Activities = activityLite };
            var searchStack = new SearchStack { Activities = activityLite };
            var cacheResult = new CacheKey<TicketByRegion>
            {
                CacheKeyName = "test",
                CacheValue = new List<TicketByRegion>
                {
                    new TicketByRegion
                    {
                        CountryCode = "INR",
                        ThemeparkTicket = new ThemeparkTicket
                        {
                            City = 1,
                            Country = 11,
                            ProductId = 111,
                            Region = 1111
                        }
                    }
                }
            };

            var tuple = Tuple.Create(searchResult, searchStack, cacheResult);
            return tuple;
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
                    ActivityType.FullDay,
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

        /// <summary>
        /// Method to prepare a mock input data
        /// </summary>
        /// <param name="searchType">type of Search criteria</param>
        /// <returns></returns>
        private SearchCriteria PrepareCriteriaForActivity(string searchType)
        {
            var searchCriteria = new SearchCriteria();

            const string sortBy = "";
            const string regionFilterIds = "";
            const string attractionFilterIds = "";
            const string criteria = "";
            var selectedDates = "";
            const string isOffer = "";
            const string isSmartPhone = "";

            var id = searchType == "Region" ? "6180" : searchType == "Category" ? "8188" : string.Empty;
            var catId = searchType == "Category" ? "43" : string.Empty;
            var keyword = searchType == "Keyword" ? "Par" : string.Empty;

            const int pageNumber = 1;
            const int productsToShow = 15;

            if (!string.IsNullOrEmpty(id))
            {
                searchCriteria.RegionId = int.Parse(id);
            }
            if (!string.IsNullOrEmpty(catId))
            {
                searchCriteria.CategoryId = int.Parse(catId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                searchCriteria.Keyword = keyword;
            }

            if (!string.IsNullOrEmpty(criteria))
            {
                var data = criteria.Split('|');
                if (!string.IsNullOrEmpty(data[0]))
                {
                    searchCriteria.RegionId = int.Parse(data[0]);
                }
                if (!string.IsNullOrEmpty(data[1]))
                {
                    searchCriteria.CategoryId = int.Parse(data[1]);
                }
                if (!string.IsNullOrEmpty(data[2]))
                {
                    searchCriteria.Keyword = data[2];
                }
            }

            searchCriteria.PageNumber = pageNumber;
            searchCriteria.PageSize = productsToShow;
            searchCriteria.RegionFilterIds = regionFilterIds;
            searchCriteria.AttractionFilterIds = attractionFilterIds;
            searchCriteria.IsOffer = isOffer.ToLowerInvariant() == "true";
            searchCriteria.IsSmartphoneFilter = isSmartPhone.ToLowerInvariant() == "true";

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower(CultureInfo.InvariantCulture))
                {
                    case "price":
                        searchCriteria.SortType = ProductSortType.Price;
                        break;

                    case "userrating":
                        searchCriteria.SortType = ProductSortType.UserReviewRating;
                        break;

                    case "offers":
                        searchCriteria.SortType = ProductSortType.Offers;
                        break;

                    default:
                        searchCriteria.SortType = ProductSortType.Default;
                        break;
                }
            }
            else
            {
                searchCriteria.SortType = ProductSortType.Default;
            }

            if (string.IsNullOrWhiteSpace(selectedDates))
            {
                selectedDates = "";
            }

            searchCriteria.SelectedDates = selectedDates;
            searchCriteria.ProductType = ProductType.Activity;

            return searchCriteria;
        }

        public Activity PrepareActivityData(APIType aPIType)
        {
            var pricingUnitPrio = new List<PerPersonPricingUnit>()
            {
                new PerPersonPricingUnit()
                {
                    Price =40,
                    PriceType =PriceType.PerPerson,
                    TotalCapacity =10,
                    UnitType =UnitType.PerPerson
                }
            };
            var price = new Price()
            {
                Amount = 50,
                Currency = new Currency
                {
                    IsoCode = "GBP",
                    Symbol = "£",
                    Name = "GBP"
                },
                DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                {
                    { DateTime.Now, new PrioPriceAndAvailability()
                    {
                        AvailabilityStatus =AvailabilityStatus.AVAILABLE,
                        PricingUnits = pricingUnitPrio.Cast<PricingUnit>().ToList()
                    }
                    }
                }
            };
            var productOptions = new List<ProductOption> { new ProductOption { Id = 1001, Name = "TestName", SupplierOptionCode = "1101", BasePrice = price } };
            var activity = new Activity()
            {
                ID = 1001,
                Code = "1101",
                RegionName = "testRegionname",
                FactsheetId = 5555,
                Regions = new List<Region>()
                {
                    new Region()
                    {
                        Id = 1001,
                        Name = "regiontest",
                        ParentId = 1101, Type = RegionType.Country, Url = "testUrl" } },
                Margin = new Margin() { CurrencyCode = "USD", IsPercentage = true, Value = 10 },
                ProductOptions = productOptions,
                ActivityType = ActivityType.FullDay,
                ApiType = aPIType,
                Id = "1001"
            };

            return activity;
        }

        private void MockOutputDataForApiTypes(APIType aPIType)
        {
            var prioCriteria = new PrioCriteria();
            var bokunCriteria = new BokunCriteria();
            var gliCriteria = new GrayLineIcelandCriteria();
            var ticketCriteria = new TicketCriteria();

            var activity = PrepareActivityData(aPIType);
            var activities = new List<Activity>
            {
                activity
            };
            var productOptions = new List<ProductOption> { new ProductOption { Id = 1001, Name = "TestName", SupplierOptionCode = "1101", BasePrice = null } };
            var listCurrency = new List<CurrencyExchangeRates>
            {
                new CurrencyExchangeRates
                {
                    ExchangeRate = 1,
                    FromCurrencyCode = "GBP",
                    ToCurrencyCode = "GBP"
                }
            };
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
            gatewayMasterService.GetConversionFactorAsync("USD", "USD").ReturnsForAnyArgs(1);
            gatewayMasterService.GetBokunPriceCategoryByActivityAsync().ReturnsForAnyArgs(new List<Entities.Bokun.PriceCategory> { new Entities.Bokun.PriceCategory() { ServiceId = 1001, PriceCategoryId = 11, AgeGroupId = 2 } });

            if (aPIType == APIType.Moulinrouge || aPIType == APIType.Graylineiceland || aPIType == APIType.Hotelbeds || aPIType == APIType.Bokun)
            {
                var activityOption = new List<ActivityOption> { new ActivityOption { Id = 1001, Name = "TestName", SupplierOptionCode = "1101", BasePrice = activity.ProductOptions?[0].BasePrice, CancellationPrices = new List<CancellationPrice>() { new CancellationPrice { CancellationAmount = 100, CancellationFromdate = DateTime.Now.AddDays(1), Percentage = 30 } } } };
                activity.ProductOptions = activityOption.Cast<ProductOption>().ToList();
            }

            gatewayActivityPersistence.LoadRegionCategoryMapping().ReturnsForAnyArgs(listRegionCategory);
            gatewayMasterService.LoadCurrencyExchangeRatesAsync().ReturnsForAnyArgs(listCurrency);
        }
    }
}