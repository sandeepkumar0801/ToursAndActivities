using Autofac;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.MoulinRouge;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.MoulinRouge;
using System;
using System.Collections.Generic;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class MoulinRougeAdapterTest : BaseTest
    {
        private IMoulinRougeAdapter _moulinRougeAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _moulinRougeAdapter = scope.Resolve<IMoulinRougeAdapter>();
            }
        }

        /// <summary>
        /// Test case to get converted date and price activity
        /// </summary>
        [Test] [Ignore("")]
        public void GetConvertedActivityDateAndPriceTest()
        {
            var token = Guid.NewGuid();
            var fromDate = new DateTime(2019, 01, 07);
            var toDate = new DateTime(2019, 01, 13);
            var result = _moulinRougeAdapter.GetConvertedActivtyDateAndPrice(fromDate, toDate, 11, token.ToString());
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(16628, result[0].ID);
        }

        /// <summary>
        /// Test case to release seats from cart
        /// </summary>
         [Test] [Ignore("")]
        public void AddToCartTest()
        {
            var inputContext = new MoulinRougeSelectedProduct()
            {
                CategoryId = 39650,
                CatalogDateId = 3644,
                ContingentId = 82647,
                BlocId = 0,
                FloorId = 78718,
                RateId = 81622,
                Quantity = 2
            };
            var token = Guid.NewGuid();
            var result = _moulinRougeAdapter.AddToCart(inputContext, token.ToString());
            Assert.NotNull(result);
            Assert.NotNull(result.TemporaryOrderId);
            Assert.NotNull(result.TemporaryOrderRowId);
            Assert.Greater(result.Ids.Count, 0);
        }

        /// <summary>
        /// Test case to release seat
        /// </summary>
         [Test] [Ignore("")]
        public void ReleaseSeatsAsyncTest()
        {
            var criteria = new MoulinRougeCriteria()
            {
                MoulinRougeContext = new APIContextMoulinRouge()
                {
                    TemporaryOrderId = "295815",
                    TemporaryOrderRowId = "325483",
                    Ids = new List<int>() { 10123554 }
                }
            };
            var token = Guid.NewGuid();
            var result = _moulinRougeAdapter.ReleaseSeatsAsync(criteria, token.ToString());
            result.Wait();
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test case to order confirm
        /// This test case depend on booking reference number which should be unique in each transaction
        /// </summary>
         [Test] [Ignore("")]
        public void OrderConfirmCombinedTest()
        {
            var token = Guid.NewGuid();
            var result = _moulinRougeAdapter.OrderConfirmCombined(PrepareCriteriaForOrderConfirm(), out _, out _, token.ToString());
            Assert.NotNull(result);
        }

         [Test] [Ignore("")]
        public void AllocSeatsAutomaticAsyncTest()
        {
            var token = Guid.NewGuid();
            var result = _moulinRougeAdapter.AllocSeatsAutomaticAsync(PrepareCriteria(), token.ToString());
            Assert.NotNull(result);
        }

         [Test] [Ignore("")]
        public void TempOrderGetSendingFeesAsyncTest()
        {
            var token = Guid.NewGuid();
            var result = _moulinRougeAdapter.TempOrderGetSendingFeesAsync("1234", token.ToString());
            Assert.NotNull(result);
        }

         [Test] 
        public void GetTempOrderGetDetailAsyncTest()
        {
            var token = "9086dfbb-8228-4051-b545-087b7b63c807";
            var result = _moulinRougeAdapter.GetOrderEticketAsync("1178713","9086dfbb-8228-4051-b545-087b7b63c807", token);
            Assert.NotNull(result);
        }

        #region Private Methods

        /// <summary>
        /// Preparing criteria for GetAvailabilityAndPrice
        /// </summary>
        /// <returns>Criteria</returns>
        private MoulinRougeCriteria PrepareCriteria()
        {
            var criteria = new MoulinRougeCriteria
            {
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {   PassengerType.Adult,
                        1
                    },
                    {   PassengerType.Child,
                        0
                    }
                },
                CheckinDate = new DateTime(2018, 10, 03),
                CheckoutDate = new DateTime(2018, 10, 09)
            };
            return criteria;
        }

        private MoulinRougeSelectedProduct PrepareCriteriaForOrderConfirm()
        {
            var datePriceAndAvailability = new Dictionary<DateTime, PriceAndAvailability>
            {
                {
                    new DateTime(2019, 01, 08),
                    new MoulinRougePriceAndAvailability()
                    {
                        IsSelected = true,
                        MoulinRouge = new APIContextMoulinRouge()
                        {
                            TemporaryOrderId = "295822",
                            TemporaryOrderRowId = "325490",
                            Amount = 220.0000M
                        }
                    }
                }
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

            var activity = new ActivityOption()
            {
                Id = 138927,

                SellPrice = new Price()
                {
                    Amount = 195.05M,
                    DatePriceAndAvailabilty = datePriceAndAvailability
                },
                IsSelected = true,
                Customers = new List<Customer> { customer }
            };

            var selectedProduct = new MoulinRougeSelectedProduct()
            {
                ProductOptions = new List<ProductOption> { activity },
                FullName = "test test",
                FirstName = "test",
                IsangoBookingReferenceNumber = "SGI622443",
            };

            return selectedProduct;
        }

        #endregion Private Methods
    }
}