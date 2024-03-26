using System;

namespace WebAPI.Models.ResponseModels.Master
{
    public class GeoDetailsMasterResponse
    {
        public Nullable<int> ContinentRegionID { get; set; }
        public string ContinentName { get; set; }
        public string ContinentRegionCode { get; set; }
        public Nullable<int> CountryRegionID { get; set; }
        public string CountryName { get; set; }
        public string CountryRegionCode { get; set; }
        public int DestinationRegionID { get; set; }
        public string DestinationName { get; set; }
        public string DestinationRegionCode { get; set; }
        public string Latitudes { get; set; }
        public string Longitudes { get; set; }
    }
}