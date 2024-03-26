using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class TwinPricingUnit : PerPersonPricingUnit
    {
        public TwinPricingUnit()
        {
            PassengerType = PassengerType.Twin;
        }
    }
}