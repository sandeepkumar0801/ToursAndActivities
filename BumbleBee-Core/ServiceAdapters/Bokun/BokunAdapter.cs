using Isango.Entities.Bokun;
using Isango.Entities.Booking;
using Isango.Entities.Enums;
using ServiceAdapters.Bokun.Bokun.Commands.Contracts;
using ServiceAdapters.Bokun.Bokun.Converters.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities;
using ServiceAdapters.Bokun.Bokun.Entities.CancelBooking;
using ServiceAdapters.Bokun.Bokun.Entities.CheckAvailabilities;
using ServiceAdapters.Bokun.Bokun.Entities.EditBooking;
using ServiceAdapters.Bokun.Bokun.Entities.GetActivity;
using ServiceAdapters.Bokun.Bokun.Entities.GetBooking;
using ServiceAdapters.Bokun.Bokun.Entities.GetPickupPlaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;
using Constant = ServiceAdapters.Bokun.Constants.Constant;
using Pricingcategory = ServiceAdapters.Bokun.Bokun.Entities.GetActivity.Pricingcategory;
using Question = Isango.Entities.Bokun.Question;

namespace ServiceAdapters.Bokun
{
    public class BokunAdapter : IBokunAdapter, IAdapter
    {
        #region "Private Members"

        private readonly ICancelBookingCommandHandler _cancelBookingCommandHandler;
        private readonly ICheckAvailabilitiesCommandHandler _checkAvailabilitiesCommandHandler;
        private readonly ICheckoutOptionsCommandHandler _checkoutOptionsCommandHandler;
        private readonly IEditBookingCommandHandler _editBookingCommandHandler;
        private readonly IGetActivityCommandHandler _getActivityCommandHandler;
        private readonly IGetBookingCommandHandler _getBookingCommandHandler;
        private readonly ISubmitCheckoutCommandHandler _submitCheckoutCommandHandler;
        private readonly IGetPickupPlacesCommandHandler _getPickupPlacesCommandHandler;
        private readonly ICancelBookingConverter _cancelBookingConverter;
        private readonly ICheckAvailabilitiesConverter _checkAvailabilitiesConverter;
        private readonly ICheckoutOptionsConverter _checkoutOptionsConverter;
        private readonly IEditBookingConverter _editBookingConverter;
        private readonly IGetActivityConverter _getActivityConverter;
        private readonly IGetBookingConverter _getBookingConverter;
        private readonly ISubmitCheckoutConverter _submitCheckoutConverter;
        private readonly IGetPickupPlacesConverter _getPickupPlacesConverter;
        private readonly bool _isRollbackLiveAPIBookingsOtherThanPROD;

        #endregion "Private Members"

        #region "Constructor"

        public BokunAdapter(
            ICancelBookingCommandHandler cancelBookingCommandHandler,
            ICheckAvailabilitiesCommandHandler checkAvailabilitiesCommandHandler,
            ICheckoutOptionsCommandHandler checkoutOptionsCommandHandler,
            IEditBookingCommandHandler editBookingCommandHandler,
            IGetActivityCommandHandler getActivityCommandHandler,
            IGetBookingCommandHandler getBookingCommandHandler,
            ISubmitCheckoutCommandHandler submitCheckoutCommandHandler,
            IGetPickupPlacesCommandHandler getPickupPlacesCommandHandler,
            ICancelBookingConverter cancelBookingConverter,
            ICheckAvailabilitiesConverter checkAvailabilitiesConverter,
            ICheckoutOptionsConverter checkoutOptionsConverter,
            IEditBookingConverter editBookingConverter,
            IGetActivityConverter getActivityConverter,
            IGetBookingConverter getBookingConverter,
            ISubmitCheckoutConverter submitCheckoutConverter,
            IGetPickupPlacesConverter getPickupPlacesConverter)
        {
            _cancelBookingCommandHandler = cancelBookingCommandHandler;
            _checkAvailabilitiesCommandHandler = checkAvailabilitiesCommandHandler;
            _editBookingCommandHandler = editBookingCommandHandler;
            _checkoutOptionsCommandHandler = checkoutOptionsCommandHandler;
            _getActivityCommandHandler = getActivityCommandHandler;
            _getBookingCommandHandler = getBookingCommandHandler;
            _submitCheckoutCommandHandler = submitCheckoutCommandHandler;
            _getPickupPlacesCommandHandler = getPickupPlacesCommandHandler;
            _checkAvailabilitiesConverter = checkAvailabilitiesConverter;
            _cancelBookingConverter = cancelBookingConverter;
            _checkoutOptionsConverter = checkoutOptionsConverter;
            _editBookingConverter = editBookingConverter;
            _getActivityConverter = getActivityConverter;
            _getBookingConverter = getBookingConverter;
            _submitCheckoutConverter = submitCheckoutConverter;
            _getPickupPlacesConverter = getPickupPlacesConverter;
            try
            {
                _isRollbackLiveAPIBookingsOtherThanPROD = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.RollbackLiveAPIBookingsOtherThanPROD) == "1";
            }
            catch (Exception)
            {
                _isRollbackLiveAPIBookingsOtherThanPROD = false;
            }
        }

