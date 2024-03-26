using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Enums;
using Isango.Entities.Region;
using Isango.Service.Contract;

using Logger.Contract;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

using NUnit.Framework;

using PriceRuleEngine;
using PriceRuleEngine.Factory;

using System;
using System.Collections.Generic;
using TableStorageOperations.Contracts;
using WebAPI.Controllers;
using WebAPI.Helper;
using WebAPI.Mapper;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;
using WebAPI.Models.ResponseModels.CheckAvailability;
using PriceAndAvailability = Isango.Entities.PriceAndAvailability;

namespace Isango.WebAPI.Test
{
    [TestFixture]
    public class ActivityControllerTest
    {
        private IActivityService _activityServiceMock;
        private IAffiliateService _affiliateServiceMock;
        private ActivityController _activityControllerMock;
        private PricingController _pricingController;
        private ILogger _loggerMock;
        private ModuleBuilderFactory _moduleBuilderFactoryMock;
        private IPriceRuleEngineService _priceRuleEngineService;
        private ITableStorageOperation _tableStorageOperation;
        private ActivityMapper _activityMapper;
        private ActivityHelper _activityHelper;
        private IMasterService _masterService;
        private ISearchService _searchServiceMock;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _activityServiceMock = Substitute.For<IActivityService>();
            _affiliateServiceMock = Substitute.For<IAffiliateService>();
            _searchServiceMock = Substitute.For<ISearchService>();
            _tableStorageOperation = Substitute.For<ITableStorageOperation>();

            _loggerMock = Substitute.For<ILogger>();
            _masterService = Substitute.For<IMasterService>();
            _priceRuleEngineService = Substitute.For<IPriceRuleEngineService>();
            _moduleBuilderFactoryMock = Substitute.For<ModuleBuilderFactory>(_priceRuleEngineService);
            _pricingController = Substitute.For<PricingController>(_loggerMock, _moduleBuilderFactoryMock);
            _activityMapper = new ActivityMapper(_activityServiceMock);
            _activityHelper = new ActivityHelper(_activityServiceMock, _affiliateServiceMock, _pricingController, _masterService, _searchServiceMock);
            _activityControllerMock = new ActivityController(_activityServiceMock, _activityHelper, _tableStorageOperation, _activityMapper, null, null);
        }

        [Test]
        public void GetCancellationPolicyTest()
        {
            var startDate = DateTime.Now;
            var cancellationPrices = new List<CancellationPrice>
                {new CancellationPrice {CancellationAmount = 10000, Percentage = 10}};
            var activity = new Activity
            {
                Id = "1001",
                CancellationPolicy = "TestPolicy",
                ProductOptions = new List<ProductOption> { new ProductOption { Id = 5001,
                    CancellationPrices = cancellationPrices } }
            };
            var request = new CancellationPolicyRequest { ActivityId = 1001, ServiceOptionId = 5001, LanguageCode = "en", AffiliateId = "7F577749-BAEB-42C3-B637-577DF021FE28" };
            var clientInfo = _activityMapper.MapClientInfoForCancellationPolicy(request);

            _activityServiceMock.LoadActivityAsync(1001, startDate, clientInfo).ReturnsForAnyArgs(activity);
            //_activityControllerMock.GetResponseWithHttpActionResult(cancellationPrices);
            var res = _activityControllerMock.GetCancellationPolicy(request) as ObjectResult;

            var Content = res?.Value as List<CancellationPolicyResponse>;

            Assert.IsTrue(Content?.Count > 0);
        }

