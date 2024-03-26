using Autofac;
using Isango.Register;
using Isango.Service.Contract;
using NUnit.Framework;

namespace Isango.Services.Test
{
    public class GoogleMapsServiceTest : BaseTest
    {
        private IGoogleMapsService _googleMapsService;

        [OneTimeSetUp]
        [Ignore("Ignore as it saves data in the database")]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _googleMapsService = scope.Resolve<IGoogleMapsService>();
            }
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void LoadMerchantFeeds()
        {
            var result = _googleMapsService.LoadMerchantFeeds();
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void LoadServiceAvailabilityFeeds()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var result = _googleMapsService.LoadServiceAvailabilityFeeds();
            watch.Stop();
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void ProcessInventoryRealTimeUpdate()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var result = _googleMapsService.InventoryRealTimeUpdate();
            watch.Stop();
            Assert.IsTrue(result);
        }
        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void OrderNotificationRealTimeUpdateTest()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var result = _googleMapsService.OrderNotificationRealTimeUpdate();
            watch.Stop();
            Assert.IsTrue(result);
        }
    }
}