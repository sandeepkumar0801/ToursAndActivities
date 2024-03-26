using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Entities
{
    public abstract class PriceAndAvailability : ICloneable
    {
        public List<PricingUnit> PricingUnits { get; set; }
        public decimal TotalPrice { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }
        public bool IsSelected { get; set; }
        public float MinDuration { get; set; }
        public float MaxDuration { get; set; }
        public int TourDepartureId { get; set; }
        public bool IsCapacityCheckRequired { get; set; }
        public int Capacity { get; set; }
        public int UnitQuantity { get; set; }
        public string ReferenceId { get; set; }

        public List<PerPersonPricingUnit> PerPersonPricingUnit { get; set; }

        public object Clone()
        {
            var priceAndAvailability = (PriceAndAvailability)MemberwiseClone();
            priceAndAvailability.PricingUnits = PricingUnits?.Select(x => x.DeepCopy()).ToList();
            priceAndAvailability.TotalPrice = TotalPrice;
            priceAndAvailability.AvailabilityStatus = AvailabilityStatus;
            priceAndAvailability.IsSelected = IsSelected;
            priceAndAvailability.MinDuration = MinDuration;
            priceAndAvailability.MaxDuration = MaxDuration;
            priceAndAvailability.TourDepartureId = TourDepartureId;
            priceAndAvailability.UnitQuantity = UnitQuantity;
            priceAndAvailability.Capacity = Capacity;
            priceAndAvailability.IsCapacityCheckRequired = IsCapacityCheckRequired;
            return priceAndAvailability;
        }
    }
}