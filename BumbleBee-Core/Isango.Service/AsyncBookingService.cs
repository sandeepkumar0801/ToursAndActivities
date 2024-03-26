using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Enums;
using Isango.Entities.PrioHub;
using Isango.Entities.Tiqets;
using Isango.Entities.v1Css;
using Isango.Mailer.Contract;
using Isango.Mailer.ServiceContracts;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.PrioHub;
using Newtonsoft.Json;
using ServiceAdapters.Tiqets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models;
using TableStorageOperations.Models.Booking;
using Util;
using ServiceAdapters.PrioHub.PrioHub.Entities.CreateBookingResponse;
using ServiceAdapters.PrioHub.PrioHub.Entities;

namespace Isango.Service
{
    public class AsyncBookingService : IAsyncBookingService
    {
        private readonly ITiqetsAdapter _tiqetsAdapter;
        private readonly IPrioHubAdapter _prioHubAdapter;
        private readonly ILogger _log;
        private readonly ITableStorageOperation _tableStorageOperation;
        private readonly IMailer _mailer;
        private readonly IMailerService _mailerService;
        private readonly IBookingPersistence _bookingPersistence;
        private readonly IBookingService _bookingService;
        private readonly string _b2bUserKey;
        private readonly IMailAttachmentService _mailAttachmentService;
        private readonly IAffiliateService _affiliateService;
        private readonly ICancellationService _cancellationService;

        public AsyncBookingService(ITiqetsAdapter tiqetsAdapter
            , ILogger log
            , IMailer mailer
            , ITableStorageOperation tableStorageOperation
            , IMailerService mailerService
            , IBookingService bookingService
            , IBookingPersistence bookingPersistence
            , IMailAttachmentService mailAttachmentService
            , IAffiliateService affiliateService
            , IPrioHubAdapter prioHubAdapter
            , ICancellationService cancellationService
        )
        {
            _tiqetsAdapter = tiqetsAdapter;
            _log = log;
            _mailer = mailer;
            _tableStorageOperation = tableStorageOperation;
            _mailerService = mailerService;
            _bookingService = bookingService;
            _bookingPersistence = bookingPersistence;
            _mailAttachmentService = mailAttachmentService;
            _affiliateService = affiliateService;
            _cancellationService = cancellationService;

            _prioHubAdapter = prioHubAdapter;
            _b2bUserKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.B2BUserKey);
            if (string.IsNullOrWhiteSpace(_b2bUserKey))
            {
                _b2bUserKey = "987c62ef-1df2-450b-b5eb-044efd357275";
            }
        }

