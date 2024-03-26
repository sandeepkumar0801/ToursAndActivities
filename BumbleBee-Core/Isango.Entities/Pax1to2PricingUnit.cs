using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class Pax1To2PricingUnit : PerPersonPricingUnit
    {
        public Pax1To2PricingUnit()
        {
            PassengerType = PassengerType.Pax1To2;
        }
    }
}