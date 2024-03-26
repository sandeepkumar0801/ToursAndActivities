using Autofac;
using Isango.Entities.BigBus;
using Isango.Entities.Booking;
using Isango.Entities.Cancellation;
using Isango.Entities.CitySightseeing;
using Isango.Entities.Enums;
using Isango.Entities.WirecardPayment;
using Isango.Persistence.Contract;
using Isango.Register;
using Isango.Service.Contract;
using Isango.Service.Factory;
using Isango.Service.PaymentServices;
using Logger.Contract;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using ServiceAdapters.Adyen;
using ServiceAdapters.Apexx;
using ServiceAdapters.BigBus.BigBus.Entities;
using ServiceAdapters.Rayna.Rayna.Entities;
using ServiceAdapters.WirecardPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using TableStorageOperations.Contracts;
using WebAPI.Controllers;
using WebAPI.Helper;
using WebAPI.Mapper;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;
using CancellationParameters = WebAPI.Models.RequestModels.CancellationParameters;
using CancellationStatus = WebAPI.Models.ResponseModels.CancellationStatus;

namespace Isango.WebAPI.Test
{
    [TestFixture]
    internal class CancellationControllerTest : BaseTest
    {
        private CancellationMapper _cancellationMapperMock;
        private ICancellationService _cancellationServiceMock;
        private CancellationHelper _cancellationHelperMock;
        private CancellationController _cancellationControllerMock;
        private IBookingService _bookingServiceMock;
        private PaymentGatewayFactory _paymentGatewayFactory;
        private ITableStorageOperation _apiStorageOperation;
        private ILifetimeScope _lifeTimeScope;
        private IWirecardPaymentAdapter _wirecardPaymentAdapter;
        private IApexxAdapter _apexxAdapter;
        private ILogger _logger;
        private WirecardPaymentService _wireCardPaymentService;
        private IAdyenAdapter _adyenAdapter;
        private IAdyenPersistence _adyenPersistence;
        [OneTimeSetUp]
        public void TestInitialise()
        {
            //Startup.Application_Start();
            //var container = Startup._builder.Build();
            _logger = Substitute.For<ILogger>();
            _wirecardPaymentAdapter = Substitute.For<IWirecardPaymentAdapter>();
            _cancellationServiceMock = Substitute.For<ICancellationService>();
            _bookingServiceMock = Substitute.For<IBookingService>();
            _lifeTimeScope = _container.BeginLifetimeScope();
            _wireCardPaymentService = new WirecardPaymentService(_wirecardPaymentAdapter, _logger);
            _paymentGatewayFactory = new PaymentGatewayFactory(_lifeTimeScope, _wirecardPaymentAdapter, _apexxAdapter, _logger,_adyenAdapter, _adyenPersistence);
            _apiStorageOperation = Substitute.For<ITableStorageOperation>();
            _cancellationHelperMock =
                new CancellationHelper(_bookingServiceMock, _cancellationServiceMock, _paymentGatewayFactory, _logger);
            _cancellationMapperMock = Substitute.For<CancellationMapper>(_apiStorageOperation);
            _cancellationControllerMock = new CancellationController(_cancellationServiceMock, _cancellationMapperMock,
                _cancellationHelperMock, _bookingServiceMock, _logger);
        }