        public void ProcessIncompleteBooking()
        {
            try
            {
                var asyncBookings = _tableStorageOperation.RetrieveAsyncBooking((int)APIType.Tiqets, "Status", "Success");

                var apiWiseGroupedData = asyncBookings.GroupBy(x => x.ApiType).ToDictionary(e => (APIType)e.Key, e => e.ToList());
                foreach (var apiWiseData in apiWiseGroupedData)
                {
                    switch (apiWiseData.Key)
                    {
                        case APIType.Tiqets:
                            RetrieveTiqetsBookingTicket(apiWiseData.Value);
                            break;
                        case APIType.PrioHub:
                            RetrievePrioHub(apiWiseData.Value);
                            break;
                        default:
                            break;
                    }
                }
                try
                {
                    SendTiqetsWebhook();
                }
                catch (Exception ex)
                {
                    //ignore
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "ProcessIncompleteBooking",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private void SendTiqetsWebhook()
        {
            try
            {
                var asyncBookings = _tableStorageOperation.RetrieveFailedWebhookAsyncBooking((int)APIType.Tiqets, "IsWebhookSuccess", "0");

                foreach (var tiqetsAsyncBooking in asyncBookings)
                {
                    try
                    {
                        if (tiqetsAsyncBooking.IsWebhookSuccess != "0")
                        {
                            continue;
                        }
                        var tiqetsTableStorageData = new TiqetsTableStorageData
                        {
                            OrderReferenceId = tiqetsAsyncBooking.OrderReferenceId,
                            BookingReferenceNo = tiqetsAsyncBooking.BookingReferenceNo,
                            ApiType = tiqetsAsyncBooking.ApiType,
                            LanguageCode = tiqetsAsyncBooking.LanguageCode,
                            Interval = tiqetsAsyncBooking.RetryInterval,
                            RetryThreshold = tiqetsAsyncBooking.RetryThreshold,
                            Token = tiqetsAsyncBooking.Token,
                            CustomerEmail = tiqetsAsyncBooking.CustomerEmail,
                            AvailabilityReferenceId = tiqetsAsyncBooking.AvailabilityReferenceId,
                            OptionName = tiqetsAsyncBooking.OptionName,
                            ServiceOptionId = tiqetsAsyncBooking.ServiceOptionId,
                            RetryCount = tiqetsAsyncBooking.RetryCount,
                            FailureEmailSentCount = tiqetsAsyncBooking.FailureEmailSentCount,
                            BookedOptionId = tiqetsAsyncBooking.BookedOptionId,
                            NextProcessingTime = tiqetsAsyncBooking.NextProcessingTime,
                            Status = tiqetsAsyncBooking.Status,
                            VoucherLink = tiqetsAsyncBooking.VoucherLink,
                            AffiliateId = tiqetsAsyncBooking.AffiliateId,
                            WebhookUrl = tiqetsAsyncBooking.WebhookUrl,
                            WebhookRequest = tiqetsAsyncBooking.WebhookRequest,
                            WebhookResponse = tiqetsAsyncBooking.WebhookResponse,
                            IsWebhookSuccess = tiqetsAsyncBooking.IsWebhookSuccess,
                            WebhookRetryCount = tiqetsAsyncBooking.WebhookRetryCount ?? 1
                        };

                        SendWebhookResponse(tiqetsAsyncBooking.Status == AsyncBookingStatus.Success.ToString(), tiqetsAsyncBooking, tiqetsTableStorageData);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("AsyncBookingService|WebHookHit", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "ProcessIncompleteBooking",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private void RetrievePrioHub(List<AsyncBooking> prioHubAsyncBookings)
        {
            var request = string.Empty;
            var response = string.Empty;

            //Remove all with empty next processing time or with status as fail or success.
            prioHubAsyncBookings.RemoveAll(y => string.IsNullOrEmpty(y.NextProcessingTime)
               || (
                    y.Status == AsyncBookingStatus.Success.ToString()
                  )
            );

            foreach (var prioHubAsyncBooking in prioHubAsyncBookings)
            {
                //Remove all with empty next processing time or with status as fail or success.
                prioHubAsyncBookings.RemoveAll(y => string.IsNullOrEmpty(y.NextProcessingTime)
                   || (
                        /*y.Status == AsyncBookingStatus.Fail.ToString() ||*/
                        y.Status == AsyncBookingStatus.Success.ToString()
                      )
                );

                if (prioHubAsyncBooking?.ApiTypeMethod?.ToLower() == ConstantPrioHub.CANCEL.ToLower() ||
                    prioHubAsyncBooking?.ApiTypeMethod?.ToLower() == ConstantPrioHub.CANCELWEBHOOK.ToLower())// only for cancel booking
                {
                    //2 scenarios:
                    //1st case:while booking creation some error comes and cancel also gives error.
                    //then cancel it again from here
                    //2nd case:while cancellation: status comes:BOOKING_PROCESSING_CANCELLATION
                    prioHubAsyncBookings.RemoveAll(x => x.RetryCount > x.RetryThreshold);
                    CancelPrioHubBooking(prioHubAsyncBooking);
                }
                else if (prioHubAsyncBooking?.ApiTypeMethod?.ToLower() == ConstantPrioHub.BOOKING.ToLower())//booking
                {
                    prioHubAsyncBookings.RemoveAll(x => x.RetryCount > x.RetryThreshold);
                    if (prioHubAsyncBookings != null && prioHubAsyncBookings.Count > 0)
                    {
                        RetrievePrioHubBookingTicket(prioHubAsyncBooking);
                    }
                }
            }
        }

        private void CancelPrioHubBooking(AsyncBooking prioHubAsyncBookingData)
        {

            var request = string.Empty;
            var response = string.Empty;
            try
            {
                var currentUtcTime = DateTime.UtcNow;
                var dataCount = (Convert.ToInt32(prioHubAsyncBookingData.RetryCount) + 1);
                var interval = dataCount * dataCount; //1*1=2...5*5 =25
                var prioTableStorageData = new TiqetsTableStorageData
                {
                    OrderReferenceId = prioHubAsyncBookingData.OrderReferenceId,
                    BookingReferenceNo = prioHubAsyncBookingData.BookingReferenceNo,
                    ApiType = prioHubAsyncBookingData.ApiType,
                    LanguageCode = prioHubAsyncBookingData.LanguageCode,
                    Interval = prioHubAsyncBookingData.RetryInterval,
                    RetryThreshold = prioHubAsyncBookingData.RetryThreshold,
                    Token = prioHubAsyncBookingData.Token,
                    CustomerEmail = prioHubAsyncBookingData.CustomerEmail,
                    AvailabilityReferenceId = prioHubAsyncBookingData.AvailabilityReferenceId,
                    OptionName = prioHubAsyncBookingData.OptionName,
                    ServiceOptionId = prioHubAsyncBookingData.ServiceOptionId,
                    RetryCount = prioHubAsyncBookingData.RetryCount,
                    FailureEmailSentCount = prioHubAsyncBookingData.FailureEmailSentCount,
                    BookedOptionId = prioHubAsyncBookingData.BookedOptionId,
                    NextProcessingTime = prioHubAsyncBookingData.NextProcessingTime,
                    Status = prioHubAsyncBookingData.Status,
                    VoucherLink = prioHubAsyncBookingData.VoucherLink,
                    AffiliateId = prioHubAsyncBookingData.AffiliateId,
                    ApiTypeMethod = prioHubAsyncBookingData.ApiTypeMethod,
                    ApiDistributerId = prioHubAsyncBookingData.ApiDistributerId,
                    IsWebhookSuccess = prioHubAsyncBookingData.IsWebhookSuccess,
                    WebhookRetryCount = prioHubAsyncBookingData.WebhookRetryCount,
                    WebhookResponse = prioHubAsyncBookingData?.WebhookResponse,
                    WebhookUrl = prioHubAsyncBookingData.WebhookUrl
                };

                try
                {
                    //PrioHub Api cancellation here:
                    var selectedProduct = new PrioHubSelectedProduct
                    {
                        PrioHubDistributerId = prioTableStorageData?.ApiDistributerId,
                        PrioHubApiConfirmedBooking = new PrioHubAPITicket()
                    };
                    selectedProduct.PrioHubApiConfirmedBooking.BookingReference = prioTableStorageData?.OrderReferenceId;
                    //Case1: For Webhook:check that WebhookResponse gives reponse then pick data from there
                    var cancellationStatus = string.Empty;
                    if (prioHubAsyncBookingData?.ApiTypeMethod?.ToLower() == ConstantPrioHub.CANCELWEBHOOK.ToLower())
                    {
                        var getTicketResponse = GetAPIStatus(prioTableStorageData?.WebhookResponse);
                        cancellationStatus = getTicketResponse?.OrderStatus;
                    }
                    else
                    {
                        //case 2: For error comes in cancel booking:Retry to Cancel from PrioHub API
                        var cancellationStatusGet = _prioHubAdapter.CancelBooking(selectedProduct, prioTableStorageData?.Token, out request, out response);
                        cancellationStatus = cancellationStatusGet?.Item3;
                    }
                    try
                    {
                        if (cancellationStatus?.ToUpper() == ConstantPrioHub.ORDERCANCELLED)
                        {
                            prioTableStorageData.Status = AsyncBookingStatus.Success.ToString();
                            prioTableStorageData.NextProcessingTime = string.Empty;
                            //Success mail to customer support
                            _mailer.SendCancellationSuccessMail(prioHubAsyncBookingData.BookingReferenceNo, APIType.PrioHub.ToString(), request, response);
                        }
                        else
                        {
                            response += $"\r\n{response}";
                            prioTableStorageData.Status = AsyncBookingStatus.Fail.ToString();
                            prioTableStorageData.NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture);
                            if (prioHubAsyncBookingData.FailureEmailSentCount < 4)
                            {
                                prioTableStorageData.FailureEmailSentCount = (prioTableStorageData?.FailureEmailSentCount ?? 0) + 1;
                                prioHubAsyncBookingData.FailureEmailSentCount = (prioHubAsyncBookingData?.FailureEmailSentCount ?? 0) + 1;
                            }
                            //Send mail to Customer Support
                            if (prioHubAsyncBookingData.FailureEmailSentCount > 3)
                            {
                                prioTableStorageData.Status = "Failed";
                                prioTableStorageData.NextProcessingTime = string.Empty;
                                //send failure email to customer support
                                _mailer.SendCancellationFailureMail(prioHubAsyncBookingData.BookingReferenceNo, APIType.Tiqets.ToString(), request, response);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        prioTableStorageData.Status = AsyncBookingStatus.InProgress.ToString();
                        prioTableStorageData.NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture);
                        if (prioHubAsyncBookingData.FailureEmailSentCount < 3)
                        {
                            //send failure email to customer support
                            _mailer.SendCancellationFailureMail(prioHubAsyncBookingData.BookingReferenceNo, APIType.PrioHub.ToString(), request, response + ex.Message);
                            prioTableStorageData.FailureEmailSentCount = (prioTableStorageData?.FailureEmailSentCount ?? 0) + 1;
                            prioHubAsyncBookingData.FailureEmailSentCount = (prioHubAsyncBookingData?.FailureEmailSentCount ?? 0) + 1;
                        }
                        _log.Error("AsyncBookingService|CancelPrioHubBooking", ex);
                    }
                }
                catch (Exception ex)
                {
                    prioTableStorageData.Status = AsyncBookingStatus.InProgress.ToString();
                    prioTableStorageData.NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture);
                    _log.Error("AsyncBookingService|CancelPrioHubBooking", ex);
                }

                prioTableStorageData.RetryCount += 1;

                // Insert into Log table

                _tableStorageOperation.InsertTiqetsGetTicketDetailsLog(prioHubAsyncBookingData.Id, $"{prioHubAsyncBookingData.OrderReferenceId}_{prioTableStorageData.RetryCount}", prioTableStorageData.Status,
                     DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), request, response, "PrioHubAPI");
                

                // Update existing record
                _tableStorageOperation.UpdateTiqetsGetTicketData(prioTableStorageData, prioHubAsyncBookingData);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AsyncBookingService",
                    MethodName = "CancelPrioHubBooking",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        private void RetrieveTiqetsBookingTicket(List<AsyncBooking> tiqetsAsyncBookings)
        {
            var request = string.Empty;
            var response = string.Empty;
            var apiStatusCode = HttpStatusCode.InternalServerError;
            try
            {
                //Remove all with empty next processing time or with status as fail or success.
                //tiqetsAsyncBookings = tiqetsAsyncBookings.Where(x => x.CustomerEmail == "skumar@isango.com").ToList().Take(1).ToList();
                //Remove all with empty next processing time or with status as fail or success.
                tiqetsAsyncBookings.RemoveAll(y => string.IsNullOrEmpty(y.NextProcessingTime)
                   || (
                        /*y.Status == AsyncBookingStatus.Fail.ToString() ||*/
                        y.Status == AsyncBookingStatus.Success.ToString()
                      )
                );

                //for product with retry count is greater than retry threshold.. send mail to customer support.
                foreach (var tiqetsAsyncBooking in tiqetsAsyncBookings)
                {
                    if (tiqetsAsyncBooking.RetryCount > tiqetsAsyncBooking.RetryThreshold)
                    {
                        if (tiqetsAsyncBooking.FailureEmailSentCount < 3)
                        {
                            _mailer.SendVoucherDownloadFailureMail(tiqetsAsyncBooking.BookingReferenceNo, APIType.Tiqets.ToString(), request, response);
                            tiqetsAsyncBooking.FailureEmailSentCount = (tiqetsAsyncBooking?.FailureEmailSentCount ?? 0) + 1;
                            var tiqetsTableStorageData = new TiqetsTableStorageData
                            {
                                OrderReferenceId = tiqetsAsyncBooking.OrderReferenceId,
                                BookingReferenceNo = tiqetsAsyncBooking.BookingReferenceNo,
                                ApiType = tiqetsAsyncBooking.ApiType,
                                LanguageCode = tiqetsAsyncBooking.LanguageCode,
                                Interval = tiqetsAsyncBooking.RetryInterval,
                                RetryThreshold = tiqetsAsyncBooking.RetryThreshold,
                                Token = tiqetsAsyncBooking.Token,
                                CustomerEmail = tiqetsAsyncBooking.CustomerEmail,
                                AvailabilityReferenceId = tiqetsAsyncBooking.AvailabilityReferenceId,
                                OptionName = tiqetsAsyncBooking.OptionName,
                                ServiceOptionId = tiqetsAsyncBooking.ServiceOptionId,
                                RetryCount = tiqetsAsyncBooking.RetryCount,
                                FailureEmailSentCount = tiqetsAsyncBooking.FailureEmailSentCount,
                                BookedOptionId = tiqetsAsyncBooking.BookedOptionId,
                                NextProcessingTime = tiqetsAsyncBooking.NextProcessingTime,
                                Status = "Failed",
                                VoucherLink = tiqetsAsyncBooking.VoucherLink,
                                AffiliateId = tiqetsAsyncBooking.AffiliateId,
                                IsWebhookSuccess = "0",
                                WebhookRetryCount = 0
                            };
                            _tableStorageOperation.UpdateTiqetsGetTicketData(tiqetsTableStorageData, tiqetsAsyncBooking);
                        }
                    }
                }

                tiqetsAsyncBookings.RemoveAll(x => x.RetryCount > x.RetryThreshold);

                var currentUtcTime = DateTime.UtcNow;

                //Get product that fits within valid date time range
                //var validTiqetsAsyncBookings = tiqetsAsyncBookings.Where(x =>
                //    DateTimeOffset.Parse(x.NextProcessingTime).UtcDateTime <= currentUtcTime
                //    && DateTimeOffset.Parse(x.NextProcessingTime).UtcDateTime > currentUtcTime.AddMinutes(-1 * interval)).ToList();

                var validTiqetsAsyncBookings = tiqetsAsyncBookings;

                foreach (var tiqetsAsyncBooking in validTiqetsAsyncBookings)
                {
                    var dataCount = (Convert.ToInt32(tiqetsAsyncBooking.RetryCount) + 1);
                    var interval = dataCount * dataCount; //1*1=2...5*5 =25
                    var tiqetsTableStorageData = new TiqetsTableStorageData
                    {
                        OrderReferenceId = tiqetsAsyncBooking.OrderReferenceId,
                        BookingReferenceNo = tiqetsAsyncBooking.BookingReferenceNo,
                        ApiType = tiqetsAsyncBooking.ApiType,
                        LanguageCode = tiqetsAsyncBooking.LanguageCode,
                        Interval = tiqetsAsyncBooking.RetryInterval,
                        RetryThreshold = tiqetsAsyncBooking.RetryThreshold,
                        Token = tiqetsAsyncBooking.Token,
                        CustomerEmail = tiqetsAsyncBooking.CustomerEmail,
                        AvailabilityReferenceId = tiqetsAsyncBooking.AvailabilityReferenceId,
                        OptionName = tiqetsAsyncBooking.OptionName,
                        ServiceOptionId = tiqetsAsyncBooking.ServiceOptionId,
                        RetryCount = tiqetsAsyncBooking.RetryCount,
                        FailureEmailSentCount = tiqetsAsyncBooking.FailureEmailSentCount,
                        BookedOptionId = tiqetsAsyncBooking.BookedOptionId,
                        NextProcessingTime = tiqetsAsyncBooking.NextProcessingTime,
                        Status = tiqetsAsyncBooking.Status,
                        VoucherLink = tiqetsAsyncBooking.VoucherLink,
                        AffiliateId = tiqetsAsyncBooking.AffiliateId
                    };

                    try
                    {
                        var bookingRequest = new BookingRequest
                        {
                            LanguageCode = tiqetsAsyncBooking.LanguageCode,
                            RequestObject = new ConfirmOrderResponse
                            {
                                OrderReferenceId = tiqetsAsyncBooking.OrderReferenceId
                            },
                            IsangoBookingReference = tiqetsAsyncBooking.BookingReferenceNo,
                            AffiliateId = tiqetsAsyncBooking?.AffiliateId
                        };

                        var getTicketResponse = _tiqetsAdapter.GetTicket(bookingRequest, tiqetsAsyncBooking.Token, out request, out response, out apiStatusCode);

                        var bookedSelectedProduct = getTicketResponse?.SelectedProducts?.OfType<TiqetsSelectedProduct>()
                            ?.FirstOrDefault();

                        if (bookedSelectedProduct?.OrderStatus == TiqetsOrderStatus.Processing ||
                            bookedSelectedProduct?.OrderStatus == TiqetsOrderStatus.Pending ||
                            bookedSelectedProduct?.OrderStatus == TiqetsOrderStatus.New
                            )
                        {
                            //Continue web job
                            tiqetsTableStorageData.Status = AsyncBookingStatus.InProgress.ToString();
                            tiqetsTableStorageData.NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture);
                        }
                        else if (bookedSelectedProduct?.OrderStatus == TiqetsOrderStatus.Cancelled || bookedSelectedProduct?.OrderStatus == TiqetsOrderStatus.Failed)
                        {
                            //Send mail to Customer Support
                            _mailer.SendVoucherDownloadFailureMail(tiqetsAsyncBooking.BookingReferenceNo, APIType.Tiqets.ToString(), request, response);

                            try
                            {
                                var bookingDetail = _bookingService.GetBookingData(tiqetsAsyncBooking.BookingReferenceNo);

                                var bookedOptionId = bookingDetail?.BookedOptions?.FirstOrDefault(x => x.AvailabilityReferenceId == tiqetsAsyncBooking.AvailabilityReferenceId)?.BookedOptionId;

                                CancelBookings(tiqetsAsyncBooking.BookingReferenceNo, bookedOptionId ?? 0, bookingDetail, tiqetsAsyncBooking.Token);
                                tiqetsTableStorageData.Status = AsyncBookingStatus.Cancelled.ToString();
                                tiqetsTableStorageData.VoucherLink = AsyncBookingStatus.Cancelled.ToString();
                                try
                                {
                                    SendWebhookResponse(false, tiqetsAsyncBooking, tiqetsTableStorageData);
                                }
                                catch (Exception e)
                                {
                                    _log.Error("AsyncBookingService|Webhook", e);
                                }
                            }
                            catch (Exception ex)
                            {
                                _log.Error($"AsyncBookingService|cancelBooking {tiqetsAsyncBooking.BookingReferenceNo}", ex);
                            }
                        }
                        else if (bookedSelectedProduct?.OrderStatus == TiqetsOrderStatus.Done)
                        {
                            try
                            {
                                //Send mail to Customer
                                var ticketPdfUrl = bookedSelectedProduct?.TicketPdfUrl;
                                //_mailer.SendTiqetsBookingTicket(ticketPdfUrl, tiqetsAsyncBooking.BookingReferenceNo, tiqetsAsyncBooking.CustomerEmail);
                                if (!string.IsNullOrWhiteSpace(ticketPdfUrl))
                                {
                                    var bookedOptionId = _bookingPersistence.UpdateAPISupplierBookingQRCode(
                                            tiqetsAsyncBooking.BookingReferenceNo
                                            , tiqetsAsyncBooking.ServiceOptionId
                                            , tiqetsAsyncBooking.AvailabilityReferenceId
                                            , ticketPdfUrl
                                        );

                                    if (bookedOptionId > 0)
                                    {
                                        tiqetsTableStorageData.BookedOptionId = bookedOptionId;
                                        tiqetsTableStorageData.VoucherLink = ticketPdfUrl;

                                        var result = _bookingService.ConfirmBooking(bookedOptionId, _b2bUserKey, tiqetsAsyncBooking.Token);

                                        if (result.Item1 || result.Item2.ToLower() == "service already confirmation.")
                                        {
                                            tiqetsTableStorageData.Status = AsyncBookingStatus.Success.ToString();
                                            tiqetsTableStorageData.NextProcessingTime = string.Empty;

                                            var bookingWebhook = new BookingWebhookRequest
                                            {
                                                BookingReferenceNumber = tiqetsTableStorageData.BookingReferenceNo,
                                                BookingStatus = tiqetsTableStorageData.Status,
                                                VoucherURL = ticketPdfUrl
                                            };
                                            try
                                            {
                                                if (!string.IsNullOrWhiteSpace(tiqetsTableStorageData.WebhookUrl))
                                                {
                                                    using (WebClient webClient = new WebClient())
                                                    {
                                                        var data = SerializeDeSerializeHelper.Serialize(bookingWebhook);
                                                        var json = webClient.UploadString(tiqetsTableStorageData.WebhookUrl, data);
                                                        _log.Info($"AsyncBookingService|bookingWebhook {tiqetsTableStorageData.WebhookUrl}|{json}");
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                _log.Error($"AsyncBookingService|bookingWebhook {tiqetsTableStorageData.WebhookUrl}", ex);
                                            }

                                            _mailerService.SendMail(tiqetsAsyncBooking.BookingReferenceNo
                                                    , null
                                                    , false
                                                    , false
                                                    , tiqetsAsyncBooking.OrderReferenceId
                                                    , ticketPdfUrl
                                                );
                                        }
                                        else
                                        {
                                            response += $"\r\n{result?.Item2?.ToLower()}";
                                            tiqetsTableStorageData.Status = AsyncBookingStatus.Fail.ToString();
                                            tiqetsTableStorageData.NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture);

                                            //Send mail to Customer Support
                                            if (tiqetsAsyncBooking.FailureEmailSentCount < 3)
                                            {
                                                _mailer.SendVoucherDownloadFailureMail(tiqetsAsyncBooking.BookingReferenceNo, APIType.Tiqets.ToString(), request, response);
                                                tiqetsTableStorageData.FailureEmailSentCount = (tiqetsTableStorageData?.FailureEmailSentCount ?? 0) + 1;
                                                tiqetsAsyncBooking.FailureEmailSentCount = (tiqetsAsyncBooking?.FailureEmailSentCount ?? 0) + 1;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                tiqetsTableStorageData.Status = AsyncBookingStatus.InProgress.ToString();
                                tiqetsTableStorageData.NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture);
                                if (tiqetsAsyncBooking.FailureEmailSentCount < 3)
                                {
                                    _mailer.SendVoucherDownloadFailureMail(tiqetsAsyncBooking.BookingReferenceNo, APIType.Tiqets.ToString(), request, response + ex.Message);
                                    tiqetsTableStorageData.FailureEmailSentCount = (tiqetsTableStorageData?.FailureEmailSentCount ?? 0) + 1;
                                    tiqetsAsyncBooking.FailureEmailSentCount = (tiqetsAsyncBooking?.FailureEmailSentCount ?? 0) + 1;
                                }
                                _log.Error("AsyncBookingService|PostProcessingOfDownloadedTicket", ex);
                            }

                            try
                            {
                                SendWebhookResponse(true, tiqetsAsyncBooking, tiqetsTableStorageData);
                            }
                            catch (Exception ex)
                            {
                                _log.Error("AsyncBookingService|Webhook", ex);
                            }
                        }
                        else if (bookedSelectedProduct == null)
                        {
                            //Send mail to Customer Support
                            _mailer.SendVoucherDownloadFailureMail(tiqetsAsyncBooking.BookingReferenceNo, APIType.Tiqets.ToString(), request, response);

                            tiqetsTableStorageData.Status = AsyncBookingStatus.Cancelled.ToString();
                            tiqetsTableStorageData.NextProcessingTime = string.Empty;
                            try
                            {
                                var bookingDetail = _bookingService.GetBookingData(tiqetsAsyncBooking.BookingReferenceNo);

                                var bookedOptionId = bookingDetail?.BookedOptions?.FirstOrDefault(x => x.AvailabilityReferenceId == tiqetsAsyncBooking.AvailabilityReferenceId)?.BookedOptionId;

                                CancelBookings(tiqetsAsyncBooking.BookingReferenceNo, bookedOptionId ?? 0, bookingDetail, tiqetsAsyncBooking.Token);
                                try
                                {
                                    SendWebhookResponse(false, tiqetsAsyncBooking, tiqetsTableStorageData);
                                }
                                catch (Exception e)
                                {
                                    _log.Error("AsyncBookingService|Webhook", e);
                                }
                            }
                            catch (Exception ex)
                            {
                                _log.Error($"AsyncBookingService|cancelBooking {tiqetsAsyncBooking.BookingReferenceNo}", ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        tiqetsTableStorageData.Status = AsyncBookingStatus.InProgress.ToString();
                        tiqetsTableStorageData.NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture);
                        _log.Error("AsyncBookingService|RetrieveTiqetsBookingTicket", ex);
                    }

                    tiqetsTableStorageData.RetryCount += 1;

                    // Insert into Log table

                    _tableStorageOperation.InsertTiqetsGetTicketDetailsLog(tiqetsAsyncBooking.Id, $"{tiqetsAsyncBooking.OrderReferenceId}_{tiqetsTableStorageData.RetryCount}", tiqetsTableStorageData.Status,
                           DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), request, response, "TiqetsAPI");
                     

                    // Update existing record
                    _tableStorageOperation.UpdateTiqetsGetTicketData(tiqetsTableStorageData, tiqetsAsyncBooking);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AsyncBookingService",
                    MethodName = "RetrieveTiqetsBookingTicket",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private void SendWebhookResponse(bool isBookingSuccess, AsyncBooking tiqetsAsyncBooking, TiqetsTableStorageData tiqetsTableStorageData)
        {
            try
            {
                var bookingDetail = GetBookingDetail(tiqetsAsyncBooking.BookingReferenceNo);
                var bookingResponse = new BookingResponse
                {
                    Message = isBookingSuccess ? Constant.SuccessBookingMessage : Constant.FailedBookingMessage,
                    Status = isBookingSuccess ? BookingStatus.Confirmed.ToString() : BookingStatus.Cancelled.ToString(),
                    ReferenceId = tiqetsAsyncBooking.BookingReferenceNo,
                    BookingDetail = bookingDetail
                };
                var affiliateInfo = _affiliateService.GetAffiliateInfoAsync(domain: string.Empty, alias: string.Empty, affiliateId: bookingDetail.AffiliateId)?.GetAwaiter().GetResult();
                var username = ConfigurationManagerHelper.GetValuefromAppSettings("cssWebHookUsername");
                var password = ConfigurationManagerHelper.GetValuefromAppSettings("cssWebHookPassword");
                var usernamePassword = $"{username}:{password}";

                var webhookURL = affiliateInfo.WebHookURL;

                //bookingDetail.BookingReferenceNumber = "SGI3XG7OE";
                //bookingDetail.ProductDetails[0].LinkValue = "https://isango-api-uat-rg-app-jf2.azurewebsites.net/api/voucher/book/SGI3XG7OE/1/1389588/false";

                if (!string.IsNullOrWhiteSpace(webhookURL))
                {
                    if (tiqetsAsyncBooking.WebhookRetryCount > tiqetsAsyncBooking.RetryThreshold || tiqetsAsyncBooking.Status == "Failed")
                    {
                        try
                        {
                            var bookingData = _bookingService.GetBookingData(tiqetsAsyncBooking.BookingReferenceNo);

                            var bookedOptionId = bookingData?.BookedOptions?.FirstOrDefault(x => x.AvailabilityReferenceId == tiqetsAsyncBooking.AvailabilityReferenceId)?.BookedOptionId;


                            CancelBookings(tiqetsAsyncBooking.BookingReferenceNo, bookedOptionId ?? 0, bookingData, tiqetsAsyncBooking.Token);
                            tiqetsTableStorageData.Status = TiqetsOrderStatus.Cancelled.ToString();
                            tiqetsTableStorageData.NextProcessingTime = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                    }

                    var httpResponseMessage = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Content = new StringContent(string.Empty),
                    };
                    var resultWebhook = string.Empty;

                    try
                    {
                        using (var client = new HttpClient())
                        {
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                            client.Timeout = TimeSpan.FromMinutes(3);

                            var byteArray = System.Text.Encoding.ASCII.GetBytes(usernamePassword);
                            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                            var uploadString = SerializeDeSerializeHelper.Serialize(bookingResponse);
                            var content = new StringContent(uploadString);

                            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                            httpResponseMessage = client.PostAsync(webhookURL, content)?.GetAwaiter().GetResult();
                        }

                        resultWebhook = httpResponseMessage?.Content?.ReadAsStringAsync()?.GetAwaiter().GetResult();
                        resultWebhook = httpResponseMessage + resultWebhook;

                        tiqetsTableStorageData.WebhookUrl = webhookURL;
                        tiqetsTableStorageData.WebhookRequest = SerializeDeSerializeHelper.Serialize(bookingResponse);
                        tiqetsTableStorageData.WebhookResponse = resultWebhook;
                        tiqetsTableStorageData.WebhookRetryCount = (tiqetsTableStorageData.WebhookRetryCount ?? 0) + 1;
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            tiqetsTableStorageData.IsWebhookSuccess = "1";
                            tiqetsTableStorageData.NextProcessingTime = string.Empty;
                        }
                        else
                        {
                            tiqetsTableStorageData.IsWebhookSuccess = "0";
                        }
                        _tableStorageOperation.UpdateTiqetsGetTicketData(tiqetsTableStorageData, tiqetsAsyncBooking);

                        try
                        {
                            // Insert into Log table
                            _tableStorageOperation.InsertTiqetsGetTicketDetailsLog(tiqetsAsyncBooking.Id, $"{tiqetsAsyncBooking.OrderReferenceId}_{tiqetsTableStorageData.RetryCount}", tiqetsTableStorageData.Status,
                                     DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), tiqetsTableStorageData.WebhookRequest, tiqetsTableStorageData.WebhookResponse, "Webhook");
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        httpResponseMessage = new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            Content = new StringContent(ex.Message),
                        };
                        tiqetsTableStorageData.WebhookResponse = ex.Message;

                    }
                }
                else
                {
                    _tableStorageOperation.UpdateTiqetsGetTicketData(tiqetsTableStorageData, tiqetsAsyncBooking);
                }
            }
            catch (Exception ex)
            {
                _log.Error("AsyncBookingService|WebHookHit", ex);
            }
        }

        private BookingDetailResponse GetBookingDetail(string referenceNumber)
        {
            BookingDetailResponse bookingDetailResponse = null;
            try
            {
                var bookingData = _bookingService.GetBookingData(referenceNumber);
                if (bookingData.BookedOptions?.Count > 0)
                    bookingDetailResponse = PrepareBookingDetailResponse(bookingData);
            }
            catch (Exception)
            {
                bookingDetailResponse = null;
            }

            return bookingDetailResponse;
        }

        private BookingDetailResponse PrepareBookingDetailResponse(ConfirmBookingDetail confirmBookingDetail)
        {
            var bookingDetailResponse = new BookingDetailResponse
            {
                AffiliateId = confirmBookingDetail.AffiliateId,
                Language = confirmBookingDetail.LanguageCode,
                CurrencyIsoCode = confirmBookingDetail.CurrencyIsoCode,
                BookingReferenceNumber = confirmBookingDetail.BookingReferenceNumber,
                CustomerEmail = confirmBookingDetail.VoucherEmail,
                BookingDate = confirmBookingDetail.BookingDate,
                ProductDetails = new List<ProductDetail>()
            };
            foreach (var bookedOption in confirmBookingDetail.BookedOptions)
            {
                var productDetail = new ProductDetail
                {
                    Status = bookedOption.ServiceStatus.Replace("from Allocation", "").Trim(),
                    TravelDate = bookedOption.TravelDate,
                    DiscountAmount = bookedOption.DiscountAmount,
                    MultiSaveAmount = bookedOption.MutliSaveDiscount,
                    SellAmount = bookedOption.SellAmount,
                    LeadTravellerName = bookedOption.LeadPaxName,
                    ProductName = bookedOption.ServiceName,
                    IsReceipt = bookedOption.IsReceipt,
                    LinkType = bookedOption.IsQRCodePerPax ? null : bookedOption.LinkType,
                    LinkValue = bookedOption.IsQRCodePerPax ? null : bookedOption.LinkValue.Replace("QR_CODE~", ""),
                    Passengers = new List<Passenger>(),
                    AvailabilityReferenceId = bookedOption.AvailabilityReferenceId,
                    IsShowSupplierVoucher = bookedOption.IsShowSupplierVoucher,
                    BookedOptionId = bookedOption.BookedOptionId,
                    BookedOptionName = bookedOption.BookedOptionName,
                    IsQRCodePerPax = bookedOption.IsQRCodePerPax,
                    ApiType = bookedOption.ApiType
                };

                if (bookedOption.IsQRCodePerPax)
                {
                    foreach (var passengerDetail in bookedOption.BookedPassengerDetails)
                    {
                        if (passengerDetail.QRCodeDetail?.Count > 0)
                        {
                            foreach (var qrCode in passengerDetail.QRCodeDetail)
                            {
                                var fileName = _mailAttachmentService.FilterIllegalCharacterFromPath(qrCode.BarCode);
                                var generatedQRCodeImageFile = $"{confirmBookingDetail.BookingReferenceNumber}_{qrCode.BookedOptionID}_{fileName}";
                                _mailAttachmentService.GenerateQrCode(qrCode.BarCode, generatedQRCodeImageFile);
                                var passenger = new Passenger
                                {
                                    AgeGroupDescription = passengerDetail.AgeGroupDescription,
                                    PaxCount = 1,
                                    LinkType = string.IsNullOrWhiteSpace(bookedOption?.LinkType) ? "QRCODE" : (bookedOption?.LinkType ?? string.Empty),
                                    QRCodeValue = qrCode.BarCode,
                                    LinkValue = qrCode.BarCode,//$"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{generatedQRCodeImageFile}.png",
                                    PassengerTypeId = passengerDetail.PassengerTypeId
                                };
                                productDetail.Passengers.Add(passenger);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var passengerDetail in bookedOption.BookedPassengerDetails)
                    {
                        var passenger = new Passenger
                        {
                            AgeGroupDescription = passengerDetail.AgeGroupDescription,
                            PaxCount = passengerDetail.PaxCount,
                            LinkType = null,
                            LinkValue = null,
                            QRCodeValue = null,
                            PassengerTypeId = passengerDetail.PassengerTypeId
                        };
                        productDetail.Passengers.Add(passenger);
                    }
                }
                bookingDetailResponse.ProductDetails.Add(productDetail);
            }

            return bookingDetailResponse;
        }

        private CancellationPolicyEntitieResponse GetCancellationPolicyForBookingOption(string bookingRefNo, int bookedOptionId, ConfirmBookingDetail bookingDetail)
        {
            try
            {
                var spId = 0;
                var currencyIsoCode = bookingDetail?.CurrencyIsoCode;
                var cancellationPolicyData = _cancellationService
                    .GetCancellationPolicyDetailAsync(bookingRefNo, bookedOptionId, currencyIsoCode, spId).GetAwaiter()
                    .GetResult();
                var cancellationPolicyDetail = MapCancellationPolicyDetail(cancellationPolicyData, currencyIsoCode);
                return cancellationPolicyDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private CancellationPolicyEntitieResponse MapCancellationPolicyDetail(
            Isango.Entities.Cancellation.CancellationPolicyDetail cancellationPolicyData, string currencyIsoCode)
        {
            if (cancellationPolicyData?.UserCurrencyCode == null) return null;
            var cancellationPolicyDetailResponse =
                new CancellationPolicyEntitieResponse
                {
                    UserCurrencyCode = currencyIsoCode ?? cancellationPolicyData.UserCurrencyCode,
                    UserRefundAmount = cancellationPolicyData.UserRefundAmount,
                    TotalAmount = cancellationPolicyData.UserRefundAmount +
                                  cancellationPolicyData.UserCancellationCharges,
                    SellingPrice = cancellationPolicyData.SellingPrice,
                    CancellationDescription = cancellationPolicyData.CancellationChargeDescription
                };

            return cancellationPolicyDetailResponse;
        }

        private CancellationResponse CancelBookings(string bookingRefNo, int bookedOptionID, ConfirmBookingDetail bookingDetail, string token)
        {
            try
            {

                var cancellationPolicy = GetCancellationPolicyForBookingOption(bookingRefNo, bookedOptionID, bookingDetail);

                var parametersForCancellation = new CancellationParameters
                {
                    BookedOptionId = bookedOptionID,
                    SupplierNotes = "AsyncBooking",
                    AlternativeTours = "AsyncBooking",
                    CustomerNotes = "AsyncBooking",
                    Reason = "AsyncBooking",
                    RequestedBy = "AsyncBooking",
                    UserCurrencyCode = cancellationPolicy?.UserCurrencyCode ?? "GBP",
                    UserRefundAmount = cancellationPolicy?.UserRefundAmount ?? 0
                };

                var cancellationRequest = new CancellationRequestB2C
                {
                    CancellationParameters = parametersForCancellation,
                    BookingRefNo = bookingRefNo,
                    TokenId = token,
                    UserName = "AsyncBooking",
                    IsBookingManager = true
                };
                if (cancellationRequest != null && !string.IsNullOrEmpty(cancellationRequest.BookingRefNo) && cancellationRequest.CancellationParameters.BookedOptionId != 0)
                {
                    var requestString = SerializeDeSerializeHelper.Serialize(cancellationRequest);
                    var serviceURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl) + ConfigurationManagerHelper.GetValuefromAppSettings("CancelBookingURL");
                    var response = PostRequestAndFetchResultObject(requestString, serviceURL);
                    //var response = HttpClientHelper.DeleteRequestResponse(requestString, serviceURL);
                    var responseObject = JsonConvert.DeserializeObject<CancellationResponse>(response);

                    return responseObject;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string PostRequestAndFetchResultObject(string inputString, string serviceUrl)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var token = GetAuthenticationAsync()?.GetAwaiter().GetResult();
                var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
                var content = new StringContent(inputString);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var t = JsonConvert.DeserializeObject<B2CToken>(token);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.AccessToken);
                }
                //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var response = client.PostAsync(serviceUrl, content).GetAwaiter().GetResult();
                var responseObj = response.Content.ReadAsStringAsync().Result;
                return responseObj;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return string.Empty;
            }
        }

        private async Task<string> GetAuthenticationAsync()
        {
            var b2cAuthentication = default(string);
            try
            {
                var b2CUserName = ConfigurationManagerHelper.GetValuefromAppSettings("B2CUserName");
                var b2CPassword = ConfigurationManagerHelper.GetValuefromAppSettings("B2CPassword");
                var key = Constant.b2CAuthentication;
                if (!string.IsNullOrWhiteSpace(key))
                {
                    b2cAuthentication = GetToken(b2CUserName, b2CPassword);
                    var t = JsonConvert.DeserializeObject<B2CToken>(b2cAuthentication);
                    if (t.Error == Constant.authorizationError) // Again hit second time for the GetToken
                    {
                        b2cAuthentication = GetToken(b2CUserName, b2CPassword);
                    }
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return b2cAuthentication;
        }

        private string GetToken(string userName, string password)
        {
            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", userName ),
                        new KeyValuePair<string, string> ( "Password", password )
                    };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl) + "/Token", content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }


        private void RetrievePrioHubBookingTicket(AsyncBooking prioHubAsyncBooking)
        {
            var request = string.Empty;
            var response = string.Empty;
            try
            {
                //for product with retry count is greater than retry threshold.. send mail to customer support.
                if (prioHubAsyncBooking.RetryCount > prioHubAsyncBooking.RetryThreshold)
                {
                    if (prioHubAsyncBooking.FailureEmailSentCount < 3)
                    {
                        _mailer.SendVoucherDownloadFailureMail(prioHubAsyncBooking.BookingReferenceNo, APIType.PrioHub.ToString(), request, response);
                        prioHubAsyncBooking.FailureEmailSentCount = (prioHubAsyncBooking?.FailureEmailSentCount ?? 0) + 1;
                        var prioHubTableStorageData = new TiqetsTableStorageData
                        {
                            OrderReferenceId = prioHubAsyncBooking.OrderReferenceId,
                            BookingReferenceNo = prioHubAsyncBooking.BookingReferenceNo,
                            ApiType = prioHubAsyncBooking.ApiType,
                            LanguageCode = prioHubAsyncBooking.LanguageCode,
                            Interval = prioHubAsyncBooking.RetryInterval,
                            RetryThreshold = prioHubAsyncBooking.RetryThreshold,
                            Token = prioHubAsyncBooking.Token,
                            CustomerEmail = prioHubAsyncBooking.CustomerEmail,
                            AvailabilityReferenceId = prioHubAsyncBooking.AvailabilityReferenceId,
                            OptionName = prioHubAsyncBooking.OptionName,
                            ServiceOptionId = prioHubAsyncBooking.ServiceOptionId,
                            RetryCount = prioHubAsyncBooking.RetryCount,
                            FailureEmailSentCount = prioHubAsyncBooking.FailureEmailSentCount,
                            BookedOptionId = prioHubAsyncBooking.BookedOptionId,
                            NextProcessingTime = prioHubAsyncBooking.NextProcessingTime,
                            Status = "Failed",
                            VoucherLink = prioHubAsyncBooking.VoucherLink,
                            AffiliateId = prioHubAsyncBooking.AffiliateId,
                            ApiDistributerId = prioHubAsyncBooking?.ApiDistributerId,
                            IsWebhookSuccess = "0",
                            WebhookRetryCount = 0,
                            WebhookUrl = prioHubAsyncBooking.WebhookUrl
                        };
                        _tableStorageOperation.UpdateTiqetsGetTicketData(prioHubTableStorageData, prioHubAsyncBooking);
                    }
                }

                var currentUtcTime = DateTime.UtcNow;
                var dataCount = (Convert.ToInt32(prioHubAsyncBooking.RetryCount) + 1);
                var interval = dataCount * dataCount; //1*1=2...5*5 =25
                var prioTableStorageData = new TiqetsTableStorageData
                {
                    OrderReferenceId = prioHubAsyncBooking.OrderReferenceId,
                    BookingReferenceNo = prioHubAsyncBooking.BookingReferenceNo,
                    ApiType = prioHubAsyncBooking.ApiType,
                    LanguageCode = prioHubAsyncBooking.LanguageCode,
                    Interval = prioHubAsyncBooking.RetryInterval,
                    RetryThreshold = prioHubAsyncBooking.RetryThreshold,
                    Token = prioHubAsyncBooking.Token,
                    CustomerEmail = prioHubAsyncBooking.CustomerEmail,
                    AvailabilityReferenceId = prioHubAsyncBooking.AvailabilityReferenceId,
                    OptionName = prioHubAsyncBooking.OptionName,
                    ServiceOptionId = prioHubAsyncBooking.ServiceOptionId,
                    RetryCount = prioHubAsyncBooking.RetryCount,
                    FailureEmailSentCount = prioHubAsyncBooking.FailureEmailSentCount,
                    BookedOptionId = prioHubAsyncBooking.BookedOptionId,
                    NextProcessingTime = prioHubAsyncBooking.NextProcessingTime,
                    Status = prioHubAsyncBooking.Status,
                    VoucherLink = prioHubAsyncBooking.VoucherLink,
                    ApiDistributerId = prioHubAsyncBooking?.ApiDistributerId,
                    WebhookResponse = prioHubAsyncBooking?.WebhookResponse,
                    AffiliateId = prioHubAsyncBooking.AffiliateId,
                    IsWebhookSuccess = prioHubAsyncBooking.IsWebhookSuccess,
                    WebhookUrl = prioHubAsyncBooking.WebhookUrl
                };

                try
                {

                    var prioHubDistributerId = prioHubAsyncBooking?.ApiDistributerId;
                    var apiReferenceNumber = prioHubAsyncBooking.OrderReferenceId;

                    var getTicketResponse = GetAPIStatus(prioTableStorageData.WebhookResponse);
                    if (getTicketResponse != null &&
                        (getTicketResponse?.OrderStatus?.ToUpper() == ConstantPrioHub.ORDERPENDING ||
                        getTicketResponse?.OrderStatus?.ToUpper() == ConstantPrioHub.BOOKINGPROCESSINGCONFIRMATION))
                    {
                        //Continue web job
                        prioTableStorageData.Status = AsyncBookingStatus.InProgress.ToString();
                        prioTableStorageData.NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture);
                    }
                    else if (getTicketResponse != null && getTicketResponse?.OrderStatus?.ToUpper() == ConstantPrioHub.ORDERCANCELLED)
                    {
                        prioTableStorageData.Status = AsyncBookingStatus.Success.ToString();
                        prioTableStorageData.VoucherLink = AsyncBookingStatus.Success.ToString();
                    }
                    else if (getTicketResponse != null &&
                        (getTicketResponse?.OrderStatus?.ToUpper() == ConstantPrioHub.ORDERCONFIRMED ||
                        getTicketResponse?.OrderStatus?.ToUpper() == ConstantPrioHub.BOOKINGCONFIRMED)
                        )
                    {
                        try
                        {

                            //Send mail to Customer
                            //2cases:
                            //check order is perPax Qrcode or have one qrCode only
                            var orderBooking = getTicketResponse?.OrderBookings[0];
                            var bookedOptionId = 0;
                            var apiQrCode = string.Empty;
                            if (!string.IsNullOrEmpty(orderBooking?.BookingGroupCode))//single QRCode
                            {
                                apiQrCode = orderBooking?.ProductCodeSettings?.ProductCodeFormat + "~" + orderBooking?.BookingGroupCode;
                                bookedOptionId = _bookingPersistence.UpdateAPISupplierBookingQRCode(
                                       prioHubAsyncBooking.BookingReferenceNo
                                       , prioHubAsyncBooking.ServiceOptionId
                                       , prioHubAsyncBooking.AvailabilityReferenceId
                                       , apiQrCode
                                       , orderBooking?.ProductCodeSettings?.ProductCodeFormat
                                       , false
                                       , string.Empty
                                );

                            }
                            else //multiple QRCode
                            {
                                var ticketLength = orderBooking?.ProductTypeDetails?.Count;
                                var lstData = new List<TicketPrioHubPaxWise>();
                                for (var ticket = 0; ticket < ticketLength; ticket++)
                                {
                                    var data = new TicketPrioHubPaxWise
                                    {
                                        passengertype = orderBooking?.ProductTypeDetails[ticket]?.ProductType,
                                        qr_code = orderBooking?.ProductTypeDetails[ticket]?.ProductTypeCode
                                    };
                                    if (data != null)
                                    {
                                        lstData.Add(data);
                                    }
                                }
                                if (lstData != null && lstData.Count > 0)
                                {
                                    apiQrCode = SerializeDeSerializeHelper.Serialize(lstData);
                                }

                                bookedOptionId = _bookingPersistence.UpdateAPISupplierBookingQRCode(
                                      prioHubAsyncBooking.BookingReferenceNo
                                      , prioHubAsyncBooking.ServiceOptionId
                                      , prioHubAsyncBooking.AvailabilityReferenceId
                                      , string.Empty
                                      , orderBooking?.ProductCodeSettings?.ProductCodeFormat
                                      , true
                                      , apiQrCode
                               );
                            }
                            if (!string.IsNullOrWhiteSpace(apiQrCode))
                            {

                                if (bookedOptionId > 0)
                                {
                                    prioTableStorageData.BookedOptionId = bookedOptionId;
                                    prioTableStorageData.VoucherLink = apiQrCode;

                                    var result = _bookingService.ConfirmBooking(bookedOptionId, _b2bUserKey, prioHubAsyncBooking.Token);

                                    if (result.Item1 || result.Item2.ToLower() == "service already confirmation.")
                                    {
                                        prioTableStorageData.Status = AsyncBookingStatus.Success.ToString();
                                        prioTableStorageData.NextProcessingTime = string.Empty;

                                        var bookingWebhook = new BookingWebhookRequest
                                        {
                                            BookingReferenceNumber = prioTableStorageData.BookingReferenceNo,
                                            BookingStatus = prioTableStorageData.Status,
                                            VoucherURL = apiQrCode
                                        };
                                        try
                                        {
                                            if (!string.IsNullOrWhiteSpace(prioTableStorageData.WebhookUrl))
                                            {
                                                using (WebClient webClient = new WebClient())
                                                {
                                                    var data = SerializeDeSerializeHelper.Serialize(bookingWebhook);
                                                    var json = webClient.UploadString(prioTableStorageData.WebhookUrl, data);
                                                    _log.Info($"AsyncBookingService|bookingWebhook {prioTableStorageData.WebhookUrl}|{json}");
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _log.Error($"AsyncBookingService|bookingWebhook {prioTableStorageData.WebhookUrl}", ex);
                                        }

                                        _mailerService.SendMail(prioHubAsyncBooking.BookingReferenceNo
                                                , null
                                                , false
                                                , false
                                                , prioHubAsyncBooking.OrderReferenceId
                                                , null
                                            );
                                    }
                                    else
                                    {
                                        response += $"\r\n{result?.Item2?.ToLower()}";
                                        prioTableStorageData.Status = AsyncBookingStatus.Fail.ToString();
                                        prioTableStorageData.NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture);

                                        //Send mail to Customer Support
                                        if (prioHubAsyncBooking.FailureEmailSentCount < 3)
                                        {
                                            _mailer.SendVoucherDownloadFailureMail(prioHubAsyncBooking.BookingReferenceNo, APIType.PrioHub.ToString(), request, response);
                                            prioTableStorageData.FailureEmailSentCount = (prioTableStorageData?.FailureEmailSentCount ?? 0) + 1;
                                            prioHubAsyncBooking.FailureEmailSentCount = (prioHubAsyncBooking?.FailureEmailSentCount ?? 0) + 1;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            prioTableStorageData.Status = AsyncBookingStatus.InProgress.ToString();
                            prioTableStorageData.NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture);
                            if (prioHubAsyncBooking.FailureEmailSentCount < 3)
                            {
                                _mailer.SendVoucherDownloadFailureMail(prioHubAsyncBooking.BookingReferenceNo, APIType.PrioHub.ToString(), request, response + ex.Message);
                                prioTableStorageData.FailureEmailSentCount = (prioTableStorageData?.FailureEmailSentCount ?? 0) + 1;
                                prioHubAsyncBooking.FailureEmailSentCount = (prioHubAsyncBooking?.FailureEmailSentCount ?? 0) + 1;
                            }
                            _log.Error("AsyncBookingService|PostProcessingOfDownloadedTicket", ex);
                        }

                    }
                    else if (getTicketResponse == null)
                    {
                        //Send mail to Customer Support
                        _mailer.SendVoucherDownloadFailureMail(prioHubAsyncBooking.BookingReferenceNo, APIType.PrioHub.ToString(), request, response);
                        prioTableStorageData.Status = AsyncBookingStatus.Fail.ToString();
                        prioTableStorageData.NextProcessingTime = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    prioTableStorageData.Status = AsyncBookingStatus.InProgress.ToString();
                    prioTableStorageData.NextProcessingTime = DateTime.UtcNow.AddMinutes(interval).ToString(CultureInfo.InvariantCulture);
                    _log.Error("AsyncBookingService|RetrievePrioHubBookingTicket", ex);
                }

                prioTableStorageData.RetryCount += 1;

                // Insert into Log table
                _tableStorageOperation.InsertTiqetsGetTicketDetailsLog(prioHubAsyncBooking.Id, $"{prioHubAsyncBooking.OrderReferenceId}_{prioTableStorageData.RetryCount}", prioTableStorageData.Status,
                     DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), request, response, "PrioHubAPI");

                // Update existing record
                _tableStorageOperation.UpdateTiqetsGetTicketData(prioTableStorageData, prioHubAsyncBooking);

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AsyncBookingService",
                    MethodName = "RetrievePrioHubBookingTicket",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Order GetAPIStatus(string apiBookingResponse)
        {
            if (!string.IsNullOrEmpty(apiBookingResponse))
            {
                try
                {
                    var createBookingRs = SerializeDeSerializeHelper.DeSerialize<CreateBookingResponse>(apiBookingResponse?.ToString());
                    return createBookingRs?.Data?.Order;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }
    }
}