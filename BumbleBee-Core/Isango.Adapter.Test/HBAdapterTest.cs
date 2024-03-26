using Autofac;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Register;
using Logger.Contract;
using NSubstitute;
using NUnit.Framework;
using ServiceAdapters.HB;
using ServiceAdapters.HB.HB.Commands.Contract;
using ServiceAdapters.HB.HB.Converters.Contracts;
using ServiceAdapters.HB.HB.Entities.ActivityDetail;
using ServiceAdapters.HB.HB.Entities.Booking;
using ServiceAdapters.HB.HB.Entities.Cancellation;
using ServiceAdapters.HB.HB.Entities.ContentMulti;
using System;
using System.Collections.Generic;
using System.Linq;
using Booking = Isango.Entities.Booking.Booking;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class HbAdapterTest : BaseTest
    {
        private IHBAdapter _hbAdapter;

        //private readonly BookingMapper _bookingMapper;
        private static string _token = string.Empty;

        static HbAdapterTest()
        {
            _token = "Testeaa7-a7c9-4afc-a722-db9aceb29737";
        }

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();
            var gatewayHbBookingCancelCmdHandler = Substitute.For<IHbBookingCancelCmdHandler>();
            var gatewayBookingCancelSimulationCmdHandler = Substitute.For<IHbBookingCancelSimulationCmdHandler>();
            var gatewayBookingConfirmCmdHandler = Substitute.For<IHbBookingConfirmCmdHandler>();
            var gatewayHBDetailCmdHandler = Substitute.For<IHbDeatilCmdHandler>();
            var gateDetailFullCmdHandler = Substitute.For<IHbDetailFullCmdHandler>();
            var gateCalendarHandler = Substitute.For<IHBCalendarCmdHandler>();
            var gateContentMultiHandler = Substitute.For<IHBContentMultiCmdHandler>();
            var gateBookingDetailCmdHandler = Substitute.For<IHbGetBookingDetailCmdHandler>();
            var gateHBSearchCmdHandler = Substitute.For<IHbSearchCmdHandler>();
            var gateBookingCancelConverter = Substitute.For<IHbBookingCancelConverter>();
            var gatewayBookingCancelSimulationConverter = Substitute.For<IHbBookingCancelSimulationConverter>();
            var gatewayhbBookingConfirmConverter = Substitute.For<IHbBookingConfirmConverter>();
            var gatewayHBDetailConverter = Substitute.For<IHbDetailConverter>();
            var getewayHBGetBookingDetailConverter = Substitute.For<IHbGetBookingDetailConverter>();
            var getewayHBGetCalendarConverter = Substitute.For<IHbCalendarConverter>();
            var logger = Substitute.For<ILogger>();
            /*
            hbBookingCancelCmdHandler,
             hbBookingCancelSimulationCmdHandlerCmdHandler,
             hbBookingConfirmCmdHandler,
             hbDeatilCmdHandler,
            hbDetailFullCmdHandler,
            hbCalendarCmdHandler,
            hbContentMultiCmdHandler,
            hbGetBookingDetailCmdHandler,
            hbSearchCmdHandler,
            hbBookingCancelConverter,
            hbBookingCancelSimulationConverter,
            hbBookingConfirmConverter,
            hbDetailConverter,
            hbGetBookingDetailConverter
            */

            _hbAdapter = new HBAdapter(
                hbBookingCancelCmdHandler: gatewayHbBookingCancelCmdHandler
                , hbBookingCancelSimulationCmdHandlerCmdHandler: gatewayBookingCancelSimulationCmdHandler
                , hbBookingConfirmCmdHandler: gatewayBookingConfirmCmdHandler
                , hbDeatilCmdHandler: gatewayHBDetailCmdHandler
                , hbDetailFullCmdHandler: gateDetailFullCmdHandler
                , hbCalendarCmdHandler: gateCalendarHandler
                , hbGetBookingDetailCmdHandler: gateBookingDetailCmdHandler
                , hbSearchCmdHandler: gateHBSearchCmdHandler
                , hbBookingCancelConverter: gateBookingCancelConverter
                , hbBookingCancelSimulationConverter: gatewayBookingCancelSimulationConverter
                , hbBookingConfirmConverter: gatewayhbBookingConfirmConverter
                , hbDetailConverter: gatewayHBDetailConverter
                , hbGetBookingDetailConverter: getewayHBGetBookingDetailConverter
                , hbContentMultiCmdHandler: gateContentMultiHandler
                , hbGetCalendarConverter: getewayHBGetCalendarConverter
                , log: logger
                );

            using (var scope = _container.BeginLifetimeScope())
            {
                _hbAdapter = scope.Resolve<IHBAdapter>();
            }
        }

        [Test]
        public void HbSearchAsyncTest()
        {
            //var searchRq = new HotelbedCriteria hotelbedCriteria
            //{
            //    From = "Paris",
            //    Language = "EN",
            //    To = "Dubai"
            //};
            //var token = Guid.NewGuid();
            //var result = _hbAdapter.SearchAsync(searchRq, token.ToString());
            //Assert.IsNotNull(result);
        }

        [Test]
        public void HbActivityDetailsAsyncTest()
        {
            var activityRq = GetActivityCriteriaReqeust();
            var result = _hbAdapter.ActivityDetailsAsync(activityRq, _token.ToString()).GetAwaiter().GetResult();
            Assert.IsNotNull(result);
        }

        [Test]
        public void HbActivityDetailsFullAsyncTest()
        {
            var _token = "HBActivityDetaileaa7-a7c9-4afc-a722-db9aceb29737";
            var activityRq = GetActivityCriteriaReqeust();
            var result = _hbAdapter.ActivityDetailsFullAsync(activityRq, _token.ToString()).GetAwaiter().GetResult();
            Assert.IsNotNull(result);
        }

        [Test]
        public void HbActivityContentMultiAsyncTest()
        {
            var _token = "HBContentMultiaa7-a7c9-4afc-a722-db9aceb29737";
            var activityRq = GetContentMultiCriteriaRequest();
            var result = _hbAdapter.ContentMultiAsync(activityRq, _token.ToString()).GetAwaiter().GetResult();
            Assert.IsNotNull(result);
        }

        [Test]
        public void HbCalendarAsyncTest()
        {
            var _token = "HBCalendareaa7-a7c9-4afc-a722-db9aceb29737";
            var activityRq = GetCalendarCriteriaRequest();
            var result = _hbAdapter.CalendarAsync(activityRq, _token.ToString()).GetAwaiter().GetResult();
            Assert.IsNotNull(result);
        }

        [Test]
        public void HbBookingConfirmAsyncTest()
        {
            var activityRq = GetActivityCriteriaReqeust();
            var activityResult = _hbAdapter.ActivityDetailsAsync(activityRq, _token.ToString()).GetAwaiter().GetResult();

            if (activityResult != null)
            {
                activityResult.ProductOptions = activityResult?.ProductOptions?.Take(2)?.ToList();
            }
            var firstOption = activityResult?.ProductOptions?.FirstOrDefault();

            var booking = new Booking
            {
                Currency = new Currency
                {
                    IsoCode = firstOption?.CostPrice?.Currency?.IsoCode ?? "GBP"
                },
                Amount = activityResult?.ProductOptions?.Sum(x => x.CostPrice.Amount) ?? 200,
                BookingId = 5001,
                VoucherEmailAddress = "skumar@isnago.com",
                VoucherPhoneNumber = "124567890",
                BookingType = BookingType.Prepaid,
                ReferenceNumber = $"TestHB{DateTime.Now.ToString("yyyyMMMddHHss")}",
            };
            var contractQuestions = (firstOption as ActivityOption)?.ContractQuestions;
            var Customers = new List<Customer> { new Customer { FirstName = "Test Customer", Age = 30, IsLeadCustomer = true } };
            activityResult?.ProductOptions?.ForEach(x =>
            {
                x.IsSelected = true;
                var ao = x as ActivityOption;
                ao.Customers = Customers;
            });
            var listProduct = new List<HotelBedsSelectedProduct>
            { new HotelBedsSelectedProduct
                {
                    Id = System.Convert.ToInt32(activityResult.Id),
                    ContractQuestions = contractQuestions,
                    ProductOptions = activityResult?.ProductOptions,
                    ActivityType = ActivityType.FullDay,
                    Name = activityResult.Name,
                    APIType = APIType.Hotelbeds,
                    ActivityCode = activityResult.Code,
                }
            };
            booking.SelectedProducts = listProduct.Cast<SelectedProduct>().ToList();

            var req = string.Empty;
            var res = string.Empty;

            //_hbAdapter.BookingConfirmAsync(listProduct, _token, out _, out _).ReturnsForAnyArgs(listProduct);

            var result = _hbAdapter.BookingConfirm(booking, _token, out req, out res);
            Assert.That(result, Is.EqualTo(listProduct));
        }

        [Test]
        public void HbGetBookingDetailAsyncTest()
        {
            var listProduct = new List<HotelBedsSelectedProduct> { new HotelBedsSelectedProduct { Id = 4001,

                ContractQuestions = new List<ContractQuestion>{new ContractQuestion{Code = "TestCode"}},
                ProductOptions = new List<ProductOption>{new ActivityOption{ Customers = new List<Customer> { new Customer { FirstName = "TestCustomer", Age = 30, IsLeadCustomer = true } },
                    TravelInfo = new TravelInfo { StartDate = new DateTime(2018, 12, 10), NumberOfNights = 3 }, Name = "TestProduct",IsSelected = true,Id = 1001}},
                ActivityType = ActivityType.Restaurant, Name = "Test", APIType = APIType.Hotelbeds, ActivityCode = "60040" } };

            var bookingRq = new BookingRq { Language = "EN", HolderSurname = "TestHolderSirName", HolderName = "TestHolderName", BookingReference = "TestBookingReference" };
            var token = Guid.NewGuid();
            _hbAdapter.GetBookingDetailAsync(bookingRq, token.ToString(), out _, out _).ReturnsForAnyArgs(listProduct);

            var result = _hbAdapter.GetBookingDetailAsync(bookingRq, token.ToString(), out _, out _);
            Assert.That(result, Is.EqualTo(listProduct));
        }

        [Test]
        [Ignore("For Testing")]
        public void HbBookingCancelAsyncTest()
        {
            var cancellationRS = new CancellationRS();
            var booking = new ServiceAdapters.HB.HB.Entities.Cancellation.Booking
            {
                Status = "CANCELLED"
            };
            cancellationRS.Booking = booking;
            var token = Guid.NewGuid();
            var result = _hbAdapter.BookingCancel("256-4515161", "en", token.ToString(), out _, out _);
            Assert.That(result.Booking.Status, Is.EqualTo(cancellationRS.Booking.Status));
        }

        [Test]
        [Ignore("For Testing")]
        public void HbBookingCancelSimulationAsyncTest()
        {
            var cancellationRS = new CancellationRS();
            var booking = new ServiceAdapters.HB.HB.Entities.Cancellation.Booking
            {
                Status = "CANCELLED"
            };
            cancellationRS.Booking = booking;
            var token = Guid.NewGuid();
            var result = _hbAdapter.BookingCancelSimulation("256-4515160", "en", token.ToString(), out _, out _);
            Assert.That(result.Booking.Status, Is.EqualTo(cancellationRS.Booking.Status));
        }

        private ActivityRq GetActivityDetailReqeust()
        {
            var activityRq = new ActivityRq
            {
                Code = "E-U10-SANGO",
                From = DateTime.Now.ToString("yyyy-MM-dd"),
                To = (DateTime.Now.AddDays(14)).ToString("yyyy-MM-dd"),
                Language = "en",
                Paxes = new List<ServiceAdapters.HB.HB.Entities.ActivityDetail.Pax>
                {
                    new ServiceAdapters.HB.HB.Entities.ActivityDetail.Pax
                    {
                        Age=30
                    }
                    ,
                    new ServiceAdapters.HB.HB.Entities.ActivityDetail.Pax
                    {
                        Age=8
                    }
                }
            };
            return activityRq;
        }

        private HotelbedCriteriaApitude GetActivityCriteriaReqeust()
        {
            var activityRq = new HotelbedCriteriaApitude
            {
                //Activity with questions
                //IsangoActivityId = "25474",
                //ActivityCode = "E-E10-ALHAMBYBUS",

                //ActivityWithMulyipleOptions
                IsangoActivityId = "6591",
                ActivityCode = "E-U10-SANGO",

                CheckinDate = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")),
                CheckoutDate = Convert.ToDateTime((DateTime.Now.AddDays(14)).ToString("yyyy-MM-dd")),
                Language = "en",
                Ages = new Dictionary<PassengerType, int>(),
                //PassengerAgeGroupIds = new Dictionary<PassengerType, int>(),
                NoOfPassengers = new Dictionary<PassengerType, int>()
            };
            activityRq.NoOfPassengers.Add(PassengerType.Adult, 1);
            activityRq.Ages.Add(PassengerType.Adult, 30);
            //activityRq.PassengerAgeGroupIds.Add(PassengerType.Adult, 15983);
            return activityRq;
        }

        private HotelbedCriteriaApitudeFilter GetCalendarCriteriaRequest()
        {
            var filters = new List<Filters>();
            var filter = new Filters();
            var searchFilterItems = new List<SearchFilterItems>() {
                                                                 new SearchFilterItems { Type = "service", Value = "E-E10-PF2SHOW" },
                                                                 new SearchFilterItems { Type = "service", Value = "E-E10-HEMISFERIC" }
                                                                  };
            filter.SearchFilterItems = searchFilterItems;
            filters.Add(filter);

            var activityRq = new HotelbedCriteriaApitudeFilter
            {
                Filters = filters,
                CheckinDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),
                CheckoutDate = Convert.ToDateTime((DateTime.Now.AddDays(6)).ToString("yyyy-MM-dd")),
                Language = "en",
                Ages = new Dictionary<PassengerType, int>(),
                //PassengerAgeGroupIds = new Dictionary<PassengerType, int>(),
                NoOfPassengers = new Dictionary<PassengerType, int>()
            };
            activityRq.NoOfPassengers.Add(PassengerType.Adult, 1);
            activityRq.Ages.Add(PassengerType.Adult, 30);
            //activityRq.PassengerAgeGroupIds.Add(PassengerType.Adult, 15983);
            return activityRq;
        }

        private ContentMultiRq GetContentMultiCriteriaRequest()
        {
            var codes = new List<Code>() {
                                            new Code {  ActivityCode = "E-E10-GRABPINTXO" },
                                            new Code {  ActivityCode = "E-E10-BASQUEBIKE" }
                                        };
            var activityRq = new ContentMultiRq
            {
                Codes = codes,
                Language = "en"
            };

            return activityRq;
        }
    }
}