using System.Collections.Generic;

namespace Isango.Entities
{
    public class CrossSellProduct
    {
        public int RegionId { get; set; }
        public int Id { get; set; }
        public int Priority { get; set; }
        public List<int> AttractionIDs { get; set; }
        public bool HasAttractionTicket { get; set; }
        public int? LineOfBusiness { get; set; }
    }
}