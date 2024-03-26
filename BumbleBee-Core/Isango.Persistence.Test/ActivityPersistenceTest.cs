//using Isango.Entities;
//using Isango.Entities.Activities;
//using Isango.Entities.Enums;
//using Isango.Entities.Payment;
//using Isango.Persistence.Contract;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Isango.Persistence.Test
//{
//    public class ActivityPersistenceTest
//    {
//        private IActivityPersistence _activityPersistence;

//        [OneTimeSetUp]
//        public void TestInitialise()
//        {
//            var container = Startup._builder.Build();

//            using (var scope = container.BeginLifetimeScope())
//            {
//                _activityPersistence = scope.Resolve<IActivityPersistence>();
//            }
//        }

//        /// <summary>
//        /// Test case to search activities  by region, category and keyword
//        /// </summary>
//        /// <param name="searchType"></param>
//        /// <param name="countryIp"></param>
//        /// <param name="b2BAffiliateId"></param>
//        [Fact]
//        [TestCase("Region", "", "")]
//        [TestCase("Category", "GB", "278B6450-0A6B-49D5-8A54-47B3BF6A57B5")]
//        //[TestCase("Keyword", "GB", "278B6450-0A6B-49D5-8A54-47B3BF6A57B5")]
//        public void SearchActivitiesTest(string searchType, string countryIp, string b2BAffiliateId)
//        {
//            var clientInfo = PrepareClientInfo(countryIp, b2BAffiliateId);
//            var currency = new Currency
//            {
//                IsoCode = "GBP",
//                Symbol = "£",
//                Name = "GBP"
//            };
//            clientInfo.Currency = currency;

//            var searchCriteria = PrepareCriteria(searchType);
//            var result = _activityPersistence.SearchActivities(searchCriteria, clientInfo);
//            Assert.IsNotNull(result.Activities);
//        }

//        [Fact]
//        public void GetSearchDataByKeywordTest()
//        {
//            var searchCriteria = PrepareCriteria("Keyword");
//            var clientInfo = new ClientInfo() { CountryIp = "AU", B2BAffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183", LanguageCode = "en" };
//            var currency = new Currency
//            {
//                IsoCode = "GBP",
//                Symbol = "£",
//                Name = "GBP"
//            };
//            clientInfo.Currency = currency;

//            var result = _activityPersistence.SearchActivities(searchCriteria, clientInfo);

//            Assert.True(result?.Activities.Count > 0);
//        }

//        /// <summary>
//        /// Test case to get activities
//        /// </summary>
//        /// <param name="activityId"></param>
//        /// <param name="languageCode"></param>
//        [Fact]
//        [TestCase("6873", "en")]
//        public void GetActivityTest(string activityId, string languageCode)
//        {
//            var result = _activityPersistence.GetActivitiesByActivityIds(activityId, languageCode);
//             Assert.True(result != null);
//        }

//        /// <summary>
//        /// Test case to get availability data
//        /// </summary>
//        /// <param name="keyword"></param>
//        /// <param name="countryIp"></param>
//        /// <param name="b2BAffiliateId"></param>
//        [Fact]
//        [TestCase("Par", "GB", "278B6450-0A6B-49D5-8A54-47B3BF6A57B5")]
//        public void GetAvailabilityDataTest(string keyword, string countryIp, string b2BAffiliateId)
//        {
//            var clientInfo = PrepareClientInfo(countryIp, b2BAffiliateId);
//            var result = _activityPersistence.GetFullTextSearchActivitiyIdMapping(string.Empty, keyword, clientInfo);
//            Assert.True(result != null);
//        }

//        /// <summary>
//        /// Test case to get Isango availability
//        /// </summary>
//        /// <param name="regionId"></param>
//        [Fact]
//        [TestCase("0")]
//        [TestCase("6267")]
//        public void GetOptionAvailabilityTest(string regionId)
//        {
//            var result = _activityPersistence.GetOptionAvailability(regionId);
//            Assert.True(result != null);
//        }

//        /// <summary>
//        /// Test case to get Calender availability
//        /// </summary>
//        [Fact]
//        public void GetCalenderAvailabilityTest()
//        {
//            var result = _activityPersistence.GetCalendarAvailability();
//            Assert.True(result != null);

//            var result1 = _activityPersistence.GetCalendarAvailability("853");
//            Assert.True(result != null);
//        }

