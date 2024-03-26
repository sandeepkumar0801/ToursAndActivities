using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class Under30PricingUnit : PerPersonPricingUnit
    {
        public Under30PricingUnit()
        {
            PassengerType = PassengerType.Under30;
        }
    }
}