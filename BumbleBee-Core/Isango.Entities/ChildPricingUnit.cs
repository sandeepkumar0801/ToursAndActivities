using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class ChildPricingUnit : PerPersonPricingUnit
    {
        public ChildPricingUnit()
        {
            PassengerType = PassengerType.Child;
        }
    }
}