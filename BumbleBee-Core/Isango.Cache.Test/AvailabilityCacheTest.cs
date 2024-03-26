using Autofac;
using CacheManager.Contract;
using Isango.Register;
using NUnit.Framework;

namespace Isango.Cache.Test
{
    [TestFixture]
    public class AvailabilityCacheTest : BaseTest
    {
        private IAvailabilityCacheManager _availabilityCacheManager;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _availabilityCacheManager = scope.Resolve<IAvailabilityCacheManager>();
            }
        }

        [Test]
        public void GetRegionAvailabilitiesTest()
        {
            var listAvailabilities = _availabilityCacheManager.GetRegionAvailabilities("148");
            Assert.IsTrue(listAvailabilities?.Count > 0);
        }

        [Test]
        public void LoadAvailabilityCacheTest()
        {
            var listAvailabilities = _availabilityCacheManager.LoadAvailabilityCache("7129", "7229");
            Assert.IsTrue(listAvailabilities?.Count > 0);
        }
    }
}
