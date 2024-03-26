namespace Isango.Entities.HotelBeds
{
    public class ContentLocation
    {
        public int Factsheetid { get; set; }
        public string StartingPointstype { get; set; }

        public string Meetingpointtype { get; set; }
        public string Geolocation_latitude { get; set; }

        public string Geolocation_longitude { get; set; }
        public string Description { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }

        public string Language { get; set; }

        public string Location_Type { get; set; }
    }
}