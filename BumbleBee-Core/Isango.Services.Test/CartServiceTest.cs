using Isango.Entities;
using Isango.Entities.Bokun;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Service;
using Isango.Service.Contract;
using Logger.Contract;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using ServiceAdapters.Bokun;
using ServiceAdapters.Bokun.Bokun.Entities.GetPickupPlaces;
using ServiceAdapters.FareHarbor;
using ServiceAdapters.GlobalTix;
using ServiceAdapters.HotelBeds;
using ServiceAdapters.Rezdy;
using System;
using System.Collections.Generic;

namespace Isango.Services.Test
{
    [TestFixture]
    public class CartServiceTest : BaseTest
    {
        private CartService _cartServiceMocking;
        private CartService _cartServiceMockingException;
        private ITicketAdapter _hotelBedsAdapter;
        private IMasterService _masterService;
        private IBokunAdapter _bokunAdapter;
        private ITicketAdapter _hotelBedsAdapterMockingException;
        private IMasterService _masterServiceMockingException;
        private IBokunAdapter _bokunAdapterMockingException;
        private IRezdyAdapter _rezdyAdapter;
        private IGlobalTixAdapter _globalTixAdapter;
        private IFareHarborAdapter _fareHarborAdapter;
        [OneTimeSetUp]
        public void TestInitialise()
        {
            var gatewayLog = Substitute.For<ILogger>();
            var gatewayLogMockingException = Substitute.For<ILogger>();
            _hotelBedsAdapter = Substitute.For<ITicketAdapter>();
            _masterService = Substitute.For<IMasterService>();
            _bokunAdapter = Substitute.For<IBokunAdapter>();
            _hotelBedsAdapterMockingException = Substitute.For<ITicketAdapter>();
            _bokunAdapterMockingException = Substitute.For<IBokunAdapter>();
            _masterServiceMockingException = Substitute.For<IMasterService>();
            _rezdyAdapter= Substitute.For<IRezdyAdapter>();
            _globalTixAdapter = Substitute.For<IGlobalTixAdapter>();
            _fareHarborAdapter = Substitute.For<IFareHarborAdapter>();
            _cartServiceMocking = new CartService(_hotelBedsAdapter, gatewayLog, _bokunAdapter, _masterService, _rezdyAdapter, _globalTixAdapter, _fareHarborAdapter,null);
            _cartServiceMockingException = new CartService(_hotelBedsAdapterMockingException, gatewayLogMockingException, _bokunAdapterMockingException, _masterServiceMockingException, _rezdyAdapter, _globalTixAdapter, _fareHarborAdapter,null);
        }

        [Test]
        public void GetExtraDetailsForBokunTest()
        {
            var bokunSelectedProduct = new BokunSelectedProduct
            {
                ActivityCode = "5508",
                APIType = APIType.Bokun,
                Name = "test",
                ProductId = 5508,
                SellPrice = 60,
                ProductOptions = new List<ProductOption>
                        {
                            new ProductOption
                            {
                                IsSelected=true,
                                AvailabilityStatus=AvailabilityStatus.AVAILABLE,
                                TravelInfo=new TravelInfo
                                {
                                    Ages=new Dictionary<PassengerType, int>{ {PassengerType.Adult,18 } },
                                    NoOfPassengers=new Dictionary<PassengerType, int>{ {PassengerType.Adult,1 } },
                                    NumberOfNights=2,
                                    StartDate=DateTime.Now.AddDays(20)
                                },
                               SellPrice= new Price()
                               {
                                   Amount=60,
                                   Currency=new Currency
                                   {
                                       Name="GBP",
                                       IsoCode="GBP",
                                       IsPostFix=false
                                   },
                                   DatePriceAndAvailabilty= null
                               }
                            }
                        }
            };
            var pickupDetails = new GetPickupPlacesRS
            {
                PickupPlaces = new List<PlaceDetails>
                {
                    new PlaceDetails()
                }
            };
            _bokunAdapter.GetPickupPlaces(bokunSelectedProduct.Id, "token").ReturnsForAnyArgs(pickupDetails);
            var result = _cartServiceMocking.GetExtraDetailsForBokun(bokunSelectedProduct, "token");
            Assert.IsTrue(result != null);
        }

