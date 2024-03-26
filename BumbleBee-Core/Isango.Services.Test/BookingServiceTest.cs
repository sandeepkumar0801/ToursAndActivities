using Autofac;
using CacheManager.Contract;
using CacheManager.FareHarborCacheManagers;
using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.CitySightseeing;
using Isango.Entities.Wrapper;
using Isango.Persistence.Contract;
using Isango.Service;
using Isango.Service.Contract;
using Isango.Service.Factory;
using Logger.Contract;
using NSubstitute;
using NUnit.Framework;
using ServiceAdapters.AlternativePayment;
using ServiceAdapters.WirecardPayment;
using System;
using System.Collections.Generic;

namespace Isango.Services.Test
{
    [TestFixture]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BookingServiceTest : BaseTest
    {
        private IBookingService _bookingService;
        private IBookingPersistence _bookingPersistenceMock;
        private IBookingService _bookingServiceException;
        private IBookingPersistence _bookingPersistenceMockException;
        private ISupplierBookingService _supplierbookingServiceMock;
        private IMasterService _bookingMasterServiceMock;
        private IWirecardPaymentAdapter _wirecardPaymentAdapterMock;
        private IAffiliateService _affiliateServiceMock;
        private IRiskifiedService _riskifiedService;
        private IAlternativePaymentService _alternativePaymentServiceMock;
        private IAlternativePaymentAdapter _alternativePaymentAdapterMock;
        private IMailerService _mailerServiceMock;
        private PaymentGatewayFactory _paymentGatewayFactory;
        private ILifetimeScope _lifeTimeScope;
        private FareHarborUserKeysCacheManager _fareHarborUserKeysCacheManager;
        private ICosmosHelper<CacheKey<FareHarborUserKey>> _cosmosHelper;
        private ILogger _log;
        private IAdyenPersistence _adyenPersistence;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            
            _bookingPersistenceMockException = Substitute.For<IBookingPersistence>();

            _supplierbookingServiceMock = Substitute.For<ISupplierBookingService>();
            _bookingMasterServiceMock = Substitute.For<IMasterService>();

            _wirecardPaymentAdapterMock = Substitute.For<IWirecardPaymentAdapter>();
            _alternativePaymentServiceMock = Substitute.For<IAlternativePaymentService>();
            _alternativePaymentAdapterMock = Substitute.For<IAlternativePaymentAdapter>();
            _mailerServiceMock = Substitute.For<IMailerService>();
            //_paymentGatewayFactory = Substitute.For<PaymentGatewayFactory>();
            _lifeTimeScope = Substitute.For<ILifetimeScope>();
            _log = Substitute.For<ILogger>();
            _cosmosHelper = Substitute.For<ICosmosHelper<CacheKey<FareHarborUserKey>>>();
            //_fareHarborUserKeysCacheManager = Substitute.For<FareHarborUserKeysCacheManager>(_cosmosHelper);
            _adyenPersistence = Substitute.For<IAdyenPersistence>();
            _bookingService = new BookingService(_bookingPersistenceMock, _log, _supplierbookingServiceMock, _wirecardPaymentAdapterMock,
                _bookingMasterServiceMock, _alternativePaymentServiceMock, _alternativePaymentAdapterMock, _mailerServiceMock, _paymentGatewayFactory, _riskifiedService, _fareHarborUserKeysCacheManager, _adyenPersistence,null);

            _bookingServiceException = _bookingService;
        }

        [Test]
        [Ignore("")]
        public void GetbookingDataTest()
        {
            var bookingReferenceNumber = "ISA0";
            var bookingData = _bookingService.GetBookingData(bookingReferenceNumber);
            Assert.IsNotNull(bookingData);
        }

