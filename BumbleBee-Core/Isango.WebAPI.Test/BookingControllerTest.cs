using Autofac;
using DiscountRuleEngine.Contracts;
using Isango.Entities.Booking;
using Isango.Entities.Cancellation;
using Isango.Entities.MyIsango;
using Isango.Service.Contract;
using Isango.Service.Factory;
using Logger.Contract;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TableStorageOperations.Contracts;
using WebAPI.Controllers;
using WebAPI.Helper;
using WebAPI.Mapper;
using WebAPI.Models.ResponseModels;

namespace Isango.WebAPI.Test
{
    [TestFixture]
    public class BookingControllerTest
    {
        private BookingController _bookingControllerMock;
        private IProfileService _profileServiceMock;
        private BookingMapper _bookingMapper;
        private IBookingService _bookingService;

        private IActivityService _activityService;
        private IMasterService _masterService;
        private IAffiliateService _affiliateService;
        private IDiscountEngine _discountEngine;
        private ITableStorageOperation _TableStorageOperations;
        private BookingHelper _bookingHelper;
        private ILogger _log;
        private CancellationHelper _cancellationHelper;
        private ICancellationService _cancellationServieMock;
        private PaymentGatewayFactory _paymentGatewayFactory;
        private ILifetimeScope _lifeTimeScope;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _profileServiceMock = Substitute.For<IProfileService>();
            _bookingService = Substitute.For<IBookingService>();
            _TableStorageOperations = Substitute.For<ITableStorageOperation>();
            _activityService = Substitute.For<IActivityService>();
            _masterService = Substitute.For<IMasterService>();
            _affiliateService = Substitute.For<IAffiliateService>();
            _discountEngine = Substitute.For<IDiscountEngine>();

            _log = Substitute.For<ILogger>();
            _lifeTimeScope = Substitute.For<ILifetimeScope>();
            _paymentGatewayFactory = Substitute.For<PaymentGatewayFactory>(_lifeTimeScope);
            _cancellationServieMock = Substitute.For<ICancellationService>();
            _cancellationHelper =
                new CancellationHelper(_bookingService, _cancellationServieMock, _paymentGatewayFactory, _log);
            _bookingMapper = Substitute.For<BookingMapper>(_TableStorageOperations, _activityService, _masterService,
                _affiliateService, _discountEngine, _bookingService);
            //_bookingControllerMock = new BookingController(_profileServiceMock, _bookingMapper, _bookingService,
                //_TableStorageOperations, _log, _bookingHelper, _cancellationHelper, null, null, null, null, null);
            _bookingHelper = Substitute.For<BookingHelper>(_bookingMapper, _bookingService);
        }

        //[Test]
        public void FetchUserEmailPreferencesTest()
        {
            var userEmaillist = new List<MyUserEmailPreferences>
            {
                new MyUserEmailPreferences
                {
                    QuestionId = 5001,
                    QuestionText = "TestQuestionData",
                    MyUserAnswers = new List<MyUserAnswer>
                    {
                        new MyUserAnswer
                        {
                            AnswerId = 1,
                            AnswerOrder = 5,
                            AnswerText = "TestAnswer"
                        }
                    },
                    UserPreferredAnswer = 5,
                    QuestionOrder = 3
                }
            };

            _profileServiceMock.FetchUserEmailPreferencesAsync(1001, "", "en").ReturnsForAnyArgs(userEmaillist);

            var result = _bookingControllerMock.GetUserEmailPreferences(1001, "", "en") as ObjectResult;
            var content = result?.Value as List<UserEmailPreferences>;


            Assert.That(userEmaillist.Count, Is.EqualTo(content?.Count));
        }

        //[Test]
        public void FetchUserBookingDetailTest()
        {
            var myBookingSummaries = new List<MyBookingSummary>
            {
                new MyBookingSummary
                {
                    BookingRefenceNumber = "Test",
                    BookedProducts = new List<MyBookedProduct>
                    {
                        new MyBookedProduct
                        {
                            BookedProductName = "test",
                            BookingAmountPaid = 100,
                            BookingStatus = "Confirmed",
                            NoOfAdults = 2,
                            TravelDate = DateTime.Now
                        }
                    },
                    BookingAmountCurrency = "USD",
                    BookingDate = DateTime.Now,
                    BookingId = 1001,
                    GetBookingDate = "test"
                }
            };

            _profileServiceMock.FetchUserBookingDetailsAsync("hsharma12@isango.com", "", false)
                .ReturnsForAnyArgs(myBookingSummaries);
            var result =
                _bookingControllerMock.GetUserBookingDetails("hsharma12@isango.com", "", false) as
                    ObjectResult;
            var Content = result?.Value as List<BookingSummaryResponse>;


            Assert.That(myBookingSummaries.Count, Is.EqualTo(Content?.Count));
        }

        //[Test]
        public void FetchBookingDetailTest()
        {
            var bookingReferenceNo = "SGI832107";
            //var userId = "8d62ef28-139b-4f4a-986b-1df10d269512";
            var statusId = "2";
            var userName = "hara";
            var userData = new UserCancellationPermission
            {
                UserId = "8d62ef28-139b-4f4a-986b-1df10d269512",
                IsPermitted = 1
            };
            var bookedOptions = new List<BookedOption>
            {
                new BookedOption
                {
                    BookedOptionId = 1300221,
                    TravelDate = DateTime.Parse("2019-12-16 00:00:00.000")
                }
            };

            var confirmDetail = new ConfirmBookingDetail
            {
                BookedOptions = bookedOptions
            };

            var bookingDetails = new List<BookingDetail>
            {
                new BookingDetail
                {
                    BookingDetailId = 1300221,
                    ServiceId = 223,
                    ServiceName = "223-Sydney Pass"
                }
            };
            var bookingOptionsDetails = new List<BookingOptionDetail>();
            foreach (var option in bookingDetails)
            {
                var bookedOption = new BookingOptionDetail();
                bookedOption.OptionId = option.BookingDetailId;
                bookedOption.OptionName = option.ServiceName;
                bookedOption.ServiceId = option.ServiceId;
                bookedOption.ServiceName = option.ServiceName;
                bookedOption.TravelDate = confirmDetail.BookedOptions
                    .Find(x => x.BookedOptionId == bookedOption.OptionId).TravelDate;
                bookingOptionsDetails.Add(bookedOption);
            }

            var bookingOptionsDetailsResponse = new BookingOptionsDetailsResponse
            {
                BookingOptionsDetails = bookingOptionsDetails
            };
            var userCancellationPermission = _cancellationHelper.GetUserCancellationPermission(userName);
            var confirmBookingDetail = _bookingService.GetBookingData(bookingReferenceNo);
            var bookingDetailLst = _bookingService.GetBookingDetailAsync(bookingReferenceNo, userData.UserId, statusId);

            var result =
                _bookingControllerMock.GetBookingDetail(bookingReferenceNo, userData.UserId) as
                    ObjectResult;
            var Content = result?.Value as List<BookingOptionsDetailsResponse>;


            Assert.That(bookingOptionsDetailsResponse?.BookingOptionsDetails?.Count,
                Is.EqualTo(Convert.ToInt32(Content?.Count)));
        }
    }
}