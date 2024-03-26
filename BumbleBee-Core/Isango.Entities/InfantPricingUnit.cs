using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class InfantPricingUnit : PerPersonPricingUnit
    {
        public InfantPricingUnit()
        {
            PassengerType = PassengerType.Infant;
        }
    }
}