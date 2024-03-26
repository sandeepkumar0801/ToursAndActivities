using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Logger.Contract;
using ServiceAdapters.HB.HB.Commands.Contract;
using ServiceAdapters.HB.HB.Converters.Contracts;
using ServiceAdapters.HB.HB.Entities;
using ServiceAdapters.HB.HB.Entities.Booking;
using ServiceAdapters.HB.HB.Entities.Calendar;
using ServiceAdapters.HB.HB.Entities.Cancellation;
using ServiceAdapters.HB.HB.Entities.ContentMulti;
using Util;
using ApiBooking = ServiceAdapters.HB.HB.Entities.Booking.BookingDetail;
using DetailFull = ServiceAdapters.HB.HB.Entities.ActivityDetailFull;
using DetailSimple = ServiceAdapters.HB.HB.Entities.ActivityDetail;

namespace ServiceAdapters.HB
{
    public class HBAdapter : IHBAdapter, IAdapter
    {
        #region "Private Members"

        private readonly IHbBookingCancelCmdHandler _hbBookingCancelCmdHandler;
        private readonly IHbBookingCancelSimulationCmdHandler _hbBookingCancelSimulationCmdHandler;
        private readonly IHbBookingConfirmCmdHandler _hbBookingConfirmCmdHandler;
        private readonly IHbDeatilCmdHandler _hbDeatilCmdHandler;
        private readonly IHbDetailFullCmdHandler _hbDetailFullCmdHandler;
        private readonly IHBCalendarCmdHandler _hbCalendarCmdHandler;
        private readonly IHBContentMultiCmdHandler _hbContentMultiCmdHandler;
        private readonly IHbGetBookingDetailCmdHandler _hbGetBookingDetailCmdHandler;
        private readonly IHbSearchCmdHandler _hbSearchCmdHandler;
        private readonly IHbBookingCancelConverter _hbBookingCancelConverter;
        private readonly IHbBookingCancelSimulationConverter _hbBookingCancelSimulationConverter;
        private readonly IHbBookingConfirmConverter _hbBookingConfirmConverter;
        private readonly IHbDetailConverter _hbDetailConverter;
        private readonly IHbCalendarConverter _hbGetCalendarConverter;

        private readonly IHbGetBookingDetailConverter _hbGetBookingDetailConverter;
        private readonly ILogger _log;
        private bool isRollbackLiveAPIBookingsOtherThanPROD;

        #endregion "Private Members"

        public HBAdapter(IHbBookingCancelCmdHandler hbBookingCancelCmdHandler,
            IHbBookingCancelSimulationCmdHandler hbBookingCancelSimulationCmdHandlerCmdHandler,
            IHbBookingConfirmCmdHandler hbBookingConfirmCmdHandler,
            IHbDeatilCmdHandler hbDeatilCmdHandler,
            IHbDetailFullCmdHandler hbDetailFullCmdHandler,
            IHBCalendarCmdHandler hbCalendarCmdHandler,
            IHBContentMultiCmdHandler hbContentMultiCmdHandler,
            IHbGetBookingDetailCmdHandler hbGetBookingDetailCmdHandler,
            IHbSearchCmdHandler hbSearchCmdHandler,
            IHbBookingCancelConverter hbBookingCancelConverter,
            IHbBookingCancelSimulationConverter hbBookingCancelSimulationConverter,
            IHbBookingConfirmConverter hbBookingConfirmConverter,
            IHbDetailConverter hbDetailConverter,
            IHbGetBookingDetailConverter hbGetBookingDetailConverter,

            IHbCalendarConverter hbGetCalendarConverter,
            ILogger log
            )
        {
            _hbBookingCancelCmdHandler = hbBookingCancelCmdHandler;
            _hbBookingCancelSimulationCmdHandler = hbBookingCancelSimulationCmdHandlerCmdHandler;
            _hbBookingConfirmCmdHandler = hbBookingConfirmCmdHandler;
            _hbDeatilCmdHandler = hbDeatilCmdHandler;
            _hbDetailFullCmdHandler = hbDetailFullCmdHandler;
            _hbCalendarCmdHandler = hbCalendarCmdHandler;
            _hbContentMultiCmdHandler = hbContentMultiCmdHandler;
            _hbGetBookingDetailCmdHandler = hbGetBookingDetailCmdHandler;
            _hbSearchCmdHandler = hbSearchCmdHandler;
            _hbBookingCancelConverter = hbBookingCancelConverter;
            _hbBookingCancelSimulationConverter = hbBookingCancelSimulationConverter;
            _hbBookingConfirmConverter = hbBookingConfirmConverter;
            _hbDetailConverter = hbDetailConverter;
            _hbGetBookingDetailConverter = hbGetBookingDetailConverter;
            _hbGetCalendarConverter = hbGetCalendarConverter;
            isRollbackLiveAPIBookingsOtherThanPROD = ConfigurationManagerHelper.GetValuefromAppSettings("RollbackLiveAPIBookingsOtherThanPROD") == "1";
            _log = log;
        }

