using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels.CheckAvailability
{
    public class PriceAndAvailability
    {
        public virtual DateTime DateAndTime { get; set; }
        public virtual string ReferenceId { get; set; }
        public virtual string AvailabilityStatus { get; set; }
        public virtual Decimal TotalPrice { get; set; }
        public virtual int Quantity { get; set; }
        public virtual string UnitType { get; set; }
        public virtual bool IsCapacityCheckRequired { get; set; }
        public virtual int Capacity { get; set; }
        public virtual List<PricingUnit> PricingUnits { get; set; }
    }
}