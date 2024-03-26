using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Tiqets;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Tiqets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Util;

namespace Isango.Service.SupplierServices
{
    public class TiqetsService : SupplierServiceBase, ISupplierService
    {
        private readonly ITiqetsPaxMappingCacheManager _tiqetsPaxMappingCacheManager;
        private readonly IMasterPersistence _masterPersistence;
        private readonly ITiqetsAdapter _tiqetsAdapter;
        private readonly ILogger _log;

        public TiqetsService(ITiqetsAdapter tiqetsAdapter, ITiqetsPaxMappingCacheManager tiqetsPaxMappingCacheManager,
            IMasterPersistence masterPersistence, ILogger log)
        {
            _tiqetsPaxMappingCacheManager = tiqetsPaxMappingCacheManager;
            _masterPersistence = masterPersistence;
            _tiqetsAdapter = tiqetsAdapter;
            _log = log;
        }

        public Activity GetAvailability(Activity activity, Criteria criteria, string token)
        {
            var tiqetsOptions = new List<ProductOption>();
            var tiqetsCriteria = (TiqetsCriteria)criteria;
            //Get requested ticket count
            var requestedTicketsCount = tiqetsCriteria.NoOfPassengers.Sum(x => x.Value);

            foreach (var po in activity.ProductOptions)
            {
                tiqetsCriteria.OptionId = po.ServiceOptionId = po.Id;
                tiqetsCriteria.OptionName = po.Name?.Trim();

                var apiOptions = _tiqetsAdapter.GetAvailabilities(tiqetsCriteria, token);

                if (apiOptions?.Any() == true)
                    tiqetsOptions.AddRange(apiOptions);
            }
            if (tiqetsOptions?.Count > 0)
            {
                ValidateOptionPaxCriteria(tiqetsOptions, tiqetsCriteria.NoOfPassengers);

                tiqetsOptions.RemoveAll(x => x.Quantity < requestedTicketsCount
                                                    || x.AvailabilityStatus == AvailabilityStatus.NOTAVAILABLE);
                if (tiqetsOptions?.Count > 0)
                {
                    var tiqetsActivities = new List<Activity>
                    {
                        new Activity { ProductOptions = tiqetsOptions}
                    };
                    //Map API options to activity
                    var tiqetsActivity = MapActivity(activity, tiqetsActivities, tiqetsCriteria);
                    return tiqetsActivity;
                }
            }
            else
            {
                var message = Constant.APIActivityOptionsNot + Constant.TiqetsAPI + " .Id:" + activity.ID;
                SendException(activity.ID, message);
            }

            return activity;
        }

        public Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo)
        {
            var paxMappings = _tiqetsPaxMappingCacheManager.GetPaxMappings();
            if (paxMappings == null)
            {
                paxMappings = _masterPersistence.LoadTiqetsPaxMappings()?.Where(x => x.APIType == APIType.Tiqets)?.ToList();
            }
            var query = from ao in activity?.ProductOptions
                        from pm in paxMappings
                        where pm != null
                              && ao != null
                              && pm.ServiceOptionId == ao.Id
                        select pm;
            paxMappings = query?.ToList();

            //Prepare tiqets criteria
            var tiqetsCriteria = new TiqetsCriteria
            {
                CheckinDate = criteria.CheckinDate,
                CheckoutDate = criteria.CheckoutDate,
                NoOfPassengers = criteria.NoOfPassengers,
                Ages = criteria.Ages,
                ProductId = activity.FactsheetId,
                Language = clientInfo.LanguageCode,
                OptionId = activity.ProductOptions?.FirstOrDefault()?.Id ?? 0,
                OptionName = activity.ProductOptions?.FirstOrDefault()?.Name?.Trim(),
                TiqetsPaxMappings = paxMappings,
                Token = criteria.Token,
                ActivityId = activity.ID,
                AffiliateId= clientInfo.AffiliateId
            };

            return tiqetsCriteria;
        }

        private void ValidateOptionPaxCriteria(List<ProductOption> productOptions, Dictionary<PassengerType, int> noOfPassengers)
        {
            var criteriaPaxes = string.Empty;
            try
            {
                var cPaxes = noOfPassengers.Keys.ToArray()?
                    .ToList()?
                    .Where(y => y != PassengerType.Infant)?
                    .OrderBy(x => x);

                criteriaPaxes = string.Join(",", cPaxes);
                foreach (var po in productOptions)
                {
                    var puQuery = from pa in po?.BasePrice?.DatePriceAndAvailabilty
                                  from pu in pa.Value?.PricingUnits
                                  from nop in noOfPassengers
                                  where ((PerPersonPricingUnit)pu).PassengerType == nop.Key
                                        && ((PerPersonPricingUnit)pu).PassengerType != PassengerType.Infant
                                  select new { AvailableDate = pa.Key, PassengerTypes = ((PerPersonPricingUnit)pu).PassengerType };

                    var groupedPassengerTypes = puQuery.GroupBy(e => e.AvailableDate);
                    foreach (var item in groupedPassengerTypes)
                    {
                        var puPaxes = string.Join(",", item.Select(e => e.PassengerTypes));
                        foreach (var pa in puPaxes.Split(','))
                        {
                            if (!criteriaPaxes?.ToLower().Contains(pa.ToString().ToLower()) ?? false)
                            {
                                if (po.BasePrice.DatePriceAndAvailabilty.ContainsKey(item.Key))
                                {
                                    po.BasePrice.DatePriceAndAvailabilty[item.Key].AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                                }
                            }
                        }
                    }
                    if (po.BasePrice.DatePriceAndAvailabilty.All(e => e.Value.AvailabilityStatus == AvailabilityStatus.NOTAVAILABLE))
                        po.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                }
                productOptions.RemoveAll(x => x.AvailabilityStatus == AvailabilityStatus.NOTAVAILABLE);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Activity MapActivity(Activity activity, List<Activity> activitiesFromAPI, Criteria criteria)
        {
            var productOptions = new List<ProductOption>();
            foreach (var productOption in activitiesFromAPI.FirstOrDefault().ProductOptions)
            {
                var activityOptionFromCache = activity.ProductOptions.FirstOrDefault(x => x.ServiceOptionId == productOption.ServiceOptionId);
                var option = MapActivityOption((ActivityOption)productOption, activityOptionFromCache, criteria);

                option.Id = productOption.Id;
                option.Name = productOption.Name;
                option.RequiresVisitorsDetails = ((ActivityOption)productOption).RequiresVisitorsDetails;
                //option.RequiresVisitorsDetailsWithVariant= ((ActivityOption)productOption).RequiresVisitorsDetailsWithVariant;
                productOptions.Add(option);
            }
            activity.ProductOptions = productOptions;

            return activity;
        }

        private IsangoErrorEntity SendException(Int32 activityId, string message)
        {
            var isangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = "TiqetsService",
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