        public async Task<object> SearchAsync(HotelbedCriteriaApitude hotelbedCriteriaApitude, string token)
        {
            //if (searchRq == null) return null;
            //var inputContext = new InputContext
            //{
            //    MethodType = MethodType.Search,
            //    Filters = searchRq.Filters,
            //    FromDate = searchRq.From,
            //    ToDate = searchRq.To,
            //    Language = searchRq.Language,
            //    Order = searchRq.Order,
            //    Pagination = searchRq.Pagination
            //};

            var returnValue = await _hbSearchCmdHandler.ExecuteAsync(hotelbedCriteriaApitude, token, MethodType.Search);
            if (returnValue != null)
            {
                returnValue = _hbDetailConverter.Convert(returnValue, MethodType.Search);
            }
            return returnValue;
        }

        /// <summary>
        /// Get product availability and price based on check-in date, Will be adding check-out date as Check-in + 6 to get multi-day options
        ///
        /// </summary>
        /// <param name="hotelbedCriteriaApitude"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Isango.Entities.Activities.Activity> ActivityDetailsAsync(HotelbedCriteriaApitude hotelbedCriteriaApitude, string token)
        {
            if (hotelbedCriteriaApitude == null) return null;

            var resultActivity = default(Isango.Entities.Activities.Activity);

            var returnValue = await _hbDeatilCmdHandler.ExecuteAsync(hotelbedCriteriaApitude, token, MethodType.Detail);

            if (returnValue != null)
            {
                var responseObject = returnValue as DetailSimple.ActivityDetailRS;
                if (responseObject != null)
                {
                    var convertedActivity = _hbDetailConverter.Convert(returnValue, MethodType.Detailfull, hotelbedCriteriaApitude);
                    resultActivity = convertedActivity as Isango.Entities.Activities.Activity;
                }
            }
            return resultActivity;
        }

        /// <summary>
        /// Get gull content of activity from api
        /// </summary>
        /// <param name="hotelbedCriteriaApitude"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<DetailFull.ActivityDetailFullRS> ActivityDetailsFullAsync(HotelbedCriteriaApitude hotelbedCriteriaApitude, string token)
        {
            var activityDetailRs = default(DetailFull.ActivityDetailFullRS);
            var returnValue = await _hbDetailFullCmdHandler.ExecuteAsync(hotelbedCriteriaApitude, token, MethodType.Detailfull);
            if (returnValue != null)
            {
                activityDetailRs = returnValue as DetailFull.ActivityDetailFullRS;
            }

            return activityDetailRs;
        }

