using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels
{
    public class ProductInfoResponse
    {
        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
    }

    public class Data
    {
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }
        [JsonProperty(PropertyName = "originalPrice")]
        public float OriginalPrice { get; set; }
        [JsonProperty(PropertyName = "keywords")]
        public object Keywords { get; set; }
        [JsonProperty(PropertyName = "blockedDate")]
        public List<Blockeddate> BlockedDate { get; set; }
        [JsonProperty(PropertyName = "fromPrice")]
        public object FromPrice { get; set; }
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }
        [JsonProperty(PropertyName = "latitude")]
        public float Latitude { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "media")]
        public List<Medium> Media { get; set; }
        [JsonProperty(PropertyName = "countryId")]
        public int CountryId { get; set; }
        [JsonProperty(PropertyName = "whatToExpect")]
        public string WhatToExpect { get; set; }
        [JsonProperty(PropertyName = "timezoneOffset")]
        public int TimezoneOffset { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "isGTRecommend")]
        public bool IsGTRecommend { get; set; }
        [JsonProperty(PropertyName = "longitude")]
        public float Longitude { get; set; }
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }
        [JsonProperty(PropertyName = "isOpenDated")]
        public bool IsOpenDated { get; set; }
        [JsonProperty(PropertyName = "isOwnContracted")]
        public bool IsOwnContracted { get; set; }
        [JsonProperty(PropertyName = "merchant")]
        public Merchant Merchant { get; set; }
        [JsonProperty(PropertyName = "isFavorited")]
        public bool IsFavorited { get; set; }
        [JsonProperty(PropertyName = "isBestSeller")]
        public bool IsBestSeller { get; set; }
        [JsonProperty(PropertyName = "fromReseller")]
        public object FromReseller { get; set; }
        [JsonProperty(PropertyName = "highlights")]
        public List<string> Highlights { get; set; }
        [JsonProperty(PropertyName = "operatingHours")]
        public Operatinghours OperatingHours { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "isInstantConfirmation")]
        public bool IsInstantConfirmation { get; set; }
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }
        [JsonProperty(PropertyName = "thingsToNote")]
        public List<string> ThingsToNote { get; set; }
    }

    public class Merchant
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }

    public class Operatinghours
    {
        [JsonProperty(PropertyName = "fixedDays")]
        public List<Fixedday> FixedDays { get; set; }
        [JsonProperty(PropertyName = "isToursActivities")]
        public string IsToursActivities { get; set; }
        [JsonProperty(PropertyName = "custom")]
        public string Custom { get; set; }
    }

    public class Fixedday
    {
        [JsonProperty(PropertyName = "day")]
        public string Day { get; set; }
        [JsonProperty(PropertyName = "startHour")]
        public string StartHour { get; set; }
        [JsonProperty(PropertyName = "endHour")]
        public string EndHour { get; set; }
    }

    public class Blockeddate
    {
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
    }

    public class Medium
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }
        [JsonProperty(PropertyName = "size")]
        public float Size { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "extension")]
        public string Extension { get; set; }
    }
}
