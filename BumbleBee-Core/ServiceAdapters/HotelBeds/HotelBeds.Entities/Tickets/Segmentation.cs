using System.Collections.Generic;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class Segmentation : EntityBase
    {
        public List<SegmentationGroup> SegmentationGroups { get; set; }
    }
}