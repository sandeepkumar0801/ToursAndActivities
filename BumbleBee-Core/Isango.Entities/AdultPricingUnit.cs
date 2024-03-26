using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class AdultPricingUnit : PerPersonPricingUnit
    {
        public AdultPricingUnit()
        {
            PassengerType = PassengerType.Adult;
        }
    }
}