        //[Test]
        public void GetCancellationPolicyDetail()
        {
            var spid = 0;
            var bookingRefNo = "SGI832146";
            var bookedOptionId = 124512;
            var confirmBookingDetail = new ConfirmBookingDetail { CurrencyIsoCode = "USD" };
            var cancellationPolicyAmountDetail = new CancellationPolicyDetail
            {
                BookedOptionId = 12324,
                BookedServiceId = 212,
                ServiceId = 123,
                SellingPrice = 43.23M,
                UserCancellationCharges = 0.00M,
                UserCurrencyCode = "USD",
                UserRefundAmount = 32.45M,
                //CostPrice = 33.45M,
                //SupplierCurrencySymbol = "$",
                //SupplierCancellationCharges = 0.00M,
                //SupplierCurrencyCode = "AUD",
                //SupplierRefundAmount = 9.36M,
                //TransactionId = 0,
                Guwid = "asdf",
                //AuthorizationCode = "adfasdf",
                RegPaxId = 1221
            };
            var currencyCode = _bookingServiceMock.GetBookingData(bookingRefNo).CurrencyIsoCode;
            _cancellationServiceMock.GetCancellationPolicyDetailAsync(bookingRefNo, bookedOptionId, currencyCode, spid)
                .ReturnsForAnyArgs(cancellationPolicyAmountDetail);
            _cancellationMapperMock.MapCancellationPolicyDetail(cancellationPolicyAmountDetail, currencyCode);
            var result = _cancellationControllerMock.GetCancellationPolicyDetail(bookingRefNo, bookedOptionId) as ObjectResult;


            var Content = result?.Value as CancellationPolicyDetailResponse;

            Assert.IsNotNull(Content?.UserCurrencyCode);
        }

