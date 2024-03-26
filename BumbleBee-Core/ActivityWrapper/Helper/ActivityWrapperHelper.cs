using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Service.Contract;
using PriceRuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ActivityWrapper.Helper
{
    public class ActivityWrapperHelper
    {
        private readonly IActivityService _activityService;
        private readonly IAffiliateService _affiliateService;
        private readonly PricingController _pricingController;

        public ActivityWrapperHelper(IActivityService activityService, PricingController pricingController, IAffiliateService affiliateService)
        {
            _activityService = activityService;
            _pricingController = pricingController;
            _affiliateService = affiliateService;
        }

        public Activity GetActivity(int activityId, ClientInfo clientInfo, Criteria criteria)
        {
            //clientInfo = SetClientInfoAffilateInfo(clientInfo);
            var activity = _activityService.GetActivityDetailsAsync(activityId, clientInfo, criteria).Result;
            activity.ProductOptions = activity.ActivityType != ActivityType.Bundle ?
                GetProductOptionsAfterPriceRuleEngine(activity.PriceTypeId, activity.ProductOptions, clientInfo, criteria.CheckinDate)
                : GetBundleProductOptionsAfterPriceRuleEngine(activity.PriceTypeId, activity.ProductOptions, clientInfo);
            activity = _activityService.CalculateActivityWithMinPricesAsync(activity).Result;
            return activity;
        }

        public List<ProductOption> GetProductOptionsAfterPriceRuleEngine(PriceTypeId priceTypeId, List<ProductOption> productOptions, ClientInfo clientInfo, DateTime checkInDateTime)
        {
            var pricingRequest = new PricingRuleRequest
            {
                PriceTypeId = priceTypeId,
                Criteria = new Criteria
                {
                    CheckinDate = checkInDateTime
                },
                ProductOptions = productOptions.Where(x => x.AvailabilityStatus != AvailabilityStatus.NOTAVAILABLE).ToList(),
                ClientInfo = clientInfo
            };
            var updatedOptions = _pricingController.Process(pricingRequest);
            updatedOptions.ForEach(e =>
            {
                if (string.IsNullOrEmpty(e.BundleOptionID.ToString()))
                    productOptions[productOptions.FindIndex(x => x.Id == e.Id)] = e;
                else
                    productOptions[productOptions.FindIndex(x => x.Id == e.Id && x.BundleOptionID == e.BundleOptionID)] = e;
            });
            return productOptions;
        }

        public List<ProductOption> GetBundleProductOptionsAfterPriceRuleEngine(PriceTypeId priceTypeId, List<ProductOption> productOptions, ClientInfo clientInfo, Dictionary<int, DateTime> checkInDates = null)
        {
            foreach (var componentServiceId in productOptions.Select(e => e.ComponentServiceID).Distinct().ToList())
            {
                var checkInDate = checkInDates?[componentServiceId] ?? DateTime.Now;
                var options = productOptions.Where(e => e.ComponentServiceID == componentServiceId).ToList();
                var updatedOptions = GetProductOptionsAfterPriceRuleEngine(priceTypeId, options, clientInfo, checkInDate);
                updatedOptions.ForEach(e =>
                {
                    productOptions[productOptions.FindIndex(x => x.Id == e.Id && x.BundleOptionID == e.BundleOptionID)] = e;
                });
            }

            return productOptions;
        }

        public IEnumerable<CalendarAvailability> GetPriceAndAvailabilities(int activityId, string affiliateId)
        {
            var activityPriceAndAvailability = _activityService.GetCalendarAvailabilityAsync(activityId, affiliateId).Result;
            //var filterActivityPriceAndAvailability = activityPriceAndAvailability.Count > 0
            //    ? activityPriceAndAvailability
            //    : _activityService.GetCalendarAvailabilityAsync(activityId, "default").Result;
            return activityPriceAndAvailability;
        }

        public bool IsB2BCategory(string affiliateId)
        {
            var affiliate = _affiliateService.GetAffiliateInformationAsync(affiliateId).Result;
            return affiliate.AffiliateConfiguration.IsB2BAffiliate;
        }

        public ClientInfo SetClientInfoAffilateInfo(ClientInfo clientInfo)
        {
            var affiliate = _affiliateService.GetAffiliateInformationAsync(clientInfo.AffiliateId).Result;
            clientInfo.IsB2BAffiliate = affiliate.AffiliateConfiguration.IsB2BAffiliate;
            clientInfo.IsSupplementOffer = affiliate.AffiliateConfiguration.IsSupplementOffer;
            return clientInfo;
        }
    }
}