using ActivityWrapper.Entities;
using Isango.Entities;
using Isango.Entities.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace ActivityWrapper.Mapper
{
	public class ActivityWrapperMapper
	{
		public IEnumerable<WrapperActivity> GetWrapperActivities(List<Activity> activities)
		{
			var wrapperActivities = new List<WrapperActivity>();

            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");

            Parallel.ForEach(activities, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, activity =>
			{
				var currencyIsoCode = activity.ProductOptions.FirstOrDefault()?.BasePrice.Currency.IsoCode;
				if (currencyIsoCode == null) return;
				var wrapperActivity = new WrapperActivity
				{
					Id = activity.ID,
					BaseMinPrice = activity.BaseMinPrice,
					GateBaseMinPrice = activity.GateBaseMinPrice,
					Currency = currencyIsoCode
				};
				wrapperActivities.Add(wrapperActivity);
			});
			return wrapperActivities;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="affiliateId"></param>
		/// <param name="calendarAvailabilities"></param>
		/// <param name="calendarWithDefaultAffiliateIdAvailabilities"></param>
		/// <param name="isB2B"></param>
		/// <param name="activityId"></param>
		/// <returns></returns>
		public ActivityCalendarAvailabilities GetPriceAndAvailabilities(int activityId, string affiliateId, IEnumerable<CalendarAvailability> calendarAvailabilities, IEnumerable<CalendarAvailability> calendarWithDefaultAffiliateIdAvailabilities, bool isB2B)
		{

			if (calendarAvailabilities == null && calendarWithDefaultAffiliateIdAvailabilities == null) return null;
			var datePriceAvailability = new Dictionary<DateTime, decimal>();
			var activityCalendarAvailability = new ActivityCalendarAvailabilities
			{
				ActivityId = activityId,
				AffiliateId = affiliateId,
				CurrencyIsoCode = calendarAvailabilities?.FirstOrDefault()?.Currency ?? calendarWithDefaultAffiliateIdAvailabilities?.FirstOrDefault()?.Currency,
				DatePriceAvailability = new Dictionary<DateTime, decimal>()

			};
			foreach (var availability in calendarWithDefaultAffiliateIdAvailabilities)
			{

				var price = isB2B ? availability.B2BBasePrice : availability.B2CBasePrice;
				var range = Enumerable.Range(0, 1 + availability.EndDate.Subtract(availability.StartDate).Days).Select(i => availability.StartDate.AddDays(i));

				foreach (var item in range)
				{
					datePriceAvailability.Add(item, price);
				}
			}

			foreach (var availability in calendarAvailabilities)
			{

				var price = isB2B ? availability.B2BBasePrice : availability.B2CBasePrice;
				var range = Enumerable.Range(0, 1 + availability.EndDate.Subtract(availability.StartDate).Days).Select(i => availability.StartDate.AddDays(i));

				foreach (var item in range)
				{
					if (datePriceAvailability.ContainsKey(item))
					{
						datePriceAvailability[item] = price;
					}
					else
					{
						datePriceAvailability.Add(item, price);
					}
				}
			}

			activityCalendarAvailability.DatePriceAvailability = datePriceAvailability.OrderBy(k => k.Key).ToDictionary(k => k.Key, k => k.Value);
			return activityCalendarAvailability;

		}
	}
}
