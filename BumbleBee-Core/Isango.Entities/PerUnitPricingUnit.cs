using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class PerUnitPricingUnit : PricingUnit
    {
        public PerUnitPricingUnit()
        {
            UnitType = UnitType.PerUnit;
        }
    }
}