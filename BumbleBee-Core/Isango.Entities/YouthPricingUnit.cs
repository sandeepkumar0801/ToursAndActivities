using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class YouthPricingUnit : PerPersonPricingUnit
    {
        public YouthPricingUnit()
        {
            PassengerType = PassengerType.Youth;
        }
    }
}