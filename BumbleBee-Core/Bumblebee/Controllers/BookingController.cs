using Isango.Entities;
using Isango.Entities.AdyenPayment;
using Isango.Entities.AdyenPayment.Adyen.Util;
using Isango.Entities.Affiliate;
using Isango.Entities.Booking;
using Isango.Entities.Booking.RequestModels;
using Isango.Entities.Enums;
using Isango.Entities.Ventrata;
using Isango.Persistence.Contract;
using Isango.Service;
using Isango.Service.Contract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceAdapters.PrioHub.PrioHub.Entities;
using ServiceAdapters.PrioHub.PrioHub.Entities.CreateBookingResponse;
using ServiceAdapters.PrioHub.PrioHub.Entities.ErrorRes;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models.Booking;
using Util;
using WebAPI.Filters;
using WebAPI.Helper;
using WebAPI.Mapper;
using WebAPI.Models;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;
using Constant = WebAPI.Constants.Constant;
using ILogger = Logger.Contract.ILogger;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Provide details of all Booking related endpoints
    /// </summary>
    [Route("api/booking")]
    [CustomActionWebApiFilter]
    [ApiController]
    public class BookingController : ApiBaseController
    {
        private readonly IProfileService _profileService;
        private readonly BookingMapper _bookingMapper;
        private readonly IBookingService _bookingService;
        private readonly ITableStorageOperation _tableStorageOperation;
        private readonly ILogger _log;
        private readonly BookingHelper _bookingHelper;
        private readonly CancellationHelper _cancelBookingHelper;

        private readonly CancellationMapper _cancellationMapper;
        private readonly ICancellationService _cancellationService;
        private readonly CancellationHelper _cancellationHelper;
        private readonly ActivityHelper _activityHelper;
        private Affiliate _isangoAffiliate;
        private string _defaultAffilaiteIsango;
        private string _defaultIsangoSupportEmail;
        private string _defaultIsangoSupportPhoneNumer;
        private string _citySightSeeingDefaultEmailForCreateBooking;
        private string _citySightSeeingDefaultPhoneForCreateBooking;
        private readonly IMasterPersistence _masterPersistence;
        private readonly IRedemptionService _redemptionservice;



        /// <summary>
        ///  Booking Controller
        /// </summary>
        /// <param name="profileService"></param>
        /// <param name="bookingMapper"></param>
        /// <param name="bookingService"></param>
        /// <param name="tableStorageOperation"></param>
        /// <param name="log"></param>
        /// <param name="bookingHelper"></param>
        /// <param name="cancelBookingHelper"></param>
        /// <param name="cancellationMapper"></param>
        /// <param name="cancellationService"></param>
        /// <param name="cancellationHelper"></param>
        /// <param name="activityHelper"></param>
        /// <param name="masterPersistence"></param>
        /// <param name="bookingPersistence"></param>
        public BookingController(IProfileService profileService
            , BookingMapper bookingMapper
            , IBookingService bookingService
            , ITableStorageOperation tableStorageOperation
            , ILogger log
            , BookingHelper bookingHelper
            , CancellationHelper cancelBookingHelper
            , CancellationMapper cancellationMapper
            , ICancellationService cancellationService
            , CancellationHelper cancellationHelper
            , ActivityHelper activityHelper
            , IMasterPersistence masterPersistence
            , IBookingPersistence bookingPersistence
            , IRedemptionService redemptionservice

           )
        {
            _profileService = profileService;
            _bookingMapper = bookingMapper;
            _bookingService = bookingService;
            _tableStorageOperation = tableStorageOperation;
            _bookingHelper = bookingHelper;
            _cancelBookingHelper = cancelBookingHelper;
            _log = log;
            _cancellationMapper = cancellationMapper;
            _cancellationService = cancellationService;
            _cancellationHelper = cancellationHelper;
            _activityHelper = activityHelper;
            _masterPersistence = masterPersistence;
            _redemptionservice = redemptionservice;


            try
            {
                var isangoAffiliateId = ConfigurationManagerHelper.GetValuefromAppSettings("IsangoAffiliateId");
                _isangoAffiliate = _activityHelper.GetAffiliateInfo(isangoAffiliateId);

                /*
                 *   <add key="CitySightSeeingDefaultEmailForCreateBooking" value="skumar@isango.com" />
    <add key="CitySightSeeingDefaultPhoneForCreateBooking" value="9910005922" />
                */
                _defaultAffilaiteIsango = ConfigurationManagerHelper.GetValuefromAppSettings("IsangoAffiliateId");
                _citySightSeeingDefaultEmailForCreateBooking = ConfigurationManagerHelper.GetValuefromAppSettings("CitySightSeeingDefaultEmailForCreateBooking");
                _citySightSeeingDefaultPhoneForCreateBooking = ConfigurationManagerHelper.GetValuefromAppSettings("CitySightSeeingDefaultPhoneForCreateBooking");

                _defaultIsangoSupportEmail = ConfigurationManagerHelper.GetValuefromAppSettings("mailfrom");
                _defaultIsangoSupportPhoneNumer = ConfigurationManagerHelper.GetValuefromAppSettings("SupportPhoneNumer");

            }
            catch
            {
            }
        }

        /// <summary>
        /// Operation is used to fetch user email preferences
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="affiliateId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("emailpreferences/{userId}/{affiliateId}/{languageCode}")]
        public IActionResult GetUserEmailPreferences(int userId, string affiliateId, string languageCode)
        {
            var userEmailPreferencesList =
                _profileService.FetchUserEmailPreferencesAsync(userId, affiliateId, languageCode).Result;
            return GetResponseWithActionResult(
                _bookingMapper.MapUserEmailPreferencesResponse(userEmailPreferencesList));
        }

        /// <summary>
        /// This operation is used for user to check their Upcoming, past and Canceled Booking.
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="affiliateId"></param>
        /// <param name="isAgent"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("userbooking/{emailId}/{affiliateId}/{isAgent}")]
        public IActionResult GetUserBookingDetails(string emailId, string affiliateId, bool isAgent)
        {
            var bookingDetails = _profileService.FetchUserBookingDetailsSitecoreAsync(emailId, affiliateId, isAgent)
                .GetAwaiter().GetResult();
            return GetResponseWithActionResult(bookingDetails);
        }

        /// <summary>
        /// This operation is used to create booking
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
       // [ValidateModel]
        [Route("create")]
       [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult CreateBooking(CreateBookingRequest request)
        {
            BookingResponse bookingResponse = null;
            var bookingGuid = string.Empty;
            var bookingReferenceNumber = string.Empty;
            var result = default(Tuple<BookingResult, Booking>);
            request = BookingRequestValidation(request);

            var error = new ErrorList();
            try
            {
                request.ActualIP = request.IPAddress;
                //if (request?.IPAddress?.Length > 15)
                //{
                //    request.IPAddress = _bookingMapper.ValidateAndGetIp(request.IPAddress);
                //}

                result = _bookingHelper.CreateBooking(request);

                if (result == null)
                {
                    return GetResponseWithActionResult(result.Item1);
                }
                if (result?.Item1?.Errors?.Any() == true)
                {
                    error.Errors.AddRange(result?.Item1?.Errors);
                }
                if (result?.Item2?.Errors?.Any() == true)
                {
                    var query = from e1 in error?.Errors
                                from e2 in result?.Item2?.Errors
                                where e1.Message != e2.Message
                                select e2;

                    if (query?.Any() == true)
                    {
                        error.Errors.AddRange(query.ToList());
                    }
                }

                var bookingResult = result.Item1;
                var booking = result.Item2;
                bookingReferenceNumber = booking.ReferenceNumber;

                BookingDetailResponse bookingDetail;
                if (bookingResult.IsDuplicateBooking)
                {
                    bookingDetail = GetBookingDetail(bookingResult.BookingRefNo);
                    bookingResponse = new BookingResponse
                    {
                        Message = bookingResult.StatusMessage,
                        Status = bookingResult.BookingStatus.ToString(),
                        ReferenceId = bookingResult.BookingRefNo,
                        BookingDetail = bookingDetail,
                        Errors = error.Errors
                    };
                    return GetResponseWithActionResult(bookingResponse);
                }

                Enum.TryParse(request.PaymentDetail.PaymentGateway, true, out PaymentGatewayType gatewayType);

                switch (gatewayType)
                {
                    case PaymentGatewayType.WireCard:
                    case PaymentGatewayType.Apexx:
                    case PaymentGatewayType.Adyen:
                        {
                            bookingGuid = bookingResult.TransactionGuwid;
                            break;
                        }
                    case PaymentGatewayType.Alternative:
                        {
                            bookingGuid = bookingResult.TransactionId;
                            break;
                        }

                    // ReSharper disable once RedundantEmptySwitchSection
                    default:
                        {
                            break; // throw new Exception("Unexpected Case");
                        }
                }
                bookingResponse = _bookingMapper.PrepareBookingResponse(bookingResult, bookingReferenceNumber,
                    request.PaymentDetail.PaymentGateway);


                //Passing gateway details null, if booking amount is less than or equal to 0 or Payment method type is Prepaid
                if (booking.Amount <= 0 || booking.PaymentMethodType == Isango.Entities.Payment.PaymentMethodType.Prepaid)
                    bookingResponse.GatewayDetail = null;
                request.PaymentDetail.CardDetails = null;
                bookingDetail = GetBookingDetail(bookingReferenceNumber);
                bookingResponse.BookingDetail = bookingDetail;
                if (!string.IsNullOrEmpty(bookingGuid))
                {
                    _tableStorageOperation.InsertBookingRequest(request, bookingGuid, bookingReferenceNumber, bookingResponse, string.Empty, Convert.ToString((int)HttpStatusCode.OK), request.TokenId);
                }
                else
                {
                    if (error?.Errors != null && error?.Errors.Count > 0)
                    {
                        bookingResponse.Errors = error?.Errors;
                        _tableStorageOperation.InsertBookingRequest(request, bookingGuid, bookingReferenceNumber, bookingResponse,
                            error?.Errors.FirstOrDefault().Code + ": " + error?.Errors?.FirstOrDefault()?.Message, Convert.ToString((int)HttpStatusCode.InternalServerError), request.TokenId);
                    }
                    else
                    {
                        _tableStorageOperation.InsertBookingRequest(request, bookingGuid, bookingReferenceNumber, bookingResponse, string.Empty, Convert.ToString((int)HttpStatusCode.OK), request.TokenId);

                    }
                }
            }
            catch (WebApiException ex)
            {
                _tableStorageOperation.InsertBookingRequest(request, bookingGuid, bookingReferenceNumber, bookingResponse, ex.Message, Convert.ToString((int)ex.StatusCode), request.TokenId);
                error.Errors.Add(new Error
                {
                    Code = CommonErrorCodes.BookingError.ToString(),
                    HttpStatus = System.Net.HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _tableStorageOperation.InsertBookingRequest(request, bookingGuid, bookingReferenceNumber, bookingResponse, ex.StackTrace, Convert.ToString((int)HttpStatusCode.InternalServerError), request.TokenId);
                error.Errors.Add(new Error
                {
                    Code = CommonErrorCodes.BookingError.ToString(),
                    HttpStatus = System.Net.HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });
            }
            if (bookingResponse == null)
            {
                bookingResponse = new BookingResponse
                {
                    Status = BookingStatus.Failed.ToString(),
                    Errors = new List<Error>()
                };
            }
            bookingResponse.Errors = error?.Errors;
            return GetResponseWithActionResult(bookingResponse);
        }

        /// <summary>
        /// This operation is used to create booking
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("create/callback")]
        [HttpPost]
        [ValidateModel]
        //[SwitchableAuthorization]
        public IActionResult CreateBookingCallback(BookingCallbackRequest request)
        {
            var bookingGuWid = string.Empty;
            BookingResponse bookingResponse = null;
            var bookingReferenceNumber = string.Empty;
            var error = new ErrorList();
            try
            {
                bookingGuWid = request.GuWid;
                //Get Data from BookingCallBackRequest and find status is start, completed or error
                //note: return value only if it is second request/thread otherwise null.
                var statusWithResponse = _tableStorageOperation.RetrieveBookingCallBackRequest(bookingGuWid);

                //(Case1: condition:completed/error - second hit or second thread)
                if (statusWithResponse != null && statusWithResponse?.Item1 == Constant.BookingCallBackCompleted || statusWithResponse?.Item1 == Constant.BookingCallBackError)
                {
                    return GetResponseWithActionResult(statusWithResponse.Item2);
                }
                //Case2: condition:start  - second hit or second thread
                if (statusWithResponse != null && statusWithResponse?.Item1 == Constant.BookingCallBackStart)
                {
                    var loopWait = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.DuplicateCallWaitTimeLoop);
                    var loopWaitTime = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.DuplicateCallWaitTime);
                    int loopCheckTime = !string.IsNullOrEmpty(loopWait) ? Convert.ToInt32(loopWait) : 2;
                    int loopCheckWitTime = !string.IsNullOrEmpty(loopWaitTime) ? Convert.ToInt32(loopWaitTime) : 5000;
                    for (int checkTime = 0; checkTime < loopCheckTime; checkTime++)// 2 times check by default
                    {
                        System.Threading.Thread.Sleep(loopCheckWitTime); //5 seconds by default
                        var isStatusWithResponse = _tableStorageOperation.RetrieveBookingCallBackRequest(bookingGuWid);
                        //(Case: completed/error - second hit or second thread)
                        if (isStatusWithResponse != null && isStatusWithResponse.Item1 == Constant.BookingCallBackCompleted || isStatusWithResponse.Item1 == Constant.BookingCallBackError)
                        {
                            return GetResponseWithActionResult(isStatusWithResponse.Item2);
                        }
                    }
                }

                var bookingRequestData = _tableStorageOperation.RetrieveBookingRequest(bookingGuWid);
                if (bookingRequestData == null) return GetResponseWithActionResult(bookingResponse);

                bookingReferenceNumber = bookingRequestData.Item1;
                var bookingRequest = bookingRequestData.Item2;

                //Insert and Update Data:Check for Duplicate Booking
                if (!string.IsNullOrEmpty(bookingGuWid))
                {
                    _tableStorageOperation.InsertBookingCallBackRequest(bookingGuWid, bookingReferenceNumber,
                        Constant.BookingCallBackStart, bookingResponse, request?.TokenId, string.Empty, Convert.ToString((int)HttpStatusCode.OK));
                }

                if (bookingRequest == null || !string.Equals(request.TokenId, bookingRequest.TokenId,
                        StringComparison.InvariantCultureIgnoreCase))
                    return GetResponseWithActionResult(bookingResponse);
                bookingRequest.PaymentDetail.CardDetails = request.CardDetails;
                var bookingResult = new BookingResult();
                var booking = default(Booking);
                //Condition for Receive Payment
                if (bookingRequest.SelectedProducts == null || bookingRequest.SelectedProducts.Count == 0)
                {
                    try
                    {
                        var receiveRequest = ((CreateReceiveBookingRequest)bookingRequest);
                        if (receiveRequest.AmendmentId > 0)
                        {
                            bookingResult = ReceiveCallBack(receiveRequest, request);
                            if (bookingResult == null) return GetResponseWithActionResult(bookingResponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "BookingController",
                            MethodName = "CreateBookingCallback",
                            Token = request.TokenId,
                            AffiliateId = bookingRequest.AffiliateId,
                            Params = $"{SerializeDeSerializeHelper.Serialize(bookingRequest)}"
                        };
                        _log.Error(isangoErrorEntity, ex);
                        throw;
                    }
                }
                else
                {
                    booking = _bookingMapper.PrepareBookingModel(bookingRequest, bookingReferenceNumber);
                    if (booking == null) return GetResponseWithActionResult(bookingResponse);
                    booking.ReferenceNumber = bookingReferenceNumber;
                    booking.PaRes = request.PaRes;
                    booking.Guwid = request.GuWid;
                    booking.AdyenPaymentData = request.AdyenPaymentData;
                    booking.FallbackFingerPrint = request.FallbackFingerPrint;

                    bookingResult = _bookingService.Create3DBooking(booking, bookingRequest.TokenId);
                    if (bookingResult == null) return GetResponseWithActionResult(bookingResponse);
                }

                if (bookingResult?.Errors?.Any() == true)
                {
                    error.Errors.AddRange(bookingResult?.Errors);
                }
                if (booking?.Errors?.Any() == true)
                {
                    error.Errors.AddRange(booking?.Errors);
                }
                bookingResponse = _bookingMapper.PrepareBookingResponse(bookingResult, bookingGuWid, bookingRequest.PaymentDetail.PaymentGateway);

                if (bookingResponse.Status != BookingStatus.Requested.ToString())
                {
                    bookingResponse.GatewayDetail = null;
                    request.CardDetails = null;
                }
                var bookingDetail = GetBookingDetail(bookingReferenceNumber);
                bookingResponse.BookingDetail = bookingDetail;

                //Insert and Update Data: Check for Duplicate Booking
                if (!string.IsNullOrEmpty(bookingGuWid) && bookingResult.BookingStatus != BookingStatus.Failed)
                {
                    _tableStorageOperation.InsertBookingCallBackRequest(bookingGuWid, bookingReferenceNumber,
                        Constant.BookingCallBackCompleted, bookingResponse, request?.TokenId, string.Empty, Convert.ToString((int)HttpStatusCode.OK));
                }
                else
                {
                    _tableStorageOperation.InsertBookingCallBackRequest(bookingGuWid, bookingReferenceNumber,
                        Constant.BookingCallBackError, bookingResponse, request?.TokenId,
                        error?.Errors.FirstOrDefault()?.Code + ": " + error?.Errors.FirstOrDefault()?.Message, Convert.ToString((int)HttpStatusCode.InternalServerError));
                }

                if (!string.IsNullOrEmpty(bookingResponse?.GatewayDetail?.Html))
                {
                    _tableStorageOperation.InsertBookingRequest(bookingRequest, bookingResult.TransactionGuwid, bookingReferenceNumber, bookingResponse, string.Empty, Convert.ToString((int)HttpStatusCode.OK), request?.TokenId);
                }
            }

            //catch (HttpResponseException ex)
            //{
            //    _tableStorageOperation.InsertBookingCallBackRequest(bookingGuWid, bookingReferenceNumber, Constant.BookingCallBackError, bookingResponse, request?.TokenId,
            //        ex.Response.ReasonPhrase, Convert.ToString((int)ex.Response.StatusCode));
            //    throw;
            //}
            catch (Exception ex)
            {
                _tableStorageOperation.InsertBookingCallBackRequest(bookingGuWid, bookingReferenceNumber, Constant.BookingCallBackError, bookingResponse, request?.TokenId,
                    ex.StackTrace, Convert.ToString((int)System.Net.HttpStatusCode.InternalServerError));
                throw;
            }
            bookingResponse.Errors = error?.Errors;
            return GetResponseWithActionResult(bookingResponse);
        }

        /// <summary>
        /// Receive CallBack Helper Method
        /// </summary>
        /// <param name="receiveRequest"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private BookingResult ReceiveCallBack(CreateReceiveBookingRequest receiveRequest, BookingCallbackRequest request)
        {
            var booking = _bookingMapper.PrepareReceiveBookingModel(receiveRequest);
            if (booking == null) return null;
            booking.PaRes = request.PaRes;
            booking.Guwid = request.GuWid;
            booking.AdyenPaymentData = request.AdyenPaymentData;
            var bookingResult = _bookingService.CreateReceive3DBooking(booking, request.TokenId);
            if (bookingResult == null)
            { return null; }
            return bookingResult;
        }

        [Route("consumertransactionfail")]
        [HttpGet]
        [ValidateModel]
        //[SwitchableAuthorization]
        public void ConsumerTransactionFail(string transactionId)
        {
            //HttpContext.Current.Request.Url.Authority
            using (var client = new HttpClient())
            {
                var postTask = client.GetAsync("https://" + HttpContext +
                                               "/api/booking/transactionfail/" + transactionId).Result;
            }
        }

        /// <summary>
        /// This operation is used when booking fails
        /// </summary>
        /// <param name="transactionId"></param>
        [Route("transactionfail/{transactionId}")]
        [HttpGet]
        [ValidateModel]
        //[SwitchableAuthorization]
        public void TransactionFail(string transactionId)
        {
            _log.Info($"BookingController|TransactionFail|{transactionId}");
            var bookingRequestData = _tableStorageOperation.RetrieveBookingRequest(transactionId);
            if (bookingRequestData == null) return;

            var bookingReferenceNumber = bookingRequestData?.Item1;
            var bookingRequest = bookingRequestData?.Item2;
            var booking = _bookingMapper.PrepareBookingModel(bookingRequest, bookingReferenceNumber);
            if (booking != null && string.IsNullOrWhiteSpace(booking.ErrorMessage))
            {
                booking.Guwid = transactionId;
                booking.ReferenceNumber = bookingReferenceNumber;
                _bookingService.ProcessTransactionFail(booking);
            }
        }

        /// <summary>
        /// /// This operation is used when booking successfully completed
        /// </summary>
        /// <param name="transactionId"></param>
        [Route("consumertransactionsuccess")]
        [HttpGet]
        [ValidateModel]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public void ConsumerTransactionSuccess(string transactionId)
        {
            var httpContext = HttpContext;
            using (var client = new HttpClient())
            {
                var postTask = client.GetAsync("https://" + $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}" +
                                               "/api/booking/transactionsuccess/" + transactionId).Result;
            }
        }

        /// <summary>
        /// This operation is used when booking successfully completed
        /// </summary>
        /// <param name="transactionId"></param>
        [Route("transactionsuccess/{transactionId}")]
        [HttpGet]
        [ValidateModel]
        //[SwitchableAuthorization]
        public void TransactionSuccess(string transactionId)
        {
            _log.Info($"BookingController|TransactionSuccess|{transactionId}");
            var bookingRequestData = _tableStorageOperation.RetrieveBookingRequest(transactionId);
            if (bookingRequestData == null) return;
            var bookingReferenceNumber = bookingRequestData.Item1;
            var bookingRequest = bookingRequestData.Item2;
            var booking = _bookingMapper.PrepareBookingModel(bookingRequest, bookingReferenceNumber);
            if (booking != null && string.IsNullOrWhiteSpace(booking.ErrorMessage))
            {
                booking.Guwid = transactionId;
                booking.ReferenceNumber = bookingReferenceNumber;
                _bookingService.ProcessTransactionSuccess(booking, true);
            }
        }

        /// <summary>
        /// This operation is called whenever an event is triggered from Alternative Payment.
        /// </summary>
        /// <returns></returns>
        [Route("transactionwebhookresponse")]
        [HttpPost]
        [ValidateModel]
        //[SwitchableAuthorization]
        public async Task<IActionResult> TransactionWebHookResponse()
        {
            var httpContext = HttpContext;
            using (var streamReader = new StreamReader(httpContext.Request.Body))
            {
                var jsonPostedData = await streamReader.ReadToEndAsync();
                var absoluteUri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}";
                _log.Info($"BookingController|TransactionWebHookResponse|{jsonPostedData}|{absoluteUri}");

                var transaction =
                    SerializeDeSerializeHelper.DeSerialize<AlternativeTransactionResponse>(jsonPostedData);
                var transactionId = transaction?.resource?.id;
                var bookingRequestData = _tableStorageOperation.RetrieveBookingRequest(transactionId);
                if (bookingRequestData == null) return Ok(); // Or return an appropriate response.

                var bookingReferenceNumber = bookingRequestData.Item1;
                var bookingRequest = bookingRequestData.Item2;
                var booking = _bookingMapper.PrepareBookingModel(bookingRequest, bookingReferenceNumber);
                booking.ReferenceNumber = bookingReferenceNumber;

                if (booking != null && string.IsNullOrWhiteSpace(booking.ErrorMessage))
                {
                    _bookingService.ProcessTransactionWebHook(booking, jsonPostedData, absoluteUri, bookingRequest.TokenId);
                }

                return Ok(); // Or return an appropriate response.
            }
        }

        /// <summary>
        /// This operation is called whenever booked with ADYEN.
        /// </summary>
        /// <returns></returns>
        [Route("adyenwebhookresponse")]
        [HttpPost]
        [ValidateModel]
        //[SwitchableAuthorization]
        public IActionResult AdyenWebhookResponse()
        {
            var WebhookReponse = new AdyenNotificationResponse();
            var httpContext = HttpContext;
            using (var streamReader = new System.IO.StreamReader(HttpContext.Request.Body))
            {
                var jsonPostedData = streamReader.ReadToEndAsync().GetAwaiter().GetResult();
                var absoluteUri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}"; _log.Info($"BookingController|AdyenWebhookResponse|{jsonPostedData}|{absoluteUri}");

                var transaction =
                    SerializeDeSerializeHelper.DeSerialize<AdyenWebhookResponse>(jsonPostedData);
                var AdyenPspReference = transaction?.NotificationItems?.FirstOrDefault().NotificationRequestItem?.OriginalReference;
                //Save the Response Paramas in DB.
                string hmacKey = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenHmacKey");
                // YOUR_HMAC_KEY from the Customer Area
                //string notificationRequestJson = "NOTIFICATION_REQUEST_JSON";

                var hmacValidator = new HmacValidator();
                var handleNotificationRequest = transaction;
                // Handling multiple notificationRequests
                List<NotificationItems> notificationRequestItemContainers = handleNotificationRequest.NotificationItems;
                foreach (var notificationRequestItemContainer in notificationRequestItemContainers)
                {
                    var notificationItem = notificationRequestItemContainer.NotificationRequestItem;
                    if (!notificationItem.EventCode.ToLower().Contains("report") && hmacValidator.IsValidHmac(notificationItem, hmacKey))
                    {
                        var eventCode = notificationItem.EventCode;
                        var bookingReference = notificationItem.MerchantReference;
                        var pspReference = notificationItem.OriginalReference;
                        var flowName = eventCode.ToLower();
                        var reason = notificationItem.Reason;
                        var success = notificationItem.Success;
                        // Process the notification based on the eventCode
                        if (flowName.Contains("authorisation"))
                        {
                            //Is payment receive from generate paymentLink
                            string paymentGenerateLinkId = notificationItem?.AdditionalData?.PaymentLinkId;
                            if (!string.IsNullOrEmpty(paymentGenerateLinkId) && notificationItem.Success)
                            {
                                var psp = notificationItem?.PspReference;
                                Task.Run(() => _bookingService.ProcessAdyenWebhookGeneratePaymentLink(paymentGenerateLinkId, psp));
                            }
                            else
                            {
                                Task.Run(() => _bookingService.ProcessAdyenWebhook(5, bookingReference,
                                    eventCode,
                                    pspReference, reason, success));
                            }
                        }
                        else if (flowName.Contains("capture"))
                        {
                            Task.Run(() => _bookingService.ProcessAdyenWebhook(4, bookingReference, eventCode, pspReference, reason, success));
                        }
                        else if (flowName.Contains("cancel"))
                        {
                            Task.Run(() => _bookingService.ProcessAdyenWebhook(3, bookingReference, eventCode, pspReference, reason, success));
                        }
                        else if (flowName.Contains("refund"))
                        {
                            Task.Run(() => _bookingService.ProcessAdyenWebhook(2, bookingReference, eventCode, pspReference, reason, success));
                        }
                    }
                    else
                    {
                        Task.Run(() => _bookingService.ProcessAdyenWebhook(0, string.Empty, notificationItem.EventCode, notificationItem?.OriginalReference ?? string.Empty, notificationItem.Reason ?? string.Empty, true));
                    }
                }
            }

            WebhookReponse.NotificationResponse = "[accepted]";
            return GetResponseWithActionResult(WebhookReponse);
        }

        [Route("adyenwebhookresponsenew")]
        [HttpPost]
        [ValidateModel]
        //[SwitchableAuthorization]
        public IActionResult AdyenWebhookResponseNew()
        {
            var WebhookReponse = new AdyenNotificationResponse();
            var httpContext = HttpContext;
            using (var streamReader = new System.IO.StreamReader(httpContext.Request.Body))
            {
                var jsonPostedData = streamReader.ReadToEndAsync().GetAwaiter().GetResult();
                var absoluteUri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}"; _log.Info($"BookingController|AdyenWebhookResponseNew|{jsonPostedData}|{absoluteUri}");

                var transaction =
                    SerializeDeSerializeHelper.DeSerialize<AdyenWebhookResponse>(jsonPostedData);
                var AdyenPspReference = transaction?.NotificationItems?.FirstOrDefault().NotificationRequestItem?.OriginalReference;
                //Save the Response Paramas in DB.
                string hmacKey = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenHmacKeyNew");
                // YOUR_HMAC_KEY from the Customer Area
                //string notificationRequestJson = "NOTIFICATION_REQUEST_JSON";

                var hmacValidator = new HmacValidator();
                var handleNotificationRequest = transaction;
                // Handling multiple notificationRequests
                List<NotificationItems> notificationRequestItemContainers = handleNotificationRequest.NotificationItems;
                foreach (var notificationRequestItemContainer in notificationRequestItemContainers)
                {
                    var notificationItem = notificationRequestItemContainer.NotificationRequestItem;
                    if (!notificationItem.EventCode.ToLower().Contains("report"))
                    {
                        var eventCode = notificationItem.EventCode;
                        var bookingReference = notificationItem.MerchantReference;
                        var pspReference = notificationItem.OriginalReference;
                        var flowName = eventCode.ToLower();
                        var reason = notificationItem.Reason;
                        var success = notificationItem.Success;
                        // Process the notification based on the eventCode
                        if (flowName.Contains("authorisation"))
                        {
                            //Is payment receive from generate paymentLink
                            string paymentGenerateLinkId = notificationItem?.AdditionalData?.PaymentLinkId;
                            if (!string.IsNullOrEmpty(paymentGenerateLinkId) && notificationItem.Success)
                            {
                                var psp = notificationItem.PspReference;
                                Task.Run(() => _bookingService.ProcessAdyenWebhookGeneratePaymentLink(paymentGenerateLinkId, psp));
                            }
                            else
                            {
                                Task.Run(() => _bookingService.ProcessAdyenWebhook(5, bookingReference?.Replace("0_", ""), eventCode, pspReference, reason, success));
                            }
                        }
                        else if (flowName.Contains("capture"))
                        {
                            Task.Run(() => _bookingService.ProcessAdyenWebhook(4, bookingReference?.Replace("0_", ""), eventCode, pspReference, reason, success));
                        }
                        else if (flowName.Contains("cancel"))
                        {
                            Task.Run(() => _bookingService.ProcessAdyenWebhook(3, bookingReference?.Replace("0_", ""), eventCode, pspReference, reason, success));
                        }
                        else if (flowName.Contains("refund"))
                        {
                            Task.Run(() => _bookingService.ProcessAdyenWebhook(2, bookingReference?.Replace("0_", ""), eventCode, pspReference, reason, success));
                        }
                    }
                    else
                    {
                        Task.Run(() => _bookingService.ProcessAdyenWebhook(0, string.Empty, notificationItem.EventCode, notificationItem?.OriginalReference ?? string.Empty, notificationItem.Reason ?? string.Empty, true));
                    }
                }
            }

            WebhookReponse.NotificationResponse = "[accepted]";
            return GetResponseWithActionResult(WebhookReponse);
        }

        /// <summary>
        /// Get the booking data for the given booking reference number
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns></returns>
        [Route("detail/{referenceNumber}")]
        [HttpGet]
        [ValidateModel]
        //[SwitchableAuthorization]
        public IActionResult GetBookingData(string referenceNumber)
        {
            var bookingDetailResponse = GetBookingDetail(referenceNumber);
            return GetResponseWithActionResult(bookingDetailResponse);
        }

        [Route("confirmbooking")]
        [HttpPost]
        [ValidateModel]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ConfirmBooking(ConfirmBookingRequest request)
        {
            var result = _bookingService.ConfirmBooking(request.BookedOptionId, request.UserId, request.TokenId, true);

            var confirmBookingReponse = new ConfirmBookingResponse
            {
                Status = result == null || !result.Item1 ? "Error" : "Success",
                Message = result?.Item2
            };
            return GetResponseWithActionResult(confirmBookingReponse);
        }

        [Route("partialrefund")]
        [HttpPost]
        [ValidateModel]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ProcessPartialRefund(PartialRefundRequest request)
        {
            var result = _bookingService.ProcessPartialRefund(request.AmendmentId, request.Remarks, request.ActionBy, request.TokenId);

            var confirmBookingReponse = new PartialRefundResponse
            {
                Status = result == null || !result.Item1 ? "Error" : "Success",
                Message = result?.Item2
            };
            return GetResponseWithActionResult(confirmBookingReponse);
        }

        /// <summary>
        /// Get the receive detail for the given amendmentid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("receivedetail/{id}")]
        [HttpGet]
        [ValidateModel]
        //[SwitchableAuthorization]
        public IActionResult GetReceiveDetail(string id)
        {
            ReceiveDetailResponse receiveDetailResponse = null;
            try
            {
                string amendmentInt = Util.SerializeDeSerializeHelper.Base64Decode(id);
                int amendmentId = Int32.Parse(string.IsNullOrWhiteSpace(amendmentInt) ? "-1" : amendmentInt);
                if (amendmentId > 0)
                {
                    var receiveResponse = _bookingService.GetReceiveDetail(amendmentId);
                    if (receiveResponse != null)
                        receiveDetailResponse = _bookingMapper.PrepareReceiveDetailResponse(receiveResponse);
                }
                else
                {
                    var confirmBookingReponse = new ReceiveResponse
                    {
                        Status = "Error",
                        Message = Constant.ReceiveNotValidLinkRetry
                    };

                    return GetResponseWithActionResult(confirmBookingReponse);
                }
            }
            catch (Exception ex)
            {
                var confirmBookingReponse = new ReceiveResponse
                {
                    Status = "Error",
                    Message = Constant.ReceiveSomeThingWentWrong
                };

                switch (ex.Message.ToLower())
                {
                    case Constant.ReceiveAmendmentExpire:
                        confirmBookingReponse.Message = Constant.ReceiveLinkExpired;
                        return GetResponseWithActionResult(confirmBookingReponse);

                    case Constant.ReceiveInvalidAmendmentId:
                        confirmBookingReponse.Message = Constant.ReceiveNotValidLink;
                        return GetResponseWithActionResult(confirmBookingReponse);

                    case Constant.ReceiveAlreadyReceived:
                        confirmBookingReponse.Message = Constant.ReceiveNotValidNow;
                        return GetResponseWithActionResult(confirmBookingReponse);

                    default:
                        return GetResponseWithActionResult(confirmBookingReponse);
                }
            }
            return GetResponseWithActionResult(receiveDetailResponse);
        }

        /// <summary>
        /// Post the receive detail for the given post data
        /// </summary>

        /// <returns></returns>
        [Route("createreceive")]
        [HttpPost]
        [ValidateModel]
        //[SwitchableAuthorization]
        public IActionResult CreateReceivePayment(CreateReceiveBookingRequest request)
        {
            ReceivePaymentResponse receivePaymentRes = null;
            //if (request?.IPAddress?.Length > 15)
            //{
            //    request.IPAddress = _bookingMapper.ValidateAndGetIp(request.IPAddress);
            //}
            BookingResponse bookingResponse = null;
            var result = _bookingHelper.CreateReceiveBooking(request);
            if (result == null) return GetResponseWithActionResult(receivePaymentRes);
            Enum.TryParse(request.PaymentDetail.PaymentGateway, true, out PaymentGatewayType gatewayType);
            var bookingGuid = string.Empty;
            switch (gatewayType)
            {
                case PaymentGatewayType.WireCard:
                case PaymentGatewayType.Apexx:
                case PaymentGatewayType.Adyen:
                    {
                        bookingGuid = result.TransactionGuwid;
                        break;
                    }
                case PaymentGatewayType.Alternative:
                    {
                        bookingGuid = result.TransactionId;
                        break;
                    }
                default:
                    {
                        break;// throw new Exception("Unexpected Case");
                    }
            }

            if (!string.IsNullOrEmpty(bookingGuid))
                _tableStorageOperation.InsertBookingRequest(request, bookingGuid, request?.BookingReferenceNumber);
            bookingResponse = _bookingMapper.PrepareBookingResponse(result, request.BookingReferenceNumber, request.PaymentDetail.PaymentGateway);
            return GetResponseWithActionResult(bookingResponse);
        }

        /// <summary>
        /// Created Private method to get booking detail to be used in create booking and create booking callback.
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns></returns>
        private BookingDetailResponse GetBookingDetail(string referenceNumber)
        {
            BookingDetailResponse bookingDetailResponse = null;
            try
            {
                var bookingData = _bookingService.GetBookingData(referenceNumber);
                if (bookingData.BookedOptions?.Count > 0)
                    bookingDetailResponse = _bookingMapper.PrepareBookingDetailResponse(bookingData);
            }
            catch (Exception)
            {
                bookingDetailResponse = null;
            }

            return bookingDetailResponse;
        }

        /// <summary>
        /// Get booking detail on the basis of booking reference number and user name
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Route("details/{referenceNumber}/userName/{userName}")]
        [HttpGet]
        [ValidateModel]
        public IActionResult GetBookingDetail(string referenceNumber, string userName)
        {
            var bookingDetailResponse = GetBookedOptionsDetail(referenceNumber, userName, null);
            return GetResponseWithActionResult(bookingDetailResponse);
        }

        /// <summary>
        /// Create private method to get booking options detail used by 'GetBookingDetails' method
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="userName"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        private BookingOptionsDetailsResponse GetBookedOptionsDetail(string referenceNumber, string userName,
            string statusId)
        {
            BookingOptionsDetailsResponse bookingOptionsDetailsResponse = null;
            var bookingData = _bookingService.GetBookingData(referenceNumber);
            var userData = _cancelBookingHelper.GetUserCancellationPermission(userName);
            if (userData == null)
            {
                return null;
            }
            var bookingOptionData = _bookingService.GetBookingDetailAsync(referenceNumber, userData.UserId, statusId)
                .GetAwaiter().GetResult();
            if (bookingData?.BookedOptions?.Count > 0 && bookingOptionData?.Count > 0)
                bookingOptionsDetailsResponse = _bookingMapper.PrepareBookingDetailResponse(bookingData, bookingOptionData);

            return bookingOptionsDetailsResponse;
        }

        /// <summary>
        /// Get booking detail on the basis of booking reference number and user name
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns></returns>
        [Route("bookingtimeout/{referenceNumber}")]
        [HttpGet]
        [ValidateModel]
        public IActionResult GetBookingResponse(string referenceNumber)
        {
            BookingResponse bookingResponse = new BookingResponse();
            bookingResponse.Status = Convert.ToString(BookingStatus.Confirmed);
            bookingResponse.BookingDetail = GetBookingDetail(referenceNumber); ;
            return GetResponseWithActionResult(bookingResponse);
        }

        /// <summary>
        /// This operation is called whenever booked with ADYEN.
        /// </summary>
        /// <returns></returns>
        [Route("createsubscription")]
        [HttpPost]
        public IActionResult Createsubscription()
        {
            logInfoSave("createsubscription", "Start");
            return null;
        }

        private void logInfoSave(string token, string dataPass, string logData = "")
        {
            var logInfo = dataPass;
            var isangoLogEntity = new IsangoErrorEntity
            {
                ClassName = "BookingController",
                MethodName = "Createsubscription",
                Token = token,
                Params = !String.IsNullOrEmpty(logData) ? (logInfo + "," + logData) : logInfo
            };
            _log.Info(isangoLogEntity);
        }

        [HttpPost]
        [ValidateModel]
        [Route("generatepaymentlink")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GeneratePaymentLink(GeneratePaymentLinkRequest request)
        {
            try
            {
                if (Convert.ToDecimal(request.Amount) > 0)
                {
                    request.CountryCode = "US";
                    request.ShopperLocale = "en_US";
                    if (request.Currency.ToUpper() != "JPY ")
                    {
                        request.Amount = Convert.ToString(Convert.ToDecimal(request.Amount) * 100);
                    }

                    var generatePaymentLinks = _bookingService.GeneratePaymentIsangoResponse
                        (request.CountryCode, request.ShopperLocale, request.Amount,
                        request.Currency, request.EmailLanguage, request.Email);
                    return Ok(new { resultData = "true" });
                }
                else
                {
                    return Ok(new { resultData = "false" });
                }
            }
            catch
            {
                return Ok(new { resultData = "false" });
            }
        }

        private CreateBookingRequest BookingRequestValidation(CreateBookingRequest createBookingRequest)
        {
            createBookingRequest.UserEmail = createBookingRequest?.UserEmail?.Trim();
            createBookingRequest.UserPhoneNumber = createBookingRequest?.UserPhoneNumber?.Replace(" ", string.Empty);
            createBookingRequest.PaymentDetail.UserFullName = createBookingRequest?.PaymentDetail?.UserFullName?.Replace("  ", " ")?.Trim();
            createBookingRequest.SelectedProducts?.ForEach(sp =>
            {
                sp?.PassengerDetails?.ForEach(p =>
                {
                    p.FirstName = p?.FirstName?.Trim();
                    p.LastName = p?.LastName?.Trim();
                });
            });
            return createBookingRequest;
        }

        /// <summary>
        /// Create and confirm booking.
        /// </summary>
        /// <param name="createBookingRequest"></param>
        /// <returns>A newly created booking</returns>
        /// <response code="200">OK</response>
        /// <response code="400">BAD_REQUEST</response>
        /// <response code="401">UNAUTHORIZED</response>
        /// <response code="403">FORBIDDEN</response>
        /// <response code="404">NOT_FOUND</response>
        /// <response code="500">INERTNAL_SERVER_ERROR</response>
        /// <response code="502">BAD_GATEWAY</response>
        /// <response code="503">SERVICE_UNAVAILABLE</response>
        /// <response code="504">GATEWAY_TIMEOUT</response>
        /// <remarks>
        /// Sample Requests:
        ///       POST
        ///
        ///       Request1 (minimal) :
        ///       {
        ///         &quot;SelectedProducts&quot;:
        ///         [
        ///           {
        ///               &quot;AvailabilityReferenceId&quot;: &quot;4fe97f10-69b7-488e-b22a-4f54b6650116&quot;,
        ///               &quot;PassengerDetails&quot;: [{
        ///                       &quot;FirstName&quot;: &quot;Sandeep1&quot;,
        ///                       &quot;LastName&quot;: &quot;Kumar&quot;,
        ///                       &quot;IsLeadPassenger&quot;: true,
        ///                       &quot;PassengerTypeId&quot;: 1
        ///                   }
        ///               ]
        ///           },
        ///           {
        ///               &quot;AvailabilityReferenceId&quot;: &quot;0ddff064-d298-4535-bb83-9667857c1e27&quot;,
        ///               &quot;PassengerDetails&quot;: [{
        ///                       &quot;FirstName&quot;: &quot;Sandeep1&quot;,
        ///                       &quot;LastName&quot;: &quot;Kumar&quot;,
        ///                       &quot;IsLeadPassenger&quot;: false,
        ///                       &quot;PassengerTypeId&quot;: 1
        ///                   }
        ///               ]
        ///           }
        ///         ]
        ///       }
        ///
        ///
        ///       Request2 (With all info in request.Provide your email Id to get supplier voucher on it, if not provided it would be picked from settings) :
        ///       {
        ///         &quot;UserEmail&quot;: &quot;skumar@isango.com&quot;,
        ///         &quot;TokenId&quot;: &quot;c001bb8e-67f5-475c-8ddf-85141b607f93&quot;,
        ///         &quot;UserPhoneNumber&quot;: &quot;9910005922&quot;,
        ///         &quot;CustomerAddress&quot;: {
        ///           &quot;Address&quot;: &quot;Avda Camino de Santiago 33A&quot;,
        ///           &quot;Town&quot;: &quot;Madrid&quot;,
        ///           &quot;PostCode&quot;: &quot;28050&quot;,
        ///           &quot;CountryIsoCode&quot;: &quot;ES&quot;,
        ///           &quot;CountryName&quot;: &quot;Espa&#241;a&quot;,
        ///           &quot;StateOrProvince&quot;: null
        ///         },
        ///         &quot;SelectedProducts&quot;: [{
        ///             &quot;AvailabilityReferenceId&quot;: &quot;3128a103-40d1-401a-a68a-34cb296adb05&quot;,
        ///             &quot;PassengerDetails&quot;: [{
        ///                 &quot;FirstName&quot;: &quot;Cristina&quot;,
        ///                 &quot;LastName&quot;: &quot;Mart&#237;nez&quot;,
        ///                 &quot;IsLeadPassenger&quot;: true,
        ///                 &quot;PassengerTypeId&quot;: 1,
        ///                 &quot;PassportNumber&quot;: &quot;999933344892&quot;,
        ///                  &quot;PassportNationality&quot;: &quot;Indian&quot;
        ///               }
        ///             ]
        ///           }, {
        ///             &quot;AvailabilityReferenceId&quot;: &quot;d7497022-5473-49ce-a545-9c165497d1c2&quot;,
        ///             &quot;PassengerDetails&quot;: [{
        ///                 &quot;FirstName&quot;: &quot;Cristina&quot;,
        ///                 &quot;LastName&quot;: &quot;Mart&#237;nez&quot;,
        ///                 &quot;IsLeadPassenger&quot;: false,
        ///                 &quot;PassengerTypeId&quot;: 1,
        ///                 &quot;PassportNumber&quot;: &quot;999933344892&quot;,
        ///                 &quot;PassportNationality&quot;: &quot;Indian&quot;
        ///               }
        ///             ]
        ///           }
        ///         ]
        ///       }
        ///
        ///
        ///       Sample Response:
        ///
        ///
        ///       {
        ///         &quot;Status&quot;: &quot;Confirmed&quot;,
        ///         &quot;ReferenceId&quot;: &quot;SGAZ16V5N&quot;,
        ///         &quot;Message&quot;: &quot;Booking Successful&quot;,
        ///         &quot;GatewayDetail&quot;: null,
        ///         &quot;BookingDetail&quot;: {
        ///         &quot;BookingReferenceNumber&quot;: &quot;SGAZ16V5N&quot;,
        ///         &quot;BookingDate&quot;: &quot;2022-08-31T00:00:00&quot;,
        ///         &quot;CustomerEmail&quot;: &quot;skumar@isango.com&quot;,
        ///         &quot;AffiliateId&quot;: &quot;58c11104-34e6-47ba-926d-e89e4242b962&quot;,
        ///         &quot;CurrencyIsoCode&quot;: &quot;EUR&quot;,
        ///         &quot;Language&quot;: &quot;en&quot;,
        ///         &quot;ProductDetails&quot;: [
        ///           {
        ///           &quot;LeadTravellerName&quot;: &quot;Cristina Mart&#237;nez&quot;,
        ///           &quot;ProductName&quot;: &quot;Louvre Museum Fast Track E-Ticket with Audio-Guided Tour&quot;,
        ///           &quot;Status&quot;: &quot;Confirmed&quot;,
        ///           &quot;IsReceipt&quot;: false,
        ///           &quot;IsShowSupplierVoucher&quot;: true,
        ///           &quot;TravelDate&quot;: &quot;2022-11-30T00:00:00&quot;,
        ///           &quot;SellAmount&quot;: 40,
        ///           &quot;MultiSaveAmount&quot;: 0,
        ///           &quot;DiscountAmount&quot;: 0,
        ///           &quot;Passengers&quot;: [
        ///             {
        ///             &quot;LinkType&quot;: null,
        ///             &quot;LinkValue&quot;: null,
        ///             &quot;AgeGroupDescription&quot;: &quot;Adult (18+)&quot;,
        ///             &quot;PaxCount&quot;: 2,
        ///             &quot;QRCodeValue&quot;: null
        ///             }
        ///           ],
        ///           &quot;IsQRCodePerPax&quot;: false,
        ///           &quot;LinkType&quot;: &quot;Link&quot;,
        ///           &quot;LinkValue&quot;: &quot;https://www.tiqets.com/en/checkout/IjcxYjVkM2U4LTA4ZGMtNDE3NS1hMTBiLTE1OTljYzRjMTI5MCI.DxjC0tjtCay5cWv-vxPWzTLkchs/voucher?source=api&quot;,
        ///           &quot;AvailabilityReferenceId&quot;: &quot;3128a103-40d1-401a-a68a-34cb296adb05&quot;,
        ///           &quot;BookedOptionId&quot;: 1389692,
        ///           &quot;BookedOptionName&quot;: &quot;Louvre Museum: E-Ticket (Entrance Only) @ 10:00:00&quot;
        ///           },
        ///           {
        ///           &quot;LeadTravellerName&quot;: &quot;Cristina Mart&#237;nez&quot;,
        ///           &quot;ProductName&quot;: &quot;Louvre Museum Fast Track E-Ticket with Audio-Guided Tour&quot;,
        ///           &quot;Status&quot;: &quot;Confirmed&quot;,
        ///           &quot;IsReceipt&quot;: false,
        ///           &quot;IsShowSupplierVoucher&quot;: true,
        ///           &quot;TravelDate&quot;: &quot;2022-11-30T00:00:00&quot;,
        ///           &quot;SellAmount&quot;: 40,
        ///           &quot;MultiSaveAmount&quot;: 0,
        ///           &quot;DiscountAmount&quot;: 0,
        ///           &quot;Passengers&quot;: [
        ///             {
        ///             &quot;LinkType&quot;: null,
        ///             &quot;LinkValue&quot;: null,
        ///             &quot;AgeGroupDescription&quot;: &quot;Adult (18+)&quot;,
        ///             &quot;PaxCount&quot;: 2,
        ///             &quot;QRCodeValue&quot;: null
        ///             }
        ///           ],
        ///           &quot;IsQRCodePerPax&quot;: false,
        ///           &quot;LinkType&quot;: &quot;Link&quot;,
        ///           &quot;LinkValue&quot;: &quot;https://www.tiqets.com/en/checkout/IjcxYjVkM2U4LTA4ZGMtNDE3NS1hMTBiLTE1OTljYzRjMTI5MCI.DxjC0tjtCay5cWv-vxPWzTLkchs/voucher?source=api&quot;,
        ///           &quot;AvailabilityReferenceId&quot;: &quot;d7497022-5473-49ce-a545-9c165497d1c2&quot;,
        ///           &quot;BookedOptionId&quot;: 1389693,
        ///           &quot;BookedOptionName&quot;: &quot;Louvre Museum: E-Ticket (Entrance Only) @ 10:30:00&quot;
        ///           }
        ///         ]
        ///         },
        ///         &quot;IsWebhookReceived&quot;: false,
        ///         &quot;Errors&quot;: []
        ///       }
        /// </remarks>
        [Route("v1/booking")]
        [HttpPost]
        [ValidateModel]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [SwaggerOperation(Tags = new[] { "City Sightseeing" })]
       //[SwaggerResponse(HttpStatusCode.OK, " Booking Detail Response", typeof(BookingDetailResponse))]
        public IActionResult CreateB2CBooking(B2C_BookingRequest createBookingRequest)
        {
            IActionResult result = null;
            var affiliate = default(Affiliate);

            try
            {
                var affiliateId = string.Empty;
                if (string.IsNullOrWhiteSpace(affiliateId))
                {
                    affiliateId = GetAffiliateFromIdentity();
                }
                if (string.IsNullOrWhiteSpace(affiliateId))
                {
                    return GetResponseWithActionResult(createBookingRequest, CommonErrorConstants.AffiliateNotFound, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    createBookingRequest.AffiliateId = affiliateId;
                }

                if (string.IsNullOrEmpty(createBookingRequest.TokenId))
                {
                    createBookingRequest.TokenId = affiliateId;
                }

                var validationResult = ValidateB2C_BookingRequest(createBookingRequest);
                if (validationResult != null)
                {
                    return validationResult;
                }
                var isangoBookingRequest = MapB2C_BookingRequest(createBookingRequest);
                result = this.CreateBooking(isangoBookingRequest);

            }
            catch (Exception ex)
            {
                return GetResponseWithActionResult(createBookingRequest, CommonErrorConstants.UNEXPECTED_ERROR, System.Net.HttpStatusCode.InternalServerError);
            }
            return result;
        }
        private string GetAffiliateFromIdentity()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                var affiliateId = userClaims.FirstOrDefault(o => o.Type == "affiliateId")?.Value;
                return affiliateId;
            }
            return null;
        }


        #region Booking Validation and mapping for CSS

        private IActionResult ValidateB2C_BookingRequest(B2C_BookingRequest createBookingRequest)
        {
            try
            {
                #region LanguageCode and CurrencyIsoCode Validation

                var languageCode = createBookingRequest?.LanguageCode;
                var currencyCode = createBookingRequest?.CurrencyIsoCode;

                if (!string.IsNullOrWhiteSpace(currencyCode) && currencyCode?.Length != 3)
                {
                    return GetResponseWithActionResult(createBookingRequest, "Please provide valid 3 characters CurrencyIsoCode.", System.Net.HttpStatusCode.BadRequest);
                }

                if (!string.IsNullOrWhiteSpace(languageCode) && languageCode?.Length != 2)
                {
                    return GetResponseWithActionResult(createBookingRequest, "Please provide valid 2 characters LanguageCode.", System.Net.HttpStatusCode.BadRequest);
                }

                #endregion LanguageCode and CurrencyIsoCode Validation

                createBookingRequest.SelectedProducts?.ForEach(sp =>
                {
                    sp?.PassengerDetails?.ForEach(p =>
                    {
                        p.FirstName = p?.FirstName?.Trim();
                        p.LastName = p?.LastName?.Trim();
                    });
                });

                var affiliate = _activityHelper.GetAffiliateInfo(createBookingRequest.AffiliateId);
                if (affiliate == null)
                {
                    return GetResponseWithActionResult(createBookingRequest, CommonErrorConstants.AffiliateNotFound, HttpStatusCode.BadRequest);
                }

                #region User Phone and Email

                if (string.IsNullOrWhiteSpace(createBookingRequest.UserEmail))
                {
                    createBookingRequest.UserEmail = _defaultIsangoSupportEmail;
                }
                else
                {
                    createBookingRequest.UserEmail = createBookingRequest?.UserEmail?.Trim();
                }
                bool isEmail = Regex.IsMatch(createBookingRequest.UserEmail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

                if (!isEmail)
                {
                    return GetResponseWithActionResult(createBookingRequest, "Please provide valid UserEmail.", System.Net.HttpStatusCode.BadRequest);
                }

                if (string.IsNullOrWhiteSpace(createBookingRequest.UserPhoneNumber))
                {
                    createBookingRequest.UserPhoneNumber = _defaultIsangoSupportPhoneNumer;
                }
                else
                {
                    createBookingRequest.UserPhoneNumber = createBookingRequest?.UserPhoneNumber?.Replace(" ", string.Empty);
                }
                if (string.IsNullOrEmpty(createBookingRequest.UserPhoneNumber)
                   || createBookingRequest.UserPhoneNumber?.Length < 8
                   )
                {
                    return GetResponseWithActionResult(createBookingRequest, "Please provide valid UserPhoneNumber.", System.Net.HttpStatusCode.BadRequest);
                }

                #endregion User Phone and Email

                var isValidGuid = Guid.TryParse(createBookingRequest.TokenId, out var tokenGuid);
                if (!isValidGuid && !string.IsNullOrWhiteSpace(createBookingRequest.TokenId))
                {
                    return GetResponseWithActionResult(createBookingRequest, "Please provide valid Guid for TokenId.", System.Net.HttpStatusCode.BadRequest);
                }

                if (createBookingRequest.SelectedProducts?.Any() != true)
                {
                    return GetResponseWithActionResult(createBookingRequest, "Please provide valid SelectedProducts.", System.Net.HttpStatusCode.BadRequest);
                }
                if (!string.IsNullOrWhiteSpace(createBookingRequest?.CustomerAddress?.CountryIsoCode)
                    && createBookingRequest?.CustomerAddress?.CountryIsoCode?.Length != 2)
                {
                    return GetResponseWithActionResult(createBookingRequest, "Please provide valid 2 characters CountryIsoCode in CustomerAddress .", System.Net.HttpStatusCode.BadRequest);
                }

                if (createBookingRequest.SelectedProducts?.Any(y => y.PassengerDetails?.Any(x => x.IsLeadPassenger) != true) == true)
                {
                    return GetResponseWithActionResult(createBookingRequest, "Please provide at least 1  IsLeadPassenger = true in PassengerDetails.", System.Net.HttpStatusCode.BadRequest);
                }

                foreach (var sp in createBookingRequest.SelectedProducts)
                {
                    isValidGuid = Guid.TryParse(sp.AvailabilityReferenceId, out var arefid);

                    if (!isValidGuid)
                    {
                        return GetResponseWithActionResult(createBookingRequest, "Please provide valid Guid for AvailabilityReferenceId.", System.Net.HttpStatusCode.BadRequest);
                    }

                    if (sp?.PassengerDetails?.Any() != true)
                    {
                        return GetResponseWithActionResult(createBookingRequest, "Please provide valid PassengerDetails.", System.Net.HttpStatusCode.BadRequest);
                    }

                    foreach (var p in sp?.PassengerDetails)
                    {
                        if (string.IsNullOrWhiteSpace(p.FirstName))
                        {
                            return GetResponseWithActionResult(createBookingRequest, "Please provide valid FirstName.", System.Net.HttpStatusCode.BadRequest);
                        }
                        if (string.IsNullOrWhiteSpace(p.LastName))
                        {
                            return GetResponseWithActionResult(createBookingRequest, "Please provide valid FirstName.", System.Net.HttpStatusCode.BadRequest);
                        }
                        var paxTypes = Enum.GetValues(typeof(PassengerType));
                        var isPaxTypeMatched = false;
                        foreach (var item in paxTypes)
                        {
                            if (p.PassengerTypeId == (Int32)item)
                            {
                                isPaxTypeMatched = true;
                                break;
                            }
                        }

                        if (!isPaxTypeMatched)
                        {
                            return GetResponseWithActionResult(createBookingRequest, "Please provide valid PassengerTypeId.", System.Net.HttpStatusCode.BadRequest);
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private CreateBookingRequest MapB2C_BookingRequest(B2C_BookingRequest createBookingRequest)
        {
            try
            {
                Isango.Entities.Booking.RequestModels.PassengerDetail leadPax = null;
                var emailId = createBookingRequest.UserEmail;
                var phoneNumber = createBookingRequest.UserPhoneNumber;
                var tokenId = string.IsNullOrEmpty(createBookingRequest.TokenId) ? createBookingRequest.AffiliateId : createBookingRequest.TokenId;
                var affiliateId = createBookingRequest.AffiliateId;
                var selectedProducts = new List<Isango.Entities.Booking.RequestModels.SelectedProduct>();
                var TiqetsLanguageCode = createBookingRequest?.LanguageCode;
                if (!string.IsNullOrEmpty(createBookingRequest?.LanguageCode))
                {
                    var languageCode = createBookingRequest?.LanguageCode;
                    languageCode = new[] { "en", "de", "es", "fr" }.Contains(languageCode) ? languageCode : "en";
                    createBookingRequest.LanguageCode = languageCode;
                }
                foreach (var b2cSelectedProduct in createBookingRequest.SelectedProducts)
                {
                    if (string.IsNullOrWhiteSpace(tokenId))
                    {
                        tokenId = createBookingRequest.TokenId = _masterPersistence.GetTokenByAvailabilityReferenceId(b2cSelectedProduct.AvailabilityReferenceId);
                    }
                    var sp = new Isango.Entities.Booking.RequestModels.SelectedProduct
                    {
                        AvailabilityReferenceId = b2cSelectedProduct.AvailabilityReferenceId,
                        PassengerDetails = new List<Isango.Entities.Booking.RequestModels.PassengerDetail>(),
                        PickupLocation = b2cSelectedProduct.PickupLocation ?? null,
                        SpecialRequest = b2cSelectedProduct.SpecialRequest ?? null,
                        PickupLocationId = b2cSelectedProduct.PickupLocationId ?? null,
                        DropOffLocation = b2cSelectedProduct.DropOffLocation ?? null,
                        DropOffLocationId = b2cSelectedProduct.DropOffLocationId ?? null
                    };
                    foreach (var b2cPassengerDetail in b2cSelectedProduct.PassengerDetails)
                    {
                        var passengerDetail = new Isango.Entities.Booking.RequestModels.PassengerDetail
                        {
                            FirstName = b2cPassengerDetail.FirstName,
                            LastName = b2cPassengerDetail.LastName,
                            IsLeadPassenger = b2cPassengerDetail.IsLeadPassenger,
                            PassengerTypeId = b2cPassengerDetail.PassengerTypeId,
                            PassportNumber = b2cPassengerDetail?.PassportNumber,
                            PassportNationality = b2cPassengerDetail?.PassportNationality
                        };
                        if (b2cPassengerDetail.IsLeadPassenger)
                        {
                            leadPax = passengerDetail;
                        }
                        sp.PassengerDetails.Add(passengerDetail);
                    }
                    if (b2cSelectedProduct?.Questions?.Count > 0)
                    {
                        sp.Questions = new List<Isango.Entities.Booking.RequestModels.Question>();
                        foreach (var question in b2cSelectedProduct?.Questions)
                        {
                            var ques = new Isango.Entities.Booking.RequestModels.Question()
                            {
                                Id = question?.Id,
                                Answer = question?.Answer,
                                IsRequired = question?.IsRequired ?? false,
                                Label = question?.Label,
                                QuestionType = question?.QuestionType
                            };
                            sp.Questions.Add(ques);
                        }
                    }
                    selectedProducts.Add(sp);
                }
                var result = new CreateBookingRequest
                {
                    AffiliateId = createBookingRequest.AffiliateId,
                    CurrencyIsoCode = createBookingRequest.CurrencyIsoCode,
                    BookingTime = DateTime.Now,
                    IsGuestUser = true,
                    LanguageCode = createBookingRequest.LanguageCode,
                    CustomerAddress =

                        new Isango.Entities.Booking.RequestModels.CustomerAddress
                        {
                            Address = createBookingRequest.CustomerAddress?.Address ?? string.Empty,
                            CountryIsoCode = createBookingRequest.CustomerAddress?.CountryIsoCode ?? string.Empty,
                            CountryName = createBookingRequest.CustomerAddress?.CountryName ?? string.Empty,
                            PostCode = createBookingRequest.CustomerAddress?.PostCode ?? string.Empty,
                            StateOrProvince = createBookingRequest.CustomerAddress?.StateOrProvince ?? string.Empty,
                            Town = createBookingRequest.CustomerAddress?.Town ?? string.Empty
                        },

                    PaymentDetail = new PaymentDetail
                    {
                        UserFullName = $"{leadPax.FirstName} {leadPax.LastName}",
                        //PaymentGateway = "0",
                        PaymentMethodType = "Prepaid"
                    },
                    SelectedProducts = selectedProducts,
                    TokenId = tokenId,
                    UserEmail = emailId,
                    UserPhoneNumber = phoneNumber,
                    UserLoginSource = "",
                    UTMParameter = "CitySightSeeing",
                    ExternalReferenceNumber = createBookingRequest.ExternalReferenceNumber,
                    SessionId = null,
                    OriginCountry = null,
                    OriginCity = null,
                    IPAddress = null,
                    DiscountCoupons = null,
                    BrowserInfo = null,
                    BookingAgent = null,
                    AgentID = null,
                    AgentEmailID = null,
                    ActualIP = null,
                    ClientDetail = null,
                    TiquetsLanguageCode = TiqetsLanguageCode
                };

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Booking Validation and mapping for CSS

        #region Get V1 Booking Details

        /// <summary>
        /// Get the booking data for the given booking reference number.
        /// </summary>
        /// <param name="bookingReferenceNumber"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">BAD_REQUEST</response>
        /// <response code="401">UNAUTHORIZED</response>
        /// <response code="403">FORBIDDEN</response>
        /// <response code="404">NOT_FOUND</response>
        /// <response code="500">INERTNAL_SERVER_ERROR</response>
        /// <response code="502">BAD_GATEWAY</response>
        /// <response code="503">SERVICE_UNAVAILABLE</response>
        /// <response code="504">GATEWAY_TIMEOUT</response>
        /// <remarks>
        /// Sample request:
        /// GET
        /// BookingReferenceNumber : SGIWKB39T
        ///
        /// Sample Response:
        ///
        /// {
        ///     &quot;Status&quot;: &quot;Requested&quot;,
        ///     &quot;ReferenceId&quot;: &quot;SGIWKB39T&quot;,
        ///     &quot;Message&quot;: &quot;Booking Successful&quot;,
        ///     &quot;GatewayDetail&quot;: null,
        ///     &quot;BookingDetail&quot;: {
        ///         &quot;BookingReferenceNumber&quot;: &quot;SGIWKB39T&quot;,
        ///         &quot;BookingDate&quot;: &quot;2022-08-29T00:00:00&quot;,
        ///         &quot;CustomerEmail&quot;: &quot;skumar@isango.com&quot;,
        ///         &quot;AffiliateId&quot;: &quot;f7feee76-7b67-4886-90ab-07488cb7a167&quot;,
        ///         &quot;CurrencyIsoCode&quot;: &quot;GBP&quot;,
        ///         &quot;Language&quot;: &quot;en&quot;,
        ///         &quot;ProductDetails&quot;: [{
        ///                 &quot;LeadTravellerName&quot;: &quot;Cristina Mart&#237;nez&quot;,
        ///                 &quot;ProductName&quot;: &quot;The Duomo di Milano, Museum &amp; Archaeological Area&quot;,
        ///                 &quot;Status&quot;: &quot;On Request&quot;,
        ///                 &quot;IsReceipt&quot;: false,
        ///                 &quot;IsShowSupplierVoucher&quot;: true,
        ///                 &quot;TravelDate&quot;: &quot;2022-06-03T00:00:00&quot;,
        ///                 &quot;SellAmount&quot;: 35.3,
        ///                 &quot;MultiSaveAmount&quot;: 0,
        ///                 &quot;DiscountAmount&quot;: 0,
        ///                 &quot;Passengers&quot;: [{
        ///                         &quot;LinkType&quot;: null,
        ///                         &quot;LinkValue&quot;: null,
        ///                         &quot;AgeGroupDescription&quot;: &quot;Adult (12+)&quot;,
        ///                         &quot;PaxCount&quot;: 3,
        ///                         &quot;QRCodeValue&quot;: null
        ///                     }
        ///                 ],
        ///                 &quot;IsQRCodePerPax&quot;: false,
        ///                 &quot;LinkType&quot;: &quot;Link&quot;,
        ///                 &quot;LinkValue&quot;: &quot;&quot;,
        ///                 &quot;AvailabilityReferenceId&quot;: &quot;0b78c1b6-2c02-4036-8682-18aec0bd7195&quot;,
        ///                 &quot;BookedOptionId&quot;: 1389688,
        ///                 &quot;BookedOptionName&quot;: &quot;The Duomo di Milano, Museum &amp; Archaeological Area @ 15:30:00&quot;
        ///             }, {
        ///                 &quot;LeadTravellerName&quot;: &quot;Cristina Mart&#237;nez&quot;,
        ///                 &quot;ProductName&quot;: &quot;The Duomo di Milano, Museum &amp; Archaeological Area&quot;,
        ///                 &quot;Status&quot;: &quot;On Request&quot;,
        ///                 &quot;IsReceipt&quot;: false,
        ///                 &quot;IsShowSupplierVoucher&quot;: true,
        ///                 &quot;TravelDate&quot;: &quot;2022-06-03T00:00:00&quot;,
        ///                 &quot;SellAmount&quot;: 35.3,
        ///                 &quot;MultiSaveAmount&quot;: 0,
        ///                 &quot;DiscountAmount&quot;: 0,
        ///                 &quot;Passengers&quot;: [{
        ///                         &quot;LinkType&quot;: null,
        ///                         &quot;LinkValue&quot;: null,
        ///                         &quot;AgeGroupDescription&quot;: &quot;Adult (12+)&quot;,
        ///                         &quot;PaxCount&quot;: 3,
        ///                         &quot;QRCodeValue&quot;: null
        ///                     }
        ///                 ],
        ///                 &quot;IsQRCodePerPax&quot;: false,
        ///                 &quot;LinkType&quot;: &quot;Link&quot;,
        ///                 &quot;LinkValue&quot;: &quot;&quot;,
        ///                 &quot;AvailabilityReferenceId&quot;: &quot;b7696a13-09e8-431f-97c7-096d8910e575&quot;,
        ///                 &quot;BookedOptionId&quot;: 1389689,
        ///                 &quot;BookedOptionName&quot;: &quot;The Duomo di Milano, Museum &amp; Archaeological Area @ 14:00:00&quot;
        ///             }
        ///         ]
        ///     },
        ///     &quot;IsWebhookReceived&quot;: false,
        ///     &quot;Errors&quot;: []
        /// }
        /// </remarks>
        [Route("v1/booking/{bookingReferenceNumber}")]
        [HttpGet]
        [ValidateModel]
        //[SwaggerResponse(HttpStatusCode.OK, "Booking Detail Response", typeof(BookingDetailResponse))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [SwaggerOperation(Tags = new[] { "City Sightseeing" })]
        public IActionResult GetBookingForCSS(string bookingReferenceNumber)
        {
            try
            {
                var bookingDetailResponse = GetBookingDetail(bookingReferenceNumber);
                if (bookingDetailResponse == null)
                {
                    return GetResponseWithActionResult(bookingReferenceNumber, "Booking Not Found", HttpStatusCode.NotFound);
                }
                var identity = User.Identity as ClaimsIdentity;
                var affiliateId = string.Empty;
                if (identity != null)
                {
                    affiliateId = identity?.FindFirst("affiliateId")?.Value;
                }
                if (string.IsNullOrWhiteSpace(affiliateId))
                {
                    return GetResponseWithActionResult(bookingReferenceNumber, CommonErrorConstants.AffiliateNotFound, System.Net.HttpStatusCode.BadRequest);
                }
                if (bookingDetailResponse.AffiliateId?.ToLower() == affiliateId?.ToLower())
                {
                    return GetResponseWithActionResult(bookingDetailResponse);
                }
                else
                {
                    return GetResponseWithActionResult(bookingReferenceNumber, "Not allowed to access this booking", HttpStatusCode.Unauthorized);
                }
            }
            catch (Exception ex)
            {
                return GetResponseWithActionResult(bookingReferenceNumber, CommonErrorConstants.UNEXPECTED_ERROR, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        #endregion Get V1 Booking Details

        #region Cancel V1 Booking

        /// <summary>
        /// Cancel booking for the given booking reference number.
        /// </summary>
        /// <param name="bookingReferenceNumber"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">BAD_REQUEST</response>
        /// <response code="401">UNAUTHORIZED</response>
        /// <response code="403">FORBIDDEN</response>
        /// <response code="404">NOT_FOUND</response>
        /// <response code="500">INERTNAL_SERVER_ERROR</response>
        /// <response code="502">BAD_GATEWAY</response>
        /// <response code="503">SERVICE_UNAVAILABLE</response>
        /// <response code="504">GATEWAY_TIMEOUT</response>
        /// <remarks>
        /// Sample request:
        /// GET
        /// BookingReferenceNumber : SGIWKB39T, SGAZ16V5N, SGAIJUKYK
        ///
        /// Sample Response:
        /// {
        ///   &quot;BookingDetail&quot;: {
        ///     &quot;BookingReferenceNumber&quot;: &quot;SGAIJUKYK&quot;,
        ///     &quot;BookingDate&quot;: &quot;2022-08-31T00:00:00&quot;,
        ///     &quot;CustomerEmail&quot;: &quot;skumar@isango.com&quot;,
        ///     &quot;AffiliateId&quot;: &quot;58c11104-34e6-47ba-926d-e89e4242b962&quot;,
        ///     &quot;CurrencyIsoCode&quot;: &quot;EUR&quot;,
        ///     &quot;Language&quot;: &quot;en&quot;,
        ///     &quot;ProductDetails&quot;: [
        ///       {
        ///         &quot;LeadTravellerName&quot;: &quot;Cristina1 Mart&#237;nez&quot;,
        ///         &quot;ProductName&quot;: &quot;Louvre Museum Fast Track E-Ticket with Audio-Guided Tour&quot;,
        ///         &quot;Status&quot;: &quot;Cancelled&quot;,
        ///         &quot;IsReceipt&quot;: false,
        ///         &quot;IsShowSupplierVoucher&quot;: true,
        ///         &quot;TravelDate&quot;: &quot;2022-11-30T00:00:00&quot;,
        ///         &quot;SellAmount&quot;: 40,
        ///         &quot;MultiSaveAmount&quot;: 0,
        ///         &quot;DiscountAmount&quot;: 0,
        ///         &quot;Passengers&quot;: [
        ///           {
        ///             &quot;LinkType&quot;: null,
        ///             &quot;LinkValue&quot;: null,
        ///             &quot;AgeGroupDescription&quot;: &quot;Adult (18+)&quot;,
        ///             &quot;PaxCount&quot;: 2,
        ///             &quot;QRCodeValue&quot;: null
        ///           }
        ///         ],
        ///         &quot;IsQRCodePerPax&quot;: false,
        ///         &quot;LinkType&quot;: &quot;Link&quot;,
        ///         &quot;LinkValue&quot;: &quot;&quot;,
        ///         &quot;AvailabilityReferenceId&quot;: &quot;3128a103-40d1-401a-a68a-34cb296adb05&quot;,
        ///         &quot;BookedOptionId&quot;: 1389694,
        ///         &quot;BookedOptionName&quot;: &quot;Louvre Museum: E-Ticket (Entrance Only) @ 10:00:00&quot;
        ///       },
        ///       {
        ///         &quot;LeadTravellerName&quot;: &quot;Cristina1 Mart&#237;nez1&quot;,
        ///         &quot;ProductName&quot;: &quot;Louvre Museum Fast Track E-Ticket with Audio-Guided Tour&quot;,
        ///         &quot;Status&quot;: &quot;Cancelled&quot;,
        ///         &quot;IsReceipt&quot;: false,
        ///         &quot;IsShowSupplierVoucher&quot;: true,
        ///         &quot;TravelDate&quot;: &quot;2022-11-30T00:00:00&quot;,
        ///         &quot;SellAmount&quot;: 40,
        ///         &quot;MultiSaveAmount&quot;: 0,
        ///         &quot;DiscountAmount&quot;: 0,
        ///         &quot;Passengers&quot;: [
        ///           {
        ///             &quot;LinkType&quot;: null,
        ///             &quot;LinkValue&quot;: null,
        ///             &quot;AgeGroupDescription&quot;: &quot;Adult (18+)&quot;,
        ///             &quot;PaxCount&quot;: 2,
        ///             &quot;QRCodeValue&quot;: null
        ///           }
        ///         ],
        ///         &quot;IsQRCodePerPax&quot;: false,
        ///         &quot;LinkType&quot;: &quot;Link&quot;,
        ///         &quot;LinkValue&quot;: &quot;&quot;,
        ///         &quot;AvailabilityReferenceId&quot;: &quot;d7497022-5473-49ce-a545-9c165497d1c2&quot;,
        ///         &quot;BookedOptionId&quot;: 1389695,
        ///         &quot;BookedOptionName&quot;: &quot;Louvre Museum: E-Ticket (Entrance Only) @ 10:30:00&quot;
        ///       }
        ///     ]
        ///   },
        ///   &quot;CancellationStatus&quot;: [
        ///     &quot;SGAIJUKYK-1389694-Cancelled&quot;,
        ///     &quot;SGAIJUKYK-1389695-Cancelled&quot;
        ///   ]
        /// }
        /// </remarks>
        [Route("v1/booking/{bookingReferenceNumber}")]
        [HttpDelete]
        [ValidateModel]
       // [SwaggerResponse(HttpStatusCode.OK, "Booking Detail ResponseWith Cancellation Status", typeof(List<string>))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [SwaggerOperation(Tags = new[] { "City Sightseeing" })]
        public IActionResult CancelBookingForCSS(string bookingReferenceNumber)
        {
            var bookingDetailResponse = default(BookingDetailResponse);
            var cancellationRes = new List<string>(); ;
            try
            {
                bookingDetailResponse = GetBookingDetail(bookingReferenceNumber);
                if (bookingDetailResponse == null)
                {
                    return GetResponseWithActionResult(bookingReferenceNumber, "Booking Not Found", HttpStatusCode.NotFound);
                }
                if (bookingDetailResponse.ProductDetails.Any(x => x.TravelDate < DateTime.Now))
                {
                    return GetResponseWithActionResult(bookingReferenceNumber, "Travel Date Expired", HttpStatusCode.InternalServerError);
                }
                var identity = User.Identity as ClaimsIdentity;
                var affiliateId = string.Empty;
                var userName = string.Empty;
                if (identity != null)
                {
                    affiliateId = identity?.FindFirst("affiliateId")?.Value;
                    userName = User.Identity.Name;
                }
                if (string.IsNullOrWhiteSpace(affiliateId))
                {
                    return GetResponseWithActionResult(bookingReferenceNumber, CommonErrorConstants.AffiliateNotFound, System.Net.HttpStatusCode.BadRequest);
                }
                if (bookingDetailResponse.AffiliateId?.ToLower() != affiliateId?.ToLower())
                {
                    return GetResponseWithActionResult(bookingReferenceNumber, "Not allowed to access this booking", HttpStatusCode.Unauthorized);
                }

                var cancellationController = new CancellationController(_cancellationService, _cancellationMapper, _cancelBookingHelper, _bookingService, _log);
                var cancellationPolicyDetails = new Dictionary<int, CancellationPolicyDetailResponse>();

                foreach (var productDetail in bookingDetailResponse.ProductDetails)
                {
                    var cancellationPolicyDetailResponse = cancellationController.GetCancellationPolicyForBookingOption(bookingReferenceNumber, productDetail.BookedOptionId);

                    if (cancellationPolicyDetailResponse != null)
                    {
                        cancellationPolicyDetails.Add(productDetail.BookedOptionId, cancellationPolicyDetailResponse);

                        var cancellationRequest = new CancellationRequest
                        {
                            BookingRefNo = bookingReferenceNumber,
                            CancellationParameters = new CancellationParameters
                            {
                                BookedOptionId = productDetail.BookedOptionId,
                                Reason = "Cancellation through API",
                                UserRefundAmount = cancellationPolicyDetailResponse.UserRefundAmount
                            },
                            IsBookingManager = false,
                            UserName = userName
                        };

                        //CancellationResponse
                        var resIActionResult = cancellationController.CancelBooking(cancellationRequest);
                        var okResult = resIActionResult as ObjectResult;
                        var resCancellationResponse = okResult?.Value as WebAPI.Models.ResponseModels.CancellationResponse;
                        if (resCancellationResponse != null)
                        {
                            string contentResult = $"{bookingReferenceNumber}-{productDetail.BookedOptionId}-{resCancellationResponse.Status?.Message}";
                            cancellationRes.Add(contentResult);
                        }
                    }
                    else
                    {
                        string contentResult = $"{bookingReferenceNumber}-{productDetail.BookedOptionId}-{productDetail.Status}";
                        cancellationRes.Add(contentResult);
                    }
                }
            }
            catch (Exception ex)
            {
                return GetResponseWithActionResult(bookingReferenceNumber, CommonErrorConstants.UNEXPECTED_ERROR, System.Net.HttpStatusCode.InternalServerError);
            }
            bookingDetailResponse = GetBookingDetail(bookingReferenceNumber);

            return GetResponseWithActionResult(
                          //new BookingDetailResponseWithCancellationStatus
                          //{
                          //    BookingDetail = bookingDetailResponse,
                          //    CancellationStatus =
                          cancellationRes
                //}
                );
        }

        #endregion Cancel V1 Booking

        #region V1 Booking Reservations

        /// <summary>
        /// This operation is used to create reservation for the booking
        /// </summary>
        /// <param name="createBookingRequest"></param>
        /// <returns>A newly created booking reservation</returns>
        /// <response code="200">OK</response>
        /// <response code="400">BAD_REQUEST</response>
        /// <response code="401">UNAUTHORIZED</response>
        /// <response code="403">FORBIDDEN</response>
        /// <response code="404">NOT_FOUND</response>
        /// <response code="500">INERTNAL_SERVER_ERROR</response>
        /// <response code="502">BAD_GATEWAY</response>
        /// <response code="503">SERVICE_UNAVAILABLE</response>
        /// <response code="504">GATEWAY_TIMEOUT</response>
        /// <remarks>
        /// Sample Requests:
        ///       POST
        ///
        ///       Request1 (minimal) :
        ///       {
        ///         &quot;SelectedProducts&quot;:
        ///         [
        ///           {
        ///               &quot;AvailabilityReferenceId&quot;: &quot;4fe97f10-69b7-488e-b22a-4f54b6650116&quot;,
        ///               &quot;PassengerDetails&quot;: [{
        ///                       &quot;FirstName&quot;: &quot;Sandeep&quot;,
        ///                       &quot;LastName&quot;: &quot;Kumar&quot;,
        ///                       &quot;IsLeadPassenger&quot;: true,
        ///                       &quot;PassengerTypeId&quot;: 1
        ///                   }
        ///               ]
        ///           },
        ///           {
        ///               &quot;AvailabilityReferenceId&quot;: &quot;0ddff064-d298-4535-bb83-9667857c1e27&quot;,
        ///               &quot;PassengerDetails&quot;: [{
        ///                       &quot;FirstName&quot;: &quot;Sandeep&quot;,
        ///                       &quot;LastName&quot;: &quot;Kumar&quot;,
        ///                       &quot;IsLeadPassenger&quot;: false,
        ///                       &quot;PassengerTypeId&quot;: 1
        ///                   }
        ///               ]
        ///           }
        ///         ]
        ///       }
        ///
        ///
        ///       Request2 (With all info in request.Provide your email Id to get supplier voucher on it, if not provided it would be picked from settings) :
        ///       {
        ///         &quot;UserEmail&quot;: &quot;skumar@isango.com&quot;,
        ///         &quot;TokenId&quot;: &quot;c001bb8e-67f5-475c-8ddf-85141b607f93&quot;,
        ///         &quot;UserPhoneNumber&quot;: &quot;9910005922&quot;,
        ///         &quot;CustomerAddress&quot;: {
        ///           &quot;Address&quot;: &quot;Avda Camino de Santiago 33A&quot;,
        ///           &quot;Town&quot;: &quot;Madrid&quot;,
        ///           &quot;PostCode&quot;: &quot;28050&quot;,
        ///           &quot;CountryIsoCode&quot;: &quot;ES&quot;,
        ///           &quot;CountryName&quot;: &quot;Espa&#241;a&quot;,
        ///           &quot;StateOrProvince&quot;: null
        ///         },
        ///         &quot;SelectedProducts&quot;: [{
        ///             &quot;AvailabilityReferenceId&quot;: &quot;3128a103-40d1-401a-a68a-34cb296adb05&quot;,
        ///             &quot;PassengerDetails&quot;: [{
        ///                 &quot;FirstName&quot;: &quot;Cristina&quot;,
        ///                 &quot;LastName&quot;: &quot;Mart&#237;nez&quot;,
        ///                 &quot;IsLeadPassenger&quot;: true,
        ///                 &quot;PassengerTypeId&quot;: 1
        ///               }
        ///             ]
        ///           }, {
        ///             &quot;AvailabilityReferenceId&quot;: &quot;d7497022-5473-49ce-a545-9c165497d1c2&quot;,
        ///             &quot;PassengerDetails&quot;: [{
        ///                 &quot;FirstName&quot;: &quot;Cristina&quot;,
        ///                 &quot;LastName&quot;: &quot;Mart&#237;nez&quot;,
        ///                 &quot;IsLeadPassenger&quot;: false,
        ///                 &quot;PassengerTypeId&quot;: 1
        ///               }
        ///             ]
        ///           }
        ///         ]
        ///       }
        ///
        ///
        /// Sample Response:
        ///
        /// {
        ///   &quot;BookingReferenceNumber&quot;: &quot;SGAUO6KBU&quot;,
        ///   &quot;Success&quot;: true,
        ///   &quot;ExpirationTimeInMinutes&quot;: 30
        ///   &quot;Products&quot;: [
        ///     {
        ///       &quot;AvailabilityReferenceID&quot;: &quot;4fe97f10-69b7-488e-b22a-4f54b6650116&quot;,
        ///     },
        ///     {
        ///       &quot;AvailabilityReferenceID&quot;: &quot;0ddff064-d298-4535-bb83-9667857c1e27&quot;,
        ///     }
        ///   ]
        /// }
        /// </remarks>
        [HttpPost]
        [ValidateModel]
        [Route("v1/reservation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [SwaggerOperation(Tags = new[] { "City Sightseeing" })]
        //[SwaggerResponse(HttpStatusCode.OK, "Create Booking Reservation Response", typeof(ReservationResponse))]
        public IActionResult CreateBookingReservation(B2C_BookingRequest createBookingRequest)
        {
            var affiliate = default(Affiliate);

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                var affiliateId = createBookingRequest.AffiliateId;
                if (string.IsNullOrWhiteSpace(affiliateId))
                {
                    if (identity != null)
                    {
                        affiliateId = identity?.FindFirst("affiliateId")?.Value;
                    }
                }
                if (string.IsNullOrWhiteSpace(affiliateId))
                {
                    return GetResponseWithActionResult(createBookingRequest, CommonErrorConstants.AffiliateNotFound, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    createBookingRequest.AffiliateId = affiliateId;
                }

                var validationResult = ValidateB2C_BookingRequest(createBookingRequest);
                if (validationResult != null)
                {
                    return validationResult;
                }
                var request = MapB2C_BookingRequest(createBookingRequest);

                var reservationResponse = new ReservationResponse()
                {
                    Success = false
                };
                var bookingGuid = string.Empty;
                var bookingReferenceNumber = string.Empty;
                var result = default(Tuple<ReservationResponse, Booking>);
                request = BookingRequestValidation(request);
                request.IsReservation = true;
                var error = new ErrorList();
                try
                {
                    request.ActualIP = request.IPAddress;
                    //if (request?.IPAddress?.Length > 15)
                    //{
                    //    request.IPAddress = _bookingMapper.ValidateAndGetIp(request.IPAddress);
                    //}

                    result = _bookingHelper.CreateBookingReservation(request);

                    if (result == null)
                    {
                        return GetResponseWithActionResult(result.Item1);
                    }
                    if (result?.Item1?.Errors?.Any() == true)
                    {
                        error.Errors.AddRange(result?.Item1?.Errors);
                    }
                    if (result?.Item2?.Errors?.Any() == true)
                    {
                        var query = from e1 in error?.Errors
                                    from e2 in result?.Item2?.Errors
                                    where e1.Message != e2.Message
                                    select e2;

                        if (query?.Any() == true)
                        {
                            error.Errors.AddRange(query.ToList());
                        }
                    }

                    reservationResponse = result.Item1;
                    var booking = result.Item2;
                    bookingReferenceNumber = booking.ReferenceNumber;

                    BookingDetailResponse bookingDetail;

                    request.PaymentDetail.CardDetails = null;
                    bookingDetail = GetBookingDetail(bookingReferenceNumber);

                    if (!string.IsNullOrEmpty(bookingGuid))
                    {
                        _tableStorageOperation.InsertReservationRequest(request, bookingGuid, bookingReferenceNumber, reservationResponse, string.Empty, Convert.ToString((int)HttpStatusCode.OK), request.TokenId);
                    }
                    else
                    {
                        if (error?.Errors != null && error?.Errors.Count > 0)
                        {
                            reservationResponse.Errors = error?.Errors;
                            _tableStorageOperation.InsertReservationRequest(request, bookingGuid, bookingReferenceNumber, reservationResponse,
                                error?.Errors.FirstOrDefault().Code + ": " + error?.Errors?.FirstOrDefault()?.Message, Convert.ToString((int)HttpStatusCode.InternalServerError), request.TokenId);
                        }
                    }
                }

                catch (WebApiException ex)
                {
                    _tableStorageOperation.InsertReservationRequest(request, bookingGuid, bookingReferenceNumber, reservationResponse, ex.Message, Convert.ToString((int)ex.StatusCode), request.TokenId);
                    error.Errors.Add(new Error
                    {
                        Code = CommonErrorCodes.BookingError.ToString(),
                        HttpStatus = System.Net.HttpStatusCode.InternalServerError,
                        Message = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    _tableStorageOperation.InsertReservationRequest(request, bookingGuid, bookingReferenceNumber, reservationResponse, ex.StackTrace, Convert.ToString((int)HttpStatusCode.InternalServerError), request.TokenId);
                    error.Errors.Add(new Error
                    {
                        Code = CommonErrorCodes.BookingError.ToString(),
                        HttpStatus = System.Net.HttpStatusCode.InternalServerError,
                        Message = ex.Message
                    });
                }
                if (reservationResponse == null)
                {
                    reservationResponse = new ReservationResponse
                    {
                        Success = false,
                        Errors = new List<Error>()
                    };
                }
                reservationResponse.Errors = error?.Errors;
                if (reservationResponse.Errors?.Count > 0)
                {
                    var error1st = reservationResponse.Errors?.FirstOrDefault();
                    return GetResponseWithActionResult(createBookingRequest, error1st.Message, error1st.HttpStatus);
                }
                return GetResponseWithActionResult(reservationResponse);
            }
            catch (Exception ex)
            {
                return GetResponseWithActionResult(createBookingRequest, CommonErrorConstants.UNEXPECTED_ERROR, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// This operation is used to cancel booking reservation.
        /// </summary>
        /// <param name="bookingReferenceNumber">booking Reference Number , All booked options under this reference will be canceled</param>
        /// <returns></returns>
        /// /// <returns>A newly created booking reservation</returns>
        /// <response code="200">OK</response>
        /// <response code="400">BAD_REQUEST</response>
        /// <response code="401">UNAUTHORIZED</response>
        /// <response code="403">FORBIDDEN</response>
        /// <response code="404">NOT_FOUND</response>
        /// <response code="500">INERTNAL_SERVER_ERROR</response>
        /// <response code="502">BAD_GATEWAY</response>
        /// <response code="503">SERVICE_UNAVAILABLE</response>
        /// <response code="504">GATEWAY_TIMEOUT</response>
        /// <remarks>
        /// Sample Requests:
        /// DELETE SGAUO6KBU
        ///
        /// Sample Response:
        ///
        /// {
        ///   &quot;BookingReferenceNumber&quot;: &quot;SGAUO6KBU&quot;,
        ///   &quot;Success&quot;: true,
        ///   &quot;ExpirationTimeInMinutes&quot;: 30
        ///   &quot;Products&quot;: [
        ///     {
        ///       &quot;AvailabilityReferenceID&quot;: &quot;4fe97f10-69b7-488e-b22a-4f54b6650116&quot;,
        ///       &quot;Status&quot;: &quot;Success&quot;
        ///     },
        ///     {
        ///       &quot;AvailabilityReferenceID&quot;: &quot;0ddff064-d298-4535-bb83-9667857c1e27&quot;,
        ///       &quot;Status&quot;: &quot;Success&quot;
        ///     }
        ///   ]
        /// }
        /// </remarks>
        [HttpDelete]
        [ValidateModel]
        [Route("v1/reservation/bookingReferenceNumber")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [SwaggerOperation(Tags = new[] { "City Sightseeing" })]
        //[SwaggerResponse(HttpStatusCode.OK, "Create Booking Reservation Response", typeof(ReservationResponse))]
        public IActionResult CancelBookingReservation(string bookingReferenceNumber)
        {
            var reservationResponse = new ReservationResponse()
            {
                Success = false
            };
            var bookingGuid = string.Empty;
            var result = default(Tuple<ReservationResponse, Booking>);

            var error = new ErrorList();
            var cancelRequest = new CancelReservationRequest
            {
                BookingReferenceNumber = bookingReferenceNumber
            };
            try
            {
                //if (request?.IPAddress?.Length > 15)
                //{
                //    request.IPAddress = _bookingMapper.ValidateAndGetIp(request.IPAddress);
                //}

                result = _bookingHelper.CancelBookingReservation(cancelRequest);

                if (result == null)
                {
                    return GetResponseWithActionResult(result.Item1);
                }
                if (result?.Item1?.Errors?.Any() == true)
                {
                    error.Errors.AddRange(result?.Item1?.Errors);
                }
                if (result?.Item2?.Errors?.Any() == true)
                {
                    var query = from e1 in error?.Errors
                                from e2 in result?.Item2?.Errors
                                where e1.Message != e2.Message
                                select e2;

                    if (query?.Any() == true)
                    {
                        error.Errors.AddRange(query.ToList());
                    }
                }

                reservationResponse = result.Item1;
            }
            catch (WebApiException ex)
            {
                error.Errors.Add(new Error
                {
                    Code = CommonErrorCodes.BookingError.ToString(),
                    HttpStatus = System.Net.HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                error.Errors.Add(new Error
                {
                    Code = CommonErrorCodes.BookingError.ToString(),
                    HttpStatus = System.Net.HttpStatusCode.InternalServerError,
                    Message = ex.Message
                });
            }
            if (reservationResponse == null)
            {
                reservationResponse = new ReservationResponse
                {
                    Success = false,
                    Errors = new List<Error>()
                };
            }
            reservationResponse.Errors = error?.Errors;
            if (reservationResponse.Errors?.Count > 0)
            {
                var error1st = reservationResponse.Errors?.FirstOrDefault();
                return GetResponseWithActionResult(cancelRequest, error1st.Message, error1st.HttpStatus);
            }
            return GetResponseWithActionResult(reservationResponse);
        }

        #endregion V1 Booking Reservations


        [Route("priohubwebhookresponse")]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        [ValidateModel]
        public IActionResult PrioHubWebhookResponse()
        {
            //Webhook handle both booking and cancellation.
            var WebhookReponse = new AdyenNotificationResponse();
            var httpContext = HttpContext;
            using (var streamReader = new System.IO.StreamReader(httpContext.Request.Body))
            {
                var jsonPostedData = streamReader.ReadToEndAsync().GetAwaiter().GetResult();
                var absoluteUri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}";
                //var absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;
                _log.Info($"BookingController|PrioHubWebhookResponse|{jsonPostedData}|{absoluteUri}");
                var createBookingRs = new CreateBookingResponse();
                var createBookingRsError = new ErrorRes();
                if (string.IsNullOrEmpty(jsonPostedData) || jsonPostedData.Contains("error"))
                //according to priohub, they not sent any error webhook response, they sent email
                //only error message, no details to save in storage
                {
                    if (!string.IsNullOrEmpty(jsonPostedData))
                    {
                        createBookingRsError = SerializeDeSerializeHelper.DeSerialize<ErrorRes>(jsonPostedData?.ToString());
                    }
                }
                else
                {
                    var finalAsyncBooking = new AsyncBooking();
                    createBookingRs = SerializeDeSerializeHelper.DeSerialize<CreateBookingResponse>(jsonPostedData?.ToString());
                    var supplierorderReference = createBookingRs?.Data?.Order?.OrderReference;
                    var supplierExternalorderReference = createBookingRs?.Data?.Order?.OrderBookings?.FirstOrDefault()?.BookingExternalReference;
                    //supplierorderReference is unique for every booking, so update asynbooking storage with it.
                    //it return from storage 1.) booking data 2)cancel data if any
                    var asyncBookings = _tableStorageOperation.RetrieveAsyncBookingRowKey((int)APIType.PrioHub, "RowKey", supplierorderReference);
                    var cancelData = _tableStorageOperation.RetrieveAsyncBookingRowKey((int)APIType.PrioHub, "RowKey", supplierorderReference + supplierExternalorderReference);
                    if (cancelData != null && cancelData.Count > 0)
                    {
                        asyncBookings.AddRange(cancelData);
                    }
                    //if asyncBookings have no data means, it is confirmed product and not pending product
                    //we only run webhook for PENDING scenario's 
                    if (asyncBookings != null && asyncBookings.Count > 0)
                    {
                        //check, Is it cancel data or booking data:
                        var apiBookingStatus = createBookingRs?.Data?.Order?.OrderStatus; //ORDER_CONFIRMED
                        var apiBookingStatusInner = createBookingRs?.Data?.Order?.OrderBookings?.FirstOrDefault()?.BookingStatus; //BOOKING_PROCESSING_CANCELLATION
                        var webhookStatus = "0";
                        //filter data:booking or cancellation
                        if (apiBookingStatus?.ToUpper() == ConstantPrioHub.ORDERCANCELLED ||
                            apiBookingStatusInner?.ToUpper() == ConstantPrioHub.BOOKINGPROCESSINGCANCELLATION || apiBookingStatusInner?.ToUpper() == ConstantPrioHub.BOOKINGCANCELLED
                           )
                        {
                            finalAsyncBooking = asyncBookings?.Where(x => x.ApiTypeMethod?.ToUpper() == ConstantPrioHub.CANCELWEBHOOK)?.FirstOrDefault();
                        }
                        else//,BOOKING_PROCESSING_CONFIRMATION//BOOKING_CONFIRMED
                        {
                            finalAsyncBooking = asyncBookings?.Where(x => x.ApiTypeMethod?.ToUpper() == ConstantPrioHub.BOOKING)?.FirstOrDefault();
                        }
                        if (finalAsyncBooking != null)
                        {
                            var webHookRetryCount = Convert.ToInt32(finalAsyncBooking.WebhookRetryCount) + 1;
                            if (finalAsyncBooking.IsWebhookSuccess == null || finalAsyncBooking.IsWebhookSuccess == "0")
                            {
                                if (apiBookingStatus?.ToUpper() == ConstantPrioHub.ORDERCONFIRMED || apiBookingStatus?.ToUpper() == ConstantPrioHub.ORDERCANCELLED ||
                                    apiBookingStatusInner?.ToUpper() == ConstantPrioHub.BOOKINGCONFIRMED || apiBookingStatusInner?.ToUpper() == ConstantPrioHub.BOOKINGCANCELLED
                                   )
                                {
                                    webhookStatus = "1";
                                }
                                _tableStorageOperation.UpdatePrioHubWebhookData(jsonPostedData, webHookRetryCount, absoluteUri, finalAsyncBooking, webhookStatus);
                            }
                            WebhookReponse.NotificationResponse = "[accepted]";
                        }
                    }
                }
            }
            return GetResponseWithActionResult(WebhookReponse);
        }

        [Route("VentrataRedempWebhookResponse")]
        [HttpPost]
        [HttpGet]
        [HttpDelete]
        [ValidateModel]
        public IActionResult VentrataRedempWebhookResponse()
        {
            bool isredemptiondone = false;
            var WebhookReponse = new VentrataRedemption();
            var httpContext = HttpContext;
            using (var streamReader = new System.IO.StreamReader(httpContext.Request.Body))
            {
                var jsonPostedData = streamReader.ReadToEndAsync().GetAwaiter().GetResult();
                var absoluteUri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}";
                _log.Info($"BookingController|VentrataRedempWebhookResponse|{jsonPostedData}|{absoluteUri}");
                var createBookingRsError = new ErrorRes();
                if (string.IsNullOrEmpty(jsonPostedData) || jsonPostedData.Contains("error"))
                {
                    if (!string.IsNullOrEmpty(jsonPostedData))
                    {
                        createBookingRsError = SerializeDeSerializeHelper.DeSerialize<ErrorRes>(jsonPostedData?.ToString());
                        isredemptiondone = false;
                    }
                    else
                    {
                        WebhookReponse = SerializeDeSerializeHelper.DeSerialize<VentrataRedemption>(jsonPostedData?.ToString());
                        isredemptiondone = _redemptionservice.VentrataRedemptionService(WebhookReponse);

                    }
                }
            }
            return GetResponseWithActionResult(isredemptiondone);
        }
    }
}