        /// <summary>
        /// Get calendar content of activity from api
        /// </summary>
        /// <param name="hotelbedCriteriaApitude"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Tuple<List<Isango.Entities.Activities.Activity>, CalendarRs>> CalendarAsync(HotelbedCriteriaApitudeFilter hotelbedCriteriaApitude, string token)
        {
            var activitiesIsango = default(List<Isango.Entities.Activities.Activity>);
            var result = default(Tuple<List<Isango.Entities.Activities.Activity>, CalendarRs>);
            var calendarRs = default(CalendarRs);
            var returnValueCalendarRs = await _hbCalendarCmdHandler.ExecuteAsync(hotelbedCriteriaApitude, token, MethodType.Calendar);

            if (returnValueCalendarRs != null)
            {
                calendarRs = returnValueCalendarRs as CalendarRs;
                var convertedActivities = _hbGetCalendarConverter.Convert(returnValueCalendarRs, MethodType.Calendar, hotelbedCriteriaApitude);
                activitiesIsango = convertedActivities as List<Isango.Entities.Activities.Activity>;
            }

            result = Tuple.Create(activitiesIsango, calendarRs);

            return result;
        }

        public List<HotelBedsSelectedProduct> BookingConfirm(Isango.Entities.Booking.Booking booking, string token, out string request, out string response)
        {
            var returnValue = new List<HotelBedsSelectedProduct>();
            var isMock = false;
            request = string.Empty;
            response = string.Empty;

            object convertedResult = null;
            var selectedProducts = booking?.SelectedProducts.Where(x => x.APIType == APIType.Hotelbeds).ToList();
            var backupBookingId = booking.BookingId;

            if (selectedProducts?.Count > 0)
            {
                foreach (var item in selectedProducts)
                {
                    //using (new PerfTimer("Execute - The customer enter details"))
                    booking.BookingId = item.ProductOptions.FirstOrDefault().Id;

                    var responseObject = _hbBookingConfirmCmdHandler.Execute(booking, token, MethodType.Bookingconfirm, out request, out response);
                    var bookingConfirmRS = SerializeDeSerializeHelper.DeSerialize<ApiBooking.BookingDetailRs>(response);

                    //Mocking api booking
                    /*

                    request = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Cancellation Policy in Booking Response\03 Hotelbeds\3 Hotelbeds T&A - BookingRequest.json");
                    response = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Cancellation Policy in Booking Response\03 Hotelbeds\3 Hotelbeds T&A - BookingResponseMock.json");
                    var bookingConfirmRS = SerializeDeSerializeHelper.DeSerialize<ApiBooking.BookingDetailRs>(response);
                    var responseObject = bookingConfirmRS;
                    isMock = true;
                    //*/

                    if (responseObject != null
                        && Convert.ToInt32(bookingConfirmRS?.Errors?.Count) == 0
                        && bookingConfirmRS?.Booking != null
                    )
                    {
                        try
                        {
                            convertedResult = _hbBookingConfirmConverter.Convert(responseObject, MethodType.Bookingconfirm, booking);

                            #region cancellation after confirmation

                            //#TODO Adding cancellation after confirmation so that it api live booking can be revered immediately
                            // uncomment to cancel api booking after creation
                            //-----------------------------------------------
                            ///*
                            if (isRollbackLiveAPIBookingsOtherThanPROD == true && isMock == false)
                            {
                                var cancelreq = string.Empty;
                                var cancelres = string.Empty;
                                try
                                {
                                    var ConfimredTicket = convertedResult as List<HotelBedsSelectedProduct>;
                                    var apiBookingRefNumbers = ConfimredTicket?
                                                            .Where(x => !string.IsNullOrWhiteSpace(x.FileNumber))?
                                                            .Select(y => y.FileNumber)?.Distinct()?
                                                            .ToArray();

                                    var apiBookingRefNumber = "N /A";
                                    if (apiBookingRefNumbers?.Length > 0)
                                    {
                                        apiBookingRefNumber = string.Join(",", apiBookingRefNumbers);
                                    }
                                    this.BookingCancel(apiBookingRefNumber, "en", token, out cancelreq, out cancelres);
                                }
                                catch (Exception ex)
                                {
                                    var isangoErrorEntity = new IsangoErrorEntity
                                    {
                                        ClassName = nameof(HBAdapter),
                                        MethodName = nameof(BookingConfirm) + "RollbackLiveAPIBookingsOtherThanPROD",
                                        Token = token,
                                        Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                                    };
                                    booking.UpdateErrors(CommonErrorCodes.SupplierBookingError, System.Net.HttpStatusCode.BadGateway, cancelres);
                                }
                                //-----------------------------------------------
                            }
                            //*/

                            #endregion cancellation after confirmation

                            var convertedResultReturnValue = convertedResult as List<HotelBedsSelectedProduct>;
                            foreach (var sp in convertedResultReturnValue)
                            {
                                returnValue.Add(sp);
                            }
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = nameof(HBAdapter),
                                MethodName = nameof(BookingConfirm),
                                Token = token,
                                Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                            };
                            booking.UpdateErrors(CommonErrorCodes.SupplierBookingError, System.Net.HttpStatusCode.BadGateway, response);
                        }
                    }
                    else
                    {
                        try
                        {
                            var isRateKeyError = bookingConfirmRS?.Errors?
                                            .Any(x => x?.Text?.ToLower().Contains("the ratekey") == true
                                                && x?.Text?.ToLower().Contains("incorrect") == true
                                                && x?.Text?.ToLower().Contains("it does not exist") == true
                                            );

                            var isRateKeyError1 = response?.ToLower()?.Contains("the ratekey") == true
                                    && response?.ToLower()?.Contains("incorrect") == true
                                    && response?.ToLower()?.Contains("it does not exist") == true;

                            if (isRateKeyError == true || isRateKeyError1 == true)
                            {
                                returnValue = BookingConfirmAfterRegeneratingRateKey(booking, token, out request, out response);
                            }
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = nameof(HBAdapter),
                                MethodName = nameof(BookingConfirm) + " Renew Availability",
                                Token = token,
                                Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                            };
                            booking.UpdateErrors(CommonErrorCodes.SupplierBookingError, System.Net.HttpStatusCode.BadGateway, response);
                        }
                    }
                }
            }
            booking.BookingId = backupBookingId;
            return returnValue;
        }

        public List<HotelBedsSelectedProduct> GetBookingDetailAsync(BookingRq getBookingRq, string token, out string request, out string response)
        {
            var inputContext = new InputContext
            {
                MethodType = MethodType.Bookingdetail,
                BookingReference = getBookingRq.BookingReference,
                ClientReference = getBookingRq.CustomerRefrerence,
                FromDate = getBookingRq.StartDate,
                ToDate = getBookingRq.EndDate,
                Language = getBookingRq.Language,
                Holder = new HB.Entities.Booking.Holder { Name = getBookingRq.HolderName, Surname = getBookingRq.HolderSurname }
            };

            request = string.Empty;
            response = string.Empty;

            object returnValue = _hbGetBookingDetailCmdHandler.ExecuteAsync(inputContext, token, MethodType.Bookingdetail);
            request = SerializeDeSerializeHelper.Serialize(getBookingRq);
            response = Convert.ToString(((Task<object>)(returnValue)).Result);
            returnValue = _hbGetBookingDetailConverter.Convert(inputContext, MethodType.Bookingdetail);
            return (List<HotelBedsSelectedProduct>)returnValue;
        }

        public CancellationRS BookingCancel(string referenceNumber, string language, string token, out string request, out string response)
        {
            var defaultRs = default(CancellationRS);
            var inputContext = new InputContext
            {
                MethodType = MethodType.Bookingcancel,
                ClientReference = referenceNumber,
                Language = language
            };
            request = string.Empty;
            response = string.Empty;
            var returnValue = _hbBookingCancelCmdHandler.Execute(inputContext, token, MethodType.Bookingcancel, out request, out response);
            var deSerializedResult = SerializeDeSerializeHelper.DeSerialize<CancellationRS>(response);
            if (returnValue != null && deSerializedResult != null)
            {
                return deSerializedResult;
            }
            return defaultRs;
        }

        public CancellationRS BookingCancelSimulation(string referenceNumber, string language, string token, out string request, out string response)
        {
            var defaultRs = default(CancellationRS);
            var inputContext = new InputContext
            {
                MethodType = MethodType.Bookingcancelsimulation,
                ClientReference = referenceNumber,
                Language = language
            };
            var returnValue = _hbBookingCancelSimulationCmdHandler.Execute(inputContext, token, MethodType.Bookingcancelsimulation, out request, out response);
            if (returnValue != null)
            {
                return SerializeDeSerializeHelper.DeSerialize<CancellationRS>(response);
            }
            else
            {
                return defaultRs;
            }
        }

        /// <summary>
        /// Get calendar content of activity from api
        /// </summary>
        /// <param name="hotelbedCriteriaApitude"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ContentMultiRS> ContentMultiAsync(ContentMultiRq hotelbedCriteriaApitude, string token)
        {
            var activityDetailRs = default(ContentMultiRS);
            var returnValue = await _hbContentMultiCmdHandler.ExecuteAsync(hotelbedCriteriaApitude, token, MethodType.ContentMulti);
            if (returnValue != null)
            {
                activityDetailRs = returnValue as ContentMultiRS;
            }

            return activityDetailRs;
        }

        /// <summary>
        /// Try to rebook with new ratekey for same option
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private List<HotelBedsSelectedProduct> BookingConfirmAfterRegeneratingRateKey(Isango.Entities.Booking.Booking booking, string token, out string request, out string response)
        {
            var returnValue = new List<HotelBedsSelectedProduct>();

            request = string.Empty;
            response = string.Empty;
            var isMock = false;

            object convertedResult = null;
            var selectedProducts = booking?.SelectedProducts.Where(x => x.APIType == APIType.Hotelbeds).ToList();
            var backupBookingId = booking.BookingId;

            if (selectedProducts?.Count > 0)
            {
                foreach (var item in selectedProducts)
                {
                    if (booking?.SelectedProducts?.Count > 0)
                    {
                        UpdateExpiredRateKeys(booking, token);
                        //using (new PerfTimer("Execute - The customer enter details"))
                        var responseObject = _hbBookingConfirmCmdHandler.Execute(booking, token, MethodType.Bookingconfirm, out request, out response);
                        var bookingConfirmRS = SerializeDeSerializeHelper.DeSerialize<ApiBooking.BookingDetailRs>(response);
                        if (responseObject != null
                            && Convert.ToInt32(bookingConfirmRS?.Errors?.Count) == 0
                            && bookingConfirmRS?.Booking != null
                        )
                        {
                            //#TODO Adding cancellation after confirmation so that it can be canceled
                            //FO not Check it in
                            try
                            {
                                convertedResult = _hbBookingConfirmConverter.Convert(responseObject, MethodType.Bookingconfirm, booking);

                                #region cancellation after confirmation

                                //#TODO Adding cancellation after confirmation so that it api live booking can be revered immediately
                                // uncomment to cancel api booking after creation
                                //-----------------------------------------------
                                ///*
                                if (isRollbackLiveAPIBookingsOtherThanPROD == true && isMock == false)
                                {
                                    var cancelreq = string.Empty;
                                    var cancelres = string.Empty;
                                    try
                                    {
                                        var ConfimredTicket = convertedResult as List<HotelBedsSelectedProduct>;
                                        var apiBookingRefNumbers = ConfimredTicket?
                                                                .Where(x => !string.IsNullOrWhiteSpace(x.FileNumber))?
                                                                .Select(y => y.FileNumber)?.Distinct()?
                                                                .ToArray();

                                        var apiBookingRefNumber = "N/A";
                                        if (apiBookingRefNumbers?.Length > 0)
                                        {
                                            apiBookingRefNumber = string.Join(",", apiBookingRefNumbers);
                                        }
                                        this.BookingCancel(apiBookingRefNumber, "en", token, out cancelreq, out cancelres);
                                    }
                                    catch (Exception ex)
                                    {
                                        var isangoErrorEntity = new IsangoErrorEntity
                                        {
                                            ClassName = nameof(HBAdapter),
                                            MethodName = nameof(BookingConfirm) + "RollbackLiveAPIBookingsOtherThanPROD",
                                            Token = token,
                                            Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                                        };
                                        booking.UpdateErrors(CommonErrorCodes.SupplierBookingError, System.Net.HttpStatusCode.BadGateway, cancelres);
                                    }
                                    //-----------------------------------------------
                                }
                                //*/

                                #endregion cancellation after confirmation

                                var convertedResultReturnValue = convertedResult as List<HotelBedsSelectedProduct>;
                                foreach (var sp in convertedResultReturnValue)
                                {
                                    returnValue.Add(sp);
                                }
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = nameof(HBAdapter),
                                    MethodName = nameof(BookingConfirmAfterRegeneratingRateKey),
                                    Token = token,
                                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                                };
                                booking.UpdateErrors(CommonErrorCodes.SupplierBookingError, System.Net.HttpStatusCode.BadGateway, response);
                            }
                        }
                    }
                }
            }
            booking.BookingId = backupBookingId;
            return returnValue;
        }

        /// <summary>
        /// Check Activity Detail and get new rate key for the options in booking.
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        private void UpdateExpiredRateKeys(Isango.Entities.Booking.Booking booking, string token)
        {
            var selectedProducts = booking.SelectedProducts
                                   .Where(x => x.APIType == Isango.Entities.Enums.APIType.Hotelbeds
                                   && x.ProductOptions.FirstOrDefault(y => y.Id == booking.BookingId) != null
                                   )
                                   .ToList()
                                   .OfType<HotelBedsSelectedProduct>().ToList();

            if (selectedProducts?.Count > 0)
            {
                foreach (var selectedProduct in booking?.SelectedProducts)
                {
                    if (selectedProduct.APIType == APIType.Hotelbeds)
                    {
                        if (selectedProduct?.ProductOptions?.Count > 0)
                        {
                            foreach (var po in selectedProduct?.ProductOptions)
                            {
                                var activityOpiton = default(ActivityOption);
                                try
                                {
                                    activityOpiton = (ActivityOption)po;
                                    var travelInfo = activityOpiton.TravelInfo;

                                    var ages = new Dictionary<PassengerType, int>();
                                    var passengerAgeGroupIds = new Dictionary<PassengerType, int>();
                                    foreach (var customers in activityOpiton?.Customers)
                                    {
                                        if (!ages.Keys.Contains(customers.PassengerType))
                                        {
                                            ages.Add(customers.PassengerType, customers.Age);
                                        }

                                        if (!passengerAgeGroupIds.Keys.Contains(customers.PassengerType))
                                        {
                                            passengerAgeGroupIds.Add(customers.PassengerType, 0);
                                        }
                                    }

                                    var hotelbedCriteriaApitude = new HotelbedCriteriaApitude
                                    {
                                        NoOfPassengers = travelInfo.NoOfPassengers,
                                        Ages = ages,
                                        FactSheetIds = new List<int> { 0 },
                                        CheckinDate = travelInfo.StartDate,
                                        CheckoutDate = travelInfo.StartDate.AddDays(6),

                                        Language = booking?.Language?.Code ?? "en",
                                        //PassengerAgeGroupIds = passengerAgeGroupIds,
                                        PassengerInfo = null,

                                        ActivityCode = activityOpiton.SupplierOptionCode,
                                        IsangoActivityId = selectedProduct?.Id.ToString(),
                                        ServiceOptionId = activityOpiton.ServiceOptionId.ToString(),
                                        ModalityCode = activityOpiton.Code
                                    };

                                    var act = ActivityDetailsAsync(hotelbedCriteriaApitude, token)?.GetAwaiter().GetResult();

                                    var refreshedActivityOption = act.ProductOptions?.Cast<ActivityOption>()?.ToList()?
                                        .FirstOrDefault(x => x.SupplierOptionCode == activityOpiton.SupplierOptionCode
                                                && x.Name == activityOpiton.Name
                                                );

                                    if (!string.IsNullOrWhiteSpace(refreshedActivityOption?.RateKey))
                                    {
                                        activityOpiton.RateKey = refreshedActivityOption?.RateKey;
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}