        //[Test]
        public void CancelCitySightSeeingBookingTest()
        {
            var cancellationResponse = new CancellationResponse
            {
                Status = new CancellationStatus
                {
                    Message = "Cancellation Succeed",
                    AllCancelStatus = new AllCancelStatus
                    {
                        IsangoBookingCancel = "Success",
                        PaymentRefundStatus = "Success",
                        SupplierBookingCancel = "Success"
                    }
                },
                Remark = ""
            };
            var spid = 0;
            var authentication = "isangokey";
            var confirmBookingDetail = new ConfirmBookingDetail
            {
                CurrencyIsoCode = "USD",
                LanguageCode = "en",
                BookedOptions = new List<BookedOption> { new BookedOption { BookedOptionId = 124512 } }
            };
            var cancellationRequest = new CancellationRequest
            {
                BookingRefNo = "SGI832146",
                TokenId = "dfadsfasd",
                UserName = "hara",
                CancellationParameters = new CancellationParameters
                {
                    AlternativeDates = null,
                    AlternativeTours = "",
                    BookedOptionId = 124512,
                    CustomerNotes = "",
                    SupplierNotes = "",
                    SupplierRefundAmount = 0.00M,
                    Reason = "Plan Changed",
                    UserRefundAmount = 12.23M
                }
            };
            var cancellation = new Cancellation
            {
                TrackerStatusId = 2,
                BookingRefNo = "SGI832146",
                TokenId = "dfadsfasd",
                CancelledByUserId = "afdsfadsf",
                CancellationParameters = new Entities.Cancellation.CancellationParameters
                {
                    AlternativeDates = null,
                    AlternativeTours = "",
                    BookedOptionId = 124512,
                    CustomerNotes = "",
                    SupplierNotes = "",
                    //SupplierRefundAmount = 0.00M,
                    Reason = "Plan Changed",
                    UserRefundAmount = 12.23M
                }
            };

            var supplierData = new SupplierCancellationData
            {
                ApiType = 1,
                BookedOptionId = 124512,
                BookedOptionStatusId = 1,
                BookingReferenceNumber = "SGI832146",
                CostCurrencyCode = "USD",
                CountryId = 1919,
                FHBSupplierShortName = "FHB",
                OfficeCode = "",
                ServiceLongName = "FHB long name",
                ServiceOptionName = "2 am show",
                Status = "Pending",
                SupplierBookingLineNumber = "12123",
                SupplierBookingReferenceNumber = "ISA123",
                TravelDate = "2019-12-16 00:00:00.000",
                CountryName = "Australia"
            };
            var cancellationDetail = new ConfirmCancellationDetail
            {
                TransactionDetail = new List<TransactionDetail>(),
                SendCancellationEmail = 1
            };
            var transactiondetail = new TransactionDetail
            {
                Amount = 12.11M,
                CurrencyCode = "USD",
                FlowName = "",
                Guwid = "fadsfadsfad",
                Is3D = 1,
                Transflow = "",
                TransId = 12121,
                PaymentGatewayType = 1,
                PaymentGatewayTypeName = "WireCard"
            };
            cancellationDetail.TransactionDetail.Add(transactiondetail);
            var productOptions = new List<Entities.ProductOption>();
            productOptions.ForEach(x => x.BookingStatus = OptionBookingStatus.Confirmed);
            var selectedProduct = new CitySightseeingSelectedProduct()
            {
                ProductOptions = productOptions,
                APIType = APIType.Citysightseeing,
                AvailabilityReferenceId = "afadseytyetfadsfa",
                Pnr = "212334"
            };
            var cancellationPolicyDetail = new CancellationPolicyDetail
            {
                //AuthorizationCode = "1231",
                BookedOptionId = 124512,
                BookedServiceId = 682,
                //CostPrice = 12.00M,
                Guwid = "fadsfadsfad",
                RegPaxId = 3212,
                SellingPrice = 15.66M,
                ServiceId = 681,
                //SupplierCancellationCharges = 0.00M,
                //SupplierCurrencyCode = "AUD",
                //SupplierCurrencySymbol = "NU$",
                //SupplierRefundAmount = 0.00M,
                //TransactionId = 5162341,
                UserCancellationCharges = 0.00M,
                UserCurrencyCode = "USD",
                UserRefundAmount = 16.11M
            };

            var optionAndServiceName = new Dictionary<string, string>
                {{"ServiceName", "BaBaService"}, {"OptionName", "JantarMantar"}};
            var booking = new Booking
            {
                ReferenceNumber = cancellation.BookingRefNo,
                SelectedProducts = new List<Entities.SelectedProduct> { selectedProduct },
                Payment = new List<Entities.Payment.Payment>
                {
                    new Entities.Payment.Payment
                    {
                        JobId = "0_SGI832146", TransactionId = "12345", Guwid = "CD234123413241", Is3D = true,
                        PaymentStatus = Entities.Payment.PaymentStatus.Paid, ChargeAmount = 12.12M, CurrencyCode = "USD"
                    }
                }
            };
            CancelBookingMailDetail cancelBookingMailDetail = new CancelBookingMailDetail
            {
                TokenId = "",
                ServiceId = 9999,
                BookingReferenceNumber = "SGI82420",
                APICancellationStatus = "Success",
                PaymentRefundAmount = "420.21",
                IsangoBookingCancellationStatus = "Success",
                PaymentRefundStatus = "Success",
                TravelDate = Convert.ToDateTime("2020/2/29"),
                CustomerEmailId = "ram@shyamtravels.com",
                ContactNumber = "N/A",
                APIBookingReferenceNumber = "5215",
                ApiTypeName = "Undefined",
                ServiceName = "Kumbh Mela Darshan",
                OptionName = "River Rafting"
            };
            var wireCardPaymentResponse = new WirecardPaymentResponse { Status = "Success" };

            var bookBackResponse =
                new Tuple<WirecardPaymentResponse, string, string>(wireCardPaymentResponse, "asdf", "dfads");
            var paymentGatewayType = PaymentGatewayType.WireCard;
            var userData = new UserCancellationPermission { UserId = "fasdfasd", IsPermitted = 1 };
            var cancellationStatus = new Entities.Cancellation.CancellationStatus
            { IsangoCancelStatus = 0, BookedOptionId = 1300442, PaymentRefundStatus = 0, SupplierCancelStatus = 0 };

            _cancellationServiceMock.GetUserCancellationPermissionAsync(cancellationRequest.UserName)
                .ReturnsForAnyArgs(userData);
            _bookingServiceMock.GetBookingData(cancellation.BookingRefNo).ReturnsForAnyArgs(confirmBookingDetail);
            var cancelData =
                _cancellationMapperMock.MapCancellationRequest(cancellationRequest, userData, confirmBookingDetail);
            //uncomment code after implementation
            //_cancellationServiceMock.CreateCancelBookingIsangoDB(cancelData,,userData.IsPermitted).ReturnsForAnyArgs(cancellationDetail);

            _cancellationServiceMock.GetSupplierCancellationDataAsync(cancellation.BookingRefNo)
                .ReturnsForAnyArgs(supplierData);
            _cancellationMapperMock.MapSupplierDataForBookedProduct(supplierData, confirmBookingDetail, cancellation);
            _cancellationServiceMock
                .GetCancellationPolicyDetailAsync(cancelData.BookingRefNo, cancellation.CancellationParameters.BookedOptionId,
                    confirmBookingDetail.CurrencyIsoCode, spid).ReturnsForAnyArgs(cancellationPolicyDetail);
            var allCancelStatus = _cancellationMapperMock.MapCancellationStatus(cancellationResponse,
                cancellationRequest.CancellationParameters.BookedOptionId);
            _cancellationServiceMock.GetCancellationStatusAsync(cancellationRequest.CancellationParameters.BookedOptionId)
                .ReturnsForAnyArgs(cancellationStatus);
            _cancellationServiceMock.InsertOrUpdateCancellationStatus(allCancelStatus);
            _bookingServiceMock
                .GetOptionAndServiceNameAsync(cancelData.BookingRefNo, false,
                    cancellation.CancellationParameters.BookedOptionId.ToString()).ReturnsForAnyArgs(optionAndServiceName);
            _cancellationMapperMock.MapCancelBookingMailDetail(confirmBookingDetail, supplierData,
                optionAndServiceName, cancellation.TokenId);
            _cancellationServiceMock.CreateCancelBookingIsangoDbAsync(cancellation, cancellationPolicyDetail, spid, userData)
                .ReturnsForAnyArgs(cancellationDetail);
            //_lifeTimeScope.Resolve<WirecardPaymentService>().ReturnsForAnyArgs(_wireCardPaymentService);
            _paymentGatewayFactory.GetPaymentGatewayService(paymentGatewayType);
            _wirecardPaymentAdapter.BookBack(booking.Payment?.FirstOrDefault(), cancellation.TokenId)
                .ReturnsForAnyArgs(bookBackResponse);
            _cancellationHelperMock.CancelAndGetIsangoDbCancellationAndPaymentRefundStatus(cancellation, cancellationPolicyDetail,
                spid, userData, cancellationStatus);
            // _wireCardPaymentService.Refund(booking, cancellation.CancelParameters.Reason, cancellation.TokenId).ReturnsForAnyArgs(true);
            _bookingServiceMock
                .CancelSupplierBookingAsync(booking, cancellation.TokenId, true)
                .ReturnsForAnyArgs(true);
            _cancellationServiceMock.SendCancelBookingMail(cancelBookingMailDetail);
            _cancellationServiceMock.InsertOrUpdateCancellationStatus(allCancelStatus);
            var result =
                _cancellationControllerMock.CancelBooking(cancellationRequest) as
                    ObjectResult;
            var Content = result?.Value as CancellationResponse;

            Assert.That(Content?.Status.Message, Is.EqualTo(cancellationResponse.Status.Message));
        }

