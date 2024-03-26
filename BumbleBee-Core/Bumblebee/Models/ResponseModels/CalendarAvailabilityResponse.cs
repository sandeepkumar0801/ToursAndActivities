using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.ResponseModels
{
    public class CalendarAvailabilityResponse
    {
        
            public int ActivityId { get; set; }
            public string AffiliateId { get; set; }
            public string CurrencyIsoCode { get; set; }
            public List<DatePriceAvailability1> DatePriceAvailability { get; set; }
            //public Dictionary<DateTime, Decimal> DatePriceAvailability { get; set; }
    }
    public class DatePriceAvailability1
        {
            public DateTime DateTimeAvailability { get; set; }
        }
}