        [Test]
        public void GetExtraDetailsForBokunExceptionTest()
        {
            var bokunSelectedProduct = new BokunSelectedProduct
            {
                ActivityCode = "5508",
                APIType = APIType.Bokun,
                Name = "test",
                ProductId = 5508,
                SellPrice = 60,
                ProductOptions = new List<ProductOption>
                {
                    new ProductOption
                    {
                        IsSelected=true,
                        AvailabilityStatus=AvailabilityStatus.AVAILABLE,
                        TravelInfo=new TravelInfo
                        {
                            Ages=new Dictionary<PassengerType, int>{ {PassengerType.Adult,18 } },
                            NoOfPassengers=new Dictionary<PassengerType, int>{ {PassengerType.Adult,1 } },
                            NumberOfNights=2,
                            StartDate=DateTime.Now.AddDays(20)
                        },
                        SellPrice= new Price()
                        {
                            Amount=60,
                            Currency=new Currency
                            {
                                Name="GBP",
                                IsoCode="GBP",
                                IsPostFix=false
                            },
                            DatePriceAndAvailabilty= null
                        }
                    }
                }
            };
            //Catch Block Scenario
            _cartServiceMockingException.GetExtraDetailsForBokun(bokunSelectedProduct, "token").Throws(new Exception());
            Assert.Throws<Exception>(() => _cartServiceMockingException.GetExtraDetailsForBokun(bokunSelectedProduct, "token"));
        }

        [Test]
        public void GetExtraDetailsForHotelBedsTest()
        {
            var hbSelectedProducts = new HotelBedsSelectedProduct
            {
                ActivityCode = "5508",
                APIType = APIType.Hotelbeds,
                Name = "test",
                ProductId = 5508,
                SellPrice = 60,
                ProductOptions = new List<ProductOption>
                        {
                            new ProductOption
                            {
                                IsSelected=true,
                                AvailabilityStatus=AvailabilityStatus.AVAILABLE,
                                TravelInfo=new TravelInfo
                                {
                                    Ages=new Dictionary<PassengerType, int>{ {PassengerType.Adult,18 } },
                                    NoOfPassengers=new Dictionary<PassengerType, int>{ {PassengerType.Adult,1 } },
                                    NumberOfNights=2,
                                    StartDate=DateTime.Now.AddDays(20)
                                },
                               SellPrice= new Price()
                               {
                                   Amount=60,
                                   Currency=new Currency
                                   {
                                       Name="GBP",
                                       IsoCode="GBP",
                                       IsPostFix=false
                                   },
                                   DatePriceAndAvailabilty= null
                               }
                            }
                        }
            };
            _hotelBedsAdapter.GetTicketPrice(hbSelectedProducts, "authstring", "token").ReturnsForAnyArgs(hbSelectedProducts);
            var result = _cartServiceMocking.GetExtraDetailsForHotelBeds(hbSelectedProducts, "token");
            Assert.IsTrue(result != null);
        }

        [Test]
        public void GetExtraDetailsForHotelBedsExceptionTest()
        {
            //Catch Block Scenario
            _cartServiceMockingException.GetExtraDetailsForHotelBeds(null, string.Empty).Throws(new Exception());
            Assert.Throws<Exception>(() => _cartServiceMockingException.GetExtraDetailsForHotelBeds(null, string.Empty));
        }

        [Test]
        public void GetExtraDetailsForGrayLineIceLandTest()
        {
            var res = System.Threading.Tasks.Task.FromResult(new List<PickupLocation>());
            _masterService.GetPickupLocationsByActivityAsync(1).ReturnsForAnyArgs(res);
            var result = _cartServiceMocking.GetExtraDetailsForGrayLineIceLand(1, "token");
            Assert.IsTrue(result != null);
        }

        [Test]
        public void GetExtraDetailsForGrayLineIceLandExceptionTest()
        {
            _masterServiceMockingException.GetPickupLocationsByActivityAsync(1).Throws(new Exception());
            Assert.Throws<Exception>(() => _cartServiceMockingException.GetExtraDetailsForGrayLineIceLand(1, "token"));
        }
    }
}