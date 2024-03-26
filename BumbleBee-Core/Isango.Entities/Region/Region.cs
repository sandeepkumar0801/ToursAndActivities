using Isango.Entities.Enums;

namespace Isango.Entities.Region
{
    public class Region
    {
        public int ParentId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public RegionType Type { get; set; }
        public string Url { get; set; }
        public string IsoCode { get; set; }
    }
}