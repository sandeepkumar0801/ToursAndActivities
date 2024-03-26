using System;

namespace Isango.Entities.HotelBeds
{
    public class ContentProduct
    {
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public string FactsheetId { get; set; }

        public string ProductName { get; set; }

        public string Town { get; set; }

        public string Street { get; set; }

        public string Zip { get; set; }

        public string LanguagCode { get; set; }
        public DateTime LastUpdate { get; set; }
        public string DestinationCode { get; set; }

        public string ActivityFactsheetType { get; set; }

        public string ActivityCode { get; set; }
        public string Description { get; set; }

        public string GuideType { get; set; }

        public bool Included { get; set; }
    }
}