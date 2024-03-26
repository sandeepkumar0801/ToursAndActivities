using Isango.Entities;
using Isango.Entities.AdyenPayment;
using Isango.Entities.Booking;
using Isango.Entities.Payment;
using Isango.Entities.WirecardPayment;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.WirecardPayment;
using System;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace Isango.Service.PaymentServices
{
    public class WirecardPaymentService : IPaymentService
    {
        private readonly IWirecardPaymentAdapter _wirecardPaymentAdapter;
        private readonly ILogger _log;

        public WirecardPaymentService(IWirecardPaymentAdapter wirecardPaymentAdapter, ILogger log)
        {
            _wirecardPaymentAdapter = wirecardPaymentAdapter;
            _log = log;
        }

        public EnrollmentCheckResponse EnrollmentCheck(Booking booking, string token)
        {
            var enrollmentCheckResponse = new EnrollmentCheckResponse();
            try
            {
                //Fill the payment object of booking object.
                //Enrollment check
                var enrollmentData = GetEnrollmentData(booking, token);
                var enrollmentStatus = enrollmentData.Item1;
                var enrollmentResponse = enrollmentData.Item2;

                //Error
                if (!enrollmentStatus)
                {
                    //In case of enrollment is failed or undefined error numbers.
                    enrollmentCheckResponse.EnrollmentErrorOrHTML = enrollmentResponse.EnrollmentCheckStatus;
                    enrollmentCheckResponse.EnrollmentErrorCode = enrollmentResponse.ErrorNumber.Replace("Error", "");
                }//2d
                else if (!string.IsNullOrEmpty(enrollmentResponse.ErrorNumber))
                {
                    enrollmentCheckResponse.BookingReferenceId = enrollmentResponse.PaymentGatewayReferenceId;
                    booking.Guwid = enrollmentCheckResponse.BookingReferenceId;
                    enrollmentCheckResponse.Is2DBooking = true;
                } //3D
                else
                {
                    //Successful enrollment check redirect to acs request
                    enrollmentCheckResponse.BookingReferenceId = enrollmentResponse.PaymentGatewayReferenceId;
                    enrollmentCheckResponse.EnrollmentErrorOrHTML = enrollmentResponse.AcsRequest;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "WirecardPaymentService",
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
            var preAuthPayment = booking?.Payment?.FirstOrDefault();
            if (preAuthPayment != null && preAuthPayment?.ChargeAmount == 0)
            {
                preAuthPayment.ChargeAmount = booking.Amount;
                preAuthPayment.PaymentStatus = PaymentStatus.PreAuthorized;
            }

            try
            {
                if (preAuthPayment != null)
                {
                    preAuthPayment.JobId = GetJobId(booking);

                    Tuple<WirecardPaymentResponse, string, string> response;
                    if (isEnrollmentCheck)
                    {
                        preAuthPayment.Is3D = true;
                        response = _wirecardPaymentAdapter.PreAuthorize3D(preAuthPayment, booking, token);
                    }
                    else
                    {
                        preAuthPayment.IpAddress = booking.IpAddress;
                        preAuthPayment.CurrencyCode = booking?.Currency?.IsoCode?.ToUpper();
                        response = _wirecardPaymentAdapter.PreAuthorize(preAuthPayment, token);
                    }

                    if (response.Item1?.PaymentStatus == PaymentStatus.UnSuccessful)
                    {
                        authorizationResponse.IsSuccess = false;
                        authorizationResponse.ErrorCode = response.Item1.ErrorNumber;
                        authorizationResponse.ErrorMessage = response.Item1.ErrorMessage;
                    }
                    else
                    {
                        authorizationResponse.IsSuccess = true;
                        preAuthPayment.AuthorizationCode = response?.Item1?.AuthorizationCode;
                        preAuthPayment.TransactionId = response?.Item1?.TransactionId;
                        var guwid = response?.Item1?.PaymentGatewayReferenceId;
                        preAuthPayment.PaymentGatewayReferenceId = guwid;
                        booking.Guwid = guwid;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "WirecardPaymentService",
                    MethodName = "Authorization",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return authorizationResponse;
        }

        public TransactionResponse Transaction(Booking booking, bool is3D, string token)
        {
            var transactionResponse = new TransactionResponse();
            Tuple<WirecardPaymentResponse, string, string> wirecardPaymentResponse;

            var preAuthorizePayment = booking.Payment.Find(FilterPreAuthPayment);
            var purchasePayment = booking.Payment.Find(FilterPurchasePayment);

            try
            {
                purchasePayment.Is3D = is3D;
                purchasePayment.JobId = GetJobId(booking);

                if (is3D)
                {
                    wirecardPaymentResponse =
                        _wirecardPaymentAdapter.CapturePreauthorize3D(preAuthorizePayment, purchasePayment, token);
                }
                else
                {
                    purchasePayment.IpAddress = booking.IpAddress;
                    wirecardPaymentResponse = _wirecardPaymentAdapter.CapturePreauthorize(purchasePayment, token);
                }
                if (wirecardPaymentResponse.Item1?.PaymentStatus == PaymentStatus.UnSuccessful)
                {
                    transactionResponse.IsSuccess = false;
                    transactionResponse.ErrorCode = wirecardPaymentResponse.Item1.ErrorNumber;
                    transactionResponse.ErrorMessage = wirecardPaymentResponse.Item1.ErrorMessage;
                }
                else
                {
                    purchasePayment.PaymentGatewayReferenceId =
                    wirecardPaymentResponse?.Item1?.PaymentGatewayReferenceId;
                    purchasePayment.AuthorizationCode = wirecardPaymentResponse?.Item1?.AuthorizationCode;
                    booking.Guwid = wirecardPaymentResponse?.Item1?.PaymentGatewayReferenceId;
                    transactionResponse.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "WirecardPaymentService",
                    MethodName = "Transaction",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params =
                        $"{SerializeDeSerializeHelper.Serialize(booking)},{SerializeDeSerializeHelper.Serialize(purchasePayment)},{is3D},{token}"
                };
                _log.Error(isangoErrorEntity, ex);
                transactionResponse.IsSuccess = false;
            }

            return transactionResponse;
        }

        public PaymentGatewayResponse CancelAuthorization(Booking booking, string token)
        {
            PaymentGatewayResponse reversalResponse = new PaymentGatewayResponse();
            try
            {
                var preAuthPayment = booking.Payment.Find(FilterPreAuthPayment);
                if (preAuthPayment != null)
                {
                    var refundResponse = _wirecardPaymentAdapter.Rollback(preAuthPayment, token);
                    var WirecardReversalResponse = refundResponse.Item1;
                    if (WirecardReversalResponse.AuthorizationCode != null)
                    {
                        reversalResponse.IsSuccess = true;
                        reversalResponse.AuthorizationCode = WirecardReversalResponse.AuthorizationCode;
                        reversalResponse.Guwid = WirecardReversalResponse.PaymentGatewayReferenceId;
                        reversalResponse.TransactionId = WirecardReversalResponse.TransactionId;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "WirecardPaymentService",
                    MethodName = "CancelAuthorization",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };
                _log.Error(isangoErrorEntity, ex);
            }

            return reversalResponse;
        }

        public PaymentGatewayResponse RefundCapture(Booking booking, string reason, string token)
        {
            PaymentGatewayResponse paymentRefundResponse = new PaymentGatewayResponse();
            try
            {
                var purchase = booking.Payment.Find(FilterPurchasePayment);
                if (purchase != null)
                {
                    var refundResponse = _wirecardPaymentAdapter.Rollback(purchase, token);
                    var wirecardRefundResponse = refundResponse?.Item1;
                    if (wirecardRefundResponse?.AuthorizationCode != null)
                    {
                        paymentRefundResponse.TransactionId = wirecardRefundResponse.TransactionId;
                        paymentRefundResponse.IsSuccess = true;
                        paymentRefundResponse.AuthorizationCode = wirecardRefundResponse.AuthorizationCode;
                        paymentRefundResponse.Guwid = wirecardRefundResponse.PaymentGatewayReferenceId;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "WirecardPaymentService",
                    MethodName = "RefundCapture",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };
                _log.Error(isangoErrorEntity, ex);
            }

            return paymentRefundResponse;
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
            PaymentGatewayResponse refundResponse = new PaymentGatewayResponse();
            try
            {
                var purchasePayment = booking.Payment?.Find(FilterPurchasePayment);
                var bookBackResponse = _wirecardPaymentAdapter.BookBack(purchasePayment, token);
                var wirecardRefundResponse = bookBackResponse?.Item1;
                if (wirecardRefundResponse?.Status == "Success")
                {
                    refundResponse.IsSuccess = true;
                    refundResponse.Guwid = wirecardRefundResponse.PaymentGatewayReferenceId;
                    refundResponse.TransactionId = wirecardRefundResponse.TransactionId;
                    refundResponse.AuthorizationCode = wirecardRefundResponse.AuthorizationCode;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "WirecardPaymentService",
                    MethodName = "Refund",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };
                _log.Error(isangoErrorEntity, ex);
            }

            return refundResponse;
        }

        #region Private Methods

        private Tuple<bool, WirecardPaymentResponse> GetEnrollmentData(Booking booking, string token)
        {
            var payment = booking.Payment.FirstOrDefault();
            var enrollmentStatus = false;

            var creditCard = (CreditCard)payment?.PaymentType;
            var wirecardPaymentResponse = _wirecardPaymentAdapter.EnrollmentCheck(booking, token);
            var errorNumber = wirecardPaymentResponse.Item1.ErrorNumber;
            var countriesWith3Dand2D = ConfigurationManagerHelper.GetValuefromAppSettings("3DAnd2DEnabledCountries") ??
                                       string.Empty;
            var errorCodeList = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.PreDefinedErrorCodes)
                .Split(',');
            var arrCountries = string.IsNullOrWhiteSpace(countriesWith3Dand2D)
                ? new string[] { }
                : countriesWith3Dand2D.Split(',').Select(p => p.Trim()).ToArray();

            if (!string.IsNullOrEmpty(errorNumber))
            {
                if (errorCodeList.Contains(errorNumber) &&
                    (!arrCountries.Any() || arrCountries.Contains(creditCard?.CardHoldersCountryName,
                         StringComparer.OrdinalIgnoreCase)))
                {
                    if (creditCard != null && creditCard.CardType.Equals(Constant.AmericanExpress))
                    {
                        wirecardPaymentResponse.Item1.EnrollmentCheckStatus = Constant.EnrollStatusFailureAmex;
                    }
                    else
                        enrollmentStatus = true;
                }
            }
            else
                enrollmentStatus = true;

            return new Tuple<bool, WirecardPaymentResponse>(enrollmentStatus, wirecardPaymentResponse.Item1);
        }

        /// <summary>
        /// Prepare the BookingXmlData model
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        private string GetJobId(Booking booking)
        {
            return $"{booking.User.UserId}_{booking.ReferenceNumber}";
        }

        /// <summary>
        /// Predicate for PreAuthorized payment object.
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        private bool FilterPreAuthPayment(Payment payment)
        {
            return payment.PaymentStatus.Equals(PaymentStatus.PreAuthorized);
        }

        private bool FilterPurchasePayment(Payment payment)
        {
            return payment.PaymentStatus.Equals(PaymentStatus.Paid);
        }

        public PaymentGatewayResponse CancelCapture(Booking booking ,string captureId,string captureReference, string token)
        {
            throw new NotImplementedException();
        }

        public PaymentGatewayResponse CreateNewTransaction(Booking booking, string token)
        {
            throw new NotImplementedException();
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

        #endregion Private Methods
    }
}