using Isango.Entities.Enums;

namespace Isango.Entities.Tiqets
{
    public class TiqetsPaxMapping
    {
        public int ServiceOptionId { get; set; }

        public int AgeGroupId { get; set; }

        public string AgeGroupCode { get; set; }

        public PassengerType PassengerType { get; set; }

        public APIType APIType { get; set; }
    }
}