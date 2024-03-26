namespace Isango.Entities.HotelBeds
{
    public class ContentRoute
    {
        public int Factsheetid { get; set; }
        public string Duration { get; set; }

        public string TimeFrom { get; set; }

        public string TimeTo { get; set; }

        public string Type { get; set; }
        public int Pointsorder { get; set; }
        public bool Stop { get; set; }

        public string GeoLocationLatitude { get; set; }
        public string GeoLocationLongitude { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }
        public string Description { get; set; }

        public string Language { get; set; }
        public string POI_description2 { get; set; }
    }
}