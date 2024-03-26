using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Cancellation;
using Isango.Entities.Mailer;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using Util;
using CancellationPolicyDetail = Isango.Entities.Cancellation.CancellationPolicyDetail;

namespace Isango.Service
{
    public class CancellationService : ICancellationService
    {
        private readonly ILogger _log;
        private readonly ICancellationPersistence _cancellationPersistence;
        private readonly IMailerService _mailerService;

        public CancellationService(ICancellationPersistence cancellationPersistence, ILogger log,
            IMailerService mailerService)
        {
            _cancellationPersistence = cancellationPersistence;
            _log = log;
            _mailerService = mailerService;
        }

        /// <summary>
        /// Get user id and permission to modify refund amount
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<UserCancellationPermission> GetUserCancellationPermissionAsync(string userName)
        {
            try
            {
                var userData = _cancellationPersistence.GetUserPermissionForCancellation(userName);
                return await Task.FromResult(userData);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationService",
                    MethodName = "GetUserCancellationPermissionAsync",
                    Params = $"{userName}",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get cancellation policy detail
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <param name="bookedOptionId"></param>
        /// <param name="currencyIsoCode"></param>
        /// <param name="spId"></param>
        /// <returns></returns>
        public async Task<CancellationPolicyDetail> GetCancellationPolicyDetailAsync(string bookingRefNo,
            int bookedOptionId, string currencyIsoCode, int spId)
        {
            try
            {
                var cancellationPolicyDetails =
                    _cancellationPersistence.GetCancellationPolicyDetail(bookingRefNo, bookedOptionId, currencyIsoCode,
                        spId);
                return await Task.FromResult(cancellationPolicyDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationService",
                    MethodName = "GetCancellationPolicyDetailAsync",
                    Params = $"{bookingRefNo}{bookedOptionId}{currencyIsoCode}{spId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Create cancel booking in isango db
        /// </summary>
        /// <param name="cancellation"></param>
        /// <param name="cancellationPolicy"></param>
        /// <param name="spId"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public async Task<ConfirmCancellationDetail> CreateCancelBookingIsangoDbAsync(Cancellation cancellation,
            CancellationPolicyDetail cancellationPolicy, int spId, UserCancellationPermission userData)
        {
            ConfirmCancellationDetail confirmCancellationData = null;
            try
            {
                var tokenValue = cancellation.BookingRefNo + "_" + cancellation.CancellationParameters.BookedOptionId;
               
                cancellation.CancellationParameters.Guwid = cancellationPolicy.Guwid;
                //cancellation.CancellationParameters.SupplierRefundAmount = cancellationPolicy.SupplierRefundAmount;
                if (cancellation.CancellationParameters.CurrencyCode == null)
                    cancellation.CancellationParameters.CurrencyCode = cancellationPolicy.UserCurrencyCode;
                if (userData.IsPermitted != 1)
                    cancellation.CancellationParameters.UserRefundAmount = cancellationPolicy.UserRefundAmount;
                logInfoSave(tokenValue, "CreateCancelBookingIsangoDbAsyncStart", SerializeDeSerializeHelper.Serialize(cancellation));
                logInfoSave(tokenValue, "CreateCancelBookingIsangoDbAsyncStartCancellationParameter", SerializeDeSerializeHelper.Serialize(cancellation.CancellationParameters));
                //create cancel booking in isango db
                confirmCancellationData = _cancellationPersistence.CreateCancelBooking(cancellation,
                    cancellation.BookingRefNo, cancellation.CancelledByUserId, cancellation.CancelledByUser);
                logInfoSave(tokenValue, "CreateCancelBookingIsangoDbAsyncEnd", SerializeDeSerializeHelper.Serialize(confirmCancellationData));
                return await Task.FromResult(confirmCancellationData);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationService",
                    MethodName = "CreateCancelBookingIsangoDbAsync",
                    Token = cancellation.TokenId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(cancellation)}{SerializeDeSerializeHelper.Serialize(cancellationPolicy)}{spId}{SerializeDeSerializeHelper.Serialize(userData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                return await Task.FromResult(confirmCancellationData);
            }
        }

        /// <summary>
        /// Get parameters required for supplier booking cancellation
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <returns></returns>
        public async Task<SupplierCancellationData> GetSupplierCancellationDataAsync(string bookingRefNo)
        {
            try
            {
                var supplierCancellationData = _cancellationPersistence.GetSupplierCancellationData(bookingRefNo);
                return await Task.FromResult(supplierCancellationData);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationService",
                    MethodName = "GetSupplierCancellationDataAsync",
                    Params = $"{bookingRefNo}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public async Task<List<SupplierCancellationData>> GetSupplierCancellationDataListAsync(string bookingRefNo)
        {
            try
            {
                var supplierCancellationData = _cancellationPersistence.GetSupplierCancellationDataList(bookingRefNo);
                return await Task.FromResult(supplierCancellationData);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationService",
                    MethodName = "GetSupplierCancellationDataListAsync",
                    Params = $"{bookingRefNo}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        /// <summary>
        /// Send cancel booking mail in both cases of success and failure
        /// </summary>
        /// <param name="cancelBookingMailDetail"></param>
        public void SendCancelBookingMail(CancelBookingMailDetail cancelBookingMailDetail)
        {
            try
            {
                var cancellationMailText = new CancellationMailText
                {
                    TokenId = cancelBookingMailDetail.TokenId,
                    ServiceId = cancelBookingMailDetail.ServiceId,
                    BookingReferenceNumber = cancelBookingMailDetail.BookingReferenceNumber,
                    SupplierCancellationStatus = cancelBookingMailDetail.APICancellationStatus,
                    PaymentRefundAmount = cancelBookingMailDetail.PaymentRefundAmount,
                    IsangoBookingCancellationStatus = cancelBookingMailDetail.IsangoBookingCancellationStatus,
                    PaymentRefundStatus = cancelBookingMailDetail.PaymentRefundStatus,
                    TravelDate = cancelBookingMailDetail.TravelDate,
                    CustomerEmailId = cancelBookingMailDetail.CustomerEmailId,
                    ContactNumber = cancelBookingMailDetail.ContactNumber,
                    APIBookingReferenceNumber = cancelBookingMailDetail.APIBookingReferenceNumber,
                    ApiTypeName = cancelBookingMailDetail.ApiTypeName,
                    ServiceName = cancelBookingMailDetail.ServiceName,
                    OptionName = cancelBookingMailDetail.OptionName
                };

                var cancellationMailContextList = new List<CancellationMailText>
                {
                    cancellationMailText
                };

                //Send cancel booking mail to customer support / Customer / Supplier
                _mailerService.SendCancelBookingMail(cancellationMailContextList);
                var bookedOptionMailDatas = _cancellationPersistence.CheckToSendmailToCustomer(cancelBookingMailDetail.BookedOptionID ?? 0);
                if (bookedOptionMailDatas.Any())
                {
                    _mailerService.SendMail(cancellationMailContextList.FirstOrDefault().BookingReferenceNumber, null, false, false, null, null, true, false);
                }
                _mailerService.SendSupplierCancelMail(cancellationMailContextList.FirstOrDefault().BookingReferenceNumber);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationService",
                    MethodName = "SendCancelBookingMail",
                    Params = $"{SerializeDeSerializeHelper.Serialize(cancelBookingMailDetail)}",
                    Token = cancelBookingMailDetail.TokenId
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get cancellation status of a product of all three steps
        /// </summary>
        /// <param name="bookedOptionId"></param>
        /// <returns></returns>
        public async Task<CancellationStatus> GetCancellationStatusAsync(int bookedOptionId)
        {
            try
            {
                var cancellationStatus = _cancellationPersistence.GetCancellationStatus(bookedOptionId);
                return await Task.FromResult(cancellationStatus);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationService",
                    MethodName = "GetCancellationStatusAsync",
                    Params = $"{bookedOptionId}",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Insert or Update cancellation status of all three steps
        /// </summary>
        /// <param name="cancellationStatus"></param>
        public void InsertOrUpdateCancellationStatus(CancellationStatus cancellationStatus)
        {
            try
            {
                _cancellationPersistence.InsertOrUpdateCancellationStatus(cancellationStatus);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationService",
                    MethodName = "InsertOrUpdateCancellationStatus",
                    Params = $"{SerializeDeSerializeHelper.Serialize(cancellationStatus)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Update Guwid and other details for Cancelled Transaction (Bookback and reversal call) in DB.
        /// </summary>
        /// <param name="transId"></param>
        /// <param name="paymentGatewayResponse"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public void UpdateCancelBookingIsangoDbAsync(int transId, PaymentGatewayResponse paymentGatewayResponse, string token)
        {
            try
            {
                //update cancel booking in isango db
                _cancellationPersistence.UpdateCancelBooking(transId, paymentGatewayResponse);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationService",
                    MethodName = "CreateCancelBookingIsangoDbAsync",
                    Token = token,
                    Params = $"{transId}{SerializeDeSerializeHelper.Serialize(paymentGatewayResponse)}{token}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
        }

        private void logInfoSave(string token, string dataPass, string logData = "")
        {
            var logInfo = dataPass;
            var isangoLogEntity = new IsangoErrorEntity
            {
                ClassName = "CancellationService",
                MethodName = "CreateCancelBookingIsangoDbAsync",
                Token = token,
                Params = !String.IsNullOrEmpty(logData) ? (logInfo + "," + logData) : logInfo
            };
            _log.Info(isangoLogEntity);
        }

    }
}