        //[Test]
        public void CancelBigBusBookingTest()
        {
            var cancellationResponse = new CancellationResponse
            {
                Status = new CancellationStatus
                {
                    Message = "Cancellation Succeed",
                    AllCancelStatus = new AllCancelStatus
                    {
                        IsangoBookingCancel = "Success",
                        PaymentRefundStatus = "Success",
                        SupplierBookingCancel = "Success"
                    }
                },
                Remark = ""
            };
            var wireCardPaymentResponse = new WirecardPaymentResponse { Status = "Success" };
            var bookBackResponse =
                new Tuple<WirecardPaymentResponse, string, string>(wireCardPaymentResponse, "asdf", "dfads");
            var optionAndServiceName = new Dictionary<string, string>
                {{"ServiceName", "BaBaService"}, {"OptionName", "JantarMantar"}};
            var cancellationPolicyDetail = new CancellationPolicyDetail
            {
                //AuthorizationCode = "1231",
                BookedOptionId = 124512,
                BookedServiceId = 682,
                //CostPrice = 12.00M,
                Guwid = "fadsfadsfad",
                RegPaxId = 3212,
                SellingPrice = 15.66M,
                ServiceId = 681,
                //SupplierCancellationCharges = 0.00M,
                //SupplierCurrencyCode = "AUD",
                //SupplierCurrencySymbol = "NU$",
                //SupplierRefundAmount = 0.00M,
                //TransactionId = 5162341,
                UserCancellationCharges = 0.00M,
                UserCurrencyCode = "USD",
                UserRefundAmount = 16.11M
            };
            var spid = 0;
            var confirmBookingDetail = new ConfirmBookingDetail
            {
                CurrencyIsoCode = "USD",
                LanguageCode = "en",
                BookingReferenceNumber = "SGI832146",
                BookedOptions = new List<BookedOption> { new BookedOption { BookedOptionId = 124512 } }
            };
            var cancellationRequest = new CancellationRequest
            {
                BookingRefNo = "SGI832146",
                TokenId = "dfadsfasd",
                UserName = "hara",
                CancellationParameters = new CancellationParameters
                {
                    AlternativeDates = null,
                    AlternativeTours = "",
                    BookedOptionId = 124512,
                    CustomerNotes = "",
                    SupplierNotes = "",
                    SupplierRefundAmount = 0.00M,
                    Reason = "Plan Changed",
                    UserRefundAmount = 12.23M
                }
            };
            var cancellation = new Cancellation
            {
                TrackerStatusId = 2,
                BookingRefNo = "SGI832146",
                TokenId = "dfadsfasd",
                CancelledByUserId = "afdsfadsf",
                CancellationParameters = new Entities.Cancellation.CancellationParameters
                {
                    AlternativeDates = null,
                    AlternativeTours = "",
                    BookedOptionId = 124512,
                    CustomerNotes = "",
                    SupplierNotes = "",
                    //SupplierRefundAmount = 0.00M,
                    Reason = "Plan Changed",
                    UserRefundAmount = 12.23M
                }
            };
            var paymentGatewayType = PaymentGatewayType.WireCard;
            var userData = new UserCancellationPermission { UserId = "fasdfasd", IsPermitted = 1 };
            var supplierData = new SupplierCancellationData
            {
                ApiType = 1,
                BookedOptionId = 124512,
                BookedOptionStatusId = 1,
                BookingReferenceNumber = "SGI832146",
                CostCurrencyCode = "USD",
                CountryId = 1919,
                FHBSupplierShortName = "FHB",
                OfficeCode = "",
                ServiceLongName = "FHB long name",
                ServiceOptionName = "2 am show",
                Status = "Pending",
                SupplierBookingLineNumber = "12123",
                SupplierBookingReferenceNumber = "ISA123",
                TravelDate = "2019-12-16 00:00:00.000",
                CountryName = "Australia"
            };

            var authentication = "isangokey";
            var cancellationDetail = new ConfirmCancellationDetail
            {
                TransactionDetail = new List<TransactionDetail>(),
                SendCancellationEmail = 1
            };
            var transdetail = new TransactionDetail
            {
                Amount = 12.11M,
                CurrencyCode = "USD",
                FlowName = "",
                Guwid = "fadsfadsfad",
                Is3D = 1,
                Transflow = "",
                TransId = 12121,
                PaymentGatewayType = 1,
                PaymentGatewayTypeName = "WireCard"
            };
            cancellationDetail.TransactionDetail.Add(transdetail);
            var availabilityReferenceId = "adsfadsfadsfadsf";
            var bigBusProduct = new BigBusSelectedProduct()
            {
                AvailabilityReferenceId = availabilityReferenceId,
                BookingStatus = BigBusApiStatus.Booked,
                BookingReference = supplierData.BookingReferenceNumber
            };

            var booking = new Booking
            {
                SelectedProducts = new List<Entities.SelectedProduct> { bigBusProduct }
            };
            booking.SelectedProducts.ForEach(x => x.AvailabilityReferenceId = availabilityReferenceId);
            CancelBookingMailDetail cancelBookingMailDetail = new CancelBookingMailDetail
            {
                TokenId = "",
                ServiceId = 9999,
                BookingReferenceNumber = "SGI82420",
                APICancellationStatus = "Success",
                PaymentRefundAmount = "420.21",
                IsangoBookingCancellationStatus = "Success",
                PaymentRefundStatus = "Success",
                TravelDate = Convert.ToDateTime("2020/2/29"),
                CustomerEmailId = "ram@shyamtravels.com",
                ContactNumber = "N/A",
                APIBookingReferenceNumber = "5215",
                ApiTypeName = "Undefined",
                ServiceName = "Kumbh Mela Darshan",
                OptionName = "River Rafting"
            };
            var cancellationStatus = new Entities.Cancellation.CancellationStatus
            { IsangoCancelStatus = 0, BookedOptionId = 1300442, PaymentRefundStatus = 0, SupplierCancelStatus = 0 };

            _cancellationServiceMock.GetUserCancellationPermissionAsync(cancellationRequest.UserName)
                .ReturnsForAnyArgs(userData);
            _bookingServiceMock.GetBookingData(cancellation.BookingRefNo).ReturnsForAnyArgs(confirmBookingDetail);
            var cancelData =
                _cancellationMapperMock.MapCancellationRequest(cancellationRequest, userData, confirmBookingDetail);
            //uncomment code after implementation
            //_cancellationServiceMock.CreateCancelBookingIsangoDB(cancelData,,userData.IsPermitted).ReturnsForAnyArgs(cancellationDetail);
            _cancellationServiceMock.GetSupplierCancellationDataAsync(cancellation.BookingRefNo)
                .ReturnsForAnyArgs(supplierData);
            _cancellationMapperMock.MapSupplierDataForBookedProduct(supplierData, confirmBookingDetail, cancellation);
            _cancellationServiceMock
                .GetCancellationPolicyDetailAsync(cancelData.BookingRefNo, cancellation.CancellationParameters.BookedOptionId,
                    confirmBookingDetail.CurrencyIsoCode, spid).ReturnsForAnyArgs(cancellationPolicyDetail);
            var allCancelStatus = _cancellationMapperMock.MapCancellationStatus(cancellationResponse,
                cancellationRequest.CancellationParameters.BookedOptionId);
            _cancellationServiceMock.GetCancellationStatusAsync(cancellationRequest.CancellationParameters.BookedOptionId)
                .ReturnsForAnyArgs(cancellationStatus);
            _cancellationServiceMock.InsertOrUpdateCancellationStatus(allCancelStatus);
            _bookingServiceMock
                .GetOptionAndServiceNameAsync(cancelData.BookingRefNo, false,
                    cancellation.CancellationParameters.BookedOptionId.ToString()).ReturnsForAnyArgs(optionAndServiceName);
            _cancellationMapperMock.MapCancelBookingMailDetail(confirmBookingDetail, supplierData,
                optionAndServiceName, cancellation.TokenId);
            _cancellationServiceMock.CreateCancelBookingIsangoDbAsync(cancellation, cancellationPolicyDetail, spid, userData)
                .ReturnsForAnyArgs(cancellationDetail);
            //_lifeTimeScope.Resolve<WirecardPaymentService>().ReturnsForAnyArgs(_wireCardPaymentService);
            _paymentGatewayFactory.GetPaymentGatewayService(paymentGatewayType);
            _wirecardPaymentAdapter.BookBack(booking.Payment?.FirstOrDefault(), cancellation.TokenId)
                .ReturnsForAnyArgs(bookBackResponse);
            _cancellationHelperMock.CancelAndGetIsangoDbCancellationAndPaymentRefundStatus(cancellation, cancellationPolicyDetail,
                spid, userData, cancellationStatus);
            // _wireCardPaymentService.Refund(booking, cancellation.CancelParameters.Reason, cancellation.TokenId).ReturnsForAnyArgs(true);
            _bookingServiceMock
                .CancelSupplierBookingAsync(booking, cancellation.TokenId, true)
                .ReturnsForAnyArgs(true);
            _cancellationServiceMock.SendCancelBookingMail(cancelBookingMailDetail);
            _cancellationServiceMock.InsertOrUpdateCancellationStatus(allCancelStatus);
            

            var result =
                _cancellationControllerMock.CancelBooking(cancellationRequest) as
                    ObjectResult;
            var Content = result?.Value as CancellationResponse;
            Assert.That(Content?.Status.Message, Is.EqualTo(cancellationResponse.Status.Message));
        }
    }
}