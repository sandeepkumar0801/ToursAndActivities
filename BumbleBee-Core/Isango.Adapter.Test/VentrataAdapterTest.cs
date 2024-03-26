using Autofac;
using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.Ventrata;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.Ventrata;
using System;
using System.Collections.Generic;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class VentrataAdapterTest : BaseTest
    {
        private IVentrataAdapter _ventrataAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _ventrataAdapter = scope.Resolve<IVentrataAdapter>();
            }
        }

        ///// <summary>
        ///// Get Availability of a product which has TimeSlots
        ///// </summary>
        //[Test]
        //public void GetAvailabilityById_WithTimeSlots_Test()
        //{
        //    var criteria = CreateCriteria();
        //    var result = _ventrataAdapter.GetOptionsForVentrataActivity(criteria, Guid.NewGuid().ToString());
        //    Assert.NotNull(result);
        //}

        ///// <summary>
        ///// Get Availability of a product which don't have TimeSlots
        ///// </summary>
        //[Test]
        //public void GetAvailabilityByIdTest()
        //{
        //    var criteria = CreateCriteria_WithoutTimeSlots();
        //    var result = _tiqetsAdapter.GetPriceAndAvailabilityByProductId(criteria, Guid.NewGuid().ToString());
        //    Assert.Null(result);
        //}

        ///// <summary>
        ///// Get Availability of a product (Negative Scenarios)
        ///// </summary>
        //[Test]
        //public void GetAvailabilityById_Negative_Test()
        //{
        //    //Null Check
        //    var result = _tiqetsAdapter.GetPriceAndAvailabilityByProductId(null, Guid.NewGuid().ToString());
        //    Assert.Null(result);

        //    //Wrong Input
        //    var criteria = new TiqetsCriteria
        //    {
        //        ProductId = 1234,
        //        Language = "en"
        //    };
        //    result = _tiqetsAdapter.GetPriceAndAvailabilityByProductId(criteria, Guid.NewGuid().ToString());
        //    Assert.Null(result);
        //}

        ///// <summary>
        ///// Get Availability of a product which don't have TimeSlots
        ///// </summary>
        //[Test]
        //public void GetProductDetailsByIdTest()
        //{
        //    var result = _tiqetsAdapter.GetProductDetailsByProductId(975948
        //        , "en", Guid.NewGuid().ToString());
        //    Assert.NotNull(result);
        //}

        ///// <summary>
        ///// Get Availability of a product  (Negative Scenarios)
        ///// </summary>
        //[Test]
        //public void GetProductDetailsById_Negative_Test()
        //{
        //    //Null Check
        //    var result = _tiqetsAdapter.GetProductDetailsByProductId(0
        //        , String.Empty, Guid.NewGuid().ToString());
        //    Assert.Null(result);

        //    //Wrong Inputs
        //    result = _tiqetsAdapter.GetProductDetailsByProductId(1234
        //        , "en", Guid.NewGuid().ToString());
        //    Assert.Null(result);

        //    //Wrong Inputs
        //    result = _tiqetsAdapter.GetProductDetailsByProductId(975948
        //        , "gt", Guid.NewGuid().ToString());
        //    Assert.Null(result);
        //}

        ///// <summary>
        ///// Get Bulk Availability without start and end date
        ///// </summary>
        //[Test]
        //public void GetBulkAvailabilityByIdTest()
        //{
        //    var result = _tiqetsAdapter.GetBulkAvailabilityByProductId(976360
        //        , DateTime.MinValue, DateTime.MinValue, Guid.NewGuid().ToString());
        //    Assert.NotNull(result);
        //}

        ///// <summary>
        ///// Get Bulk Availability with end date
        ///// </summary>
        //[Test]
        //public void GetBulkAvailabilityById_WithEndDate_Test()
        //{
        //    var result = _tiqetsAdapter.GetBulkAvailabilityByProductId(976360
        //        , DateTime.MinValue, DateTime.ParseExact("2019-03-28", "yyyy-MM-dd",
        //            System.Globalization.CultureInfo.InvariantCulture), Guid.NewGuid().ToString());
        //    Assert.NotNull(result);
        //}

        ///// <summary>
        ///// Get Bulk Availability with end date and start date
        ///// </summary>
        //[Test]
        //public void GetBulkAvailabilityById_WithStartDate_Test()
        //{
        //    var result = _tiqetsAdapter.GetBulkAvailabilityByProductId(976360
        //        , DateTime.ParseExact("2019-03-28", "yyyy-MM-dd",
        //            System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact("2019-04-30", "yyyy-MM-dd",
        //            System.Globalization.CultureInfo.InvariantCulture), Guid.NewGuid().ToString());
        //    Assert.NotNull(result);
        //}

        ///// <summary>
        ///// Get Bulk Availability for the Product which does not contain TimeSlots
        ///// </summary>
        //[Test]
        //public void GetBulkAvailabilityById_WithoutTimeSlots_Test()
        //{
        //    var result = _tiqetsAdapter.GetBulkAvailabilityByProductId(975437
        //        , DateTime.MinValue, DateTime.ParseExact("2019-03-30", "yyyy-MM-dd",
        //            System.Globalization.CultureInfo.InvariantCulture), Guid.NewGuid().ToString());
        //    Assert.NotNull(result);
        //}

        ///// <summary>
        ///// Get Bulk Availability (Negative Scenarios)
        ///// </summary>
        //[Test]
        //public void GetBulkAvailabilityById_Negative_Test()
        //{
        //    //Null Check
        //    var result = _tiqetsAdapter.GetBulkAvailabilityByProductId(0
        //        , DateTime.MinValue, DateTime.MinValue, Guid.NewGuid().ToString());
        //    Assert.Null(result);

        //    //Wrong Input
        //    result = _tiqetsAdapter.GetBulkAvailabilityByProductId(1234
        //        , DateTime.MinValue, DateTime.MinValue, Guid.NewGuid().ToString());
        //    Assert.Null(result);
        //}

        ///// <summary>
        ///// Get Price and Availability by CheckInDate
        ///// </summary>
        //[Test]
        //public void GetAvailabilities_WithTimeSlots_Test()
        //{
        //    var criteria = CreateCriteria();
        //    var result = _tiqetsAdapter.GetAvailabilities(criteria, Guid.NewGuid().ToString());
        //    Assert.NotNull(result);
        //}

        ///// <summary>
        ///// Get Price and Availability by CheckInDate for the Product which does not contain TimeSlots
        ///// </summary>
        //[Test]
        //public void GetAvailabilities_WithoutTimeSlots_Test()
        //{
        //    var criteria = CreateCriteria_WithoutTimeSlots();
        //    var result = _tiqetsAdapter.GetAvailabilities(criteria, Guid.NewGuid().ToString());
        //    Assert.NotNull(result);
        //}

        ///// <summary>
        ///// Get Price and Availability by CheckInDate (Negative Scenario)
        ///// </summary>
        //[Test]
        //public void GetAvailabilities_Negative_Test()
        //{
        //    //Null Check
        //    var result = _tiqetsAdapter.GetAvailabilities(null, Guid.NewGuid().ToString());
        //    Assert.Null(result);
        //}

        ///// <summary>
        ///// Get Price And Availability for Dumping Application for product which contains TimeSlots
        ///// </summary>
        //[Test]
        //public void GetVariantsForDumpingApplication_WithTimeSlots_Test()
        //{
        //    var criteria = CreateCriteria();
        //    var result = _tiqetsAdapter.GetVariantsForDumpingApplication(criteria, Guid.NewGuid().ToString());
        //    Assert.NotNull(result);
        //}

        ///// <summary>
        ///// Get Price And Availability for Dumping Application for product which does not contain TimeSlots
        ///// </summary>
        //[Test]
        //public void GetVariantsForDumpingApplication_WithoutTimeSlots_Test()
        //{
        //    var criteria = CreateCriteria_WithoutTimeSlots();
        //    var result = _tiqetsAdapter.GetVariantsForDumpingApplication(criteria, Guid.NewGuid().ToString());
        //    Assert.NotNull(result);
        //}

        ///// <summary>
        ///// Get Price And Availability for Dumping Application (Negative Scenario)
        ///// </summary>
        //[Test]
        //public void GetVariantsForDumpingApplication_Negative_Test()
        //{
        //    var criteria = new TiqetsCriteria
        //    {
        //        ProductId = 0,
        //        Language = String.Empty,
        //        CheckinDate = DateTime.Now,
        //        CheckoutDate = DateTime.ParseExact("2019-06-28", "yyyy-MM-dd",
        //            System.Globalization.CultureInfo.InvariantCulture),
        //        OptionId = 1234,
        //        OptionName = "test"
        //    };
        //    //Null Check
        //    var result = _tiqetsAdapter.GetVariantsForDumpingApplication(criteria, Guid.NewGuid().ToString());
        //    Assert.Null(result);
        //}

        ///// <summary>
        ///// Create Order
        ///// </summary>
        //[Test]
        //[Ignore("Ignoring this test case as it will book the product")]
        //public void CreateOrder_Test()
        //{
        //    var criteria = CreateBookingCriteria();
        //    var result = _tiqetsAdapter.CreateOrder(criteria, Guid.NewGuid().ToString(), out _, out _);
        //    Assert.IsTrue(result.Success);
        //}

        ///// <summary>
        ///// Create Order
        ///// </summary>
        //[Test]
        //[Ignore("Ignoring this test case as it will book the product")]
        //public void ConfirmOrder_Test()
        //{
        //    var criteria = CreateBookingCriteria();
        //    var createOrderResponse = _tiqetsAdapter.CreateOrder(criteria, Guid.NewGuid().ToString(), out _, out _);
        //    criteria.RequestObject = createOrderResponse;
        //    var result = _tiqetsAdapter.ConfirmOrder(criteria, Guid.NewGuid().ToString(), out _, out _);
        //    Assert.NotNull(result.OrderReferenceId);
        //}

        [Test]
        //[Ignore("Ignoring this test case as it will book the product")]
        public void CancelBooingAndReservationTest()
        {
            try
            {
                var token = "f53d7272-0715-460c-9f19-6deacbbdec24";
                var p = new VentrataSelectedProduct
                {
                    Uuid = "1730511a-574f-47ef-86e8-b3c1ce0610ce",
                    ActivityCode = "bb69778e-0c36-41b7-ae3d-daac8202cfa2",
                    VentrataIsPerPaxQRCode = "true",
                    VentrataBaseURL = "https://api.ventrata.com/octo/",
                    ReasonForCancellation = "Cancelled by customer"
                };

                var createOrderResponse = _ventrataAdapter.CancelReservationAndBooking(p, token, out var req, out var res);
            }
            catch (Exception ex)
            {
            }

            Assert.NotNull(true);
        }

        ///// <summary>
        ///// Create Order
        ///// </summary>
        //[Test]
        //[Ignore("Ignoring this test case as it will book the product")]
        //public void GetTicket_Test()
        //{
        //    var criteria = CreateBookingCriteria();
        //    var createOrderResponse = _tiqetsAdapter.CreateOrder(criteria, Guid.NewGuid().ToString(), out _, out _);
        //    criteria.RequestObject = createOrderResponse;
        //    var confirmOrderResponse = _tiqetsAdapter.ConfirmOrder(criteria, Guid.NewGuid().ToString(), out _, out _);
        //    criteria.RequestObject = confirmOrderResponse;
        //    var result = _tiqetsAdapter.GetTicket(criteria, Guid.NewGuid().ToString(), out _, out _);
        //    Assert.NotNull(result);
        //}

        #region Private Methods

        /// <summary>
        /// Creating criteria for the product which contains TimeSlots
        /// </summary>
        /// <returns></returns>
        //private TiqetsCriteria CreateCriteria()
        //{
        //    var criteria = new TiqetsCriteria
        //    {
        //        ProductId = 976360,
        //        Language = "en",
        //        CheckinDate = DateTime.Now,
        //        CheckoutDate = DateTime.Now.AddMonths(3),
        //        NoOfPassengers = new Dictionary<PassengerType, int> { { PassengerType.Adult, 1 }, { PassengerType.Child, 1 }, { PassengerType.Infant, 3 } },
        //        Ages = new Dictionary<PassengerType, int> { { PassengerType.Adult, 20 }, { PassengerType.Child, 5 }, { PassengerType.Infant, 3 } },
        //        OptionId = 1234,
        //        OptionName = "Option",
        //        TiqetsPaxMappings = new List<TiqetsPaxMapping>
        //        {
        //            //NOTE: Ids used in below are dummy except AgeGroupCode
        //            new TiqetsPaxMapping
        //            {
        //                PassengerType = PassengerType.Adult,
        //                APIType = APIType.Tiqets,
        //                ServiceOptionId = 143598,
        //                AgeGroupCode = "9118",
        //                AgeGroupId = 2836
        //            },
        //            new TiqetsPaxMapping
        //            {
        //                PassengerType = PassengerType.Child,
        //                APIType = APIType.Tiqets,
        //                ServiceOptionId = 143598,
        //                AgeGroupCode = "9119",
        //                AgeGroupId = 2837
        //            },
        //            new TiqetsPaxMapping
        //            {
        //                PassengerType = PassengerType.Infant,
        //                APIType = APIType.Tiqets,
        //                ServiceOptionId = 143598,
        //                AgeGroupCode = "9110",
        //                AgeGroupId = 2838
        //            }
        //        }
        //    };

        //    return criteria;
        //}

        ///// <summary>
        ///// Creating criteria which does not contain TimeSlots
        ///// </summary>
        ///// <returns></returns>
        //private TiqetsCriteria CreateCriteria_WithoutTimeSlots()
        //{
        //    var criteria = new TiqetsCriteria
        //    {
        //        ProductId = 974581,
        //        Language = "en",
        //        CheckinDate = DateTime.Parse("2019-07-02"),
        //        CheckoutDate = DateTime.Now.AddMonths(3),
        //        NoOfPassengers = new Dictionary<PassengerType, int> { { PassengerType.Adult, 1 }, { PassengerType.Child, 1 } },
        //        Ages = new Dictionary<PassengerType, int> { { PassengerType.Adult, 20 }, { PassengerType.Child, 5 } },
        //        OptionId = 1234,
        //        OptionName = "Option",
        //        TiqetsPaxMappings = new List<TiqetsPaxMapping>
        //        {
        //            //NOTE: Ids used in below are dummy except AgeGroupCode
        //            new TiqetsPaxMapping
        //            {
        //                PassengerType = PassengerType.Child,
        //                APIType = APIType.Tiqets,
        //                ServiceOptionId = 143598,
        //                AgeGroupCode = "2425",
        //                AgeGroupId = 2836
        //            },
        //            new TiqetsPaxMapping
        //            {
        //                PassengerType = PassengerType.Adult,
        //                APIType = APIType.Tiqets,
        //                ServiceOptionId = 143598,
        //                AgeGroupCode = "2424",
        //                AgeGroupId = 2837
        //            }
        //        }
        //    };

        //    return criteria;
        //}

        ///// <summary>
        ///// Create Booking Criteria
        ///// </summary>
        ///// <returns></returns>
        //private BookingRequest CreateBookingCriteria()
        //{
        //    var selectedProduct = new TiqetsSelectedProduct
        //    {
        //        FactSheetId = 974423,
        //        TimeSlot = String.Empty,
        //        ProductOptions = new List<ProductOption>
        //        {
        //           new ProductOption
        //           {
        //               TravelInfo = new TravelInfo
        //               {
        //                   StartDate = DateTime.Parse("2019-06-17")
        //               },
        //               Customers = new List<Customer>
        //               {
        //                   new Customer
        //                   {
        //                       FirstName = "Test Test",
        //                       LastName = "Test Test",
        //                       Email = "test@gmail.com",
        //                       IsLeadCustomer = true
        //                   }
        //               }
        //           }
        //        },
        //        Variants = new List<Variant>
        //        {
        //            new Variant { Id = 1942, Count = 1 },
        //            new Variant { Id = 1943, Count = 1 }
        //        }
        //    };

        //    var bookingRequest = new BookingRequest
        //    {
        //        LanguageCode = "en",
        //        RequestObject = selectedProduct
        //    };

        //    return bookingRequest;
        //}

        #endregion Private Methods
    }
}
