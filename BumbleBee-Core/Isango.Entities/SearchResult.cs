using Isango.Entities.Activities;
using Isango.Entities.Region;
using System.Collections.Generic;

namespace Isango.Entities
{
    public class SearchResult
    {
        public RegionMetaData RegionMeta { get; set; }

        public List<Product> Products { get; set; }

        public List<Activity> Activities { get; set; }

        public List<int> CategoryIds { get; set; }

        public int TotalActivities { get; set; }
    }
}