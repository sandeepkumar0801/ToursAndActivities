using Autofac;
using Isango.Entities;
using Isango.Entities.Aot;
using Isango.Entities.Enums;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.Aot;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class AotAdapterTest : BaseTest
    {
        private IAotAdapter _aotAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _aotAdapter = scope.Resolve<IAotAdapter>();
            }
        }

        [Test]
        public void GetLocationsTest()
        {
            var getLocationRequest = new GetLocationsRequest { AgentId = "OTICOM", Password = "new069", LocationCode = "AU", LocationType = "C" };
            var token = Guid.NewGuid().ToString();
            var testResult = _aotAdapter.GetLocations(getLocationRequest, token);
            Assert.That(testResult.Location.Count > 0);
        }

        [Test]
        public void GetLocationsAsyncTest()
        {
            var getLocationRequest = new GetLocationsRequest { AgentId = "TestAgent", LocationCode = "AU", LocationType = "C" };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.GetLocationsAsync(getLocationRequest, token);
            result.Wait();
            Assert.That(result.Result.Location.Count > 0);
        }

        [Test]
        public void GetSupplierInformationTest()
        {
            var supplierInfoRequest = new SupplierInfoRequest
            {
                LocationCode = "TestLocationCode",
                LocationType = "TestLocationType",
                ServiceType = "AC",
                SupplierCodes = new SupplierCodes { SupplierCode = "QUASYD" }
            };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.GetSupplierInformation(supplierInfoRequest, token);
            Assert.That(result.Supplier.Count > 0);
        }

        [Test]
        public void GetSupplierInformationAsyncTest()
        {
            var supplierInfoRequest = new SupplierInfoRequest
            {
                LocationCode = "TestLocationCode",
                LocationType = "TestLocationType",
                ServiceType = "AC",
                SupplierCodes = new SupplierCodes { SupplierCode = "QUASYD" }
            };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.GetSupplierInformationAsync(supplierInfoRequest, token);
            result.Wait();
            Assert.That(result.Result.Supplier.Count > 0);
        }

        [Test]
        public void GetProductDetailsTest()
        {
            var optionGeneralInfoRequest = new OptionGeneralInfoRequest
            {
                Opts = new Opts { Opt = new List<string> { "DRWTEAATAUSEXTV" } },
            };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.GetProductDetails(optionGeneralInfoRequest, token);
            Assert.That(result.OptGeneralInfo.Count > 0);
        }

        [Test]
        public void GetProductDetailsAsyncTest()
        {
            var optionGeneralInfoRequest = new OptionGeneralInfoRequest
            {
                LocationCode = "AU",
                LocationType = "C",
                Opts = new Opts { Opt = new List<string> { "DRWTEAATAUSEXTV" } },
                StarRating = "TestRating"
            };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.GetProductDetailsAsync(optionGeneralInfoRequest, token);
            result.Wait();
            Assert.That(result.Result.OptGeneralInfo.Count > 0);
        }

        [Test]
        public void GetDetailedPricingAvailabilityTest()
        {
            var criteria = new AotCriteria
            {
                OptCode = new List<string> { "DRWTDAATAUSD5" },
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {
                        PassengerType.Adult,
                        2
                    },
                    {
                        PassengerType.Child,
                        1
                    }
                },
                CheckinDate = new DateTime(2019, 4, 16),
                CheckoutDate = new DateTime(2019, 4, 16),
            };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.GetDetailedPricingAvailability(criteria, token);
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetDetailedPricingAvailabilityAsyncTest()
        {
            var criteria = new AotCriteria
            {
                OptCode = new List<string> { "DRWTEAATAUSEXTV" },
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {
                        PassengerType.Adult,
                        2
                    },
                    {
                        PassengerType.Child,
                        1
                    }
                },
                CheckinDate = new DateTime(2018, 12, 20),
                CheckoutDate = new DateTime(2018, 12, 30),
            };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.GetDetailedPricingAvailabilityAsync(criteria, token);
            result.Wait();
            Assert.IsNull(result.Result);
        }

        [Test]
        public void GetBulkPricingAvailabilityDetailsTest()
        {
            var criteria = new AotCriteria
            {
                OptCode = new List<string> { "DRWTEAATAUSEXTV" },
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {
                        PassengerType.Adult,
                        2
                    },
                    {
                        PassengerType.Child,
                        1
                    }
                },
                CheckinDate = new DateTime(2018, 12, 20),
                CheckoutDate = new DateTime(2018, 12, 30),
            };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.GetBulkPricingAvailabilityDetails(criteria, token);
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetBulkPricingAvailabilityDetailsAsyncTest()
        {
            var criteria = new AotCriteria
            {
                OptCode = new List<string> { "DRWTEAATAUSEXTV" },
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {
                        PassengerType.Adult,
                        2
                    },
                    {
                        PassengerType.Child,
                        1
                    }
                },
                CheckinDate = new DateTime(2018, 12, 20),
                CheckoutDate = new DateTime(2018, 12, 30),
            };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.GetBulkPricingAvailabilityDetailsAsync(criteria, token);
            result.Wait();
            Assert.IsNotNull(result.Result);
        }

        [Test]
        public void CreateBookingTest()
        {
            var selectedProductList = new List<SelectedProduct>
            {
                new AotSelectedProduct
                {
                    Name = "Sameer",
                    ProductOptions = new List<ProductOption>
                    {
                        new ProductOption
                        {
                            TravelInfo = new TravelInfo
                            {
                                NoOfPassengers = new Dictionary<PassengerType, int>{
                                    {PassengerType.Adult, 1},
                                    {PassengerType.Child, 1}
                                },
                                StartDate = new DateTime(2019, 01, 12),
                                NumberOfNights = 5
                            },
                            Customers = new List<Customer>
                            {
                                new Customer
                                {
                                    FirstName = "TestUserAdult",
                                    PassengerType = PassengerType.Adult
                                },
                            },
                            IsSelected = true
                        }
                    }
                }
            };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.CreateBooking(selectedProductList, token, out string request, out string response);
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateBookingAsyncTest()
        {
            var selectedProducts = new AddBookingRequest
            {
                AgentId = "TestAgent",
                Name = "TestName",
                DeliveryMethod = "DRP",
                PaymentMethod = "INV",
                PaymentRef = "123XYZ",

                ContactDetails = new ContactDetails
                {
                    Title = "Mr",
                    Forename = "Forename",
                    Surname = "Smith",
                }
            };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.CreateBookingAsync(selectedProducts, token);
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetBookingDetailsTest()
        {
            var getBookingRequest = new GetBookingRequest() { 
             AgentId= "OTICOM", Password= "Tr@vel007", Ref= "AIIFJW8531"
            };
            var token = Guid.NewGuid().ToString();
            var result = _aotAdapter.GetBookingAsync(getBookingRequest, token).GetAwaiter().GetResult();
            Assert.IsNotNull(result);
        }
    }
}