        #endregion "Constructor"

        #region Bokun Api Calls

        /// <summary>
        /// This method is used to get the booking by BookingID
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public GetBookingRs GetBooking(string bookingId, string token)
        {
            var request = new GetBookingRq() { BookingId = bookingId };
            var result = _getBookingCommandHandler.Execute(request, MethodType.Getbooking, token);
            if (result == null)
                return null;
            return _getBookingConverter.Convert(result, MethodType.Getbooking) as GetBookingRs;
        }

        /// <summary>
        /// This method is used to get the booking by BookingID
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<GetBookingRs> GetBookingAsync(string bookingId, string token)
        {
            var request = new GetBookingRq() { BookingId = bookingId };
            var result = await _getBookingCommandHandler.ExecuteAsync(request, MethodType.Getbooking, token);
            if (result == null)
                return null;
            result = _getBookingConverter.Convert(result, MethodType.Getbooking);
            return result as GetBookingRs;
        }

        /// <summary>
        /// This method is used to check activity availablities and pricing
        /// </summary>
        /// <param name="criteria">Input criteria Criteria entity</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<Isango.Entities.Activities.Activity> CheckAvailabilities(BokunCriteria criteria, Isango.Entities.Activities.Activity activity, string token)
        {
            var availabilities = new List<CheckAvailabilitiesRs>();
            var activities = new List<Isango.Entities.Activities.Activity>();
            var taskArray = new Task<List<CheckAvailabilitiesRs>>[criteria.FactSheetIds.Count];
            var count = 0;
            foreach (var factsheetId in criteria?.FactSheetIds)
            {
                try
                {
                    var availability = UpdateAvailabilitiesForEachItem(criteria, factsheetId, token);
                    if (availability?.Count > 0)
                        availabilities.AddRange(availability);
                    count++;
                }
                catch (Exception ex)
                {
                    continue; // should not stop whole process
                }
            }
            //if (taskArray?.Length > 0)
            //{
            //    Task.WaitAll(taskArray);
            //    foreach (var task in taskArray)
            //    {
            //        var data = task.GetAwaiter().GetResult();
            //        if (data != null)
            //            availabilities.AddRange(data);
            //    }
            //}
            if (availabilities.Count > 0)
            {
                return _checkAvailabilitiesConverter.Convert(availabilities, MethodType.Checkavailabilities, criteria, activity) as List<Isango.Entities.Activities.Activity>;
            }
            return activities;
        }

