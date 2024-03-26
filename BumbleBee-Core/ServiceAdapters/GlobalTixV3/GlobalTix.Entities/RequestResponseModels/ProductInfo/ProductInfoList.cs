using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.ProductInfo
{
    public class ProductInfoList
    {

        [JsonProperty(PropertyName = "data")]
        public Data data { get; set; }

        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }

        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
        

        public class Data
        {
            [JsonProperty(PropertyName = "country")]
            public string Country { get; set; }

            [JsonProperty(PropertyName = "originalPrice")]
            public double OriginalPrice { get; set; }

            [JsonProperty(PropertyName = "keywords")]
            public object Keywords { get; set; }

            [JsonProperty(PropertyName = "blockedDate")]
            public List<object> BlockedDate { get; set; }

            [JsonProperty(PropertyName = "fromPrice")]
            public float? FromPrice { get; set; }

            [JsonProperty(PropertyName = "city")]
            public string City { get; set; }

            [JsonProperty(PropertyName = "latitude")]
            public double? Latitude { get; set; }

            [JsonProperty(PropertyName = "description")]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "media")]
            public List<object> Media { get; set; }

            [JsonProperty(PropertyName = "countryId")]
            public int CountryId { get; set; }

            [JsonProperty(PropertyName = "timezoneOffset")]
            public int? TimezoneOffset { get; set; }

            [JsonProperty(PropertyName = "currency")]
            public string Currency { get; set; }

            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }

            [JsonProperty(PropertyName = "isGTRecommend")]
            public bool IsGTRecommend { get; set; }

            [JsonProperty(PropertyName = "longitude")]
            public double? Longitude { get; set; }

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
            public List<object> Highlights { get; set; }

            [JsonProperty(PropertyName = "operatingHours")]
            public Operatinghours OperatingHours { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "isInstantConfirmation")]
            public bool IsInstantConfirmation { get; set; }

            [JsonProperty(PropertyName = "location")]
            public string Location { get; set; }

            [JsonProperty(PropertyName = "category")]
            public string Category { get; set; }

            [JsonProperty(PropertyName = "thingsToNote")]
            public List<object> ThingsToNote { get; set; }
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
            public List<object>  FixedDays { get; set; }

            [JsonProperty(PropertyName = "isToursActivities")]
            public object IsToursActivities { get; set; }

            [JsonProperty(PropertyName = "custom")]
            public string Custom { get; set; }
        }

    }


}
