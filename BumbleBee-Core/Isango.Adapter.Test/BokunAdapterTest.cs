using Autofac;
using Isango.Entities.Bokun;
using Isango.Entities.Enums;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.Bokun;
using ServiceAdapters.Bokun.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class BokunAdapterTest : BaseTest
    {
        private IBokunAdapter _bokunAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _bokunAdapter = scope.Resolve<IBokunAdapter>();
            }
        }

        /// <summary>
        /// Method to test the GetActivity method
        /// </summary>
        [Test]
        public void GetActivityTest()
        {
            var token = Guid.NewGuid();
            var criteria = new BokunCriteria
            {
                ActivityCode = "1272"
            };
            var result = _bokunAdapter.GetActivity(criteria.ActivityCode, token.ToString());
            Assert.IsNotNull(result);
            if (result.ActualId != null) Assert.AreEqual(result.ActualId.Value, 1272);

            criteria.ActivityCode = "0";
            result = _bokunAdapter.GetActivity(criteria.ActivityCode, token.ToString());
            Assert.IsNull(result);
        }

        /// <summary>
        /// Test method to test the GetActivityAsync method
        /// </summary>
        [Test]
        public async Task GetActivityAsyncTest()
        {
            var token = Guid.NewGuid();
            var criteria = new BokunCriteria
            {
                ActivityCode = "1272"
            };
            var result = await _bokunAdapter.GetActivityAsync(criteria, token.ToString());
            Assert.IsNotNull(result);
            if (result.ActualId != null) Assert.AreEqual(result.ActualId.Value, 1272);

            criteria.ActivityCode = "0";
            result = await _bokunAdapter.GetActivityAsync(criteria, token.ToString());
            Assert.IsNull(result);
        }

        /// <summary>
        /// Method to test the CheckAvailabilities method
        /// </summary>
        [Test]
        public void CheckAvailabilitiesTest()
        {
            var token = Guid.NewGuid();
            var criteria = new BokunCriteria
            {
                ActivityCode = Convert.ToString(1269),
                CheckinDate = DateTime.Today.Date,
                CheckoutDate = DateTime.Today.Date.AddDays(1),
                NoOfPassengers = new Dictionary<PassengerType, int> { { PassengerType.Adult, 1 } },
                PriceCategoryIdMapping = new List<PriceCategory>
                {
                    new PriceCategory
                    {
                        PriceCategoryId = 1691, ServiceOptionCode = 1269, Title = Constant.Adults
                    }
                },
                FactSheetIds = new List<int> { 1269 }
            };
            var result = _bokunAdapter.CheckAvailabilities(criteria, null, token.ToString());
            Assert.IsNotNull(result);

            // Passing invalid Id to test the null response
            criteria.ActivityCode = "0";
            criteria.FactSheetIds = new List<int> { 0 };
            result = _bokunAdapter.CheckAvailabilities(criteria, null, token.ToString());
            Assert.IsNull(result);
        }

        /// <summary>
        /// Method to test the CheckAvailabilities method
        /// </summary>
        [Test]
        public void CheckAvailabilitiesForDumpingAppTest()
        {
            var token = Guid.NewGuid();
            var criteria = new BokunCriteria
            {
                ActivityCode = Convert.ToString(1269),
                CheckinDate = DateTime.Today.Date,
                CheckoutDate = DateTime.Today.Date.AddDays(1),
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {   PassengerType.Adult,
                        1
                    }
                },
                PriceCategoryIdMapping = new List<PriceCategory>
                {
                    new PriceCategory
                    {
                        PriceCategoryId = 1691,
                        ServiceOptionCode = 1269,
                        Title = Constant.Adults
                    }
                },
                ActivityId = 1269
            };

            var activity = new Isango.Entities.Activities.Activity();
            var result = _bokunAdapter.CheckAvailabilitiesForDumpingApp(criteria, activity, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the CheckAvailabilitiesAsync method
        /// </summary>
        [Test]
        public async Task CheckAvailabilitiesAsyncTest()
        {
            var token = Guid.NewGuid();
            var criteria = new BokunCriteria
            {
                ActivityCode = Convert.ToString(1269),
                CheckinDate = DateTime.Today.Date,
                CheckoutDate = DateTime.Today.Date.AddDays(1),
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {   PassengerType.Adult,
                        1
                    }
                },
                PriceCategoryIdMapping = new List<PriceCategory>
                {
                    new PriceCategory
                    {
                        PriceCategoryId = 1691,
                        ServiceOptionCode = 1269,
                        Title = Constant.Adults
                    }
                },
                FactSheetIds = new List<int> { 1269 }
            };
            var result = await _bokunAdapter.CheckAvailabilitiesAsync(criteria, null, token.ToString());
            Assert.IsNotNull(result);

            // Passing invalid Id to test the null response
            criteria.ActivityCode = "0";
            criteria.FactSheetIds = new List<int> { 0 };
            result = await _bokunAdapter.CheckAvailabilitiesAsync(criteria, null, token.ToString());
            Assert.IsNull(result);
        }
        /// <summary>
        /// Method to test the CheckoutOption method
        /// </summary>
        [Test]
        public void CheckoutOptionTest()
        {
            var token = Guid.NewGuid();
            var selectedProduct = new BokunSelectedProduct
            {
                Id = 1269,
                DateStart = DateTime.Today.Date,
                PricingCategoryIds = new List<int> { 1691 },
                StartTimeId = 2697
            };
            var result = _bokunAdapter.CheckoutOptions(selectedProduct, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the CheckoutOptionAsync method
        /// </summary>
        [Test]
        public async Task CheckoutOptionAsyncTest()
        {
            var token = Guid.NewGuid();
            var selectedProduct = new BokunSelectedProduct
            {
                Id = 1269,
                DateStart = DateTime.Today.Date,
                PricingCategoryIds = new List<int> { 1691 },
                StartTimeId = 2697
            };
            var result = await _bokunAdapter.CheckoutOptionsAsync(selectedProduct, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the GetBooking method
        /// </summary>
        [Test]
        public void GetBookingTest()
        {
            var token = Guid.NewGuid();
            var result = _bokunAdapter.GetBooking("ISA-28985", token.ToString());
            Assert.IsNotNull(result);

            // To test the null response passing Id 0,
            result = _bokunAdapter.GetBooking("test", token.ToString());
            Assert.IsNull(result);
        }

        /// <summary>
        /// Method to test the GetBookingAsync method
        /// </summary>
        [Test]
        public async Task GetBookingAsyncTest()
        {
            var token = Guid.NewGuid();
            var result = await _bokunAdapter.GetBookingAsync("ISA-28985", token.ToString());
            Assert.IsNotNull(result);

            // Passing dummy confirmation code to test the null response
            result = await _bokunAdapter.GetBookingAsync("test", token.ToString());
            Assert.IsNull(result);
        }

        /// <summary>
        /// Method to test the EditBooking method
        /// </summary>
        [Test]
        public void EditBookingTest()
        {
            var token = Guid.NewGuid();
            var selectedProduct = new BokunSelectedProduct
            {
                EditType = "RemoveParticipantAction",
                Id = 85322,
                PricingCategoryIds = new List<int> { 169523 }
            };
            var result = _bokunAdapter.EditBooking(selectedProduct, token.ToString());
            Assert.AreEqual(result, "FINISHED");

            // Passing dummy Id to test the neagtive response
            selectedProduct.Id = 0;
            result = _bokunAdapter.EditBooking(selectedProduct, token.ToString());
            Assert.IsNull(result);
        }

        /// <summary>
        /// Method to test the EditBookingAsync method
        /// </summary>
        [Test]
        public async Task EditBookingAsyncTest()
        {
            var token = Guid.NewGuid();
            var selectedProduct = new BokunSelectedProduct
            {
                EditType = "RemoveParticipantAction",
                Id = 85322,
                PricingCategoryIds = new List<int> { 169523 }
            };
            var result = await _bokunAdapter.EditBookingAsync(selectedProduct, token.ToString());
            Assert.AreEqual(result, "FINISHED");

            // Passing dummy Id to test the neagtive response
            selectedProduct.Id = 0;
            result = await _bokunAdapter.EditBookingAsync(selectedProduct, token.ToString());
            Assert.IsNull(result);
        }

        /// <summary>
        /// Method to test the CancelBooking method
        /// </summary>
        [Test]
        public void CancelBookingTest()
        {
            var token = Guid.NewGuid();
            var result = _bokunAdapter.CancelBooking("ISA-29893", token.ToString());
            Assert.IsTrue(result);

            // Passing dummy confirmation code to test the neagtive response
            result = _bokunAdapter.CancelBooking("test", token.ToString());
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Method to test the CancelBookingAsync method
        /// </summary>
        [Test]
        public async Task CancelBookingAsyncTest()
        {
            var token = Guid.NewGuid();
            var result = await _bokunAdapter.CancelBookingAsync("ISA-29894", token.ToString());
            Assert.IsTrue(result);

            // Passing dummy confirmation code to test the neagtive response
            result = await _bokunAdapter.CancelBookingAsync("test", token.ToString());
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Method to test the SubmitCheckout method
        /// </summary>
        [Test]
        public void SubmitCheckoutTest()
        {
            var token = Guid.NewGuid();
            var selectedProduct = GetSelectedProduct();
            var result = _bokunAdapter.SubmitCheckout(selectedProduct, token.ToString());
            Assert.IsNotNull(result);

            // Passing invalid Id to check null response
            selectedProduct.FactsheetId = 0;
            result = _bokunAdapter.SubmitCheckout(selectedProduct, token.ToString());
            Assert.IsNull(result);
        }

        /// <summary>
        /// Method to test the SubmitCheckoutAsync method
        /// </summary>
        [Test]
        public async Task SubmitCheckoutAsyncTest()
        {
            var token = Guid.NewGuid();
            var selectedProduct = GetSelectedProduct();
            var result = await _bokunAdapter.SubmitCheckoutAsync(selectedProduct, token.ToString());
            Assert.IsNotNull(result);

            // Passing invalid Id to check null response
            selectedProduct.FactsheetId = 0;
            result = await _bokunAdapter.SubmitCheckoutAsync(selectedProduct, token.ToString());
            Assert.IsNull(result);
        }

        #region Private Methods

        private BokunSelectedProduct GetSelectedProduct()
        {
            var selectedProduct = new BokunSelectedProduct
            {
                Id = 1269,
                FactsheetId = 1269,
                StartTimeId = 2697,
                DateStart = DateTime.Today.Date,
                PricingCategoryIds = new List<int> { 1691 },
                Questions = new List<Question>()
            };
            var question = new Question
            {
                QuestionId = "firstName",
                Answers = new List<string>() { "John" },
                QuestionType = Constant.MainContactDetails
            };
            selectedProduct.Questions.Add(question);
            question = new Question
            {
                QuestionId = "lastName",
                Answers = new List<string>() { "Doe" },
                QuestionType = Constant.MainContactDetails
            };
            selectedProduct.Questions.Add(question);
            question = new Question
            {
                QuestionId = "email",
                Answers = new List<string>() { "John@email.com" },
                QuestionType = Constant.MainContactDetails
            };
            selectedProduct.Questions.Add(question);

            return selectedProduct;
        }

        #endregion Private Methods
    }
}