//        /// <summary>
//        /// Test case to load region category mapping
//        /// </summary>
//        [Fact]
//        public void LoadRegionCategoryMappingTest()
//        {
//            var result = _activityPersistence.LoadRegionCategoryMapping();
//            Assert.True(result != null);
//        }

//        /// <summary>
//        /// Test case to get region ids from attraction id
//        /// </summary>
//        /// <param name="affiliateId"></param>
//        /// <param name="attractionId"></param>
//        [Fact]
//        [TestCase("278B6450-0A6B-49D5-8A54-47B3BF6A57B5", 2)]
//        public void GetRegionIdsFromAttractionIdTest(string affiliateId, int attractionId)
//        {
//            var result = _activityPersistence.GetRegionIdsFromAttractionId(affiliateId, attractionId);
//             Assert.True(result != null);
//        }

//        /// <summary>
//        /// Test case to load region metadata
//        /// </summary>
//        /// <param name="regionId"></param>
//        /// <param name="catid"></param>
//        /// <param name="languageCode"></param>
//        [Fact]
//        [TestCase(7129, 8188, "en")]
//        public void LoadRegionMetaDataTest(int regionId, int catid, string languageCode)
//        {
//            var result = _activityPersistence.LoadRegionMetaData(regionId, catid, languageCode);
//             Assert.True(result != null);
//        }

//        /// <summary>
//        /// Test method to load live HB activities
//        /// </summary>
//        [Fact]
//        public void LoadLiveHbActivitiesTest()
//        {
//            var serviceIDs = "32964,32965,32966,32967,32968";
//            var list = serviceIDs.Split(',').ToList();
//            var result = default(List<Activity>);
//            foreach (var item in list)
//            {
//                result = _activityPersistence.LoadLiveHbActivities(Convert.ToInt32(item), "EN");
//            }
//             Assert.True(result != null);
//            Assert.True(result?.Count > 0);
//        }

//        /// <summary>
//        /// Test method to get live services by language code
//        /// </summary>
//        [Fact]
//        [TestCase("en")]
//        [TestCase("esp")]
//        public void GetLiveServicesTest(string languageCode)
//        {
//            var result = _activityPersistence.GetLiveActivityIds(languageCode);
//             Assert.True(result != null);
// Assert.True(result?.Length> 0);
//        }

//        /// <summary>
//        /// Test method to get activities by activity id
//        /// </summary>
//        [Fact]
//        public void GetActivitiesByActivityIdsTest()
//        {
//            var result = _activityPersistence.GetActivitiesByActivityIds("842,846,850,853,855", "en");
//             Assert.True(result != null);
//            Assert.True(result?.Count > 0);
//        }

//        /// <summary>
//        /// Test method to get auto suggest data
//        /// </summary>
//        [Fact]
//        public void GetAutoSuggestDataTest()
//        {
//            var result = _activityPersistence.GetAutoSuggestData("5beef089-3e4e-4f0f-9fbf-99bf1f350183");
//             Assert.True(result != null);
//            Assert.True(result?.Count > 0);
//        }

//        /// <summary>
//        /// Test case to get activity id
//        /// </summary>
//        [Fact]
//        public void GetActivityIdTest()
//        {
//            var result = _activityPersistence.GetActivityId(14341);
//             Assert.True(result != null);
//        }

//        /// <summary>
//        /// Test case to get the Modified Services for caching service
//        /// </summary>
//        [Fact]
//        public void GetModifiedServicesTest()
//        {
//            var result = _activityPersistence.GetModifiedServices();
//             Assert.True(result != null);
//        }

//        [Fact]
//        public void GetPriceAndAvailabilityTest()
//        {
//            var activity = new Activity()
//            {
//                ID = 853,
//                ProductOptions = new List<ProductOption>()
//                {
//                    new ProductOption()
//                    {
//                        Id = 56750,
//                        TravelInfo = new TravelInfo()
//                        {
//                            NumberOfNights = 1,
//                            NoOfPassengers = new Dictionary<PassengerType, int>()
//                            {
//                                {
//                                    PassengerType.Adult,
//                                    1
//                                }
//                            },
//                            StartDate = new DateTime(2019,1,4)
//                        },
//                        BasePrice = new Price()
//                        {
//                            Currency = new Currency
//                            {
//                                IsoCode = "usd",
//                                IsPostFix = false
//                            },
//                            Amount = 972.5M,
//                            DatePriceAndAvailabilty = null
//                        },
//                        IsSelected = true
//                    }
//                }
//            };

