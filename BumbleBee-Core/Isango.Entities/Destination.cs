namespace Isango.Entities
{
    public class Destination
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LanguageCode { get; set; }

        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public bool IsCountryChange { get; set; }

        public string Latitudes { get; set; }
        public string Longitudes { get; set; }
    }
}