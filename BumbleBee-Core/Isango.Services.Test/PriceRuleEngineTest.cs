using Autofac;
using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Register;
using Isango.Service.Contract;
using NUnit.Framework;
using PriceRuleEngine;
using System;
using System.Collections.Generic;

namespace Isango.Services.Test
{
    [TestFixture]
    public class PriceRuleEngineTest : BaseTest
    {
        private PricingController _pricingController;
        private IPriceRuleEngineService _pricingRuleEngineService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
           // var container = Startup._builder.Build();
            using (var scope = _container.BeginLifetimeScope())
            {
                _pricingController = scope.Resolve<PricingController>();
                _pricingRuleEngineService = scope.Resolve<IPriceRuleEngineService>();
            }
        }

        /// <summary>
        /// Method to test the pricing controller of PriceRuleEngine
        /// </summary>
        [Test]
        [TestCase(12857, "0E03E273-5FE3-4C80-A80F-2100E9A26513", true, PriceTypeId.Commission)]
        [TestCase(136426, "0E03E273-5FE3-4C80-A80F-2100E9A26513", false, PriceTypeId.Margin)]
        [TestCase(12857, "b5287a34-9da6-40f1-b6e5-0198ab912cf5", true, PriceTypeId.Margin)]
        public void PricingRuleEngineTest(int optionId, string affiliateId, bool isB2BAffiliate, PriceTypeId priceTypeId)
        {
            var pricingRequest = PreparePricingRuleRequest(optionId, affiliateId, isB2BAffiliate, priceTypeId);
            var result = _pricingController.Process(pricingRequest);
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to test the GetProductRules method which fetches all the product sales rules
        /// </summary>
        [Test]
        public void GetProductRules()
        {
            var rules = _pricingRuleEngineService.GetProductSaleRule();
            Assert.IsNotNull(rules);
        }

        /// <summary>
        /// Method to test the GetB2BSaleRules method which fetches all the B2B sale rules
        /// </summary>
        [Test]
        public void GetB2BSaleRules()
        {
            var rules = _pricingRuleEngineService.GetB2BSaleRules();
            Assert.IsNotNull(rules);
        }

        /// <summary>
        /// Method to test the GetB2BNetRateRules method which fetches all the B2B net rate rules
        /// </summary>
        [Test]
        public void GetB2BNetRateRules()
        {
            var rules = _pricingRuleEngineService.GetB2BNetRateRules();
            Assert.IsNotNull(rules);
        }

        /// <summary>
        /// Method to test the GetSupplierSaleRules method which fetches all the supplier sale rules
        /// </summary>
        [Test]
        public void GetSupplierSaleRules()
        {
            var rules = _pricingRuleEngineService.GetSupplierSaleRule();
            Assert.IsNotNull(rules);
        }

        #region Private Methods

        private PricingRuleRequest PreparePricingRuleRequest(int optionId, string affiliateId, bool isB2BAffiliate, PriceTypeId priceTypeId)
        {
            var pricingRequest = new PricingRuleRequest
            {
                PriceTypeId = PriceTypeId.Margin, // Pass Commission if BasePrice available and margin if cost price available
                Criteria = new Criteria
                {
                    CheckinDate = DateTime.Now
                },
                ProductOptions = new List<ProductOption>
                {
                    PrepareProductOption(optionId, priceTypeId)
                },
                ClientInfo = new ClientInfo
                {
                    // F2DE3C6A-048D-4C7E-AAF0-86CF0D51DDEF // (B2BNetRateModule)
                    // 0E03E273-5FE3-4C80-A80F-2100E9A26513 // (B2BSale Module)
                    // E9B092AD-F8F7-4B15-A478-CB3D444A6D8A // (Product Sale Module)
                    AffiliateId = affiliateId,
                    AffiliateName = "B2C",
                    IsB2BAffiliate = isB2BAffiliate,
                    CountryIp = "gb",
                    IsSupplementOffer = true,
                    Currency = new Currency
                    {
                        IsoCode = "GBP"
                    }
                }
            };
            if (priceTypeId.Equals(PriceTypeId.Margin))
            {
                pricingRequest.PriceTypeId = PriceTypeId.Margin;
            }
            return pricingRequest;
        }

        private ProductOption PrepareProductOption(int optionId, PriceTypeId priceTypeId)
        {
            var option = new ProductOption
            {
                Id = optionId,
                AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                TravelInfo = PrepareTravelInfo(),
                IsSelected = true,
                GateBasePrice = new Price
                {
                    Amount = 100,
                    Currency = new Currency
                    {
                        IsoCode = "AUD"
                    },
                    DatePriceAndAvailabilty = PrepareDatePriceAndAvailabilty()
                }
            };
            if (priceTypeId.Equals(PriceTypeId.Margin))
            {
                option.CostPrice = new Price
                {
                    Amount = 100,
                    Currency = new Currency
                    {
                        IsoCode = "AUD"
                    },
                    DatePriceAndAvailabilty = PrepareDatePriceAndAvailabilty()
                };
                option.Margin = new Margin
                {
                    Value = 5,
                    IsPercentage = true
                };
            }
            else
            {
                option.BasePrice = new Price
                {
                    Amount = 100,
                    Currency = new Currency
                    {
                        IsoCode = "AUD"
                    },
                    DatePriceAndAvailabilty = PrepareDatePriceAndAvailabilty()
                };
                option.CommisionPercent = 10;
            }
            return option;
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

        #endregion Private Methods
    }
}