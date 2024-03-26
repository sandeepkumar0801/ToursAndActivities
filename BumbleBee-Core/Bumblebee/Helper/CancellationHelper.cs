using Isango.Entities.Booking;
using Isango.Entities.Enums;
using Isango.Entities.Payment;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Isango.Service.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Isango.Entities.Cancellation;
using WebAPI.Models.RequestModels;
using System.Threading.Tasks;
using Logger.Contract;
using Util;
using System.Diagnostics;
using ILogger = Logger.Contract.ILogger;

namespace WebAPI.Helper
{
    public class CancellationHelper
    {
        private readonly IBookingService _bookingService;
        private readonly ICancellationService _cancellationService;
        private readonly PaymentGatewayFactory _paymentGatewayFactory;
        private readonly ILogger _log;
        public CancellationHelper(IBookingService bookingService, ICancellationService cancellationService,
            PaymentGatewayFactory paymentGatewayFactory, ILogger log)
        {
            _bookingService = bookingService;
            _cancellationService = cancellationService;
            _paymentGatewayFactory = paymentGatewayFactory;
            _log = log;
        }

        /// <summary>
        /// Get Booking Details with tracker status id
        /// </summary>
        /// <param name="cancellationRequest"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public ConfirmBookingDetail GetBookingDetails(CancellationRequest cancellationRequest, UserCancellationPermission userData)
        {
            //getting availability reference id and booking details
            var bookingData = _bookingService.GetBookingData(cancellationRequest.BookingRefNo);

            //getting tracker status id
            var bookingOptionsData = _bookingService
                .GetBookingDetailAsync(cancellationRequest.BookingRefNo, userData.UserId, null)
                .GetAwaiter().GetResult();

            //get booked option from bookedOptionsData
            var bookedOption = bookingOptionsData.FirstOrDefault(x => x.BookingDetailId == cancellationRequest.CancellationParameters.BookedOptionId);

            //check for booked option details
            if (bookingData.BookedOptions.All(x =>
                x.BookedOptionId != cancellationRequest.CancellationParameters.BookedOptionId))
            {
                throw new Exception($"BookedOptionId {cancellationRequest.CancellationParameters.BookedOptionId} not found in booking {cancellationRequest.BookingRefNo}. Please check booking detail response at endpoint : /api/booking/detail/{cancellationRequest.BookingRefNo}");

            }

            //map option status id to booking data
            bookingData.BookedOptions.ForEach(m =>
            {
                if (m.BookedOptionId == bookedOption?.BookingDetailId)
                    m.BookedOptionStatusId = bookedOption.TrakerStatusId;
            });
            return bookingData;
        }

        /// <summary>
        /// Get user id and cancellation permission by user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserCancellationPermission GetUserCancellationPermission(string userName)
        {
            var userData = _cancellationService.GetUserCancellationPermissionAsync(userName).GetAwaiter().GetResult();
            return userData;
        }

        /// <summary>
        /// check the product is non-cancellable
        /// </summary>
        /// <param name="supplierData"></param>
        /// <returns></returns>
        public bool CheckNonCancellableProduct(SupplierCancellationData supplierData)
        {
            var nonCancellableSuppliers = new List<APIType>
            {
                APIType.Goldentours,
                APIType.Moulinrouge
            };

            return supplierData != null && nonCancellableSuppliers.Contains((APIType)supplierData.ApiType);
        }

