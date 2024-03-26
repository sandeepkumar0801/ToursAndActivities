using Autofac;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.GoldenTours;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.GoldenTours;
using System;
using System.Collections.Generic;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class GoldenToursAdapterTest : BaseTest
    {
        private IGoldenToursAdapter _goldenToursAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _goldenToursAdapter = scope.Resolve<IGoldenToursAdapter>();
            }
        }

        [Test]
        public void ProductDetailTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var criteria = new GoldenToursCriteria
            {
                SupplierOptionCodes = new List<string> { "34" },
                CheckinDate = DateTime.Today.AddDays(10),
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    { PassengerType.Adult, 2 },
                    { PassengerType.Child, 2 }
                },
                PassengerMappings = PrepareDummyPassengerMapping()
            };
            var result = _goldenToursAdapter.GetProductDetails(criteria, tokenId);
            Assert.IsNotNull(result);
        }

        [Test]
        public void ProductDetailAsyncTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var criteria = new GoldenToursCriteria
            {
                SupplierOptionCodes = new List<string> { "4694" }
            };
            var result = _goldenToursAdapter.GetProductDetailsAsync(criteria, tokenId).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetAvailabilityTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var criteria = new GoldenToursCriteria
            {
                SupplierOptionCode = "4694",
                CheckinDate = DateTime.Today.AddDays(10)
            };
            var result = _goldenToursAdapter.GetAvailability(criteria, tokenId);
            Assert.IsNotNull(result);

            criteria = null;
            result = _goldenToursAdapter.GetAvailability(criteria, tokenId);
            Assert.IsNull(result);
        }

        [Test]
        public void GetAvailabilityAsyncTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var criteria = new GoldenToursCriteria
            {
                SupplierOptionCode = "4694",
                CheckinDate = DateTime.Today.AddDays(10)
            };
            var result = _goldenToursAdapter.GetAvailabilityAsync(criteria, tokenId).Result;
            Assert.IsNotNull(result);

            criteria = null;
            result = _goldenToursAdapter.GetAvailabilityAsync(criteria, tokenId).Result;
            Assert.IsNull(result);
        }

        [Test]
        public void GetProductDatesTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var criteria = new GoldenToursCriteria
            {
                SupplierOptionCode = "4694",
                CheckinDate = DateTime.Today.AddDays(10),
                CheckoutDate = DateTime.Today.AddDays(13)
            };
            var result = _goldenToursAdapter.GetProductDates(criteria, tokenId);
            Assert.IsNotNull(result);

            criteria = null;
            result = _goldenToursAdapter.GetProductDates(criteria, tokenId);
            Assert.IsNull(result);
        }

        [Test]
        public void GetProductDatesAsyncTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var criteria = new GoldenToursCriteria
            {
                SupplierOptionCode = "4694",
                CheckinDate = DateTime.Today.AddDays(10),
                CheckoutDate = DateTime.Today.AddDays(13)
            };
            var result = _goldenToursAdapter.GetProductDatesAsync(criteria, tokenId).Result;
            Assert.IsNotNull(result);

            criteria = null;
            result = _goldenToursAdapter.GetProductDatesAsync(criteria, tokenId).Result;
            Assert.IsNull(result);
        }

        [Test]
        public void GetBookingDatesTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var criteria = new GoldenToursCriteria
            {
                SupplierOptionCode = "4694",
                CheckinDate = DateTime.Today.AddDays(10),
                CheckoutDate = DateTime.Today.AddDays(13)
            };
            var result = _goldenToursAdapter.GetBookingDates(criteria, tokenId);
            Assert.IsNotNull(result);

            criteria = null;
            result = _goldenToursAdapter.GetBookingDates(criteria, tokenId);
            Assert.IsNull(result);
        }

        [Test]
        public void GetBookingDatesAsyncTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var criteria = new GoldenToursCriteria
            {
                SupplierOptionCode = "4694",
                CheckinDate = DateTime.Today.AddDays(10),
                CheckoutDate = DateTime.Today.AddDays(13)
            };
            var result = _goldenToursAdapter.GetBookingDatesAsync(criteria, tokenId).Result;
            Assert.IsNotNull(result);

            criteria = null;
            result = _goldenToursAdapter.GetBookingDatesAsync(criteria, tokenId).Result;
            Assert.IsNull(result);
        }

        [Test]
        public void GetPickupPointsTest()
        {
            var productId = "5310";
            var tokenId = Guid.NewGuid().ToString();
            var result = _goldenToursAdapter.GetPickupPoints(productId, tokenId);
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetPickupPointsAsyncTest()
        {
            var productId = "5310";
            var tokenId = Guid.NewGuid().ToString();
            var result = _goldenToursAdapter.GetPickupPointsAsync(productId, tokenId).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("")]
        public void CreateBookingTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var goldenToursSelectedProducts = new List<SelectedProduct>
            {
                PrepareSelectedProduct(),
                PrepareSelectedProduct("290")
            };
            var result = _goldenToursAdapter.CreateBooking(goldenToursSelectedProducts, tokenId, out string apiRequest, out string apiResponse);
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("")]
        public void CreateBookingAsyncTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var goldenToursSelectedProducts = new List<SelectedProduct>
            {
                PrepareSelectedProduct(),
                PrepareSelectedProduct("290")
            };
            var result = _goldenToursAdapter.CreateBookingAsync(goldenToursSelectedProducts, tokenId).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetPriceAvailabilityForDumpingTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var checkinDate = DateTime.Today.AddDays(10);
            var criteria = new GoldenToursCriteria
            {
                SupplierOptionCode = "31",
                CheckinDate = checkinDate,
                CheckoutDate = checkinDate.AddDays(90),
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    { PassengerType.Adult, 1 }
                },
                PassengerMappings = PrepareDummyPassengerMapping()
            };
            var result = _goldenToursAdapter.GetPriceAvailabilityForDumping(criteria, tokenId);
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetProductDetailsResponseTest()
        {
            var tokenId = Guid.NewGuid().ToString();
            var checkinDate = DateTime.Today.AddDays(2);
            var criteria = new GoldenToursCriteria
            {
                SupplierOptionCode = "31",
                CheckinDate = checkinDate,
                CheckoutDate = checkinDate
            };
            var result = _goldenToursAdapter.GetProductDetailsResponse(criteria, tokenId);
            Assert.IsNotNull(result);
        }

        #region Temporary Test Cases

        //[Test]
        //public void GetPaxTypes()
        //{
        //    var productIdsString = "3442, 79, 2984, 2936, 8, 2741, 2742, 2668, 1345, 2437, 61, 23, 3, 14, 290, 94, 93, 34, 5292, 5310, 5601, 129, 3841, 31, 5894, 1002 ";
        //    var productIds = productIdsString.Split(',');

        //    var tokenId = Guid.NewGuid().ToString();
        //    var productWisePaxTypes = new Dictionary<string, Dictionary<string, string>>();
        //    var commonPaxTypes = new Dictionary<string, string>();

        //    foreach (var productId in productIds)
        //    {
        //        var criteria = new GoldenToursCriteria
        //        {
        //            ProductId = Int32.Parse(productId)
        //        };
        //        var productDetails = _goldenToursAdapter.GetProductDetails(criteria, tokenId);
        //        var pricePeriods = productDetails?.Product?.Priceperiods?.Period;
        //        if (pricePeriods == null) continue;
        //        var paxTypes = new Dictionary<string, string>();

        //        foreach (var period in pricePeriods)
        //        {
        //            if (period?.Priceunits?.Unit == null) continue;
        //            foreach (var unit in period?.Priceunits?.Unit)
        //            {
        //                if (unit != null && !paxTypes.Keys.Contains(unit.Id))
        //                    paxTypes.Add(unit.Id, unit.Title);

        //                if (!commonPaxTypes.Keys.Contains(unit.Id))
        //                    commonPaxTypes.Add(unit.Id, unit.Title);
        //            }
        //        }
        //        productWisePaxTypes.Add(productId, paxTypes);
        //    }

        //    var json = Util.SerializeDeSerializeHelper.Serialize(commonPaxTypes);
        //    Assert.IsNotNull(productWisePaxTypes);
        //}

        //[Test]
        //public void GetPickup()
        //{
        //    var productIdsString = "3442, 79, 2984, 2936, 8, 2741, 2742, 2668, 1345, 2437, 61, 23, 3, 14, 290, 94, 93, 34, 5292, 5310, 5601, 129, 3841, 31, 5894, 1002 ";
        //    var productIds = productIdsString.Split(',');
        //    var tokenId = Guid.NewGuid().ToString();
        //    var pickup = new Dictionary<string, List<Pickup>>();

        //    foreach (var productId in productIds)
        //    {
        //        var criteria = new GoldenToursCriteria
        //        {
        //            SupplierOptionCode = productId
        //        };
        //        var pickups = _goldenToursAdapter.GetPickupPoints(criteria, tokenId);
        //        var pickupAvailable = pickups?.Productpickups?.Pickups?.Pickup;
        //        if(!pickup.Keys.Contains(productId))
        //            pickup.Add(productId, pickupAvailable);
        //    }
        //    Assert.IsNotNull(pickup);
        //}

        //[Test]
        //public void GetProductTypesTest()
        //{
        //    var passengerMappings = PrepareDummyPassengerMapping();
        //    var codes = "2984, 2936, 8, 2741, 2742, 2668, 2437, 61, 23, 3, 14, 94, 93, 34, 5292, 5310, 3841".Split(',');
        //    var dict = new Dictionary<string, string>();
        //    foreach (var code in codes)
        //    {
        //        var criteria = new GoldenToursCriteria
        //        {
        //            SupplierOptionCode = code,
        //            CheckinDate = new DateTime(2019, 05, 27),//DateTime.Today.AddDays(2),
        //            NoOfPassengers = new Dictionary<PassengerType, int>
        //            {
        //                { PassengerType.Adult, 2 },
        //                { PassengerType.Child, 2 }
        //            },
        //            PassengerMappings = passengerMappings
        //        };
        //        var productDetails = _goldenToursAdapter.GetProductDetails(criteria, "lkd");
        //        var productType = ((ActivityOption)productDetails.FirstOrDefault())?.ProductType;
        //        if (!dict.Keys.Contains(code))
        //            dict.Add(code, productType);
        //    }
        //    Assert.IsNotNull(dict);
        //}

        //[Test]
        //public void BookingIntegrationTest()
        //{
        //    var apiRequest = "";
        //    var apiResponse = "";

        //    var tokenId = "BookingIntegration1.0";
        //    var passengerMappings = PrepareDummyPassengerMapping();
        //    var codes = "79, 1345, 290, 5601, 129, 31, 5894, 1002".Split(',');
        //    foreach (var code in codes)
        //    {
        //        var criteria = new GoldenToursCriteria
        //        {
        //            SupplierOptionCode = "34",
        //            CheckinDate = new DateTime(2019, 05, 27), //DateTime.Today.AddDays(2),
        //            NoOfPassengers = new Dictionary<PassengerType, int>
        //            {
        //                {PassengerType.Adult, 2},
        //                {PassengerType.Child, 2}
        //            },
        //            PassengerMappings = passengerMappings
        //        };
        //        var productOption = (ActivityOption)_goldenToursAdapter.GetProductDetails(criteria, tokenId).FirstOrDefault();
        //        if (productOption == null) continue;

        //        var goldenToursSelectedProduct = PrepareSelectedProduct(productOption.SupplierOptionCode, productOption.ScheduleId);
        //        var result = _goldenToursAdapter.CreateBooking(goldenToursSelectedProduct, tokenId, out apiRequest, out apiResponse);
        //    }
        //}

        //[Test]
        //public void GetPaxTypes()
        //{
        //    var productIdsString = "3442, 87, 88, 89, 90, 79, 5049, 2984, 2936, 8, 1345, 2437, 61, 23, 3, 14, 94, 93, 34, 5292, 5310, 5461, 5323, 5325, 5460, 3841, 5399, 5360, 5359, 5361, 92, 91, 31, 5894, 1002, 2388, 2388, 2389, 2389, 2745, 2741, 2741, 2742, 2742, 2743, 2743, 2744, 2746, 2746, 2747, 2747, 3169, 2669, 4031, 4032, 4101, 4102, 4103, 4366, 5351, 2668, 2668, 290, 5601, 129, 2678, 4503, 834, 5087, 288";
        //    var productIds = productIdsString.Split(',');

        //    var tokenId = Guid.NewGuid().ToString();
        //    var productWisePaxTypes = new Dictionary<string, Dictionary<string, string>>();
        //    var commonPaxTypes = new Dictionary<string, string>();
        //    var productTypes = new Dictionary<string, string>();
        //    var unavailableProducts = new List<string>();
        //    var availableProducts = new List<string>();

        //    foreach (var productId in productIds)
        //    {
        //        var criteria = new GoldenToursCriteria
        //        {
        //            SupplierOptionCode = productId
        //        };
        //        var response = _goldenToursAdapter.GetProductDetailsResponse(criteria, tokenId);

        //        if (response == null)
        //        {
        //            unavailableProducts.Add(productId);
        //            continue;
        //        }

        //        var productDetails = response?.ProductDetails;
        //        if (productDetails != null)
        //        {
        //            foreach (var productDetail in productDetails)
        //            {
        //                if (productDetail == null) continue;
        //                if (!productTypes.Keys.Contains(productId))
        //                    productTypes.Add(productId, productDetail.ProductType);
        //                availableProducts.Add(productId);
        //            }
        //        }

        //        var ageGroups = response?.AgeGroups;
        //        if (ageGroups != null)
        //        {
        //            var paxTypes = new Dictionary<string, string>();
        //            foreach (var ageGroup in ageGroups)
        //            {
        //                if (!paxTypes.Keys.Contains(ageGroup.UnitID))
        //                    paxTypes.Add(ageGroup.UnitID, ageGroup.UnitTitle);

        //                if (!commonPaxTypes.Keys.Contains(ageGroup.UnitID))
        //                    commonPaxTypes.Add(ageGroup.UnitID, ageGroup.UnitTitle);
        //            }
        //            if (!productWisePaxTypes.Keys.Contains(productId))
        //                productWisePaxTypes.Add(productId, paxTypes);
        //        }
        //    }

        //    var json = Util.SerializeDeSerializeHelper.Serialize(commonPaxTypes);
        //    Assert.IsNotNull(productWisePaxTypes);
        //}

        #endregion Temporary Test Cases

        #region Private Methods

        private GoldenToursSelectedProduct PrepareSelectedProduct(string productId = "31", string scheduleId = "843")
        {
            var goldenToursSelectedProduct = new GoldenToursSelectedProduct
            {
                ProductId = 31,
                City = "London",
                Country = "GB",
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
                            StartDate = new DateTime(2019, 12, 10),
                            NoOfPassengers = new Dictionary<PassengerType, int>
                            {
                                { PassengerType.Adult, 1 }
                            }
                        },
                        IsSelected = true,
                        SupplierOptionCode = productId,
                        ScheduleId = scheduleId
                    }
                }
            };
            return goldenToursSelectedProduct;
        }

        private List<PassengerMapping> PrepareDummyPassengerMapping()
        {
            // Hardcoding as we cannot call the service level call from here and this can be accessed through DB.
            var passengerMappings = new List<PassengerMapping>
            {
                GetDummyPassengerMapping(1, 1),
                GetDummyPassengerMapping(2, 2),
                GetDummyPassengerMapping(10, 3),
                GetDummyPassengerMapping(11, 3),
                GetDummyPassengerMapping(12, 9),
                GetDummyPassengerMapping(9, 38),
                GetDummyPassengerMapping(13, 72),
                GetDummyPassengerMapping(14, 80)
            };
            return passengerMappings;
        }

        private PassengerMapping GetDummyPassengerMapping(int passengerTypeId, int supplierPassengerTypeId)
        {
            return new PassengerMapping
            {
                PassengerTypeId = passengerTypeId,
                SupplierPassengerTypeId = supplierPassengerTypeId
            };
        }

        #endregion Private Methods
    }
}