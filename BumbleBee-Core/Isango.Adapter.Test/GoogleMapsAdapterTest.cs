using System.Collections.Generic;
using Autofac;
using Isango.Entities.GoogleMaps;
using Isango.Entities.GoogleMaps.BookingServer;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.GoogleMaps;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class GoogleMapsAdapterTest : BaseTest
    {
        private IGoogleMapsAdapter _googleMapsAdapter;
        private IGoogleMapsAdapter _googleMapsAdapterMock;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();
            using (var scope = _container.BeginLifetimeScope())
            {
                _googleMapsAdapter = scope.Resolve<IGoogleMapsAdapter>();
            }
            _googleMapsAdapterMock = Substitute.For<IGoogleMapsAdapter>();
        }

        [Test]
        public void ProcessInventoryRealTimeUpdateTest()
        {
            var merchantActivitiesDto = new List<MerchantActivitiesDto>
            {
                new MerchantActivitiesDto()
            };
            _googleMapsAdapterMock.ProcessInventoryRealTimeUpdate(merchantActivitiesDto).ReturnsForAnyArgs(true);
            var response = _googleMapsAdapterMock.ProcessInventoryRealTimeUpdate(merchantActivitiesDto);
            Assert.IsTrue(response);
        }

        [Test]
        public void ProcessOrderNotificationTest()
        {
            var orders = new List<Order>
            {
                new Order()
            };
            _googleMapsAdapterMock.ProcessOrderNotification(orders).ReturnsForAnyArgs(orders);
            var response = _googleMapsAdapterMock.ProcessOrderNotification(orders);
            Assert.IsNotNull(response);
        }
    }
}