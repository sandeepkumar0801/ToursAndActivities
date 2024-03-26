using Autofac;
using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Persistence.Contract;
using Isango.Register;
using Isango.Service.Contract;

using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Isango.Services.Test
{
    [TestFixture]
    public class ServiceAvailabilityServiceTest : BaseTest
    {
        private IServiceAvailabilityService _serviceAvailabilityService;
        private IServiceAvailabilityPersistence _serviceAvailabilityPersistence;
        private IMasterPersistence _masterPersistence;

        [OneTimeSetUp]
        public void TestInitialise()
        {
           // var container = Startup._builder.Build();
            using (var scope = _container.BeginLifetimeScope())
            {
                _serviceAvailabilityService = scope.Resolve<IServiceAvailabilityService>();
                _serviceAvailabilityPersistence = scope.Resolve<IServiceAvailabilityPersistence>();
                _masterPersistence = scope.Resolve<IMasterPersistence>();
            }
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void GetGrayLineIceLandAvailabilitiesTest()
        {
            var result = _serviceAvailabilityService.GetGrayLineIceLandAvailabilities();
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void GetMoulinRougeAvailabilitiesTest()
        {
            var result = _serviceAvailabilityService.GetMoulinRougeAvailabilities();
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void GetPrioAvailabilitiesTest()
        {
            var result = _serviceAvailabilityService.GetPrioAvailabilities();
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void GetFareHarborAvailabilitiesTest()
        {
            var result = _serviceAvailabilityService.GetFareHarborAvailabilities();
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void GetBokunAvailabilitiesTest()
        {
            var result = _serviceAvailabilityService.GetBokunAvailabilities();
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void GetAotAvailabilitiesTest()
        {
            var result = _serviceAvailabilityService.GetAotAvailabilities();
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void GetTiqetsAvailabilitiesTest()
        {
            var result = _serviceAvailabilityService.GetTiqetsAvailabilities();
            _serviceAvailabilityPersistence.SaveServiceAvailabilities(result.Item2);
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void GetGoldenToursAvailabilitiesTest()
        {
            var result = _serviceAvailabilityService.GetGoldenToursAvailabilities();
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void GetIsangoAvailabilitiesTest()
        {
            var result = _serviceAvailabilityService.GetIsangoAvailabilities();
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveApiTudeAvailabilitiesTest()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var products = _masterPersistence.LoadLiveHBOptions()?
                             .Where(x => x.ApiType.Equals(APIType.Hotelbeds)
                                          && !string.IsNullOrWhiteSpace(x.SupplierCode)
                                   )
                             .Distinct().OrderBy(x => x.FactSheetId).Take(50)
                             .ToList();

            foreach (var bacth in products.Batch(500))
            {
                try
                {
                    var productsBatch = bacth.ToList();
                    var result = _serviceAvailabilityService.SaveApiTudeAvailabilities(productsBatch);
                    _serviceAvailabilityPersistence.SaveServiceAvailabilities(result.Item2);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "DataDumpingHelper",
                        MethodName = "LoadHBApitudeData"
                    };
                }
            }

            Task.Run(() => _serviceAvailabilityService.SyncPriceAvailabilities());
            stopWatch.Stop();
            long duration = stopWatch.ElapsedMilliseconds;
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveRedeamAvailabilitiesTest()
        {
            _serviceAvailabilityService.SaveRedeamAvailabilities();
            _serviceAvailabilityService.SyncPriceAvailabilities();
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveGlobalTixAvailabilitiesTest()
        {
            var AvailabilityData = _serviceAvailabilityService.SaveGlobalTixAvailabilities();
            _serviceAvailabilityService.SyncPriceAvailabilities();
        }

        [Test]
        [Ignore("Ignore as it delete data from the database")]
        public void DeleteExistingAvailabilityDetailsTest()
        {
            _serviceAvailabilityService.DeleteExistingAvailabilityDetails();
        }

        [Test]
        [Ignore("Ignore as it sync data between the databases")]
        public void SyncPriceAvailabilitiesTest()
        {
            _serviceAvailabilityService.SyncPriceAvailabilities();
        }

        [Test]
        [Ignore("Ignore as it sync data between the databases")]
        public void SaveRezdyAvailabilitiesTest()
        {
            _serviceAvailabilityService.SaveRezdyAvailabilities();
            _serviceAvailabilityService.SyncPriceAvailabilities();
        }
    }
}