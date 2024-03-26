using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class SinglePricingUnit : PerPersonPricingUnit
    {
        public SinglePricingUnit()
        {
            PassengerType = PassengerType.Single;
        }
    }
}