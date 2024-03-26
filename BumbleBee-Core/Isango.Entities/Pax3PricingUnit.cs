using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class Pax3PricingUnit : PerPersonPricingUnit
    {
        public Pax3PricingUnit()
        {
            PassengerType = PassengerType.Pax3;
        }
    }
}