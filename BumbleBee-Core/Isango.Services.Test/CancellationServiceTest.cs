using Isango.Persistence.Contract;
using Isango.Register;
using Isango.Service;
using Isango.Service.Contract;
using Logger.Contract;
using NSubstitute;
using NUnit.Framework;
using System;
using Isango.Entities.Cancellation;
using Autofac;

namespace Isango.Services.Test
{
    [TestFixture]
    public class CancellationServiceTest : BaseTest
    {
        private ICancellationService _cancellationServiceMock;
        private ICancellationPersistence _cancellationPersistenceMock;
        private ILogger _logger;
        private IMailerService _mailerService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _cancellationPersistenceMock = _container.Resolve<ICancellationPersistence>();
            _logger = _container.Resolve<ILogger>();
            _mailerService = _container.Resolve<IMailerService>();
            _cancellationServiceMock = new CancellationService(
                _cancellationPersistenceMock, _logger, _mailerService);

        }

        [Test]
        public void GetCancellationPolicyAmountDetailTest()
        {
            var bookingRefNo = "SGI832146";
            var bookedOptionId = 1300245;
            var currencyIsoCode = "USD";
            var spid = 0;

            var cancellationPolicyAmountDetails = new CancellationPolicyDetail
            {
                BookedOptionId = 123,
                BookedServiceId = 123,
                ServiceId = 21,
                SellingPrice = 10.00M,
                UserCancellationCharges = 9.00M,
                UserCurrencyCode = "USD",
                UserRefundAmount = 6.5M,
                //CostPrice = 1.2M,
                //SupplierCancellationCharges = 0.00M,
                //SupplierCurrencySymbol = "$",
                //SupplierRefundAmount = 3.0M,
                //SupplierCurrencyCode = "EUR",
               // TransactionId = 123,
                //AuthorizationCode = "",
                Guwid = "",
                RegPaxId = 1234
            };

            _cancellationPersistenceMock
                .GetCancellationPolicyDetail(bookingRefNo, bookedOptionId, currencyIsoCode, spid)
                .ReturnsForAnyArgs(cancellationPolicyAmountDetails);
            var result =
                _cancellationServiceMock.GetCancellationPolicyDetailAsync(bookingRefNo, bookedOptionId, currencyIsoCode,
                    spid);

            Assert.IsNotNull(result);
        }