        /// <summary>
        /// Hit availabilities and activity call
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="factsheetId"></param>
        /// <param name="token"></param>
        /// <param name="pricingCategoryIds"></param>
        /// <param name="pricingCategories"></param>
        /// <param name="availabilities"></param>
        private List<CheckAvailabilitiesRs> UpdateAvailabilitiesForEachItem(BokunCriteria criteria, int factsheetId, string token)
        {
            var pricingCategories = criteria.PriceCategoryIdMapping?.Where(x => (System.Convert.ToString(x.ServiceOptionCode).Equals(factsheetId.ToString()))).ToList();
            var pricingCategoryIds = pricingCategories.Select(x => x.PriceCategoryId).Distinct().ToList();

            var availabilities = new List<CheckAvailabilitiesRs>();
            try
            {
                var request = new CheckAvailabilitiesRq()
                {
                    ActivityId = factsheetId,
                    StartDate = criteria.CheckinDate.ToString(Constant.DateInyyyyMMdd),
                    EndDate = criteria.CheckoutDate.ToString(Constant.DateInyyyyMMdd),
                    CurrencyIsoCode = criteria.CurrencyIsoCode
                };

                var executeResult = _checkAvailabilitiesCommandHandler.Execute(request, MethodType.Checkavailabilities, token);

                if (executeResult != null)
                {
                    var response =
                        SerializeDeSerializeHelper.DeSerialize<List<CheckAvailabilitiesRs>>(
                            executeResult.ToString());
                    response = response?.Where(x => x?.SoldOut == false)?.ToList();

                    if (token?.ToLower().Contains("dumping") ?? false)
                    {
                        response?.ForEach(result =>
                        {
                            var paxAgeGroupIds = new Dictionary<PassengerType, int>();
                            foreach (var rate in result?.Rates)
                            {
                                foreach (var priceId in rate?.PricingCategoryIds)
                                {
                                    try
                                    {
                                        var pricingCategory = new Pricingcategory()
                                        {
                                            Id = priceId,
                                            MinAge = 0,
                                            MaxAge = 0
                                        };
                                        UpdatePaxAgeGroup(pricingCategories, pricingCategory, ref paxAgeGroupIds, ref result);
                                    }
                                    catch (Exception ex)
                                    {
                                        //ignore
                                    }
                                }
                            }
                            result.PaxAgeGroupIds = paxAgeGroupIds;
                            availabilities.Add(result);
                        });
                    }
                    else
                    {
                        var activity = GetActivity(factsheetId.ToString(), token);
                        activity.PricingCategories = activity?.PricingCategories?
                            .Where(x => pricingCategoryIds.Contains(System.Convert.ToInt32(x.Id)))
                            .ToList();

                        response?.ForEach(result =>
                        {
                            var paxAgeGroupIds = new Dictionary<PassengerType, int>();
                            activity?.PricingCategories?.ForEach(pricingCategory =>
                            {
                                UpdatePaxAgeGroup(pricingCategories, pricingCategory, ref paxAgeGroupIds, ref result);
                            });
                            result.PaxAgeGroupIds = paxAgeGroupIds;
                            availabilities.Add(result);
                        });
                    }
                }
            }
            catch (Exception)
            {
                //ignored //#TODO Add logging here
                return availabilities;
            }
            return availabilities;
        }

        /// <summary>
        /// This method is used to get activity availabilities and pricing for dumping application
        /// </summary>
        /// <param name="criteria">Input criteria entity</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<Isango.Entities.Activities.Activity> CheckAvailabilitiesForDumpingApp(BokunCriteria criteria, Isango.Entities.Activities.Activity isangoActivity, string token)
        {
            var availabilities = new List<CheckAvailabilitiesRs>();
            if (criteria != null)
            {
                foreach (var factSheetId in criteria.FactSheetIds)
                {
                    criteria.ActivityCode = factSheetId.ToString();
                    var request = new CheckAvailabilitiesRq()
                    {
                        ActivityId = factSheetId,
                        StartDate = criteria.CheckinDate.ToString("yyyy-MM-dd"),
                        EndDate = criteria.CheckoutDate.ToString("yyyy-MM-dd"),
                        CurrencyIsoCode = criteria.CurrencyIsoCode
                    };

                    var executeResult = _checkAvailabilitiesCommandHandler.Execute(request, MethodType.Checkavailabilities, token);
                    if (executeResult == null)
                        return null;

                    var activity = GetActivity(criteria.ActivityCode, token);
                    var response = SerializeDeSerializeHelper.DeSerialize<List<CheckAvailabilitiesRs>>(executeResult.ToString());

                    if (response != null)
                    {
                        response.ForEach(result =>
                        {
                            var paxAgeGroupIds = new Dictionary<PassengerType, int>();
                            var pricingCategories = activity.PricingCategories;
                            foreach (var pricingCategory in pricingCategories)
                            {
                                if (pricingCategory.TicketCategory == PassengerType.Adult.ToString().ToUpperInvariant())
                                {
                                    if (!paxAgeGroupIds.ContainsKey(PassengerType.Adult))
                                    {
                                        if (pricingCategory.Id != null)
                                            paxAgeGroupIds.Add(PassengerType.Adult, pricingCategory.Id.Value);
                                        if (pricingCategory.MinAge != null)
                                            result.MinAdultAge = pricingCategory.MinAge.Value;
                                        if (pricingCategory.MaxAge != null)
                                            result.MaxAdultAge = pricingCategory.MaxAge.Value;
                                        result.IsAdultAllowed = true;
                                    }
                                }
                                else if (pricingCategory.TicketCategory == PassengerType.Child.ToString().ToUpperInvariant())
                                {
                                    if (pricingCategory.Id != null)
                                        paxAgeGroupIds.Add(PassengerType.Child, pricingCategory.Id.Value);
                                    if (pricingCategory.MinAge != null)
                                        result.MinChildAge = pricingCategory.MinAge.Value;
                                    if (pricingCategory.MaxAge != null)
                                        result.MaxChildAge = pricingCategory.MaxAge.Value;
                                    result.IsChildAllowed = true;
                                }
                            }
                            result.PaxAgeGroupIds = paxAgeGroupIds;
                            availabilities.Add(result);
                        });
                    }
                }
                if (availabilities.Count > 0)
                {
                    return _checkAvailabilitiesConverter.Convert(availabilities, MethodType.Checkavailabilities, criteria, isangoActivity) as List<Isango.Entities.Activities.Activity>;
                }
            }

            return null;
        }