        [Test]
        [Ignore("")]
        public void GetConfirmedProductBookingDetailByUserAndStatusIdTest()
        {
            string bookingRefNo = "SGI832107";
            string userId = "8d62ef28-139b-4f4a-986b-1df10d269512";
            string statusId = "2"; // 2 or 4 -- booking confirmed
            var bookingDetail = new BookingDetail
            {
                BookingDetailId = 1300224,
                ServiceName = "223-Sydney ByPass",
                ServiceId = 223,
                BookingCurrency = "USD",
                SupplierCurrency = "USD",
                ServiceOptionId = 13638,
                PassengerId = 832255,
                IsHotelbed = false,
                HbReferenceNo = null,
                OfficeCode = null,
                LanguageCode = "en",
                TrakerStatusId = 3
            };
            var bookingDetails = new List<BookingDetail> { bookingDetail };
            _bookingPersistenceMock.GetBookingDetails(bookingRefNo, userId, statusId).ReturnsForAnyArgs(bookingDetails);
            var result = _bookingService.GetBookingDetailAsync(bookingRefNo, userId, statusId).GetAwaiter().GetResult();
            Assert.IsNotEmpty(result);
        }
        [Ignore("")]
        [Test]
        public void GetOnRequestBookingDetailByUserAndStatusIdTest()
        {
            string bookingRefNo = "SGI832107";
            string userId = "8d62ef28-139b-4f4a-986b-1df10d269512";
            string statusId = "1"; // 1 -- OnRequest Product status
            var bookingDetail = new BookingDetail
            {
                BookingDetailId = 1300221,
                ServiceName = "223-Sydney Pass",
                ServiceId = 223,
                BookingCurrency = "USD",
                SupplierCurrency = "USD",
                ServiceOptionId = 13638,
                PassengerId = 832255,
                IsHotelbed = false,
                HbReferenceNo = null,
                OfficeCode = null,
                LanguageCode = "en",
                TrakerStatusId = 1
            };
            var bookingDetails = new List<BookingDetail> { bookingDetail };
            _bookingPersistenceMock.GetBookingDetails(bookingRefNo, userId, statusId).ReturnsForAnyArgs(bookingDetails);
            var result = _bookingService.GetBookingDetailAsync(bookingRefNo, userId, statusId).GetAwaiter().GetResult();
            Assert.IsNotEmpty(result);
        }

        [Test]
        [Ignore("")]
        public void GetCancelledProductBookingDetailByUserAndStatusIdTest()
        {
            string bookingRefNo = "SGI832107";
            string userId = "8d62ef28-139b-4f4a-986b-1df10d269512";
            string statusId = "3"; // 3 -- cancelled product status
            var bookingDetail = new BookingDetail
            {
                BookingDetailId = 1300221,
                ServiceName = "223-Sydney Pass",
                ServiceId = 223,
                BookingCurrency = "USD",
                SupplierCurrency = "USD",
                ServiceOptionId = 13638,
                PassengerId = 832255,
                IsHotelbed = false,
                HbReferenceNo = null,
                OfficeCode = null,
                LanguageCode = "en",
                TrakerStatusId = 3
            };
            var bookingDetails = new List<BookingDetail> { bookingDetail };
            _bookingPersistenceMock.GetBookingDetails(bookingRefNo, userId, statusId).ReturnsForAnyArgs(bookingDetails);
            var result = _bookingService.GetBookingDetailAsync(bookingRefNo, userId, statusId).GetAwaiter().GetResult();
            Assert.IsNotEmpty(result);
        }

        [Test]
        [Ignore("")]
        public void GetEmptyBookingDetailByUserAndStatusIdTest()
        {
            string bookingRefNo = "SGI832107";
            string userId = "8d62ef28-139b-4f4a-986b-1df10d269512";
            string statusId = "0";

            var bookingDetails = new List<BookingDetail>();
            _bookingPersistenceMock.GetBookingDetails(bookingRefNo, userId, statusId).ReturnsForAnyArgs(bookingDetails);
            var result = _bookingService.GetBookingDetailAsync(bookingRefNo, userId, statusId).GetAwaiter().GetResult();
            Assert.IsEmpty(result);
        }

        [Test]
        [Ignore("")]
        public void CancelSupplierBooking()
        {
            var selectedProduct = new CitySightseeingSelectedProduct()
            {
                APIType = Entities.Enums.APIType.Citysightseeing,
                AvailabilityReferenceId = "afadseytyetfadsfa",
                Pnr = "212334",
                Id = 121,
                ProductOptions = new List<ProductOption>
                {
                    new ProductOption
                        {TravelInfo = new TravelInfo {StartDate = Convert.ToDateTime("16-12-2019")}}
                }
            };

            var booking = new Booking
            {
                User = new ISangoUser { EmailAddress = "testUser@test.com", MobileNumber = "1234567890" },
                IsangoBookingData = new IsangoBookingData
                {
                    BookedProducts = new List<BookedProduct>
                    {
                        new BookedProduct
                        {
                            AvailabilityReferenceId = selectedProduct.AvailabilityReferenceId,
                            APIExtraDetail = new ApiExtraDetail {SupplieReferenceNumber = "asdfasdf"}
                        }
                    }
                },
                SelectedProducts = new List<SelectedProduct> { selectedProduct }
            };
            var dictionary = new Dictionary<string, bool> { { selectedProduct.AvailabilityReferenceId, true } };
            var token = "adsfadsfasdfads";
            var authentication = "isangokey";
            var notProcessedProductIds = new List<string>();
            _supplierbookingServiceMock.CancelSightSeeingBooking(booking.SelectedProducts, token)
                .ReturnsForAnyArgs(dictionary);
            var isCancelled = _bookingService
                .CancelSupplierBookingAsync(booking, token,true).GetAwaiter().GetResult();
            Assert.That(isCancelled, Is.EqualTo(true));
        }
    }
}