//            var clientInfo = new ClientInfo()
//            {
//                B2BAffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
//                Currency = new Currency()
//                {
//                    IsoCode = "USD"
//                },
//            };

//            var result = _activityPersistence.GetPriceAndAvailability(activity, clientInfo, false);
//             Assert.True(result != null);
//        }

//        [Fact]
//        public void CheckActivityTypeTest()
//        {
//            var result = _activityPersistence.GetActivityType(79);
//             Assert.True(result == 2);
//        }

//        [Fact]
//        public void CategoryServiceMappingTest()
//        {
//            var result = _activityPersistence.CategoryServiceMapping();
//            Assert.True(result?.Count > 0);
//        }

//        [Fact]
//        public void LoadMaxPaxDetailsTest()
//        {
//            var result = _activityPersistence.LoadMaxPaxDetails(79);
//             Assert.True(result != null);
//        }

//        /// <summary>
//        /// Test case to get pax price
//        /// </summary>
//        [Fact]
//        public void GetPaxPriceTest()
//        {
//            var paxPriceReq = new PaxPriceRequest()
//            {
//                AffiliateId = "91EFA4C9-8DF1-4632-BB86-801142350FC2",
//                CheckIn = Convert.ToDateTime("16-Apr-19"),
//                CheckOut = Convert.ToDateTime("16-Apr-19"),
//                PaxDetail = "<?xml version='1.0' encoding='UTF-8' standalone='yes'?><PAXDETAIL><PAXTYPE PAXTYPEID='1' COUNT='1'/><PAXTYPE PAXTYPEID='2' COUNT='1'/></PAXDETAIL>",
//                ServiceId = 5151
//            };
//            var result = _activityPersistence.GetPaxPrices(paxPriceReq);
//            Assert.True(result?.Count > 0);
//        }

//        [Fact]
//        public void GetAllOptionsAvailabilityTest()
//        {
//            var activity = new Activity()
//            {
//                ID = 853,
//                ProductOptions = new List<ProductOption>()
//                {
//                    new ProductOption()
//                    {
//                        Id = 56750,
//                        TravelInfo = new TravelInfo()
//                        {
//                            NumberOfNights = 1,
//                            NoOfPassengers = new Dictionary<PassengerType, int>()
//                            {
//                                {
//                                    PassengerType.Adult,
//                                    1
//                                }
//                            },
//                            StartDate = new DateTime(2019,1,4)
//                        },
//                        BasePrice = new Price()
//                        {
//                            Currency = new Currency
//                            {
//                                IsoCode = "usd",
//                                IsPostFix = false
//                            },
//                            Amount = 972.5M,
//                            DatePriceAndAvailabilty = null
//                        },
//                        IsSelected = true
//                    }
//                }
//            };

//            var result = _activityPersistence.GetAllOptionsAvailability(activity, DateTime.Now.AddDays(10), DateTime.Now.AddDays(13));
//             Assert.True(result != null);
//        }

//        [Fact]
//        public void GetPassengerInfoDetailsTest()
//        {
//            var result = _activityPersistence.GetPassengerInfoDetails();
//             Assert.True(result != null);

//            var result1 = _activityPersistence.GetPassengerInfoDetails("853");
//            Assert.True(result != null);
//        }

//        #region Private Methods

//        /// <summary>
//        /// Method to prepare a mock client info input data
//        /// </summary>
//        /// <returns></returns>
//        private ClientInfo PrepareClientInfo(string countryIp, string b2BAffiliateId)
//        {
//            var clientInfo = new ClientInfo();
//            var currency = new Currency();

//            clientInfo.AffiliateDisplayName = "Isango";
//            clientInfo.AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183";
//            clientInfo.AffiliateName = "Isango";
//            clientInfo.AffiliatePartner = null;
//            clientInfo.B2BAffiliateId = string.IsNullOrEmpty(b2BAffiliateId) ? "5beef089-3e4e-4f0f-9fbf-99bf1f350183" : b2BAffiliateId;
//            clientInfo.CityCode = null;
//            clientInfo.CompanyAlias = "ien";
//            clientInfo.CountryIp = string.IsNullOrEmpty(countryIp) ? "GB" : countryIp;

