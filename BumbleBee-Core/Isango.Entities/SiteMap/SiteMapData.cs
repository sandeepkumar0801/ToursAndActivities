namespace Isango.Entities.SiteMap
{
    public class SiteMapData
    {
        public string RegionName { get; set; }
        public int RegionId { get; set; }
        public string RegionType { get; set; }
        public int ParentId { get; set; }
        public int Order { get; set; }
        public string Url { get; set; }
    }
}