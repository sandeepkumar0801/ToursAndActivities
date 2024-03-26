using Isango.Entities;
using Isango.Entities.AdyenPayment;
using Isango.Entities.ApexxPayment;
using Isango.Entities.Booking;
using Isango.Entities.Payment;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Apexx;
using System;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace Isango.Service.PaymentServices
{
    public class ApexxPaymentService : IPaymentService
    {
        private readonly IApexxAdapter _apexxAdapter;
        private readonly ILogger _log;

        public ApexxPaymentService(IApexxAdapter apexxAdapter, ILogger log)
        {
            _apexxAdapter = apexxAdapter;
            _log = log;
        }

        public EnrollmentCheckResponse EnrollmentCheck(Booking booking, string token)
        {
            var enrollmentCheckResponse = new EnrollmentCheckResponse();

            try
            {
                var enrollmentData = _apexxAdapter.EnrollmentCheck(booking, token);

                enrollmentCheckResponse.BookingReferenceId = enrollmentData.TransactionID;

                //2d
                if (enrollmentData.Is2D)
                {
                    booking.Guwid = enrollmentData.TransactionID;
                    enrollmentCheckResponse.Is2DBooking = true;
                } //3D
                else if (!string.IsNullOrEmpty(enrollmentData.AcsRequest))
                {
                    enrollmentCheckResponse.EnrollmentErrorOrHTML = enrollmentData.AcsRequest;
                } // Error
                else if (!string.IsNullOrEmpty(enrollmentData.Code))
                {
                    enrollmentCheckResponse.EnrollmentErrorCode = enrollmentData.Code;
                    enrollmentCheckResponse.EnrollmentErrorOrHTML = enrollmentData.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ApexxPaymentService",
                    MethodName = "EnrollmentCheck",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return enrollmentCheckResponse;
        }

        public AuthorizationResponse Authorization(Booking booking, bool isEnrollmentCheck, string token)
        {
            var authorizationResponse = new AuthorizationResponse();
            var response = default(ApexxPaymentResponse);
            try
            {
                if (isEnrollmentCheck)
                {
                    response = _apexxAdapter.ThreeDSVerify(booking.Guwid, booking.PaRes, token);
                    authorizationResponse.IsSuccess = IsAuthorizationSuccess(response);
                    if (authorizationResponse.IsSuccess)
                    {
                        authorizationResponse.IsangoReference = response?.MerchantReference;
                        authorizationResponse.Token = response?.Token;
                    }
                    else
                    {
                        authorizationResponse.ErrorMessage = response.ReasonMessage;
                        authorizationResponse.ErrorCode = response.ReasonCode;

                        booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                    , System.Net.HttpStatusCode.BadRequest
                                    , response.ReasonMessage);
                    }
                }
                else
                {
                    authorizationResponse.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ApexxPaymentService",
                    MethodName = "Authorization",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };

                var msg = $"Exception {ex.Message}, Response {SerializeDeSerializeHelper.Serialize(response)}";
                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                   , System.Net.HttpStatusCode.BadRequest
                                   , msg);

                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return authorizationResponse;
        }

        private bool IsAuthorizationSuccess(ApexxPaymentResponse response)
        {
            return response != null && response?.Status == "AUTHORISED";
        }

        public TransactionResponse Transaction(Booking booking, bool is3D, string token)
        {
            var transactionResponse = new TransactionResponse();
            var apexxPaymentResponse = default(ApexxPaymentResponse);
            try
            {
                var purchasePayment = booking.Payment.FirstOrDefault(x => x.PaymentStatus.Equals(PaymentStatus.Paid));

                apexxPaymentResponse = _apexxAdapter.CaptureCardTransaction(purchasePayment.ChargeAmount,
                    purchasePayment.TransactionId, booking.Guwid, token);

                if (apexxPaymentResponse?.Status == "CAPTURED")
                {
                    transactionResponse.IsSuccess = true;
                    purchasePayment.PaymentGatewayReferenceId = apexxPaymentResponse?.TransactionID;
                    purchasePayment.AuthorizationCode = apexxPaymentResponse?.AuthorizationCode;
                }
                else
                {
                    transactionResponse.IsSuccess = false;
                    transactionResponse.ErrorCode = apexxPaymentResponse.Code;
                    transactionResponse.ErrorMessage = apexxPaymentResponse.ErrorMessage;

                    booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                   , System.Net.HttpStatusCode.BadRequest
                                   , apexxPaymentResponse.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ApexxPaymentService",
                    MethodName = "Transaction",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{is3D},{token}"
                };

                var msg = $"Exception {ex.Message}, Response {SerializeDeSerializeHelper.Serialize(apexxPaymentResponse)}";
                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                   , System.Net.HttpStatusCode.BadRequest
                                   , msg);

                _log.Error(isangoErrorEntity, ex);

                transactionResponse.IsSuccess = false;
            }
            return transactionResponse;
        }

        public PaymentGatewayResponse CancelAuthorization(Booking booking, string token)
        {
            PaymentGatewayResponse paymentGatewayResponse = new PaymentGatewayResponse();
            var cancelResponse = default(ApexxPaymentResponse);
            try
            {
                cancelResponse = _apexxAdapter.CancelCardTransaction(booking.Guwid, token);
                if (cancelResponse?.Status == "CANCELLED")
                {
                    paymentGatewayResponse.IsSuccess = true;
                    paymentGatewayResponse.AuthorizationCode = cancelResponse.AuthorizationCode;
                    paymentGatewayResponse.Guwid = cancelResponse.TransactionID;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ApexxPaymentService",
                    MethodName = "CancelAuthorization",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };

                var msg = $"Exception {ex.Message}, Response {SerializeDeSerializeHelper.Serialize(cancelResponse)}";
                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                   , System.Net.HttpStatusCode.BadRequest
                                   , msg);

                _log.Error(isangoErrorEntity, ex);
            }

            return paymentGatewayResponse;
        }

        public PaymentGatewayResponse RefundCapture(Booking booking, string reason, string token)
        {
            PaymentGatewayResponse paymentGatewayResponse = new PaymentGatewayResponse();
            var refundResponse = default(ApexxPaymentResponse);
            try
            {
                var purchasePayment = booking.Payment.FirstOrDefault(x => x.PaymentStatus.Equals(PaymentStatus.Paid));
                if (purchasePayment != null)
                {
                    refundResponse =
                       _apexxAdapter.RefundCardTransaction(purchasePayment.ChargeAmount, reason, purchasePayment.Guwid, token, purchasePayment.CaptureGuwid);

                    if (refundResponse?.Status == "REFUNDED")
                    {
                        paymentGatewayResponse.IsSuccess = true;
                        paymentGatewayResponse.AuthorizationCode = refundResponse.AuthorizationCode;
                        paymentGatewayResponse.Guwid = refundResponse.TransactionID;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ApexxPaymentService",
                    MethodName = "RefundCapture",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };

                var msg = $"Exception {ex.Message}, Response {SerializeDeSerializeHelper.Serialize(refundResponse)}";
                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                   , System.Net.HttpStatusCode.BadRequest
                                   , msg);

                _log.Error(isangoErrorEntity, ex);
            }

            return paymentGatewayResponse;
        }

        /// <summary>
        /// Refund amount of single product after cancellation
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="reason"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public PaymentGatewayResponse Refund(Booking booking, string reason, string token)
        {
            return RefundCapture(booking, reason, token);
        }

        public PaymentGatewayResponse CancelCapture(Booking booking, string captureId, string captureReference, string token)
        {
            PaymentGatewayResponse paymentGatewayResponse = new PaymentGatewayResponse();
            var cancelResponse = default(ApexxPaymentResponse);
            try
            {
                cancelResponse = _apexxAdapter.CancelCaptureCardTransaction(captureId, captureReference, token);
                if (cancelResponse?.Status == "CANCELLED")
                {
                    paymentGatewayResponse.IsSuccess = true;
                    paymentGatewayResponse.AuthorizationCode = cancelResponse.AuthorizationCode;
                    paymentGatewayResponse.Guwid = cancelResponse.TransactionID;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ApexxPaymentService",
                    MethodName = "CancelCapture",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };

                var msg = $"Exception {ex.Message}, Response {SerializeDeSerializeHelper.Serialize(cancelResponse)}";
                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                   , System.Net.HttpStatusCode.BadRequest
                                   , msg);

                _log.Error(isangoErrorEntity, ex);
            }

            return paymentGatewayResponse;
        }

        public PaymentGatewayResponse CreateNewTransaction(Booking booking, string token)
        {
            var paymentGatewayResponse = new PaymentGatewayResponse();
            var paymentGatewayData = default(ApexxPaymentResponse);
            try
            {
                paymentGatewayData = _apexxAdapter.CreateCardTransaction(booking, token);
                //2d
                if (paymentGatewayData.Is2D)
                {
                    booking.Guwid = paymentGatewayData.TransactionID;
                    paymentGatewayResponse.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ApexxPaymentService",
                    MethodName = "CreateNewTransaction",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };

                var msg = $"Exception {ex.Message}, Response {SerializeDeSerializeHelper.Serialize(paymentGatewayData)}";
                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                   , System.Net.HttpStatusCode.BadRequest
                                   , msg);

                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return paymentGatewayResponse;
        }

        public Task<bool> WebhookConfirmation(string bookingRefNo, int transFlowID, string returnStatus)
        {
            return Task.FromResult(true);
        }

        EnrollmentCheckResponse IPaymentService.EnrollmentCheck(Booking booking, string token)
        {
            throw new NotImplementedException();
        }

        AuthorizationResponse IPaymentService.Authorization(Booking booking, bool isEnrollmentCheck, string token)
        {
            throw new NotImplementedException();
        }

        TransactionResponse IPaymentService.Transaction(Booking booking, bool is3D, string token)
        {
            throw new NotImplementedException();
        }

        PaymentGatewayResponse IPaymentService.CancelAuthorization(Booking booking, string token)
        {
            throw new NotImplementedException();
        }

        PaymentGatewayResponse IPaymentService.RefundCapture(Booking booking, string reason, string token)
        {
            throw new NotImplementedException();
        }

        PaymentGatewayResponse IPaymentService.Refund(Booking booking, string reason, string token)
        {
            throw new NotImplementedException();
        }

        PaymentGatewayResponse IPaymentService.CancelCapture(Booking booking, string captureId, string captureReference, string token)
        {
            throw new NotImplementedException();
        }

        PaymentGatewayResponse IPaymentService.CreateNewTransaction(Booking booking, string token)
        {
            throw new NotImplementedException();
        }

        Task<Tuple<bool,bool>> IPaymentService.WebhookConfirmation(string bookingRefNo, int transFlowID, string returnStatus)
        {
            throw new NotImplementedException();
        }

        Task<GeneratePaymentIsangoResponse> IPaymentService.GetAdyenGeneratePaymentLinksAsync(string countryCode, string shopperLocale, string amount, string currency)
        {
            throw new NotImplementedException();
        }
    }
}