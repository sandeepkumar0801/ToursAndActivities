using System.Collections.Generic;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class Contract : EntityBase
    {
        public string Name { get; set; }
        public string IncomingOffice { get; set; }
        public string Classification { get; set; }
        public List<Comment> Comments { get; set; }
    }
}