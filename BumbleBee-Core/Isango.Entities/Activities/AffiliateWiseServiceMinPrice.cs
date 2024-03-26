using Isango.Entities.Enums;
using System;

namespace Isango.Entities.Activities
{
    public class AffiliateWiseServiceMinPrice
    {
        public string AffiliateId { get; set; }
        public int ServiceId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal BasePrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal OfferPercent { get; set; }
        public string CurrencyIsoCode { get; set; }
    }
}