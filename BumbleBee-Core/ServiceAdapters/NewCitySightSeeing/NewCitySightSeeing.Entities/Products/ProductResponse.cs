using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Product
{

    public class Products
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("sku")]
        public string Sku { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("notes")]
        public string Notes { get; set; }
        [JsonProperty("duration")]
        public string Duration { get; set; }
        [JsonProperty("availableDays")]
        public string? AvailableDays { get; set; }
        [JsonProperty("cancellationPolicy")]
        public string CancellationPolicy { get; set; }
        [JsonProperty("destination")]
        public Destination Destination { get; set; }
        [JsonProperty("meetingPoints")]
        public List<Meetingpoint> MeetingPoints { get; set; }
        [JsonProperty("variants")]
        public List<Variant> Variants { get; set; }
    }

    public class Destination
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("subTitle")]
        public string SubTitle { get; set; }
        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }
        [JsonProperty("geoData")]
        public Geodata GeoData { get; set; }
    }

    public class Geodata
    {
        [JsonProperty("lat")]
        public float Lat { get; set; }
        [JsonProperty("lon")]
        public float Lon { get; set; }
        [JsonProperty("zoom")]
        public float Zoom { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
    }

    public class Meetingpoint
    {
        [JsonProperty("lat")]
        public float Lat { get; set; }
        [JsonProperty("lon")]
        public float Lon { get; set; }
        [JsonProperty("zoom")]
        public float Zoom { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
    }

    public class Variant
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("variantCode")]
        public string VariantCode { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("variantName")]
        public string VariantName { get; set; }
    }
}