using Isango.Entities;
using Isango.Entities.AdyenPayment;
using Isango.Entities.Booking;
using Isango.Entities.Payment;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Adyen;
using ServiceAdapters.Adyen.Constants;
using System;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace Isango.Service.PaymentServices
{
    public class AdyenPaymentService : IPaymentService
    {
        private readonly IAdyenAdapter _adyenAdapter;
        private readonly ILogger _log;
        private readonly IAdyenPersistence _adyenPersistence;

        public AdyenPaymentService(IAdyenAdapter adyenAdapter, ILogger log, IAdyenPersistence adyenPersistence)
        {
            _adyenAdapter = adyenAdapter;
            _log = log;
            _adyenPersistence = adyenPersistence;
        }

        public EnrollmentCheckResponse EnrollmentCheck(Booking booking, string token)
        {
            var enrollmentCheckResponse = new EnrollmentCheckResponse();

            try
            {
                int adyenMerchantType = GetMerchantType(booking);
                var enrollmentData = _adyenAdapter.EnrollmentCheck(booking, token, adyenMerchantType);

                enrollmentCheckResponse.BookingReferenceId = enrollmentData.TransactionID;

                //2d
                if (enrollmentData.Is2D)
                {
                    booking.Guwid = enrollmentData.TransactionID;
                    enrollmentCheckResponse.Is2DBooking = true;
                } //3D
                else if (!string.IsNullOrEmpty(enrollmentData.AcsRequest))
                {
                    var key = $"EnrollmentData_{booking.ReferenceNumber}";
                    CacheHelper.Set<AdyenPaymentResponse>(key, enrollmentData);
                    enrollmentCheckResponse.EnrollmentErrorOrHTML = enrollmentData.AcsRequest;
                } // Error
                else if (!string.IsNullOrEmpty(enrollmentData.Code) || !string.IsNullOrEmpty(enrollmentData.RefusalReason))
                {
                    enrollmentCheckResponse.EnrollmentErrorCode = enrollmentData?.Code;
                    enrollmentCheckResponse.EnrollmentErrorOrHTML = enrollmentData?.ErrorMessage;

                    var response = enrollmentData;
                    if (!string.IsNullOrWhiteSpace(response?.RefusalReason))
                    {
                        enrollmentCheckResponse.EnrollmentErrorOrHTML = response?.RefusalReason;
                    }
                    if (!string.IsNullOrWhiteSpace(response?.ErrorMessage) || !string.IsNullOrWhiteSpace(response?.RefusalReason))
                    {
                        booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                    , System.Net.HttpStatusCode.BadRequest
                                    , $"ErrorCode {response.Code}, Error Message {response?.ErrorMessage}, ReasonCode {response?.RefusalReasonCode} , ReasonMessage {response.RefusalReason}"
                                    );
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AdyenPaymentService",
                    MethodName = "EnrollmentCheck",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };

                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                       , System.Net.HttpStatusCode.InternalServerError
                                       , $"EnrollmentCheck runtime exception {ex.Message}"
                                       );

                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return enrollmentCheckResponse;
        }

        public AuthorizationResponse Authorization(Booking booking, bool isEnrollmentCheck, string token)
        {
            var authorizationResponse = new AuthorizationResponse();

            try
            {
                var AdyenPaymentData = string.Empty;
                var key = $"EnrollmentData_{booking.ReferenceNumber}";
                if (isEnrollmentCheck)
                {
                    if (!CacheHelper.Exists(key) || !CacheHelper.Get<AdyenPaymentResponse>(key, out var adyenPaymentResponse))
                    {
                        AdyenPaymentData = booking.AdyenPaymentData;
                    }
                    else
                    {
                        int Pos1 = adyenPaymentResponse.AcsRequest.IndexOf("paymentData\":\"") + "paymentData\":\"".Length;
                        int Pos2 = adyenPaymentResponse.AcsRequest.IndexOf("\",\"paymentMethodType");
                        AdyenPaymentData = adyenPaymentResponse.AcsRequest.Substring(Pos1, Pos2 - Pos1);
                    }
                    int adyenMerchantType = GetMerchantType(booking);
                    var response = _adyenAdapter.ThreeDSVerify(booking.Guwid, booking.PaRes, AdyenPaymentData, token, booking.FallbackFingerPrint, adyenMerchantType);
                    authorizationResponse.IsSuccess = IsAuthorizationSuccess(response);
                    authorizationResponse.IsWebhookReceived = true;
                    if (authorizationResponse.IsSuccess)
                    {
                        authorizationResponse.IsangoReference = response?.MerchantReference;
                        //authorizationResponse.AdyenPsPReference = response?.PspReference;
                        booking.Guwid = response?.PspReference;
                    }
                    else if (IsAuthorizationReceived(response))
                    {
                        var WebhookResult = WebhookConfirmation(booking.ReferenceNumber, 5, "authorisation")?.GetAwaiter().GetResult();
                        authorizationResponse.IsSuccess = WebhookResult.Item1;
                        authorizationResponse.IsWebhookReceived = WebhookResult.Item2;
                        booking.Guwid = response?.PspReference;
                        authorizationResponse.IsangoReference = response?.MerchantReference;
                    }
                    else if (!string.IsNullOrEmpty(response.AcsRequest))
                    {
                        int Pos1 = response.AcsRequest.IndexOf("paymentData\":\"") + "paymentData\":\"".Length;
                        int Pos2 = response.AcsRequest.IndexOf("\",\"paymentMethodType");
                        AdyenPaymentData = response.AcsRequest.Substring(Pos1, Pos2 - Pos1);
                        CacheHelper.Set<AdyenPaymentResponse>(key, response);
                        authorizationResponse.TransactionID = response.TransactionID;
                        authorizationResponse.AcsRequest = response.AcsRequest;
                        authorizationResponse.FallbackFingerPrint = string.IsNullOrEmpty(booking.FallbackFingerPrint) ? booking.PaRes.Replace("{\"threeds2.fingerprint\":\"", "").Replace("\"}", "") : booking.FallbackFingerPrint;
                    }
                    else
                    {
                        authorizationResponse.ErrorMessage = response.ErrorMessage;
                        authorizationResponse.ErrorCode = response.Code;
                        if (!string.IsNullOrWhiteSpace(response?.RefusalReason))
                        {
                            authorizationResponse.ErrorMessage = response.RefusalReason;
                        }
                        if (!string.IsNullOrWhiteSpace(response?.ErrorMessage) || !string.IsNullOrWhiteSpace(response?.RefusalReason))
                        {
                            booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                        , System.Net.HttpStatusCode.BadRequest
                                        , $"ErrorCode {response.Code}, Error Message {response.ErrorMessage}, ReasonCode {response.RefusalReasonCode} , ReasonMessage {response.RefusalReason}"
                                        );
                        }
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
                    ClassName = "AdyenPaymentService",
                    MethodName = "Authorization",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };

                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                        , System.Net.HttpStatusCode.InternalServerError
                                        , ex.Message
                                        );

                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return authorizationResponse;
        }

        public PaymentGatewayResponse CancelAuthorization(Booking booking, string token)
        {
            PaymentGatewayResponse paymentGatewayResponse = new PaymentGatewayResponse();
            try
            {
                int adyenMerchantType = GetMerchantType(booking);
                var cancelResponse = _adyenAdapter.CancelCardTransaction(booking.Guwid, booking.ReferenceNumber, token, adyenMerchantType);
                if (cancelResponse?.Status.ToLower().Contains("cancel-received") ?? false)
                {
                    paymentGatewayResponse.IsSuccess = true;
                    paymentGatewayResponse.Guwid = cancelResponse.PspReference;
                }
                else
                {
                    paymentGatewayResponse.IsSuccess = false;

                    var response = cancelResponse;
                    if (!string.IsNullOrWhiteSpace(response?.ErrorMessage) || !string.IsNullOrWhiteSpace(response?.RefusalReason))
                    {
                        booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                    , System.Net.HttpStatusCode.BadRequest
                                    , $"ErrorCode {response.Code}, Error Message {response.ErrorMessage}, ReasonCode {response.RefusalReasonCode} , ReasonMessage {response.RefusalReason}"
                                    );
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AdyenPaymentService",
                    MethodName = "CancelAuthorization",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };

                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                       , System.Net.HttpStatusCode.InternalServerError
                                       , ex.Message
                                       );
                _log.Error(isangoErrorEntity, ex);
            }

            return paymentGatewayResponse;
        }

        public PaymentGatewayResponse CancelCapture(Booking booking, string captureId, string captureReference, string token)
        {
            PaymentGatewayResponse paymentGatewayResponse = new PaymentGatewayResponse();
            try
            {
                int adyenMerchantType = GetMerchantType(booking);
                var cancelResponse = _adyenAdapter.CancelCardTransaction(booking.Guwid, booking.ReferenceNumber, token, adyenMerchantType);
                if (cancelResponse?.Status.ToLower().Contains("cancel-received") ?? false)
                {
                    paymentGatewayResponse.IsSuccess = true;
                    paymentGatewayResponse.Guwid = cancelResponse.PspReference;
                }
                else
                {
                    paymentGatewayResponse.IsSuccess = false;

                    var response = cancelResponse;
                    if (!string.IsNullOrWhiteSpace(response?.ErrorMessage) || !string.IsNullOrWhiteSpace(response?.RefusalReason))
                    {
                        booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                    , System.Net.HttpStatusCode.BadRequest
                                    , $"ErrorCode {response.Code}, Error Message {response.ErrorMessage}, ReasonCode {response.RefusalReasonCode} , ReasonMessage {response.RefusalReason}"
                                    );
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AdyenPaymentService",
                    MethodName = "CancelAuthorization",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };

                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                       , System.Net.HttpStatusCode.InternalServerError
                                       , ex.Message
                                       );

                _log.Error(isangoErrorEntity, ex);
            }

            return paymentGatewayResponse;
        }

        private bool IsAuthorizationSuccess(AdyenPaymentResponse response)
        {
            return response != null && response?.Status == "Authorised";
        }

        private bool IsAuthorizationReceived(AdyenPaymentResponse response)
        {
            return response != null && ((response?.Status?.ToLower()?.Contains("received") ?? false) || (response?.Status?.ToLower()?.Contains("pending") ?? false));
        }

        public PaymentGatewayResponse Refund(Booking booking, string reason, string token)
        {
            return RefundCapture(booking, reason, token);
        }

        public PaymentGatewayResponse RefundCapture(Booking booking, string reason, string token)
        {
            PaymentGatewayResponse paymentGatewayResponse = new PaymentGatewayResponse();
            try
            {
                var purchasePayment = booking.Payment.FirstOrDefault(x => x.PaymentStatus.Equals(PaymentStatus.Paid));
                if (purchasePayment != null)
                {

                    int adyenMerchantType = GetMerchantType(booking);

                    var refundResponse =
                        _adyenAdapter.RefundCardTransaction(purchasePayment.ChargeAmount, purchasePayment.CurrencyCode, reason, purchasePayment.Guwid, purchasePayment.JobId, token, adyenMerchantType);

                    if (refundResponse?.Status.ToLower().Contains("refund-received") ?? false)
                    {
                        paymentGatewayResponse.IsSuccess = true;
                        paymentGatewayResponse.Guwid = refundResponse.PspReference;
                    }
                    else
                    {
                        paymentGatewayResponse.IsSuccess = false;

                        var response = refundResponse;
                        if (!string.IsNullOrWhiteSpace(response?.ErrorMessage) || !string.IsNullOrWhiteSpace(response?.RefusalReason))
                        {
                            booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                        , System.Net.HttpStatusCode.BadRequest
                                        , $"ErrorCode {response.Code}, Error Message {response.ErrorMessage}, ReasonCode {response.RefusalReasonCode} , ReasonMessage {response.RefusalReason}"
                                        );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AdyenPaymentService",
                    MethodName = "RefundCapture",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{token}"
                };

                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                       , System.Net.HttpStatusCode.InternalServerError
                                       , ex.Message
                                       );

                _log.Error(isangoErrorEntity, ex);
            }

            return paymentGatewayResponse;
        }

        public TransactionResponse Transaction(Booking booking, bool is3D, string token)
        {
            var transactionResponse = new TransactionResponse();
            transactionResponse.IsAdyen = true;
            try
            {
                var purchasePayment = booking.Payment.FirstOrDefault(x => x.PaymentStatus.Equals(PaymentStatus.Paid));

                int adyenMerchantType = GetMerchantType(booking);

                var adyenPaymentResponse = new AdyenPaymentResponse();
                //in case of sofort .. skip capture and return
                if (!string.IsNullOrEmpty(booking?.FallbackFingerPrint)
                    &&
                    (booking?.FallbackFingerPrint?.ToLower() == "sofort")
                    ||
                    booking?.FallbackFingerPrint?.ToLower() == "ideal")
                {
                    transactionResponse.IsWebHookRecieved = true;
                    transactionResponse.IsSuccess = true;
                    transactionResponse.IsAdyen = true;
                    return transactionResponse;
                }
                else
                {
                    adyenPaymentResponse = _adyenAdapter.CaptureCardTransaction(purchasePayment.ChargeAmount,
                    purchasePayment.CurrencyCode, purchasePayment.Guwid, booking.ReferenceNumber, token, adyenMerchantType);
                }

                if (adyenPaymentResponse?.Status?.Contains("capture-received") ?? false)
                {
                    transactionResponse.IsWebHookRecieved = true;
                    transactionResponse.IsSuccess = true;
                    purchasePayment.PaymentGatewayReferenceId = adyenPaymentResponse?.PspReference;
                    //purchasePayment.AuthorizationCode = adyenPaymentResponse?.PspReference;
                }
                else
                {
                    transactionResponse.IsSuccess = false;
                    transactionResponse.ErrorCode = adyenPaymentResponse.Code;
                    transactionResponse.ErrorMessage = adyenPaymentResponse.ErrorMessage;

                    var response = adyenPaymentResponse;
                    if (!string.IsNullOrWhiteSpace(response?.ErrorMessage) || !string.IsNullOrWhiteSpace(response?.RefusalReason))
                    {
                        booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                    , System.Net.HttpStatusCode.BadRequest
                                    , $"ErrorCode {response.Code}, Error Message {response.ErrorMessage}, ReasonCode {response.RefusalReasonCode} , ReasonMessage {response.RefusalReason}"
                                    );
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AdyenPaymentService",
                    MethodName = "Transaction",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{is3D},{token}"
                };

                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                       , System.Net.HttpStatusCode.InternalServerError
                                       , ex.Message
                                       );

                _log.Error(isangoErrorEntity, ex);
                transactionResponse.IsSuccess = false;
            }
            return transactionResponse;
        }

        public async Task<Tuple<bool, bool>> WebhookConfirmation(string bookingRefNo, int transFlowID, string returnStatus)
        {
            try
            {
                int i = 0;
                while (i < 60)
                {
                    Task.Delay(3000).Wait();
                    //Fetch Data from DB- can implement a service and persistance
                    var result = _adyenPersistence.GetAdyenWebhookRepsonse(bookingRefNo, transFlowID);
                    if (result?.Item1?.ToLower() == returnStatus?.ToLower() && (result?.Item2 ?? false))
                    {
                        return Tuple.Create(true, true);
                    }
                    else if ((!result?.Item2 ?? false) && (result?.Item1?.ToLower()?.Contains(returnStatus?.ToLower()) ?? false))
                    {
                        return Tuple.Create(false, true);
                    }
                    i = i + 1;
                }
                return Tuple.Create(false, false);
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, false);
            }
        }

        public PaymentGatewayResponse CreateNewTransaction(Booking booking, string token)
        {
            throw new NotImplementedException();
        }

        //private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        //{
        //    Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
        //                      e.SignalTime);
        //}

        /// <summary>
        /// This method returns list of the Currency Data.
        /// </summary>
        /// <returns></returns>
        public async Task<GeneratePaymentIsangoResponse> GetAdyenGeneratePaymentLinksAsync(string countryCode
            , string shopperLocale, string amount, string currency)
        {
            try
            {
                var generatePaymentLinks = await Task.FromResult(_adyenAdapter.
                    GeneratePaymentLinks(countryCode
              , shopperLocale, amount, currency, "adyenPaymentGenerateLinks"));
                return generatePaymentLinks;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AdyenPaymentService",
                    MethodName = "GetAdyenGeneratePaymentLinksAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Merchant Type
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        private int GetMerchantType(Booking booking)
        {
            var adyenReleaseDate = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenReleaseDate");
            int adyenMerchantType = (int)AdyenMerchantType.AdyenNewAccount;
            try
            {
                if (string.IsNullOrEmpty(booking.AdyenMerchantAccountCancelRefund))
                {
                    var isOldAdyen = booking.Date < Convert.ToDateTime(adyenReleaseDate);
                    //check product level or affiliate level(adyen stringent account):
                    //case1: if AdyenStringentAccount=true and AdyenStringentAccountRestrictProducts
                    //case2: if AffiliateLevelStringent=true and AdyenStringentAccount=true
                    var affiliatLevelStringent = booking?.Affiliate;

                    var checkTwoStep = false;
                    if (booking?.SelectedProducts != null)
                    {
                        var productoptions = booking?.SelectedProducts.All(x=>x.IsReceipt==true);
                        if (productoptions == true)
                        {
                            checkTwoStep = true;
                        }
                    }
                    if (checkTwoStep)
                    {
                        adyenMerchantType = isOldAdyen == true ? (int)AdyenMerchantType.AdyenOldAccount : (int)AdyenMerchantType.AdyenNewAccount;
                    }

                    else if (
                        (affiliatLevelStringent?.AdyenStringentAccount == true
                        && affiliatLevelStringent?.AdyenStringentAccountRestrictProducts == false)
                       )
                    {
                        adyenMerchantType = (int)AdyenMerchantType.AdyenStringentAccount;
                    }
                    else if ((affiliatLevelStringent?.AdyenStringentAccount == true
                        && booking.SelectedProducts.Any(x => x.AdyenStringentAccount == true)))
                    {
                        adyenMerchantType = (int)AdyenMerchantType.AdyenStringentAccount;
                    }
                    else
                    {
                        adyenMerchantType = isOldAdyen == true ? (int)AdyenMerchantType.AdyenOldAccount : (int)AdyenMerchantType.AdyenNewAccount;
                    }
                }
                else
                {
                    var adyenMerchantccountOld = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenMerchantAccount");
                    var adyenMerchantAccountNew = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenMerchantAccountNew");
                    var adyenMerchantAccountStringent = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenMerchantAccountStringent");
                    if (booking.AdyenMerchantAccountCancelRefund?.ToUpper() == adyenMerchantccountOld?.ToUpper())
                    {
                        return (int)AdyenMerchantType.AdyenOldAccount;
                    }
                    else if (booking.AdyenMerchantAccountCancelRefund?.ToUpper() == adyenMerchantAccountNew?.ToUpper())
                    {
                        return (int)AdyenMerchantType.AdyenNewAccount;
                    }
                    else if (booking.AdyenMerchantAccountCancelRefund?.ToUpper() == adyenMerchantAccountStringent?.ToUpper())
                    {
                        return (int)AdyenMerchantType.AdyenStringentAccount;
                    }
                    else
                    {
                        return (int)AdyenMerchantType.AdyenNewAccount;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AdyenPaymentService",
                    MethodName = "GetMerchantType"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return adyenMerchantType;
        }
    }
}