using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Service.Contract;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Util;
using WebAPI.Filters;
using WebAPI.Helper;
using WebAPI.Mapper;
using WebAPI.Models;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;
using CancellationStatus = WebAPI.Models.ResponseModels.CancellationStatus;
using ILogger = Logger.Contract.ILogger;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Provide details of cancellation policy of Booked product options endpoints
    /// </summary>
    [Route("api/cancellation")]
    [ApiController]
    public class CancellationController : ApiBaseController
    {
        private readonly CancellationMapper _cancellationMapper;
        private readonly ICancellationService _cancellationService;
        private readonly CancellationHelper _cancellationHelper;
        private readonly IBookingService _bookingService;
        private readonly ILogger _log;

        public CancellationController(ICancellationService cancellationService, CancellationMapper cancelBookingMapper,
            CancellationHelper cancelBookingHelper, IBookingService bookingService, ILogger log)
        {
            _cancellationService = cancellationService;
            _cancellationMapper = cancelBookingMapper;
            _cancellationHelper = cancelBookingHelper;
            _bookingService = bookingService;
            _log = log;
        }

        /// <summary>
        /// Get cancellation policy amount detail by booking reference number and booked option id
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <param name="bookedOptionId"></param>
        /// <returns></returns>
        [Route("policydetail/{bookingRefNo}/bookedOptionId/{bookedOptionId}")]
        [HttpGet]
        [ValidateModel]
        public IActionResult GetCancellationPolicyDetail(string bookingRefNo, int bookedOptionId)
        {
            var spId = 0;
            var bookingDetail = _bookingService.GetBookingData(bookingRefNo);
            var currencyIsoCode = bookingDetail?.CurrencyIsoCode;
            var cancellationPolicyData = _cancellationService
                .GetCancellationPolicyDetailAsync(bookingRefNo, bookedOptionId, currencyIsoCode, spId).GetAwaiter()
                .GetResult();
            var cancellationPolicyDetail =
                _cancellationMapper.MapCancellationPolicyDetail(cancellationPolicyData, currencyIsoCode);
            return GetResponseWithActionResult(cancellationPolicyDetail);
        }

        /// <summary>
        /// This method used to cancel booking of single product at a time
        /// </summary>
        /// <param name="cancellationRequest"></param>
        /// <returns></returns>
        [Route("cancelbooking")]
        [HttpPost]
        [ValidateModel]
        //[SwitchableAuthorization]
        public IActionResult CancelBooking(CancellationRequest cancellationRequest)
        {
            var watchWholeCancellation = Stopwatch.StartNew();
            cancellationRequest.IsBookingManager = true;// it is always true here(client pass or not doesn't matter- resolved issue of 2 can. emails comes)
            var tokenValue = cancellationRequest.BookingRefNo + "_" + cancellationRequest.CancellationParameters.BookedOptionId;
            var TimerMethodName = "CancelBooking" + "_" + tokenValue;
            logInfoSave(tokenValue, "Start");
            var cancellationResponse = new CancellationResponse
            {
                Status = new CancellationStatus
                {
                    AllCancelStatus = new AllCancelStatus
                    {
                        IsangoBookingCancel = OptionCancelStatus.Failed.ToString(),
                        PaymentRefundStatus = OptionCancelStatus.Failed.ToString(),
                        SupplierBookingCancel = OptionCancelStatus.Failed.ToString()
                    },
                    Message = Constants.Constant.CancellationFailed
                },
                Remark = ""
            };

            //get cancellation status of all three steps from db
            var watchGetCancellation = Stopwatch.StartNew();
            var cancellationStatus = _cancellationService
                .GetCancellationStatusAsync(cancellationRequest.CancellationParameters.BookedOptionId).GetAwaiter().GetResult();
            watchGetCancellation.Stop();
            _log.WriteTimer("GetCancellationStatusAsync", TimerMethodName, string.Empty, watchGetCancellation.Elapsed.ToString());
            //if cancellation completed for the product or all cancellation steps are done successfully
            if (_cancellationMapper.IsCancellationCompleted(cancellationStatus))
            {
                cancellationResponse = _cancellationMapper.MapCancellationStatusResponse(cancellationStatus);
                cancellationResponse.Remark = Constants.Constant.ProductAlreadyCancelledRemark;
                logInfoSave(tokenValue, "cancellationAlreadyCompleted", SerializeDeSerializeHelper.Serialize(cancellationResponse));
                return GetResponseWithActionResult(cancellationResponse);
            }

            //get user id and has permission by username to modify refund amount
            var watchUserCancellationPermission = Stopwatch.StartNew();
            var userData = _cancellationHelper.GetUserCancellationPermission(cancellationRequest.UserName);
            watchUserCancellationPermission.Stop();
            _log.WriteTimer("GetUserCancellationPermission", TimerMethodName, string.Empty, watchUserCancellationPermission.Elapsed.ToString());
            if (userData == null)
            {
                cancellationResponse = _cancellationMapper.MapCancellationStatusResponse(cancellationStatus);
                cancellationResponse.Remark = Constants.Constant.UserDataNotPresentRemark;
                logInfoSave(tokenValue, "UserDataNull", SerializeDeSerializeHelper.Serialize(cancellationResponse));
                return GetResponseWithActionResult(cancellationResponse);
            }

            //get booking data
            var watchBookingData = Stopwatch.StartNew();
            var bookingData = _cancellationHelper.GetBookingDetails(cancellationRequest, userData);
            watchBookingData.Stop();
            _log.WriteTimer("GetBookingDetails", TimerMethodName, string.Empty, watchBookingData.Elapsed.ToString());
            //getting booked option data from booking data
            var bookedOption = bookingData.BookedOptions.FirstOrDefault(x => x.BookedOptionId == cancellationRequest.CancellationParameters.BookedOptionId);

            //if booked option data is not present
            if (bookedOption == null)
            {
                cancellationResponse = _cancellationMapper.MapCancellationStatusResponse(cancellationStatus);
                cancellationResponse.Remark = Constants.Constant.BookingDetailNotPresentRemark;
                logInfoSave(tokenValue, "BookedOptionDataNull", SerializeDeSerializeHelper.Serialize(cancellationResponse));
                return GetResponseWithActionResult(cancellationResponse);
            }

            var sellingPrice = bookedOption.ChargedAmount;
            var cancellation = _cancellationMapper.MapCancellationRequest(cancellationRequest, userData, bookingData);

            //comparing entered refund amount with booked product's selling price
            //max refund amount allowed is the selling price of that product
            if (cancellation.CancellationParameters?.UserRefundAmount <= sellingPrice)
            {
                var spId = 0;
                var watchCancellationPolicy = Stopwatch.StartNew();
                //get cancellation policy amount details
                var cancellationPolicyData = _cancellationService.GetCancellationPolicyDetailAsync(cancellation.BookingRefNo,
                        cancellation.CancellationParameters.BookedOptionId, cancellation.CancellationParameters.CurrencyCode, spId)
                    .GetAwaiter().GetResult();
                watchCancellationPolicy.Stop();
                _log.WriteTimer("GetCancellationPolicyDetailAsync", TimerMethodName, cancellationPolicyData.ApiTypeId, watchCancellationPolicy.Elapsed.ToString());

                logInfoSave(tokenValue, "cancellationPolicyData", SerializeDeSerializeHelper.Serialize(cancellationPolicyData));
                //if cancellation policy data is null and isango db cancellation is not succeeded
                if (cancellationPolicyData == null && !_cancellationMapper.IsIsangoCancellationSucceed(cancellationStatus))
                {
                    cancellationResponse = _cancellationMapper.MapCancellationStatusResponse(cancellationStatus);
                    cancellationResponse.Remark = Constants.Constant.CancellationPolicyNotPresentRemark;
                    logInfoSave(tokenValue, "cancellationPolicyDataNull", SerializeDeSerializeHelper.Serialize(cancellationResponse));
                    return GetResponseWithActionResult(cancellationResponse);
                }

                //To cancel and get status for isango db cancellation and payment refund
                //User refund amount is the user entered Amount with permission
                var serviceStatus = bookedOption?.ServiceStatus?.ToLowerInvariant();
                //check product is OR or not
                if (!serviceStatus.Contains("on request"))
                {
                    if (userData.IsPermitted == 1)
                    {
                        cancellationPolicyData.UserRefundAmount = Convert.ToDecimal(cancellation?.CancellationParameters?.UserRefundAmount);
                    }
                    else
                    {
                        //check product is OR or not
                        if (cancellationPolicyData.UserRefundAmount != Convert.ToDecimal(cancellation?.CancellationParameters?.UserRefundAmount))
                        {
                            cancellationResponse = _cancellationMapper.MapCancellationStatusResponse(cancellationStatus);
                            cancellationResponse.Remark = Constants.Constant.UserEnteredValueAndPolicy;
                            logInfoSave(tokenValue, "RefundAmountNotMatched", SerializeDeSerializeHelper.Serialize(cancellationResponse));
                            return GetResponseWithActionResult(cancellationResponse);
                        }
                    }
                }
                var watchCancelAndGetIsangoDb = Stopwatch.StartNew();
                var isangoDbCancelled = _cancellationHelper.CancelAndGetIsangoDbCancellationAndPaymentRefundStatus(cancellation,
                cancellationPolicyData, spId, userData, cancellationStatus);
                watchCancelAndGetIsangoDb.Stop();
                _log.WriteTimer("CancelAndGetIsangoDbCancellationAndPaymentRefundStatus", TimerMethodName, cancellationPolicyData.ApiTypeId, watchCancelAndGetIsangoDb.Elapsed.ToString());

                logInfoSave(tokenValue, "isangoDbCancelled", SerializeDeSerializeHelper.Serialize(isangoDbCancelled));
                cancellationResponse =
                    _cancellationMapper.MapCancellationResponseAfterCancellationSteps(cancellationResponse, isangoDbCancelled, OptionCancelStatus.Failed);

                var watchInsertOrUpdateCancelStatus = Stopwatch.StartNew();
                //insert or update cancellation status in db for all three steps
                InsertOrUpdateCancelStatus(cancellationResponse, cancellationRequest.CancellationParameters.BookedOptionId);
                watchInsertOrUpdateCancelStatus.Stop();
                _log.WriteTimer("InsertOrUpdateCancelStatus", TimerMethodName, cancellationPolicyData.ApiTypeId, watchInsertOrUpdateCancelStatus.Elapsed.ToString());
                //get supplier data

                var watchSupplierData = Stopwatch.StartNew();
                var supplierDataList = _cancellationService.GetSupplierCancellationDataListAsync(cancellationRequest.BookingRefNo)
                    .GetAwaiter().GetResult();

                var supplierData = supplierDataList?.Where(x => x.BookedOptionId == cancellationRequest?.CancellationParameters?.BookedOptionId)?.FirstOrDefault();
                watchSupplierData.Stop();
                _log.WriteTimer("GetSupplierCancellationDataAsync", TimerMethodName, cancellationPolicyData.ApiTypeId, watchSupplierData.Elapsed.ToString());

                logInfoSave(tokenValue, "supplierData", SerializeDeSerializeHelper.Serialize(supplierData));
                //Check for non-cancellable supplier
                var isNonCancellableProduct = _cancellationHelper.CheckNonCancellableProduct(supplierData);

                var watchOptionAndServiceName = Stopwatch.StartNew();
                //get option name and service name of the product for cancel booking mail
                var optionAndServiceName = _bookingService.GetOptionAndServiceNameAsync(cancellation.BookingRefNo, false,
                    cancellation.CancellationParameters.BookedOptionId.ToString()).GetAwaiter().GetResult();
                watchOptionAndServiceName.Stop();
                _log.WriteTimer("GetOptionAndServiceNameAsync", TimerMethodName, cancellationPolicyData.ApiTypeId, watchOptionAndServiceName.Elapsed.ToString());

                var cancelBookingMailData =
                    _cancellationMapper.MapCancelBookingMailDetail(bookingData, supplierData,
                    optionAndServiceName, cancellation.TokenId, bookedOption);

                //if isango db cancellation was not failed, then it will go for supplier cancellation
                //else it will not go for supplier cancellation
                if (!string.Equals(isangoDbCancelled["isCancelled"], OptionCancelStatus.Failed.ToString()))
                {
                    logInfoSave(tokenValue, "IsangoDBCancelledSuccess");
                    //check for non-cancellable product
                    if (!isNonCancellableProduct)
                    {
                        // _log.Info($"Cancellation|CancelBooking|CancellableProduct");
                        //if the supplierData is not null and booked product is not NonCancellableProduct then supplier cancellation will proceed
                        //if supplierData is null ,then it's isango product and supplier cancellation is not applicable
                        if (supplierData != null &&
                            cancellationStatus.SupplierCancelStatus == (int)OptionCancelStatus.Failed
                            && cancellationPolicyData?.ApiTypeId?.ToLowerInvariant() != Convert.ToString((Convert.ToInt32(APIType.Undefined)))?.ToLowerInvariant())
                        {
                            //Mapping selected product for different suppliers
                            var booking = _cancellationMapper.MapSupplierDataForBookedProduct(supplierData, bookingData,
                                cancellation);
                            //logic for supplier booking cancellation
                            try
                            {
                                logInfoSave(tokenValue, "supplierCancellationStart");
                                var watchSupplierCancellation = Stopwatch.StartNew();
                                var supplierCancellation = _bookingService
                                    .CancelSupplierBookingAsync(booking, cancellation?.TokenId ?? tokenValue, cancellationRequest.IsBookingManager).GetAwaiter()
                                    .GetResult();
                                watchSupplierCancellation.Stop();
                                _log.WriteTimer("CancelSupplierBookingAsync", TimerMethodName, cancellationPolicyData.ApiTypeId, watchSupplierCancellation.Elapsed.ToString());

                                logInfoSave(tokenValue, "supplierCancellationEnd", SerializeDeSerializeHelper.Serialize(supplierCancellation));

                                cancellationResponse.Status.AllCancelStatus.SupplierBookingCancel = supplierCancellation
                                    ? OptionCancelStatus.Success.ToString()
                                    : OptionCancelStatus.Failed.ToString();
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "Cancellation",
                                    MethodName = "CancelBooking",
                                    Token = cancellationRequest.TokenId
                                };
                                _log.Error(isangoErrorEntity, ex);
                                logInfoSave(tokenValue, "supplierCancellationError");
                                cancellationResponse.Status.AllCancelStatus.SupplierBookingCancel = OptionCancelStatus.Failed.ToString();
                            }
                        }
                        else
                        {
                            cancellationResponse.Status.AllCancelStatus.SupplierBookingCancel =
                                OptionCancelStatus.NotApplicable.ToString();
                        }

                        cancellationResponse.Status.Message = Constants.Constant.CancellationSucceed;
                    }
                    else
                    {
                        cancellationResponse =
                            _cancellationMapper.MapCancellationResponseAfterCancellationSteps(cancellationResponse,
                                isangoDbCancelled, OptionCancelStatus.NotApplicable);

                        //map message according to isango db cancellation status
                        cancellationResponse.Status.Message =
                            cancellationResponse.Status.AllCancelStatus.IsangoBookingCancel ==
                            OptionCancelStatus.Failed.ToString()
                                ? Constants.Constant.CancellationFailed
                                : Constants.Constant.CancellationSucceed;
                        cancellationResponse.Remark = Constants.Constant.NonCancellableProductRemark;
                        logInfoSave(tokenValue, "NonCancellableProduct", SerializeDeSerializeHelper.Serialize(cancellationResponse));
                    }

                    //preparing cancel booking mail data
                    cancelBookingMailData = _cancellationMapper.PrepareCancelBookingMailData(cancellationRequest,
                        cancelBookingMailData, cancellationResponse, userData, cancellationPolicyData);

                    if (string.IsNullOrEmpty(cancelBookingMailData.PaymentRefundAmount))
                    {
                        cancelBookingMailData.PaymentRefundAmount = "";
                    }
                    else
                    {
                        var currencyCode = bookingData.CurrencyIsoCode?.ToUpperInvariant();
                        cancelBookingMailData.PaymentRefundAmount += " " + currencyCode;
                    }

                    if (cancellationRequest?.CancellationParameters?.BookedOptionId != null)
                    {
                        cancelBookingMailData.BookedOptionID = cancellationRequest?.CancellationParameters?.BookedOptionId;
                    }
                    var watchSendCancelBookingMail = Stopwatch.StartNew();
                    //send cancel booking mail to customer support
                    Task.Run(() => _cancellationService.SendCancelBookingMail(cancelBookingMailData));
                    watchSendCancelBookingMail.Stop();
                    _log.WriteTimer("SendCancelBookingMail", TimerMethodName, cancellationPolicyData.ApiTypeId, watchSendCancelBookingMail.Elapsed.ToString());
                    logInfoSave(tokenValue, "SendCancelBookingMail End");
                }
                else
                {
                    cancellationResponse =
                        _cancellationMapper.MapCancellationResponseAfterCancellationSteps(cancellationResponse,
                            isangoDbCancelled, OptionCancelStatus.Failed);
                    logInfoSave(tokenValue, "IsangoDBCancelledFailed", SerializeDeSerializeHelper.Serialize(cancellationResponse));
                }
            }
            else
            {
                cancellationResponse = _cancellationMapper.MapCancellationStatusResponse(cancellationStatus);
                cancellationResponse.Remark = Constants.Constant.RefundAmountRemark;
                logInfoSave(tokenValue, "RefundAmountGreaterThanSellPrice", SerializeDeSerializeHelper.Serialize(cancellationResponse));
            }

            var watchInsertOrUpdateCancel = Stopwatch.StartNew();
            InsertOrUpdateCancelStatus(cancellationResponse, cancellationRequest.CancellationParameters.BookedOptionId);
            watchInsertOrUpdateCancel.Stop();
            _log.WriteTimer("InsertOrUpdateCancelStatus", TimerMethodName, string.Empty, watchInsertOrUpdateCancel.Elapsed.ToString());
            logInfoSave(tokenValue, "CancelBooking End", SerializeDeSerializeHelper.Serialize(cancellationResponse));
            
            watchWholeCancellation.Stop();
            _log.WriteTimer("WatchWholeCancellation", TimerMethodName, string.Empty, watchWholeCancellation.Elapsed.ToString());
            return GetResponseWithActionResult(cancellationResponse);
        }


        [Route("OnlyCancelAPIBooking")]
        [HttpPost]
        [ValidateModel]
        
        public IActionResult OnlyCancelAPIBooking(OnlyAPICancellationRequest onlyAPICancellationRequest)
        {
            var tokenValue = "";
            var bookingStatus = "Cancellation is either done or the deadline has passed.";
            var cancellationRequest = new CancellationRequest();
            try
            {
                cancellationRequest.BookingRefNo = onlyAPICancellationRequest.BookingRefNo;  //required
                cancellationRequest.IsBookingManager = true;
                cancellationRequest.UserName = onlyAPICancellationRequest.UserName; //required
                cancellationRequest.CancellationParameters = new CancellationParameters();
                cancellationRequest.CancellationParameters.BookedOptionId = onlyAPICancellationRequest.BookedOptionId;//required



                var supplierDataList = _cancellationService.GetSupplierCancellationDataListAsync(cancellationRequest.BookingRefNo)
                        .GetAwaiter().GetResult();

                var supplierData = supplierDataList?.Where(x => x.BookedOptionId == cancellationRequest?.CancellationParameters?.BookedOptionId)?.FirstOrDefault();

                var userData = _cancellationHelper.GetUserCancellationPermission(cancellationRequest.UserName);
                var bookingData = _cancellationHelper.GetBookingDetails(cancellationRequest, userData);
                var cancellation = _cancellationMapper.MapCancellationRequest(cancellationRequest, userData, bookingData);

                var booking = _cancellationMapper.MapSupplierDataForBookedProduct(supplierData, bookingData,
                                    cancellation);

                tokenValue = cancellationRequest.BookingRefNo + "_" + cancellationRequest.CancellationParameters.BookedOptionId;

                var supplierCancellation = _bookingService
                                       .CancelSupplierBookingAsync(booking, cancellation?.TokenId ?? tokenValue, cancellationRequest.IsBookingManager).GetAwaiter()
                                       .GetResult();
                if (supplierCancellation)
                {
                    bookingStatus = "Cancelled Successfully.";
                    return GetResponseWithActionResult(bookingStatus);
                }
            }

            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "Cancellation",
                    MethodName = "OnlyCancelAPIBookings",
                    Token = tokenValue
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return GetResponseWithActionResult(bookingStatus);
        }


        [Route("GetCancellationPolicyForBookingOption")]

        [HttpGet]
        public CancellationPolicyDetailResponse GetCancellationPolicyForBookingOption(string bookingRefNo, int bookedOptionId)
        {
            try
            {
                var spId = 0;
                var bookingDetail = _bookingService.GetBookingData(bookingRefNo);
                var currencyIsoCode = bookingDetail?.CurrencyIsoCode;
                var cancellationPolicyData = _cancellationService
                    .GetCancellationPolicyDetailAsync(bookingRefNo, bookedOptionId, currencyIsoCode, spId).GetAwaiter()
                    .GetResult();
                var cancellationPolicyDetail =
                    _cancellationMapper.MapCancellationPolicyDetail(cancellationPolicyData, currencyIsoCode);
                return cancellationPolicyDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Private Methods

        /// <summary>
        /// Private method to insert or update cancellation status in db used by cancel booking
        /// </summary>
        /// <param name="cancellationResponse"></param>
        /// <param name="bookedOptionId"></param>
        private void InsertOrUpdateCancelStatus(CancellationResponse cancellationResponse, int bookedOptionId)
        {
            var allCancelStatus = _cancellationMapper.MapCancellationStatus(cancellationResponse, bookedOptionId);
            _cancellationService.InsertOrUpdateCancellationStatus(allCancelStatus);
        }

        private void logInfoSave(string token, string dataPass, string logData = "")
        {
            var logInfo = dataPass;
            var isangoLogEntity = new IsangoErrorEntity
            {
                ClassName = "CancellationController",
                MethodName = "CancelBooking",
                Token = token,
                Params = !String.IsNullOrEmpty(logData) ? (logInfo + "," + logData) : logInfo
            };
            _log.Info(isangoLogEntity);
        }

        #endregion Private Methods
    }
}