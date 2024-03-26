using System.Collections.Generic;

namespace Isango.Entities
{
    public class Contract
    {
        public string Name { get; set; }
        public string InComingOfficeCode { get; set; }
        public string Classification { get; set; }
        public string ClassificationCode { get; set; }
        public List<HotelBeds.Comment> Comments { get; set; }
    }
}