        /// <summary>
        /// This method is used to check activity availabilities and pricing
        /// </summary>
        /// <param name="criteria">Input criteria entity</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<Isango.Entities.Activities.Activity>> CheckAvailabilitiesAsync(BokunCriteria criteria, Isango.Entities.Activities.Activity isangoActivity, string token)
        {
            var availabilities = new List<CheckAvailabilitiesRs>();
            var pricingCategories = criteria.PriceCategoryIdMapping?.Where(x => (System.Convert.ToString(x.ServiceOptionCode).Equals(criteria.ActivityCode))).ToList();
            if (pricingCategories != null && pricingCategories.Any())
            {
                var pricingCategoryIds = pricingCategories.Select(x => x.PriceCategoryId).ToList();
                criteria.FactSheetIds.ForEach(factsheetId =>
                {
                    var request = new CheckAvailabilitiesRq()
                    {
                        ActivityId = factsheetId,
                        StartDate = criteria.CheckinDate.ToString(Constant.DateInyyyyMMdd),
                        EndDate = criteria.CheckoutDate.ToString(Constant.DateInyyyyMMdd)
                    };
                    var executeResult = _checkAvailabilitiesCommandHandler.Execute(request, MethodType.Checkavailabilities, token);
                    if (executeResult != null)
                    {
                        var response =
                            SerializeDeSerializeHelper.DeSerialize<List<CheckAvailabilitiesRs>>(executeResult.ToString());
                        var activity = GetActivity(factsheetId.ToString(), token);

                        activity.PricingCategories = activity.PricingCategories.Where(x => pricingCategoryIds.Contains(System.Convert.ToInt32(x.Id))).ToList();
                        response.ForEach(result =>
                        {
                            var paxAgeGroupIds = new Dictionary<PassengerType, int>();
                            activity?.PricingCategories.ForEach(pricingCategory =>
                            {
                                UpdatePaxAgeGroup(pricingCategories, pricingCategory, ref paxAgeGroupIds, ref result);
                            });
                            result.PaxAgeGroupIds = paxAgeGroupIds;
                            availabilities.Add(result);
                        });
                    }
                });
            }
            if (availabilities.Count > 0)
            {
                return await Task.FromResult(_checkAvailabilitiesConverter.Convert(availabilities, MethodType.Checkavailabilities, criteria, isangoActivity) as List<Isango.Entities.Activities.Activity>);
            }
            return null;
        }

        /// <summary>
        /// This method is used to get the checkout options
        /// </summary>
        /// <param name="request">Input criteria BokunSelectedProduct entity</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<Question> CheckoutOptions(BokunSelectedProduct request, string token)
        {
            var result = _checkoutOptionsCommandHandler.Execute(request, MethodType.Checkoutoptions, token);
            if (result == null) return null;
            return (List<Question>)_checkoutOptionsConverter.Convert(result, MethodType.Checkoutoptions);
        }

        /// <summary>
        /// This method is used to get the checkout options
        /// </summary>
        /// <param name="request">Input criteria BokunSelectedProduct entity</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<Question>> CheckoutOptionsAsync(BokunSelectedProduct request, string token)
        {
            var result = await _checkoutOptionsCommandHandler.ExecuteAsync(request, MethodType.Checkoutoptions, token);
            if (result == null)
                return null;
            return _checkoutOptionsConverter.Convert(result, MethodType.Checkoutoptions) as List<Question>;
        }

