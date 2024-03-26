using Autofac;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.Enums;
using Isango.Entities.GrayLineIceLand;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.GrayLineIceLand;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class GrayLineIceLandAdapterTest : BaseTest
    {
        private IGrayLineIceLandAdapter _grayLineIceLandAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _grayLineIceLandAdapter = scope.Resolve<IGrayLineIceLandAdapter>();
            }
        }

        /// <summary>
        /// Test method to get availability and price
        /// </summary>
        [Test]
        public void GetAvailabilityAndPriceTest()
        {
            var criteria = PrepareCriteriaForGetAvailabilityAndPrice();
            var token = Guid.NewGuid();
            var result = _grayLineIceLandAdapter.GetAvailabilityAndPrice(criteria, token.ToString());
            Assert.NotNull(result);
            Assert.AreEqual("AH11", result.FirstOrDefault().Code);
        }

        /// <summary>
        /// Test case of async method to get availability and price
        /// </summary>
        [Test]
        public void GetAvailabilityAndPriceAsyncTest()
        {
            var criteria = PrepareCriteriaForGetAvailabilityAndPrice();
            var token = Guid.NewGuid();
            var result = _grayLineIceLandAdapter.GetAvailabilityAndPriceAsync(criteria, token.ToString());
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test case to create booking
        /// </summary>
        [Test]
        public void CreateBookingTest()
        {
            var criteria = PrepareCriteriaForCreateBooking();
            var bookingReference = "SGI622428";
            var token = Guid.NewGuid();
            var result = _grayLineIceLandAdapter.CreateBooking(criteria, bookingReference, token.ToString(), out _, out _);
            Assert.NotNull(result);
            Assert.AreEqual(BookingStatus.Confirmed, result.Status);
        }

        /// <summary>
        /// Test case to create async booking
        /// </summary>
        [Test]
        [TestCase(BookingStatus.Confirmed)]
        public void CreateBookingAsyncTest(BookingStatus expectedResult)
        {
            var token = Guid.NewGuid();
            var result = _grayLineIceLandAdapter.CreateBookingAsync(PrepareCriteriaForCreateBooking(), token.ToString()).GetAwaiter();
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test case to delete booking asynchronously
        /// </summary>
        [Test]
        public void DeleteBookingAsyncTest()
        {
            string[] refnos = { "2026684", "2026689", "2026694", "2026697", "2026701" };
            foreach (var refno in refnos)
            {
                var criteria = new Booking { ReferenceNumber = refno };
                var token = Guid.NewGuid();
                var result = _grayLineIceLandAdapter.DeleteBookingAsync(criteria, token.ToString());
                result.Wait();
                Assert.NotNull(result);
                Assert.AreEqual(true, result.Result);
            }
        }

        /// <summary>
        /// Test case to delete booking
        /// </summary>
        [Test]
        public void DeleteBookingTest()
        {
            var token = Guid.NewGuid();
            var result = _grayLineIceLandAdapter.DeleteBooking("1923052", token.ToString(), out string _, out string _);

            Assert.AreEqual(true, result);
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test case to get age groups by tours
        /// </summary>
        [Test]
        public void GetAgeGroupsByToursAsyncTest()
        {
            var token = Guid.NewGuid();
            var result = _grayLineIceLandAdapter.GetAgeGroupsByToursAsync(new List<IsangoHBProductMapping> { new IsangoHBProductMapping { HotelBedsActivityCode = "AH12" } }, token.ToString());
            result.Wait();
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test case to get all pickup location
        /// </summary>
        [Test]
        public void GetAllPickupLocationsAsyncTest()
        {
            var token = Guid.NewGuid();
            var result = _grayLineIceLandAdapter.GetAllPickupLocationsAsync(new List<IsangoHBProductMapping> { new IsangoHBProductMapping { HotelBedsActivityCode = "AH12" } }, token.ToString());
            result.Wait();
            Assert.NotNull(result);
        }

        #region Private Methods

        /// <summary>
        /// Preparing criteria for GetAvailabilityAndPrice
        /// </summary>
        /// <returns>Criteria</returns>
        private GrayLineIcelandCriteria PrepareCriteriaForGetAvailabilityAndPrice()
        {
            var criteria = new GrayLineIcelandCriteria
            {
                ActivityCode = "AH11",
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {   PassengerType.Adult,
                        1
                    },
                    {   PassengerType.Child,
                        0
                    }
                },
                CheckinDate = new DateTime(2019, 01, 01),
                CheckoutDate = new DateTime(2019, 01, 10),
                Language = "2",
                PaxAgeGroupIds = new Dictionary<PassengerType, int> { { PassengerType.Adult, 7 } },
                Ages = new Dictionary<PassengerType, int> { { PassengerType.Child, 1 } }
            };
            return criteria;
        }

        /// <summary>
        /// Prepare criteria for create booking
        /// </summary>
        /// <returns></returns>
        private List<GrayLineIceLandSelectedProduct> PrepareCriteriaForCreateBooking()
        {
            var productOption = new List<ProductOption>();
            var paxAgeGroupIds = new Dictionary<PassengerType, int> { { PassengerType.Adult, 7 } };
            var pickupData = new PickupPointDetails { PickupPointId = "209667" };

            var customer = new List<Customer>()
            {
                new Customer()
                {
                    Age = 25,
                    PassengerType =  PassengerType.Adult,
                    FirstName = "Bharti",
                    LastName = "Tijare",
                    IsLeadCustomer = true,
                    CustomerId = 0,
                    Email = "test@email.com"
                }
            };
            var travelInfo = new TravelInfo()
            {
                StartDate = new DateTime(2018, 11, 30),
                NumberOfNights = -1,
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Adult, 1 },
                    {PassengerType.Child, 1 },
                    {PassengerType.Youth, 1 }
                }
            };

            var adult = new AdultPricingUnit()
            {
                Price = 60.3500M
            };

            var pricingunit = new List<PricingUnit>
            {
                adult
            };

            var datePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
            {
                {
                    new DateTime(2018, 11, 30),
                    new DefaultPriceAndAvailability()
                    {
                        TourDepartureId = 1060773,
                        AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                        PricingUnits = pricingunit
                    }
                }
            };
            var activityOption = new ActivityOption()
            {
                SellPrice = new Price()
                {
                    Amount = 60.3500M,
                    DatePriceAndAvailabilty = datePriceAndAvailabilty
                },
                TravelInfo = travelInfo,
                Customers = customer,
                IsSelected = true
            };
            productOption.Add(activityOption);

            var selectedProduct = new List<GrayLineIceLandSelectedProduct>()
            {
                new GrayLineIceLandSelectedProduct()
                {
                    APIType = APIType.Graylineiceland,
                    ActivityType = ActivityType.FullDay,
                    AdultPriceId = 0,
                    BlocId = 0,
                    CategoryId = 0,
                    FactsheetId = 0,
                    SellPrice = 60.35,
                    ProductOptions = productOption,
                    PaxAgeGroupIds = paxAgeGroupIds,
                    PickupPointDetails = new[]{pickupData},
                    Code = "AH11",
                    Price = 60.3500M
                }
            };
            return selectedProduct;
        }

        #endregion Private Methods
    }
}