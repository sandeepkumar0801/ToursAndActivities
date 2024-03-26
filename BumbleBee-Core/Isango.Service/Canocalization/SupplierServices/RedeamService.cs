using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Redeam;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Redeam;
using System;
using System.Collections.Generic;
using System.Linq;
using Util;

namespace Isango.Service.SupplierServices
{
    public class RedeamService : SupplierServiceBase, ISupplierService
    {
        private IRedeamAdapter _redeamAdapter;
        private readonly IMemCache _memCache;
        private readonly ILogger _log;

        public RedeamService(IRedeamAdapter redeamAdapter, IMemCache memCache, ILogger log)
        {
            _redeamAdapter = redeamAdapter;
            _memCache = memCache;
            _log = log;
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var supplierId = activity.Code;
            var productId = string.Empty;
            var rateIds = new List<string>();
            /*checking for activity code to get supplier id and product id and
                            if length after spiting code by # is 2, then both supplier id and product id are present*/
            activity.ProductOptions = activity?.ProductOptions?.Where(x => x?.SupplierOptionCode?.Split('#').Length == 2).ToList();
            if (!string.IsNullOrWhiteSpace(activity?.Code) && activity?.ProductOptions?.Count > 0)
            {
                //var splitResult = activity.Code.Split('#');

                rateIds = activity?.ProductOptions?
                    .Where(y => !string.IsNullOrWhiteSpace(y.SupplierOptionCode))?
                    .Select(x => x.SupplierOptionCode)?
                    .Distinct().ToList();

                var rateIdAndType = new Dictionary<string, string>();

                activity?.ProductOptions?.ForEach(x =>
                {
                    if (!rateIdAndType.Keys.Contains(x.SupplierOptionCode))
                    {
                        rateIdAndType.Add(x.SupplierOptionCode, x.PrefixServiceCode);
                    }
                });

                var redeamCriteria = new RedeamCriteria
                {
                    CheckinDate = criteria.CheckinDate,
                    CheckoutDate = criteria.CheckoutDate,
                    NoOfPassengers = criteria.NoOfPassengers,
                    SupplierId = supplierId,
                    ProductId = productId,
                    RateIds = rateIds,
                    RateIdAndType = rateIdAndType,
                    ApiToken = clientInfo.ApiToken,
                    Token = criteria.Token,
                    Language = criteria.Language
                };

                return redeamCriteria;
            }
            else
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamService",
                    MethodName = "CreateCriteria",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"supplierId:-{supplierId},productId:-{productId}, RateIds:-{string.Join(",", rateIds?.ToArray())}"
                };
                _log.Error(isangoErrorEntity, new Exception("Supplier Ids and Product Ids not found."));
            }
            return null;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var redeamCriteria = (RedeamCriteria)criteria;
            try
            {
                var readeamActivity = MapActivity(activity, new List<Activity>(), redeamCriteria);
                return readeamActivity;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamService",
                    MethodName = "GetAvailability",
                    Token = token,
                    Params = $"redeamCriteria:{SerializeDeSerializeHelper.Serialize(redeamCriteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
            }
            return null;
        }

        public Activity MapActivity(Activity activity, List<Activity> activitiesFromApi, Criteria criteria)
        {
            var redeamCriteria = (RedeamCriteria)criteria;

            var apiActivity = GetRedeamActivity(activity, redeamCriteria.ApiToken, redeamCriteria);
            return activity;
        }

        #region Redeam Private Methods code moved from develop branch to develop saviant

        /// <summary>
        /// Make adapter calls and map the activity cache and supplier
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="clientInfo"></param>
        /// <param name="redeamCriteria"></param>
        /// <returns></returns>
        private Activity GetRedeamActivity(Activity activity, string token, RedeamCriteria redeamCriteria)
        {
            var redeamProductOptions = _redeamAdapter.GetAvailabilities(redeamCriteria, token).GetAwaiter().GetResult();
            if (redeamProductOptions?.Count > 0)
            {
                var readeamActivity = LoadMappedDataRedeam(activity, redeamProductOptions, redeamCriteria);
                return readeamActivity;
            }
            return activity;
        }

        /// <summary>
        /// Map data for Redeam
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="optionsFromAPI"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private Activity LoadMappedDataRedeam(Activity activity, List<ProductOption> optionsFromAPI, Criteria criteria)
        {
            var productOptions = new List<ProductOption>();
            foreach (var activityOption in optionsFromAPI)
            {
                var activityOptionFromAPI = (ActivityOption)activityOption;
                var activityOptionFromCache = activity.ProductOptions.FirstOrDefault(
                    x => (x).SupplierOptionCode == activityOptionFromAPI.SupplierOptionCode);
                if (activityOptionFromCache == null) continue;

                var option = GetMappedActivityOption(activityOptionFromAPI, activityOptionFromCache, criteria);
                option.Id = Math.Abs(Guid.NewGuid().GetHashCode());
                option.PickupLocations = activityOptionFromAPI.PickupLocations;
                // Update name of Option with time from API
                if (!string.IsNullOrWhiteSpace(activityOptionFromAPI.Name)
                    && (activityOptionFromAPI.Name != Constant.ZeroTime)
                    )
                    option.Name = $"{activityOptionFromCache.Name.TrimEnd()} - {activityOptionFromAPI.Name}";
                productOptions.Add(option);
            }
            activity.ProductOptions = productOptions;
            if (activity.ProductOptions.Count == 0)
            {
                activity.ProductOptions = optionsFromAPI;
            }
            return activity;
        }

        /// <summary>
        /// Map ActivityOption data from source to destination object
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="productOption"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private ActivityOption GetMappedActivityOption(ActivityOption option, ProductOption productOption, Criteria criteria)
        {
            return new ActivityOption
            {
                ActivitySeasons = option.ActivitySeasons,
                AvailToken = option.AvailToken,
                Code = option.Code,
                Contract = option.Contract,
                ContractQuestions = option.ContractQuestions,
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

                Id = productOption.Id,
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
                CancellationText = option.CancellationText,
                ApiCancellationPolicy = option.ApiCancellationPolicy,
            };
        }

        #endregion Redeam Private Methods code moved from develop branch to develop saviant

    }
}