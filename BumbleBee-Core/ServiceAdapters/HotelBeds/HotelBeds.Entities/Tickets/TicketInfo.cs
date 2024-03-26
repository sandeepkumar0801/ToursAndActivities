using System.Collections.Generic;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class TicketInfo : EntityBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<Description> DescriptionList { get; set; }
        public string CompanyCode { get; set; }
        public char TicketClass { get; set; }
        public Destination Destination { get; set; }
        public string Url { get; set; }
        public List<Image> ImageList { get; set; }
    }
}