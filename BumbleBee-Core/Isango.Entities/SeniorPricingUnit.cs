using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class SeniorPricingUnit : PerPersonPricingUnit
    {
        public SeniorPricingUnit()
        {
            PassengerType = PassengerType.Senior;
        }
    }
}