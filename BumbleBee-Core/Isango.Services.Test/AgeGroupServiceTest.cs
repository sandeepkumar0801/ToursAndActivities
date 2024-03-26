using Autofac;

using Isango.Register;
using Isango.Service.Contract;

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Util;

namespace Isango.Services.Test
{
    [TestFixture]
    internal class AgeGroupServiceTest : BaseTest
    {
        private IAgeGroupService _ageGroupService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            var builder = new ContainerBuilder(); // Create a new ContainerBuilder
            builder.RegisterModule<StartupModule>();
            var container = builder.Build(); // Build the container
            //var container = Startup._builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                _ageGroupService = scope.Resolve<IAgeGroupService>();
            }
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveAotAgeGroupsTest()
        {
            _ageGroupService.SaveAOTAgeGroups("AgeGroup_AOT");
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveGrayLineIceLandAgeGroupsTest()
        {
            _ageGroupService.SaveGrayLineIceLandAgeGroups("AgeGroup_GrayLineIceLand").GetAwaiter();
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveGrayLineIceLandPickupLocationsTest()
        {
            _ageGroupService.SaveGrayLineIceLandPickupLocations("AgeGroup_GrayLineIceLand").Wait();
        }

        [Test]
        [Ignore("Ignore as it sync data between databases")]
        public void SyncGrayLineIceLandDataTest()
        {
            _ageGroupService.SyncGrayLineIceLandData();
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveFareHarborCompaniesTest()
        {
            _ageGroupService.SaveFareHarborCompanies("AgeGroup_FareHarbor");
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveFareHarborCustomerProtoTypesTest()
        {
            _ageGroupService.SaveFareHarborCustomerProtoTypes("AgeGroup_FareHarbor");
        }

        [Test]
        [Ignore("Ignore as it sync data between databases")]
        public void SyncFareHarborDataTest()
        {
            _ageGroupService.SyncFareHarborData();
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SavePrioTicketDetailsTest()
        {
            _ageGroupService.SavePrioTicketDetails("AgeGroup_Prio");
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveTiqetsVariantsTest()
        {
            _ageGroupService.SaveTiqetsVariants("AgeGroup_Tiqets");
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveGoldenToursAgeGroupsTest()
        {
            _ageGroupService.SaveGoldenToursAgeGroups("AgeGroup_GoldenTours");
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveApiTudeContentTest()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            _ageGroupService.SaveAPITudeContentData("ApiTude_CalendarCon");
            stopWatch.Stop();
            long duration = stopWatch.ElapsedMilliseconds;
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveRedeamDataTest()
        {
            _ageGroupService.SaveRedeamData("AgeGroup_Redeam");
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveBokunAgeGroupsTest()
        {
            _ageGroupService.SaveBokunAgeGroups($"AgeGroup_Bokun_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        [Test]
        [Ignore("Ignored as it saves data in the database")]
        public void SaveRezdyAgeGroupTest()
        {
            _ageGroupService.SaveRezdyDataInDB("AgeGroup_Rezdy");
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveVentrataProductsTest()
        {
            _ageGroupService.SaveVentrataProductDetails($"AgeGroup_Ventrata_{DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss")}");
        }

        [Test]
        [Ignore("Ignored")]
        public void SaveGlobalTixCountryCityList()
        {
            _ageGroupService.SaveGlobalTixCountryCityList(Guid.NewGuid().ToString());
        }

        [Test]
        [Ignore("Ignored")]
        public void SaveGlobalTixActivities()
        {
            _ageGroupService.SaveGlobalTixActivities(Guid.NewGuid().ToString());
        }

        [Test]
        [Ignore("Ignored")]
        public void SaveGlobalTixPackages()
        {
            _ageGroupService.SaveGlobalTixPackages(Guid.NewGuid().ToString());
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveTourCMSChannelDataTest()
        {
            _ageGroupService.SaveTourCMSChannelData("AgeGroup_TourCMSChannelData");
        }
        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveTourCMSTourDataTest()
        {
            _ageGroupService.SaveTourCMSTourData("AgeGroup_TourCMSTourData");
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveNewCitySightSeeingProductList()
        {
            _ageGroupService.SaveNewCitySightSeeingProductList(Guid.NewGuid().ToString());
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveGoCityProductList()
        {
            _ageGroupService.SaveGoCityProductList(Guid.NewGuid().ToString());
        }
        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SavePrioHubProductList()
        {
            _ageGroupService.SavePrioHubProductData("AgeGroup_PrioHubData");
        }

        [Test]
        [Ignore("Ignore as it saves data in the database")]
        public void SaveRaynaProductList()
        {
            _ageGroupService.SaveRaynaProductList(Guid.NewGuid().ToString());
        }

        // [Test]
        // [Ignore("Ignore as it saves data in the database")]
        // public void SaveCssExternalProduct()
        // {
        //     _ageGroupService.SaveExternalProducts("AgeGroup_CitySightSeeing");
        // }

        // [Test]
        // //[Ignore("Ignore as it saves data in the database")]
        // public void CreateBooking()
        // {
        //     Guid idempotentGuid = Guid.NewGuid(); // Generate a new UUID
        //     string idempotentKey = idempotentGuid.ToString();
        //     var createBookingRequest = new CreateBookingRequest
        //     {
        //         adult = 1,
        //         agent = "isango.com",
        //         barcode = "SGIO1VC73",
        //         booking = null,
        //         child = 0, // or set the appropriate value
        //         customer = new BookingCustomer
        //         {
        //             country = "GB",
        //             email = "vaishnavee.jaiswal@isango.com",
        //             full_name = "vgh test",
        //             language = "en",
        //             lastName = "test",
        //             mobile = "+34 666 111 222",
        //             name = "vgh"
        //         },
        //         date = "2023-09-11",
        //         family = 0, // or set the appropriate value
        //         infant = 0, // or set the appropriate value
        //         integration_booking_code = "SGIO1VC73",
        //         notes = null,
        //         option = 10970,
        //         product = 4057,
        //         reference = "SGIO1VC73",
        //         reservation = null,
        //         resident = 0, // or set the appropriate value
        //         senior = 0, // or set the appropriate value
        //         student = 0, // or set the appropriate value
        //         supplier_id = 1012,
        //         tickets = new List<Ticket>
        //         {
        //             new Ticket
        //             {
        //                 barcode = "SGIO1VC73",
        //                 reference = null,
        //                 type = null
        //             }
        //         },
        //         time = "11:00",
        //         youth = 0 // or set the appropriate value
        //     };
        //     var result = _ageGroupService.CreateBooking(idempotentKey, createBookingRequest);
        // }
        // [Test]
        // public void CancelBooking()
        // {
        //     Guid idempotentGuid = Guid.NewGuid(); // Generate a new UUID
        //     string idempotentKey = idempotentGuid.ToString();
        //     var cancelbooking = new CancellationRequest
        //     {
        //         agent= "isango.com",
        //         barcode= "SGIO1VC73",
        //         booking= null,
        //         supplier_id= 1012

        //     };
        //     var result = _ageGroupService.CancelBooking(idempotentKey, cancelbooking);

        // }

        [Test]
        public void RemoveRedisKeys()
        {
            RedixManagement.Initalize();
            RedixManagement.RemoveInactiveSessions();

        }

        // [Test]
        //// [Ignore("Ignore as it saves data in the database")]
        // public void CssWebJobs()
        // {
        //     _ageGroupService.csswebjob();

        // }
    }
}