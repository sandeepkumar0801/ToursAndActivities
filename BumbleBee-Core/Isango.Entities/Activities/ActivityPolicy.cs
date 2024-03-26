using System.Collections.Generic;

namespace Isango.Entities.Activities
{
    public class ActivityPolicy
    {
        public string Id { get; set; }
        public List<PolicyCategory> PolicyCategories { get; set; }
    }
}