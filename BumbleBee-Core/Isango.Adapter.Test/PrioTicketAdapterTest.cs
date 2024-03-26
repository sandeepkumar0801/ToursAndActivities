using Autofac;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Prio;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.PrioTicket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class PrioTicketAdapterTest : BaseTest
    {
        private IPrioTicketAdapter _prioTicketAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _prioTicketAdapter = scope.Resolve<IPrioTicketAdapter>();
            }
        }

        /// <summary>
        /// Test case to update option for PrioTicket activity
        /// </summary>
        [Test]
        public void UpdateOptionForPrioActivityTest()
        {
            var token = Guid.NewGuid();
            List<Activity> result = null;
            var activity = PrepareActivityData();
            var criteria = PrepareCriteriaForUpdateOptionForPrioActivity();
            if (activity?.ProductOptions != null)
            {
                foreach (var option in activity?.ProductOptions)
                {
                    criteria.SupplierOptionCodes.Add(option.SupplierOptionCode);
                }

                var isSingleOption = activity.ProductOptions.Count.Equals(1);
                result = _prioTicketAdapter.UpdateOptionforPrioActivity(criteria, token.ToString());
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(activity.ProductOptions.Select(x => x.SupplierOptionCode), result.FirstOrDefault().ProductOptions?.Select(x => x.SupplierOptionCode));
            Assert.Greater(result.Count, 0);

            //Second Scenario
            result = _prioTicketAdapter.UpdateOptionforPrioActivity(new PrioCriteria(), token.ToString());
            Assert.Null(result);

            //Third Scenario
            result = _prioTicketAdapter.UpdateOptionforPrioActivity(criteria, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Test case to create booking
        /// </summary>
        [Test]
        [Ignore("Ignoring this test case as it was hitting production API")]
        public void CreateBookingTest()
        {
            var activity = PrepareCriteriaForCreateBooking();
            activity.PrioApiConfirmedBooking = new PrioApi()
            {
                //data is dummy
                DistributorReference = "SGI622433",
                BookingReference = "SGI622433"
            };

            //First Scenario
            var token = Guid.NewGuid();
            var result = _prioTicketAdapter.CreateBooking(activity, token.ToString(), out _, out _);
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test case to cancel reservation
        /// </summary>
        [Test]
        [Ignore("Ignore this test it is broken")]
        public void CancelReservationTest()
        {
            var criteria = new PrioSelectedProduct();
            var token = "39edc60d-f3e7-45c2-979b-53c95dd38c58";
            criteria.PrioReservationReference = "155006155140836";

            var result = _prioTicketAdapter.CancelReservation(criteria, "352138d3-5a19-4c96-b3a7-6101cd6caf92", token, out string _, out string _);
            Assert.IsNull(result);
        }

        /// <summary>
        /// Test case to cancel booking
        /// </summary>
        [Test]
        [Ignore("Ignore this test it is broken")]
        public void CancelBookingTest()
        {
            var criteria = new PrioSelectedProduct();
            var token = Guid.NewGuid();
            criteria.PrioApiConfirmedBooking = new PrioApi()
            {
                DistributorReference = "158170301982940",
                BookingReference = "ISA909308_2889"
            };
            var result = _prioTicketAdapter.CancelBooking(criteria, token.ToString(), out string _, out string _);
            Assert.IsTrue(result.Item3 == "Cancelled");
        }

        /// <summary>
        /// Test case to get prio availability
        /// This method have zero reference in unity
        /// </summary>
        [Test]
        public void GetPrioAvailablityTest()
        {
            var criteria = PrepareCriteriaForUpdateOptionForPrioActivity();
            var tokenKey = "L95-Q68P-KG89-L50";
            var distributionId = "1070";

            //First Scenario
            var token = Guid.NewGuid();
            var result = _prioTicketAdapter.GetPrioAvailablity(criteria, distributionId, tokenKey, token.ToString(), criteria.ActivityCode);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.AvailabilityStatus, AvailabilityStatus.AVAILABLE);

            //Second Scenario
            result = _prioTicketAdapter.GetPrioAvailablity(null, "", "", token.ToString(), criteria.ActivityCode);
            Assert.NotNull(result);

            //Third Scenario
            result = _prioTicketAdapter.GetPrioAvailablity(criteria, "", "", token.ToString(), criteria.ActivityCode);
            Assert.IsNull(result);
        }

        [Test]
        public void GetPrioTicketDetailsTest()
        {
            var criteria = new PrioTicketDetailsCriteria
            {
                Activity = new Activity
                {
                    Code = "3455",
                    Id = "23345",
                    ApiType = APIType.Prio,
                    ProductType = ProductType.Activity,
                    ProductOptions = new List<ProductOption>()
                },
                DistributorId = "1070",
                ApiCriteria = null,
                TokenKey = "L95-Q68P-KG89-L50",
                Token = Guid.NewGuid().ToString()
            };
            var result = _prioTicketAdapter.GetPrioTicketDetails(criteria);
            Assert.NotNull(result);
        }

        [Test]
        [Ignore("Ignoring this test case as it was hitting production API")]
        public void CreateReservationTest()
        {
            var activity = PrepareCriteriaForCreateBooking();
            activity.PrioApiConfirmedBooking = new PrioApi();

            var referencebnumber = "5121545121ASD";
            var token = Guid.NewGuid();
            var result = _prioTicketAdapter.CreateReservation(activity, referencebnumber, out string _, out string _, token.ToString());
            Assert.IsNull(result);
        }

        [Test]
        public void GetPrioTicketDetailsByTicketIdTest()
        {
            var result = _prioTicketAdapter.GetPrioTicketDetails("2616", Guid.NewGuid().ToString());
            Assert.NotNull(result);
        }

        [Test]
        public void GetPrioProductCurrencyCodeTest()
        {
            var criteria = new PrioTicketDetailsCriteria
            {
                Activity = new Activity
                {
                    Code = "3455",
                    Id = "23345",
                    ApiType = APIType.Prio,
                    ProductType = ProductType.Activity,
                    ProductOptions = new List<ProductOption>()
                },
                DistributorId = "2425",
                ApiCriteria = null,
                TokenKey = "X57-V69O-JG69-M54",
                Token = Guid.NewGuid().ToString()
            };
            var result = _prioTicketAdapter.GetPrioProductCurrencyCode(criteria);
            Assert.NotNull(result);
        }

        #region Private Methods

        private PrioCriteria PrepareCriteriaForUpdateOptionForPrioActivity()
        {
            var criteria = new PrioCriteria
            {
                ActivityCode = "2147",
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {
                        PassengerType.Adult,
                        1
                    },
                    {
                        PassengerType.Child,
                        0
                    },
                    {
                        PassengerType.Infant,
                        0
                    },
                    {
                        PassengerType.Senior,
                        0
                    }
                },
                CheckinDate = DateTime.Now.AddDays(5),
                CheckoutDate = DateTime.Now.AddDays(8),
                SupplierOptionCodes = new List<string>()
            };

            return criteria;
        }

        private Activity PrepareActivityData()
        {
            var firstOption =
                new ActivityOption()
                {
                    AvailabilityStatus = AvailabilityStatus.ONREQUEST,
                    Id = 135394,
                    SupplierOptionCode = "2147"
                };

            var secondOption =
                new ActivityOption()
                {
                    AvailabilityStatus = AvailabilityStatus.ONREQUEST,
                    Id = 135395,
                    SupplierOptionCode = "2144"
                };

            var activity = new Activity
            {
                ApiType = APIType.Prio,
                ActivityType = ActivityType.FullDay,
                ProductOptions = new List<ProductOption> { firstOption, secondOption },
                Id = "19400",
                Code = "12345"
            };
            return activity;
        }

        private PrioSelectedProduct PrepareCriteriaForCreateBooking()
        {
            var numberOfAdults = new Dictionary<PassengerType, int>
            {
                {PassengerType.Adult, 1}
            };

            var customer = new Customer()
            {
                Age = 19,
                PassengerType = PassengerType.Adult,
                Email = "test@email.com",
                IsLeadCustomer = true,
                FirstName = "test",
                LastName = "test"
            };

            var datePriceAndAvailability = new Dictionary<DateTime, PriceAndAvailability>
            {
                {
                    DateTime.Now.AddDays(7),
                    new PrioPriceAndAvailability()
                    {
                        IsSelected = true,
                        FromDateTime = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd'T'HH:mm:sszzz"),
                        ToDateTime = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd'T'HH:mm:sszzz")
                    }
                }
            };

            var pickupData = new PickupPointDetails { PickupPointId = "209667" };

            var activityOption = new ActivityOption
            {
                Id = 138940,
                AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                IsSelected = true,

                SupplierOptionCode = "2146",
                SellPrice = new Price()
                {
                    Amount = 195.05M,
                    DatePriceAndAvailabilty = datePriceAndAvailability
                },
                BasePrice = new Price()
                {
                    Amount = 1M,
                    DatePriceAndAvailabilty = datePriceAndAvailability
                },
                TravelInfo = new TravelInfo()
                {
                    NoOfPassengers = numberOfAdults,
                    StartDate = DateTime.Now.AddDays(7)
                },
                Customers = new List<Customer> { customer }
            };

            var prioSelectedProduct = new PrioSelectedProduct
            {
                ProductOptions = new List<ProductOption> { activityOption },
                Supplier = new Supplier()
                {
                    AddressLine1 = "test",
                    ZipCode = "123",
                    City = "test",
                    PhoneNumber = "9090909090"
                },

                ReservationExpiry = "SGI622433",
                PrioTicketClass = 3,
                PrioBookingStatus = "Reserved",
                PrioReservationReference = "153872548379717",
                PickupPointDetails = new PickupPointDetails[] { pickupData },
                PickupPoints = "mandatory",
                PrioApiConfirmedBooking = new PrioApi()
                {
                    DistributorReference = "1234",
                    BookingReference = "4354"
                },
                APIType = APIType.Prio,
                Code = "PHL",
                ActivityType = ActivityType.FullDay
            };

            return prioSelectedProduct;
        }

        #endregion Private Methods
    }
}