using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.AdyenPayment;
using Isango.Entities.AlternativePayment;
using Isango.Entities.Booking;
using Isango.Entities.Canocalization;
using Isango.Entities.Enums;
using Isango.Entities.FareHarbor;
using Isango.Entities.GoCity;
using Isango.Entities.HotelBeds;
using Isango.Entities.Payment;
using Isango.Entities.Prio;
using Isango.Entities.PrioHub;
using Isango.Entities.Rayna;
using Isango.Entities.Rezdy;
using Isango.Entities.RiskifiedPayment;
using Isango.Entities.Tiqets;
using Isango.Entities.Ventrata;
using Isango.Persistence.Contract;
using Isango.Service.Canocalization;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Isango.Service.Factory;
using Logger.Contract;
using Newtonsoft.Json;
using ServiceAdapters.AlternativePayment;
using ServiceAdapters.WirecardPayment;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Transactions;
using Util;
using BookingDataOthers = Isango.Entities.Mailer.Voucher.BookingDataOthers;
using Transaction = Isango.Entities.AlternativePayment.Transaction;
using WirecardPaymentResponse = Isango.Entities.WirecardPayment.WirecardPaymentResponse;

namespace Isango.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BookingService : IBookingService
    {
        private readonly IBookingPersistence _bookingPersistence;
        private readonly ISupplierBookingService _supplierBookingService;
        private readonly IWirecardPaymentAdapter _wirecardPaymentAdapter;
        private readonly IMasterService _masterService;
        private readonly ILogger _log;
        private readonly IAlternativePaymentService _alternativePaymentService;
        private readonly IAlternativePaymentAdapter _alternativePaymentAdapter;
        private readonly IMailerService _mailerService;
        private readonly PaymentGatewayFactory _paymentGatewayFactory;
        private readonly IRiskifiedService _riskifiedService;
        private readonly Dictionary<APIType, List<SelectedProduct>> _failedSupplierProducts = new Dictionary<APIType, List<SelectedProduct>>();
        private readonly IAdyenPersistence _adyenPersistence;
        private readonly ICanocalizationService _icanocalizationService;
        private static bool _isRiskifiedEnabled;
        private static bool _isRiskifiedEnabledWith3D;
        private static bool _isRiskifiedTestingPhase;

        private readonly IFareHarborUserKeysCacheManager _fareHarborUserKeysCacheManager;
        private readonly bool _IsReservation;

        public BookingService(IBookingPersistence bookingPersistence, ILogger log,
            ISupplierBookingService supplierBookingService,
            IWirecardPaymentAdapter wirecardPaymentAdapter, IMasterService masterService,
            IAlternativePaymentService alternativePaymentService, IAlternativePaymentAdapter alternativePaymentAdapter, IMailerService mailerService, PaymentGatewayFactory paymentGatewayFactory
            , IRiskifiedService riskifiedService
            , IFareHarborUserKeysCacheManager fareHarborUserKeysCacheManager
            , IAdyenPersistence adyenPersistence
            , ICanocalizationService icanocalizationService
            )
        {
            _bookingPersistence = bookingPersistence;
            _supplierBookingService = supplierBookingService;
            _wirecardPaymentAdapter = wirecardPaymentAdapter;
            _masterService = masterService;
            _alternativePaymentService = alternativePaymentService;
            _log = log;
            _alternativePaymentAdapter = alternativePaymentAdapter;
            _mailerService = mailerService;
            _paymentGatewayFactory = paymentGatewayFactory;
            _riskifiedService = riskifiedService;
            var riskifiedEnable = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsRiskifiedEnabled);
            _isRiskifiedEnabled = !string.IsNullOrEmpty(riskifiedEnable) && Convert.ToBoolean(riskifiedEnable);
            var riskifiedEnabledWith3D = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsRiskifiedEnabledWith3D);
            _isRiskifiedEnabledWith3D = !string.IsNullOrEmpty(riskifiedEnabledWith3D) && Convert.ToBoolean(riskifiedEnabledWith3D);
            var riskifiedTestingPhase = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsRiskifiedTestingPhase);
            _isRiskifiedTestingPhase = !string.IsNullOrEmpty(riskifiedTestingPhase) && Convert.ToBoolean(riskifiedTestingPhase);
            _fareHarborUserKeysCacheManager = fareHarborUserKeysCacheManager;
            _adyenPersistence = adyenPersistence;
            _IsReservation = Convert.ToBoolean(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsReservation));
            _icanocalizationService = icanocalizationService;
        }

        public BookingResult Book(Booking booking, string token)
        {
            var bookingResponse = new BookingResult
            {
                BookingStatus = BookingStatus.Failed,
                BookingRefNo = booking.ReferenceNumber,
            };
            try
            {
                if (booking.SelectedProducts == null || booking.Affiliate == null)
                {
                    var message = Constant.NoSelectedProductOrAffiliate;
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "BookingService",
                        MethodName = "Book",
                        Token = token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                    };
                    _log.Error(isangoErrorEntity, new Exception(message));
                    var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        ReasonPhrase = message
                    };
                    bookingResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.NotFound, message);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking?.User?.EmailAddress, booking?.User?.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                        bookingResponse.UpdateDBLogFlag();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                    return bookingResponse;
                }

                #region isAPIBooking Allowed

                try
                {
                    var notAllowedService =
                        booking?.SelectedProducts?.FirstOrDefault(x => IsBookingAllowed(x.APIType) == false);

                    if (notAllowedService != null)
                    {
                        var msg = string.Format((Constant.APIBookingNotAllowed),
                            Convert.ToString(notAllowedService?.APIType), notAllowedService?.Id);
                        bookingResponse.BookingStatus = BookingStatus.Failed;
                        bookingResponse.StatusMessage = msg;
                        bookingResponse.IsDuplicateBooking = false;
                        bookingResponse.BookingRefNo = string.Empty;
                        bookingResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.BadRequest, msg);
                        try
                        {
                            LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                            bookingResponse.UpdateDBLogFlag();
                        }
                        catch (Exception z)
                        {
                            //ignore
                        }
                        return bookingResponse;
                    }
                }
                catch (Exception ex)
                {
                    var msg = $"{Constant.ExceptionIsAPIBooking} {ex.Message} \n {ex.StackTrace}";
                    bookingResponse.BookingStatus = BookingStatus.Failed;
                    bookingResponse.StatusMessage = msg;
                    bookingResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.InternalServerError, msg);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                        bookingResponse.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }
                    return bookingResponse;
                    //ignore
                }

                #endregion isAPIBooking Allowed

                #region Duplicate booking check

                var duplicateBookingData = CheckDuplicateBooking(booking);
                var isDuplicateBooking = duplicateBookingData?.Item1 ?? false;

                if (isDuplicateBooking)
                {
                    bookingResponse.BookingStatus = BookingStatus.Failed;
                    bookingResponse.StatusMessage = Constant.DuplicateBookingMessage;
                    bookingResponse.IsDuplicateBooking = true;
                    bookingResponse.BookingRefNo = duplicateBookingData.Item2;
                    bookingResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.BadRequest, Constant.DuplicateBookingMessage);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                        bookingResponse.UpdateDBLogFlag();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                    return bookingResponse;
                }

                #endregion Duplicate booking check

                #region Alternative Payment booking

                if (booking.PaymentGateway.Equals(PaymentGatewayType.Alternative))
                {
                    if ((!string.IsNullOrEmpty(booking.PaymentOption.ToString()) &&
                         (booking.PaymentOption.Equals(PaymentOptionType.Sofort) ||
                          booking.PaymentOption.Equals(PaymentOptionType.SafetyPay))))
                    {
                        var isAnyAPIProduct = booking.SelectedProducts.Any(x => x.APIType != APIType.Undefined);
                        var isAnyORProduct = booking.SelectedProducts.Any(x => x.ProductOptions.Any(y =>
                            y.IsSelected && y.AvailabilityStatus.Equals(AvailabilityStatus.ONREQUEST)));

                        if (isAnyAPIProduct || isAnyORProduct)
                        {
                            bookingResponse.BookingStatus = BookingStatus.Failed;
                            bookingResponse.StatusMessage = Constant.FailedBookingMessage;
                        }
                        else
                        {
                            bookingResponse.BookingStatus = BookingStatus.Initiated;
                            var alternativePaymentResult = CreateAlternativePaymentTransaction(booking, token);
                            var redirectUrl = alternativePaymentResult.Item1;
                            var transactionId = alternativePaymentResult.Item2;

                            if (string.IsNullOrEmpty(redirectUrl))
                            {
                                bookingResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.BadGateway, "RedirectUrl empty");
                                bookingResponse.BookingStatus = BookingStatus.Failed;
                                try
                                {
                                    LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                                    bookingResponse.UpdateDBLogFlag();
                                }
                                catch (Exception ex)
                                {
                                    //ignore
                                }
                            }

                            bookingResponse.Url = redirectUrl;
                            bookingResponse.TransactionId = transactionId;
                        }

                        return bookingResponse;
                    }

                    bookingResponse.StatusMessage = Constant.InvalidPayment;
                    return bookingResponse;
                }

                #endregion Alternative Payment booking

                #region Prepaid booking

                if (booking.Amount <= 0 || booking.PaymentMethodType == PaymentMethodType.Prepaid)
                {
                    var prepaidBookingResponse = CreatePrepaidBooking(booking, false, token);
                    var isBookingSucessful = prepaidBookingResponse.Item1;
                    var isPartialBooking = prepaidBookingResponse.Item2;

                    if (isBookingSucessful)
                    {
                        bookingResponse.StatusMessage = Constant.SuccessBookingMessage;
                        bookingResponse.BookingStatus = booking.Status;
                    }
                    else
                    {
                        bookingResponse.Errors = booking.Errors;
                        var checkError = bookingResponse?.Errors?.Any(x => x.Message == Constant.InvalidPrepaidBooking);
                        if (checkError == true)
                        {
                            bookingResponse.StatusMessage = Constant.InvalidPrepaid;
                        }
                        else
                        {
                            bookingResponse.StatusMessage = Constant.FailedBookingMessage;
                        }

                        bookingResponse.BookingStatus = BookingStatus.Failed;
                    }

                    return bookingResponse;
                }

                #endregion Prepaid booking

                #region Transaction booking

                if (booking.PaymentMethodType == PaymentMethodType.Transaction)
                {
                    var riskifiedEnable = _isRiskifiedEnabled && booking.isRiskifiedEnabled;
                    var riskifiedStatus = string.Empty;
                    RiskifiedAuthorizationResponse riskifiedResult = new RiskifiedAuthorizationResponse();
                    var paymentGateway = _paymentGatewayFactory.GetPaymentGatewayService(booking.PaymentGateway);

                    if (riskifiedEnable)
                    {
                        riskifiedResult = _riskifiedService.Decide(booking, token);
                        riskifiedStatus = riskifiedResult.Order.Status.ToLower();
                    }

                    if (riskifiedStatus == "approved" && !_isRiskifiedEnabledWith3D && !booking.PaymentGateway.Equals(PaymentGatewayType.Apexx) && !_isRiskifiedTestingPhase)
                    {
                        var reservationResponse = Reserve(booking, token);
                        if (reservationResponse.Success)
                        {
                            var createBookingResponse = CreateBooking(booking, false, token);
                            var isBookingSuccessful = createBookingResponse.Item1;

                            if (isBookingSuccessful)
                            {
                                var riskifiedDecisionResult = _riskifiedService.Decision(riskifiedResult, token);
                                bookingResponse.TransactionGuwid = booking.Guwid;
                                bookingResponse.BookingRefNo = booking.ReferenceNumber;
                                bookingResponse.BookingStatus = booking.Status;
                                bookingResponse.StatusMessage = Constant.SuccessBookingMessage;
                            }
                            else
                            {
                                bookingResponse.BookingStatus = BookingStatus.Failed;
                                bookingResponse.StatusMessage = Constant.FailedBookingMessage;
                            }
                        }
                        else
                        {
                            bookingResponse.BookingStatus = BookingStatus.Failed;
                            bookingResponse.StatusMessage = Constant.FailedBookingMessage;
                        }
                    }
                    else if (!riskifiedEnable
                        || (riskifiedEnable && ((riskifiedStatus == "approved" && (_isRiskifiedEnabledWith3D || booking.PaymentGateway.Equals(PaymentGatewayType.Apexx) || booking.PaymentGateway.Equals(PaymentGatewayType.Adyen))) || _isRiskifiedTestingPhase)))
                    {
                        var enrollmentResult = paymentGateway.EnrollmentCheck(booking, token);

                        if (enrollmentResult.Is2DBooking)
                        {
                            booking.Guwid = enrollmentResult.BookingReferenceId;
                            var reservationResponse = Reserve(booking, token);
                            if (reservationResponse.Success)
                            {
                                var createBookingResponse = CreateBooking(booking, false, token);
                                var isBookingSuccessful = createBookingResponse.Item1;

                                if (isBookingSuccessful)
                                {
                                    if (riskifiedEnable)
                                    {
                                        var riskifiedDecisionResult = _riskifiedService.Decision(riskifiedResult, token);
                                    }
                                    bookingResponse.BookingStatus = booking.Status;
                                    bookingResponse.StatusMessage = Constant.SuccessBookingMessage;
                                }
                                else
                                {
                                    bookingResponse.BookingStatus = BookingStatus.Failed;
                                    bookingResponse.StatusMessage = Constant.FailedBookingMessage;
                                }
                            }
                            else
                            {
                                bookingResponse.BookingStatus = BookingStatus.Failed;
                                bookingResponse.StatusMessage = Constant.FailedBookingMessage;
                            }
                        }
                        else if ((enrollmentResult.EnrollmentErrorOrHTML.IndexOf("<html>", StringComparison.Ordinal) > -1) || (enrollmentResult.EnrollmentErrorOrHTML.IndexOf("sdk", StringComparison.Ordinal) > -1))
                        {
                            var reservationResponse = Reserve(booking, token);
                            if (reservationResponse.Success)
                            {
                                bookingResponse.BookingStatus = BookingStatus.Requested;
                                bookingResponse.RequestHtml = enrollmentResult.EnrollmentErrorOrHTML;
                            }
                            else
                            {
                                bookingResponse.BookingStatus = BookingStatus.Failed;
                                bookingResponse.StatusMessage = Constant.FailedBookingMessage;
                            }
                        }
                        else
                        {
                            if (riskifiedEnable)
                            {
                                var riskifiedCheckoutDeniedResult = _riskifiedService.CheckoutDenied(booking,
                                    new AuthorizationError()
                                    {
                                        CreatedAt = DateTime.Now,
                                        ErrorCode = enrollmentResult.EnrollmentErrorCode,
                                        Message = enrollmentResult.EnrollmentErrorOrHTML
                                    }
                                    , token);
                            }
                            bookingResponse.BookingStatus = BookingStatus.Failed;
                            bookingResponse.StatusMessage = Constant.FailedBookingMessage;
                        }
                        if (enrollmentResult.BookingReferenceId != string.Empty)
                        {
                            bookingResponse.TransactionGuwid = enrollmentResult.BookingReferenceId;
                        }
                    }
                    else
                    {
                        if (riskifiedEnable && riskifiedStatus != "approved")
                        {
                            var riskifiedDecisionResult = _riskifiedService.Decision(riskifiedResult, token);
                        }
                        bookingResponse.BookingStatus = BookingStatus.Failed;
                        bookingResponse.StatusMessage = Constant.FailedBookingMessage;
                    }
                }

                #endregion Transaction booking

                return bookingResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "Book",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public ReservationResponse Reserve(Booking booking, string token)
        {
            if (!_IsReservation)
            {
                return new ReservationResponse()
                {
                    Success = true
                };
            }
            var reservationResponse = new ReservationResponse
            {
                Success = false,
                Products = new List<BookedProductStatus>(),
                BookingReferenceNumber = booking.ReferenceNumber
            };

            try
            {
                if (booking.SelectedProducts == null || booking.Affiliate == null)
                {
                    var message = Constant.NoSelectedProductOrAffiliate;
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "BookingService",
                        MethodName = "Book",
                        Token = token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                    };
                    _log.Error(isangoErrorEntity, new Exception(message));
                    var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        ReasonPhrase = message
                    };
                    reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.NotFound, message);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking?.User?.EmailAddress, booking?.User?.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                        reservationResponse.UpdateDBLogFlag();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                    return reservationResponse;
                }

                #region isAPIBooking Allowed

                try
                {
                    var notAllowedService =
                        booking?.SelectedProducts?.FirstOrDefault(x => IsBookingAllowed(x.APIType) == false);

                    if (notAllowedService != null)
                    {
                        var msg = string.Format((Constant.APIBookingNotAllowed),
                            Convert.ToString(notAllowedService?.APIType), notAllowedService?.Id);
                        reservationResponse.BookingReferenceNumber = string.Empty;
                        reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.Forbidden, msg);
                        try
                        {
                            LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                            reservationResponse.UpdateDBLogFlag();
                        }
                        catch (Exception z)
                        {
                            //ignore
                        }
                        return reservationResponse;
                    }
                }
                catch (Exception ex)
                {
                    var msg = $"{Constant.ExceptionIsAPIBooking} {ex.Message} \n {ex.StackTrace}";
                    reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.InternalServerError, msg);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                        reservationResponse.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }
                    return reservationResponse;
                    //ignore
                }

                #endregion isAPIBooking Allowed

                #region Duplicate booking check

                var duplicateBookingData = CheckDuplicateBooking(booking);
                var isDuplicateBooking = duplicateBookingData?.Item1 ?? false;

                if (isDuplicateBooking)
                {
                    reservationResponse.BookingReferenceNumber = duplicateBookingData.Item2;
                    reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.Forbidden, Constant.DuplicateBookingMessage);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                        reservationResponse.UpdateDBLogFlag();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                    return reservationResponse;
                }

                #endregion Duplicate booking check

                #region Alternative Payment booking

                if (booking.PaymentGateway.Equals(PaymentGatewayType.Alternative))
                {
                    if ((!string.IsNullOrEmpty(booking.PaymentOption.ToString()) &&
                         (booking.PaymentOption.Equals(PaymentOptionType.Sofort) ||
                          booking.PaymentOption.Equals(PaymentOptionType.SafetyPay))))
                    {
                        var isAnyAPIProduct = booking.SelectedProducts.Any(x => x.APIType != APIType.Undefined);
                        var isAnyORProduct = booking.SelectedProducts.Any(x => x.ProductOptions.Any(y =>
                            y.IsSelected && y.AvailabilityStatus.Equals(AvailabilityStatus.ONREQUEST)));

                        if (isAnyAPIProduct || isAnyORProduct)
                        {
                            reservationResponse.Success = false;
                        }
                        else
                        {
                            var alternativePaymentResult = CreateAlternativePaymentTransaction(booking, token);
                            var redirectUrl = alternativePaymentResult.Item1;
                            var transactionId = alternativePaymentResult.Item2;

                            if (string.IsNullOrEmpty(redirectUrl))
                            {
                                reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.BadGateway, "RedirectUrl empty");

                                try
                                {
                                    LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                                    reservationResponse.UpdateDBLogFlag();
                                }
                                catch (Exception ex)
                                {
                                    //ignore
                                }
                            }
                        }

                        return reservationResponse;
                    }

                    return reservationResponse;
                }

                #endregion Alternative Payment booking

                reservationResponse = CreatePrepaidReservation(booking, false, token);
                reservationResponse.BookingReferenceNumber = booking.ReferenceNumber;

                try
                {
                    if (booking?.SelectedProducts?.FirstOrDefault()?.APIType == APIType.Tiqets)
                    {
                        reservationResponse.ExpirationTimeInMinutes = 30;
                    }
                }
                catch
                {
                }

                return reservationResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "Book",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public ReservationResponse CancelReservation(Booking booking, string token)
        {
            var reservationResponse = new ReservationResponse
            {
                Success = false,
                Products = new List<BookedProductStatus>(),
                BookingReferenceNumber = booking.ReferenceNumber
            };
            try
            {
                if (booking.SelectedProducts == null)
                {
                    var message = Constant.NoSelectedProductOrAffiliate;
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "BookingService",
                        MethodName = "CancelReservation",
                        Token = token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                    };
                    _log.Error(isangoErrorEntity, new Exception(message));
                    var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        ReasonPhrase = message
                    };
                    reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.NotFound, message);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking?.User?.EmailAddress, booking?.User?.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                        reservationResponse.UpdateDBLogFlag();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                    return reservationResponse;
                }

                reservationResponse = SupplierReservationCancellation(booking, token, new List<string>());
                reservationResponse.BookingReferenceNumber = booking.ReferenceNumber;

                return reservationResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CancelReservation",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #region Alternative payment booking

        /// <summary>
        /// This operation is used to handle logging and mailing
        /// in case of transaction success webhook
        /// </summary>
        /// <param name="booking"></param>
        public void ProcessTransactionSuccess(Booking booking, bool? isAlternativePayment = false)
        {
            try
            {
                booking = CreatePaymentForORAndFS(booking, isAlternativePayment);

                var isangoBookingData = MapIsangoBookingData(booking);
                _log.Info(
                    $"BookingService|FinalIsangoBookingData|{SerializeDeSerializeHelper.Serialize(isangoBookingData)}");
                var isBookingSuccess = CreateIsangoBookingInDB(isangoBookingData, true);

                var criteria = new WireCardXmlCriteria
                {
                    JobId = booking.User.UserId + "_" + booking.ReferenceNumber,
                    TransGuWId = string.IsNullOrEmpty(booking.Payment[0].TransactionId)
                        ? string.Empty
                        : booking.Payment[0].TransactionId,
                    Status = Constant.StatusSuccessFull,
                    Request = Constant.TransactionCompleted,
                    Response = Constant.TransactionCompleted,
                    RequestType = Constant.Alternativepayment,
                    TransDate = DateTime.Now
                };
                LogTransaction(criteria);

                if (isBookingSuccess)
                {
                    AfterBookingProcess(booking, null, null, isAlternativePayment);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ProcessTransactionSuccess",
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                var criteria = new WireCardXmlCriteria
                {
                    JobId = booking.User.UserId + "_" + booking.ReferenceNumber,
                    TransGuWId = string.IsNullOrEmpty(booking.Payment[0].TransactionId)
                        ? string.Empty
                        : booking.Payment[0].TransactionId,
                    Status = Constant.FailedStatus,
                    TransDate = DateTime.Now,
                    Request = Constant.BookingFailedInTransactionSuccess,
                    Response = Constant.BookingFailedInTransactionSuccess,
                    RequestType = Constant.Alternativepayment
                };
                LogTransaction(criteria);

                throw;
            }
        }

        /// <summary>
        /// This operation is used to handle logging and mailing
        /// in case of transaction fail webhook
        /// </summary>
        /// <param name="booking"></param>
        public void ProcessTransactionFail(Booking booking)
        {
            try
            {
                var criteria = new WireCardXmlCriteria
                {
                    JobId = booking.User.UserId + "_" + booking.ReferenceNumber,
                    TransGuWId = string.IsNullOrEmpty(booking.Payment[0].TransactionId)
                        ? string.Empty
                        : booking.Payment[0].TransactionId,
                    Status = Constant.FailedStatus,
                    Request = Constant.TransactionFailed,
                    Response = Constant.TransactionFailed,
                    RequestType = Constant.Alternativepayment
                };
                LogTransaction(criteria);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ProcessTransactionFail",
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This operation is used to handle logging, mailing
        /// depending the status received in transaction web-hook
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="jsonPostedData"></param>
        /// <param name="absoluteUri"></param>
        /// <param name="token"></param>
        public void ProcessTransactionWebHook(Booking booking, string jsonPostedData, string absoluteUri, string token)
        {
            var isBookingSuccessful = false;
            try
            {
                if (booking == null) return;

                var alternativeTransactionResponse =
                    SerializeDeSerializeHelper.DeSerialize<AlternativeTransactionResponse>(jsonPostedData);
                var criteria = new WireCardXmlCriteria
                {
                    JobId = booking.User.UserId + "_" + booking.ReferenceNumber,
                    TransGuWId = string.IsNullOrEmpty(alternativeTransactionResponse.resource.id)
                        ? string.Empty
                        : alternativeTransactionResponse.resource.id,
                    Status = alternativeTransactionResponse.resource.status,
                    TransDate = DateTime.Now,
                    Request = Constant.WebHookResponse,
                    Response = jsonPostedData,
                    RequestType = Constant.Alternativepayment
                };
                LogTransaction(criteria);

                var authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(GetAPIKeyUrl(absoluteUri)));
                var baseUrl = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternateGetAPIURL);
                var transactionResponse = _alternativePaymentService
                    .GetAlternativeTransaction(alternativeTransactionResponse.resource.id, authInfo, token, baseUrl)
                    .Result;

                criteria.Request = Constant.WebHookResponseAfterGetTrans;
                criteria.Response = SerializeDeSerializeHelper.Serialize(transactionResponse);
                LogTransaction(criteria);

                Enum.TryParse(transactionResponse.Status, true, out AlternativePaymentStatus transactionStatus);

                switch (transactionStatus)
                {
                    case AlternativePaymentStatus.FUNDED:
                        isBookingSuccessful =
                            ProcessFundedWebhookResponse(booking, alternativeTransactionResponse, token);
                        break;

                    case AlternativePaymentStatus.DECLINED:
                        ProcessDeclinedWebhookResponse(booking, alternativeTransactionResponse);
                        break;

                    case AlternativePaymentStatus.CHARGEBACK:
                        ProcessChargebackWebhookResponse(alternativeTransactionResponse, jsonPostedData);
                        break;

                    case AlternativePaymentStatus.UNDEFINED:
                    case AlternativePaymentStatus.PENDING:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                isBookingSuccessful = false;
                var criteria = new WireCardXmlCriteria
                {
                    JobId = Constant.WebhookException,
                    TransGuWId = string.IsNullOrEmpty(booking?.Payment[0].TransactionId)
                        ? string.Empty
                        : booking.Payment[0].TransactionId,
                    Status = Constant.WebhookResponseFailed,
                    TransDate = DateTime.Now,
                    Request = Constant.WebHookResponse,
                    Response = ex.Message,
                    RequestType = Constant.Alternativepayment
                };
                LogTransaction(criteria);
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ProcessTransactionWebHook",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            finally
            {
                if (isBookingSuccessful)
                {
                    //Send mail to customer and supplier on successful booking
                    //Need to verify - mail goes two times for fail case
                    AfterBookingProcess(booking, null, null, false);
                }
            }
        }

        #endregion Alternative payment booking

        #region Get Booking

        /// <summary>
        /// This operation is used to confirm the email with booking reference
        /// </summary>
        /// <param name="email"></param>
        /// <param name="bookingRef"></param>
        /// <returns></returns>
        public bool MatchBookingByEmailRef(string email, string bookingRef)
        {
            try
            {
                return _bookingPersistence.MatchBookingByEmailRef(email, bookingRef);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "MatchBookingByEmailRef",
                    Params = $"{email}{bookingRef}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<ReservationDBDetails> GetResrvationDetailsFromDB(string bookingRef)
        {
            try
            {
                return _bookingPersistence.GetReservationData(bookingRef);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "MatchBookingByEmailRef",
                    Params = $"{bookingRef}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This operation is used to insert failed booking details in db
        /// </summary>
        /// <param name="failedBooking"></param>
        /// <returns></returns>
        public string SaveFailedBookingInDb(Booking failedBooking)
        {
            try
            {
                var hbProducts =
                    failedBooking.SelectedProducts.FindAll(hbproducts => hbproducts.APIType.Equals(APIType.Hotelbeds));

                //Temporarily checking if the cart contains Hb products and if yes, checking whether the cart has only hb products.
                //If thats the condition then we are not sending failure mail and neither we are removing the product from the cart, so that the customer can continue again if he/she wants to
                if (failedBooking.SelectedProducts.Count == hbProducts.Count)
                    return string.Empty;

                var savedProduct = failedBooking.SelectedProducts.FindAll(product =>
                    product.APIType.Equals(APIType.Undefined) || product.APIType.Equals(APIType.Citysightseeing));

                var selectedProductText = GetSerializedSelectedProducts(savedProduct);

                if (!string.IsNullOrEmpty(selectedProductText))
                {
                    var hashText = StringConversionHelper.Hash(
                        $"{failedBooking.User.EmailAddress},{failedBooking.Affiliate.Id},{selectedProductText},{(int)SaveCartType.PAYMENTFAILURE}");
                    var affiliateId = new Guid(failedBooking.Affiliate.Id);
                    var isSaved = _bookingPersistence.SaveFailedBookingInDb(affiliateId,
                        failedBooking.User.EmailAddress, selectedProductText, hashText,
                        (int)SaveCartType.PAYMENTFAILURE);

                    if (isSaved)
                    {
                        return hashText;
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "SaveFailedBookingInDb",
                    AffiliateId = failedBooking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(failedBooking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LogBookingFailureInDB(Booking failedBooking, string bookingRefNo, int? serviceID, string tokenID, string apiRefID, string custEmail, string custContact, int? ApiType, int? optionID, string optionName, string avlbltyRefID, string ErrorLevel)
        {
            Task.Run(() => _bookingPersistence.LogBookingFailureInDB(failedBooking, bookingRefNo, serviceID, tokenID, apiRefID, custEmail, custContact, ApiType, optionID, optionName, avlbltyRefID, ErrorLevel));
        }

        /// <summary>
        /// This method creates a booking in isango database and hits the wire-card and return the updated booking object.
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="amendmentId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool AmendBooking(Booking booking, int amendmentId, string token)
        {
            try
            {
                return ProcessAmendedBooking(booking, amendmentId, token);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "AmendBooking",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)},{amendmentId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public bool RefundService(int amendmentId, string remarks, string actionBy, string token)
        {
            try
            {
                return ProcessBookBack(amendmentId, remarks, actionBy, token);
            }
            catch (Exception ex)
            {
                //Booking reference number to be used in the error logging process
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "RefundService",
                    Token = token,
                    Params = $"{amendmentId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        ///<summary>
        ///Method for getting Booking data for Booking Manager's payment process
        ///</summary>
        public PaymentBookingData ReceivePaymentDataService(int amendmentId)
        {
            try
            {
                var dataForPayRec = _bookingPersistence.GetPaymentRelatedBookingData(amendmentId);
                return dataForPayRec;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ReceivePaymentDataService",
                    Params = $"{amendmentId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public PaymentBookingData PartialRefundDataService(int amendmentId)
        {
            try
            {
                var dataForPayRec = _bookingPersistence.GetPartialRefundData(amendmentId);
                return dataForPayRec;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "PartialRefundDataService",
                    Params = $"{amendmentId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public bool LogTransaction(WireCardXmlCriteria wireCardXmlCriteria)
        {
            try
            {
                return _bookingPersistence.InsertWirecardXml(wireCardXmlCriteria);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "LogTransaction",
                    Params = $"{SerializeDeSerializeHelper.Serialize(wireCardXmlCriteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public string GetAuthString(string affiliateId)
        {
            try
            {
                var cacheResult = _masterService.GetHBAuthorizationDataAsync()?.GetAwaiter().GetResult();
                var authentication = cacheResult.FirstOrDefault(x => x.AffiliateId == affiliateId)?.Authentication;
                if (string.IsNullOrEmpty(authentication))
                    authentication = Constant.GetTicketAuthString;
                return authentication;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetAuthString",
                    AffiliateId = affiliateId,
                    Params = $"{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public BookingResult Create3DBooking(Booking booking, string token)
        {
            var riskifiedEnable = _isRiskifiedEnabled && booking.isRiskifiedEnabled;
            try
            {
                var bookingResponse = new BookingResult();
                var createBookingResponse = CreateBooking(booking, true, token);

                var isBookingSuccessful = createBookingResponse.Item1;
                var isPartialBooking = createBookingResponse.Item2;
                bookingResponse.BookingRefNo = booking.ReferenceNumber;
                if (isBookingSuccessful)
                {
                    if (riskifiedEnable)
                    {
                        var riskifiedDecisionResult = _riskifiedService.Decision(
                            new RiskifiedAuthorizationResponse()
                            {
                                Order = new OrderResponse()
                                {
                                    Id = booking.ReferenceNumber,
                                    Status = "approved"
                                }
                            }, token
                            );
                    }
                    bookingResponse.BookingStatus = booking.Status;
                    bookingResponse.StatusMessage = Constant.SuccessBookingMessage;
                }
                else if (createBookingResponse.Item3 != null)
                {
                    if (string.IsNullOrEmpty(createBookingResponse.Item3?.ErrorMessage))
                    {
                        bookingResponse.BookingStatus = BookingStatus.Requested;
                        bookingResponse.RequestHtml = createBookingResponse.Item3?.AcsRequest;
                        bookingResponse.IsWebhookReceived = createBookingResponse?.Item3?.IsWebhookReceived ?? false;
                        bookingResponse.TransactionGuwid = createBookingResponse.Item3?.TransactionID;
                        bookingResponse.FallbackFingerPrint = createBookingResponse.Item3?.FallbackFingerPrint;
                    }
                    else
                    {
                        bookingResponse.BookingStatus = BookingStatus.Failed;
                        bookingResponse.StatusMessage = Constant.FailedBookingMessage;
                        bookingResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.NotFound, createBookingResponse.Item3?.ErrorMessage);
                        try
                        {
                            LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                            bookingResponse.UpdateDBLogFlag();
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                    }
                }
                else
                {
                    bookingResponse.BookingStatus = BookingStatus.Failed;
                    bookingResponse.StatusMessage = Constant.FailedBookingMessage;
                }

                return bookingResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "Create3DBooking",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get the booking data for the given booking reference number
        /// </summary>
        /// <param name="bookingReferenceNumber"></param>
        public ConfirmBookingDetail GetBookingData(string bookingReferenceNumber)
        {
            try
            {
                var baseUrl = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl);
                var bookingData = _bookingPersistence.GetBookingData(bookingReferenceNumber);

                foreach (var bo in bookingData.BookedOptions)
                {
                    if (bo != null)
                    {
                        if ((APIType)bo.ApiType == APIType.Moulinrouge)
                        {
                            bo.IsShowSupplierVoucher = true;
                            bo.LinkType = "Link";
                            bo.LinkValue = $"{baseUrl}/api/voucher/book/{bookingData.BookingReferenceNumber}/1/{bo.BookedOptionId}/false";
                        }
                    }
                }
                return bookingData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetBookingData",
                    Params = $"{bookingReferenceNumber}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// After OR products get confirmed by supplier, this will update booking product status in DB and charge the customer for the price of OR product.
        /// </summary>
        /// <param name="bookedOptionId"></param>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="isCalledByAsyncJob"></param>
        /// <returns></returns>
        public Tuple<bool, string> ConfirmBooking(int bookedOptionId, string userId, string token, bool isCalledByAsyncJob = false)
        {
            // var riskifiedEnabled = _isRiskifiedEnabled && booking.isRiskifiedEnabled;
            bool isTransactionSuccess;
            var bookingReferenceNumber = string.Empty;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { Timeout = new TimeSpan(0, 3, 0) }))
            {
                try
                {
                    var bookedProductPaymentData = _bookingPersistence.ConfirmBookingUpdateStatusAndGetPaymentData(userId, bookedOptionId);
                    if (bookedProductPaymentData == null)
                    {
                        var message = "Failed to update status in DB and retrieve payment data";
                        //var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                        //{
                        //    ReasonPhrase = message
                        //};
                        transaction.Dispose();
                        //throw new HttpResponseException(data);
                        throw new WebApiException(message, HttpStatusCode.NotFound);

                    }
                    if (bookedProductPaymentData != null && bookedProductPaymentData.Amount == 0)
                    {
                        transaction.Complete();
                    }

                    bookingReferenceNumber = bookedProductPaymentData.BookingReferenceNumber;
                    if (bookedProductPaymentData.PaymentGateway != PaymentGatewayType.Undefined)
                    {
                        var preAuthPayment = new Payment
                        {
                            PaymentStatus = PaymentStatus.PreAuthorized,
                            TransactionFlowType = TransactionFlowType.Payment,
                            ChargeAmount = bookedProductPaymentData.Amount,
                            PaymentGatewayReferenceId = bookedProductPaymentData.GuWId,
                            CurrencyCode = bookedProductPaymentData.CurrencyCode.ToUpper(),
                            Is3D = bookedProductPaymentData.Is3D,
                            Token = bookedProductPaymentData.AuthorizationCode
                        };

                        var purchasePayment = new Payment
                        {
                            ChargeAmount = bookedProductPaymentData.Amount,
                            PaymentStatus = PaymentStatus.Paid,
                            CurrencyCode = bookedProductPaymentData.CurrencyCode,
                            Guwid = bookedProductPaymentData.GuWId,
                            TransactionId = UniqueTransactionIdGenerator.GenerateTransactionId(),
                            PaymentGatewayReferenceId = bookedProductPaymentData.GuWId,
                            Token = bookedProductPaymentData.AuthorizationCode
                        };

                        var booking = new Booking
                        {
                            Guwid = bookedProductPaymentData.GuWId,
                            User = new ISangoUser(),
                            ReferenceNumber = bookedProductPaymentData.BookingReferenceNumber,
                            Payment = new List<Payment>
                            {
                                preAuthPayment,
                                purchasePayment,
                            },
                            Currency = new Currency()
                            {
                                IsoCode = bookedProductPaymentData.CurrencyCode
                            },
                            Date = bookedProductPaymentData.BookingDate ?? DateTime.Now,
                            AdyenMerchantAccountCancelRefund = bookedProductPaymentData.AdyenMerchantAccount
                        };

                        var paymentgateway = _paymentGatewayFactory.GetPaymentGatewayService(bookedProductPaymentData.PaymentGateway);
                        if (paymentgateway == null)
                        {
                            var message = "Payment gateway is null";
                            //var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                            //{
                            //    ReasonPhrase = message
                            //};
                            transaction.Dispose();
                            //throw new HttpResponseException(data);
                            throw new WebApiException(message, HttpStatusCode.NotFound);

                        }
                        var newTransactionResponse = new PaymentGatewayResponse();
                        if (bookedProductPaymentData.PaymentGateway == PaymentGatewayType.Apexx)
                        {
                            newTransactionResponse = paymentgateway.CreateNewTransaction(booking, token);
                        }
                        else
                        {
                            newTransactionResponse.IsSuccess = true;
                        }
                        if (newTransactionResponse.IsSuccess)
                        {
                            if (bookedProductPaymentData?.CardType?.ToLower() == "ideal" || bookedProductPaymentData?.CardType?.ToLower() == "sofort")
                            {
                                booking.FallbackFingerPrint = bookedProductPaymentData?.CardType;
                            }

                            var transactionResponse = paymentgateway.Transaction(booking, bookedProductPaymentData.Is3D, token);
                            isTransactionSuccess = transactionResponse.IsSuccess;
                            //if (transactionResponse.IsSuccess && bookedProductPaymentData.PaymentGateway == PaymentGatewayType.Adyen)
                            //{
                            //    isTransactionSuccess = paymentgateway.WebhookConfirmation(bookedProductPaymentData.BookingReferenceNumber, 4, "capture")?.GetAwaiter().GetResult() ?? false;
                            //}
                            //else
                            //{
                            //    isTransactionSuccess = transactionResponse.IsSuccess;
                            //}
                            if (!isTransactionSuccess)
                            {
                                var message = "Payment gateway transaction failed";
                                //var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                                //{
                                //    ReasonPhrase = message
                                //};
                                transaction.Dispose();
                                //throw new HttpResponseException(data);
                                throw new WebApiException(message, HttpStatusCode.NotFound);

                            }
                            var updatedPurchasePayment = booking.Payment.Find(FilterPurchasePayment);
                            //No Need to Send Authorization Code, Not Use AnyWhere
                            var isStatusUpdated = _bookingPersistence.UpdatePaymentStatus(bookedProductPaymentData.CaptureTransactionId, updatedPurchasePayment.PaymentGatewayReferenceId, string.Empty, booking.Guwid);
                            if (!isStatusUpdated)
                            {
                                var message = "Failed to update status of booking in database after charging successful ";
                                //var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                                //{
                                //    ReasonPhrase = message
                                //};
                                transaction.Dispose();
                                //throw new HttpResponseException(data);
                                throw new WebApiException(message, HttpStatusCode.NotFound);

                            }
                            transaction.Complete();
                        }
                        else
                        {
                            _log.Error(new IsangoErrorEntity
                            {
                                ClassName = "BookingService",
                                MethodName = "ConfirmBooking",
                                Params = $"{bookedOptionId}{userId}",
                                Token = token
                            }, new Exception("New Transaction Fail from API "));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                        _log.Error(new IsangoErrorEntity
                        {
                            ClassName = "BookingService",
                            MethodName = "ConfirmBooking",
                            Params = $"{bookedOptionId}{userId}",
                            Token = token
                        }, ex)
                    );
                    transaction.Dispose();
                    return new Tuple<bool, string>(false, ex.GetBaseException().Message);
                }
            }

            try
            {
                if (isCalledByAsyncJob)
                {
                    var bookedOptionMailDatas = _bookingPersistence.CheckToSendmailToCustomer(bookedOptionId);
                    if (bookedOptionMailDatas.Any() && !string.IsNullOrWhiteSpace(bookingReferenceNumber))
                        _mailerService.SendMail(bookingReferenceNumber, null, false, true, null, null, false, true);
                }
                return new Tuple<bool, string>(true, "");
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ConfirmBooking",
                    Params = $"{bookedOptionId}{userId}",
                    Token = token
                };
                _log.Error(isangoErrorEntity, ex);
                return new Tuple<bool, string>(false, ex.GetBaseException().Message);
            }
        }

        #region Partial Refund API

        /// <summary>
        /// This method will process the partial refund against the given amended booking
        /// </summary>
        /// <param name="amendmentId"></param>
        /// <param name="remarks"></param>
        /// <param name="actionBy"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Tuple<bool, string> ProcessPartialRefund(int amendmentId, string remarks, string actionBy, string token)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    var partialRefundPaymentData = _bookingPersistence.InsertPartialRefundAndGetPaymentInfo(amendmentId, remarks, actionBy);

                    if (partialRefundPaymentData == null)
                    {
                        var message = "Error occurred while fetching partial refund payment information.";
                        //var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                        //{
                        //    ReasonPhrase = message
                        //};
                        //throw new HttpResponseException(data);
                        throw new WebApiException(message, HttpStatusCode.NotFound);

                    }

                    #region Payment Gateway Refund Call

                    if (partialRefundPaymentData.PaymentGateway != PaymentGatewayType.Undefined)
                    {
                        var preAuthPayment = new Payment
                        {
                            PaymentStatus = PaymentStatus.PreAuthorized,
                            TransactionFlowType = TransactionFlowType.Payment,
                            ChargeAmount = 0,
                            PaymentGatewayReferenceId = partialRefundPaymentData.GuWId,
                            CurrencyCode = partialRefundPaymentData.CurrencyCode.ToUpper(),
                            Is3D = partialRefundPaymentData.Is3DSecure
                        };

                        var purchasePayment = new Payment
                        {
                            ChargeAmount = partialRefundPaymentData.Amount,
                            CurrencyCode = partialRefundPaymentData.CurrencyCode,
                            PaymentStatus = PaymentStatus.Paid,
                            TransactionId = partialRefundPaymentData.BookBackTransactionId.ToString(),
                            PaymentGatewayReferenceId = partialRefundPaymentData.GuWId,
                            Guwid = partialRefundPaymentData.GuWId,
                            JobId = $"{partialRefundPaymentData.UserId}_{partialRefundPaymentData.BookingReferenceNumber}",
                            Is3D = partialRefundPaymentData.Is3DSecure
                        };

                        var booking = new Booking
                        {
                            Guwid = partialRefundPaymentData.GuWId,
                            User = new ISangoUser(),
                            ReferenceNumber = partialRefundPaymentData.BookingReferenceNumber,
                            Date = partialRefundPaymentData.BookingDate ?? DateTime.Now,
                            AdyenMerchantAccountCancelRefund = partialRefundPaymentData?.AdyenMerchantAccout
                        };

                        booking.Payment = new List<Payment>
                        {
                            preAuthPayment,
                            purchasePayment
                        };

                        var paymentgateway = _paymentGatewayFactory.GetPaymentGatewayService(partialRefundPaymentData.PaymentGateway);
                        if (paymentgateway == null)
                        {
                            var message = "Payment gateway is null";
                            //var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                            //{
                            //    ReasonPhrase = message
                            //};
                            //throw new HttpResponseException(data);
                            throw new WebApiException(message, HttpStatusCode.NotFound);

                        }
                        var refundResponse = paymentgateway.Refund(booking, remarks, token);
                        var isTransactionSuccess = refundResponse.IsSuccess;
                        if (!isTransactionSuccess)
                        {
                            var message = "Payment gateway transaction failed";
                            //var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                            //{
                            //    ReasonPhrase = message
                            //};
                            //throw new HttpResponseException(data);
                            throw new WebApiException(message, HttpStatusCode.NotFound);

                        }

                        var updatedPurchasePayment = booking.Payment.Find(FilterPurchasePayment);
                        var isStatusUpdated = _bookingPersistence.UpdatePaymentStatus
                            (partialRefundPaymentData.BookBackTransactionId,
                            refundResponse.Guwid,
                            partialRefundPaymentData.AuthorizationCode);

                        if (!isStatusUpdated)
                        {
                            var message = "Failed to update status of booking in database after charging successful ";
                            //var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                            //{
                            //    ReasonPhrase = message
                            //};
                            //throw new HttpResponseException(data);
                            throw new WebApiException(message, HttpStatusCode.NotFound);

                        }
                    }

                    #endregion Payment Gateway Refund Call

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                        _log.Error(new IsangoErrorEntity
                        {
                            ClassName = "BookingService",
                            MethodName = "ProcessPartialRefund",
                            Params = $"{amendmentId}{remarks}{actionBy}",
                            Token = token
                        }, ex)
                    );
                    return new Tuple<bool, string>(false, ex.GetBaseException().Message);
                }
                finally
                {
                    transaction.Dispose();
                }
                return new Tuple<bool, string>(true, "");
            }
        }

        #endregion Partial Refund API

        #endregion Get Booking

        #region Cancellation API related methods

        /// <summary>
        /// Get booking details by booking reference number , userid and statusid
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="userId"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public async Task<List<BookingDetail>> GetBookingDetailAsync(string referenceNumber, string userId, string statusId)
        {
            var bookingDetails = new List<BookingDetail>();
            try
            {
                if (userId != string.Empty)
                {
                    bookingDetails = _bookingPersistence.GetBookingDetails(referenceNumber, userId, statusId);
                }
                return await Task.FromResult(bookingDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetBookingDetailAsync",
                    Params = $"{referenceNumber}{userId}{statusId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Cancel supplier booking for single product
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> CancelSupplierBookingAsync(Booking booking, string token, bool isBookingManager = false)
        {
            try
            {
                var authentication = booking?.Language?.Code;
                booking?.SelectedProducts.ForEach(x =>
                {
                    if (x.APIType == APIType.Fareharbor)
                    {
                        var code = booking.SelectedProducts.Select(m => (FareHarborSelectedProduct)m).Select(y => y.Code)
                            .FirstOrDefault();
                        var userKey = _fareHarborUserKeysCacheManager.GetFareHarborUserKeys()
                            .Where(y => y.CompanyShortName.Trim()
                                .Equals(code, StringComparison.InvariantCultureIgnoreCase))
                            .Select(s => s.UserKey).First();
                        x.ProductOptions.ForEach(y => { ((ActivityOption)y).UserKey = userKey; });
                    }
                });

                //notProcessedProductIds is not applicable for cancellation api flow
                var notProcessedProductIds = new List<string>();

                var isCancelled = SupplierBookingCancellation(booking, authentication, token, notProcessedProductIds, isBookingManager).Values
                    .FirstOrDefault();
                return await Task.FromResult(isCancelled);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CancelSupplierBookingAsync",
                    Token = token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get option and service name of booked product
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <param name="isSupplier"></param>
        /// <param name="bookedOptionId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetOptionAndServiceNameAsync(string bookingRefNo, bool isSupplier,
            string bookedOptionId)
        {
            try
            {
                var bookingOptionNameAndServiceName = new Dictionary<string, string>();
                var bookingVoucherData = _bookingPersistence.GetBookingDataForMail(bookingRefNo, false);
                var bookingDataOthers = (BookingDataOthers)bookingVoucherData;
                var bookedBookedProductDetail =
                    bookingDataOthers?.BookedProductDetailList?.FirstOrDefault(x => x.BookedOptionId == bookedOptionId);
                bookingOptionNameAndServiceName.Add("ServiceName", bookedBookedProductDetail?.ServiceName);
                bookingOptionNameAndServiceName.Add("OptionName", bookedBookedProductDetail?.OptionName);
                return await Task.FromResult(bookingOptionNameAndServiceName);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetOptionAndServiceNameAsync",
                    Params = $"{bookingRefNo}{isSupplier}{bookedOptionId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #endregion Cancellation API related methods

        #region Private Methods

        private async Task<Transaction> CreateAlternativeTransaction(Transaction transaction, string apiKey,
            string token, string baseUrl = null)
        {
            try
            {
                var request = SerializeDeSerializeHelper.SerializeWithNullValueHandling(transaction);
                var result = await _alternativePaymentAdapter.Create(apiKey, baseUrl, request, token);
                var isSuccessStatusCode = result.Item1;
                var responseObject = result.Item2;
                if (isSuccessStatusCode)
                    return SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<Transaction>(responseObject);
                return null;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreateAlternativeTransaction",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private async Task<Transaction> CreateTransaction(Booking booking, string successUrl, string failureUrl,
            string token)
        {
            try
            {
                var authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(GetAPIKeyUrl(successUrl)));
                var baseUrl = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternateApiurl);

                var customerBuilder = new AlternativePaymentCustomer.Builder(booking.User.FirstName, booking.User.LastName,
                    booking.User.EmailAddress, booking.User.Country);

                var customer = customerBuilder.Build();
                var transactionBuild = new Transaction.Builder(customer,
                    decimal.Parse((booking.Amount * 100).ToString("0")),
                    booking.Currency.IsoCode, booking.IpAddress,
                    ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternateAPIMode));

                transactionBuild.WithRedirectUrls(successUrl, failureUrl);

                var transaction = transactionBuild.Build();
                var responseTransaction = await CreateAlternativeTransaction(transaction, authInfo, token, baseUrl);

                //insert to db
                LogTransactionDetails(transaction, responseTransaction, booking);
                return responseTransaction;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreateTransaction",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private void LogTransactionDetails(Transaction transaction, Transaction responseTransaction, Booking booking)
        {
            try
            {
                var requestTransacionJson = SerializeDeSerializeHelper.Serialize(transaction);
                var responseTransactionJson = SerializeDeSerializeHelper.Serialize(responseTransaction);

                var xmlCriteria = new WireCardXmlCriteria
                {
                    JobId = booking.User.UserId + "_" + booking.ReferenceNumber,
                    TransGuWId = string.IsNullOrEmpty(booking.Payment?[0].TransactionId)
                        ? string.Empty
                        : booking.Payment[0].TransactionId,
                    TransDate = DateTime.Now,
                    Status = responseTransaction != null ? responseTransaction.Status : "Failed",
                    Request = requestTransacionJson,
                    Response = responseTransactionJson,
                    RequestType = Constant.Alternativepayment
                };
                LogTransaction(xmlCriteria);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "LogTransactionDetails",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private string GetAPIKeyUrl(string requestUrl)
        {
            var url = ExtractDomainName(requestUrl);
            string secretKey;
            try
            {
                switch (url)
                {
                    case Constant.HopOnHoffOffBus:
                        secretKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternateAPIKeyHOHO);
                        break;

                    case Constant.LocalGranCanariaTour:
                        secretKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternateAPIKeyLGCT);
                        break;

                    case Constant.AlhambraGranadaTour:
                        secretKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternateAPIKeyAGT);
                        break;

                    case Constant.LocalDubaiTours:
                        secretKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternateAPIKeyLDT);
                        break;

                    case Constant.LocalVeniceTours:
                        secretKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternateAPIKeyLVT);
                        break;

                    case Constant.LocalParisTours:
                        secretKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternateAPIKeyLPT);
                        break;

                    default:
                        secretKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternateAPIKey);
                        break;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetAPIKeyUrl",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return secretKey;
        }

        private string ExtractDomainName(string url)
        {
            try
            {
                if (url.Contains(@"://"))
                    url = url.Split(new[] { "://" }, 2, StringSplitOptions.None)[1];

                return url.Split('/')[0];
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ExtractDomainName",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private bool ProcessFundedWebhookResponse(Booking booking, AlternativeTransactionResponse response,
            string token)
        {
            try
            {
                var isBookingSuccessfull = _alternativePaymentService
                    .CompleteTransactionAfterBookingAsync(booking.ReferenceNumber, response.resource.id, token).Result;
                var criteria = new WireCardXmlCriteria
                {
                    JobId = booking.User.UserId + "_" + booking.ReferenceNumber,
                    TransGuWId = string.IsNullOrEmpty(response.resource.id) ? string.Empty : response.resource.id,
                    TransDate = DateTime.Now,
                    RequestType = Constant.Alternativepayment
                };

                if (isBookingSuccessfull)
                {
                    criteria.Status = Constant.StatusSuccessFull;
                    criteria.Request = Constant.WebhookTransactionCompleted;
                    criteria.Response = Constant.WebhookTransactionCompleted;
                    LogTransaction(criteria);
                }
                else
                {
                    criteria.Status = Constant.FailedStatus;
                    criteria.Request = Constant.WebhookResponseFailed;
                    criteria.Response = Constant.WebhookResponseFailed;
                    LogTransaction(criteria);
                }

                return isBookingSuccessfull;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ProcessFundedWebhookResponse",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private void ProcessDeclinedWebhookResponse(Booking booking, AlternativeTransactionResponse response)
        {
            try
            {
                var criteria = new WireCardXmlCriteria
                {
                    JobId = booking.User.UserId + "_" + booking.ReferenceNumber,
                    TransGuWId = string.IsNullOrEmpty(response.resource.id) ? string.Empty : response.resource.id,
                    Status = response.resource.status,
                    TransDate = DateTime.Now,
                    Request = Constant.WebhookResponseAfterGetDeclined,
                    Response = Constant.StatusDeclined,
                    RequestType = Constant.Alternativepayment
                };
                LogTransaction(criteria);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ProcessDeclinedWebhookResponse",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private void ProcessChargebackWebhookResponse(AlternativeTransactionResponse response, string jsonPostedData)
        {
            try
            {
                var criteria = new WireCardXmlCriteria
                {
                    JobId = Constant.Chargeback,
                    TransGuWId = response.resource.id, // zero in unity. Need to verify
                    Status = Constant.WebhookResponseChargeBack,
                    TransDate = DateTime.Now,
                    Request = Constant.Chargeback,
                    Response = jsonPostedData,
                    RequestType = Constant.Alternativepayment
                };

                var isSuccessFull = _alternativePaymentService
                    .UpdateSofortChargeBackAsync(response.resource.id, response.resource.status).Result;
                LogTransaction(criteria);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ProcessChargebackWebhookResponse",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Tuple<string, string> CreateAlternativePaymentTransaction(Booking booking, string token)
        {
            try
            {
                var redirectUrl = string.Empty;
                var transactionId = string.Empty;
                var successUrl =
                    $"{booking.Affiliate.AffiliateBaseURL}{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternartiveSuccessUrl)}";
                var failureUrl =
                    $"{booking.Affiliate.AffiliateBaseURL}{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternartiveFailureUrl)}";

                var transactionResult = CreateTransaction(booking, successUrl, failureUrl, token).Result;

                if (transactionResult != null)
                {
                    redirectUrl = transactionResult.RedirectUrl;
                    transactionId = transactionResult.Id;
                }

                return new Tuple<string, string>(redirectUrl, transactionId);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreateAlternativePaymentTransaction",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private string GetSerializedSelectedProducts(object selectedProducts)
        {
            try
            {
                //TODO: resolve seriallization logic
                //  var binder = new TypeNameSerializationBinder("Isango.Contracts.Entities.{0}, Isango.Contracts");
                // Create Serialize json string
                var selectedProductText = JsonConvert.SerializeObject(selectedProducts,
                    (Formatting)System.Xml.Formatting.None,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        //     Binder = binder
                    });

                return selectedProductText;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetSerializedSelectedProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Tuple<bool, string> CheckDuplicateBooking(Booking booking)
        {
            try
            {
                var firstSelectedProduct = booking.SelectedProducts?.FirstOrDefault();
                var productOption = firstSelectedProduct?.ProductOptions?.FirstOrDefault(x => x.IsSelected);
                if (productOption == null) return new Tuple<bool, string>(false, string.Empty);

                var leadPax = ((ProductOption)productOption).Customers.FirstOrDefault(x => x.IsLeadCustomer);

                var bookedProduct = booking.IsangoBookingData?.BookedProducts?.FirstOrDefault(x =>
                    x.AvailabilityReferenceId == firstSelectedProduct.AvailabilityReferenceId);
                var independentBookablePaxCount = bookedProduct?.PassengerDetails?.Count(e => e.IndependablePax);
                var referenceIds = booking.IsangoBookingData?.BookedProducts?.Select(x => x.AvailabilityReferenceId)?.ToArray();
                var availabilityReferenceIds = string.Join(",", referenceIds);
                var criteria = new DuplicateBookingCriteria
                {
                    SmcPasswordId = 0,
                    TravelDate = productOption.TravelInfo.StartDate,
                    ServiceOptionId = productOption.Id,
                    AdultCount = independentBookablePaxCount ?? 0,
                    LeadPaxName = $"{leadPax?.FirstName} {leadPax?.LastName}",
                    AffiliateId = booking.Affiliate.Id,
                    UserEmailId = booking.User.EmailAddress,
                    AvailabilityReferenceIds = availabilityReferenceIds
                };
                return _bookingPersistence.CheckDuplicateBooking(criteria);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CheckDuplicateBooking",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// method to process the booking details
        /// ProcessBooking and process enroll booking are merged
        /// and will be handled using flag isEnrollBooking
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private Tuple<bool, bool> CreatePrepaidBooking(Booking booking, bool isEnrollBooking, string token)
        {
            try
            {
                var isBookingSuccess = false;
                var isPartialBooking = false;
                var isValidPrepaidBooking = booking.Amount <= 0 || IsAffiliateValidForPrepaidBooking(ref booking);
                if (isValidPrepaidBooking)
                {
                    var createBookingResponse = CreateBooking(booking, isEnrollBooking, token);
                    isBookingSuccess = createBookingResponse.Item1;
                    isPartialBooking = createBookingResponse.Item2;
                }
                else
                {
                    booking.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.BadRequest, Constant.InvalidPrepaidBooking);
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                        booking.UpdateDBLogFlag();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                }

                return new Tuple<bool, bool>(isBookingSuccess, isPartialBooking);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreatePrepaidBooking",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private ReservationResponse CreatePrepaidReservation(Booking booking, bool isEnrollBooking, string token)
        {
            try
            {
                var createBookingResponse = new ReservationResponse()
                {
                    Success = false,
                    Products = new List<BookedProductStatus>()
                };
                var isValidPrepaidBooking = booking.Amount <= 0 || booking.PaymentMethodType == PaymentMethodType.Transaction || IsAffiliateValidForPrepaidBooking(ref booking);
                if (isValidPrepaidBooking)
                {
                    createBookingResponse = CreateReservation(booking, isEnrollBooking, token);
                }
                else
                {
                    booking.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.BadRequest, "Invalid PrepaidBooking");
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token, string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber, Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId, booking.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                        booking.UpdateDBLogFlag();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                }

                return createBookingResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreatePrepaidBooking",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private bool IsAffiliateValidForPrepaidBooking(ref Booking booking)
        {
            try
            {
                decimal totalOnRequestBooking = 0;
                decimal totalConfirmedBooking = 0;
                var isValidPrepaidBooking = false;

                foreach (var selectedProduct in booking.SelectedProducts)
                {
                    var productOptions = selectedProduct.ProductOptions.Where(x => x.IsSelected).ToList();

                    foreach (var option in productOptions)
                    {
                        if (option.AvailabilityStatus.Equals(AvailabilityStatus.ONREQUEST))
                        {
                            totalOnRequestBooking +=
                                Math.Round(
                                    option.SellPrice.Amount -
                                    (selectedProduct.DiscountedPrice + selectedProduct.MultisaveDiscountedPrice), 2);
                        }
                        else
                        {
                            totalConfirmedBooking +=
                                Math.Round(
                                    option.SellPrice.Amount -
                                    (selectedProduct.DiscountedPrice + selectedProduct.MultisaveDiscountedPrice), 2);
                        }
                    }
                }

                var totalBookingCost = Math.Round(totalOnRequestBooking + totalConfirmedBooking, 2);
                // Step 1: Make sure above amount is in GBP to make comparisons with Affiliate credit details
                var conversionFactor = _masterService
                    .GetConversionFactorAsync(
                        booking.SelectedProducts.FirstOrDefault()?.ProductOptions.FirstOrDefault()?.SellPrice.Currency
                            .IsoCode, "GBP").Result;

                var totalBookingCostInGbp = totalBookingCost * conversionFactor;

                var affiliateAvailableCredit = booking.Affiliate.AffiliateCredit.AvailableCredit;

                if (affiliateAvailableCredit + booking.Affiliate.AffiliateCredit.OverdraftAmount >= totalBookingCostInGbp ||
                    booking.Affiliate.AffiliateCredit.CanBreachLimit || affiliateAvailableCredit >= totalBookingCostInGbp)
                {
                    isValidPrepaidBooking = true;
                }

                #region Emails

                var remainingAvailableCredit = affiliateAvailableCredit +
                                               booking.Affiliate.AffiliateCredit.OverdraftAmount - totalBookingCostInGbp;

                if (remainingAvailableCredit <= booking.Affiliate.AffiliateCredit.ThresholdAmount)
                {
                    //Send alert mail to affiliate
                    _mailerService.SendAlertMail(booking.Affiliate);
                }

                #endregion Emails

                return isValidPrepaidBooking;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "IsAffiliateValidForPrepaidBooking",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method to create booking
        /// Create booking and CreateEnrollBooking are merged and handled using is3D check
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="isEnrollmentCheck"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private Tuple<bool, bool, AuthorizationResponse> CreateBooking(Booking booking, bool isEnrollmentCheck, string token)
        {
            var isAdyen = false;
            var riskifiedEnabled = _isRiskifiedEnabled && booking.isRiskifiedEnabled;
            var isBookingSuccess = false;
            var isPartialBooking = false;
            var isPaymentAuthorizationSucceed = false;
            var authentication = string.Empty;// GetAuthString(booking.Affiliate.Id);
            IPaymentService paymentGateway = null;
            var authorizationResponse = new AuthorizationResponse();
            if (booking.PaymentMethodType == PaymentMethodType.Transaction
                && booking.Amount > 0
            )
            {
                // Get Payment Gateway
                paymentGateway = _paymentGatewayFactory.GetPaymentGatewayService(booking.PaymentGateway);

                // Calculate total amount of OR and FS products
                authorizationResponse.IsWebhookReceived = true;
                authorizationResponse = paymentGateway.Authorization(booking, isEnrollmentCheck, token);
                isPaymentAuthorizationSucceed = authorizationResponse.IsSuccess;
                //authorizationResponse.IsWebhookReceived
                if (!authorizationResponse.IsSuccess)
                {
                    try
                    {
                        //this means we are not retrying the athentication
                        if (string.IsNullOrEmpty(authorizationResponse.AcsRequest) && string.IsNullOrEmpty(authorizationResponse.TransactionID))
                        {
                            LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                            /*booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId*/ string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber,
                            Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.PaymentGatewayError.ToString());
                            //booking?.UpdateDBLogFlag();
                        }
                    }
                    catch (Exception e)
                    {
                        //ignore
                    }

                    if (riskifiedEnabled)
                    {
                        var riskifiedCheckoutDeniedResult = _riskifiedService.CheckoutDenied(booking,
                            new AuthorizationError()
                            {
                                CreatedAt = DateTime.Now,
                                ErrorCode = authorizationResponse.ErrorCode,
                                Message = authorizationResponse.ErrorMessage
                            }
                            , token);
                    }
                    else if (!string.IsNullOrEmpty(authorizationResponse.AcsRequest) && !string.IsNullOrEmpty(authorizationResponse.TransactionID))
                    {
                        //try
                        //{
                        //    if (string.IsNullOrEmpty(authorizationResponse.AcsRequest) && string.IsNullOrEmpty(authorizationResponse.TransactionID))
                        //    {
                        //        LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                        //        /*booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId*/ string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber,
                        //        Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        //        booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.PaymentGatewayError.ToString());
                        //        //booking?.UpdateDBLogFlag();
                        //    }
                        //}
                        //catch (Exception e)
                        //{
                        //    //ignore
                        //}
                        return new Tuple<bool, bool, AuthorizationResponse>(isBookingSuccess, isPartialBooking, authorizationResponse);
                    }
                    else
                    {
                        try
                        {
                            if (!authorizationResponse.IsWebhookReceived)
                            {
                                try
                                {
                                    paymentGateway.CancelAuthorization(booking, token);
                                    paymentGateway.RefundCapture(booking, "Booking failed", token);
                                    booking?.UpdateErrors(CommonErrorCodes.NoWebhookError
                                    , System.Net.HttpStatusCode.BadGateway
                                    , "Webhook Not Received!");
                                }
                                catch (Exception ex)
                                {
                                    //ignore
                                }
                            }
                            //this means we are not retrying the athentication
                            if (string.IsNullOrEmpty(authorizationResponse.AcsRequest) && string.IsNullOrEmpty(authorizationResponse.TransactionID))
                            {
                                LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                                /*booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId*/ string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber,
                                Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.PaymentGatewayError.ToString());
                                //booking?.UpdateDBLogFlag();
                            }
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }
                        return new Tuple<bool, bool, AuthorizationResponse>(isBookingSuccess, isPartialBooking, authorizationResponse);
                    }
                }
            }
            booking = CreatePaymentForORAndFS(booking, false, authorizationResponse.IsangoReference, authorizationResponse.Token);

            // This dictionary maintain mapping of SelectedProduct and its PK id from table.
            // this will be helpful while updating the booking status for selected product
            var selectedProductPartialBookingItemIdMapping = new Dictionary<string, int>();
            try
            {
                bool isTransactionSuccess;
                if (booking.ApiBookingIds == null)
                {
                    booking.ApiBookingIds = new Dictionary<APIType, string>();
                }

                var bookedProducts = new List<BookedProduct>();
                var isangoProductReferenceIds = booking.SelectedProducts.Where(x => x.APIType == APIType.Undefined)
                    .Select(x => x.AvailabilityReferenceId);

                if (isangoProductReferenceIds?.Any() == true)
                {
                    var products = booking?.IsangoBookingData?.BookedProducts?.Where(product =>
                           isangoProductReferenceIds.Contains(product.AvailabilityReferenceId)
                       )?.ToList();

                    if (products?.Any() == true)
                    {
                        bookedProducts.AddRange(products);
                    }
                }

                #region Supplier Booking

                var supplierProductCount = booking.SelectedProducts.Count(x => x.APIType != APIType.Undefined);
                if (supplierProductCount > 0)
                {
                    var isAnyCancellableSupplierProductFailed = false;
                    var isAnyNonCancellableSupplierProductFailed = false;

                    var criteria = new ActivityBookingCriteria
                    {
                        Booking = booking,
                        Authentication = authentication,
                        Token = token
                    };
                    var canocalizationCriteria = new CanocalizationActivityBookingCriteria
                    {
                        Booking = booking,
                        Authentication = authentication,
                        Token = token
                    };

                    // Call to Supplier booking.
                    try
                    {
                        var bookedProductsFromSupplier = CancellableSupplierBooking(criteria, canocalizationCriteria);
                        if (bookedProductsFromSupplier?.Count > 0)
                        {
                            bookedProducts.AddRange(bookedProductsFromSupplier);
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingService",
                            MethodName = "CreateBooking",
                            Token = token,
                            AffiliateId = booking.Affiliate.Id,
                            Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                        };

                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , ex.Message);
                        _log.Error(isangoErrorEntity, ex);
                        try
                        {
                            LogBookingFailureInDB(booking, booking?.ReferenceNumber, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                                /*_failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.AvailabilityReferenceId*/ string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber,
                                Convert.ToInt32(_failedSupplierProducts?.FirstOrDefault().Key ?? 0), _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            var isangoErrorEntityDB = new IsangoErrorEntity
                            {
                                ClassName = "BookingService",
                                MethodName = "CreateBooking",
                                Token = token,
                                AffiliateId = booking.Affiliate.Id,
                                Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                            };
                            _log.Error(isangoErrorEntityDB, e);
                        }
                        isAnyCancellableSupplierProductFailed = true;
                    }

                    if (!booking.Affiliate.AffiliateConfiguration.IsPartialBookingSupport)
                    {
                        List<string> notProcessedProductIds = null;

                        if (!isAnyCancellableSupplierProductFailed)
                        {
                            isAnyCancellableSupplierProductFailed =
                                _failedSupplierProducts.Values.Any(x => x.Count > 0);
                        }

                        //Check if any cancellable supplier product failed
                        if (!isAnyCancellableSupplierProductFailed)
                        {
                            try
                            {
                                var result = NonCancellableSupplierBooking(criteria);
                                var nonCancellableBookedProducts = result.Item1;
                                notProcessedProductIds = result.Item2;
                                if (nonCancellableBookedProducts?.Count > 0)
                                {
                                    bookedProducts.AddRange(nonCancellableBookedProducts);
                                }
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "BookingService",
                                    MethodName = "CreateBooking",
                                    Token = token,
                                    AffiliateId = booking.Affiliate.Id,
                                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                                };
                                _log.Error(isangoErrorEntity, ex);
                                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , ex.Message);

                                try
                                {
                                    LogBookingFailureInDB(booking, booking?.ReferenceNumber, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                                        /*_failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.AvailabilityReferenceId*/ string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber,
                                        Convert.ToInt32(_failedSupplierProducts?.FirstOrDefault().Key ?? 0), _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                        _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                                    criteria?.Booking?.UpdateDBLogFlag();
                                }
                                catch (Exception e)
                                {
                                    var isangoErrorEntityDB = new IsangoErrorEntity
                                    {
                                        ClassName = "BookingService",
                                        MethodName = "CreateBooking",
                                        Token = token,
                                        AffiliateId = booking.Affiliate.Id,
                                        Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                                    };
                                    _log.Error(isangoErrorEntityDB, ex);
                                    //ignore
                                }


                                isAnyNonCancellableSupplierProductFailed = true;
                            }

                            if (!isAnyNonCancellableSupplierProductFailed)
                            {
                                isAnyNonCancellableSupplierProductFailed =
                                    _failedSupplierProducts.Values.Any(x => x.Count > 0);
                            }
                        }
                        else
                        {
                            notProcessedProductIds = GetNonCancellableSelectedProducts(booking.SelectedProducts).Values
                                .SelectMany(x => x).Select(e => e.AvailabilityReferenceId).ToList();
                        }

                        if (isAnyCancellableSupplierProductFailed || isAnyNonCancellableSupplierProductFailed)
                        {
                            //booking.SelectedProducts = GetBookedSelectedProduct(booking);
                            var selectedProducts = GetBookedSelectedProduct(booking);
                            if (booking?.SelectedProducts?.Count > 0
                            ) // if not a single product is booked then no need to call cancel booking.
                            {
                                SupplierBookingCancellation(booking, authentication, token,
                                    notProcessedProductIds);

                                if (isPaymentAuthorizationSucceed)
                                {
                                    if (!string.IsNullOrEmpty(booking?.FallbackFingerPrint) && (booking?.FallbackFingerPrint?.ToLower() == "sofort" || booking?.FallbackFingerPrint?.ToLower() == "ideal"))
                                    {
                                        paymentGateway.RefundCapture(booking, "Booking failed", token);
                                    }
                                    else
                                    {
                                        paymentGateway.CancelAuthorization(booking, token);
                                    }
                                    if (riskifiedEnabled)
                                    {
                                        var riskifiedFullRefundResult = _riskifiedService.FullRefund(booking.ReferenceNumber, token);
                                    }
                                }

                                return new Tuple<bool, bool, AuthorizationResponse>(isBookingSuccess, isPartialBooking, authorizationResponse);
                            }
                        }
                    }
                    else
                    {
                        var result = NonCancellableSupplierBooking(criteria);
                        var nonCancellableBookedProducts = result.Item1;
                        bookedProducts.AddRange(nonCancellableBookedProducts);
                    }
                }

                var failedSupplierProductsCount = _failedSupplierProducts.Values.Sum(x => x.Count);
                var iSangoProducts = booking.SelectedProducts.Where(x => x.APIType == APIType.Undefined);

                if (supplierProductCount == failedSupplierProductsCount && !iSangoProducts.Any())
                {
                    if (isPaymentAuthorizationSucceed)
                    {
                        if (!string.IsNullOrEmpty(booking?.FallbackFingerPrint) && booking?.FallbackFingerPrint?.ToLower() == "sofort")
                        {
                            paymentGateway.RefundCapture(booking, "Booking failed", token);
                        }
                        else
                        {
                            paymentGateway.CancelAuthorization(booking, token);
                        }
                        if (riskifiedEnabled)
                        {
                            var riskifiedFullRefundResult = _riskifiedService.FullRefund(booking.ReferenceNumber, token);
                        }
                        return new Tuple<bool, bool, AuthorizationResponse>(isBookingSuccess, isPartialBooking, authorizationResponse);
                    }
                }

                #endregion Supplier Booking

                // Insert booking details in new table - call persistence to insert table.
                InsertIntoPartialBookingItem(booking, ref selectedProductPartialBookingItemIdMapping);
                var isTransactionRequired = false;

                if (booking.PaymentMethodType == PaymentMethodType.Transaction)
                {
                    if (booking.Amount <= 0)
                    {
                        isTransactionSuccess = true;
                    }
                    else
                    {
                        booking.Payment.RemoveAll(x => x.PaymentStatus.Equals(PaymentStatus.Paid));
                        //Need to calculate PreAuth and Purchase object amount again after supplier booking
                        var paidPayment = CreatePaymentForFS(booking);
                        if (paidPayment != null)
                        {
                            isTransactionRequired = true;
                            booking.Payment.Add(paidPayment);
                            // ReSharper disable once PossibleNullReferenceException
                            var transactionResponse = paymentGateway.Transaction(booking, isEnrollmentCheck, token);
                            isAdyen = transactionResponse.IsAdyen ?? false;
                            authorizationResponse.IsWebhookReceived = transactionResponse.IsWebHookRecieved ?? false;
                            if (!transactionResponse.IsSuccess)
                            {
                                try
                                {
                                    LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                                        /*booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId*/ string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber,
                                        Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                        booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.PaymentGatewayError.ToString());
                                    booking?.UpdateDBLogFlag();
                                }
                                catch (Exception e)
                                {
                                    var isangoErrorEntityDBB = new IsangoErrorEntity
                                    {
                                        ClassName = "BookingService",
                                        MethodName = "CreateBooking",
                                        Token = token,
                                        AffiliateId = booking.Affiliate.Id,
                                        Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                                    };
                                    _log.Error(isangoErrorEntityDBB, e);
                                    //ignore
                                }

                                if (riskifiedEnabled)
                                {
                                    var riskifiedCheckoutDeniedResult = _riskifiedService.CheckoutDenied(booking,
                                        new AuthorizationError()
                                        {
                                            CreatedAt = DateTime.Now,
                                            ErrorCode = transactionResponse.ErrorCode,
                                            Message = transactionResponse.ErrorMessage
                                        }
                                        , token);
                                }
                            }
                            isTransactionSuccess = transactionResponse.IsSuccess;
                        }
                        else
                        {
                            isTransactionSuccess = true;
                        }
                    }
                }
                else
                {
                    isTransactionSuccess = true;
                }

                // Based on status of method from wirecard adapter

                // if status is Success
                // Call to isango booking SP.
                // Update booking status in partial booking item table - call persistence to update table
                //in case of wirecard transaction, wirecard transaction status will decide whether to create isango booking.
                if (booking.PaymentMethodType != PaymentMethodType.Transaction || !isTransactionRequired ||
                    isTransactionSuccess && bookedProducts?.Count > 0)
                {
                    var isangoBookingData = MapIsangoBookingData(booking);
                    isangoBookingData.TransactionDetail?.ForEach(x => x.Is3DSecure = isEnrollmentCheck);
                    isangoBookingData.BookedProducts = bookedProducts;

                    _log.Info(
                        $"BookingService|FinalIsangoBookingData|{SerializeDeSerializeHelper.Serialize(isangoBookingData)}");
                    isBookingSuccess = CreateIsangoBookingInDB(isangoBookingData, false);

                    if (isBookingSuccess)
                    {
                        if (_failedSupplierProducts.Values.Any(x => x.Count > 0))
                        {
                            isPartialBooking = true;
                        }

                        var attachment = MoulinRougeAttachment(booking, bookedProducts);
                        AfterBookingProcess(booking, selectedProductPartialBookingItemIdMapping, attachment, false);
                    }
                    else
                    {
                        var dbErrors = isangoBookingData?.BookedProducts?.FirstOrDefault()?.Errors;
                        if (dbErrors?.Any() == true)
                        {
                            if (booking.Errors == null)
                            {
                                booking.Errors = new List<Error>();
                            }
                            booking.Errors.AddRange(dbErrors);
                        }
                    }
                }

                // if status is failed
                // Call to api for cancellation
                // Update booking status to cancelled in partial booking item table - call persistence to update table
                if (!isTransactionSuccess || !isBookingSuccess)
                {
                    try
                    {
                        LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                            /*booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId*/ string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber,
                            Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                            booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, $"{CommonErrorCodes.PaymentGatewayError} OR {CommonErrorCodes.SupplierBookingError.ToString()}");
                        booking?.UpdateDBLogFlag();
                    }
                    catch (Exception e)
                    {
                        var isangoErrorEntityF = new IsangoErrorEntity
                        {
                            ClassName = "BookingService",
                            MethodName = "CreateBooking",
                            Token = token,
                            AffiliateId = booking.Affiliate.Id,
                            Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                        };
                        _log.Error(isangoErrorEntityF, e);
                        //ignore
                    }
                    //if iSango booking failed and capture transaction succeed
                    if (isTransactionRequired && isTransactionSuccess)
                    {
                        if (booking.PaymentGateway == PaymentGatewayType.Apexx)
                        {
                            var purchasePayment = booking?.Payment.FirstOrDefault(x => x.PaymentStatus.Equals(PaymentStatus.Paid));
                            string captureId = purchasePayment?.PaymentGatewayReferenceId;
                            string captureReference = purchasePayment?.TransactionId;
                            paymentGateway.CancelCapture(booking, captureId, captureReference, token);
                        }
                        else
                        {
                            paymentGateway.RefundCapture(booking, "Booking failed", token);
                        }
                    }
                    else
                    {
                        if (isPaymentAuthorizationSucceed)
                        {
                            if (!string.IsNullOrEmpty(booking?.FallbackFingerPrint) && (booking?.FallbackFingerPrint?.ToLower() == "sofort" || booking?.FallbackFingerPrint?.ToLower() == "ideal"))
                            {
                                paymentGateway.RefundCapture(booking, "Booking failed", token);
                            }
                            else
                            {
                                paymentGateway.CancelAuthorization(booking, token);
                            }
                        }
                    }

                    //Get all Products as we need them for updating Partial Booking item table
                    var allSelectedProducts = booking.SelectedProducts;

                    if (booking.SelectedProducts.Any(x => x.APIType != APIType.Undefined))
                    {
                        booking.SelectedProducts = GetBookedSelectedProduct(booking);
                        SupplierBookingCancellation(booking, authentication, token, new List<string>());
                    }

                    booking.SelectedProducts = allSelectedProducts;
                    UpdatePartialBookingItem(booking, selectedProductPartialBookingItemIdMapping,
                        PartialBookingStatus.Cancelled); // changed booked product status to Cancelled
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreateBooking",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , ex.Message);

                try
                {
                    LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                        /*booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId*/ string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber,
                        Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                    booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return new Tuple<bool, bool, AuthorizationResponse>(isBookingSuccess, isPartialBooking, authorizationResponse);
        }

        private ReservationResponse CreateReservation(Booking booking, bool isEnrollmentCheck, string token)
        {
            var isBookingSuccess = false;
            var isPartialBooking = false;
            var authentication = string.Empty;// GetAuthString(booking.Affiliate.Id);
            var reservationResponse = new ReservationResponse()
            {
                Products = new List<BookedProductStatus>(),
                Success = false
            };

            // This dictionary maintain mapping of SelectedProduct and its PK id from table.
            // this will be helpful while updating the booking status for selected product
            var selectedProductPartialBookingItemIdMapping = new Dictionary<string, int>();
            try
            {
                if (booking.ApiBookingIds == null)
                {
                    booking.ApiBookingIds = new Dictionary<APIType, string>();
                }

                var bookedProducts = new List<BookedProduct>();
                var isangoProductReferenceIds = booking.SelectedProducts.Where(x => x.APIType == APIType.Undefined)
                    .Select(x => x.AvailabilityReferenceId);

                if (isangoProductReferenceIds?.Any() == true)
                {
                    var products = booking?.IsangoBookingData?.BookedProducts?.Where(product =>
                           isangoProductReferenceIds.Contains(product.AvailabilityReferenceId)
                       )?.ToList();
                    products.ForEach(x => x.OptionStatus = "Not Supported");

                    if (products?.Any() == true)
                    {
                        bookedProducts.AddRange(products);
                    }
                    reservationResponse.Success = true;
                }

                #region Supplier Booking

                var supplierProductCount = booking.SelectedProducts.Count(x => x.APIType != APIType.Undefined);
                if (supplierProductCount > 0)
                {
                    var isAnyCancellableSupplierProductFailed = false;

                    var criteria = new ActivityBookingCriteria
                    {
                        Booking = booking,
                        Authentication = authentication,
                        Token = token
                    };

                    // Call to Supplier booking.
                    try
                    {
                        var bookedProductsFromSupplier = CancellableSupplierReservation(criteria);
                        if (bookedProductsFromSupplier != null && bookedProductsFromSupplier?.Count > 0)
                        {
                            bookedProducts.AddRange(bookedProductsFromSupplier);
                        }
                        else
                        {
                            reservationResponse.Success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingService",
                            MethodName = "CreateBooking",
                            Token = token,
                            AffiliateId = booking.Affiliate.Id,
                            Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                        };

                        criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , ex.Message);
                        try
                        {
                            LogBookingFailureInDB(booking, booking?.ReferenceNumber, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.Id ?? booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                                /*_failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.AvailabilityReferenceId*/ string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber,
                                Convert.ToInt32(_failedSupplierProducts?.FirstOrDefault().Key ?? 0), _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                                _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.AvailabilityReferenceId ?? booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                            criteria?.Booking?.UpdateDBLogFlag();
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }

                        _log.Error(isangoErrorEntity, ex);
                        isAnyCancellableSupplierProductFailed = true;
                    }

                    if (!booking.Affiliate.AffiliateConfiguration.IsPartialBookingSupport)
                    {
                        List<string> notProcessedProductIds = null;

                        if (!isAnyCancellableSupplierProductFailed)
                        {
                            isAnyCancellableSupplierProductFailed =
                                _failedSupplierProducts.Values.Any(x => x.Count > 0);
                        }

                        notProcessedProductIds = GetNonCancellableSelectedProducts(booking.SelectedProducts).Values
                            .SelectMany(x => x).Select(e => e.AvailabilityReferenceId).ToList();

                        if (isAnyCancellableSupplierProductFailed)
                        {
                            //booking.SelectedProducts = GetBookedSelectedProduct(booking);
                            var selectedProducts = GetBookedSelectedProduct(booking);
                            if (booking?.SelectedProducts?.Count > 0
                            ) // if not a single product is booked then no need to call cancel booking.
                            {
                                SupplierBookingCancellation(booking, authentication, token,
                                    notProcessedProductIds);

                                foreach (var item in bookedProducts)
                                {
                                    var product = new BookedProductStatus()
                                    {
                                        AvailabilityReferenceID = item.AvailabilityReferenceId,
                                        Status = item.OptionStatus
                                    };

                                    reservationResponse.Products.Add(product);
                                }

                                return reservationResponse;
                            }
                        }
                        else
                        {
                            foreach (var item in bookedProducts)
                            {
                                var product = new BookedProductStatus()
                                {
                                    AvailabilityReferenceID = item.AvailabilityReferenceId,
                                    Status = item.OptionStatus
                                };

                                reservationResponse.Products.Add(product);
                                reservationResponse.Success = true;
                                reservationResponse.BookingReferenceNumber = booking?.ReferenceNumber;
                            }
                        }
                    }
                }

                var failedSupplierProductsCount = _failedSupplierProducts.Values.Sum(x => x.Count);
                var iSangoProducts = booking.SelectedProducts.Where(x => x.APIType == APIType.Undefined);

                if (supplierProductCount == failedSupplierProductsCount && !iSangoProducts.Any())
                {
                    foreach (var item in bookedProducts)
                    {
                        var product = new BookedProductStatus()
                        {
                            AvailabilityReferenceID = item.AvailabilityReferenceId,
                            Status = item.OptionStatus
                        };

                        reservationResponse.Products.Add(product);
                    }
                    return reservationResponse;
                }

                #endregion Supplier Booking
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreateBooking",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };

                booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , ex.Message);

                try
                {
                    LogBookingFailureInDB(booking, booking?.ReferenceNumber, booking?.SelectedProducts?.FirstOrDefault()?.Id, token,
                        /*booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId*/ string.Empty, booking.User.EmailAddress, booking.User.PhoneNumber,
                        Convert.ToInt32(booking?.SelectedProducts?.FirstOrDefault()?.APIType ?? 0), booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.BookingError.ToString());
                    booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return reservationResponse;
        }

        private void AfterBookingProcess(Booking booking,
            Dictionary<string, int> selectedProductPartialBookingItemIdMapping = null,
            List<Attachment> attachment = null, bool? isAlternativePayment = false, bool? isAdyen = false)
        {
            try
            {
                if (isAdyen != null && isAdyen == false)
                {
                    //Send mail to customer and supplier on successful booking
                    _mailerService.SendMail(booking.ReferenceNumber, attachment, isAlternativePayment);
                    _mailerService.SendSupplierMail(booking.ReferenceNumber);
                }

                var anySupplierProducts = booking.SelectedProducts.Any(x => x.APIType != APIType.Undefined);

                if (!anySupplierProducts) return;

                var remainingAmountRows = new StringBuilder();
                if (selectedProductPartialBookingItemIdMapping != null)
                    UpdatePartialBookingItem(booking, selectedProductPartialBookingItemIdMapping,
                        PartialBookingStatus.Booked); // changed booked product status to Booked

                foreach (var item in booking.SelectedProducts.Where(x => x.APIType != APIType.Undefined))
                {
                    if (_failedSupplierProducts[item.APIType].Any(x => x.Id == item.Id) && item.DiscountedPrice > 0)
                    {
                        foreach (var discount in item.AppliedDiscountCoupons)
                        {
                            remainingAmountRows.Append(
                                $"<tr><td>{item.Id}</td><td>{discount.Code}</td><td>{discount.Price}</td><td>{booking.Currency.IsoCode}</td></tr>");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(remainingAmountRows.ToString()))
                {
                    //Send mail to customer support team
                    _mailerService.SendRemainingDiscountAmountMail(remainingAmountRows.ToString());
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "AfterBookingProcess",
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
        }

        private bool CreateIsangoBookingInDB(IsangoBookingData isangoBooking, bool isAlternativePayment)
        {
            try
            {
                return _bookingPersistence.CreateIsangoBooking(isangoBooking, isAlternativePayment);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreateIsangoBookingInDB",
                    AffiliateId = isangoBooking.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(isangoBooking)}"
                };
                var bp = isangoBooking?.BookedProducts?.FirstOrDefault();
                bp.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.InternalServerError, ex.Message + ex.StackTrace);
                try
                {
                    LogBookingFailureInDB(null, isangoBooking?.BookingReferenceNumber, bp.ServiceId, string.Empty,
                        string.Empty, isangoBooking?.CustomerEmailId, string.Empty,
                        0, bp.OptionId,
                        bp.OptionName,
                        bp.AvailabilityReferenceId, CommonErrorCodes.DBException.ToString());
                    bp.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }
                _log.Error(isangoErrorEntity, ex);
                return false;
            }
        }

        private Dictionary<string, bool> SupplierBookingCancellation(Booking booking, string authentication,
            string token, List<string> notProcessedProductIds, bool isBookingManager = false)
        {
            var cancelStatusWithReferenceIds = new Dictionary<string, bool>();
            try
            {
                var taskArrayCount = 0;

                var cancellableProducts = GetCancellableSelectedProducts(booking.SelectedProducts);
                var nonCancellableSelectedProductsByAPI = GetNonCancellableSelectedProducts(booking.SelectedProducts);
                cancelStatusWithReferenceIds = nonCancellableSelectedProductsByAPI.Values.SelectMany(x => x)
                    .Where(e => !notProcessedProductIds.Contains(e.AvailabilityReferenceId))
                    .ToDictionary(x => x.AvailabilityReferenceId, y => false);

                var validCancellableProducts = cancellableProducts.Where(e => e.Value.Count != 0).ToList();

                var taskArray = new Task<Dictionary<string, bool>>[validCancellableProducts.Count];

                foreach (var validCancellableProduct in validCancellableProducts)
                {
                    try
                    {
                        if (validCancellableProduct.Value.Count != 0)
                        {
                            switch (validCancellableProduct.Key)
                            {
                                case APIType.Hotelbeds:
                                    var hotelBedsSelectedProducts = validCancellableProduct.Value
                                        .Cast<HotelBedsSelectedProduct>().ToList();

                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.TicketAdapterPurchaseCancel(hotelBedsSelectedProducts,
                                            booking?.Language?.Code, booking.ReferenceNumber, token));
                                    break;

                                case APIType.Graylineiceland:
                                    var gliSelectedProducts = validCancellableProduct.Value;
                                    var gliAvailabilityReferenceIds =
                                        gliSelectedProducts.Select(x => x.AvailabilityReferenceId);
                                    var supplierReferenceNumber = booking.IsangoBookingData.BookedProducts
                                        .FirstOrDefault(e =>
                                            e.OptionStatus != "0" &&
                                            gliAvailabilityReferenceIds.Contains(e.AvailabilityReferenceId))
                                        ?.APIExtraDetail.SupplieReferenceNumber;

                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.GrayLineIceLandDeleteBooking(gliSelectedProducts,
                                            supplierReferenceNumber, booking.ReferenceNumber, token));
                                    break;

                                case APIType.Prio:
                                    var prioSelectedProducts = validCancellableProduct.Value.Cast<PrioSelectedProduct>()
                                        .ToList();
                                    var referenceNumber = booking.ReferenceNumber;
                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.PrioCancelReservationAndBooking(prioSelectedProducts,
                                            referenceNumber, token));
                                    break;

                                case APIType.Ventrata:
                                    var ventrataSelectedProducts = validCancellableProduct.Value.Cast<VentrataSelectedProduct>()
                                        .ToList();
                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.VentrataCancelReservationAndBooking(ventrataSelectedProducts,
                                            booking.ReferenceNumber, token, booking));
                                    break;

                                case APIType.Tiqets:
                                    var tiqetsSelectedProducts = validCancellableProduct.Value.Cast<TiqetsSelectedProduct>()
                                        .ToList();
                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.CancelTiqetsBooking(tiqetsSelectedProducts,
                                            booking.ReferenceNumber, token, booking.Language.Code, booking?.Affiliate?.Id));
                                    break;

                                case APIType.Fareharbor:
                                    var fareHarborSelectedProducts = validCancellableProduct.Value
                                        .Cast<FareHarborSelectedProduct>().ToList();

                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.FareharborDeleteBooking(fareHarborSelectedProducts,
                                            booking.ReferenceNumber, token));
                                    break;

                                case APIType.Aot:
                                    var aotSelectedProducts = validCancellableProduct.Value;
                                    var aotAvailabilityReferenceIds =
                                        aotSelectedProducts.Select(x => x.AvailabilityReferenceId);
                                    var aotBookedProducts = booking.IsangoBookingData.BookedProducts
                                        .Where(e =>
                                            e.OptionStatus != "0" &&
                                            aotAvailabilityReferenceIds.Contains(e.AvailabilityReferenceId)).ToList();

                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.CancelAotBooking(aotBookedProducts,
                                            booking.ReferenceNumber, token));
                                    break;

                                case APIType.Bokun:
                                    var bokunSelectedProducts = validCancellableProduct.Value;
                                    var bokunAvailabilityReferenceIds =
                                        bokunSelectedProducts.Select(x => x.AvailabilityReferenceId);
                                    var confirmationCode = booking.IsangoBookingData.BookedProducts
                                        .FirstOrDefault(e =>
                                            e.OptionStatus != "0" &&
                                            bokunAvailabilityReferenceIds.Contains(e.AvailabilityReferenceId))
                                        ?.APIExtraDetail.SupplieReferenceNumber;

                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.CancelBokunBooking(bokunSelectedProducts,
                                            confirmationCode, booking.ReferenceNumber, token));
                                    break;

                                case APIType.BigBus:
                                    var selectedProducts = validCancellableProduct.Value;
                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.CancelBigBusBooking(selectedProducts, token));
                                    break;

                                case APIType.Citysightseeing:
                                    var sightSeeingSelectedProducts = validCancellableProduct.Value;
                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.CancelSightSeeingBooking(sightSeeingSelectedProducts,
                                            token));
                                    break;

                                case APIType.Redeam:
                                    {
                                        var redeamSelectedProducts = validCancellableProduct.Value
                                            .Where(x => x.APIType == APIType.Redeam).ToList()
                                            //.FindAll(product => ((ActivityOption)product.ProductOptions?.FirstOrDefault(x => x.IsSelected)).Cancellable)
                                            .ToList();
                                        taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                            _supplierBookingService.CancelRedeamBooking(redeamSelectedProducts, token));
                                        break;
                                    }
                                case APIType.Rezdy:
                                    {
                                        var rezdySelectedProducts = validCancellableProduct.Value;
                                        taskArray[taskArrayCount] = Task.Factory.StartNew(() => _supplierBookingService.CancelRezdyBooking(rezdySelectedProducts, booking.ReferenceNumber, token));
                                        break;
                                    }
                                case APIType.GlobalTix:
                                    {
                                        var globalTixSelectedProducts = validCancellableProduct.Value;
                                        taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                           _supplierBookingService.CancelGlobalTixBooking(globalTixSelectedProducts, token));
                                        break;
                                    }

                                case APIType.TourCMS:
                                    {
                                        var tourCMSSelectedProducts = validCancellableProduct.Value;
                                        taskArray[taskArrayCount] = Task.Factory.StartNew(() => _supplierBookingService.CancelTourCMSBooking(tourCMSSelectedProducts, booking.ReferenceNumber, token));
                                        break;
                                    }

                                case APIType.NewCitySightSeeing:
                                    {
                                        var newCitySightSeeingSelectedProducts = validCancellableProduct.Value;
                                        taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                           _supplierBookingService.CancelNewCitySightSeeingBooking(newCitySightSeeingSelectedProducts, booking.ReferenceNumber, token));
                                        break;
                                    }
                                case APIType.GoCity:
                                    {
                                        var goCitySelectedProducts = validCancellableProduct.Value;
                                        var customerEmail = ((GoCitySelectedProduct)goCitySelectedProducts.FirstOrDefault())?.CustomerEmail;

                                        taskArray[taskArrayCount] = Task.Factory.StartNew(() => _supplierBookingService.CancelGoCityBooking
                                        (goCitySelectedProducts, booking.ReferenceNumber, token,
                                         customerEmail));
                                        break;
                                    }
                                case APIType.PrioHub:
                                    var prioHubSelectedProducts = validCancellableProduct.Value.Cast<PrioHubSelectedProduct>()
                                        .ToList();
                                    var isangoRefNumber = booking.ReferenceNumber;
                                    var languageCode = booking?.Language?.Code?.ToLowerInvariant();
                                    var affiliateId = booking?.Affiliate?.Id;
                                    var email = booking?.VoucherEmailAddress;

                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.PrioHubCancelReservationAndBooking(prioHubSelectedProducts,
                                            isangoRefNumber, token, languageCode, affiliateId, email));
                                    break;

                                case APIType.Rayna:
                                    {
                                        var raynaSelectedProducts = validCancellableProduct.Value.Cast<RaynaSelectedProduct>().ToList();
                                        taskArray[taskArrayCount] = Task.Factory.StartNew(() => _supplierBookingService.CancelRaynaBooking
                                       (raynaSelectedProducts, booking.ReferenceNumber, token));
                                        break;
                                    }
                                case APIType.RedeamV12:
                                    {
                                        var redeamSelectedProducts = validCancellableProduct.Value
                                            .Where(x => x.APIType == APIType.RedeamV12).ToList()
                                             .ToList();
                                        taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                            _icanocalizationService.CancelBooking(redeamSelectedProducts, token, APIType.RedeamV12));
                                        break;
                                    }
                                case APIType.GlobalTixV3:
                                    {
                                        var globalTixV3SelectedProducts = validCancellableProduct.Value
                                            .Where(x => x.APIType == APIType.GlobalTixV3).ToList()
                                            .ToList();
                                        if (globalTixV3SelectedProducts != null && globalTixV3SelectedProducts.Count > 0)
                                        {
                                            foreach (var item in globalTixV3SelectedProducts)
                                            {
                                                ((CanocalizationSelectedProduct)item).BookingReferenceNumber = booking.ReferenceNumber;
                                            }
                                        }
                                        taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                            _icanocalizationService.CancelBooking(globalTixV3SelectedProducts, token, APIType.GlobalTixV3));
                                        break;
                                    }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingService",
                            MethodName = "SupplierBookingCancellation",
                            Token = token,
                            AffiliateId = booking.Affiliate.Id,
                            Params = $"{SerializeDeSerializeHelper.Serialize(validCancellableProduct)}"
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }

                    taskArrayCount += 1;
                }

                Task.WaitAll(taskArray);

                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                Parallel.ForEach(taskArray, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, task =>
                {
                    try
                    {
                        var data = task?.GetAwaiter().GetResult();
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Key) && !cancelStatusWithReferenceIds.Keys.Contains(item.Key))
                                {
                                    cancelStatusWithReferenceIds.Add(item.Key, item.Value);
                                }
                                else if (!string.IsNullOrWhiteSpace(item.Key) && cancelStatusWithReferenceIds.Keys.Contains(item.Key))
                                {
                                    cancelStatusWithReferenceIds[item.Key] = item.Value;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingService",
                            MethodName = "SupplierBookingCancellation",
                            Token = token,
                            AffiliateId = booking.Affiliate.Id,
                            Params = $"{SerializeDeSerializeHelper.Serialize(validCancellableProducts)}"
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }
                });
            }
            catch (Exception ae)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "SupplierBookingCancellation",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ae);
            }
            // Send failure mail to customer support
            if (isBookingManager == false)
            {
                SendFailureMail(cancelStatusWithReferenceIds, booking, token);
            }
            return cancelStatusWithReferenceIds;
        }

        private ReservationResponse SupplierReservationCancellation(Booking booking,
            string token, List<string> notProcessedProductIds)
        {
            var reservationResponse = new ReservationResponse()
            {
                Success = true,
                Products = new List<BookedProductStatus>(),
                BookingReferenceNumber = booking.ReferenceNumber,
                ExpirationTimeInMinutes = 30
            };
            foreach (var product in booking.SelectedProducts)
            {
                product.ProductOptions = new List<ProductOption>();
            }
            var cancelStatusWithReferenceIds = new Dictionary<string, bool>();
            try
            {
                var taskArrayCount = 0;

                var cancellableProducts = GetCancellableSelectedProducts(booking.SelectedProducts);
                var nonCancellableSelectedProductsByAPI = GetNonCancellableSelectedProducts(booking.SelectedProducts);
                cancelStatusWithReferenceIds = nonCancellableSelectedProductsByAPI.Values.SelectMany(x => x)
                    .Where(e => !notProcessedProductIds.Contains(e.AvailabilityReferenceId))
                    .ToDictionary(x => x.AvailabilityReferenceId, y => false);

                var validCancellableProducts = cancellableProducts.Where(e => e.Value.Count != 0).ToList();

                var taskArray = new Task<Dictionary<string, bool>>[validCancellableProducts.Count];

                foreach (var validCancellableProduct in validCancellableProducts)
                {
                    try
                    {
                        if (validCancellableProduct.Value.Count != 0)
                        {
                            switch (validCancellableProduct.Key)
                            {
                                case APIType.Tiqets:
                                    var tiqetsSelectedProducts = new List<TiqetsSelectedProduct>();
                                    foreach (var product in validCancellableProduct.Value)
                                    {
                                        var tiqetsSelectedProduct = new TiqetsSelectedProduct()
                                        {
                                            AvailabilityReferenceId = product.AvailabilityReferenceId
                                        };
                                        tiqetsSelectedProducts.Add(tiqetsSelectedProduct);
                                    }

                                    taskArray[taskArrayCount] = Task.Factory.StartNew(() =>
                                        _supplierBookingService.CancelTiqetsBooking(tiqetsSelectedProducts,
                                            booking.ReferenceNumber, token, booking.Language.Code));
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingService",
                            MethodName = "SupplierReservationCancellation",
                            Token = token,
                            AffiliateId = booking.Affiliate.Id,
                            Params = $"{SerializeDeSerializeHelper.Serialize(validCancellableProduct)}"
                        };
                        _log.Error(isangoErrorEntity, ex);
                        reservationResponse.Success = false;
                    }

                    taskArrayCount += 1;
                }

                Task.WaitAll(taskArray);

                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                Parallel.ForEach(taskArray, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, task =>
                {
                    try
                    {
                        var data = task?.GetAwaiter().GetResult();
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Key) &&
                                    !cancelStatusWithReferenceIds.Keys.Contains(item.Key))
                                    cancelStatusWithReferenceIds.Add(item.Key, item.Value);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingService",
                            MethodName = "SupplierReservationCancellation",
                            Token = token,
                            AffiliateId = booking.Affiliate.Id,
                            Params = $"{SerializeDeSerializeHelper.Serialize(validCancellableProducts)}"
                        };
                        _log.Error(isangoErrorEntity, ex);
                        reservationResponse.Success = false;
                    }
                });
            }
            catch (Exception ae)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "SupplierReservationCancellation",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ae);
            }
            foreach (var item in cancelStatusWithReferenceIds)
            {
                var productWithStatus = new BookedProductStatus()
                {
                    AvailabilityReferenceID = item.Key,
                    Status = item.Value ? "Success" : "NotBooked"
                };

                reservationResponse.Products.Add(productWithStatus);
            }

            return reservationResponse;
        }

        /// <summary>
        /// Send failure mail to customer support
        /// </summary>
        /// <param name="cancelStatusWithReferenceIds"></param>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        private void SendFailureMail(Dictionary<string, bool> cancelStatusWithReferenceIds, Booking booking,
            string token)
        {
            try
            {
                if (cancelStatusWithReferenceIds.Keys.Count > 0)
                {
                    var failureContextList = new List<Entities.Mailer.FailureMailContext>();
                    foreach (var referenceId in cancelStatusWithReferenceIds.Keys)
                    {
                        var bookedProduct =
                            booking.IsangoBookingData.BookedProducts.FirstOrDefault(x =>
                                x.AvailabilityReferenceId == referenceId);
                        var selectedProduct =
                            booking.SelectedProducts.FirstOrDefault(x => x.AvailabilityReferenceId == referenceId);

                        if (selectedProduct?.ProductOptions?.Count > 0)
                        {
                            var po = (ActivityOption)selectedProduct.ProductOptions.FirstOrDefault();
                            var failureMailContext = new Entities.Mailer.FailureMailContext
                            {
                                TokenId = token,
                                ServiceId = selectedProduct?.Id ?? 0,
                                BookingReferenceNumber = booking?.ReferenceNumber,
                                APICancellationStatus = cancelStatusWithReferenceIds[referenceId],
                                TravelDate = po.TravelInfo.StartDate,
                                CustomerEmailId = booking?.User?.EmailAddress,
                                ContactNumber = booking?.User?.MobileNumber ?? booking?.User?.PhoneNumber,
                                APIBookingReferenceNumber = bookedProduct?.APIExtraDetail?.SupplieReferenceNumber,
                                ApiTypeName = selectedProduct?.APIType.ToString(),
                                AvailabilityReferenceId = referenceId,
                                ServiceOptionId = po?.ServiceOptionId.ToString(),
                                OptionName = po.Name,
                                SupplierOptionCode = $"{po.SupplierOptionCode}-{po.Code}",
                                CustomerName = booking?.User?.FirstName + " " + booking?.User?.LastName,
                                ApiErrorMessage = SerializeDeSerializeHelper.Serialize(booking?.Errors?.FirstOrDefault()?.Message)
                            };

                            if (selectedProduct?.APIType == APIType.PrioHub)
                            {
                                var distributrerID = ((PrioHubSelectedProduct)selectedProduct).PrioHubDistributerId;
                                if (distributrerID == "2425" || distributrerID == "1070569")//prio Products
                                {
                                    failureMailContext.ApiTypeName = "PrioTicket";
                                }
                            }

                            failureContextList.Add(failureMailContext);
                        }
                    }

                    if (failureContextList?.Count > 0)
                    {
                        //Send failure mail to customer support
                        var lastnode = failureContextList?.LastOrDefault();
                        if (lastnode != null)
                        {
                            lastnode.BookingErrors = SerializeDeSerializeHelper.Serialize(booking.Errors);
                        }
                        _mailerService.SendFailureMail(failureContextList);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "SendFailureMail",
                };
                _log.Error(isangoErrorEntity, ex);
            }
        }

        private List<BookedProduct> CancellableSupplierBooking(ActivityBookingCriteria criteria)
        {
            #region API calls to make booking

            var bookedProducts = new List<BookedProduct>();
            var cancellableSelectedProductsByAPI = GetCancellableProducts(criteria.Booking.SelectedProducts);

            var taskArray =
                new Task<List<BookedProduct>>[cancellableSelectedProductsByAPI.Count(x => x.Value.Count != 0)];

            var taskArrayCount = 0;
            try
            {
                criteria.SelectedProducts = new List<SelectedProduct>();
                var apiTypes = cancellableSelectedProductsByAPI.Keys;
                foreach (var apiType in apiTypes)
                {
                    var selectedProducts = cancellableSelectedProductsByAPI[apiType];
                    if (selectedProducts.Count != 0)
                    {
                        criteria.SelectedProducts.AddRange(selectedProducts);
                        switch (apiType)
                        {
                            case APIType.Hotelbeds:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookHBActivity(criteria));
                                break;

                            case APIType.Citysightseeing:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookCitysightseeing(criteria));
                                break;

                            case APIType.Graylineiceland:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookGraylineIceland(criteria));
                                break;

                            case APIType.Prio:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookPrioProducts(criteria));
                                break;

                            case APIType.Fareharbor:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookFareHarbor(criteria));
                                break;

                            case APIType.Bokun:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookBokunProducts(criteria));
                                break;

                            case APIType.Aot:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookAotProducts(criteria));
                                break;

                            case APIType.BigBus:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookBigBusProduct(criteria));
                                break;

                            case APIType.Redeam:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookRedeamProducts(criteria));
                                //bookedProducts.AddRange(BookRedeamProducts(criteria));
                                break;

                            case APIType.Rezdy:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookRezdyProducts(criteria));
                                break;

                            case APIType.GlobalTix:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookGlobalTixProducts(criteria));
                                break;

                            case APIType.Ventrata:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookVentrataProducts(criteria));
                                break;

                            //case APIType.Tiqets:
                            //    taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookTiqetsProducts(criteria));
                            //    break;

                            case APIType.TourCMS:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookTourCMSActivity(criteria));
                                break;

                            case APIType.NewCitySightSeeing:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookNewCitySightSeeingProducts(criteria));
                                break;

                            case APIType.GoCity:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookGoCity(criteria));
                                break;

                            case APIType.PrioHub:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookPrioHubProducts(criteria));
                                break;

                            case APIType.Rayna:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookRaynaProducts(criteria));
                                break;
                        }

                        taskArrayCount += 1;
                    }
                }

                Task.WaitAll(taskArray);

                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                Parallel.ForEach(taskArray, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, task =>
                {
                    var data = task.GetAwaiter().GetResult();
                    if (data != null)
                        bookedProducts.AddRange(data);
                });
                return bookedProducts;
            }
            catch (AggregateException ae)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "SupplierBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ae);

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , ae.Message);

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        /*_failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.A*/ string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(_failedSupplierProducts?.FirstOrDefault().Key ?? 0), _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception exdb)
                {
                    var isangoErrorEntitydb = new IsangoErrorEntity
                    {
                        ClassName = "BookingService",
                        MethodName = "SupplierBooking",
                        Token = criteria.Token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                    };
                    _log.Error(isangoErrorEntitydb, exdb);
                    //ignore
                }
                // no need to throw the exception.
                throw;
            }
            catch (Exception exp)
            {
                var isangoErrorEntitydb = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "SupplierBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntitydb, exp);
                throw;
            }
        }
        
        private List<BookedProduct> CancellableSupplierBooking(ActivityBookingCriteria criteria, 
            CanocalizationActivityBookingCriteria canocalizationCriteria = null)
        {
           

            var bookedProducts = new List<BookedProduct>();
            var cancellableSelectedProductsByAPI = GetCancellableProducts(criteria.Booking.SelectedProducts);
            var cancellableSelectedProductsByAPICanocalization = GetCancellableProductsCanocalization(criteria.Booking.SelectedProducts);

            var taskArray =
                new Task<List<BookedProduct>>[cancellableSelectedProductsByAPI.Count(x => x.Value.Count != 0) +
                cancellableSelectedProductsByAPICanocalization.Count(x => x.Value.Count != 0)];



            var taskArrayCount = 0;
            try
            {
                criteria.SelectedProducts = new List<SelectedProduct>();
                canocalizationCriteria.SelectedProducts = new List<SelectedProduct>();

                var mergeDic = new Dictionary<APIType, List<SelectedProduct>>();
                mergeDic = cancellableSelectedProductsByAPI.Union(cancellableSelectedProductsByAPICanocalization).
                    ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                var apiTypes = mergeDic.Keys;

                foreach (var apiType in apiTypes)
                {
                    var selectedProducts = cancellableSelectedProductsByAPI.ContainsKey(apiType) ? cancellableSelectedProductsByAPI[apiType] : null;
                    var canocalizationSelectedProducts = cancellableSelectedProductsByAPICanocalization.ContainsKey(apiType) ? cancellableSelectedProductsByAPICanocalization[apiType] : null;

                    if (selectedProducts?.Count != 0 || canocalizationSelectedProducts?.Count != 0)
                    {
                        if (selectedProducts != null)
                        {
                            criteria.SelectedProducts.AddRange(selectedProducts);
                        }
                        if (canocalizationSelectedProducts != null)
                        {
                            canocalizationCriteria.SelectedProducts.AddRange(canocalizationSelectedProducts);
                        }
                        switch (apiType)
                        {
                            case APIType.Hotelbeds:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookHBActivity(criteria));
                                break;

                            case APIType.Citysightseeing:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookCitysightseeing(criteria));
                                break;

                            case APIType.Graylineiceland:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookGraylineIceland(criteria));
                                break;

                            case APIType.Prio:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookPrioProducts(criteria));
                                break;

                            case APIType.Fareharbor:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookFareHarbor(criteria));
                                break;

                            case APIType.Bokun:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookBokunProducts(criteria));
                                break;

                            case APIType.Aot:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookAotProducts(criteria));
                                break;

                            case APIType.BigBus:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookBigBusProduct(criteria));
                                break;

                            case APIType.Redeam:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookRedeamProducts(criteria));
                                //bookedProducts.AddRange(BookRedeamProducts(criteria));
                                break;

                            case APIType.Rezdy:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookRezdyProducts(criteria));
                                break;

                            case APIType.GlobalTix:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookGlobalTixProducts(criteria));
                                break;

                            case APIType.Ventrata:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookVentrataProducts(criteria));
                                break;

                            //case APIType.Tiqets:
                            //    taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookTiqetsProducts(criteria));
                            //    break;

                            case APIType.TourCMS:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookTourCMSActivity(criteria));
                                break;

                            case APIType.NewCitySightSeeing:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookNewCitySightSeeingProducts(criteria));
                                break;

                            case APIType.GoCity:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookGoCity(criteria));
                                break;

                            case APIType.PrioHub:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookPrioHubProducts(criteria));
                                break;

                            case APIType.Rayna:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookRaynaProducts(criteria));
                                break;
                            case APIType.RedeamV12:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookRedeamV12Products(canocalizationCriteria));

                                break;
                            case APIType.GlobalTixV3:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => BookGlobalTixV3Products(canocalizationCriteria));

                                break;
                        }

                        taskArrayCount += 1;
                    }
                }

                Task.WaitAll(taskArray);

                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                Parallel.ForEach(taskArray, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, task =>
                {
                    var data = task.GetAwaiter().GetResult();
                    if (data != null)
                        bookedProducts.AddRange(data);
                });
                return bookedProducts;
            }
            catch (AggregateException ae)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "SupplierBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ae);

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , ae.Message);

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        /*_failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.A*/ string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(_failedSupplierProducts?.FirstOrDefault().Key ?? 0), _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception exdb)
                {
                    var isangoErrorEntitydb = new IsangoErrorEntity
                    {
                        ClassName = "BookingService",
                        MethodName = "SupplierBooking",
                        Token = criteria.Token,
                        Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                    };
                    _log.Error(isangoErrorEntitydb, exdb);
                    //ignore
                }
                // no need to throw the exception.
                throw;
            }
            catch (Exception exp)
            {
                var isangoErrorEntitydb = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "SupplierBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntitydb, exp);
                throw;
            }
        }
        private Dictionary<APIType, List<SelectedProduct>> GetCancellableProductsCanocalization(
            List<SelectedProduct> selectedProducts)
        {
            try
            {
                var cancellableSuppliers = new List<APIType>
            {
                APIType.RedeamV12,
                APIType.GlobalTixV3
            };
                var selectedProductData = selectedProducts.Where(e => cancellableSuppliers.Contains(e.APIType))
                    .GroupBy(e => e.APIType)
                    .ToDictionary(e => e.Key, e => e.ToList());
                return selectedProductData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetCancellableProductsCanocalization",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        #endregion API calls to make booking

        private List<BookedProduct> CancellableSupplierReservation(ActivityBookingCriteria criteria)
        {
            #region API calls to make booking

            var bookedProducts = new List<BookedProduct>();
            var cancellableSelectedProductsByAPI = GetCancellableReservationSelectedProducts
                (criteria.Booking.SelectedProducts);

            var taskArray =
                new Task<List<BookedProduct>>[cancellableSelectedProductsByAPI.Count(x => x.Value.Count != 0)];

            var taskArrayCount = 0;
            try
            {
                criteria.SelectedProducts = new List<SelectedProduct>();
                var apiTypes = cancellableSelectedProductsByAPI.Keys;
                foreach (var apiType in apiTypes)
                {
                    var selectedProducts = cancellableSelectedProductsByAPI[apiType];
                    if (selectedProducts.Count != 0)
                    {
                        criteria.SelectedProducts.AddRange(selectedProducts);
                        switch (apiType)
                        {
                            case APIType.Tiqets:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => ReserveTiqetsProducts(criteria));
                                break;
                            case APIType.PrioHub:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => ReservePrioHubProducts(criteria));
                                break;
                            case APIType.TourCMS:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => ReserveTourCMSProducts(criteria));
                                break;
                            case APIType.Ventrata:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => ReserveVentrataProducts(criteria));
                                break;
                            case APIType.NewCitySightSeeing:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => ReserveNewCitySightSeeingProducts(criteria));
                                break;
                            case APIType.Redeam:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => ReserveRedeamProducts(criteria));
                                break;
                            case APIType.Moulinrouge:
                                taskArray[taskArrayCount] = Task.Factory.StartNew(() => ReserveMoulinRougeProducts(criteria));
                                break;
                        }

                        taskArrayCount += 1;
                    }
                }

                Task.WaitAll(taskArray);

                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                Parallel.ForEach(taskArray, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, task =>
                {
                    var data = task?.GetAwaiter().GetResult();
                    if (data != null)
                        bookedProducts.AddRange(data);
                });
                return bookedProducts;
            }
            catch (AggregateException ae)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "SupplierBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };

                criteria?.Booking?.UpdateErrors(CommonErrorCodes.SupplierBookingError
                                , System.Net.HttpStatusCode.BadGateway
                                , ae.Message);

                try
                {
                    LogBookingFailureInDB(criteria?.Booking, criteria?.Booking?.ReferenceNumber, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.Id ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.Id, criteria.Token,
                        /*_failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.A*/ string.Empty, criteria?.Booking?.User.EmailAddress, criteria?.Booking?.User.PhoneNumber,
                        Convert.ToInt32(_failedSupplierProducts?.FirstOrDefault().Key ?? 0), _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.ServiceOptionId,
                        _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.Name, _failedSupplierProducts?.FirstOrDefault().Value?.FirstOrDefault()?.AvailabilityReferenceId ?? criteria?.Booking?.SelectedProducts?.FirstOrDefault()?.AvailabilityReferenceId, CommonErrorCodes.SupplierBookingError.ToString());
                    criteria?.Booking?.UpdateDBLogFlag();
                }
                catch (Exception e)
                {
                    //ignore
                }

                _log.Error(isangoErrorEntity, ae);
                // no need to throw the exception.
                throw;
            }

            #endregion API calls to make booking
        }

        private Tuple<List<BookedProduct>, List<string>> NonCancellableSupplierBooking(ActivityBookingCriteria criteria)
        {
            #region API calls to make booking

            var isPartialSupported = criteria.Booking.Affiliate.AffiliateConfiguration.IsPartialBookingSupport;
            var bookedProducts = new List<BookedProduct>();
            var nonCancellableSelectedProductsByAPI =
                GetNonCancellableSelectedProducts(criteria.Booking.SelectedProducts);
            var nonCancellableProductsRefIds = nonCancellableSelectedProductsByAPI.Values.SelectMany(x => x)
                .Select(e => e.AvailabilityReferenceId);

            try
            {
                var apiTypes = nonCancellableSelectedProductsByAPI.Keys;
                foreach (var apiType in apiTypes)
                {
                    var selectedProducts = nonCancellableSelectedProductsByAPI[apiType];
                    if (selectedProducts.Count == 0) continue;

                    criteria.SelectedProducts.AddRange(selectedProducts);
                    switch (apiType)
                    {
                        case APIType.Tiqets:
                            bookedProducts.AddRange(BookTiqetsProducts(criteria));
                            break;

                        case APIType.Goldentours:
                            bookedProducts.AddRange(BookGoldenToursProducts(criteria));
                            break;

                        case APIType.Moulinrouge:
                            bookedProducts.AddRange(BookMoulinRougeProducts(criteria));
                            break;

                            //case APIType.Redeam:
                            //    bookedProducts.AddRange(BookRedeamProducts(criteria));
                            //    break;
                    }

                    if (!isPartialSupported && _failedSupplierProducts.Values.Any(x => x.Count > 0))
                    {
                        var bookedProductsAvailabilityRefIds =
                            bookedProducts.Select(e => e.AvailabilityReferenceId);
                        var notProcessedProducts = criteria.Booking.IsangoBookingData.BookedProducts.Where(e =>
                            nonCancellableProductsRefIds.Contains(e.AvailabilityReferenceId) &&
                            !bookedProductsAvailabilityRefIds.Contains(e.AvailabilityReferenceId)).ToList();
                        var notProcessedProductIds =
                            notProcessedProducts.Select(x => x.AvailabilityReferenceId).ToList();

                        bookedProducts.AddRange(notProcessedProducts);
                        return new Tuple<List<BookedProduct>, List<string>>(bookedProducts, notProcessedProductIds);
                    }
                }

                return new Tuple<List<BookedProduct>, List<string>>(bookedProducts, new List<string>());
            }
            catch (AggregateException ae)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "NonCancellableSupplierBooking",
                    Token = criteria.Token,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ae);
                throw;
            }

            #endregion API calls to make booking
        }

        private Dictionary<APIType, List<SelectedProduct>> GetCancellableSelectedProducts(
            List<SelectedProduct> selectedProducts)
        {
            try
            {
                var cancellableSuppliers = new List<APIType>
            {
                APIType.Hotelbeds,
                APIType.Aot,
                APIType.Bokun,
                APIType.Citysightseeing,
                APIType.Fareharbor,
                APIType.Graylineiceland,
                APIType.Prio,
                APIType.BigBus,
                APIType.Redeam,
                APIType.Rezdy,
                APIType.GlobalTix,
                APIType.Ventrata,
                APIType.Tiqets,
                APIType.TourCMS,
                APIType.GoCity,
                APIType.NewCitySightSeeing,
                APIType.PrioHub,

                APIType.Rayna
            };
                var selectedProductData = selectedProducts.Where(e => cancellableSuppliers.Contains(e.APIType))
                    .GroupBy(e => e.APIType)
                    .ToDictionary(e => e.Key, e => e.ToList());
                return selectedProductData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetCancellableSelectedProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Dictionary<APIType, List<SelectedProduct>> GetCancellableReservationSelectedProducts(
    List<SelectedProduct> selectedProducts)
        {
            try
            {
                var cancellableSuppliers = new List<APIType>
            {

                APIType.Ventrata,
                APIType.Tiqets,
                APIType.TourCMS,
                APIType.PrioHub,
                APIType.NewCitySightSeeing,
                APIType.Redeam,
                APIType.Moulinrouge
            };
                var selectedProductData = selectedProducts.Where(e => cancellableSuppliers.Contains(e.APIType))
                    .GroupBy(e => e.APIType)
                    .ToDictionary(e => e.Key, e => e.ToList());
                return selectedProductData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetCancellableReservationSelectedProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Dictionary<APIType, List<SelectedProduct>> GetCancellableProducts(
            List<SelectedProduct> selectedProducts)
        {
            try
            {
                var cancellableSuppliers = new List<APIType>
            {
                APIType.Hotelbeds,
                APIType.Aot,
                APIType.Bokun,
                APIType.Citysightseeing,
                APIType.Fareharbor,
                APIType.Graylineiceland,
                APIType.Prio,
                APIType.BigBus,
                APIType.Redeam,
                APIType.Rezdy,
                APIType.GlobalTix,
                APIType.Ventrata,
                //APIType.Tiqets,
                APIType.TourCMS,
                APIType.GoCity,
                APIType.NewCitySightSeeing,
                APIType.PrioHub,
                APIType.Rayna
            };
                var selectedProductData = selectedProducts.Where(e => cancellableSuppliers.Contains(e.APIType))
                    .GroupBy(e => e.APIType)
                    .ToDictionary(e => e.Key, e => e.ToList());
                return selectedProductData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetCancellableProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Dictionary<APIType, List<SelectedProduct>> GetNonCancellableSelectedProducts(
            List<SelectedProduct> selectedProducts)
        {
            try
            {
                var selectedProductData = new Dictionary<APIType, List<SelectedProduct>>();

                var moulinRougeSelectedProducts =
                    selectedProducts.FindAll(product => product.APIType.Equals(APIType.Moulinrouge));
                var tiqetsSelectedProducts = selectedProducts.FindAll(product => product.APIType.Equals(APIType.Tiqets));
                var goldenToursSelectedProducts =
                    selectedProducts.FindAll(product => product.APIType.Equals(APIType.Goldentours));
                var bigBusSelectedProducts = selectedProducts.FindAll(product => product.APIType.Equals(APIType.BigBus));

                // Maintaining supplier sequence for the Non Cancellable Booking call
                if (tiqetsSelectedProducts.Count > 0)
                    selectedProductData.Add(APIType.Tiqets, tiqetsSelectedProducts);
                if (goldenToursSelectedProducts.Count > 0)
                    selectedProductData.Add(APIType.Goldentours, goldenToursSelectedProducts);
                if (moulinRougeSelectedProducts.Count > 0)
                    selectedProductData.Add(APIType.Moulinrouge, moulinRougeSelectedProducts);

                return selectedProductData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetNonCancellableSelectedProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private void MaintainLog(WirecardPaymentResponse wirecardPaymentResponse)
        {
            try
            {
                var wireCardXmlCriteria = new WireCardXmlCriteria
                {
                    JobId = wirecardPaymentResponse.JobId,
                    TransGuWId = string.IsNullOrEmpty(wirecardPaymentResponse.TransactionId)
                        ? string.Empty
                        : wirecardPaymentResponse.TransactionId,
                    Status = wirecardPaymentResponse.Status,
                    Request = wirecardPaymentResponse.RequestXml.Replace("'", "\\\""),
                    Response = wirecardPaymentResponse.ResponseXml.Replace("'", "\\\""),
                    RequestType = !string.IsNullOrEmpty(wirecardPaymentResponse.EnrollmentCheckStatus)
                        ? wirecardPaymentResponse.EnrollmentCheckStatus
                        : wirecardPaymentResponse.RequestType,
                    TransDate = DateTime.Now
                };

                LogTransaction(wireCardXmlCriteria);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "MaintainLog",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Payment GetPaymentDetailsOfBooking(Booking booking, decimal amount, PaymentStatus paymentStatus, string IsangoReference = null, string token = null)
        {
            try
            {
                var payment = new Payment
                {
                    PaymentType = booking.Payment?.Count > 0 ? booking.Payment[0].PaymentType : null,
                    PaymentStatus = paymentStatus,
                    JobId = "0",
                    TransactionFlowType = TransactionFlowType.Payment,
                    ChargeAmount = amount,
                    CurrencyCode = booking.Currency.IsoCode.ToUpper(),
                    Guwid = booking.Guwid,
                    PaymentGatewayReferenceId = booking.Guwid
                };

                if (booking.PaymentGateway == PaymentGatewayType.Apexx)//Apexx
                {
                    payment.Token = token;
                    if (!string.IsNullOrEmpty(IsangoReference) && paymentStatus == PaymentStatus.PreAuthorized)
                    {
                        payment.TransactionId = IsangoReference;
                    }
                    else if (!string.IsNullOrEmpty(booking.ReferenceNumber) && paymentStatus == PaymentStatus.Paid)
                    {
                        payment.TransactionId = booking?.ReferenceNumber + "_" + UniqueTransactionIdGenerator.GenerateTransactionId();
                    }
                    else
                    {
                        payment.TransactionId = UniqueTransactionIdGenerator.GenerateTransactionId();
                    }
                }
                else //wirecard or Adyen
                {
                    payment.TransactionId = UniqueTransactionIdGenerator.GenerateTransactionId();
                }

                return payment;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetPaymentDetailsOfBooking",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        ///<summary>
        /// Service method to update existing booking against the amendments done to the booking via Booking Manager
        /// </summary>
        private bool ProcessAmendedBooking(Booking amendedBooking, int amendmentId, string token)
        {
            try
            {
                var isBookingSuccessful = false;
                if (amendedBooking.PaymentMethodType == PaymentMethodType.Transaction)
                {
                    // Payment status is paid 'coz OnRequest bookings are not eligible for any amendments.
                    var payment = GetPaymentDetailsOfBooking(amendedBooking, 0, PaymentStatus.Paid);
                    payment.Is3D = true;
                    amendedBooking.Payment.Add(payment);

                    isBookingSuccessful = UpdateBookingAgainstAmendment(amendedBooking, amendmentId, token);
                    return isBookingSuccessful;
                }

                return isBookingSuccessful;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ProcessAmendedBooking",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary/>
        /// Perform the booking for the amended products in the booking object.
        /// This is a transactional operation, with rollback support if the booking fails at any stage.
        ///<summary>
        ///This action does a transaction that :-
        ///1. Update the booking concerned in the database and get transaction Id in return.
        ///2. Hit the wire card and get GUWID in case of success.
        ///3. Update the database with new GUWID that gets returned from the wire card.
        ///4. Send confirmation mail to the customer and to the CS Team.
        ///</summary>
        ///
        private bool UpdateBookingAgainstAmendment(Booking amendedBooking, int amendmentId, string token)
        {
            var rollbackBooking = false;
            bool returnStatus;
            Payment preauthPayment = null;
            Payment purchasePayment = null;
            var isWireCardTransactionFailed = false;
            //Is3d bool temporarily set to false
            var is3D = true;

            try
            {
                //Step one of transaction
                //Update Isango Booking in DB
                //How to figure out whether the card is 3d or not ??
                var purchaseTransactionId =
                    _bookingPersistence.UpdateIsangoBooking(amendmentId, is3D, ref purchasePayment);
                var isWireCardEnable = ConfigurationManagerHelper.GetValuefromAppSettings("IsWireCardEnable");
                if (isWireCardEnable != null && Convert.ToBoolean(isWireCardEnable))
                {
                    if (purchaseTransactionId != 0)
                    {
                        purchasePayment = amendedBooking.Payment.Find(FilterPurchasePayment);
                        purchasePayment.TransactionId = Convert.ToString(purchaseTransactionId);
                        purchasePayment.ChargeAmount = amendedBooking.Amount;
                        isWireCardTransactionFailed =
                            WireCardTransactionForAmendedBooking(amendedBooking, purchasePayment, token);
                    }
                    else
                    {
                        rollbackBooking = true;
                    }

                    if (isWireCardTransactionFailed)
                        rollbackBooking = true;
                }
            }
            catch (Exception ex)
            {
                rollbackBooking = true;
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "UpdateBookingAgainstAmendment",
                    Token = token,
                    AffiliateId = amendedBooking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(amendedBooking)},{amendmentId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            finally
            {
                if (rollbackBooking)
                {
                    Tuple<WirecardPaymentResponse, string, string> wirecardPaymentResponse = null;
                    try
                    {
                        if (preauthPayment != null)
                        {
                            preauthPayment.Is3D = false;
                            wirecardPaymentResponse = _wirecardPaymentAdapter.Rollback(preauthPayment, token);
                        }

                        if (purchasePayment != null)
                        {
                            purchasePayment.Is3D = false;
                            wirecardPaymentResponse = _wirecardPaymentAdapter.Rollback(purchasePayment, token);
                        }
                    }
                    finally
                    {
                        returnStatus = false;
                        MaintainLog(wirecardPaymentResponse?.Item1);
                    }
                }
                else
                    returnStatus = true;
            }

            return returnStatus;
        }

        /// <summary>
        /// Predicate for Purchase payment object.
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        private bool FilterPurchasePayment(Payment payment)
        {
            return payment.PaymentStatus.Equals(PaymentStatus.Paid);
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

        /// <summary>
        /// Perform a standard wirecard transaction for supported card types.
        /// </summary>
        /// <param name="amendedBooking"></param>
        /// <param name="purchasePayment"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool WireCardTransactionForAmendedBooking(Booking amendedBooking, Payment purchasePayment, string token)
        {
            var isWireCardTransactionFailed = false;
            // Step 2: if every thing is fine than Hit the WireCard
            try
            {
                if (amendedBooking.PaymentMethodType == PaymentMethodType.Transaction)
                {
                    if (purchasePayment != null)
                    {
                        PurchaseAmendedBooking(purchasePayment, amendedBooking, token);
                    }
                    else
                    {
                        isWireCardTransactionFailed = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "WireCardTransactionForAmendedBooking",
                    Token = token,
                    AffiliateId = amendedBooking.Affiliate.Id,
                    Params =
                        $"{SerializeDeSerializeHelper.Serialize(purchasePayment)},{SerializeDeSerializeHelper.Serialize(amendedBooking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            // End Step 2
            return isWireCardTransactionFailed;
        }

        /// <summary>
        /// This method is used for Payment of amended free sale items
        /// </summary>
        /// <param name="purchasePayment"></param>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        private void PurchaseAmendedBooking(Payment purchasePayment, Booking booking, string token)
        {
            Tuple<WirecardPaymentResponse, string, string> wirecardPaymentResponse = null;
            try
            {
                purchasePayment.JobId = GetJobId(booking);
                purchasePayment.IpAddress = booking.IpAddress;
                wirecardPaymentResponse = _wirecardPaymentAdapter.Charge3D(purchasePayment, booking, token);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "TransactRefund",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params =
                        $"{SerializeDeSerializeHelper.Serialize(purchasePayment)},{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            finally
            {
                MaintainLog(wirecardPaymentResponse?.Item1);
            }
        }

        /// <summary>
        ///  Book Back the amended products.
        ///  This is a transactional operation, with rollback support if the booking related refund fails at any stage.
        /// This action does a transaction that :-
        /// 1. Update the booking concerned in the database and get transaction Id in return.
        /// 2. Hit the wire card and get GUWID in case of successful refund.
        /// 3. Update the database with new GUWID that gets returned from the wire card.
        /// 4. Send refund confirmation mail to the customer and to the CS Team.
        /// </summary>
        ///  <param name="amendmentId"></param>
        /// <param name="remarks"></param>
        /// <param name="actionBy"></param>
        /// <param name="token"></param>
        private bool TransactRefund(int amendmentId, string remarks, string actionBy, string token)
        {
            bool returnStatus;
            var rollbackBooking = false;
            var noOfSuccessfulWCHits = 0;
            var amendedBooking = new Booking();
            try
            {
                //Step one of transaction
                //Update Isango Booking in DB with the amendments made
                //How to figure out whether the card is 3d or not ??

                amendedBooking = _bookingPersistence.UpdateIsangoBookingAgainstRefund(amendmentId, remarks, actionBy);
                var isWireCardEnable = ConfigurationManagerHelper.GetValuefromAppSettings("IsWireCardEnable");
                if (isWireCardEnable != null && Convert.ToBoolean(isWireCardEnable))
                {
                    //Step 2
                    //Hit the wire card if the returned transaction Id is not 0 (Default value of an integer)
                    //This code portion also updates the booking with the new Transaction Id and GUWID in case of wire card success

                    foreach (var bookBackPayment in amendedBooking.Payment)
                    {
                        // (Need to think about it) isWireCardBookBackFailed = WireCardTransactionForBookBack(amendedBooking, bookBackPayment, paymentService);
                        //noOfSuccessfulWCHits = WireCardTransactionForBookBack(amendedBooking, bookBackPayment, noOfSuccessfulWCHits);
                    }
                }

                if (noOfSuccessfulWCHits != amendedBooking.Payment.Count)
                {
                    rollbackBooking = true;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "TransactRefund",
                    Token = token,
                    Params = $"{amendmentId},{remarks},{actionBy}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            finally
            {
                if (rollbackBooking)
                {
                    Tuple<WirecardPaymentResponse, string, string> wirecardPaymentResponse = null;
                    for (var noOfWCHitsToBeDone = 0; noOfWCHitsToBeDone < noOfSuccessfulWCHits; noOfWCHitsToBeDone++)
                    {
                        try
                        {
                            wirecardPaymentResponse =
                                _wirecardPaymentAdapter.Rollback(amendedBooking.Payment[noOfWCHitsToBeDone], token);
                        }
                        finally
                        {
                            MaintainLog(wirecardPaymentResponse?.Item1);
                        }
                    }

                    returnStatus = false;
                }
                else
                {
                    returnStatus = true;
                }
            }

            return returnStatus;
        }

        /// <summary>
        /// Prepare the BookingXmlData model
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        public string GetJobId(Booking booking)
        {
            return $"{booking.User.UserId}_{booking.ReferenceNumber}";
        }

        /// <summary>
        /// Prepare the BookingXmlData model
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        private IsangoBookingData MapIsangoBookingData(Booking booking)
        {
            try
            {
                if (booking?.IsangoBookingData == null) return null;

                var isangoBookingData = booking.IsangoBookingData;
                isangoBookingData.BookingReferenceNumber = booking.ReferenceNumber;

                if (booking.Amount > 0)
                    isangoBookingData.TransactionDetail = GetTransactionDetails(booking);

                foreach (var bookedProduct in isangoBookingData.BookedProducts)
                {
                    var selectedProduct = booking.SelectedProducts.FirstOrDefault(x =>
                        x.AvailabilityReferenceId.Equals(bookedProduct.AvailabilityReferenceId));

                    if (selectedProduct == null) continue;

                    if (selectedProduct is HotelBedsSelectedProduct hbSelectedProduct)
                    {
                        if (hbSelectedProduct.ServiceContract != null)
                        {
                            bookedProduct.ContractComment = hbSelectedProduct.ServiceContract.Comments?.FirstOrDefault(x =>
                                x.Type.Equals(Constant.Contract, StringComparison.InvariantCultureIgnoreCase))?.CommentText;
                        }
                    }

                    bookedProduct.AvailabilityReferenceId = selectedProduct.AvailabilityReferenceId;
                    bookedProduct.ServiceId = selectedProduct.Id;
                    bookedProduct.PickUpLocation = selectedProduct.HotelPickUpLocation;
                    bookedProduct.SpecialRequest = selectedProduct.SpecialRequest;
                    bookedProduct.UnitType = selectedProduct.UnitType;
                    bookedProduct.DiscountList = GetDiscountList(selectedProduct.AppliedDiscountCoupons);
                    bookedProduct.OptionPrice.MultiSave = selectedProduct.MultisaveDiscountedPrice;

                    var selectedOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                    bookedProduct.CancellationPolicy = selectedOption.CancellationPrices;

                    if (
                        //No CancellationPolicy Policy in avail or booking calls
                        selectedProduct.APIType == APIType.Citysightseeing

                        //No CancellationPolicy Policy in avail or booking calls
                        || selectedProduct.APIType == APIType.Prio

                        /*No CancellationPolicy Policy in avail, Although Comes in booking but good enough to transform.
                         *
                         Sending in APIExtraDetail
                         "{\"Cutoff\":null,\"Type\":\"never\",\"IsEligibleForCancellation\":true}"
                         */
                        || selectedProduct.APIType == APIType.Fareharbor

                    )
                    {
                        bookedProduct.CancellationPolicy = null;
                    }
                    //if (bookedProduct.CancellationPolicy == null)
                    //{
                    //    if (!string.IsNullOrWhiteSpace(selectedOption.CancellationText))
                    //    {
                    //        var ao = selectedOption as ActivityOption;
                    //        bookedProduct.CancellationPolicy = new List<CancellationPrice>();
                    //        var cancellationPrice = default(CancellationPrice);
                    //        if (ao.Cancellable)
                    //        {
                    //            //Default Cancellation policy 24 b4 cancellation free if its enhanceable.
                    //            cancellationPrice = new CancellationPrice
                    //            {
                    //                CancellationDescription = selectedOption.CancellationText,
                    //                CancellationFromdate = bookedProduct.CheckinDate.AddDays(-1),
                    //                CancellationToDate = bookedProduct.CheckinDate,
                    //                CancellationDateRelatedToOpreationDate = bookedProduct.CheckinDate,
                    //                CancellationAmount = bookedProduct.OptionPrice.SellPrice,
                    //                Percentage = 100
                    //            };
                    //        }
                    //        else
                    //        {
                    //            cancellationPrice = new CancellationPrice
                    //            {
                    //                CancellationDescription = selectedOption.CancellationText,
                    //                CancellationFromdate = DateTime.Now.Date,
                    //                CancellationToDate = bookedProduct.CheckinDate,
                    //                CancellationDateRelatedToOpreationDate = bookedProduct.CheckinDate,
                    //                CancellationAmount = bookedProduct.OptionPrice.SellPrice,
                    //                Percentage = 100
                    //            };
                    //        }
                    //        bookedProduct.CancellationPolicy.Add(cancellationPrice);
                    //    }
                    //}

                    if (selectedOption != null)
                    {
                        bookedProduct.OptionId = selectedOption.ServiceOptionId;
                        bookedProduct.BundleOptionId = selectedOption.BundleOptionID;
                        bookedProduct.OptionName = selectedOption.Name;
                        bookedProduct.CheckinDate = selectedOption.TravelInfo.StartDate.Date;
                        bookedProduct.CheckoutDate = selectedOption.TravelInfo.StartDate.Date;

                        //1: ON REQUEST, 2: AVAILABLE, 3: CANCEL, 0: FAILURE
                        if (selectedProduct.APIType.Equals(APIType.Undefined))
                        {
                            switch (selectedOption.AvailabilityStatus)
                            {
                                case AvailabilityStatus.ONREQUEST:
                                    selectedOption.BookingStatus = OptionBookingStatus.Requested;
                                    bookedProduct.OptionStatus = "1";
                                    break;

                                case AvailabilityStatus.AVAILABLE:
                                    selectedOption.BookingStatus = OptionBookingStatus.Confirmed;
                                    bookedProduct.OptionStatus = "2";
                                    break;
                            }
                        }
                    }
                }

                return isangoBookingData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "MapIsangoBookingData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Prepares the DiscountList from the Discount Coupons model
        /// </summary>
        /// <param name="discountCoupons"></param>
        /// <returns></returns>
        private List<Discount> GetDiscountList(List<AppliedDiscountCoupon> discountCoupons)
        {
            try
            {
                var discountList = new List<Discount>();
                if (discountCoupons == null || discountCoupons.Count == 0) return discountList;

                foreach (var discountCoupon in discountCoupons)
                {
                    var discount = new Discount
                    {
                        DiscountCode = discountCoupon.Code,
                        DiscountSellAmount = discountCoupon.Price
                    };
                    discountList.Add(discount);
                }

                return discountList;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetDiscountList",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<TransactionDetail> GetTransactionDetails(Booking booking)
        {
            try
            {
                var transactionDetails = new List<TransactionDetail>();

                var preAuthPayment = booking.Payment.Find(FilterPreAuthPayment);
                var purchasePayment = booking.Payment.Find(FilterPurchasePayment);

                if (preAuthPayment != null)
                {
                    if (preAuthPayment.TransactionId.Equals("0"))
                        preAuthPayment.TransactionId = UniqueTransactionIdGenerator.GenerateTransactionId();
                }

                if (purchasePayment != null)
                {
                    if (purchasePayment.TransactionId.Equals("0"))
                        purchasePayment.TransactionId = UniqueTransactionIdGenerator.GenerateTransactionId();
                }

                CreditCard creditCard;
                Payment payment;
                var cardType = booking.PaymentOption.ToString();

                if (booking.PaymentGateway.Equals(PaymentGatewayType.WireCard) || booking.PaymentGateway.Equals(PaymentGatewayType.Apexx) || booking.PaymentGateway.Equals(PaymentGatewayType.Adyen))
                {
                    payment = booking.Payment.FirstOrDefault();
                    creditCard = (CreditCard)payment?.PaymentType;
                    cardType = creditCard != null ? creditCard.CardType : "scheme";
                }

                var bookedSelectedProducts = GetBookedSelectedProduct(booking);

                if (preAuthPayment != null)
                {
                    var allSelectedProductIdList = new List<int>();
                    foreach (var selectedProduct in bookedSelectedProducts)
                    {
                        var optionIds = selectedProduct?.ProductOptions?.Select(x => x.ServiceOptionId > 0 ? x.ServiceOptionId : x.Id)?.ToList();
                        if (optionIds?.Count > 0)
                        {
                            allSelectedProductIdList.AddRange(optionIds);
                        }
                    }

                    var authTransactionDetail = new TransactionDetail
                    {
                        CardHolderName = $"{booking.User.FirstName} {booking.User.LastName}",
                        CardType = cardType,
                        PaymentGatewayId = preAuthPayment.TransactionId,
                        PaymentGatewayTransactionId = !string.IsNullOrEmpty(preAuthPayment?.PaymentGatewayReferenceId)
                            ? preAuthPayment?.PaymentGatewayReferenceId
                            : booking.Guwid,
                        AuthorizationCode = booking.PaymentGateway.Equals(PaymentGatewayType.Apexx) ? preAuthPayment.Token : preAuthPayment.AuthorizationCode,//preAuthPayment.AuthorizationCode,
                                                                                                                                                              //Token Add only in PreAuthorize ,
                                                                                                                                                              //if booking contain ONREQUEST product, then only it have value
                        TransactionFlowName = preAuthPayment.PaymentStatus.ToString(),
                        TransactionAmount = preAuthPayment.ChargeAmount,
                        PaymentGateway = booking.PaymentGateway.ToString(),
                        OptionIds = new List<int>(),
                        AdyenMerchantAccout = booking.AdyenMerchantAccount
                    };

                    authTransactionDetail.OptionIds.AddRange(allSelectedProductIdList);

                    transactionDetails.Add(authTransactionDetail);
                }

                if (purchasePayment != null)
                {
                    var fsSelectedProductIdList = new List<int>();
                    foreach (var selectedProduct in bookedSelectedProducts)
                    {
                        var optionIds =
                            selectedProduct.ProductOptions
                            .Where(y => y.AvailabilityStatus != AvailabilityStatus.ONREQUEST).Select(x => x.ServiceOptionId > 0 ? x.ServiceOptionId : x.Id)?.ToList();

                        if (optionIds?.Count > 0)
                        {
                            fsSelectedProductIdList.AddRange(optionIds);
                        }
                    }

                    var authTransactionDetail = new TransactionDetail
                    {
                        CardHolderName = $"{booking.User.FirstName} {booking.User.LastName}",
                        CardType = cardType,
                        PaymentGatewayId = purchasePayment?.TransactionId,
                        PaymentGatewayTransactionId = !string.IsNullOrEmpty(purchasePayment?.PaymentGatewayReferenceId)
                            ? purchasePayment?.PaymentGatewayReferenceId
                            : booking.Guwid,
                        AuthorizationCode = purchasePayment.AuthorizationCode,
                        TransactionFlowName = purchasePayment.PaymentStatus.ToString(),
                        TransactionAmount = purchasePayment.ChargeAmount,
                        PaymentGateway = booking.PaymentGateway.ToString(),
                        OptionIds = new List<int>(),
                        AdyenMerchantAccout = booking.AdyenMerchantAccount

                    };
                    authTransactionDetail.OptionIds.AddRange(fsSelectedProductIdList);

                    transactionDetails.Add(authTransactionDetail);
                }

                return transactionDetails;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetTransactionDetails",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #endregion Private Methods

        #region API Booking Calls Private Methods

        private List<BookedProduct> BookHBActivity(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateHbActivityBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Hotelbeds, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookHBActivity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookCitysightseeing(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.GenerateSightSeeingQRCode(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Citysightseeing, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookCitysightseeing",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookGraylineIceland(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateGraylineIcelandBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Graylineiceland, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookGraylineIceland",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookPrioProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreatePrioProductsBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Prio, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookPrioProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookPrioHubProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreatePrioHubProductsBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.PrioHub, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookPrioProducts",
                    Token = criteria.Token
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookVentrataProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateVentrataProductsBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Ventrata, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookVentrataProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookFareHarbor(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts?.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateFareHarborBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Fareharbor, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookFareHarbor",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookBokunProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateBokunBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Bokun, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookBokunProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookAotProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateAotBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Aot, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookAotProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookMoulinRougeProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateMoulinRougeProductsBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Moulinrouge, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookMoulinRougeProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private bool ProcessBookBack(int amendmentId, string remarks, string actionBy, string token)
        {
            try
            {
                //TODO: IPaymentService paymentService = PaymentFactory.GetPaymentGateway(true);
                var isRefunded = TransactRefund(amendmentId, remarks, actionBy, token);
                return isRefunded;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ProcessBookBack",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Booking CreatePaymentForORAndFS(Booking booking, bool? isAlternativePayment = false, string IsangoReference = null, string token = null)
        {
            try
            {
                if (booking.Payment == null)
                    booking.Payment = new List<Payment>();
                if (booking.PaymentMethodType == PaymentMethodType.Transaction)
                {
                    if (isAlternativePayment == false)
                    {
                        var preAuthorizePayment = CreatePaymentForOR(booking, IsangoReference, token);
                        booking.Payment.Add(preAuthorizePayment);
                    }

                    var paidPayment = CreatePaymentForFS(booking, IsangoReference, token);
                    if (paidPayment != null)
                        booking.Payment.Add(paidPayment);
                }

                return booking;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreatePaymentForORAndFS",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Payment CreatePaymentForFS(Booking booking, string IsangoReference = null, string token = null)
        {
            try
            {
                decimal totalConfirmedBookingAmount = 0;

                var bookedSelectedProducts = GetBookedSelectedProduct(booking);
                foreach (var selectedProduct in bookedSelectedProducts)
                {
                    var productOptions = selectedProduct.ProductOptions.Where(x => x.IsSelected).ToList();

                    var confirmedProductOptions =
                        productOptions.Where(x => !x.AvailabilityStatus.Equals(AvailabilityStatus.ONREQUEST)).ToList();

                    if (confirmedProductOptions.Any())
                    {
                        totalConfirmedBookingAmount += confirmedProductOptions.Sum(y => y.SellPrice.Amount);
                    }
                }

                if (totalConfirmedBookingAmount > 0)
                {
                    totalConfirmedBookingAmount = Convert.ToDecimal((totalConfirmedBookingAmount * 100).ToString("0")) / 100;
                    var paidPayment = GetPaymentDetailsOfBooking(booking, totalConfirmedBookingAmount, PaymentStatus.Paid, IsangoReference, token);
                    return paidPayment;
                }

                return null;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreatePaymentForFS",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Payment CreatePaymentForOR(Booking booking, string IsangoReference = null, string token = null)
        {
            try
            {
                decimal totalORBookingAmount = 0;
                var bookedSelectedProducts = GetBookedSelectedProduct(booking);
                foreach (var selectedProduct in bookedSelectedProducts)
                {
                    var productOptions = selectedProduct.ProductOptions.Where(x => x.IsSelected).ToList();

                    totalORBookingAmount += productOptions.Sum(y => y.SellPrice.Amount);
                }

                totalORBookingAmount = Convert.ToDecimal((totalORBookingAmount * 100).ToString("0")) / 100;

                var preAuthorizePayment = GetPaymentDetailsOfBooking(booking, totalORBookingAmount, PaymentStatus.PreAuthorized, IsangoReference, token);
                return preAuthorizePayment;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreatePaymentForOR",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookBigBusProduct(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateBigBusBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.BigBus, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookBigBusProduct",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookTiqetsProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateTiqetsBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Tiqets, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookTiqetsProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> ReserveTiqetsProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateTiqetsBookingReservation(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    if (failedProducts != null && failedProducts.Count > 0)
                    {
                        _failedSupplierProducts.Add(APIType.Tiqets, failedProducts);
                    }
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookTiqetsProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        private List<BookedProduct> ReservePrioHubProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreatePrioHubProductsBookingReservation(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    if (failedProducts != null && failedProducts.Count > 0)
                    {
                        _failedSupplierProducts.Add(APIType.PrioHub, failedProducts);
                    }
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ReservePrioHubProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        private List<BookedProduct> ReserveTourCMSProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateTourCMSProductsBookingReservation(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    if (failedProducts != null && failedProducts.Count > 0)
                    {
                        _failedSupplierProducts.Add(APIType.TourCMS, failedProducts);
                    }
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ReserveTourCMSProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> ReserveVentrataProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateVentrataProductsBookingReservation(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    if (failedProducts != null && failedProducts.Count > 0)
                    {
                        _failedSupplierProducts.Add(APIType.Ventrata, failedProducts);
                    }
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ReserveVentrataProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> ReserveNewCitySightSeeingProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateNewCitySightSeeingReservation(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    if (failedProducts != null && failedProducts.Count > 0)
                    {
                        _failedSupplierProducts.Add(APIType.NewCitySightSeeing, failedProducts);
                    }
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ReserveVentrataProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        private List<BookedProduct> ReserveRedeamProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateRedeamBookingReservation(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    if (failedProducts != null && failedProducts.Count > 0)
                    {
                        _failedSupplierProducts.Add(APIType.Redeam, failedProducts);
                    }
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ReserveRedeamProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> ReserveMoulinRougeProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateMoulinRougeBookingReservation(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    if (failedProducts != null && failedProducts.Count > 0)
                    {
                        _failedSupplierProducts.Add(APIType.Moulinrouge, failedProducts);
                    }
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "ReserveMoulinRougeProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookNewCitySightSeeingProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateNewCitySightSeeingBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.NewCitySightSeeing, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookNewCitySightSeeingProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookGoldenToursProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateGoldenToursProductsBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Goldentours, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookGoldenToursProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookRedeamProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateRedeamBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Redeam, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookRedeamProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookGlobalTixProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateGlobalTixBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.GlobalTix, failedProducts);
                    return bookedProducts;
                }
                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookGlobalTixProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private void InsertIntoPartialBookingItem(Booking booking,
            ref Dictionary<string, int> selectedProductPartialBookingItemIdMapping)
        {
            try
            {
                foreach (var item in booking.SelectedProducts)
                {
                    var partialBooking = new PartialBooking
                    {
                        ItemStatus = PartialBookingStatus.NotBooked,
                        SelectedProductId = item.Id,
                        AvailabilityReferenceId = item.AvailabilityReferenceId,
                        BookingReferenceNumber = booking.ReferenceNumber
                    };

                    var selectedProductPartialId = _bookingPersistence.InsertPartialBooking(partialBooking);

                    selectedProductPartialBookingItemIdMapping.Add(item.AvailabilityReferenceId, selectedProductPartialId);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "InsertIntoPartialBookingItem",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Update status if selected product is of apitype undefined or all are booked or _failedSupplierProducts does not contain selected product.
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="selectedProductPartialBookingItemIdMapping"></param>
        /// <param name="operationType"></param>
        private void UpdatePartialBookingItem(Booking booking,
            Dictionary<string, int> selectedProductPartialBookingItemIdMapping, PartialBookingStatus operationType)
        {
            try
            {
                foreach (var item in booking.SelectedProducts)
                {
                    var changeStatusForPartialBooking = item.APIType == APIType.Undefined ||
                                                        _failedSupplierProducts.Count == 0 ||
                                                        _failedSupplierProducts[item.APIType].All(x => x.Id != item.Id);

                    if (!changeStatusForPartialBooking) continue;
                    var partialBooking = new PartialBooking
                    {
                        Id = selectedProductPartialBookingItemIdMapping[item.AvailabilityReferenceId],
                        SelectedProductId = item.Id,
                        ItemStatus = operationType
                    };

                    _bookingPersistence.InsertPartialBooking(partialBooking);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "UpdatePartialBookingItem",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<SelectedProduct> GetFailedProducts(List<BookedProduct> bookedProducts,
            List<SelectedProduct> selectedProducts)
        {
            try
            {
                var failedProductIds = bookedProducts
                    .Where(x => x.OptionStatus == ((int)OptionBookingStatus.Failed).ToString())
                    .Select(x => x.AvailabilityReferenceId).ToList();

                var failedProducts = new List<SelectedProduct>();
                foreach (var failedProductId in failedProductIds)
                {
                    var product = selectedProducts.FirstOrDefault(x => x.AvailabilityReferenceId.Equals(failedProductId));
                    failedProducts.Add(product);
                }

                return failedProducts;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetFailedProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<SelectedProduct> GetBookedSelectedProduct(Booking booking)
        {
            try
            {
                var failedToBookActivityIds = new List<int>();
                foreach (var item in _failedSupplierProducts.Select(x => x.Value))
                {
                    failedToBookActivityIds.AddRange(item.Select(x => x.Id));
                }

                return booking.SelectedProducts.Where(y => !failedToBookActivityIds.Contains(y.Id)).ToList();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetBookedSelectedProduct",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// //Moulin Rouge Attachment
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="bookedProducts"></param>
        /// <returns></returns>
        private List<Attachment> MoulinRougeAttachment(Booking booking, List<BookedProduct> bookedProducts)
        {
            try
            {
                //Check any product in the list is MoulinRouge API then Pass the Attachments
                var moulinRougeAPIProduct =
                    booking.SelectedProducts.FindAll(product => product.APIType.Equals(APIType.Moulinrouge));
                var attachment = new List<Attachment>();
                if (moulinRougeAPIProduct != null && moulinRougeAPIProduct.Count > 0 && bookedProducts != null &&
                    bookedProducts.Count > 0)
                {
                    foreach (var item in bookedProducts)
                    {
                        if (item?.APIExtraDetail?.ConfirmedTicketAttachments != null &&
                            item.APIExtraDetail.ConfirmedTicketAttachments.Count > 0)
                        {
                            foreach (var attach in item.APIExtraDetail.ConfirmedTicketAttachments)
                            {
                                attachment.Add(attach);
                            }
                        }
                    }
                }

                return attachment;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "MoulinRougeAttachment",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #endregion API Booking Calls Private Methods

        /// <summary>
        /// This method returns list of API for which are allowed for creating booking.
        /// It has been implemented to restrict LIVE API Bookings on staging.
        /// </summary>
        /// <returns></returns>
        private bool IsBookingAllowed(APIType apiType)
        {
            bool result;
            try
            {
                var allowedAPITypeIDs = ConfigurationManagerHelper.GetValuefromAppSettings("AllowedAPITypeIDs");
                if (!string.IsNullOrWhiteSpace(allowedAPITypeIDs))
                {
                    var list = allowedAPITypeIDs.Split(',').ToList().Select(x => (APIType)Convert.ToInt32(x));
                    //Allow booking if a valid key is Found else block live bookings.
                    result = list.Contains(apiType);
                }
                else
                {
                    // If key has no values. then all products bookings are allowed
                    result = true;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "IsBookingAllowed",
                };
                _log.Error(isangoErrorEntity, ex);
                // If key not found then all products bookings are allowed
                result = true;
            }
            return result;
        }

        public string GenerateBookingRefNumber(string affiliateID, string currencyCode)
        {
            return _bookingPersistence.GenerateBookingRefNumber(affiliateID, currencyCode);
        }

        /// <summary>
        /// Get the Receive Detail for the given id
        /// </summary>
        /// <param name="id"></param>
        public ReceiveDetail GetReceiveDetail(int id)
        {
            try
            {
                var bookingData = _bookingPersistence.GetReceiveDetail(id);
                return bookingData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetReceiveDetail",
                    Params = $"{id}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Book Receive
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public BookingResult BookReceive(Booking booking, string token)
        {
            try
            {
                var bookingResponse = new BookingResult();

                #region Transaction booking

                if (booking.PaymentMethodType == PaymentMethodType.Transaction)
                {
                    var paymentGateway = _paymentGatewayFactory.GetPaymentGatewayService(booking.PaymentGateway);
                    var enrollmentResult = paymentGateway.EnrollmentCheck(booking, token);
                    if (enrollmentResult.Is2DBooking)
                    {
                        booking.Guwid = enrollmentResult.BookingReferenceId;
                    }
                    else if (enrollmentResult.EnrollmentErrorOrHTML.IndexOf("<html>", StringComparison.Ordinal) > -1)
                    {
                        bookingResponse.BookingStatus = BookingStatus.Requested;
                        bookingResponse.RequestHtml = enrollmentResult.EnrollmentErrorOrHTML;
                    }
                    else
                    {
                        bookingResponse.BookingStatus = BookingStatus.Failed;
                    }
                    if (enrollmentResult.BookingReferenceId != string.Empty)
                    {
                        bookingResponse.TransactionGuwid = enrollmentResult.BookingReferenceId;
                    }
                    return bookingResponse;
                }

                #endregion Transaction booking

                return bookingResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookReceive",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public BookingResult CreateReceive3DBooking(Booking booking, string token)
        {
            try
            {
                var bookingResponse = new BookingResult();
                var createBookingResponse = CreateReceiveBooking(booking, true, token);
                var isBookingSuccessful = createBookingResponse;
                bookingResponse.BookingRefNo = booking.ReferenceNumber;
                if (isBookingSuccessful.Item1)
                {
                    bookingResponse.BookingStatus = BookingStatus.Confirmed;
                    bookingResponse.StatusMessage = Constant.SuccessBookingMessage;
                }
                else if (isBookingSuccessful.Item3 != null && !string.IsNullOrEmpty(isBookingSuccessful.Item3.AcsRequest))
                {
                    bookingResponse.BookingStatus = BookingStatus.Requested;
                    bookingResponse.RequestHtml = createBookingResponse.Item3?.AcsRequest;
                    bookingResponse.IsWebhookReceived = createBookingResponse?.Item3?.IsWebhookReceived ?? false;
                    bookingResponse.TransactionGuwid = createBookingResponse.Item3?.TransactionID;
                    bookingResponse.StatusMessage = BookingStatus.Requested.ToString();
                }
                else
                {
                    bookingResponse.BookingStatus = BookingStatus.Failed;
                    bookingResponse.StatusMessage = isBookingSuccessful.Item2;
                }
                return bookingResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreateReceive3DBooking",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Create Receive Booking
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="isEnrollmentCheck"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Tuple<bool, string, AuthorizationResponse> CreateReceiveBooking(Booking booking, bool isEnrollmentCheck, string token)
        {
            var isBookingSuccess = false;
            var isPaymentAuthorizationSucceed = false;
            IPaymentService paymentGateway = null;
            var authorizationResponse = new AuthorizationResponse();
            Payment purchasePayment = null;
            bool isTransactionSuccess = false;
            Int32 purchaseTransactionId = 0;
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        if (booking.PaymentMethodType == PaymentMethodType.Transaction
                        && booking.Amount > 0
                        )
                        {
                            // Get Payment Gateway - WireCard or APEXX
                            paymentGateway = _paymentGatewayFactory.GetPaymentGatewayService(booking.PaymentGateway);
                            if (paymentGateway == null)
                            {
                                return new Tuple<bool, string, AuthorizationResponse>(isBookingSuccess, "Payment gateway is null", authorizationResponse);
                            }
                            //1.) 3DS Verify Call

                            bool is3D = true;
                            purchaseTransactionId = _bookingPersistence.UpdateIsangoBooking(booking.BookingId, is3D, ref purchasePayment, "", (int)booking.PaymentGateway);
                            booking.Payment.FirstOrDefault().TransactionId = Convert.ToString(purchaseTransactionId);
                            booking.Payment.FirstOrDefault().CurrencyCode = booking.Currency.IsoCode;
                            booking.Payment.FirstOrDefault().PaymentGatewayReferenceId = booking.Guwid;
                            authorizationResponse = paymentGateway.Authorization(booking, isEnrollmentCheck, token);
                            isPaymentAuthorizationSucceed = authorizationResponse.IsSuccess;
                            if (!authorizationResponse.IsSuccess)
                            {
                                if (!string.IsNullOrEmpty(authorizationResponse.AcsRequest) && !string.IsNullOrEmpty(authorizationResponse.TransactionID))
                                {
                                    return new Tuple<bool, string, AuthorizationResponse>(isBookingSuccess, "Requested", authorizationResponse);
                                }
                                return Tuple.Create(isBookingSuccess, "Authorization Fail", authorizationResponse);
                            }
                            //Using Guwid Now
                            //if(booking.PaymentGateway == PaymentGatewayType.Adyen)
                            //{
                            //    booking.AdyenPspReference = authorizationResponse.AdyenPsPReference;
                            //}

                            // 2.) Capture Call
                            booking.Payment.RemoveAll(x => x.PaymentStatus.Equals(PaymentStatus.Paid));
                            purchasePayment = GetPaymentDetailsOfBooking(booking, booking.Amount, PaymentStatus.Paid, booking.ReferenceNumber);
                            if (purchasePayment != null)
                            {
                                booking.Payment.Add(purchasePayment);
                                var transactionResponse = paymentGateway.Transaction(booking, isEnrollmentCheck, token);
                                isTransactionSuccess = transactionResponse.IsSuccess;
                            }
                            else
                            {
                                isTransactionSuccess = true;
                            }
                        }
                        else
                        {
                            isTransactionSuccess = false;
                        }

                        if (isTransactionSuccess == true)
                        {
                            purchasePayment.TransactionId = Convert.ToString(purchaseTransactionId);
                            //Data save in Database
                            _bookingPersistence.UpdateReceiveBookingTransaction(booking.Payment.Find(FilterPurchasePayment));
                            isBookingSuccess = true;
                        }
                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingService",
                            MethodName = "CreateReceiveBooking",
                            Token = token,
                            AffiliateId = booking.Affiliate.Id,
                            Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                        };
                        isBookingSuccess = false;
                        _log.Error(isangoErrorEntity, ex);
                        return new Tuple<bool, string, AuthorizationResponse>(false, ex.GetBaseException().Message, authorizationResponse);
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreateReceiveBooking",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                isBookingSuccess = false;
                _log.Error(isangoErrorEntity, ex);
                return new Tuple<bool, string, AuthorizationResponse>(false, ex.GetBaseException().Message, authorizationResponse);
            }
            //RollBack Transaction from APEXX/WireCard if any Authorization Fail
            finally
            {
                if (!isTransactionSuccess)
                {
                    if (isPaymentAuthorizationSucceed)
                    {
                        if (!string.IsNullOrEmpty(booking?.FallbackFingerPrint) && booking?.FallbackFingerPrint?.ToLower() == "sofort")
                        {
                            paymentGateway.RefundCapture(booking, "Booking failed", token);
                        }
                        else
                        {
                            paymentGateway.CancelAuthorization(booking, token);
                        }
                    }
                }
            }

            //Send Mail
            try
            {
                if (isTransactionSuccess)
                {
                    _mailerService.SendMail(booking.ReferenceNumber, null, false, true, null, null);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "CreateReceiveBooking",
                    Token = token,
                    AffiliateId = booking.Affiliate.Id,
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                return new Tuple<bool, string, AuthorizationResponse>(false, ex.GetBaseException().Message, authorizationResponse);
            }
            return Tuple.Create(isBookingSuccess, string.Empty, authorizationResponse);
        }

        /// <summary>
        /// BookRezdyProducts
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private List<BookedProduct> BookRezdyProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var rezdyLabel = _masterService.GetLabelDetailsAsync().GetAwaiter().GetResult();
                    var selectedProducts = criteria.Booking.SelectedProducts.Where(x => x.APIType.Equals(APIType.Rezdy)).ToList();
                    var supplierCurrency = selectedProducts?.FirstOrDefault()?.ProductOptions?.FirstOrDefault()?.BasePrice?.Currency?.IsoCode;
                    var customerCurrency = criteria?.Booking?.Currency?.IsoCode;
                    var factor = (_masterService.GetConversionFactorAsync(customerCurrency, supplierCurrency).GetAwaiter().GetResult());
                    for (var i = 0; i < selectedProducts.Count; i++)
                    {
                        var distinctProductOptionIds = criteria.SelectedProducts.Where(x => x.APIType.Equals(APIType.Rezdy)).Select(x => x.ProductOptions.Select(y => y.Id).Distinct().ToList()).ToList()[i];
                        var paxMapping = _masterService.GetPaxMappingsAsync().GetAwaiter().GetResult().Where(x => distinctProductOptionIds.Contains(x.ServiceOptionId)).ToList();
                        ((RezdySelectedProduct)selectedProducts[i]).PaxMappings = new List<RezdyPaxMapping>();
                        ((RezdySelectedProduct)selectedProducts[i]).PaxMappings.AddRange(paxMapping);
                        ((RezdySelectedProduct)selectedProducts[i]).RezdyLabelDetails = new List<RezdyLabelDetail>();
                        ((RezdySelectedProduct)selectedProducts[i]).RezdyLabelDetails.AddRange(rezdyLabel);
                        ((RezdySelectedProduct)selectedProducts[i]).ReferenceNumber = criteria?.Booking?.ReferenceNumber;
                        ((RezdySelectedProduct)selectedProducts[i]).Price = (((RezdySelectedProduct)selectedProducts[i]).Price) * factor;
                    }
                    var bookedProducts = _supplierBookingService.CreateRezdyBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Rezdy, failedProducts);
                    return bookedProducts;
                }
                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookRezdyProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public string GetReferenceNumberfromDB(string affiliateID, string currencyISO)
        {
            try
            {
                var RefNo = string.Empty;
                int count = 3;
                for (int i = 0; i < count; i++)
                {

                    Random random = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    //return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());

                    var randomString = new string(Enumerable.Range(1, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());  // it is better
                    RefNo = _bookingPersistence.BookingReferenceNumberfromDB(affiliateID, currencyISO, randomString);
                    if (!string.IsNullOrEmpty(RefNo))
                    {
                        break;
                    }
                }
                if (string.IsNullOrEmpty(RefNo))
                {
                    RefNo = _bookingPersistence.GenerateBookingRefNumber(affiliateID, currencyISO);
                }
                /*while (string.IsNullOrEmpty(RefNo))
                {
                    Random random = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    //return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());

                    var randomString = new string(Enumerable.Range(1, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());  // it is better

                    RefNo = _bookingPersistence.BookingReferenceNumberfromDB(affiliateID, currencyISO, randomString);
                }*/
                return RefNo;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetReferenceNumberfromDB",
                };
                _log.Error(isangoErrorEntity, ex);
                return string.Empty;
            }
        }

        public void ProcessAdyenWebhook(int flowName, string bookingReference, string status,
            string pspReference, string reason = "", bool? success = true)
        {
            try
            {
                Task.Run(() => AfterAdyenWebhookProcess(flowName, bookingReference, status, pspReference, reason, success));
            }
            catch (Exception ex)
            {
                //ignore
            }
            //var ReturnStatus = _adyenPersistence.GetAdyenWebhookRepsonse(bookingReference, flowName);
            //if (ReturnStatus.Item1.ToLower() != status.ToLower())
            //{
            //Task.Run(() => AfterAdyenWebhookProcess(flowName, bookingReference, status, pspReference, reason, success));
            //}
        }

        public void ProcessAdyenWebhookGeneratePaymentLink(string id = "", string pspReference = "")
        {
            var ReturnStatus = _adyenPersistence.UpdatePaymentLinkData(id, pspReference);
            if (!string.IsNullOrEmpty(ReturnStatus.Item1))
            {
                var customerEmail = ReturnStatus.Item1;
                var lang = ReturnStatus.Item2;
                var price = ReturnStatus.Item3 + " " + ReturnStatus.Item4;
                var temporaryRefNo = ReturnStatus.Item5;
                Task.Run(() => AfterAdyenWebhookProcessReceivedPayment(customerEmail, price, lang, temporaryRefNo));
            }
        }

        private void AfterAdyenWebhookProcess(int flowName, string bookingReference, string status, string pspReference, string reason, bool? success)
        {
            var IsCustomerMailSent = false;
            var IsSupplierMailSent = false;
            if (flowName != 5)
            {
                if (status.ToLower().Contains("fail") || success == false)
                {
                    //send Failure Mails to CS/Tech team
                    _mailerService.SendAdyenWebhookErrorMail(bookingReference, status, pspReference, reason);
                }
            }
            //else if (flowName == 4 )
            //{
            //IsCustomerMailSent = _mailerService.SendMailCustomer(bookingReference);
            //IsSupplierMailSent = _mailerService.SendSupplierMail(bookingReference);
            //}
            _adyenPersistence.UpdateWebhookStatusinDB(flowName, bookingReference, status, pspReference, reason, IsCustomerMailSent, IsSupplierMailSent, success);
        }

        private void AfterAdyenWebhookProcessReceivedPayment(string customerEmail, string price, string lang, string temporaryRefNo)
        {
            //mail send to customer and customer team
            _mailerService.SendAdyenReceivedLinkMail(customerEmail, price, lang, temporaryRefNo);
        }

        private List<BookedProduct> BookTourCMSActivity(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateTourCMSActivityBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.TourCMS, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookHBActivity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookGoCity(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateGoCityBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.GoCity, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookGoCity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private List<BookedProduct> BookRaynaProducts(ActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    var bookedProducts = _supplierBookingService.CreateRaynaBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.Rayna, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookRaynaProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        private List<BookedProduct> BookRedeamV12Products(CanocalizationActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    criteria.APIType = APIType.RedeamV12;
                    var bookedProducts = _icanocalizationService.CreateBooking(criteria);
                    var failedProducts = GetFailedProductsCanocalization(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.RedeamV12, failedProducts);
                    return bookedProducts;
                }

                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookAPIProducts",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        private List<SelectedProduct> GetFailedProductsCanocalization(List<BookedProduct> bookedProducts,
            List<SelectedProduct> selectedProducts)
        {
            try
            {
                var failedProductIds = bookedProducts
                    .Where(x => x.OptionStatus == ((int)OptionBookingStatus.Failed).ToString())
                    .Select(x => x.AvailabilityReferenceId).ToList();

                var failedProducts = new List<SelectedProduct>();
                foreach (var failedProductId in failedProductIds)
                {
                    var product = selectedProducts.FirstOrDefault(x => x.AvailabilityReferenceId.Equals(failedProductId));
                    failedProducts.Add(product);
                }

                return failedProducts;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GetFailedProductsCanocalization",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        private List<BookedProduct> BookGlobalTixV3Products(CanocalizationActivityBookingCriteria criteria)
        {
            try
            {
                if (criteria.SelectedProducts != null && criteria.SelectedProducts.Count > 0)
                {
                    criteria.APIType = APIType.GlobalTixV3;
                    var bookedProducts = _icanocalizationService.CreateBooking(criteria);
                    var failedProducts = GetFailedProducts(bookedProducts, criteria.SelectedProducts);
                    _failedSupplierProducts.Add(APIType.GlobalTixV3, failedProducts);
                    return bookedProducts;
                }
                return new List<BookedProduct>();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "BookGlobalTixV3Products",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public bool GeneratePaymentIsangoResponse(string countryCode
            , string shopperLocale, string amount, string currency, string emailLanguage, string customerEmail)
        {
            try
            {
                var paymentGateway = _paymentGatewayFactory.GetPaymentGatewayService(PaymentGatewayType.Adyen);
                //1. Step : Get payment Link from Adyen API
                var generatePaymentIsango = paymentGateway.GetAdyenGeneratePaymentLinksAsync(countryCode
                , shopperLocale, amount, currency).Result;
                //2.) Step2:  Insert Data into Database
                //if payment link generate from API, insert into Database
                if (!string.IsNullOrEmpty(generatePaymentIsango.Url))
                {
                    var generatePaymentLinkResponse = new Isango.Entities.Booking.RequestModels.GeneratePaymentLinkResponse();
                    generatePaymentLinkResponse = AssignDataGeneratePaymentLink(generatePaymentIsango);
                    generatePaymentLinkResponse.CustomerEmail = customerEmail;
                    generatePaymentLinkResponse.CustomerLanguage = emailLanguage;
                    if (currency.ToUpper() != "JPY ")
                    {
                        generatePaymentLinkResponse.Value = System.Convert.ToString(System.Convert.ToDecimal(generatePaymentLinkResponse.Value) / 100);
                    }

                    var tempBookingRefNumber = GetReferenceNumberfromDB("5BEEF089-3E4E-4F0F-9FBF-99BF1F350183", currency);
                    tempBookingRefNumber = tempBookingRefNumber?.Replace("SGI", "TPL")?.Replace("ISA", "TPL");
                    generatePaymentLinkResponse.TemporaryRefNo = tempBookingRefNumber;
                    _bookingPersistence.InsertGeneratePaymentLink(generatePaymentLinkResponse);

                    //3.)Step 3: Send Payment link in Mail to the Customer
                    _mailerService.SendAdyenGenerateLinkMail(customerEmail, generatePaymentIsango.Url, emailLanguage, tempBookingRefNumber);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingService",
                    MethodName = "GeneratePaymentIsangoResponse",
                };
                _log.Error(isangoErrorEntity, ex);
                return false;
            }
        }

        private Isango.Entities.Booking.RequestModels.GeneratePaymentLinkResponse AssignDataGeneratePaymentLink(GeneratePaymentIsangoResponse generatePayment)
        {
            var generatePaymentResponse = new Isango.Entities.Booking.RequestModels.GeneratePaymentLinkResponse
            {
                CountryCode = generatePayment.CountryCode,
                Currency = generatePayment.GeneratePaymentAmount.Currency,
                Description = generatePayment.Description,
                ExpiresAt = generatePayment.ExpiresAt,
                Id = generatePayment.Id,
                MerchantAccount = generatePayment.MerchantAccount,
                Reference = generatePayment.Reference,
                ShopperLocale = generatePayment.ShopperLocale,
                ShopperReference = generatePayment.ShopperReference,
                Url = generatePayment.Url,
                Value = generatePayment.GeneratePaymentAmount.Value
            };
            return generatePaymentResponse;
        }
    }
}