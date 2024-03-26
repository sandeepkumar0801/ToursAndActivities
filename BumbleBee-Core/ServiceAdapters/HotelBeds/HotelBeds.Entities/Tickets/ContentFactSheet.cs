using System.Collections.Generic;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class ContentFactSheet : EntityBase
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public List<Description> DescriptionList { get; set; }
        public List<Image> ImageList { get; set; }
        public Destination Destination { get; set; }
        public TicketPosition TicketPosition { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public List<SegmentationGroup> SegmentationGroups { get; set; }
        public string ShortDescription { get; set; }
        public List<Day> OperationDays { get; set; }
        public List<TicketFeature> TicketFeatureList { get; set; }
    }
}