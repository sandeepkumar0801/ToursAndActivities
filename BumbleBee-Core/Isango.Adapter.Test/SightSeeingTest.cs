//using Autofac;
//using Isango.Entities;
//using Isango.Entities.Activities;
//using Isango.Entities.CitySightseeing;
//using Isango.Entities.Enums;
//using Isango.Register;
//using NUnit.Framework;
//using ServiceAdapters.SightSeeing;
//using System;
//using System.Collections.Generic;

//namespace Isango.Adapter.Test
//{
//    [TestFixture]
//    internal class SightSeeingTest : BaseTest
//    {
//        private ISightSeeingAdapter _sightSeeingAdapter;

//        [OneTimeSetUp]
//        public void TestInitialise()
//        {
//            var container = Startup._builder.Build();

//            using (var scope = container.BeginLifetimeScope())
//            {
//                _sightSeeingAdapter = scope.Resolve<ISightSeeingAdapter>();
//            }
//        }

//        /// <summary>
//        /// Method to test Issue Ticket
//        /// </summary>
//        [Test]
//        public void IssueTicketTest()
//        {
//            var selectedProduct = PrepareCitySightseeingSelectedProduct();
//            var token = Guid.NewGuid();
//            var result = _sightSeeingAdapter.IssueTicket(selectedProduct, "SGI622432", token.ToString(), out _, out _);
//            Assert.IsNotNull(result);
//        }

//        /// <summary>
//        /// Method to test Confirm ticket
//        /// </summary>
//        [Test]
//        public void ConfirmTicketTest()
//        {
//            var token = Guid.NewGuid();
//            var selectedProduct = PrepareCitySightseeingSelectedProduct();
//            var result = _sightSeeingAdapter.ConfirmTicket(selectedProduct, token.ToString(), out _, out _);
//            Assert.IsNotNull(result);
//            Assert.IsTrue(result);
//        }

//        /// <summary>
//        /// Method to test async Issue Ticket
//        /// </summary>
//        [Test]
//        public void IssueTicketAsyncTest()
//        {
//            var token = Guid.NewGuid();
//            var selectedProduct = PrepareCitySightseeingSelectedProduct();
//            var result = _sightSeeingAdapter.IssueTicketAsync(selectedProduct, token.ToString());
//            result.Wait();
//            Assert.IsNotNull(result);
//        }

//        /// <summary>
//        /// Method to test async Confirm ticket
//        /// </summary>
//        [Test]
//        public void ConfirmTicketAsyncTest()
//        {
//            var token = Guid.NewGuid();
//            var selectedProduct = PrepareCitySightseeingSelectedProduct();
//            var result = _sightSeeingAdapter.ConfirmTicketAsync(selectedProduct, token.ToString());
//            result.Wait();
//            Assert.IsNotNull(result);
//        }

//        private List<SelectedProduct> PrepareCitySightseeingSelectedProduct()
//        {
//            var citySightseeingSelectedProducts = new List<SelectedProduct>();
//            var productOption = new List<ProductOption>();
//            var adult = new AdultPricingUnit()
//            {
//                Price = 60.3500M
//            };
//            var customer = new List<Customer>()
//            {
//                new Customer()
//                {
//                    Age = 28,
//                    PassengerType =  PassengerType.Adult,
//                    FirstName = "John",
//                    LastName = "Doh",
//                    IsLeadCustomer = true,
//                    CustomerId = 0
//                }
//            };

//            var travelInfo = new TravelInfo()
//            {
//                StartDate = new DateTime(2018, 10, 03),
//                NumberOfNights = -1,
//                NoOfPassengers = new Dictionary<PassengerType, int>
//                {
//                    {PassengerType.Adult, 1 }
//                }
//            };

//            var datePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
//            {
//                {
//                    new DateTime(2018, 10, 03),
//                    new DefaultPriceAndAvailability()
//                    {
//                        TourDepartureId = 1060773,
//                        AvailabilityStatus = AvailabilityStatus.AVAILABLE,
//                        PricingUnits = new List<PricingUnit>()
//                        {
//                            adult
//                        }
//                    }
//                }
//            };

