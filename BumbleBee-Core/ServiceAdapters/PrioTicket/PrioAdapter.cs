using Factories;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Prio;
using Logger.Contract;
using ServiceAdapters.PrioTicket.Constants;
using ServiceAdapters.PrioTicket.PrioTicket.Commands.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.PrioTicket
{
    public class PrioAdapter : IPrioTicketAdapter, IAdapter
    {
        #region "Private Members"

        private readonly List<PassengerType> _validPassengerTypes = new List<PassengerType> { PassengerType.Adult, PassengerType.Child, PassengerType.Infant, PassengerType.Senior, PassengerType.Family, PassengerType.Student, PassengerType.Youth };

        private static readonly string PrioTokenKey;
        private static readonly string PrioDistributionId;

        private readonly IAvailablityCmdHandler _availablityCmdHandler;
        private readonly ICancelBookingCmdHandler _cancelBookingCmdHandler;
        private readonly ICancelReservationCmdHandler _cancelReservationCmdHandler;
        private readonly ICreateBookingCmdHandler _createBookingCmdHandler;
        private readonly IReservationCmdHandler _reservationCmdHandler;
        private readonly ITicketDetailsCmdHandler _ticketDetailsCmdHandler;
        private readonly ITicketListCmdHandler _ticketListCmdHandler;

        private readonly IAvailablityConverter _availablityConverter;
        private readonly ICancelBookingConverter _cancelBookingConverter;
        private readonly ICancelReservationConverter _cancelReservationConverter;
        private readonly IReservationConverter _reservationConverter;
        private readonly ICreateBookingConverter _createBookingConverter;
        private readonly ITicketDetailsConverter _ticketDetailsConverter;
        private readonly ITicketListConverter _ticketListConverter;
        private readonly ILogger _logger;
        private static readonly int _maxParallelThreadCount;

        #endregion "Private Members"

        #region "Constructor"

        static PrioAdapter()
        {
            PrioTokenKey = ConfigurationManagerHelper.GetValuefromAppSettings("PrioTokenKey");
            PrioDistributionId = ConfigurationManagerHelper.GetValuefromAppSettings("PrioDistributorId");
            _maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
        }

        public PrioAdapter(IAvailablityCmdHandler availablityCmdHandler, ICancelBookingCmdHandler cancelBookingCmdHandler,
            ICancelReservationCmdHandler cancelReservationCmdHandler, ICreateBookingCmdHandler createBookingCmdHandler,
            IReservationCmdHandler reservationCmdHandler, ITicketDetailsCmdHandler ticketDetailsCmdHandler, IAvailablityConverter availablityConverter,
            ICancelBookingConverter cancelBookingConverter, ICancelReservationConverter cancelReservationConverter,
            IReservationConverter reservationConverter,
            ICreateBookingConverter createBookingConverter,
            ITicketDetailsConverter ticketDetailsConverter,
            ILogger logger, ITicketListConverter ticketListConverter,
            ITicketListCmdHandler ticketListCmdHandler)
        {
            _availablityCmdHandler = availablityCmdHandler;
            _cancelBookingCmdHandler = cancelBookingCmdHandler;
            _cancelReservationCmdHandler = cancelReservationCmdHandler;
            _createBookingCmdHandler = createBookingCmdHandler;
            _reservationCmdHandler = reservationCmdHandler;
            _ticketDetailsCmdHandler = ticketDetailsCmdHandler;
            _availablityConverter = availablityConverter;
            _cancelBookingConverter = cancelBookingConverter;
            _cancelReservationConverter = cancelReservationConverter;
            _reservationConverter = reservationConverter;
            _createBookingConverter = createBookingConverter;
            _ticketDetailsConverter = ticketDetailsConverter;
            _ticketListConverter = ticketListConverter;
            _ticketListCmdHandler = ticketListCmdHandler;
            _logger = logger;
        }

        #endregion "Constructor"

        /// <summary>
        /// Update Option for Prio Activity
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<Activity> UpdateOptionforPrioActivity(PrioCriteria criteria, string token)
        {
            var activities = new List<Activity>();
            int.TryParse(criteria.ActivityCode, out var activityCode);
            var activity = new Activity { ID = activityCode };
            var productOptions = new List<ProductOption>();

            if (criteria?.SupplierOptionCodes != null)
            {
                //var taskArray = new Task<List<ActivityOption>>[criteria.SupplierOptionCodes.Count];
                //var count = 0;

                foreach (var code in criteria.SupplierOptionCodes)
                {
                    try
                    {
                        var data = CreateActivityOptions(criteria, code, token)?.GetAwaiter().GetResult();
                        if (data != null && data.Count > 0)
                        {
                            productOptions.AddRange(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() =>
                                    _logger.Error(new IsangoErrorEntity
                                    {
                                        ClassName = "PrioTicket.PrioAdapter",
                                        MethodName = "UpdateOptionforPrioActivity"
                                    }, ex)
                                );
                    }
                  }
                    //foreach (var code in criteria.SupplierOptionCodes)
                    //{
                    //    var codeCopy = code;
                    //    taskArray[count] = Task.Run(() => CreateActivityOptions(criteria, codeCopy, token));
                    //    taskArray[count].ConfigureAwait(false);
                    //    count++;
                    //}
                    //if (taskArray?.Length > 0)
                    //{
                    //    Task.WaitAll(taskArray);
                    //    Parallel.ForEach(taskArray, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, task =>
                    //    {
                    //        try
                    //        {
                    //            var data = task.GetAwaiter().GetResult();
                    //            if (data != null)
                    //                productOptions.AddRange(data);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            Task.Run(() =>
                    //                        _logger.Error(new IsangoErrorEntity
                    //                        {
                    //                            ClassName = "PrioTicket.PrioAdapter",
                    //                            MethodName = "UpdateOptionforPrioActivity"
                    //                        }, ex)
                    //                    );
                    //        }
                    //    });
                    //}
                    if (productOptions.Count > 0)
                    activity.ProductOptions = productOptions;
                activities.Add(activity);
                return activities;
            }
            return null;
        }

        private async Task<List<ActivityOption>> CreateActivityOptions(PrioCriteria criteria, string code, string token)
        {
            try
            {
                var apiActivityId = code;
                var taskAvailablity = await GetPrioAvailablityAsync(criteria, PrioDistributionId, PrioTokenKey, token, apiActivityId);
                var prioAvailablity = taskAvailablity;
                //taskAvailablity?.Wait();
                //var prioAvailablity = taskAvailablity?.GetAwaiter().GetResult();
                if (prioAvailablity == null) return await Task.FromResult<List<ActivityOption>>(null);

                if (prioAvailablity.AvailabilityStatus == AvailabilityStatus.AVAILABLE)
                {
                    var taskCreateOption = new List<ActivityOption>();
                    if (prioAvailablity.IsTimeBasedOption)
                    {
                        taskCreateOption = await CreateOptionsAsync(prioAvailablity, criteria, token, apiActivityId);
                    }
                    else
                    {
                        taskCreateOption = await CreateOptionAsync(prioAvailablity, criteria, token, apiActivityId);
                    }
                    // taskCreateOption?.Wait();
                    //var activityOption = taskCreateOption?.GetAwaiter().GetResult();
                    var activityOptions = taskCreateOption;
                    if (activityOptions != null && activityOptions?.Count > 0)
                    {
                        foreach(var activityOption in activityOptions)
                        {
                            activityOption.SupplierOptionCode = apiActivityId;
                            activityOption.Code = apiActivityId;
                            activityOption.TravelInfo = new TravelInfo { NoOfPassengers = criteria.NoOfPassengers };
                        }
                    }
                    return await Task.FromResult(activityOptions);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "Prio.PrioAdapter",
                    MethodName = "CreateActivityOption",
                    Token = token
                };
                _logger.Error(isangoErrorEntity, ex);
            }
            return await Task.FromResult<List<ActivityOption>>(null);
        }

        /// <summary>
        /// Get PrioAvailablity Option
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="distributorId"></param>
        /// <param name="tokenKey"></param>
        /// <param name="apiLoggingToken"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActivityOption GetPrioAvailablity(PrioCriteria criteria, string distributorId, string tokenKey, string apiLoggingToken, string code)
        {
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (criteria != null)
            {
                var apiActivityId = code;
                _inputContext.ActivityId = apiActivityId;
                _inputContext.UserName = distributorId;
                _inputContext.Password = tokenKey;
                _inputContext.FromDateTime = criteria.CheckinDate;
                _inputContext.CheckInDate = criteria.CheckinDate.ToString(Constant.DateFormat);
                _inputContext.CheckOutDate = criteria.CheckoutDate.ToString(Constant.DateFormat);
                _inputContext.MethodType = MethodType.Availability;
                _returnValue = _availablityCmdHandler.Execute(_inputContext, _inputContext.Password, apiLoggingToken);
                try
                {
                    var castedResult = _returnValue as Task<object>;
                    if (castedResult != null)
                    {
                        _returnValue = castedResult.GetAwaiter().GetResult();
                    }
                }
                catch { }
                if (_returnValue != null)
                    try
                    {
                        _returnValue = _availablityConverter.Convert((EntityBase)_returnValue);
                    }
                    catch (Exception ex)
                    {
                        //ignored - Type cast Error in case of differnet response than what is expected.
                    }
            }
            return _returnValue as ActivityOption;
        }

        public async Task<ActivityOption> GetPrioAvailablityAsync(PrioCriteria criteria, string distributorId, string tokenKey, string apiLoggingToken, string code)
        {
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (criteria != null)
            {
                var apiActivityId = code;
                _inputContext.ActivityId = apiActivityId;
                _inputContext.UserName = distributorId;
                _inputContext.Password = tokenKey;
                _inputContext.FromDateTime = criteria.CheckinDate;
                _inputContext.CheckInDate = criteria.CheckinDate.ToString(Constant.DateFormat);
                _inputContext.CheckOutDate = criteria.CheckoutDate.ToString(Constant.DateFormat);
                _inputContext.MethodType = MethodType.Availability;
                _returnValue = await _availablityCmdHandler.ExecuteAsync(_inputContext, _inputContext.Password, apiLoggingToken);
                try
                {
                    var castedResult = _returnValue as Task<object>;
                    if (castedResult != null)
                    {
                        _returnValue = castedResult.GetAwaiter().GetResult();
                    }
                }
                catch { }
                if (_returnValue != null)
                    try
                    {
                        _returnValue = _availablityConverter.Convert((EntityBase)_returnValue);
                    }
                    catch (Exception ex)
                    {
                        //ignored - Type cast Error in case of differnet response than what is expected.
                    }
            }
            return _returnValue as ActivityOption;
        }

        /// <summary>
        /// Create Booking
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public PrioApi CreateBooking(PrioSelectedProduct selectedProduct, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (selectedProduct != null)
            {
                var productOptions = selectedProduct.ProductOptions.SingleOrDefault(x => x.IsSelected);
                _inputContext.ActivityId = Convert.ToString(((ActivityOption)productOptions)?.SupplierOptionCode);
                _inputContext.UserName = PrioDistributionId;
                _inputContext.Password = PrioTokenKey;

                AddPassengerCountInInputContext(productOptions, _inputContext);

                var leadPassenger = productOptions?.Customers.FirstOrDefault(x => x.IsLeadCustomer);

                _inputContext.BookingName = $"{leadPassenger?.FirstName} {leadPassenger?.LastName}";
                _inputContext.BookingEmail = leadPassenger?.Email;
                _inputContext.Street = selectedProduct.Supplier.AddressLine1;
                _inputContext.PostalCode = selectedProduct.Supplier.ZipCode;
                _inputContext.City = selectedProduct.Supplier.City;
                _inputContext.PhoneNumber = selectedProduct.Supplier.PhoneNumber;
                _inputContext.Notes = new List<string> { selectedProduct.SpecialRequest };
                _inputContext.Language = Constant.En;
                _inputContext.DistributorReference = selectedProduct.ReservationExpiry;

                if (((selectedProduct.PrioTicketClass == (int)TicketClass.TicketClassTwo) || (selectedProduct.PrioTicketClass == (int)TicketClass.TicketClassThree)) && selectedProduct.PrioBookingStatus.ToLower() == PrioApiStatus.Reserved.ToLower(CultureInfo.InvariantCulture))
                {
                    _inputContext.ReservationReference = selectedProduct.PrioReservationReference;
                }
                _inputContext.PrioTicketClass = selectedProduct.PrioTicketClass;
                _inputContext.MethodType = MethodType.CreateBooking;
                _returnValue = _createBookingCmdHandler.Execute(_inputContext, _inputContext.Password, token, out request, out response);
                //Mocking api booking
                /*
                var isMock = false;
                request = SerializeDeSerializeHelper.Serialize(_inputContext);
                response = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\5 PrioTicket - BookingResponse.json");
                var bookingConfirmRS = SerializeDeSerializeHelper.DeSerialize<ReservationRs>(response);
                _returnValue = bookingConfirmRS;
                isMock = true;
                //*/
                if (_returnValue == null)
                {
                    return null;
                }

                _returnValue = _createBookingConverter.Convert((EntityBase)_returnValue);
            }
            return (PrioApi)_returnValue;
        }

        /// <summary>
        /// Cancel Booking
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Tuple<string, string, string, DateTime> CancelBooking(PrioSelectedProduct selectedProduct, string token)
        {
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            try
            {
                if (selectedProduct != null)
                {
                    _inputContext.MethodType = MethodType.CancelBooking;
                    _inputContext.UserName = PrioDistributionId;
                    _inputContext.DistributorReference = selectedProduct.PrioApiConfirmedBooking.DistributorReference;
                    _inputContext.BookingReference = selectedProduct.PrioApiConfirmedBooking.BookingReference;
                    _returnValue = _cancelBookingCmdHandler.Execute(_inputContext, PrioTokenKey, token);
                    if (_returnValue == null)
                        return null;
                    _returnValue = _cancelBookingConverter.Convert((EntityBase)_returnValue);
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                       _logger.Error(new IsangoErrorEntity
                       {
                           ClassName = "Prio.PrioAdapter",
                           MethodName = "CancelBooking",
                           Token = token
                       }, ex)
                   );
            }

            return (Tuple<string, string, string, DateTime>)_returnValue;
        }

        /// <summary>
        /// CancelBooking
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public Tuple<string, string, string, DateTime> CancelBooking(PrioSelectedProduct selectedProduct, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();

            if (selectedProduct != null)
            {
                _inputContext.MethodType = MethodType.CancelBooking;
                _inputContext.UserName = PrioDistributionId;
                _inputContext.DistributorReference = selectedProduct.PrioApiConfirmedBooking.DistributorReference;
                _inputContext.BookingReference = selectedProduct.PrioApiConfirmedBooking.BookingReference;
                _returnValue = _cancelBookingCmdHandler.Execute(_inputContext, PrioTokenKey, token, out request, out response);
                if (_returnValue == null)
                    return null;
                _returnValue = _cancelBookingConverter.Convert((EntityBase)_returnValue);
            }

            return (Tuple<string, string, string, DateTime>)_returnValue;
        }

        /// <summary>
        /// Cancel Reservation
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public Tuple<string, string, string, DateTime> CancelReservation(PrioSelectedProduct selectedProduct, string referenceNumber, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (selectedProduct != null)
            {
                _inputContext.MethodType = MethodType.CancelReservation;
                _inputContext.UserName = PrioDistributionId;
                _inputContext.DistributorReference = referenceNumber;
                _inputContext.ReservationReference = selectedProduct.PrioReservationReference;
                _returnValue = _cancelReservationCmdHandler.Execute(_inputContext, PrioTokenKey, token, out request, out response);
                if (_returnValue != null)
                    _returnValue = _cancelReservationConverter.Convert((EntityBase)_returnValue);
            }

            return (Tuple<string, string, string, DateTime>)_returnValue;
        }

        /// <summary>
        /// Create Reservation
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="distributorReference"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Tuple<string, string, string> CreateReservation(PrioSelectedProduct selectedProduct,
            string distributorReference, out string request, out string response, string token)
        {
            request = string.Empty;
            response = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();

            if (selectedProduct != null)
            {
                var productOptions = selectedProduct.ProductOptions.SingleOrDefault(x => x.IsSelected);
                _inputContext.ActivityId = Convert.ToString(((ActivityOption)productOptions)?.SupplierOptionCode);
                _inputContext.UserName = PrioDistributionId;
                _inputContext.Password = PrioTokenKey;

                AddPassengerCountInInputContext(productOptions, _inputContext);

                _inputContext.DistributorReference = distributorReference;
                _inputContext.PickupPointId = string.Empty;
                if (selectedProduct.PickupPoints != null && string.Equals(selectedProduct.PickupPoints, Convert.ToString(PrioApiStatus.Mandatory), StringComparison.InvariantCultureIgnoreCase) && selectedProduct.PickupPointDetails != null && selectedProduct.PickupPointDetails.Length > 0)
                {
                    _inputContext.PickupPointId = selectedProduct.PickupPointDetails[0].PickupPointId; //Picking up First PickUp Point
                }

                if (productOptions != null)
                {
                    var priceAndAvailabiltySelected = productOptions.BasePrice.DatePriceAndAvailabilty.Where(x => x.Value.IsSelected);
                    var prioPandA = priceAndAvailabiltySelected.SingleOrDefault().Value as PrioPriceAndAvailability;
                    _inputContext.CheckInDate = prioPandA?.FromDateTime;
                    _inputContext.CheckOutDate = prioPandA?.ToDateTime;
                }

                _inputContext.MethodType = MethodType.Reservation;

                _returnValue = _reservationCmdHandler.Execute(_inputContext, _inputContext.Password, token, out request, out response);
                //Mocking api booking
                /*
                var isMock = false;
                request = SerializeDeSerializeHelper.Serialize(_inputContext);
                response = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\3 Hotelbeds T&A - BookingResponse.json");
                var bookingConfirmRS = SerializeDeSerializeHelper.DeSerialize<ReservationRs>(response);
                _returnValue = bookingConfirmRS;
                isMock = true;
                //*/

                if (_returnValue != null)
                {
                    _returnValue = _reservationConverter.Convert((EntityBase)_returnValue);
                }
            }
            return (Tuple<string, string, string>)_returnValue;
        }

        /// <summary>
        /// Get Prio Ticket Details
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<PrioPriceAndAvailability> GetPrioTicketDetails(PrioTicketDetailsCriteria criteria)
        {
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (criteria.Activity != null)
            {
                _inputContext.ActivityId = criteria.Activity.Code;
                _inputContext.UserName = PrioDistributionId;
                _inputContext.Password = PrioTokenKey;
                _inputContext.MethodType = MethodType.TicketDetails;
                _returnValue = _ticketDetailsCmdHandler.Execute(_inputContext, _inputContext.Password, criteria.Token);
                var ticketDetailRsObj = (TicketDetailRs)_returnValue;
                if (ticketDetailRsObj != null)
                {
                    if (criteria.ApiCriteria == null)
                    {
                        criteria.ApiCriteria = new Criteria { CheckinDate = DateTime.Now.Date, CheckoutDate = DateTime.Now.Date };
                    }
                    var fromDate = criteria.ApiCriteria.CheckinDate;
                    var toDate = criteria.ApiCriteria.CheckoutDate;
                    var filteredTicketTypeDetails = new List<TicketTypeDetails>();

                    while (fromDate <= toDate)
                    {
                        ticketDetailRsObj.Data.TicketTypeDetails?.ToList().ForEach(ttd =>
                        {
                            if (fromDate >= DateTime.ParseExact(ttd.StartDate, Constant.DateFormat, CultureInfo.InvariantCulture)
                                && fromDate <= DateTime.ParseExact(ttd.EndDate, Constant.DateFormat, CultureInfo.InvariantCulture)
                                && fromDate >= DateTime.ParseExact(ticketDetailRsObj.Data.BookingStartDate, Constant.DateFormat, CultureInfo.InvariantCulture) && !filteredTicketTypeDetails.Any(item => item.TicketType.ToLower().Equals(ttd.TicketType.ToLower()) && item.StartDate.Equals(ttd.StartDate) && item.EndDate.Equals(ttd.EndDate))
                            )
                            {
                                filteredTicketTypeDetails.Add(ttd);
                            }
                        });
                        fromDate = fromDate.AddDays(1);
                    }
                    ticketDetailRsObj.Data.TicketTypeDetails = filteredTicketTypeDetails.ToArray();
                    if (_returnValue != null)
                        _returnValue = _ticketDetailsConverter.Convert((EntityBase)_returnValue);
                }
            }
            return (List<PrioPriceAndAvailability>)_returnValue;
        }

        /// <summary>
        /// Get ticket details by ticket id
        /// </summary>
        /// <param name="activityCode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public object GetPrioTicketDetails(string activityCode, string token)
        {
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (!String.IsNullOrEmpty(activityCode))
            {
                _inputContext.ActivityId = activityCode;
                _inputContext.UserName = PrioDistributionId;
                _inputContext.Password = PrioTokenKey;
                _inputContext.MethodType = MethodType.TicketDetails;
                _returnValue = _ticketDetailsCmdHandler.Execute(_inputContext, _inputContext.Password, token);
            }
            if (_returnValue != null && !(_returnValue is string))
            {
                return (EntityBase)_returnValue;
            }
            else
            {
                return null;
            }
        }

        public object GetPrioTicketDetailsAsync(string activityCode, string token)
        {
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (!String.IsNullOrEmpty(activityCode))
            {
                _inputContext.ActivityId = activityCode;
                _inputContext.UserName = PrioDistributionId;
                _inputContext.Password = PrioTokenKey;
                _inputContext.MethodType = MethodType.TicketDetails;
                _returnValue = _ticketDetailsCmdHandler.Execute(_inputContext, _inputContext.Password, token);
            }
            if (_returnValue != null)
            {
                return (EntityBase)_returnValue;
            }
            else
            {
                return null;
            }
        }

        public async Task<object> GetPrioTicketDetailsAsyncV2(string activityCode, string token)
        {
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (!String.IsNullOrEmpty(activityCode))
            {
                _inputContext.ActivityId = activityCode;
                _inputContext.UserName = PrioDistributionId;
                _inputContext.Password = PrioTokenKey;
                _inputContext.MethodType = MethodType.TicketDetails;
                _returnValue = await _ticketDetailsCmdHandler.ExecuteAsync(_inputContext, _inputContext.Password, token);
            }
            if (_returnValue != null && !(_returnValue is string))
            {
                return (EntityBase)_returnValue;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Prio Ticket Currency Code
        /// This method is added to get currency code as it is needed in dumping application
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public string GetPrioProductCurrencyCode(PrioTicketDetailsCriteria criteria)
        {
            var currencyCode = string.Empty;

            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (criteria.Activity != null)
            {
                _inputContext.ActivityId = criteria.Activity.Code;
                _inputContext.UserName = PrioDistributionId;
                _inputContext.Password = PrioTokenKey;
                _inputContext.MethodType = MethodType.TicketDetails;
                _returnValue = _ticketDetailsCmdHandler.Execute(_inputContext, _inputContext.Password, criteria.Token);
                var ticketDetailRsObj = (TicketDetailRs)_returnValue;
                currencyCode = ticketDetailRsObj?.Data?.Currency;
            }

            return currencyCode;
        }

        public TicketListRs GetPrioTicketList(string token)
        {
            var ticketDetailRsObj = new TicketListRs();
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            _inputContext.UserName = PrioDistributionId;
            _inputContext.Password = PrioTokenKey;
            _inputContext.MethodType = MethodType.TicketList;
            _returnValue = _ticketListCmdHandler.Execute(_inputContext, _inputContext.Password, token);
            ticketDetailRsObj = (TicketListRs)_returnValue;
            return ticketDetailRsObj;
        }

        #region Private methods

        private ActivityOption CreateOption(ActivityOption activityOption, PrioCriteria criteria, string token, string code)
        {
            try
            {
                var ticketDetailRsObj = (TicketDetailRs)GetPrioTicketDetailsAsync(code, token);

                if (ticketDetailRsObj != null)
                {
                    try
                    {
                        var basePrice = CreatePrice(criteria, ticketDetailRsObj, activityOption?.BasePrice?.DatePriceAndAvailabilty, Constant.BasePrice);

                        var costPrice = CreatePrice(criteria, ticketDetailRsObj, activityOption?.CostPrice?.DatePriceAndAvailabilty, Constant.CostPrice);

                        var checkAvailabilityStatus = activityOption != null;

                        var option = new ActivityOption
                        {
                            AvailabilityStatus = checkAvailabilityStatus ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE,
                            BasePrice = basePrice,
                            CostPrice = costPrice,
                            PrioTicketClass = ticketDetailRsObj.Data.TicketClass,
                            PickupPoints = ticketDetailRsObj.Data.PickupPoints
                        };

                        if (!string.IsNullOrEmpty(option.PickupPoints) && ticketDetailRsObj.Data.PickupPoints != null && ticketDetailRsObj.Data.PickupPoints.ToLower() == Convert.ToString(PrioApiStatus.Mandatory).ToLower(CultureInfo.InvariantCulture))
                        {
                            option.PickupPointDetails = ticketDetailRsObj.Data.PickupPointDetails;
                        }

                        return option;
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() =>
                            _logger.Error(new IsangoErrorEntity
                            {
                                ClassName = "Prio.PrioAdapter",
                                MethodName = "CreateOption",
                                Token = token
                            }, ex)
                        );
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                        _logger.Error(new Isango.Entities.IsangoErrorEntity
                        {
                            ClassName = "PrioAdapter",
                            MethodName = "CreateOption",
                            Token = token
                        }, ex)
                );
            }
            return null;
        }

        private async Task<List<ActivityOption>> CreateOptionAsync(ActivityOption activityOption, PrioCriteria criteria, string token, string code)
        {
            try
            {
                var detail = await GetPrioTicketDetailsAsyncV2(code, token);
                if (detail != null)
                {
                    var ticketDetailRsObj = (TicketDetailRs)detail;

                    if (ticketDetailRsObj != null)
                    {
                        try
                        {
                            var basePrice = CreatePrice(criteria, ticketDetailRsObj, activityOption?.BasePrice?.DatePriceAndAvailabilty, Constant.BasePrice);

                            var costPrice = CreatePrice(criteria, ticketDetailRsObj, activityOption?.CostPrice?.DatePriceAndAvailabilty, Constant.CostPrice);

                            var checkAvailabilityStatus = activityOption != null;

                            var option = new ActivityOption
                            {
                                AvailabilityStatus = checkAvailabilityStatus ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE,
                                BasePrice = basePrice,
                                CostPrice = costPrice,
                                PrioTicketClass = ticketDetailRsObj.Data.TicketClass,
                                PickupPoints = ticketDetailRsObj.Data.PickupPoints,
                                StartTime = activityOption?.BasePrice?.DatePriceAndAvailabilty?.FirstOrDefault().Key.TimeOfDay ?? default(TimeSpan)
                            };

                            if (!string.IsNullOrEmpty(option.PickupPoints) && ticketDetailRsObj.Data.PickupPoints != null && ticketDetailRsObj.Data.PickupPoints.ToLower() == Convert.ToString(PrioApiStatus.Mandatory).ToLower(CultureInfo.InvariantCulture))
                            {
                                option.PickupPointDetails = ticketDetailRsObj.Data.PickupPointDetails;
                            }

                            return new List<ActivityOption>() { option };
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(new IsangoErrorEntity
                            {
                                ClassName = "Prio.PrioAdapter",
                                MethodName = "CreateOption",
                                Token = token
                            }, ex);

                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = "PrioAdapter",
                    MethodName = "CreateOption",
                    Token = token
                }, ex);
            }
            return null;
        }

        private async Task<List<ActivityOption>> CreateOptionsAsync(ActivityOption activityOption, PrioCriteria criteria, string token, string code)
        {
            try
            {
                var detail = await GetPrioTicketDetailsAsyncV2(code, token);
                var optionsList = new List<ActivityOption>();
                if (detail != null)
                {
                    foreach (var timeslot in activityOption?.BasePrice?.DatePriceAndAvailabilty)
                    {
                        try
                        {
                            var ticketDetailRsObj = (TicketDetailRs)detail;

                            if (ticketDetailRsObj != null)
                            {
                                try
                                {
                                    var PandA = new Dictionary<DateTime, PriceAndAvailability>();
                                    PandA.Add(timeslot.Key, timeslot.Value);
                                    var basePrice = CreatePrice(criteria, ticketDetailRsObj, PandA, Constant.BasePrice);

                                    var costPrice = CreatePrice(criteria, ticketDetailRsObj, PandA, Constant.CostPrice);

                                    var checkAvailabilityStatus = activityOption != null;

                                    var option = new ActivityOption
                                    {
                                        AvailabilityStatus = checkAvailabilityStatus ? AvailabilityStatus.AVAILABLE : AvailabilityStatus.NOTAVAILABLE,
                                        BasePrice = basePrice,
                                        CostPrice = costPrice,
                                        PrioTicketClass = ticketDetailRsObj.Data.TicketClass,
                                        PickupPoints = ticketDetailRsObj.Data.PickupPoints,
                                        StartTime = timeslot.Key.TimeOfDay,
                                        Id = Math.Abs(Guid.NewGuid().GetHashCode())
                                    };

                                    if (!string.IsNullOrEmpty(option.PickupPoints) && ticketDetailRsObj.Data.PickupPoints != null && ticketDetailRsObj.Data.PickupPoints.ToLower() == Convert.ToString(PrioApiStatus.Mandatory).ToLower(CultureInfo.InvariantCulture))
                                    {
                                        option.PickupPointDetails = ticketDetailRsObj.Data.PickupPointDetails;
                                    }

                                    if (option != null)
                                    {
                                        optionsList.Add(option);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error(new IsangoErrorEntity
                                    {
                                        ClassName = "Prio.PrioAdapter",
                                        MethodName = "CreateOption",
                                        Token = token
                                    }, ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(new IsangoErrorEntity
                            {
                                ClassName = "Prio.PrioAdapter",
                                MethodName = "CreateOption",
                                Token = token
                            }, ex);
                        }
                    }
                }
                return optionsList;
            }
            catch (Exception ex)
            {
                _logger.Error(new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = "PrioAdapter",
                    MethodName = "CreateOption",
                    Token = token
                }, ex);
            }
            return null;
        }

        /// <summary>
        /// Create price based on passed priceType(Base or Cost)
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="ticketDetailRsObj"></param>
        /// <param name="prioDatePriceAndAvailabilty"></param>
        /// <param name="priceType"></param>
        /// <returns></returns>
        private Price CreatePrice(PrioCriteria criteria, TicketDetailRs ticketDetailRsObj, Dictionary<DateTime, PriceAndAvailability> prioDatePriceAndAvailabilty, string priceType)
        {
            var priceAndAvailability = GetPrice(ticketDetailRsObj, criteria, priceType);

            var price = new Price
            {
                Amount = priceAndAvailability?.FirstOrDefault().Value?.TotalPrice ?? 0,
                Currency = new Currency { IsoCode = ticketDetailRsObj.Data.Currency },
                DatePriceAndAvailabilty = priceAndAvailability
            };

            if (prioDatePriceAndAvailabilty?.Any() == true)
            {
                price.DatePriceAndAvailabilty = CreateDatePriceAndAvailabilty(criteria, priceAndAvailability, prioDatePriceAndAvailabilty);
            }

            return price;
        }

        /// <summary>
        /// Create PriceAndAvailability dictionary for passed criteria check-in and checkout date
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="prioPriceAndAvailabilities"></param>
        /// <param name="prioDatePriceAndAvailabilty"></param>
        /// <returns></returns>
        private static Dictionary<DateTime, PriceAndAvailability> CreateDatePriceAndAvailabilty(Criteria criteria, Dictionary<DateTime, PriceAndAvailability> prioPriceAndAvailabilities, Dictionary<DateTime, PriceAndAvailability> prioDatePriceAndAvailabilty)
        {
            try
            {
                var datePriceAndAvailabilities = new Dictionary<DateTime, PriceAndAvailability>();
                var startDateTime = DateTime.Now.Date;
                if (prioPriceAndAvailabilities?.Any() == true)
                {
                    foreach (var item in prioPriceAndAvailabilities)
                    {
                        var prioPriceAndAvailabilty = item.Value;
                        startDateTime = item.Key;

                        if (prioDatePriceAndAvailabilty.Any(x => x.Key.Date == startDateTime))
                        {
                            var pandAfromAPI = prioDatePriceAndAvailabilty.Where(x => x.Key.Date == startDateTime).FirstOrDefault().Value;
                            var prioPriceAndAvailabiltyCloned = (PrioPriceAndAvailability)prioPriceAndAvailabilty.Clone();
                            if (pandAfromAPI != null)
                            {
                                var prioPandA = (PrioPriceAndAvailability)pandAfromAPI;
                                prioPriceAndAvailabiltyCloned.Vacancies = prioPandA.Vacancies;
                                prioPriceAndAvailabiltyCloned.FromDateTime = prioPandA.FromDateTime;
                                prioPriceAndAvailabiltyCloned.ToDateTime = prioPandA.ToDateTime;
                                prioPriceAndAvailabiltyCloned.AvailabilityStatus = pandAfromAPI.AvailabilityStatus;
                                prioPriceAndAvailabiltyCloned.IsCapacityCheckRequired = prioPandA.IsCapacityCheckRequired;
                                prioPriceAndAvailabiltyCloned.Capacity = prioPandA.Capacity;

                                if (!datePriceAndAvailabilities.Keys.Contains(startDateTime))
                                {
                                    datePriceAndAvailabilities.Add(startDateTime, prioPriceAndAvailabiltyCloned);
                                }
                            }
                        }
                    }
                }

                return datePriceAndAvailabilities;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get Base Price
        /// </summary>
        /// <param name="ticketDetailRs"></param>
        /// <param name="criteria"></param>
        /// <param name="priceType"></param>
        /// <returns></returns>
        private Dictionary<DateTime, PriceAndAvailability> GetPrice(TicketDetailRs ticketDetailRs, Criteria criteria, string priceType)
        {
            var priceAndAvailabilities = new Dictionary<DateTime, PriceAndAvailability>();

            var ticketTypeDetails = ticketDetailRs.Data.TicketTypeDetails;
            var inputCheckinDate = criteria.CheckinDate;
            var inputCheckoutDate = criteria.CheckoutDate;

            var dates = new List<DateTime>();
            for (var dt = criteria.CheckinDate; dt <= inputCheckoutDate; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }
            foreach (var ticketTypeDetail in ticketTypeDetails)
            {
                ticketTypeDetail.StartDateAsDate = DateTimeOffset.ParseExact(ticketTypeDetail.StartDate, Constant.DateFormat, null).Date;
                ticketTypeDetail.EndDateAsDate = DateTimeOffset.ParseExact(ticketTypeDetail.EndDate, Constant.DateFormat, null).Date;
            }

            //Filter response price based on dates in criteria
            ticketTypeDetails = ticketTypeDetails
                                                    ?.Where(x =>
                                                                 (inputCheckinDate >= x.StartDateAsDate
                                                                &&
                                                                 inputCheckinDate <= x.EndDateAsDate)
                                                                ||
                                                                (inputCheckoutDate >= x.StartDateAsDate
                                                                &&
                                                                 inputCheckoutDate <= x.EndDateAsDate)

                                                    )
                                                    ?.ToArray();
            if (ticketTypeDetails?.Any() != true)
            {
                return null;
            }

            //Create P&A for each matching date in input and response from api
            //Parallel.ForEach(dates, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (date) =>
            foreach (var date in dates)
            {
                try
                {
                    var ticketTypeDetailsForDate = ticketTypeDetails.Where(x =>
                                date >= x.StartDateAsDate &&
                                date <= x.EndDateAsDate
                            )?.ToList();

                    if (ticketTypeDetailsForDate?.Count > 0)
                    {
                        var priceAndAvailability = new PrioPriceAndAvailability
                        {
                            PricingUnits = new List<PricingUnit>()
                        };

                        //Create pricing unit for each type of pax
                        foreach (var ticketTypeDetail in ticketTypeDetailsForDate)
                        {
                            var startDate = ticketTypeDetail.StartDateAsDate;
                            var endDate = ticketTypeDetail.EndDateAsDate;
                            if (date >= startDate && date <= endDate)
                            {
                                try
                                {
                                    var price = priceType == Constant.BasePrice ? Convert.ToDecimal(ticketTypeDetail.UnitListPrice) : Convert.ToDecimal(ticketTypeDetail.UnitPrice);
                                    var ticketType = ticketTypeDetail.TicketType.ToUpper();
                                    var pricingUnit = CreatePricingUnit(ticketType, price);

                                    //Add pricing unit if not already added and its in input criteria
                                    if (
                                        priceAndAvailability?.PricingUnits.Cast<PerPersonPricingUnit>()?
                                        .Any(x => x.PassengerType == ((PerPersonPricingUnit)pricingUnit).PassengerType) == false

                                     && (criteria.NoOfPassengers.Keys
                                            .Contains(((PerPersonPricingUnit)pricingUnit).PassengerType)
                                            || !criteria.NoOfPassengers.Any()
                                        )
                                    )
                                    {
                                        priceAndAvailability.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                                        priceAndAvailability.PricingUnits.Add(pricingUnit);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Task.Run(() =>
                                        _logger.Error(new IsangoErrorEntity
                                        {
                                            ClassName = "Prio.PrioAdapter",
                                            MethodName = "GetPrice",
                                            Params = SerializeDeSerializeHelper.Serialize(ticketTypeDetail),
                                        }, ex)
                                    );
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        priceAndAvailability.TourDepartureId = ticketDetailRs.Data.TicketId;
                        priceAndAvailability.Name = ticketDetailRs.Data.TicketTitle;
                        priceAndAvailability.TotalPrice = priceAndAvailability.PricingUnits.Sum(e => e.Price);

                        if (!priceAndAvailabilities.Keys.Contains(date))
                        {
                            priceAndAvailabilities.Add(date, priceAndAvailability);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                        _logger.Error(new IsangoErrorEntity
                        {
                            ClassName = "Prio.PrioAdapter",
                            MethodName = "GetPrice",
                            Params = $"Error for date :-{date.ToString()}"
                        }, ex)
                    );
                }
            }
            //);
            priceAndAvailabilities?.OrderBy(x => x.Key);
            return priceAndAvailabilities;
        }

        private PricingUnit CreatePricingUnit(string ticketType, decimal price)
        {
            var passengerType = GetPassengerType(ticketType);
            var pricingUnit = (PerPersonPricingUnit)PricingUnitFactory.GetPricingUnit(passengerType);
            pricingUnit.Price = price;
            return pricingUnit;
        }

        private PassengerType GetPassengerType(string ticketType)
        {
            switch (ticketType.ToUpper())
            {
                case Constant.Adult:
                    return PassengerType.Adult;

                case Constant.Child:
                    return PassengerType.Child;

                case Constant.Infant:
                    return PassengerType.Infant;

                case Constant.Senior:
                    return PassengerType.Senior;

                case Constant.Family:
                    return PassengerType.Family;

                case Constant.Student:
                    return PassengerType.Student;

                case Constant.Youth:
                    return PassengerType.Youth;

                default:
                    return PassengerType.Adult;
            }
        }

        private static string GetPrioPassengerType(PassengerType passengerType)
        {
            switch (passengerType)
            {
                case PassengerType.Adult:
                    return Constant.Adult;

                case PassengerType.Child:
                    return Constant.Child;

                case PassengerType.Infant:
                    return Constant.Infant;

                case PassengerType.Senior:
                    return Constant.Senior;

                case PassengerType.Student:
                    return Constant.Student;

                case PassengerType.Family:
                    return Constant.Family;

                case PassengerType.Youth:
                    return Constant.Youth;

                default:
                    return Constant.Adult;
            }
        }

        private void AddPassengerCountInInputContext(ProductOption productOptions, InputContext _inputContext)
        {
            var travelInfo = productOptions?.TravelInfo;

            _inputContext.TicketType = new List<string>();
            _inputContext.Count = new List<int>();
            var validPassengers = travelInfo.NoOfPassengers.Where(e => _validPassengerTypes.Contains(e.Key));
            foreach (var passengerType in validPassengers)
            {
                var count = passengerType.Value;
                if (count > 0 && passengerType.Key != PassengerType.Infant)
                {
                    _inputContext.TicketType.Add(GetPrioPassengerType(passengerType.Key));
                    _inputContext.Count.Add(count);
                }
            }
        }

        #endregion Private methods
    }
}