        /// <summary>
        /// This method is used to submit the checkout
        /// </summary>
        /// <param name="request">Input criteria Bokun SelectedProduct model</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Booking SubmitCheckout(BokunSelectedProduct request, string token)
        {
            var apiRequest = string.Empty;
            var apiResponse = string.Empty;
            return SubmitCheckout(request, token, out apiRequest, out apiResponse);
        }

        /// <summary>
        /// This method is used to submit the checkout
        /// </summary>
        /// <param name="request">Input criteria Bokun SelectedProduct model</param>
        /// <param name="token"></param>
        /// <param name="apiRequest"></param>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public Booking SubmitCheckout(BokunSelectedProduct request, string token, out string apiRequest, out string apiResponse)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;
            request.Token = token;
            var result = _submitCheckoutCommandHandler.Execute(request, MethodType.Submitcheckout, token, out apiRequest, out apiResponse);

            var isMock = false;
            /*
            var result = default(object);
            apiRequest = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\7 Bokun ApiREQ Booking.json");
            apiResponse = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\7 Bokun ApiRES Booking.json");
            result = apiResponse;
            isMock = true;
            //*/

            if (result == null) return null;
            var resultBooking = _submitCheckoutConverter.Convert(result, MethodType.Submitcheckout) as Booking;

            //Uncomment to cancel the booking after creating it.

            #region Cancel the booking after creating it.

            if (resultBooking != null
                && _isRollbackLiveAPIBookingsOtherThanPROD
                && isMock == false
            )
            {
                try
                {
                    var confimationCode = resultBooking?.ReferenceNumber;
                    if (!string.IsNullOrWhiteSpace(confimationCode))
                    {
                        CancelBooking(confimationCode, token);
                    }
                }
                catch (System.Exception ex)
                {
                    //ignored
                }
            }

            #endregion Cancel the booking after creating it.

