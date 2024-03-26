using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class TwoAndUnderPricingUnit : PerPersonPricingUnit
    {
        public TwoAndUnderPricingUnit()
        {
            PassengerType = PassengerType.TwoAndUnder;
        }
    }
}