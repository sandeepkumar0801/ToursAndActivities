using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class CitySightseeingMapping
    {
        public string SupplierCode { get; set; }

        public int ChildFromAge { get; set; }

        public int ChildToAge { get; set; }

        public PassengerType PassengerType { get; set; }
    }
}