            return resultBooking;
        }

        /// <summary>
        /// This method is used to submit the checkout
        /// </summary>
        /// <param name="request">Input criteria BokunSelectedProduct model</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Booking> SubmitCheckoutAsync(BokunSelectedProduct request, string token)
        {
            var result = await _submitCheckoutCommandHandler.ExecuteAsync(request, MethodType.Submitcheckout, token);
            if (result == null) return null;
            var resultBooking = _submitCheckoutConverter.Convert(result, MethodType.Submitcheckout) as Booking;

            return resultBooking;
        }

        /// <summary>
        /// This method is used to cancel the booking
        /// </summary>
        /// <param name="confirmationCode">Input criteria Booking entity</param>
        /// <param name="token"></param>
        public bool CancelBooking(string confirmationCode, string token)
        {
            var result = _cancelBookingCommandHandler.Execute(confirmationCode, MethodType.Cancelbooking, token);
            if (result == null) return false;
            _cancelBookingConverter.Convert(result, MethodType.Cancelbooking);
            return true;
        }

        /// <summary>
        /// This method is used to cancel the booking
        /// </summary>
        /// <param name="confirmationCode"></param>
        /// <param name="token"></param>
        /// <param name="apiRequest"></param>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public bool CancelBooking(string confirmationCode, string token, out string apiRequest, out string apiResponse)
        {
            bool isCancelled;
            apiRequest = string.Empty;
            apiResponse = string.Empty;
            try
            {
                var result = _cancelBookingCommandHandler.Execute(confirmationCode, MethodType.Cancelbooking, token, out apiRequest, out apiResponse);

                var responseObj = _cancelBookingConverter.Convert(result, MethodType.Cancelbooking) as CancelBookingRs;
                isCancelled = responseObj?.Message?.ToLower() == "ok";
            }
            catch (System.Exception)
            {
                //#TODO add logging here
                isCancelled = false;
            }
            return isCancelled;
        }

        /// <summary>
        /// This method is used to cancel the booking
        /// </summary>
        /// <param name="confirmationCode">Input criteria Booking entity</param>
        /// <param name="token"></param>
        public async Task<bool> CancelBookingAsync(string confirmationCode, string token)
        {
            var result = await _cancelBookingCommandHandler.ExecuteAsync(confirmationCode, MethodType.Cancelbooking, token);
            if (result == null) return false;
            _cancelBookingConverter.Convert(result, MethodType.Cancelbooking);
            return true;
        }

        /// <summary>
        /// This method is used to get the activity
        /// </summary>
        /// <param name="factsheetId">factsheetId</param>
        /// <param name="token"></param>
        public GetActivityRs GetActivity(string factsheetId, string token)
        {
            var result = _getActivityCommandHandler.Execute(factsheetId, MethodType.Getactivity, token);
            if (result == null) return null;
            return _getActivityConverter.Convert(result, MethodType.Getactivity) as GetActivityRs;
        }

        /// <summary>
        /// This method is used to get the activity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <param name="apiRequest"></param>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public GetActivityRs GetActivity(BokunCriteria request, string token, out string apiRequest, out string apiResponse)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;
            var result = _getActivityCommandHandler.Execute(request, MethodType.Getactivity, token, out apiRequest, out apiResponse);
            if (result == null) return null;
            return _getActivityConverter.Convert(result, MethodType.Getactivity) as GetActivityRs;
        }

        /// <summary>
        /// This method is used to get the activity
        /// </summary>
        /// <param name="request">Input criteria model</param>
        /// <param name="token"></param>
        public async Task<GetActivityRs> GetActivityAsync(BokunCriteria request, string token)
        {
            var result = await _getActivityCommandHandler.ExecuteAsync(request, MethodType.Getactivity, token);
            if (result == null) return null;
            return _getActivityConverter.Convert(result, MethodType.Getactivity) as GetActivityRs;
        }

        /// <summary>
        /// This method is used to get the booking by BookingID
        /// </summary>
        /// <param name="request">input BokunSelectedProduct model</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string EditBooking(BokunSelectedProduct request, string token)
        {
            var response = string.Empty;
            var editBookingRq = new EditBookingRq
            {
                Type = request.EditType,
                ActivityBookingId = request.Id
            };
            foreach (var pricingCategoryId in request.PricingCategoryIds)
            {
                editBookingRq.PricingCategoryBookingId = pricingCategoryId;
                var result = _editBookingCommandHandler.Execute(editBookingRq, MethodType.Editbooking, token);
                if (result == null) return null;

                if (_editBookingConverter.Convert(result, MethodType.Editbooking) is EditBookingRs convertedResult) response = convertedResult.Status;
            }
            return response;
        }

        /// <summary>
        /// This method is used to get the booking by BookingID
        /// </summary>
        /// <param name="request">input BokunSelectedProduct model</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> EditBookingAsync(BokunSelectedProduct request, string token)
        {
            var response = string.Empty;
            var editBookingRq = new EditBookingRq
            {
                Type = request.EditType,
                ActivityBookingId = request.Id
            };
            foreach (var pricingCategoryId in request.PricingCategoryIds)
            {
                editBookingRq.PricingCategoryBookingId = pricingCategoryId;
                var result = await _editBookingCommandHandler.ExecuteAsync(editBookingRq, MethodType.Editbooking, token);
                if (result == null) return null;

                if (_editBookingConverter.Convert(result, MethodType.Editbooking) is EditBookingRs convertedResult) response = convertedResult.Status;
            }
            return response;
        }

        /// <summary>
        /// This method is used to get the pick places for the given activity
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public GetPickupPlacesRS GetPickupPlaces(int activityId, string token)
        {
            var result = _getPickupPlacesCommandHandler.Execute(activityId, MethodType.Getpickupplaces, token);
            if (result == null) return null;
            return _getPickupPlacesConverter.Convert(result, MethodType.Getpickupplaces) as GetPickupPlacesRS;
        }

        /// <summary>
        /// This method is used to get the pick places for the given activity asynchronously
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<GetPickupPlacesRS> GetPickupPlacesAsync(int activityId, string token)
        {
            var result = await _getPickupPlacesCommandHandler.ExecuteAsync(activityId, MethodType.Getpickupplaces, token);
            if (result == null) return null;
            return _getPickupPlacesConverter.Convert(result, MethodType.Getpickupplaces) as GetPickupPlacesRS;
        }

        #endregion Bokun Api Calls

        private void UpdatePaxAgeGroup(List<PriceCategory> cachedPricingCategories, Pricingcategory pricingCategory, ref Dictionary<PassengerType, int> paxAgeGroupIds, ref CheckAvailabilitiesRs result)
        {
            if (pricingCategory != null)
            {
                var priceCategoryId = pricingCategory.Id ?? 0;
                try
                {
                    if (cachedPricingCategories?.FirstOrDefault(x => x.PassengerTypeId == PassengerType.Adult && x.PriceCategoryId == pricingCategory.Id) != null)
                    {
                        paxAgeGroupIds.Add(PassengerType.Adult, priceCategoryId);
                        if (pricingCategory.MinAge != null) result.MinAdultAge = pricingCategory.MinAge.Value;
                        if (pricingCategory.MaxAge != null) result.MaxAdultAge = pricingCategory.MaxAge.Value;
                        result.IsAdultAllowed = true;
                    }
                    else
                                if (cachedPricingCategories?.FirstOrDefault(x => x.PassengerTypeId == PassengerType.Child && x.PriceCategoryId == pricingCategory.Id) != null)
                    {
                        paxAgeGroupIds.Add(PassengerType.Child, priceCategoryId);
                        if (pricingCategory.MinAge != null) result.MinChildAge = pricingCategory.MinAge.Value;
                        if (pricingCategory.MaxAge != null) result.MaxChildAge = pricingCategory.MaxAge.Value;
                        result.IsChildAllowed = true;
                    }
                    else
                                if (cachedPricingCategories?.FirstOrDefault(x => x.PassengerTypeId == PassengerType.Youth && x.PriceCategoryId == pricingCategory.Id) != null)
                    {
                        paxAgeGroupIds.Add(PassengerType.Youth, priceCategoryId);
                        if (pricingCategory.MinAge != null) result.MinYouthAge = pricingCategory.MinAge.Value;
                        if (pricingCategory.MaxAge != null) result.MaxYouthAge = pricingCategory.MaxAge.Value;
                        result.IsYouthAllowed = true;
                    }
                    else if (cachedPricingCategories?.FirstOrDefault(x => x.PassengerTypeId == PassengerType.Infant && x.PriceCategoryId == pricingCategory.Id) != null)
                    {
                        paxAgeGroupIds.Add(PassengerType.Infant, priceCategoryId);
                        if (pricingCategory.MinAge != null) result.MinInfantAge = pricingCategory.MinAge.Value;
                        if (pricingCategory.MaxAge != null) result.MaxInfantAge = pricingCategory.MaxAge.Value;
                        result.IsInfantAllowed = true;
                    }
                    else if (cachedPricingCategories?.FirstOrDefault(x => x.PassengerTypeId == PassengerType.Senior && x.PriceCategoryId == pricingCategory.Id) != null)

                    {
                        paxAgeGroupIds.Add(PassengerType.Senior, priceCategoryId);
                        if (pricingCategory.MinAge != null) result.MinSeniorAge = pricingCategory.MinAge.Value;
                        if (pricingCategory.MaxAge != null) result.MaxSeniorAge = pricingCategory.MaxAge.Value;
                        result.IsSeniorAllowed = true;
                    }
                    else if (cachedPricingCategories?.FirstOrDefault(x => x.PassengerTypeId == PassengerType.Student && x.PriceCategoryId == pricingCategory.Id) != null)

                    {
                        paxAgeGroupIds.Add(PassengerType.Student, priceCategoryId);
                        if (pricingCategory.MinAge != null) result.MinStudentAge = pricingCategory.MinAge.Value;
                        if (pricingCategory.MaxAge != null) result.MaxStudentAge = pricingCategory.MaxAge.Value;
                        result.IsStudentAllowed = true;
                    }
                    else if (cachedPricingCategories?.FirstOrDefault(x => x.PassengerTypeId == PassengerType.Family && x.PriceCategoryId == pricingCategory.Id) != null)
                    {
                        paxAgeGroupIds.Add(PassengerType.Family, priceCategoryId);
                        if (pricingCategory.MinAge != null) result.MinFamilyAge = pricingCategory.MinAge.Value;
                        if (pricingCategory.MaxAge != null) result.MaxFamilyAge = pricingCategory.MaxAge.Value;
                        result.IsFamilyAllowed = true;
                    }
                    else if (cachedPricingCategories?.FirstOrDefault(x => x.PassengerTypeId == PassengerType.Concession && x.PriceCategoryId == pricingCategory.Id) != null)
                    {
                        paxAgeGroupIds.Add(PassengerType.Concession, priceCategoryId);
                        if (pricingCategory.MinAge != null) result.MinConcessionAge = pricingCategory.MinAge.Value;
                        if (pricingCategory.MaxAge != null) result.MaxConcessionAge = pricingCategory.MaxAge.Value;
                        result.IsConcessionAllowed = true;
                    }
                }
                catch (Exception ex)
                {

                    // throw;
                }
            }
        }
    }
}