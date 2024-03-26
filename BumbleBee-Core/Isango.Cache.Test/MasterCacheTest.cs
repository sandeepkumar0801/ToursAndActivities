using CacheManager;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.GoldenTours;
using Isango.Entities.Wrapper;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace Isango.Cache.Test
{
    [TestFixture]
    public class MasterCacheTest : BaseTest
    {
        private IMasterCacheManager _masterCacheManagerMock;
        private MasterCacheManager _masterCacheManager;

        private CollectionDataFactory<Currency> _collectionDataFactory;
        private CollectionDataFactory<CacheKey<LatLongVsurlMapping>> _collectionDataFactoryForLatLongVsurlMapping;
        private CollectionDataFactory<CacheKey<AutoSuggest>> _collectionDataFactoryForAutoSuggest;
        private CollectionDataFactory<CacheKey<BlogData>> _collectionDataFactoryForBlogData;
        private CollectionDataFactory<CacheKey<CurrencyExchangeRates>> _collectionDataFactoryCurrencyExchangeRates;
        private CollectionDataFactory<CacheKey<TicketByRegion>> _collectionDataFactoryTickets;
        private CollectionDataFactory<CacheKey<AutoSuggest>> _collectionDataFactoryAutoSuggest;
        private CollectionDataFactory<CacheKey<HotelBedsCredentials>> _collectionDataFactoryForHotelBedsCredentials;
        private CollectionDataFactory<CacheKey<PassengerMapping>> _collectionDataFactoryForPassengerMapping;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _masterCacheManagerMock = Substitute.For<IMasterCacheManager>();
            _collectionDataFactory = Substitute.For<CollectionDataFactory<Currency>>();
            _collectionDataFactoryForLatLongVsurlMapping = Substitute.For<CollectionDataFactory<CacheKey<LatLongVsurlMapping>>>();
            _collectionDataFactoryForAutoSuggest = Substitute.For<CollectionDataFactory<CacheKey<AutoSuggest>>>();
            _collectionDataFactoryForBlogData = Substitute.For<CollectionDataFactory<CacheKey<BlogData>>>();
            _collectionDataFactoryCurrencyExchangeRates = Substitute.For<CollectionDataFactory<CacheKey<CurrencyExchangeRates>>>();
            _collectionDataFactoryTickets = Substitute.For<CollectionDataFactory<CacheKey<TicketByRegion>>>();
            _collectionDataFactoryAutoSuggest = Substitute.For<CollectionDataFactory<CacheKey<AutoSuggest>>>();
            _collectionDataFactoryForHotelBedsCredentials = Substitute.For<CollectionDataFactory<CacheKey<HotelBedsCredentials>>>();
            _collectionDataFactoryForPassengerMapping = Substitute.For<CollectionDataFactory<CacheKey<PassengerMapping>>>();

            _masterCacheManager = new MasterCacheManager(_collectionDataFactory, _collectionDataFactoryForLatLongVsurlMapping, _collectionDataFactoryForAutoSuggest, _collectionDataFactoryForBlogData,
                _collectionDataFactoryCurrencyExchangeRates, _collectionDataFactoryTickets, _collectionDataFactoryAutoSuggest, _collectionDataFactoryForHotelBedsCredentials, _collectionDataFactoryForPassengerMapping);
        }

        [Test]
        public void GetCurrencies()
        {
            var currencyList = new List<Currency>()
            {
                new Currency()
                {
                    IsoCode="INR"
                },
                new Currency()
                {
                    IsoCode = "AUD"
                }
            };
            _collectionDataFactory.GetCollectionDataHelper().GetResultList("MasterDataCollection", string.Empty).ReturnsForAnyArgs(currencyList);
            var result = _masterCacheManager.GetCurrencies("5beef089-3e4e-4f0f-9fbf-99bf1f350183");
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetRegionTest()
        {
            var cacheObject = CreateLatLongObj();

            _collectionDataFactoryForLatLongVsurlMapping.GetCollectionDataHelper().GetResult("MasterDataCollection", string.Empty).ReturnsForAnyArgs(cacheObject);
            var result = _masterCacheManager.GetRegionData("GeoCoordinateMasterMapping");
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetMasterAutoSuggestDataTest()
        {
            var list = new List<AutoSuggest>()
            {
                new AutoSuggest()
                {
                    ParentId = "10",
                    Category = "test"
                },
                new AutoSuggest()
                {
                    ParentId = "11",
                    Category = "test"
                }
            };

            var cacheObject = new CacheKey<AutoSuggest>()
            {
                Id = "MasterAutoSuggestData",
                CacheValue = list
            };

            _collectionDataFactoryForAutoSuggest.GetCollectionDataHelper().GetResult("MasterDataCollection", string.Empty).ReturnsForAnyArgs(cacheObject);
            var result = _masterCacheManager.GetMasterAutoSuggestData("MasterAutoSuggestData");
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetBlogDataTest()
        {
            var cacheObject = CreateBlogDataObj();

            _collectionDataFactoryForBlogData.GetCollectionDataHelper().GetResult("MasterDataCollection", string.Empty).ReturnsForAnyArgs(cacheObject);
            var result = _masterCacheManager.GetBlogData("BlogDataForIsango");
            Assert.IsTrue(result?.CacheValue.Count > 0);
        }

        [Test]
        public void SetFilteredTicketsToCacheTest()
        {
            var cacheKeyObj = new CacheKey<TicketByRegion>
            {
                Id = "FilteredTickets",
                CacheValue = new List<TicketByRegion>
                {
                    new TicketByRegion { CountryCode = "AU", ThemeparkTicket =
                        new ThemeparkTicket { City = 7229, Country = 7166, Region = 7245, ProductId = 21233 } }
                }
            };

            _collectionDataFactoryTickets.GetCollectionDataHelper().InsertDocument("MasterDataCollection", cacheKeyObj).ReturnsForAnyArgs(true);
            var result = _masterCacheManager.SetFilteredTicketsToCache(cacheKeyObj);
            Assert.IsTrue(result);
        }

        [Test]
        public void GetFilteredTicketsTest()
        {
            var list = new List<TicketByRegion>()
            {
                new TicketByRegion()
                {
                  CountryCode = "IND",
                  ThemeparkTicket = new ThemeparkTicket()
                  {
                      ProductId = 10
                  }
                },
                new TicketByRegion()
                {
                  CountryCode = "IND",
                  ThemeparkTicket = new ThemeparkTicket()
                  {
                      ProductId = 11
                  }
                }
            };

            var cacheObject = new CacheKey<TicketByRegion>()
            {
                Id = "FilteredTickets",
                CacheValue = list
            };

            _collectionDataFactoryTickets.GetCollectionDataHelper().GetResult("MasterDataCollection", string.Empty).ReturnsForAnyArgs(cacheObject);
            var result = _masterCacheManager.GetFilteredTickets("FilteredTickets");
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetAutoSuggestDataTest()
        {
            var list = new List<AutoSuggest>()
            {
                new AutoSuggest()
                {
                    ParentId = "10",
                    Category = "test"
                },
                new AutoSuggest()
                {
                    ParentId = "11",
                    Category = "test"
                }
            };

            var cacheObject = new CacheKey<AutoSuggest>()
            {
                Id = "MasterAutoSuggestData",
                CacheValue = list
            };

            _collectionDataFactoryAutoSuggest.GetCollectionDataHelper().GetResult("MasterDataCollection", string.Empty).ReturnsForAnyArgs(cacheObject);
            var result = _masterCacheManager.GetAutoSuggestData("5BEEF089-3E4E-4F0F-9FBF-99BF1F350183");
            Assert.IsNotNull(result);
        }

        [Test]
        public void SetAutoSuggestData()
        {
            var cacheObject = CreateAutoSuggestObj();

            _collectionDataFactoryAutoSuggest.GetCollectionDataHelper().InsertDocument("MasterDataCollection", cacheObject).ReturnsForAnyArgs(true);
            var result = _masterCacheManager.SetAutoSuggestData(cacheObject);
            Assert.IsTrue(result);
        }

        [Test]
        public void InsertCurrencyInCacheTest()
        {
            var currancy = new Currency
            {
                IsoCode = "inr",
                Name = "indian rupee"
            };

            _collectionDataFactory.GetCollectionDataHelper().InsertDocument("MasterDataCollection", currancy).ReturnsForAnyArgs(true);
            var result = _masterCacheManager.InsertCurrencyInCache(currancy);
            Assert.IsTrue(result);
        }

        [Test]
        public void SetRegionDataTest()
        {
            var cacheObj = CreateLatLongObj();

            _collectionDataFactoryForLatLongVsurlMapping.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "latLongVSURLMapping").ReturnsForAnyArgs(true);
            _collectionDataFactoryForLatLongVsurlMapping.GetCollectionDataHelper().UpdateDocument("MasterDataCollection", cacheObj).ReturnsForAnyArgs(true);
            var result = _masterCacheManager.SetRegionData(cacheObj);
            Assert.IsTrue(result);

            _collectionDataFactoryForLatLongVsurlMapping.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "latLongVSURLMapping").ReturnsForAnyArgs(false);
            _collectionDataFactoryForLatLongVsurlMapping.GetCollectionDataHelper().InsertDocument("MasterDataCollection", cacheObj).ReturnsForAnyArgs(true);
            var result1 = _masterCacheManager.SetRegionData(cacheObj);
            Assert.IsTrue(result1);
        }

        [Test]
        public void SetMasterAutoSuggestDataTest()
        {
            var cacheObj = CreateAutoSuggestObj();

            _collectionDataFactoryForAutoSuggest.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "autoSuggestList").ReturnsForAnyArgs(true);
            _collectionDataFactoryForAutoSuggest.GetCollectionDataHelper().UpdateDocument("MasterDataCollection", cacheObj).ReturnsForAnyArgs(true);
            var result = _masterCacheManager.SetMasterAutoSuggestData(cacheObj);
            Assert.IsTrue(result);

            _collectionDataFactoryForAutoSuggest.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "autoSuggestList").ReturnsForAnyArgs(false);
            _collectionDataFactoryForAutoSuggest.GetCollectionDataHelper().InsertDocument("MasterDataCollection", cacheObj).ReturnsForAnyArgs(true);
            var result1 = _masterCacheManager.SetMasterAutoSuggestData(cacheObj);
            Assert.IsTrue(result1);
        }

        [Test]
        public void SetBlogDataTest()
        {
            var cacheObj = CreateBlogDataObj();

            _collectionDataFactoryForBlogData.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "blogData").ReturnsForAnyArgs(true);
            _collectionDataFactoryForBlogData.GetCollectionDataHelper().UpdateDocument("MasterDataCollection", cacheObj).ReturnsForAnyArgs(true);
            var result = _masterCacheManager.SetBlogData(cacheObj);
            Assert.IsTrue(result);

            _collectionDataFactoryForBlogData.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "blogData").ReturnsForAnyArgs(false);
            _collectionDataFactoryForBlogData.GetCollectionDataHelper().InsertDocument("MasterDataCollection", cacheObj).ReturnsForAnyArgs(true);
            var result1 = _masterCacheManager.SetBlogData(cacheObj);
            Assert.IsTrue(result1);
        }

        [Test]
        public void LoadCurrencyExchangeRateTest()
        {
            var cacheObj = CreateCurrencyExchangeObj();

            _collectionDataFactoryCurrencyExchangeRates.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "blogData").ReturnsForAnyArgs(true);
            _collectionDataFactoryCurrencyExchangeRates.GetCollectionDataHelper().UpdateDocument("MasterDataCollection", cacheObj).ReturnsForAnyArgs(true);
            var result = _masterCacheManager.LoadCurrencyExchangeRate(cacheObj);
            Assert.IsTrue(result);

            _collectionDataFactoryCurrencyExchangeRates.GetCollectionDataHelper().CheckIfDocumentExist("MasterDataCollection", "blogData").ReturnsForAnyArgs(false);
            _collectionDataFactoryCurrencyExchangeRates.GetCollectionDataHelper().InsertDocument("MasterDataCollection", cacheObj).ReturnsForAnyArgs(true);
            var result1 = _masterCacheManager.LoadCurrencyExchangeRate(cacheObj);
            Assert.IsTrue(result1);
        }

        [Test]
        public void GetCurrencyExchangeRateTest()
        {
            var cacheObj = CreateCurrencyExchangeObj();
            _collectionDataFactoryCurrencyExchangeRates.GetCollectionDataHelper().GetResult("MasterDataCollection", string.Empty).ReturnsForAnyArgs(cacheObj);
            var result = _masterCacheManager.GetCurrencyExchangeRate();
            Assert.IsNotNull(result);
        }

        [Test]
        public void DeleteDocumentStringTest()
        {
            _collectionDataFactory.GetCollectionDataHelper().DeleteDocument("MasterDataCollection", string.Empty, string.Empty).ReturnsForAnyArgs(true);
            var result = _masterCacheManager.DeleteDocument("MasterDataCollection", string.Empty, string.Empty);
            Assert.IsTrue(result.Result);
        }

        [Test]
        public void DeleteDocumentIntTest()
        {
            _collectionDataFactory.GetCollectionDataHelper().DeleteDocument("MasterDataCollection", string.Empty, 0).ReturnsForAnyArgs(true);
            var result = _masterCacheManager.DeleteDocument("MasterDataCollection", string.Empty, 0);
            Assert.IsTrue(result.Result);
        }

        [Test]
        public void GetAutoSuggestByAffiliateId()
        {
            var list = new List<AutoSuggest>()
            {
                new AutoSuggest()
                {
                    ParentId = "10",
                    Category = "test"
                },
                new AutoSuggest()
                {
                    ParentId = "11",
                    Category = "test"
                }
            };

            var cacheObject = new CacheKey<AutoSuggest>()
            {
                Id = "MasterAutoSuggestData",
                CacheValue = list
            };

            _collectionDataFactoryAutoSuggest.GetCollectionDataHelper().GetResult("MasterDataCollection", string.Empty).ReturnsForAnyArgs(cacheObject);
            _masterCacheManagerMock.GetAutoSuggestData("5BEEF089-3E4E-4F0F-9FBF-99BF1F350183").ReturnsForAnyArgs(cacheObject);
            var result = _masterCacheManager.GetAutoSuggestByAffiliateId("5BEEF089-3E4E-4F0F-9FBF-99BF1F350183");
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetHBAuthorizationDataTest()
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

            var cacheObject = new CacheKey<HotelBedsCredentials>()
            {
                Id = "HBauthData",
                CacheValue = hotelBedsCredentials
            };

            _collectionDataFactoryForHotelBedsCredentials.GetCollectionDataHelper().GetResult("test", "query").ReturnsForAnyArgs(cacheObject);
            var result = _masterCacheManager.GetHBAuthorizationData();
            Assert.AreEqual(result, cacheObject.CacheValue);
        }

        [Test]
        public void SetHBAuthorizationDataTest()
        {
            var hotelBedsCredentials = new List<HotelBedsCredentials>
            {
                new HotelBedsCredentials
                {
                    AffiliateId="test",
                    LanguageCode="ENG",
                    Authentication="username+password"
                }
            };

            var cacheObject = new CacheKey<HotelBedsCredentials>()
            {
                Id = "HBauthData",
                CacheValue = hotelBedsCredentials
            };

            _collectionDataFactoryForHotelBedsCredentials.GetCollectionDataHelper().CheckIfDocumentExist("collection", "query").ReturnsForAnyArgs(false);
            _collectionDataFactoryForHotelBedsCredentials.GetCollectionDataHelper().InsertDocument("collection", cacheObject).ReturnsForAnyArgs(true);
            var result = _masterCacheManager.SetHBAuthorizationData(cacheObject);
            Assert.IsTrue(result);
        }

        [Test]
        public void LoadPassengerMappingsTest()
        {
            var passengerMappings = new List<PassengerMapping>
            {
                new PassengerMapping
                {
                    PassengerTypeId = 1,
                    SupplierPassengerTypeId = 1
                }
            };

            var cacheObject = new CacheKey<PassengerMapping>()
            {
                Id = "GoldenToursPaxMapping",
                CacheValue = passengerMappings
            };

            _collectionDataFactoryForPassengerMapping.GetCollectionDataHelper().CheckIfDocumentExist("collection", "query").ReturnsForAnyArgs(false);
            _collectionDataFactoryForPassengerMapping.GetCollectionDataHelper().InsertDocument("collection", cacheObject).ReturnsForAnyArgs(true);
            var result = _masterCacheManager.LoadGoldenToursPassengerMappings(cacheObject);
            Assert.IsTrue(result);
        }

        [Test]
        public void GetPassengerMappingsTest()
        {
            var result = _masterCacheManager.GetGoldenToursPaxMappings();
            Assert.IsNotNull(result);
        }

        private CacheKey<LatLongVsurlMapping> CreateLatLongObj()
        {
            var latLongVsurlMappinglist = new List<LatLongVsurlMapping>()
            {
                new LatLongVsurlMapping()
                {
                    CityName = "test",
                    CountryName = "india"
                },
                new LatLongVsurlMapping()
                {
                    CityName = "test2",
                    CountryName = "india"
                }
            };

            var cacheObject = new CacheKey<LatLongVsurlMapping>()
            {
                Id = "GeoCoordinateMasterMapping",
                CacheValue = latLongVsurlMappinglist
            };

            return cacheObject;
        }

        private CacheKey<AutoSuggest> CreateAutoSuggestObj()
        {
            var list = new List<AutoSuggest>()
            {
                new AutoSuggest()
                {
                    ParentId = "10",
                    Category = "test"
                },
                new AutoSuggest()
                {
                    ParentId = "11",
                    Category = "test"
                }
            };

            var cacheObject = new CacheKey<AutoSuggest>()
            {
                Id = "MasterAutoSuggestData",
                CacheValue = list
            };

            return cacheObject;
        }

        private CacheKey<BlogData> CreateBlogDataObj()
        {
            var list = new List<BlogData>()
            {
                new BlogData()
                {
                    BlogId = 10,
                    Category = "test"
                },
                new BlogData()
                {
                    BlogId = 11,
                    Category = "test 2"
                }
            };

            var cacheObject = new CacheKey<BlogData>()
            {
                Id = "BlogDataForIsango",
                CacheValue = list
            };

            return cacheObject;
        }

        private CacheKey<CurrencyExchangeRates> CreateCurrencyExchangeObj()
        {
            var list = new List<CurrencyExchangeRates>()
            {
                new CurrencyExchangeRates()
                {
                    ExchangeRate = 10,
                    FromCurrencyCode = "inr",
                    ToCurrencyCode ="aud"
                },
                new CurrencyExchangeRates()
                {
                    ExchangeRate = 15,
                    FromCurrencyCode = "usd",
                    ToCurrencyCode ="aud"
                }
            };

            var cacheObject = new CacheKey<CurrencyExchangeRates>()
            {
                Id = "CurrencyExchangeRates",
                CacheValue = list
            };

            return cacheObject;
        }
    }
}