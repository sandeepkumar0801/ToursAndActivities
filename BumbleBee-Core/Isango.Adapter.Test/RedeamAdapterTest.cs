using Autofac;

using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Redeam;
using Isango.Register;

using NUnit.Framework;

using ServiceAdapters.Redeam;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isango.Adapter.Test
{
    public class RedeamAdapterTest : BaseTest
    {
        private IRedeamAdapter _redeamAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
           // var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _redeamAdapter = scope.Resolve<IRedeamAdapter>();
            }
        }

        /// <summary>
        /// Method to test the GetAvailabilities method
        /// </summary>
        [Test]
        public async Task GetAvailabilitiesTestAsync()
        {
            var token = Guid.NewGuid();
            var criteria = new RedeamCriteria
            {
                RateId = "90926134-f811-4f4e-a2bc-a3083e4f257c",
                SupplierId = "1ab0c295-1d16-4b5e-8719-9e7eeb13b3c6",
                //"603db15d-33b4-43ce-ab46-f81c303c64d1", //"fc49b925-6942-4df8-954b-ed7df10adf7e",
                ProductId = "f591d348-f702-453b-8a3f-5bdfe456fa90",
                //"0740b61b-89e2-49e8-9bfc-c8b8ed8b0a8e", //"02f0c6cb-77ae-4fcc-8f4d-99bc0c3bee18",
                RateIds = new List<string>
                {
                    "90926134-f811-4f4e-a2bc-a3083e4f257c"
                },
                CheckinDate = DateTime.Today,
                CheckoutDate = DateTime.Today.AddDays(1),
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Adult, 2},
                    {PassengerType.Child, 2}
                },
                RateIdAndType = new Dictionary<string, string> { { "90926134-f811-4f4e-a2bc-a3083e4f257c", "RESERVED" } }
            };
            var result = await _redeamAdapter.GetAvailabilities(criteria, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the GetRates method
        /// </summary>
        [Test]
        public async Task GetRatesTestAsync()
        {
            var token = Guid.NewGuid();
            var criteria = new RedeamCriteria
            {
                SupplierId = "1ab0c295-1d16-4b5e-8719-9e7eeb13b3c6",
                ProductId = "f591d348-f702-453b-8a3f-5bdfe456fa90"
            };
            var result = await _redeamAdapter.GetRates(criteria, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the GetSuppliers method
        /// </summary>
        [Test]
        public async Task GetSuppliersTestAsync()
        {
            var token = Guid.NewGuid();
            var result = await _redeamAdapter.GetSuppliers(token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the GetProducts method
        /// </summary>
        [Test]
        public async Task GetProductsTestAsync()
        {
            var token = Guid.NewGuid();
            var criteria = new RedeamCriteria
            {
                SupplierId = "43c9fa93-5ba8-4b0f-9dd8-af8b9b302cb5"
            };
            var result = await _redeamAdapter.GetProducts(criteria, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the GetRatesWrapper method
        /// </summary>
        [Test]
        public async Task GetRatesWrapperTestAsync()
        {
            var token = Guid.NewGuid();
            var criteria = new RedeamCriteria
            {
                SupplierId = "43c9fa93-5ba8-4b0f-9dd8-af8b9b302cb5",
                ProductId = "ce4d035a-4ef2-4269-b5bd-7ba8e5679275"
            };
            var result = await _redeamAdapter.GetRatesWrapper(criteria, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the CreateHold method
        /// </summary>
        [Test]
        public async Task CreateHoldTestAsync()
        {
            var token = Guid.NewGuid();
            var selectedProduct = new RedeamSelectedProduct
            {
                PriceId = new Dictionary<string, string> { { "ADULT", "5b1a2386-e413-4177-b754-186afa4fb16f" } },
                RateId = "a7e414c2-50ee-4a30-a4a9-2631ff8efbd3",
                SupplierId = "43c9fa93-5ba8-4b0f-9dd8-af8b9b302cb5",
                ProductOptions = new List<ProductOption>
                {
                    new ActivityOption
                    {
                        Customers = new List<Customer>
                        {
                            new Customer
                            {
                                FirstName = "Piyush",
                                LastName = "Yadav",
                                Email = "abc@email.com",
                                Title = "Mr.",
                                IsLeadCustomer = true
                            }
                        },
                        TravelInfo = new TravelInfo
                        {
                            StartDate = new DateTime(2019, 08, 20),
                            NoOfPassengers = new Dictionary<PassengerType, int>
                            {
                                { PassengerType.Adult, 2 },
                                { PassengerType.Senior, 2 }
                            }
                        },
                        IsSelected = true
                    }
                }
            };
            var result = await _redeamAdapter.CreateHold(selectedProduct, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the DeleteHold method
        /// </summary>
        [Test]
        public async Task DeleteHoldTestAsync()
        {
            var token = Guid.NewGuid();
            var holdIds = new List<string>
            {
                "1bd92dd5-c630-48f8-8ca7-b86feb190cdd",
                "78ab1116-e4f2-4a65-b7c6-ad193284d797"
            };
            var result = await _redeamAdapter.DeleteHold(holdIds, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the CreateBookingAsync method
        /// </summary>
        [Test]
        public async Task CreateBookingTestAsync()
        {
            var token = Guid.NewGuid();
            var selectedProduct = new RedeamSelectedProduct
            {
                HoldId = "5fd78809-4700-46d7-8386-3b8738117f4a",
                PriceId = new Dictionary<string, string>() { { "ADULT", "5b1a2386-e413-4177-b754-186afa4fb16f" } },
                RateId = "a7e414c2-50ee-4a30-a4a9-2631ff8efbd3",
                SupplierId = "43c9fa93-5ba8-4b0f-9dd8-af8b9b302cb5",
                ProductOptions = new List<ProductOption>
                {
                    new ActivityOption
                    {
                        Customers = new List<Customer>
                        {
                            new Customer
                            {
                                FirstName = "Piyush",
                                LastName = "Yadav",
                                Email = "abc@email.com",
                                Title = "Mr.",
                                IsLeadCustomer = true,
                                PassengerType = PassengerType.Adult
                            },
                            new Customer
                            {
                                FirstName = "abc",
                                LastName = "as",
                                Email = "abc@email.com",
                                Title = "Mr.",
                                IsLeadCustomer = false,
                                PassengerType = PassengerType.Adult
                            },
                            new Customer
                            {
                                FirstName = "wer",
                                LastName = "fbcfbg",
                                Email = "abc@email.com",
                                Title = "Mr.",
                                IsLeadCustomer = false,
                                PassengerType = PassengerType.Senior
                            },
                            new Customer
                            {
                                FirstName = "vbnvb",
                                LastName = "jklj",
                                Email = "abc@email.com",
                                Title = "Mr.",
                                IsLeadCustomer = false,
                                PassengerType = PassengerType.Senior
                            }
                        },
                        TravelInfo = new TravelInfo
                        {
                            StartDate = new DateTime(2019, 08, 25),
                            NoOfPassengers = new Dictionary<PassengerType, int>
                            {
                                { PassengerType.Adult, 2 },
                                { PassengerType.Senior, 2 }
                            }
                        },
                        IsSelected = true
                    }
                }
            };
            var result = await _redeamAdapter.CreateBooking(selectedProduct, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the CreateBooking method
        /// </summary>
        [Test]
        public void CreateBookingTest()
        {
            var token = Guid.NewGuid();
            var selectedProduct = new RedeamSelectedProduct
            {
                HoldId = "5fd78809-4700-46d7-8386-3b8738117f4a",
                PriceId = new Dictionary<string, string>() { { "ADULT", "5b1a2386-e413-4177-b754-186afa4fb16f" } },
                RateId = "a7e414c2-50ee-4a30-a4a9-2631ff8efbd3",
                SupplierId = "43c9fa93-5ba8-4b0f-9dd8-af8b9b302cb5",
                ProductOptions = new List<ProductOption>
                {
                    new ActivityOption
                    {
                        Customers = new List<Customer>
                        {
                            new Customer
                            {
                                FirstName = "Piyush",
                                LastName = "Yadav",
                                Email = "abc@email.com",
                                Title = "Mr.",
                                IsLeadCustomer = true,
                                PassengerType = PassengerType.Adult
                            },
                            new Customer
                            {
                                FirstName = "abc",
                                LastName = "as",
                                Email = "abc@email.com",
                                Title = "Mr.",
                                IsLeadCustomer = false,
                                PassengerType = PassengerType.Adult
                            },
                            new Customer
                            {
                                FirstName = "wer",
                                LastName = "fbcfbg",
                                Email = "abc@email.com",
                                Title = "Mr.",
                                IsLeadCustomer = false,
                                PassengerType = PassengerType.Senior
                            },
                            new Customer
                            {
                                FirstName = "vbnvb",
                                LastName = "jklj",
                                Email = "abc@email.com",
                                Title = "Mr.",
                                IsLeadCustomer = false,
                                PassengerType = PassengerType.Senior
                            }
                        },
                        TravelInfo = new TravelInfo
                        {
                            StartDate = new DateTime(2019, 08, 25),
                            NoOfPassengers = new Dictionary<PassengerType, int>
                            {
                                { PassengerType.Adult, 2 },
                                { PassengerType.Senior, 2 }
                            }
                        },
                        IsSelected = true
                    }
                }
            };
            var result = _redeamAdapter.CreateBooking(selectedProduct, token.ToString(), out var apiRequest, out var apiResponse);
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the CancelBooking method
        /// </summary>
        [Test]
        public async Task CancelBookingTestAsync()
        {
            var token = Guid.NewGuid();
            var bookingReferenceNumbers = new List<string>
            {
                "17a50737-7b05-4646-9701-8768496fce80"
            };
            var result = await _redeamAdapter.CancelBooking(bookingReferenceNumbers, token.ToString());
            Assert.IsNotNull(result);
        }

        #region Unused Methods Test Cases

        /// <summary>
        /// Method to test the GetSingleRate method
        /// </summary>
        [Test]
        public void GetSingleRateTest()
        {
            var token = Guid.NewGuid();
            var criteria = new RedeamCriteria
            {
                SupplierId = "43c9fa93-5ba8-4b0f-9dd8-af8b9b302cb5",
                ProductId = "ce4d035a-4ef2-4269-b5bd-7ba8e5679275",
                RateId = "a7e414c2-50ee-4a30-a4a9-2631ff8efbd3"
            };
            var result = _redeamAdapter.GetSingleRate(criteria, token.ToString());
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the GetSingleAvailability method
        /// </summary>
        [Test]
        [Ignore("Do not have proper input data")]
        public void GetSingleAvailabilityTest()
        {
            var token = Guid.NewGuid();
            var criteria = new RedeamCriteria
            {
                SupplierId = "1ab0c295-1d16-4b5e-8719-9e7eeb13b3c6",
                //"603db15d-33b4-43ce-ab46-f81c303c64d1", //"fc49b925-6942-4df8-954b-ed7df10adf7e",
                ProductId = "f591d348-f702-453b-8a3f-5bdfe456fa90",
                //"0740b61b-89e2-49e8-9bfc-c8b8ed8b0a8e", //"02f0c6cb-77ae-4fcc-8f4d-99bc0c3bee18",
                RateIds = new List<string>
                {
                    "f591d348-f702-453b-8a3f-5bdfe456fa90"
                },
                CheckinDate = DateTime.Today,
                CheckoutDate = DateTime.Today.AddDays(1),
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Adult, 2},
                    {PassengerType.Child, 2}
                },
                Quantity = "4"
            };
            var result = _redeamAdapter.GetSingleAvailability(criteria, token.ToString());
            Assert.IsNotNull(result);
        }

        #endregion Unused Methods Test Cases
    }
}