using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class MilitaryPricingUnit : PerPersonPricingUnit
    {
        public MilitaryPricingUnit()
        {
            PassengerType = PassengerType.Military;
        }
    }
}
