using Autofac;

using Isango.Register;
using Isango.Service.Contract;

using Logger.Contract;
using NSubstitute;

using NUnit.Framework;

using System;

namespace Isango.Services.Test
{
    [TestFixture]
    public class CacheLoaderServiceActualTest : BaseTest
    {
        private ICacheLoaderService _cacheLoaderService;
        private ILogger _log;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _log = Substitute.For<Logger.Logger>();
            //var container = Startup._builder.Build();
            var builder = new ContainerBuilder(); // Create a new ContainerBuilder

            // Register the StartupModule
            builder.RegisterModule<StartupModule>();

            // Register your test-specific dependencies here if needed

            var container = builder.Build(); // Build the container
            using (var scope = container.BeginLifetimeScope())
            {
                _cacheLoaderService = scope.Resolve<ICacheLoaderService>();
            }
        }

        [Test, Order(5)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadGliAgeGroupByActivityTest()
        {
            var result = _cacheLoaderService.LoadGliAgeGroupByActivityAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(6)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadPrioAgeGroupByActivityTest()
        {
            var result = _cacheLoaderService.LoadPrioAgeGroupByActivityAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(7)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadAotAgeGroupByActivityTest()
        {
            var result = _cacheLoaderService.LoadAotAgeGroupByActivityAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(4)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadFareHarborAgeGroupByActivityTest()
        {
            var result = _cacheLoaderService.LoadFareHarborAgeGroupByActivityAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(27)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadCacheMappingTest()
        {
            var result = _cacheLoaderService.LoadCacheMappingAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(9)]
        [Ignore("ignored because this method insert data into cache.")]
        public void RegionCategoryMappingTest()
        {
            var result = _cacheLoaderService.RegionCategoryMappingAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(11)]
        [Ignore("ignored because this method insert data into cache.")]
        public void RegionDestinationMappingTest()
        {
            var result = _cacheLoaderService.RegionDestinationMappingAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(12)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadMappedLanguageTest()
        {
            var result = _cacheLoaderService.LoadMappedLanguageAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(13)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadNetPriceMasterDataTest()
        {
            var result = _cacheLoaderService.LoadNetPriceMasterDataAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(14)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadRegionCategoryMappingProductsTest()
        {
            var result = _cacheLoaderService.LoadRegionCategoryMappingProductsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(3)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadAvailabilityCacheTest()
        {
            var result = _cacheLoaderService.LoadAvailabilityCacheAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(1)]
        [Ignore("ignored because this method insert data into cache.")]
        public void InsertAvailabilityCacheTest()
        {
            _cacheLoaderService.InsertOptionAvailability().GetAwaiter().GetResult();
        }

        [Test, Order(16)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadHotelCacheTest()
        {
            //var result = _cacheLoaderService.LoadHotelCacheAsync().GetAwaiter().GetResult();
            //Assert.IsTrue(result);
        }

        [Test, Order(17)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadCurrencyExchangeRatesTest()
        {
            var result = _cacheLoaderService.LoadCurrencyExchangeRatesAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(2)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadActivitiesCollectionTest()
        {
            var result = _cacheLoaderService.LoadActivitiesCollectionAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(20)]
        [Ignore("ignored because this method insert data into cache.")]
        public void SetRegionTest()
        {
            var result = _cacheLoaderService.SetRegionAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(21)]
        [Ignore("ignored because this method insert data into cache.")]
        public void SetAffiliateFiltersTest()
        {
            var result = _cacheLoaderService.SetAffiliateFiltersAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(22)]
        [Ignore("ignored because this method insert data into cache.")]
        public void GetCustomerPrototypeByActivityTest()
        {
            var result = _cacheLoaderService.GetCustomerPrototypeByActivityAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(23)]
        [Ignore("ignored because this method insert data into cache.")]
        public void GetAllFareHarborUserKeysTest()
        {
            var result = _cacheLoaderService.GetAllFareHarborUserKeysAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(24)]
        [Ignore("ignored because this method insert data into cache.")]
        public void SetUrlPageIdMappingMappingTest()
        {
            var result = _cacheLoaderService.SetUrlPageIdMappingMappingAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(25)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadAffiliateDataByDomainAsyncTest()
        {
            var result = _cacheLoaderService.LoadAffiliateDataByDomainAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(26)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadPricingRules()
        {
            var result = _cacheLoaderService.LoadPricingRulesAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(31)]
        [Ignore("ignored because this method insert data into cache.")]
        public void SetAffiliateFilterTest()
        {
            var result = _cacheLoaderService.SetAffiliateFilterAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("ignored because this method insert data into cache.")]
        public void GetAllActivities()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var result = _cacheLoaderService.GetAllActivities();
            watch.Stop();
            LogElapsedTimeInTableStorage(watch.Elapsed, "GetAllActivities");
            Assert.NotNull(result.Result);
        }

        [Test]
        [Ignore("ignored because this method insert data into cache.")]
        public void GetSingleActivities()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var result = _cacheLoaderService.GetSingleActivity("2060");
            watch.Stop();
            LogElapsedTimeInTableStorage(watch.Elapsed, "GetSingleActivity");
            Assert.NotNull(result.Result);
        }

        [Test]
        [Ignore("ignored because this method insert data into cache.")]
        public void GetPriceAndAvailability()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var result = _cacheLoaderService.GetPriceAndAvailability();
            watch.Stop();
            LogElapsedTimeInTableStorage(watch.Elapsed, "GetPriceAndAvailability");
            Assert.NotNull(result);
        }

        [Test, Order(8)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadCalendarAvailabilityTest()
        {
            var result = _cacheLoaderService.LoadCalendarAvailability().GetAwaiter().GetResult();
            Assert.IsEmpty(result);
        }

        [Test, Order(28)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadHBAuthorizationDataTest()
        {
            var result = _cacheLoaderService.LoadHBAuthorizationDataAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(29)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadPickupLocationsDataTest()
        {
            var result = _cacheLoaderService.LoadPickupLocationsDataAsync().GetAwaiter().GetResult();
            Assert.IsEmpty(result);
        }

        [Test, Order(30)]
        [Ignore("ignored because this method insert data into cache.")]
        public void SetFilteredTicketAsyncTest()
        {
            var result = _cacheLoaderService.SetFilteredTicketAsync().Result;
            Assert.IsTrue(result);
        }

        [Test, Order(31)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadTiqetsPaxMappingsAsyncTest()
        {
            var result = _cacheLoaderService.LoadTiqetsPaxMappingsAsync().Result;
            Assert.IsTrue(result);
        }

        [Test, Order(31)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadGoldenToursPaxMappingsAsyncTest()
        {
            var result = _cacheLoaderService.LoadGoldenToursPaxMappingsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(31)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadRezdyLabelDetailsTest()
        {
            var result = _cacheLoaderService.LoadRezdyLabelDetailsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(31)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadRezdyPaxMappingTest()
        {
            var result = _cacheLoaderService.LoadRezdyPaxMappingsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }
        [Test]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadVentrataPaxMappingTest()
        {
            var result = _cacheLoaderService.LoadVentrataPaxMappingsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("ignored because this method insert data into cache.")]
        [Description("Do not run this test case while loading collections")]
        public void LoadSelectedActivitiesAsyncTest()
        {
            var result = _cacheLoaderService.LoadSelectedActivitiesAsync("29136")?.Result;
            Assert.IsTrue(result);
        }

        [Test, Order(32)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadTourCMSPaxMappingTest()
        {
            var result = _cacheLoaderService.LoadTourCMSPaxMappingsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(33)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadElasticSearchDestinationsTest()
        {
            var result = _cacheLoaderService.LoadElasticSearchDestinationsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }
        [Test, Order(34)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadElasticSearchProductsTest()
        {
            var result = _cacheLoaderService.LoadElasticSearchProductsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(35)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadElasticSearchAttractionsTest()
        {
            var result = _cacheLoaderService.LoadElasticSearchAttractionsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(36)]
        [Ignore("ignored because this method insert data into cache.")]
        public void LoadElasticAffiliateTest()
        {
            var result = _cacheLoaderService.LoadElasticAffiliateAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test, Order(37)]
        [Ignore("ignored because this method insert data into cache.")]
        public void ClearMongoSessions()
        {
            var result = _cacheLoaderService.ClearMongoSessions().GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        private void LogElapsedTimeInTableStorage(TimeSpan elapsedTime, string method)
        {
            _log.WriteTimer(method, string.Empty, string.Empty, elapsedTime.ToString());
        }
    }
}