using Autofac;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.Enums;
using Isango.Entities.FareHarbor;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.FareHarbor;
using ServiceAdapters.FareHarbor.FareHarbor.Entities;
using System;
using System.Collections.Generic;
using Product = Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor.Product;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class FareHarborAdapterTest : BaseTest
    {
        private IFareHarborAdapter _fareHarborAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
           // var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _fareHarborAdapter = scope.Resolve<IFareHarborAdapter>();
            }
        }

        /// <summary>
        /// Test case to get companies
        /// </summary>
        [Test]
        public void GetCompaniesTest()
        {
            var token = Guid.NewGuid();
            var userKey = new FareHarborUserKey
            {
                UserKey = "5d9531fc-a1e4-4355-b364-1f0b2ac37d04"
            };
            var companyBatch = _fareHarborAdapter.GetCompanies(userKey, token.ToString());
            Assert.True(companyBatch.Count > 0);
        }

        /// <summary>
        /// Test case to get companies asynchronously (Need to work on it)
        /// </summary>
        [Test]
        public void GetCompaniesAsyncTest()
        {
            var companyBatch = _fareHarborAdapter.GetCompaniesAsync();
            companyBatch.Wait();
            var companyResponse = companyBatch.Result;
            Assert.True(companyResponse.Count > 0);
        }

        /// <summary>
        /// Test case to get items
        /// </summary>
        [Test]
        public void GetItemsTest()
        {
            var token = Guid.NewGuid();
            var supplier = new Supplier
            {
                ShortName = "carpedcfoodtours",
                UserKey = "5d9531fc-a1e4-4355-b364-1f0b2ac37d04"
            };
            var items = _fareHarborAdapter.GetItems(supplier, token.ToString());

            Assert.True(items.Count > 0);
        }

        /// <summary>
        /// Test case to get items asynchronously
        /// </summary>
        [Test]
        public void GetItemsAsyncTest()
        {
            //Need valid inputs
            var supplier = new Supplier
            {
                ShortName = "carpedcfoodtours",
                UserKey = "5d9531fc-a1e4-4355-b364-1f0b2ac37d04"
            };
            var token = Guid.NewGuid();
            var items = _fareHarborAdapter.GetItemsAsync(supplier, token.ToString());
            items.Wait();
            var itemResult = items.Result;

            Assert.True(itemResult.Count > 0);
        }

        /// <summary>
        /// Test case to get availabilities
        /// </summary>
        [Test]
        public void GetAvailabilitiesTest()
        {
            var criteria = new FareHarborCriteria
            {
                CompanyName = "bodyglove",
                ActivityCode = "183",
                CheckinDate = new DateTime(2018, 12, 20),
                CheckoutDate = new DateTime(2018, 12, 21),
                UserKey = "5d9531fc-a1e4-4355-b364-1f0b2ac37d04"
            };
            var token = Guid.NewGuid();
            var availabilities = _fareHarborAdapter.GetAvailabilities(criteria, token.ToString());
            Assert.True(availabilities.Count > 0);
        }

        /// <summary>
        /// Test case to get availabilities (Need to work on async methods)
        /// </summary>
        [Test]
        public void GetAvailabilitiesAsyncTest()
        {
            var criteria = new FareHarborCriteria
            {
                CheckinDate = new DateTime(2018, 12, 20),
                CheckoutDate = new DateTime(2018, 12, 21),
                UserKey = "5d9531fc-a1e4-4355-b364-1f0b2ac37d04"
            };
            var token = Guid.NewGuid();
            var availabilities = _fareHarborAdapter.GetAvailabilitiesAsync(criteria, token.ToString());
            Assert.NotNull(availabilities);
        }

        /// <summary>
        /// Test case to get availabilities by date
        /// </summary>
        [Test]
        public void GetAvailabilitiesByDateTest()
        {
            var criteria = new FareHarborCriteria
            {
                CheckinDate = new DateTime(2018, 12, 20),
                CheckoutDate = new DateTime(2018, 12, 21),
                UserKey = "5d9531fc-a1e4-4355-b364-1f0b2ac37d04"
            };
            var token = Guid.NewGuid();
            var availabilities = _fareHarborAdapter.GetAvailabilitiesByDate(criteria, token.ToString());
            Assert.True(availabilities.Count > 0);
        }

        /// <summary>
        /// Test case to get availabilities async (Need to work on async calls)
        /// </summary>
        [Test]
        public void GetAvailabilitiesByDateAsyncTest()
        {
            var criteria = new FareHarborCriteria
            {
                CheckinDate = new DateTime(2018, 12, 20),
                CheckoutDate = new DateTime(2018, 12, 21),
                UserKey = "5d9531fc-a1e4-4355-b364-1f0b2ac37d04"
            };
            var token = Guid.NewGuid();
            var availabilities = _fareHarborAdapter.GetAvailabilitiesByDateAsync(criteria, token.ToString());
            Assert.NotNull(availabilities);
        }

        /// <summary>
        /// Test case to create booking (Need valid input criteria)
        /// </summary>
        [Test]
        public void CreateBookingTest()
        {
            var criteria = PrepareBookingCriteria();
            var token = Guid.NewGuid();
            var bookingRes = _fareHarborAdapter.CreateBooking(criteria, token.ToString());
            Assert.NotNull(bookingRes);
        }

        /// <summary>
        /// Test case to create booking asynchronously (Need valid input criteria)
        /// </summary>
        [Test]
        public void CreateBookingAsyncTest()
        {
            var token = Guid.NewGuid();
            var criteria = PrepareBookingCriteria();
            var bookingRes = _fareHarborAdapter.CreateBookingAsync(criteria, token.ToString());
            bookingRes.Wait();
            var itemResult = bookingRes.Result;

            Assert.NotNull(itemResult);
        }

        /// <summary>
        /// Test case to validate booking (Need valid input criteria)
        /// </summary>
        [Test]
        public void ValidateBookingTest()
        {
            var token = Guid.NewGuid();
            var criteria = PrepareBookingCriteria();
            var bookingRes = _fareHarborAdapter.ValidateBooking(criteria, token.ToString());

            Assert.NotNull(bookingRes);
        }

        /// <summary>
        /// Test case to validate booking asynchronously (Need valid input criteria)
        /// </summary>
        [Test]
        public void ValidateBookingAsyncTest()
        {
            var token = Guid.NewGuid();
            var criteria = PrepareBookingCriteria();
            var bookingRes = _fareHarborAdapter.ValidateBookingAsync(criteria, token.ToString());
            bookingRes.Wait();
            var itemResult = bookingRes.Result;

            Assert.NotNull(itemResult);
        }

        /// <summary>
        /// Test case to get booking (Need valid input criteria)
        /// </summary>
        [Test]
        public void GetBookingTest()
        {
            var token = Guid.NewGuid();
            var bookingRes = _fareHarborAdapter.GetBooking("carpedcfoodtours", "SGI622543", token.ToString());
            Assert.NotNull(bookingRes);
        }

        /// <summary>
        /// Test case to get booking asynchronously (Need valid input criteria)
        /// </summary>
        [Test]
        public void GetBookingAsyncTest()
        {
            var token = Guid.NewGuid();
            var bookingRes = _fareHarborAdapter.GetBookingAsync("carpedcfoodtours", "SGI622543", token.ToString());
            bookingRes.Wait();
            var itemResult = bookingRes.Result;

            Assert.NotNull(itemResult);
        }

        /// <summary>
        /// Test case to delete booking (Need valid input criteria)
        /// </summary>
        [Test]
        public void DeleteBookingTest()
        {
            var token = Guid.NewGuid();
            var selectedProduct = new List<FareHarborSelectedProduct> { PrepareSelectedProductCriteria() };
            var bookingRes = _fareHarborAdapter.DeleteBooking(selectedProduct, token.ToString());
            Assert.NotNull(bookingRes);
        }

        /// <summary>
        /// Test case to delete booking asynchronously (Need valid input criteria)
        /// </summary>
        [Test]
        public void DeleteBookingAsyncTest()
        {
            var token = Guid.NewGuid();
            var bookingRes = _fareHarborAdapter.DeleteBookingAsync("fittoursnyc", "0e40b66f-b484-458b-b1fb-e37b08a1d2d3", token.ToString());
            bookingRes.Wait();
            var itemResult = bookingRes.Result;

            Assert.NotNull(itemResult);
        }

        /// <summary>
        /// Test case to get lodging
        /// </summary>
        [Test]
        public void GetLodgingsTest()
        {
            var token = Guid.NewGuid();
            var criteria = new FareHarborRequest
            {
                ShortName = "carpedcfoodtours"
            };
            var bookingRes = _fareHarborAdapter.GetLodgings(criteria, token.ToString());
            Assert.NotNull(bookingRes);
        }

        /// <summary>
        /// Test case to get lodging asynchronously
        /// </summary>
        [Test]
        public void GetLodgingsAsyncTest()
        {
            var token = Guid.NewGuid();
            var criteria = new FareHarborRequest
            {
                ShortName = "carpedcfoodtours"
            };
            var bookingRes = _fareHarborAdapter.GetLodgingsAsync(criteria, token.ToString());
            bookingRes.Wait();
            var itemResult = bookingRes.Result;

            Assert.NotNull(itemResult);
        }

        /// <summary>
        /// Test case to get lodging availabilities
        /// </summary>
        [Test]
        public void GetLodgingsAvailabilityTest()
        {
            var token = Guid.NewGuid();
            var criteria = new FareHarborRequest
            {
                ShortName = "carpedcfoodtours",
                Availability = "108852513"
            };
            var bookingRes = _fareHarborAdapter.GetLodgingsAvailability(criteria, token.ToString());
            Assert.NotNull(bookingRes);
        }

        /// <summary>
        /// Test case to get lodging availabilities asynchronously
        /// </summary>
        [Test]
        public void GetLodgingsAvailabilityAsyncTest()
        {
            var token = Guid.NewGuid();
            var criteria = new FareHarborRequest
            {
                ShortName = "carpedcfoodtours",
                Availability = "108852513"
            };
            var bookingRes = _fareHarborAdapter.GetLodgingsAvailabilityAsync(criteria, token.ToString());
            bookingRes.Wait();
            var itemResult = bookingRes.Result;

            Assert.NotNull(itemResult);
        }

        /// <summary>
        /// Test case to get updated booking note (Need valid input criteria)
        /// </summary>
        [Test]
        public void UpdateBookingNoteTest()
        {
            var token = Guid.NewGuid();
            var criteria = PrepareBookingCriteria();
            var bookingRes = _fareHarborAdapter.UpdateBookingNote(criteria, token.ToString());
            Assert.NotNull(bookingRes);
        }

        /// <summary>
        /// Test case to get updated booking note asynchronously (Need valid input criteria)
        /// </summary>
        [Test]
        public void UpdateBookingNoteAsyncTest()
        {
            var token = Guid.NewGuid();
            var criteria = PrepareBookingCriteria();
            var bookingRes = _fareHarborAdapter.UpdateBookingNoteAsync(criteria, token.ToString());
            bookingRes.Wait();
            var itemResult = bookingRes.Result;

            Assert.NotNull(itemResult);
        }

        /// <summary>
        /// Test case to re booking (Need valid input criteria)
        /// </summary>
        [Test]
        public void ReBookingTest()
        {
            var token = Guid.NewGuid();
            var criteria = PrepareBookingCriteria();
            var bookingRes = _fareHarborAdapter.Rebooking(criteria, token.ToString());
            Assert.NotNull(bookingRes);
        }

        /// <summary>
        /// Test case to re booking asynchronously (Need valid input criteria)
        /// </summary>
        [Test]
        public void ReBookingAsyncTest()
        {
            var token = Guid.NewGuid();
            var criteria = PrepareBookingCriteria();
            var bookingRes = _fareHarborAdapter.RebookingAsync(criteria, token.ToString());
            bookingRes.Wait();
            var itemResult = bookingRes.Result;

            Assert.NotNull(itemResult);
        }

        /// <summary>
        /// Test case to re booking asynchronously (Need valid input criteria)
        /// </summary>
        [Test]
        public void FareHarborCreateBookingTest()
        {
            var token = Guid.NewGuid();
            var criteria = new List<FareHarborSelectedProduct> { PrepareSelectedProductCriteria() };
            var bookingRes = _fareHarborAdapter.CreateBooking(criteria, "SGI622543", token.ToString(), out _, out _);

            Assert.NotNull(bookingRes);
        }

        /// <summary>
        /// Get all customer prototypes
        /// </summary>
        [Test]
        public void GetAllCustomerPrototypesTest()
        {
            var token = Guid.NewGuid();
            var criteria = new Product
            {
                SupplierName = "universal-tourguide",
                FactsheetId = 83260,
                UserKey = "669e7c10-9fa6-4bac-a3be-7cce5aa3d71c",
                ServiceId = 26871
            };
            var customerProtoTypeCustomerTypes = _fareHarborAdapter.GetCustomerPrototypesByProductId(criteria, token.ToString());

            Assert.IsTrue(customerProtoTypeCustomerTypes != null);
        }

        #region Private Methods

        /// <summary>
        /// Prepare booking criteria
        /// </summary>
        /// <returns></returns>
        private Booking PrepareBookingCriteria()
        {
            var booking = new Booking
            {
                SelectedProducts = new List<SelectedProduct>
                {
                    PrepareSelectedProductCriteria()
                }
            };
            return booking;
        }

        /// <summary>
        /// Prepare selected product criteria
        /// </summary>
        /// <returns></returns>
        private FareHarborSelectedProduct PrepareSelectedProductCriteria()
        {
            var adult = new AdultPricingUnit()
            {
                Price = 60.3500M
            };
            var fareHarborSelected = new FareHarborSelectedProduct()
            {
                APIType = APIType.Fareharbor,
                ActivityType = ActivityType.FullDay,
                AdultPriceId = 0,

                FactsheetId = 0,
                SellPrice = 60.35,
                ProductOptions = new List<ProductOption>
                {
                    new ActivityOption()
                    {
                        TravelInfo = new TravelInfo()
                        {
                            StartDate = new DateTime(2018, 10, 03),
                            NumberOfNights = -1,
                            NoOfPassengers = new Dictionary<PassengerType, int>
                            {
                                {PassengerType.Adult, 1 },
                                {PassengerType.Child, 1 },
                                {PassengerType.Youth, 1 },
                                {PassengerType.Infant, 1 }
                            }
                        },
                        Customers = new List<Customer>()
                        {
                            new Customer()
                            {
                                Age = 19,
                                PassengerType = PassengerType.Adult,
                                FirstName = "Test",
                                LastName = "Test",
                                IsLeadCustomer = true,
                                CustomerId = 0,
                                Email = "test@email.com"
                            }
                        },
                        SellPrice = new Price()
                        {
                            Amount = 60.3500M,
                            DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
                            {
                                {
                                    new DateTime(2018, 10, 03),
                                    new FareHarborPriceAndAvailability()
                                    {
                                        TourDepartureId = 1060773,
                                        AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                                        PricingUnits = new List<PricingUnit>()
                                        {
                                           adult
                                        },
                                        IsSelected = true,
                                        CustomerTypePriceIds = new Dictionary<PassengerType, long>
                                        {
                                            {PassengerType.Adult, 12}
                                        }
                                    }
                                }
                            }
                        },
                        IsSelected = true,
                        UserKey = "5d9531fc-a1e4-4355-b364-1f0b2ac37d04",
                        AvailToken = "108852513",
                        Id = 108852513
                    }
                },
                BookingReferenceNumber = "SGI622543",
                Code = "AH11",
                Price = 60.3500M
            };

            return fareHarborSelected;
        }

        #endregion Private Methods
    }
}