//            clientInfo.DiscountCode = "";
//            clientInfo.DiscountCodePercentage = 0M;
//            clientInfo.FacebookAppId = "656660554485822";
//            clientInfo.FacebookAppSecret = "af34c66444b9c19d38bc4e55cf2d54cf";
//            clientInfo.GtmIdentifier = "GTM-PSQPTWZ";
//            clientInfo.IsB2BAffiliate = false;
//            clientInfo.IsB2BNetPriceAffiliate = false;
//            clientInfo.IsSupplementOffer = true;
//            clientInfo.LanguageCode = "en";
//            clientInfo.LineOfBusiness = "TOURS & ACTIVITIES - isango!";
//            clientInfo.PaymentMethodType = PaymentMethodType.Transaction;
//            clientInfo.WidgetDate = DateTime.Now; // This date value is not valid

//            currency.IsPostFix = false;
//            currency.IsoCode = "GBP";
//            currency.Name = "GBP";
//            currency.Symbol = "£";

//            clientInfo.Currency = currency;

//            return clientInfo;
//        }

//        /// <summary>
//        /// Method to prepare a mock input data
//        /// </summary>
//        /// <param name="searchType">type of Search criteria</param>
//        /// <returns></returns>
//        private SearchCriteria PrepareCriteria(string searchType)
//        {
//            var searchCriteria = new SearchCriteria();

//            const string sortBy = "";
//            const string regionFilterIds = "";
//            const string attractionFilterIds = "";
//            const string criteria = "";
//            var selectedDates = "";
//            const string isOffer = "";
//            const string isSmartPhone = "";

//            var id = searchType == "Region" ? "7129" : searchType == "Category" ? "8188" : string.Empty;
//            var catId = searchType == "Category" ? "2" : string.Empty;
//            var keyword = searchType == "Keyword" ? "Par" : string.Empty;

//            const int pageNumber = 1;
//            const int productsToShow = 15;

//            if (!string.IsNullOrEmpty(id))
//            {
//                searchCriteria.RegionId = int.Parse(id);
//            }
//            if (!string.IsNullOrEmpty(catId))
//            {
//                searchCriteria.CategoryId = int.Parse(catId);
//            }

//            if (!string.IsNullOrEmpty(keyword))
//            {
//                searchCriteria.Keyword = keyword;
//            }

//            if (!string.IsNullOrEmpty(criteria))
//            {
//                var data = criteria.Split('|');
//                if (!string.IsNullOrEmpty(data[0]))
//                {
//                    searchCriteria.RegionId = int.Parse(data[0]);
//                }
//                if (!string.IsNullOrEmpty(data[1]))
//                {
//                    searchCriteria.CategoryId = int.Parse(data[1]);
//                }
//                if (!string.IsNullOrEmpty(data[2]))
//                {
//                    searchCriteria.Keyword = data[2];
//                }
//            }

//            searchCriteria.PageNumber = pageNumber;
//            searchCriteria.PageSize = productsToShow;
//            searchCriteria.RegionFilterIds = regionFilterIds;
//            searchCriteria.AttractionFilterIds = attractionFilterIds;
//            searchCriteria.IsOffer = isOffer.ToLowerInvariant() == "true";
//            searchCriteria.IsSmartphoneFilter = isSmartPhone.ToLowerInvariant() == "true";

//            if (!string.IsNullOrEmpty(sortBy))
//            {
//                switch (sortBy.ToLower(CultureInfo.InvariantCulture))
//                {
//                    case "price":
//                        searchCriteria.SortType = ProductSortType.Price;
//                        break;

//                    case "userrating":
//                        searchCriteria.SortType = ProductSortType.UserReviewRating;
//                        break;

//                    case "offers":
//                        searchCriteria.SortType = ProductSortType.Offers;
//                        break;

//                    default:
//                        searchCriteria.SortType = ProductSortType.Default;
//                        break;
//                }
//            }
//            else
//            {
//                searchCriteria.SortType = ProductSortType.Default;
//            }

//            if (string.IsNullOrWhiteSpace(selectedDates))
//            {
//                selectedDates = "";
//            }

//            searchCriteria.SelectedDates = selectedDates;
//            searchCriteria.ProductType = ProductType.Activity;

//            return searchCriteria;
//        }

//        #endregion Private Methods
    
//    }
//}
