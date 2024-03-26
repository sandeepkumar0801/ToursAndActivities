using Isango.Entities.Activities;
using Isango.Entities.Region;
using System.Collections.Generic;

namespace Isango.Entities
{
    public class SearchStack
    {
        public List<RegionCategoryMapping> RegionCategoryMappings { get; set; }
        public RegionMetaData RegionMeta { get; set; }
        public List<Product> Products { get; set; }
        public List<Region.Region> Regions { get; set; }
        public List<Activity> Activities { get; set; }
        public List<int> CategoryIds { get; set; }
        public bool IsShowMoreVisible { get; set; }
        public int TotalActivities { get; set; }
    }
}