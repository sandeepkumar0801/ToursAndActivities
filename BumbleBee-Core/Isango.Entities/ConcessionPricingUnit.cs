using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class ConcessionPricingUnit : PerPersonPricingUnit
    {
        public ConcessionPricingUnit()
        {
            PassengerType = PassengerType.Concession;
        }
    }
}