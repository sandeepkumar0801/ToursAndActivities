using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class FamilyPricingUnit : PerPersonPricingUnit
    {
        public FamilyPricingUnit()
        {
            PassengerType = PassengerType.Family;
        }
    }
}