        [Test]
        public void CancelBookingInIsangoDbTest()
        {
            var bookingRefNo = "SGI832146";
            var bookedOptionId = 1300245;
            var currencyIsoCode = "USD";
            var spid = 0;
            var userCancellationPermission = new UserCancellationPermission
            { IsPermitted = 1, UserId = "fasdfasdfasdfasdf" };
            var cancellation = new Cancellation
            {
                TrackerStatusId = 1,
                BookingRefNo = "SGI832146",
                CancelledByUser = 1,
                CancelledByUserId = "",
                CancellationParameters = new CancellationParameters
                {
                    AlternativeDates = null,
                    AlternativeTours = "",
                    BookedOptionId = 123,
                    CurrencyCode = "USD",
                    CustomerNotes = "",
                    Guwid = "asdfadsgeerqewrq",
                    SupplierCurrencyCode = "USD",
                    SupplierNotes = "",
                    //SupplierRefundAmount = 0.00M,
                    Reason = "Plan Changed",
                    UserRefundAmount = 10.98M
                },
                TokenId = "adsfadsfagwer"
            };
            var cancellationPolicyAmountDetails = new CancellationPolicyDetail
            {
                BookedOptionId = 123,
                BookedServiceId = 123,
                ServiceId = 21,
                SellingPrice = 10.00M,
                UserCancellationCharges = 9.00M,
                UserCurrencyCode = "USD",
                UserRefundAmount = 6.5M,
                //CostPrice = 1.2M,
                //SupplierCancellationCharges = 0.00M,
                //SupplierCurrencySymbol = "$",
                //SupplierRefundAmount = 3.0M,
               // SupplierCurrencyCode = "EUR",
                //TransactionId = 123,
                //AuthorizationCode = "",
                Guwid = "",
                RegPaxId = 1234
            };
            var confirmCancellationDetails = new ConfirmCancellationDetail
            {
                TransactionDetail = new System.Collections.Generic.List<TransactionDetail>(),
                SendCancellationEmail = 1
            };
            var transdetail = new TransactionDetail
            {
                Amount = 10.98M,
                CurrencyCode = "USD",
                FlowName = "BookBack",
                Guwid = "asdfadsgeerqewrq",
                Is3D = 1,
                Transflow = "2",
                TransId = 12321
            };
            confirmCancellationDetails.TransactionDetail.Add(transdetail);
            _cancellationPersistenceMock
                .GetCancellationPolicyDetail(bookingRefNo, bookedOptionId, currencyIsoCode, spid)
                .ReturnsForAnyArgs(cancellationPolicyAmountDetails);
            _cancellationPersistenceMock
                .CreateCancelBooking(cancellation, cancellation.BookingRefNo, cancellation.CancelledByUserId,
                    cancellation.CancelledByUser).ReturnsForAnyArgs(confirmCancellationDetails);
            var result = _cancellationServiceMock.CreateCancelBookingIsangoDbAsync(cancellation,
                cancellationPolicyAmountDetails, spid, userCancellationPermission);
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetSupplierCancellationDataTest()
        {
            var bookingRefNo = "SGI832107";
            var supplierCancellationData = new SupplierCancellationData
            {
                ApiType = 8,
                BookedOptionId = 12312,
                BookedOptionStatusId = 2,
                BookingReferenceNumber = "SGI832107",
                CostCurrencyCode = "USD",
                CountryId = 6169,
                FHBSupplierShortName = "",
                OfficeCode = "",
                ServiceLongName = "2 Day Uluru Highlights",
                ServiceOptionName = "2 Day Uluru Highlights (Budget)",
                Status = "Pending",
                SupplierBookingLineNumber = "17090435",
                SupplierBookingReferenceNumber = "AIIFJS3409",
                TravelDate = "16-12-2019  00:00:00"
            };
            _cancellationPersistenceMock.GetSupplierCancellationData(bookingRefNo)
                .ReturnsForAnyArgs(supplierCancellationData);
            var result = _cancellationServiceMock.GetSupplierCancellationDataAsync(bookingRefNo);
            Assert.IsNotNull(result);
        }

        [Test]
        public void UserPermittedToChangeRefundAmount()
        {
            var userName = "hara";
            var userData = new UserCancellationPermission { UserId = "asdfasdfaweetrgsdf", IsPermitted = 1 };
            _cancellationPersistenceMock.GetUserPermissionForCancellation(userName).ReturnsForAnyArgs(userData);
            var user = _cancellationServiceMock.GetUserCancellationPermissionAsync(userName).GetAwaiter().GetResult();
            Assert.AreEqual(user.IsPermitted, 1);
        }

        [Test]
        public void UserNotPermittedToChangeRefundAmount()
        {
            var userName = "shubham";
            var userData = new UserCancellationPermission { UserId = "afdsftwreewyrtym", IsPermitted = 0 };
            _cancellationPersistenceMock.GetUserPermissionForCancellation(userName).ReturnsForAnyArgs(userData);
            var user = _cancellationServiceMock.GetUserCancellationPermissionAsync(userName).GetAwaiter().GetResult();
            Assert.AreEqual(user.IsPermitted, 0);
        }

        [Test]
        [Ignore("Just to check the functinality of sending mail is working or not")]
        public void SendCancelBookingMailTest()
        {
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
            _cancellationServiceMock.SendCancelBookingMail(cancelBookingMailDetail);
        }

        [Test]
        public void GetCancellationStatus()
        {
            var bookedOptionID = 1300442;
            var cancellationStatus = new CancellationStatus
            { IsangoCancelStatus = 1, PaymentRefundStatus = 1, SupplierCancelStatus = 1 };
            _cancellationPersistenceMock.GetCancellationStatus(bookedOptionID).ReturnsForAnyArgs(cancellationStatus);
            var status = _cancellationServiceMock.GetCancellationStatusAsync(bookedOptionID).GetAwaiter().GetResult();
            Assert.AreEqual(status.IsangoCancelStatus, cancellationStatus.IsangoCancelStatus);
        }

        [Test]
        [Ignore("Just to insert or update canncellation status in db")]
        public void InsertOrUpdateCancellationStatus()
        {
            var cancellationStatus = new CancellationStatus
            { BookedOptionId = 1300442, IsangoCancelStatus = 1, PaymentRefundStatus = 1, SupplierCancelStatus = 1 };
            _cancellationServiceMock.InsertOrUpdateCancellationStatus(cancellationStatus);
        }
    }
}