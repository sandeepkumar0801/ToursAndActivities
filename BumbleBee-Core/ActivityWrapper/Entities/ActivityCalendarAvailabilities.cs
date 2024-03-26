using System;
using System.Collections.Generic;

// ReSharper disable All

namespace ActivityWrapper.Entities
{
	public class ActivityCalendarAvailabilities
	{
		public int ActivityId { get; set; }
		public string AffiliateId { get; set; }
		public Dictionary<DateTime, Decimal> DatePriceAvailability { get; set; }
		public string CurrencyIsoCode { get; set; }
	}
}