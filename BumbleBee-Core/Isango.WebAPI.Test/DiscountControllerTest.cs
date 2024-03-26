using DiscountRuleEngine.Contracts;
using DiscountRuleEngine.Model;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Service.Contract;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using WebAPI.Controllers;
using WebAPI.Mapper;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;

namespace Isango.WebAPI.Test
{
    [TestFixture]
    public class DiscountControllerTest
    {
        private IDiscountEngine _discountEngine;
        private DiscountMapper _discountMapper;
        private ITableStorageOperation _TableStorageOperations;
        private IActivityService _activityServiceMock;
        private IMasterService _masterServiceMock;

        private DiscountController _discountController;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _discountEngine = Substitute.For<IDiscountEngine>();
            _TableStorageOperations = Substitute.For<ITableStorageOperation>();
            _activityServiceMock = Substitute.For<IActivityService>();
            _masterServiceMock = Substitute.For<IMasterService>();
            _discountMapper = new DiscountMapper(_TableStorageOperations, _activityServiceMock, _masterServiceMock);

            _discountController = new DiscountController(_discountEngine, _discountMapper);
        }

        [Test]
        public void ProcessDiscountTest()
        {
            var processDiscountRequest = new ProcessDiscountRequest
            {
                AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                TokenId = "32B1034A-3588-407F-9E15-A3B1D70E5595",
                LanguageCode = "en",
                CurrencyIsoCode = "USD",
                AvailabilityReferenceIds = new List<string>
                {
                    "32B1034A-3588-407F-9E15-A3B1D70E5595",
                    "32B1034A-3588-407F-9E15-A3B1D70E5596"
                },
                CustomerEmail = "test@gmail.com",
                DiscountCoupons = new List<string>
                {
                    "testCoupon"
                },
                UTMParameter = ""
            };

            var processDiscountResponse = new ProcessDiscountResponse
            {
                CurrencyIsoCode = "USD",
                SelectedProducts = new List<global::WebAPI.Models.ResponseModels.SelectedProduct>()
                {
                    new global::WebAPI.Models.ResponseModels.SelectedProduct
                    {
                        Id = 1001
                    }
                },
                FinalPrice = 120m,
                Messages = null,
                OriginalPrice = 150m,
                TotalDiscountedPrice = 30m
            };

            var discountModel = new DiscountModel
            {
                AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                UTMParameter = "",
                CustomerEmail = "test@gmail.com",
                Cart = new DiscountCart
                {
                    CurrencyIsoCode = "USD",
                    TotalPrice = 150m,
                    IsMultiSaveApplied = false,
                    Messages = null,
                    SelectedProducts = new List<DiscountSelectedProduct>
                    {
                        new DiscountSelectedProduct
                        {
                            Id = 1001
                        }
                    },
                    TotalNotSaleProductsPrice = 10m
                },
                Vouchers = new List<VoucherInfo>
                {
                    new VoucherInfo
                    {
                        DiscountType = DiscountType.Gift,
                        VoucherCode = "test"
                    }
                }
            };

            var discountCart = new DiscountCart
            {
                CurrencyIsoCode = "USD",
                SelectedProducts = new List<DiscountSelectedProduct>
                {
                    new DiscountSelectedProduct
                    {
                        Id = 1001
                    }
                },
                TotalPrice = 150m,
                IsMultiSaveApplied = false,
                TotalNotSaleProductsPrice = 10m,
                Messages = null
            };

            var availabilityData = new BaseAvailabilitiesEntity
            {
                ActivityId = 1000,
                ServiceOptionId = 1001,
                ApiType = 0
            };

            var activity = GetActivity();

            _discountMapper.MapDiscountRequest(processDiscountRequest);

            //_TableStorageOperations.RetrieveData<BaseAvailabilitiesEntity>("", "").ReturnsForAnyArgs(availabilityData);

            _activityServiceMock.GetActivityById(activity.ID, DateTime.Now, "en").ReturnsForAnyArgs(activity);

            _discountEngine.Process(discountModel).ReturnsForAnyArgs(discountCart);

            var result = _discountController.ProcessDiscount(processDiscountRequest) as
                ObjectResult;

            var Content = result?.Value as ProcessDiscountResponse;
            Assert.AreEqual(Content?.OriginalPrice, processDiscountResponse.OriginalPrice);
        }

        private Activity GetActivity()
        {
            return new Activity
            {
                Id = "1001",
                PriceTypeId = PriceTypeId.Margin,
                CancellationPolicy = "TestPolicy",
                ProductOptions = new List<ProductOption> { PrepareProductOption() },
                PassengerInfo = new List<Entities.Booking.PassengerInfo>
                {
                    new Entities.Booking.PassengerInfo
                    {
                        ActivityId = 21776, FromAge = 0, ToAge = 5, PassengerTypeId = 1,
                        IndependablePax = true, MinSize = 0, MaxSize = 10 , Label = "Adult", MeasurementDesc = "Adult >120cm"
                    }
                }
            };
        }

        private ProductOption PrepareProductOption()
        {
            return new ProductOption
            {
                Id = 135385,
                AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                Name = "test",
                Description = "test option",
                BasePrice = new Price
                {
                    Amount = 100,
                    Currency = new Currency
                    {
                        IsoCode = "AUD"
                    },
                    DatePriceAndAvailabilty = PrepareDatePriceAndAvailabilty()
                },
                CostPrice = new Price
                {
                    Amount = 100,
                    Currency = new Currency
                    {
                        IsoCode = "AUD"
                    },
                    DatePriceAndAvailabilty = PrepareDatePriceAndAvailabilty()
                },
                TravelInfo = PrepareTravelInfo(),
                IsSelected = true,
                Margin = new Margin
                {
                    Value = 5,
                    IsPercentage = true
                },
            };
        }

        private Dictionary<DateTime, PriceAndAvailability> PrepareDatePriceAndAvailabilty()
        {
            return new Dictionary<DateTime, PriceAndAvailability>
            {
                {
                    DateTime.Today,
                    new DefaultPriceAndAvailability()
                    {
                        TotalPrice = 100,
                        AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                        PricingUnits = new List<PricingUnit>()
                        {
                            new AdultPricingUnit
                            {
                                Price = 100
                            }
                        }
                    }
                },
                {
                    DateTime.Today.AddDays(1),
                    new DefaultPriceAndAvailability()
                    {
                        TotalPrice = 200,
                        AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                        PricingUnits = new List<PricingUnit>()
                        {
                            new AdultPricingUnit
                            {
                                Price = 200
                            }
                        }
                    }
                }
            };
        }

        private TravelInfo PrepareTravelInfo()
        {
            return new TravelInfo
            {
                StartDate = DateTime.Today,
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Adult, 1 }
                },
                Ages = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Child, 2 }
                }
            };
        }
    }
}