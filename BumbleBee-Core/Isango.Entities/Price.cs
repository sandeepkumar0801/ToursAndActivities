using System;
using System.Collections.Generic;

namespace Isango.Entities
{
    public class Price
    {
        public Currency Currency { get; set; }

        public decimal Amount { get; set; }

        public Dictionary<DateTime, PriceAndAvailability> DatePriceAndAvailabilty { get; set; }

        public Price DeepCopy()
        {
            var price = (Price)MemberwiseClone();
            price.Amount = Amount;
            price.Currency = new Currency(Currency);
            price.DatePriceAndAvailabilty = CloneDatePriceAndAvailabilty(DatePriceAndAvailabilty);
            return price;
        }

        public Dictionary<TKey, TValue> CloneDatePriceAndAvailabilty<TKey, TValue>
            (Dictionary<TKey, TValue> values) where TValue : ICloneable
        {
            if (values == null)
                return null;
            var ret = new Dictionary<TKey, TValue>(values.Count,
                values.Comparer);
            foreach (var value in values)
            {
                ret.Add(value.Key, (TValue)value.Value.Clone());
            }
            return ret;
        }
    }
}