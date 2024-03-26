using Autofac;
using Isango.Entities.TourCMSCriteria;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.TourCMS;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class TourCMSAdapterTest : BaseTest
    {
        private ITourCMSAdapter _tourAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
           // var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _tourAdapter = scope.Resolve<ITourCMSAdapter>();
            }
        }


        /// <summary>
        /// Get GetChannelData
        /// </summary>
        [Test]
        public void GetChannelDataTest()
        {
            _tourAdapter.GetChannelData("tourCMSChannelata",0);
            Assert.NotNull(0);
        }

        [Test]
        public void GetChannelShowDataTest()
        {
            _tourAdapter.GetChannelShowData("tourCMSChannelShow",3930);
            Assert.NotNull(0);
        }


        [Test]
        public void GetCalendarDatafromAPI()
        {
            var tourCMSCriteria = new TourCMSCriteria
            {
                ChannelId = 3930,
                TourId =80
            };
            _tourAdapter.GetCalendarDatafromAPI(tourCMSCriteria,"tourCMSChannelCalendar");
            Assert.NotNull(0);
        }

        [Test]
        public void GetCheckAvailablityfromAPI()
        {
            //24,27,71
            var tourCMSCriteria = new TourCMSCriteria
            {
                ChannelId = 3930,
                TourId = 83
            };
            _tourAdapter.GetAvailablityfromAPI(tourCMSCriteria, "tourCMSAvailability");
            Assert.NotNull(0);
        }
    }
}