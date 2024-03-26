using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class PerPersonPricingUnit : PricingUnit
    {
        public PerPersonPricingUnit()
        {
            UnitType = UnitType.PerPerson;
            PriceType = PriceType.PerPerson;
        }

        public PassengerType PassengerType { get; set; }
        //We can get multiple AgegroupID for Same pax type for Check availability API from UI.
        public int AgeGroupId { get; set; } //Rezdy
    }
}