//            var activityOption = new ActivityOption()
//            {
//                SellPrice = new Price()
//                {
//                    Amount = 60.3500M,
//                    DatePriceAndAvailabilty = datePriceAndAvailabilty
//                },
//                TravelInfo = travelInfo
//            };
//            productOption.Add(activityOption);

//            var citySightseeingSelectedProd = new CitySightseeingSelectedProduct()
//            {
//                Name = "test",
//                APIType = APIType.Citysightseeing,
//                ProductOptions = productOption,
//                ProductName = null,
//                ProductType = ProductType.Activity,
//                Regions = null,
//                ActivityType = ActivityType.Undefined,
//                Price = 60.3500M,
//                Supplier = null,
//                AvailabilityStatus = AvailabilityStatus.AVAILABLE,
//                Code = null,
//                ActivityCode = "CSFI24HR",
//                Pnr = null,
//                Quantity = 1,
//                FactsheetId = 0
//            };

//            citySightseeingSelectedProducts.Add(citySightseeingSelectedProd);

//            return citySightseeingSelectedProducts;
//        }

//        /// <summary>
//        /// Method to cancel single ticket
//        /// </summary>
//        [Test]
//        public void CancelSingleTicketAsyncTest()
//        {
//            var token = Guid.NewGuid();
//            var IsCancelled = false;
//            var selectedProduct = new List<SelectedProduct>
//            {
//                new CitySightseeingSelectedProduct()
//                {
//                    AvailabilityReferenceId ="TestAvailabilityReferenceId",
//                    Pnr = "366655"
//                }
//            };
//            var result = _sightSeeingAdapter.CancelTicketAsync(selectedProduct, token.ToString());
//            result.Wait();

//            //currently we are getting either true or false,same value for all PNR number
//            //in this case we get status against one PNR number
//            if (result.Result.ContainsValue(true)) IsCancelled = true;

//            //Check for cancellation status - if false
//            //then throw error message - "Cancellation Failed"
//            Assert.That(IsCancelled, "Cancellation Failed");
//        }

//        /// <summary>
//        /// Method to cancel multiple tickets
//        /// </summary>
//        [Test]
//        public void CancelMultipleTicketAsyncTest()
//        {
//            var token = Guid.NewGuid();
//            var IsCancelled = false;
//            var selectedProducts = new List<SelectedProduct>
//            {
//                new CitySightseeingSelectedProduct()
//                {
//                    AvailabilityReferenceId = "TestAvailabilityReferenceId1",
//                    Pnr = "366655"
//                },
//                new CitySightseeingSelectedProduct()
//                {
//                    AvailabilityReferenceId ="TestAvailabilityReferenceId2",
//                    Pnr="369008"
//                },
//                new CitySightseeingSelectedProduct()
//                {
//                    AvailabilityReferenceId ="TestAvailabilityReferenceId3",
//                    Pnr="369012"
//                }
//            };
//            var result = _sightSeeingAdapter.CancelTicketAsync(selectedProducts, token.ToString());
//            result.Wait();

//            //currently we are getting either true or false,same value for all PNR number
//            //so just checking for true in all status
//            if (result.Result.ContainsValue(true)) IsCancelled = true;

//            //Check for cancellation status - if false
//            //then throw error message - "Cancellation Failed"
//            Assert.That(IsCancelled, "Cancellation Failed");
//        }

//        /// <summary>
//        /// Method to cancel ticket having empty pnr
//        /// Expected result - cancellation will fail
//        /// </summary>
//        [Test]
//        public void CancelTicketWithEmptyPnrAsyncTest()
//        {
//            var token = Guid.NewGuid();
//            var IsCancelled = false;
//            var selectedProducts = new List<SelectedProduct>
//            {
//                new CitySightseeingSelectedProduct()
//                {
//                    AvailabilityReferenceId="TestAvailabilityReferenceId",
//                    Pnr = string.Empty
//                }
//            };
//            var result = _sightSeeingAdapter.CancelTicketAsync(selectedProducts, token.ToString());
//            result.Wait();

//            //currently we are getting cancellation status false for empty PNR number
//            //so we will get false as ticket cancellation status
//            //then cancellation for city sight seeing is false
//            if (result.Result.ContainsValue(true)) IsCancelled = true;

//            //Cancellation Fail as expected value
//            Assert.False(IsCancelled);
//        }
//    }
//}