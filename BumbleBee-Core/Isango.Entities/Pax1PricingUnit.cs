using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class Pax1PricingUnit : PerPersonPricingUnit
    {
        public Pax1PricingUnit()
        {
            PassengerType = PassengerType.Pax1;
        }
    }
}