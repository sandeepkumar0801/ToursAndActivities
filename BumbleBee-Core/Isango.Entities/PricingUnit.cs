using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Entities
{
    public class PricingUnit
    {
        public UnitType UnitType { get; set; }
        public PriceType PriceType { get; set; }
        //Property addition for minimum selling price task - IS-11485 - Sellprice for GlobalTix
        public decimal MinimumSellingPrice { get; set; }
        public string Currency { get; set; }
        public bool IsMinimumSellingPriceRestrictionApplicable { get; set; }

        public decimal Price { get; set; }
        public int TotalCapacity { get; set; }
        public int Quantity { get; set; }
        public bool SupportedByIsangoOnly { get; set; }
        public int? Mincapacity { get; set; }

        public List<int> Ages { get; set; }

        public PricingUnit DeepCopy()
        {
            var pricingUnit = (PricingUnit)MemberwiseClone();
            pricingUnit.UnitType = UnitType;
            pricingUnit.PriceType = PriceType;
            pricingUnit.Price = Price;
            pricingUnit.TotalCapacity = TotalCapacity;
            pricingUnit.Quantity = Quantity;
            pricingUnit.SupportedByIsangoOnly = SupportedByIsangoOnly;
            pricingUnit.Mincapacity = Mincapacity;
            pricingUnit.Ages = Ages;

            return pricingUnit;
        }
    }
}

public static class Distinct
{
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
    this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : class
    {
        var seenKeys = new HashSet<TKey>();
        return source.Where(element => seenKeys.Add(keySelector.Invoke(element) ?? default(TKey)));
    }
}