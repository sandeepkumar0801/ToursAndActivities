using Autofac;
using CacheManager;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Payment;
using Isango.Entities.Region;
using Isango.Entities.Wrapper;
using Isango.Persistence.Test;
using Isango.Register;
using Logger.Contract;
using MongoDB.Driver;
using NSubstitute;
using ServiceAdapters.Rayna.Rayna.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Isango.Cache.Test
{
    public class ActivityCacheTest : BaseTest
    {
        private IActivityCacheManager _activityCacheManager;

        private IActivityCacheManager _activityCacheManagerMock;
        private ActivityCacheManager _activityCacheManagerService;
        //private ActivityCacheManager _activityCacheManagerServiceExcpetion;
        private CollectionDataFactory<Activity> _collectionDataFactory;
        private CollectionDataFactory<RegionCategoryMapping> _collectionDataRegionCategoryMapping;
        private CollectionDataFactory<ActivityWithApiType> _collectionDataActivityIdApiType;
        private CollectionDataFactory<CacheKey<RegionCategoryMapping>> _collectionCacheKeyRegionCategoryMapping;
        private ILogger _log;

        public void TestInitialise()
        {
            _activityCacheManagerMock = Substitute.For<IActivityCacheManager>();
            _collectionDataFactory = Substitute.For<CollectionDataFactory<Activity>>();
            _collectionDataRegionCategoryMapping = Substitute.For<CollectionDataFactory<RegionCategoryMapping>>();
            _collectionDataActivityIdApiType = Substitute.For<CollectionDataFactory<ActivityWithApiType>>();
            _collectionCacheKeyRegionCategoryMapping = Substitute.For<CollectionDataFactory<CacheKey<RegionCategoryMapping>>>();
            _log = Substitute.For<ILogger>();
            _activityCacheManagerService = new ActivityCacheManager(_collectionDataRegionCategoryMapping, _collectionDataActivityIdApiType, _collectionCacheKeyRegionCategoryMapping, _log, _collectionDataFactory);
            //_activityCacheManagerServiceExcpetion = new ActivityCacheManager(_collectionDataRegionCategoryMapping, _collectionDataActivityIdApiType, _collectionCacheKeyRegionCategoryMapping , _log, _collectionDataFactory);

            //var container = Startup._builder.Build();

            //using (var scope = container.BeginLifetimeScope())
            //{
            //    _activityCacheManager = scope.Resolve<IActivityCacheManager>();
            //}
        }

        [Fact]
        public void GetActivityTest()
        {
            var activity = new Activity()
            {
                Id = "842"
            };

            var activities = new List<Activity>
            {
                activity
            };

            _collectionDataFactory.GetCollectionDataHelper().GetResultList("test", "").ReturnsForAnyArgs(activities);
            _collectionDataFactory.GetCollectionDataHelper().GetResult("test", "").ReturnsForAnyArgs(activity);
            _activityCacheManagerMock.GetActivity("842", "en").ReturnsForAnyArgs(activity);
            var searchedActivity = _activityCacheManager.GetActivity("842", "en");
            Assert.True(searchedActivity.Id == "842");
        }

        #region SearchDataTestCases

        /// <summary>
        /// Test method to search by region
        /// </summary>
        [Fact]
        public void GetSearchDataByRegionTest()
        {
            var clientInfo = PrepareClientInfo();
            var currency = new Currency
            {
                IsoCode = "GBP",
                Symbol = "£",
                Name = "GBP"
            };
            clientInfo.Currency = currency;
            var searchCriteria = PrepareCriteria("Region");
            var result = _activityCacheManager.SearchActivities(searchCriteria, clientInfo);
            Assert.True(result?.Activities.Count > 0);
        }

        /// <summary>
        /// Test method to search by category.
        /// This Test will always Pass.
        /// </summary>
        [Fact]
        public void GetSearchDataByCategoryTest()
        {
            var searchCriteria = PrepareCriteria("Category");
            var clientInfo = new ClientInfo()
            {
                CountryIp = "AU",
                B2BAffiliateId = "278B6450-0A6B-49D5-8A54-47B3BF6A57B5",
                LanguageCode = "EN"
            };
            var currency = new Currency
            {
                IsoCode = "GBP",
                Symbol = "£",
                Name = "GBP"
            };
            clientInfo.Currency = currency;

            var result = _activityCacheManager.SearchActivities(searchCriteria, clientInfo);
            Assert.True(result?.Activities.Count > 0);
        }

        /// <summary>
        /// Test method to search by keyword
        /// </summary>
        [Fact]
        public void GetSearchDataByKeywordTest()
        {
            var searchCriteria = PrepareCriteria("Keyword");
            var clientInfo = new ClientInfo()
            {
                CountryIp = "AU",
                B2BAffiliateId = "278B6450-0A6B-49D5-8A54-47B3BF6A57B5",
                LanguageCode = "EN"
            };
            var currency = new Currency
            {
                IsoCode = "GBP",
                Symbol = "£",
                Name = "GBP"
            };
            clientInfo.Currency = currency;

            var result = _activityCacheManager.SearchActivities(searchCriteria, clientInfo);
            Assert.True(result?.Activities.Count > 0);
        }

        /// <summary>
        /// Test method to search with attraction filter
        /// </summary>
        [Theory]
        [InlineData("43")]
        [InlineData("43:4106: ")]
        public void GetSearchDataWithAttractionFilter(string attractionFilterIds)
        {
            var clientInfo = PrepareClientInfo();
            var searchCriteria = PrepareCriteria("Region");
            searchCriteria.AttractionFilterIds = attractionFilterIds;
            searchCriteria.CategoryId = 0;

            var result = _activityCacheManager.SearchActivities(searchCriteria, clientInfo);
            Assert.True(result?.Activities.Count > 0);
        }

        /// <summary>
        /// Test method to search with smartphone filter
        /// </summary>
        [Fact]
        public void GetSearchDataWithSmartphoneFilter()
        {
            var clientInfo = PrepareClientInfo();
            var searchCriteria = PrepareCriteria("Region");
            searchCriteria.IsSmartphoneFilter = true;
            searchCriteria.CategoryId = 0;

            var result = _activityCacheManager.SearchActivities(searchCriteria, clientInfo);
            Assert.True(result != null);
        }

        /// <summary>
        /// Test method to search with date filter
        /// </summary>
        [InlineData("27-09-2018", "Region")]
        [InlineData("28-09-2018", "Region")]
        [InlineData("27-09-2018@28-09-2018", "Region")]
        public void GetSearchDataWithDateFilter(string dates, string criteriaType)
        {
            var clientInfo = PrepareClientInfo();
            var searchCriteria = PrepareCriteria(criteriaType);
            searchCriteria.SelectedDates = dates;
            searchCriteria.CategoryId = 0;

            var result = _activityCacheManager.SearchActivities(searchCriteria, clientInfo);
            Assert.True(result?.Activities.Count > 0);
        }

        /// <summary>
        /// Test method to search by offer
        /// </summary>
        [Fact]
        public void GetSearchDataByOfferTest()
        {
            var clientInfo = PrepareClientInfo();
            var currency = new Currency
            {
                IsoCode = "GBP",
                Symbol = "£",
                Name = "GBP"
            };
            clientInfo.Currency = currency;
            var searchCriteria = PrepareCriteria("Region");
            searchCriteria.IsOffer = true;
            var result = _activityCacheManager.SearchActivities(searchCriteria, clientInfo);
            Assert.True(result?.Activities.Count > 0);
        }

        #endregion SearchDataTestCases

        [Fact(Skip = "Insert activity in activity collection")]
        public void ActivityIdUpdateCacheTest()
        {
            var activity = new Activity()
            {
                Id = "353",
                ActivityType = ActivityType.FullDay,
                Name = "Test",
                ApiType = APIType.Undefined
            };

            var result = _activityCacheManager.InsertActivity(activity);
            Assert.True(result != null);
        }

        [Fact]
        public void GetRegioncategoryMapping()
        {
            var result = _activityCacheManager.GetRegioncategoryMapping();
            Assert.True(result != null);
        }

        [Fact]
        public void DeleteAndCreateCollection()
        {
            _collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist("TestCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().DeleteCollection("TestCollection").ReturnsForAnyArgs(true);
            _collectionDataFactory.GetCollectionDataHelper().CreateCollection("TestCollection", "/activityId").ReturnsForAnyArgs(true);
            var result = _activityCacheManagerService.DeleteAndCreateCollection("en");
            Assert.True(result != null);
        }

        [Fact]
        public void InsertActivityTest()
        {
            var activity = new Activity()
            {
                ID = 1,
                Name = "test 1"
            };

            _collectionDataFactory.GetCollectionDataHelper().InsertDocument("ActivityCollection", activity).ReturnsForAnyArgs(true);
            var result = _activityCacheManagerService.InsertActivity(activity, "en");
            Assert.True(result != null);
        }

        [Fact]
        public void GetSearchResultByProductIdsTest()
        {
            var searchCriteria = new SearchCriteria()
            {
                CategoryId = 0,
                Keyword = string.Empty,
                ProductIDs = "1,2,3"
            };

            var activity = new Activity()
            {
                ID = 1,
                Name = "test",
                ShortName = "sn",
                ShortIntroduction = "EN",
                DurationString = "asds",
                ScheduleOperates = "asdas",
                SellMinPrice = 10M,
                Regions = new List<Region>()
                        {
                            new Region()
                            {
                                Id = 7128
                            }
                        },
                TotalReviewCount = 5,
                OverAllRating = 4.5,
                AdditionalMarkUp = 2.3F,
                ActualServiceUrl = string.Empty,
                CoOrdinates = string.Empty,
                Priority = 5,
                Images = null,
                CategoryIDs = new List<int>()
                        {
                            4,5
                        },
                ReasonToBook = new List<string>()
                        {
                            "testing","mock"
                        },
                ApiType = APIType.Hotelbeds,
                ProductOptions = new List<ProductOption>()
                        {
                            new ProductOption()
                            {
                                Id = 5,
                                Description ="sdkhakjd"
                            }
                        },
                ActivityType = ActivityType.FullDay
            };

            var activityList = new List<Activity>
            {
                activity
            };

            var searchResult = new SearchResult()
            {
                Activities = new List<Activity>()
                {
                    new Activity()
                    {
                        ID = 1,
                        Name = "test",
                        ShortName = "sn",
                        ShortIntroduction = "EN",
                        DurationString = "asds",
                        ScheduleOperates = "asdas",
                        SellMinPrice = 10M,
                        Regions = new List<Region>()
                        {
                            new Region()
                            {
                                Id = 7128
                            }
                        },
                        TotalReviewCount = 5,
                        OverAllRating = 4.5,
                        AdditionalMarkUp = 2.3F,
                        ActualServiceUrl = string.Empty,
                        CoOrdinates = string.Empty,
                        Priority = 5,
                        Images = null,
                        CategoryIDs =  new List<int>()
                        {
                            4,5
                        },
                        ReasonToBook = new List<string>()
                        {
                            "testing","mock"
                        },
                        ApiType = APIType.Hotelbeds,
                        ProductOptions = new List<ProductOption>()
                        {
                            new ProductOption()
                            {
                                Id = 5,
                                Description ="sdkhakjd"
                            }
                        },
                        ActivityType = ActivityType.FullDay,
                    }
                }
            };

            _collectionDataFactory.GetCollectionDataHelper().GetResult("TestCollection", string.Empty).ReturnsForAnyArgs(activity);
            _collectionDataFactory.GetCollectionDataHelper().GetResultList("TestCollection", "").ReturnsForAnyArgs(activityList);
            _activityCacheManagerMock.SearchActivities(searchCriteria, null).ReturnsForAnyArgs(searchResult);
            var result = _activityCacheManagerService.SearchActivities(searchCriteria, null);
            Assert.True(result != null);
        }

        [Fact]
        public void FilterProductsByApiTypeTest()
        {
            var searchCriteria = new SearchCriteria()
            {
                CategoryId = 0,
                Keyword = string.Empty,
                ProductIDs = "1,2,3",
                CategoryIDs = "4,5,6",
                RegionFilterIds = "6,7",
                AttractionFilterIds = "4,5,6",
                IsSmartphoneFilter = true
            };

            var activity = new Activity()
            {
                ID = 1,
                ApiType = APIType.Hotelbeds,
                CategoryIDs = new List<int>()
                        {
                            4,5,6
                        },
                Regions = new List<Region>()
                        {
                            new Region()
                            {
                                Id = 6,
                                Type = RegionType.City
                            }
                        },

                Name = "test",
                ShortName = "sn",
                ShortIntroduction = "EN",
                DurationString = "asds",
                ScheduleOperates = "asdas",
                SellMinPrice = 10M,
                TotalReviewCount = 5,
                OverAllRating = 4.5,
                AdditionalMarkUp = 2.3F,
                ActualServiceUrl = string.Empty,
                CoOrdinates = string.Empty,
                Priority = 5,
                Images = null,
                ReasonToBook = new List<string>()
                        {
                            "testing","mock"
                        },
                ProductOptions = new List<ProductOption>()
                        {
                            new ProductOption()
                            {
                                Id = 5,
                                Description ="sdkhakjd"
                            }
                        },
                ActivityType = ActivityType.FullDay
            };

            var searchResult = new SearchResult()
            {
                Activities = new List<Activity>()
                {
                    new Activity()
                    {
                        ID = 1,
                        ApiType = APIType.Hotelbeds,
                        CategoryIDs = new List<int>()
                        {
                            4,5,6
                        },
                        Regions = new List<Region>()
                        {
                            new Region()
                            {
                                Id = 6,
                                Type = RegionType.City
                            }
                        },

                        Name = "test",
                        ShortName = "sn",
                        ShortIntroduction = "EN",
                        DurationString = "asds",
                        ScheduleOperates = "asdas",
                        SellMinPrice = 10M,
                        TotalReviewCount = 5,
                        OverAllRating = 4.5,
                        AdditionalMarkUp = 2.3F,
                        ActualServiceUrl = string.Empty,
                        CoOrdinates = string.Empty,
                        Priority = 5,
                        Images = null,
                        ReasonToBook = new List<string>()
                        {
                            "testing","mock"
                        },
                        ProductOptions = new List<ProductOption>()
                        {
                            new ProductOption()
                            {
                                Id = 5,
                                Description ="sdkhakjd"
                            }
                        },
                        ActivityType = ActivityType.FullDay
                    },
                    new Activity()
                    {
                        ID = 2,
                        ApiType = APIType.Graylineiceland,
                        CategoryIDs = new List<int>()
                        {
                            4,5,6
                        },
                        Regions = new List<Region>()
                        {
                            new Region()
                            {
                                Id = 6,
                                Type = RegionType.City
                            }
                        },

                        Name = "test",
                        ShortName = "sn",
                        ShortIntroduction = "EN",
                        DurationString = "asds",
                        ScheduleOperates = "asdas",
                        SellMinPrice = 10M,
                        TotalReviewCount = 5,
                        OverAllRating = 4.5,
                        AdditionalMarkUp = 2.3F,
                        ActualServiceUrl = string.Empty,
                        CoOrdinates = string.Empty,
                        Priority = 5,
                        Images = null,
                        ReasonToBook = new List<string>()
                        {
                            "testing","mock"
                        },
                        ProductOptions = new List<ProductOption>()
                        {
                            new ProductOption()
                            {
                                Id = 5,
                                Description ="sdkhakjd"
                            }
                        },
                        ActivityType = ActivityType.FullDay,
                    }
                }
            };
            _collectionDataFactory.GetCollectionDataHelper().GetResult("ActivityCollection", string.Empty).ReturnsForAnyArgs(activity);
            _activityCacheManagerMock.SearchActivities(searchCriteria, null).ReturnsForAnyArgs(searchResult);
            var result = _activityCacheManagerService.SearchActivities(searchCriteria, null);
            Assert.True(result != null);
        }
 
        [Fact]
        public void GetActivityWithApiType()
        {
            var result = _activityCacheManagerService.GetActivityWithApiType();
            Assert.True(result != null);
        }

        //[Test]
        //public void Test()
        //{
        //    var result = _activityCacheManagerService.();
        //    Assert.IsTrue(result);
        //}

        //[Test]
        //public void Test()
        //{
        //    var result = _activityCacheManagerService.();
        //    Assert.IsTrue(result);
        //}

        private ClientInfo PrepareClientInfo()
        {
            var clientInfo = new ClientInfo();
            var currency = new Currency();

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
            clientInfo.IsB2BAffiliate = false;
            clientInfo.IsB2BNetPriceAffiliate = false;
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

            return clientInfo;
        }

        private SearchCriteria PrepareCriteria(string searchType)
        {
            var searchCriteria = new SearchCriteria();

            const string sortBy = "";
            const string regionFilterIds = "";
            const string attractionFilterIds = "";
            const string criteria = "";
            var selectedDates = "";
            const string isOffer = "";
            const string isSmartPhone = "";

            var id = searchType == "Region" ? "7129" : searchType == "Category" ? "8188" : string.Empty;
            var catId = searchType == "Category" ? "3800" : string.Empty;
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
    }
}