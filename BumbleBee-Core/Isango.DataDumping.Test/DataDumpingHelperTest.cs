using Autofac;
using DataDumping.HangFire.Contracts;
using Isango.Register;
using NUnit.Framework;
using System.Diagnostics;

namespace Isango.DataDumping.Test
{
    [TestFixture]
    public class DataDumpingHelperTest : BaseTest
    {
        private IDataDumpingHelper _dataDumpingHelper;

        public DataDumpingHelperTest()
        {
            _dataDumpingHelper = _container.Resolve<IDataDumpingHelper>();
        }


        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadGrayLineIceLandDataTest()
        {
            _dataDumpingHelper.LoadGrayLineIceLandData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadMoulinRougeDataTest()
        {
            _dataDumpingHelper.LoadMoulinRougeData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadPrioDataTest()
        {
            _dataDumpingHelper.LoadPrioData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadFareHarborDataTest()
        {
            _dataDumpingHelper.LoadFareHarborData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadBokunDataTest()
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            _dataDumpingHelper.LoadBokunData();
            stopWatch.Stop();

            long duration = stopWatch.ElapsedMilliseconds;
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadAotDataTest()
        {
            _dataDumpingHelper.LoadAotData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadTiqetsDataTest()
        {
            _dataDumpingHelper.LoadTiqetsData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadGoldenToursDataTest()
        {
            _dataDumpingHelper.LoadGoldenToursData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadHBApiTudeData()
        {
            _dataDumpingHelper.LoadHBApitudeData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadGlobalTixData()
        {
            _dataDumpingHelper.LoadGlobalTixData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadVentrataData()
        {
            _dataDumpingHelper.LoadVentrataData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadRezdyData()
        {
            _dataDumpingHelper.LoadRezdyData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadRedeamData()
        {
            _dataDumpingHelper.LoadRedeamData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadRedeamV12Data()
        {
            _dataDumpingHelper.LoadRedeamV12Data();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadIsangoDataTest()
        {
            _dataDumpingHelper.LoadIsangoData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadAPIImagesTest()
        {
            _dataDumpingHelper.LoadAPIImages();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadTourCMSDataTest()
        {
            _dataDumpingHelper.LoadTourCMSData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadNewCitySightSeeingData()
        {
            _dataDumpingHelper.LoadNewCitySightSeeingData();
        }
        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadPrioHubDataTest()
        {
            _dataDumpingHelper.LoadPrioHubData();
        }

        [Test]
        [Ignore("Ignore this test case as it has database and insert in storage calls")]
        public void LoadRaynaData()
        {
            _dataDumpingHelper.LoadRaynaData();
        }
    }
}