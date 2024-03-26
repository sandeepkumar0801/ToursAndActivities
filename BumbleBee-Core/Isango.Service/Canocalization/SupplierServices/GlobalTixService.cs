using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.GlobalTix;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.GlobalTix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class GlobalTixService : SupplierServiceBase, ISupplierService
    {
        private readonly IGlobalTixAdapter _globalTixAdapter;
        private readonly ILogger _log;

        public GlobalTixService(IGlobalTixAdapter globalAdapter, ILogger log = null)
        {
            _globalTixAdapter = globalAdapter;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var globalTixCriteria = (GlobalTixCriteria)criteria;
            bool isNonThailandProduct = !activity.Regions?.Where(x => x.Type == Entities.Enums.RegionType.Country)?.FirstOrDefault().Name?.ToLower().Equals("thailand") ?? true;

            var getActivity = _globalTixAdapter.GetActivityInformation(globalTixCriteria, token, isNonThailandProduct) as Activity;
            var activities = new List<Activity>();
            if (getActivity != null)
            {
                activities.Add(getActivity);
                if (activities?.Count > 0)
                {
                    if (activities.FirstOrDefault().ProductOptions == null)
                    {
                        var message = Constant.APIActivityOptionsNot + Constant.GlobalTixAPI + " .Id:" + activity.ID;
                        SendException(activity.ID, message);
                    }
                    return MapActivity(activity, activities, criteria);
                }
            }
            else
            {
                var message = Constant.APIActivityNot + Constant.GlobalTixAPI + " .Id:" + activity.ID;
                SendException(activity.ID, message);
            }
            return activity;
        }

        /// <summary>
        /// CreateCriteria
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="criteria"></param>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var globalTixCriteria = new GlobalTixCriteria
            {
                ActivityId = activity.Id.ToString(),
                FactSheetId = activity.FactsheetId,
                CheckinDate = criteria.CheckinDate,
                NoOfPassengers = criteria.NoOfPassengers,
                Days2Fetch = 1,
                Token = criteria.Token,
                Language = criteria.Language
            };
            if (criteria.CheckoutDate != null && criteria.CheckinDate != null)
            {
                globalTixCriteria.Days2Fetch = System.Convert.ToInt32(((criteria.CheckoutDate - criteria.CheckinDate).TotalDays) + 1);
            }
            return globalTixCriteria;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitiesFromAPI, Criteria criteria)
        {
            var activityFromAPI = activitiesFromAPI.FirstOrDefault(act => act.ID == activity.FactsheetId);
            if (activityFromAPI != null)
            {
                var productOptions = new List<ProductOption>();
                foreach (var activityOptionFromCache in activity.ProductOptions)
                {
                    var activityOptionFromAPI = (ActivityOption)activityFromAPI.ProductOptions.FirstOrDefault(
                        x => ((ActivityOption)x).Code.Trim() == activityOptionFromCache.SupplierOptionCode.Trim());
                    if (activityOptionFromAPI != null)
                    {
                        var option = GetMappedActivityOption(activityOptionFromAPI, activityOptionFromCache, criteria);
                        productOptions.Add(option);
                    }
                }
                activity.ProductOptions = productOptions;
                if (activity.ProductOptions.Count == 0)
                {
                    activity.ProductOptions = activityFromAPI.ProductOptions;
                }
            }
            return activity;
        }

        /// <summary>
        /// GetMappedActivityOption
        /// </summary>
        /// <param name="option"></param>
        /// <param name="productOption"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private ActivityOption GetMappedActivityOption(ActivityOption option, ProductOption productOption, Criteria criteria)
        {
            return new ActivityOption
            {
                ActivitySeasons = option.ActivitySeasons,
                AvailToken = option.AvailToken,
                Code = option.Code,
                Contract = option.Contract,
                HotelPickUpLocation = option.HotelPickUpLocation,
                PickUpOption = option.PickUpOption,
                PickupPointDetails = option.PickupPointDetails,
                PickupPoints = option.PickupPoints,
                PricingCategoryId = option.PricingCategoryId,
                PrioTicketClass = option.PrioTicketClass,
                RateKey = option.RateKey,
                ScheduleReturnDetails = option.ScheduleReturnDetails,
                StartTimeId = option.StartTimeId,
                OptionType = option.OptionType,
                ServiceType = option.ServiceType,
                RoomType = option.RoomType,
                ScheduleId = option.ScheduleId,
                ProductType = option.ProductType,
                RefNo = option.RefNo,

                Cancellable = option.Cancellable,
                CancellationText = option.CancellationText,
                Holdable = option.Holdable,
                Refundable = option.Refundable,
                Type = option.Type,
                HoldablePeriod = option.HoldablePeriod,
                Time = option.Time,
                RateId = option.RateId,
                PriceId = option.PriceId,
                SupplierId = option.SupplierId,

                BasePrice = CalculatePriceForAllPax(option.BasePrice, criteria),
                CostPrice = CalculatePriceForAllPax(option.CostPrice, criteria),
                GateBasePrice = CalculatePriceForAllPax(option.GateBasePrice, criteria),
                AvailabilityStatus = option.AvailabilityStatus,
                Customers = option.Customers,
                TravelInfo = option.TravelInfo,
                CommisionPercent = option.CommisionPercent,
                CancellationPrices = option.CancellationPrices,
                IsSelected = option.IsSelected,

                Id = option.Id,
                ServiceOptionId = productOption.Id,
                Name = productOption.Name,
                SupplierName = productOption.SupplierName,
                Description = productOption.Description,
                BookingStatus = productOption.BookingStatus,
                OptionKey = productOption.OptionKey,
                Capacity = productOption.Capacity,
                Quantity = productOption.Quantity,
                SupplierOptionCode = productOption.SupplierOptionCode,
                Margin = productOption.Margin,
                TicketTypeIds = option.TicketTypeIds,
                ApiCancellationPolicy = option.ApiCancellationPolicy,
            };
        }

        private IsangoErrorEntity SendException(Int32 activityId, string message)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "GlobalTixService",
                MethodName = "GetAvailability",
                Params = $"{activityId}"
            };
            _log.Error(isangoErrorEntity, new Exception(message));
            var data = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                ReasonPhrase = message
            };
            throw new HttpResponseException(data);
        }
    }
}