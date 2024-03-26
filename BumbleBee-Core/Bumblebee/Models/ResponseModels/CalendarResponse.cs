using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels
{
    public class CalendarResponse
    {
        public int ActivityId { get; set; }
        public string AffiliateId { get; set; }
        public string CurrencyIsoCode { get; set; }
        public List<DatePriceAvailability> DatePriceAvailability { get; set; }
        //public Dictionary<DateTime, Decimal> DatePriceAvailability { get; set; }
    }
    public class CalendarResponse_activity
    {
        public int ActivityId { get; set; }
        public string AffiliateId { get; set; }
        public string CurrencyIsoCode { get; set; }
        public List<DatePriceAvailability_activity> DatePriceAvailability { get; set; }
        //public Dictionary<DateTime, Decimal> DatePriceAvailability { get; set; }
    }

    public class DatePriceAvailability
    {
        public DateTime DateTimeAvailability { get; set; }
        public decimal GateBaseMinPrice { get; set; }
        public decimal BaseMinPrice { get; set; }
    }
    public class DatePriceAvailability_activity
    {
        public DateTime DateTimeAvailability { get; set; }
       
    }

}