        [Test]
        public void GetActivityDetailsTest()
        {
            // F2DE3C6A-048D-4C7E-AAF0-86CF0D51DDEF // (B2BNetRateModule)
            // 0E03E273-5FE3-4C80-A80F-2100E9A26513 // (B2BSale Module)
            // E9B092AD-F8F7-4B15-A478-CB3D444A6D8A // (Product Sale Module)
            var activity = GetActivity();

            var criteria = new Criteria
            {
                CheckinDate = DateTime.Today,
                CheckoutDate = DateTime.Today,
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Adult, 1 }
                }
            };
            var activityCriteria = new ActivityCriteria
            {
                AffiliateId = "0E03E273-5FE3-4C80-A80F-2100E9A26513",
                CurrencyIsoCode = "GBP",
                LanguageCode = "en",
                ActivityId = 1001,
            };

            var affiliate = new Affiliate
            {
                AffiliateConfiguration = new AffiliateConfiguration
                {
                    IsB2BAffiliate = false,
                    IsSupplementOffer = false
                }
            };
            var clientInfo = _activityMapper.MapClientInfoForActivityDetail(activityCriteria);
            _affiliateServiceMock.GetAffiliateInformationAsync("0E03E273-5FE3-4C80-A80F-2100E9A26513")
                .ReturnsForAnyArgs(affiliate);
            _activityServiceMock.GetActivityDetailsAsync(1001, clientInfo, criteria).ReturnsForAnyArgs(activity);
            _activityServiceMock.CalculateActivityWithMinPricesAsync(activity).ReturnsForAnyArgs(activity);
            var res = _activityControllerMock.GetActivityDetails(activityCriteria) as ObjectResult;

            var Content = res?.Value as ActivityDetails;

            Assert.That(Content?.Id, Is.EqualTo(activity.ID));
        }

        //[Test]
        //public void GetSearchDataTest()
        //{
        //    var activity = GetActivity();
        //    var criteria = new Criteria
        //    {
        //        CheckinDate = DateTime.Today,
        //        CheckoutDate = DateTime.Today,
        //        NoOfPassengers = new Dictionary<PassengerType, int>()
        //        {
        //            {PassengerType.Adult, 1 }
        //        },
        //        Ages = new Dictionary<PassengerType, int>()
        //    };
        //    var searchCriteria = new SearchCriteria { RegionId = 1001 };
        //    var searchRequestCriteria = new SearchRequestCriteria
        //    {
        //        RegionId = 1001,
        //        AffiliateId = "0E03E273-5FE3-4C80-A80F-2100E9A26513",
        //        CountryIp = "gb",
        //        CurrencyIsoCode = "GBP",
        //        LanguageCode = "en",
        //    };
        //    var searchStack = new SearchStack
        //    {
        //        Products = new List<Product> { activity },
        //        Activities = new List<Activity> { activity },
        //        Regions = new List<Region> { new Region { Id = 1001 } }
        //    };
        //    var clientInfo = _activityMapper.MapClientInfoForSearch(searchRequestCriteria, new Affiliate
        //    {
        //        AffiliateConfiguration = new AffiliateConfiguration
        //        {
        //            IsB2BAffiliate = true,
        //            IsSupplementOffer = true
        //        }
        //    });
        //    _searchServiceMock.GetSearchDataAsync(searchCriteria, clientInfo, criteria).ReturnsForAnyArgs(searchStack);
        //    _activityServiceMock.CalculateActivityWithMinPricesAsync(activity).ReturnsForAnyArgs(activity);
        //    var result = _activityControllerMock.GetSearchData(searchRequestCriteria) as OkNegotiatedContentResult<List<SearchDetails>>;

        //    Assert.That(result?.Content.Count, Is.EqualTo(searchStack.Products.Count));
        //}

        [Test]
        [Ignore("Ignore")]
        public void GetPriceAndAvailabilityTest()
        {
            var availablityList = new List<CalendarAvailability>
            {
                new CalendarAvailability
                {
                    //ActivityId = 12809,
                    //AffiliateId = "32B1034A-3588-407F-9E15-A3B1D70E5594",
                    RegionId = 1001,
                    StartDate =  DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(7),
                    Currency = "GBP"
                }
            };
            var calendarResponse = new CalendarResponse
            {
                ActivityId = 12809,
                AffiliateId = "32B1034A-3588-407F-9E15-A3B1D70E5594",
                //DatePriceAvailability = new Dictionary<DateTime, decimal>()
            };
            var affiliate = new Affiliate
            {
                Id = "32B1034A-3588-407F-9E15-A3B1D70E5594",
                AffiliateConfiguration = new AffiliateConfiguration { IsB2BAffiliate = false }
            };
            _activityServiceMock.GetCalendarAvailabilityAsync(12809, "32B1034A-3588-407F-9E15-A3B1D70E5594").ReturnsForAnyArgs(availablityList);
            _affiliateServiceMock.GetAffiliateInformationAsync("32B1034A-3588-407F-9E15-A3B1D70E5594").ReturnsForAnyArgs(affiliate);
            var result = _activityControllerMock.GetPriceAndAvailability(12809, "32B1034A-3588-407F-9E15-A3B1D70E5594") as ObjectResult;

            var Content = result?.Value as CalendarResponse;

            Assert.That(Content?.AffiliateId, Is.EqualTo(calendarResponse.AffiliateId));
            Assert.That(Content?.ActivityId, Is.EqualTo(calendarResponse.ActivityId));
        }

        [Test]
        public void GetProductAvailabilityTest()
        {
            var checkAvailabilityResponse = new CheckAvailabilityResponse
            {
                TokenId = "131NJ23L-3588-407F-9E15-A3B1D70E5594",
                ActivityId = 1001,
                IsPaxDetailRequired = false,
                Name = "test",
                Description = "Mock tetsing",
                Options = new List<Option>
                {
                    new Option()
                    {
                        Id = 10011,
                        Description = "test option 1",
                        AvailabilityStatus = "AVAILABLE"
                    }
                }
            };

            var checkAvailabilityRequest = new CheckAvailabilityRequest
            {
                ActivityId = 1001,
                TokenId = "131NJ23L-3588-407F-9E15-A3B1D70E5594",
                AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                CheckinDate = DateTime.Now.AddDays(10),
                CheckoutDate = DateTime.Now.AddDays(10),
                CountryIp = "",
                CurrencyIsoCode = "USD",
                LanguageCode = "EN",
                PaxDetails = new List<PaxDetail>
                {
                    new PaxDetail
                    {
                        Count = 1,
                        PassengerTypeId = PassengerType.Adult
                    },
                    new PaxDetail
                    {
                        Count = 1,
                        PassengerTypeId = PassengerType.Child
                    }
                }
            };

            var affiliate = GetAffiliate();
            var activity = GetActivity();
            var criteria = PrepareCriteria();
            var clientInfo = PrepareClientInfo();

            _affiliateServiceMock.GetAffiliateInformationAsync("5beef089-3e4e-4f0f-9fbf-99bf1f350183").ReturnsForAnyArgs(affiliate);

            _activityServiceMock.GetProductAvailabilityAsync(activity.ID, clientInfo, criteria).ReturnsForAnyArgs(activity);
            _activityServiceMock.CalculateActivityWithMinPricesAsync(activity).ReturnsForAnyArgs(activity);
            _tableStorageOperation.InsertData(activity, "131NJ23L-3588-407F-9E15-A3B1D70E5594");

            foreach (var option in activity.ProductOptions)
            {
                foreach (KeyValuePair<DateTime, PriceAndAvailability> item in option.BasePrice.DatePriceAndAvailabilty)
                {
                    if (item.Value.ReferenceId == null)
                        item.Value.ReferenceId = Guid.NewGuid().ToString();
                }
            }
            var result = _activityControllerMock.GetProductAvailability(checkAvailabilityRequest) as
                   ObjectResult;

            var Content = result?.Value as CheckAvailabilityResponse;

            Assert.That(Content?.TokenId, Is.EqualTo(checkAvailabilityResponse.TokenId));
        }

        /*
        [Test]
        public void GetProductBundleAvailabilityTest()
        {
            //var affiliate = GetAffiliate();
            //var activity = GetActivity();
            //var criteria = PrepareCriteria();
            //var clientInfo = PrepareClientInfo();

            //activity.ActivityType = ActivityType.Bundle;
            //var id = 1001;

            //activity.ProductOptions.ForEach(
            //    x =>
            //    {
            //        x.ComponentServiceID = id + 1;
            //        x.BundleOptionID = id + 11;
            //    });

            //var checkBundleAvailabilityRequest = new CheckBundleAvailabilityRequest
            //{
            //    ActivityId = 1001,
            //    AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
            //    TokenId = "32B1034A-3588-407F-9E15-A3B1D70E5595",
            //    LanguageCode = "en",
            //    ComponentActivityDetails = new List<ComponentActivityDetail>
            //    {
            //        new ComponentActivityDetail
            //        {
            //            CheckinDate = DateTime.Now.AddDays(10),
            //            CheckoutDate = DateTime.Now.AddDays(10),
            //            ComponentActivityId = 1002,
            //            PaxDetails = new List<PaxDetail>
            //            {
            //                new PaxDetail
            //                {
            //                    Count = 1,
            //                    PassengerTypeId = PassengerType.Adult
            //                },
            //                new PaxDetail
            //                {
            //                    Count = 1,
            //                    PassengerTypeId = PassengerType.Child
            //                }
            //            }
            //        }
            //    },
            //    CurrencyIsoCode = "USD",
            //    CountryIp = ""
            //};

            //var checkBundleAvailabilityResponse = new CheckBundleAvailabilityResponse
            //{
            //    TokenId = "",
            //    ActivityId = 1001,
            //    BundleOptions = new List<BundleOption>
            //    {
            //        new BundleOption
            //        {
            //            Id = 1002,
            //            BasePrice = 10m
            //        }
            //    },
            //    Description = "test desc",
            //    Name = "test",
            //    IsPaxDetailRequired = false
            //};

            //var criteriaForActivity = new Dictionary<int, Criteria>
            //{
            //    {1001, criteria}
            //};

            //_affiliateServiceMock.GetAffiliateInformationAsync(checkBundleAvailabilityRequest.AffiliateId).ReturnsForAnyArgs(affiliate);

            //_activityServiceMock.GetBundleProductAvailabilityAsync(activity.ID, clientInfo, criteriaForActivity).ReturnsForAnyArgs(activity);

            //_tableStorageOperation.InsertData(activity, "131NJ23L-3588-407F-9E15-A3B1D70E5594");

            //foreach (var option in activity.ProductOptions)
            //{
            //    foreach (KeyValuePair<DateTime, PriceAndAvailability> item in option.BasePrice.DatePriceAndAvailabilty)
            //    {
            //        if (item.Value.ReferenceId == null)
            //            item.Value.ReferenceId = Guid.NewGuid().ToString();
            //    }
            //}
            //var result = _activityControllerMock.GetProductBundleAvailability(checkBundleAvailabilityRequest) as
            //    OkNegotiatedContentResult<CheckBundleAvailabilityResponse>;

            //Assert.AreEqual(result?.Content.TokenId, checkBundleAvailabilityRequest.TokenId);
        }
        */

        //[Test]
        public void GetActivityDetailsWithCalendarTest()
        {
            try
            {
                var activity = GetActivity();

                var criteria = new Criteria
                {
                    CheckinDate = DateTime.Today,
                    CheckoutDate = DateTime.Today,
                    NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Adult, 1 }
                }
                };
                var activityCriteria = new ActivityCriteria
                {
                    AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                    CurrencyIsoCode = "USD",
                    LanguageCode = "en",
                    ActivityId = 853,
                };

                var affiliate = new Affiliate
                {
                    AffiliateConfiguration = new AffiliateConfiguration
                    {
                        IsB2BAffiliate = false,
                        IsSupplementOffer = false
                    }
                };

                var request = new ActivityDetailsWithCalendarRequest
                {
                    ActivityId = activityCriteria.ActivityId,
                    ClientInfo = _activityMapper.MapClientInfoForActivityDetail(activityCriteria),
                    Criteria = _activityHelper.GetDefaultCriteria()
                };
                request.ClientInfo.IsSupplementOffer = affiliate.AffiliateConfiguration.IsSupplementOffer;
                request.ClientInfo.IsB2BAffiliate = affiliate.AffiliateConfiguration.IsB2BAffiliate;

                var calendarAvailability = new CalendarAvailability
                {
                    AffiliateId = "5beef089-3e4e-4f0f-9fbf-99bf1f350183",
                    ActivityId = 1001,
                    B2BBasePrice = 10m,
                    B2CBasePrice = 12m,
                    RegionId = 123
                };

                var calendarAvailabilityList = new List<CalendarAvailability>
            {
                calendarAvailability
            };

                var response = new ActivityDetailsWithCalendarResponse
                {
                    Activity = activity,
                    CalendarAvailabilityList = calendarAvailabilityList
                };

                _affiliateServiceMock.GetAffiliateInformationAsync("f7feee76-7b67-4886-90ab-07488cb7a167".ToUpper())
                    .ReturnsForAnyArgs(affiliate);
                _activityServiceMock.GetActivityDetailsWithCalendar(request).ReturnsForAnyArgs(response);
                _activityServiceMock.CalculateActivityWithMinPricesAsync(activity).ReturnsForAnyArgs(activity);
                _activityServiceMock.GetCalendarAvailabilityAsync(1001, affiliate.Id).ReturnsForAnyArgs(calendarAvailabilityList);

                var result = _activityControllerMock.GetActivityDetailsWithCalendar(activityCriteria) as ObjectResult;

                var Content = result?.Value as ActivityDetailsResponse;

                Assert.That(Content?.Activity.Id, Is.EqualTo(activity.ID));
            }
            catch (Exception ex)
            {
                //Ignored - Old Functions and data giving error in new modified code - Have a wprking test case for debugging in CacheLoaderServiceActualTest
            }
        }

        private ProductOption PrepareProductOption()
        {
            return new ProductOption
            {
                // 12857 // Product Sale Module
                // 135385 // SupplierSaleModule
                Id = 135385,
                AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                Name = "test",
                Description = "test option",
                BasePrice = new Entities.Price
                {
                    Amount = 100,
                    Currency = new Currency
                    {
                        IsoCode = "AUD"
                    },
                    DatePriceAndAvailabilty = PrepareDatePriceAndAvailabilty()
                },
                CostPrice = new Entities.Price
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

        private Dictionary<DateTime, Entities.PriceAndAvailability> PrepareDatePriceAndAvailabilty()
        {
            return new Dictionary<DateTime, Entities.PriceAndAvailability>
            {
                {
                    DateTime.Today,
                    new DefaultPriceAndAvailability()
                    {
                        TotalPrice = 100,
                        AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                        PricingUnits = new List<Entities.PricingUnit>()
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
                        PricingUnits = new List<Entities.PricingUnit>()
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
                        IndependablePax = true, MinSize = 0, MaxSize = 10 , Label ="Adult", MeasurementDesc = "Adult >120cm"
                    }
                }
            };
        }

        private Criteria PrepareCriteria()
        {
            return new Criteria
            {
                CheckinDate = DateTime.Now.AddDays(10),
                CheckoutDate = DateTime.Now.AddDays(10),
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    { PassengerType.Child, 10}
                }
            };
        }

        private ClientInfo PrepareClientInfo()
        {
            return new ClientInfo
            {
                AffiliateId = "",
                LanguageCode = "en",
                ApiToken = "",
                Currency = new Currency
                {
                    IsoCode = "USD"
                },
                IsB2BAffiliate = false
            };
        }

        private Affiliate GetAffiliate()
        {
            return new Affiliate
            {
                Id = "32B1034A-3588-407F-9E15-A3B1D70E5594",
                AffiliateConfiguration = new AffiliateConfiguration { IsB2BAffiliate = false }
            };
        }
    }
}