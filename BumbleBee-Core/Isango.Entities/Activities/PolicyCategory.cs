using Isango.Entities.Enums;

namespace Isango.Entities.Activities
{
    public class PolicyCategory
    {
        public string Id { get; set; }
        public int MinimumCustomers { get; set; }
        public int MaximumCustomers { get; set; }
        public PassengerType PolicyCategoryType { get; set; }
        public ActivityPrice PerUnitPrice { get; set; }
        public int FromAge { get; set; }
        public int ToAge { get; set; }
    }
}