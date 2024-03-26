using System;

namespace Isango.Entities.Master
{
    public class GeoDetails
    {
        public Int32 ContinentRegionID { get; set; }
        public string ContinentName { get; set; }

        public string ContinentRegionCode { get; set; }
        public Int32 CountryRegionID { get; set; }
        public string CountryName { get; set; }
        public string CountryRegionCode { get; set; }
        public Int32 DestinationRegionID { get; set; }
        public string DestinationName { get; set; }

        public string DestinationRegionCode { get; set; }
        public string Latitudes { get; set; }
        public string Longitudes { get; set; }

        public bool IsCountryChange { get; set; }
    }
}