        /// <summary>
        /// Get isango db cancellation status after cancel booking in db and refund status of single product
        /// </summary>
        /// <param name="cancellation"></param>
        /// <param name="cancellationPolicy"></param>
        /// <param name="spId"></param>
        /// <param name="userData"></param>
        /// <param name="cancellationStatus"></param>
        /// <returns></returns>
        public Dictionary<string, string> CancelAndGetIsangoDbCancellationAndPaymentRefundStatus(Cancellation cancellation,
            CancellationPolicyDetail cancellationPolicy, int spId, UserCancellationPermission userData,
            CancellationStatus cancellationStatus)
        {
            var tokenValue = cancellation.BookingRefNo + "_" + cancellation.CancellationParameters.BookedOptionId;
            var TimerMethodName = "CancelBooking" + "_" + tokenValue;

            var status = new Dictionary<string, string>
            {
                {"isCancelled", OptionCancelStatus.Failed.ToString()},
                {"isRefunded", OptionCancelStatus.Failed.ToString()}
            };
            var isRefunded = false;
            PaymentGatewayResponse gatewayResponse = new PaymentGatewayResponse();
            logInfoSave(tokenValue, "CancelAndGetIsangoDbCancellationAndPaymentRefundStatus|Start");
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { Timeout = new TimeSpan(0, 3, 0) }))
            {
                try
                {
                    //if isango db cancellation has not been done
                    if (cancellationStatus.IsangoCancelStatus == (int)OptionCancelStatus.Failed)
                    {
                        //cancel booked product in isango db

                        var watchCreateCancelBookingIsangoDbAsync = Stopwatch.StartNew();
                        var cancelBookingInIsangoDb = _cancellationService
                            .CreateCancelBookingIsangoDbAsync(cancellation, cancellationPolicy, spId, userData).GetAwaiter()
                            .GetResult();
                        watchCreateCancelBookingIsangoDbAsync.Stop();
                        _log.WriteTimer("cancelBookingInIsangoDb", TimerMethodName, string.Empty, watchCreateCancelBookingIsangoDbAsync.Elapsed.ToString());
                        logInfoSave(tokenValue, "CancellationHelper|cancelBookingInIsangoDb:Response", SerializeDeSerializeHelper.Serialize(cancelBookingInIsangoDb));
                        if (cancelBookingInIsangoDb == null)
                        {
                            logInfoSave(tokenValue, "cancelBookingInIsangoDb is Null");
                            return status;
                        }
                        else if((cancelBookingInIsangoDb?.TransactionDetail?.Count <= 0)
                            || String.IsNullOrEmpty(cancelBookingInIsangoDb?.TransactionDetail?.FirstOrDefault().Guwid))
                        {
                            logInfoSave(tokenValue, "TransactionDetail is Null");
                            status["isCancelled"] = OptionCancelStatus.Success.ToString();
                            status["isRefunded"] = OptionCancelStatus.NotApplicable.ToString();
                            transactionScope.Complete();
                            return status;
                        }
                        //we will be proceeding for confirmed product for payment refund
                        // 1  On Request
                        // 2  Confirmed
                        // 4  Confirmed
                        // 3  Cancelled
                        if (cancellation.TrackerStatusId != (int)OptionBookingStatus.Requested && cancellation.TrackerStatusId != (int)OptionBookingStatus.Cancelled)
                        {
                            //for transaction booking - we are getting a valid transaction id from db
                            //for prepaid booking - we are getting zero transaction id from db
                            //if (cancellationPolicy?.TransactionId <= 0
                               // || System.Convert.ToInt32(cancellationPolicy?.UserRefundAmount) == 0
                            if (cancelBookingInIsangoDb?.TransactionDetail?.FirstOrDefault().TransId <= 0
                                || cancellationPolicy?.UserRefundAmount == 0)

                            {
                                logInfoSave(tokenValue, "CancellationHelper|TransactionId is Zero or UserRefundAmount is Zero");
                                status["isCancelled"] = OptionCancelStatus.Success.ToString();
                                status["isRefunded"] = OptionCancelStatus.NotApplicable.ToString();
                                transactionScope.Complete();
                            }
                            else
                            {
                                status["isCancelled"] = OptionCancelStatus.Success.ToString();

                                //foreach will be implemented for multiple guwid
                                foreach(var transaction in cancelBookingInIsangoDb.TransactionDetail)
                                {
                                    //Mapping payment detail in booking object
                                    logInfoSave(tokenValue, "CancellationHelper|CreateBookingPaymentObject,Start");
                                    var payment = CreateBookingPaymentObject(transaction,
                                        cancellation.BookingRefNo);
                                    logInfoSave(tokenValue, "CancellationHelper|CreateBookingPaymentObject,End");
                                    var booking = new Booking { Payment = new List<Payment> { payment }, Date =  transaction.BookingDate ?? DateTime.Now,
                                        AdyenMerchantAccountCancelRefund= transaction?.AdyenMerchantAccount};

                                    //get payment gateway
                                    var paymentGateway = _paymentGatewayFactory.GetPaymentGatewayService(
                                        (PaymentGatewayType)transaction.PaymentGatewayType);
                                    if (paymentGateway == null)
                                    {
                                        logInfoSave(tokenValue, "CancellationHelper|paymentGateway is Null");
                                        status["isRefunded"] = OptionCancelStatus.NotApplicable.ToString();
                                        //transactionScope.Complete();
                                    }
                                    else
                                    {  // logic for payment refund process according to flow name
                                        logInfoSave(tokenValue, "CancellationHelper|paymentRefundProcess is Start");
                                        try
                                        {
                                            switch (transaction.FlowName)
                                            {
                                                case "BookBack":
                                                case "Payment":
                                                case "Capture":
                                                    var watchRefund = Stopwatch.StartNew();
                                                    logInfoSave(tokenValue, "CancellationHelper|RefundSwith  Start");
                                                    gatewayResponse = paymentGateway.Refund(booking,
                                                        cancellation.CancellationParameters.Reason,
                                                        cancellation.TokenId);
                                                    isRefunded = gatewayResponse.IsSuccess;
                                                    //if (gatewayResponse.IsSuccess && (PaymentGatewayType)transaction.PaymentGatewayType == PaymentGatewayType.Adyen)
                                                    //{
                                                    //    isRefunded = paymentGateway.WebhookConfirmation(cancellation.BookingRefNo, 2, "refund")?.GetAwaiter().GetResult() ?? false;
                                                    //}
                                                    //else
                                                    //{
                                                    //    isRefunded = gatewayResponse.IsSuccess;
                                                    //}

                                                    if (!isRefunded)
                                                    {
                                                        status["isCancelled"] = OptionCancelStatus.Failed.ToString();
                                                    }
                                                    watchRefund.Stop();
                                                    _log.WriteTimer( "RefundSwith", TimerMethodName, string.Empty, watchRefund.Elapsed.ToString());
                                                    logInfoSave(tokenValue, "CancellationHelper|RefundSwith  End Status" + isRefunded);
                                                    break;

                                                case "Reversal":
                                                case "PreAuthorize":
                                                    var watchCancelAuthorization = Stopwatch.StartNew();
                                                    logInfoSave(tokenValue, "CancellationHelper|PreAuthorizeSwitch  Start");
                                                    gatewayResponse = paymentGateway.CancelAuthorization(booking, cancellation.TokenId);
                                                    isRefunded = gatewayResponse.IsSuccess;
                                                    //if (gatewayResponse.IsSuccess && (PaymentGatewayType)transaction.PaymentGatewayType == PaymentGatewayType.Adyen)
                                                    //{
                                                    //    isRefunded = paymentGateway.WebhookConfirmation(cancellation.BookingRefNo, 3, "CANCELLATION")?.GetAwaiter().GetResult() ?? false;
                                                    //}
                                                    //else
                                                    //{
                                                    //    isRefunded = gatewayResponse.IsSuccess;
                                                    //}
                                                    watchCancelAuthorization.Stop();
                                                    _log.WriteTimer("PreAuthorizeSwitch", TimerMethodName, string.Empty, watchCancelAuthorization.Elapsed.ToString());
                                                    logInfoSave(tokenValue, "CancellationHelper|PreAuthorizeSwitch  End");
                                                    break;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logInfoSave(tokenValue, "CancellationHelper|paymentRefundProcessError", SerializeDeSerializeHelper.Serialize(ex));
                                            status["isRefunded"] = OptionCancelStatus.Failed.ToString();
                                            status["isCancelled"] = OptionCancelStatus.Failed.ToString();
                                            var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
                                            {
                                                ClassName = "CancellationHelper",
                                                MethodName = "CancelAndGetIsangoDbCancellationAndPaymentRefundStatus_refund",
                                                Token = cancellation.TokenId
                                            };
                                            _log.Error(isangoErrorEntity, ex);
                                        }
                                        if (isRefunded)
                                        {
                                            logInfoSave(tokenValue, "CancellationHelper|paymentRefundProcess is Refunded Successfully");
                                            status["isRefunded"] = OptionCancelStatus.Success.ToString();
                                            _cancellationService
                                                       .UpdateCancelBookingIsangoDbAsync(transaction?.TransId ?? 0, gatewayResponse, cancellation.TokenId);
                                            //transactionScope.Complete();
                                           logInfoSave(tokenValue, "CancellationHelper|paymentRefundProcess is Refunded Complete");
                                        }
                                        else
                                        {
                                            logInfoSave(tokenValue, "CancellationHelper|paymentRefundProcess not Refunded");
                                            status["isRefunded"] = OptionCancelStatus.Failed.ToString();
                                            //transactionScope.Dispose();
                                        }
                                    }
                                }
                                if (status["isRefunded"] == OptionCancelStatus.Failed.ToString())
                                {
                                    transactionScope.Dispose();
                                }
                                else
                                {
                                    transactionScope.Complete();
                                }
                                logInfoSave(tokenValue, "CancellationHelper|paymentRefundProcess is End");
                            }
                        }
                        else
                        {
                            logInfoSave(tokenValue, "CancellationHelper|ThisisnotConfirmedProduct");
                            status["isCancelled"] = OptionCancelStatus.Success.ToString();
                            status["isRefunded"] = OptionCancelStatus.NotApplicable.ToString();
                            transactionScope.Complete();
                        }
                    }
                    else
                    {
                        logInfoSave(tokenValue, "CancellationHelper|IsangoDBCancellationAlreadyDone");
                        status["isCancelled"] = cancellationStatus.IsangoCancelStatus == (int)OptionCancelStatus.Success
                            ? OptionCancelStatus.Success.ToString()
                            : OptionCancelStatus.NotApplicable.ToString();
                        status["isRefunded"] = cancellationStatus.PaymentRefundStatus == (int)OptionCancelStatus.Success
                            ? OptionCancelStatus.Success.ToString()
                            : OptionCancelStatus.NotApplicable.ToString();
                        transactionScope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
                    {
                        ClassName = "CancellationHelper",
                        MethodName = "CancelAndGetIsangoDbCancellationAndPaymentRefundStatus",
                        Token = cancellation.TokenId
                    };
                    _log.Error(isangoErrorEntity,ex);
                    logInfoSave(tokenValue, "CancellationHelper|Error");
                    
                    status["isCancelled"] = OptionCancelStatus.Failed.ToString();
                    status["isRefunded"] = OptionCancelStatus.Failed.ToString();
                    transactionScope.Dispose();
                }
            }
            logInfoSave(tokenValue, "CancellationHelper|CancelAndGetIsangoDbCancellationAndPaymentRefundStatus,End Status:"+ SerializeDeSerializeHelper.Serialize(status));
            return status;
        }

        /// <summary>
        /// Create payment object to refund user refund amount
        /// </summary>
        /// <param name="transactionDetail"></param>
        /// <param name="bookingRefNo"></param>
        /// <returns></returns>
        private Payment CreateBookingPaymentObject(TransactionDetail transactionDetail, string bookingRefNo)
        {
            _log.Info($"CancellationHelper|CreateBookingPaymentObjectStart,{ SerializeDeSerializeHelper.Serialize(transactionDetail)}");

            var payment = new Payment
            {
                ChargeAmount = transactionDetail.Amount,
                CurrencyCode = transactionDetail.CurrencyCode,
                Guwid = transactionDetail.Guwid,
                CaptureGuwid= transactionDetail.CaptureGuwid
            };
            _log.Info($"CancellationHelper|CreateBookingPaymentObjectEnd");
            if (transactionDetail.Is3D == 1)
            {
                payment.Is3D = true;
            }

            payment.TransactionId = transactionDetail.TransId.ToString();
            switch (transactionDetail.FlowName)
            {
                case "BookBack":
                case "Payment":
                case "Capture":
                    payment.PaymentStatus = PaymentStatus.Paid;
                    break;

                case "Reversal":
                case "PreAuthorize":
                    payment.PaymentStatus = PaymentStatus.PreAuthorized;
                    break;
            }
            payment.JobId = Constant.UserId + "_" + bookingRefNo;
            return payment;
        }
        private void logInfoSave(string token, string dataPass, string logData = "")
        {
            var logInfo = dataPass;
            var isangoLogEntity = new Isango.Entities.IsangoErrorEntity
            {
                ClassName = "CancellationHelper",
                MethodName = "CancelAndGetIsangoDbCancellation",
                Token = token,
                Params = !String.IsNullOrEmpty(logData) ? (logInfo + "," + logData) : logInfo
            };
            _log.Info(isangoLogEntity);
        }
    }
}