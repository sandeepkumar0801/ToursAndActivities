using System.Collections.Generic;

namespace Isango.Entities.Activities
{
    public class ActivityCountryRestriction
    {
        public List<string> Countries { get; set; }
        public int ProductId { get; set; }
        public bool IsShow { get; set; }
    }
}