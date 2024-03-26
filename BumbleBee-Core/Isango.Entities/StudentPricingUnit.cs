using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class StudentPricingUnit : PerPersonPricingUnit
    {
        public StudentPricingUnit()
        {
            PassengerType = PassengerType.Student;
        }
    }
}