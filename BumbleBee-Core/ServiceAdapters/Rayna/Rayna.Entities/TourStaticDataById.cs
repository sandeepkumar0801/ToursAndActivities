
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Rayna.Rayna.Entities
{

    public class TourStaticDataById
    {
        [JsonProperty(PropertyName = "statuscode")]
        public int StatusCode { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "result")]
        public List<ResultTourStaticDataById> ResultTourStaticDataById { get; set; }
    }

    public class ResultTourStaticDataById
    {
        [JsonProperty(PropertyName = "tourId")]
        public int TourId { get; set; }
        [JsonProperty(PropertyName = "countryId")]
        public int CountryId { get; set; }
        [JsonProperty(PropertyName = "countryName")]
        public string CountryName { get; set; }
        [JsonProperty(PropertyName = "cityId")]
        public int CityId { get; set; }
        [JsonProperty(PropertyName = "cityName")]
        public string CityName { get; set; }
        [JsonProperty(PropertyName = "tourName")]
        public string TourName { get; set; }
        [JsonProperty(PropertyName = "reviewCount")]
        public int ReviewCount { get; set; }
        [JsonProperty(PropertyName = "rating")]
        public float Rating { get; set; }
        [JsonProperty(PropertyName = "duration")]
        public string Duration { get; set; }
        [JsonProperty(PropertyName = "departurePoint")]
        public string DeparturePoint { get; set; }
        [JsonProperty(PropertyName = "reportingTime")]
        public string ReportingTime { get; set; }
        [JsonProperty(PropertyName = "tourLanguage")]
        public string TourLanguage { get; set; }
        [JsonProperty(PropertyName = "imagePath")]
        public string ImagePath { get; set; }
        [JsonProperty(PropertyName = "imageCaptionName")]
        public string ImageCaptionName { get; set; }
        [JsonProperty(PropertyName = "cityTourTypeId")]
        public string CityTourTypeId { get; set; }
        [JsonProperty(PropertyName = "cityTourType")]
        public string CityTourType { get; set; }
        [JsonProperty(PropertyName = "tourDescription")]
        public string TourDescription { get; set; }
        [JsonProperty(PropertyName = "tourInclusion")]
        public string TourInclusion { get; set; }
        [JsonProperty(PropertyName = "tourShortDescription")]
        public string TourShortDescription { get; set; }
        [JsonProperty(PropertyName = "raynaToursAdvantage")]
        public string RaynaToursAdvantage { get; set; }
        [JsonProperty(PropertyName = "whatsInThisTour")]
        public string WhatsInThisTour { get; set; }
        [JsonProperty(PropertyName = "importantInformation")]
        public string ImportantInformation { get; set; }
        [JsonProperty(PropertyName = "itenararyDescription")]
        public string ItenararyDescription { get; set; }
        [JsonProperty(PropertyName = "usefulInformation")]
        public string UsefulInformation { get; set; }
        [JsonProperty(PropertyName = "faqDetails")]
        public string FaqDetails { get; set; }
        [JsonProperty(PropertyName = "termsAndConditions")]
        public string TermsAndConditions { get; set; }
        [JsonProperty(PropertyName = "cancellationPolicyName")]
        public string CancellationPolicyName { get; set; }
        [JsonProperty(PropertyName = "cancellationPolicyDescription")]
        public string CancellationPolicyDescription { get; set; }
        [JsonProperty(PropertyName = "childCancellationPolicyName")]
        public string ChildCancellationPolicyName { get; set; }
        [JsonProperty(PropertyName = "childCancellationPolicyDescription")]
        public string ChildCancellationPolicyDescription { get; set; }
        [JsonProperty(PropertyName = "childAge")]
        public string ChildAge { get; set; }
        [JsonProperty(PropertyName = "infantAge")]
        public string InfantAge { get; set; }
        [JsonProperty(PropertyName = "infantCount")]
        public int InfantCount { get; set; }
        [JsonProperty(PropertyName = "isSlot")]
        public bool IsSlot { get; set; }
        [JsonProperty(PropertyName = "onlyChild")]
        public bool OnlyChild { get; set; }
        [JsonProperty(PropertyName = "contractId")]
        public int ContractId { get; set; }
        [JsonProperty(PropertyName = "latitude")]
        public string Latitude { get; set; }
        [JsonProperty(PropertyName = "longitude")]
        public string Longitude { get; set; }
        [JsonProperty(PropertyName = "startTime")]
        public string StartTime { get; set; }
        [JsonProperty(PropertyName = "meal")]
        public string Meal { get; set; }
        //[JsonProperty(PropertyName = "tourImages")]
        //public List<Tourimage> TourImages { get; set; }
        //[JsonProperty(PropertyName = "tourReview")]
        //public List<object> TourReview { get; set; }
        //[JsonProperty(PropertyName = "questions")]
        //public object Questions { get; set; }
    }

    //public class Tourimage
    //{
    //    [JsonProperty(PropertyName = "tourId")]
    //    public int TourId { get; set; }
    //    [JsonProperty(PropertyName = "imagePath")]
    //    public string ImagePath { get; set; }
    //    [JsonProperty(PropertyName = "imageCaptionName")]
    //    public string ImageCaptionName { get; set; }
    //    [JsonProperty(PropertyName = "isFrontImage")]
    //    public int IsFrontImage { get; set; }
    //    [JsonProperty(PropertyName = "isBannerImage")]
    //    public int IsBannerImage { get; set; }
    //    [JsonProperty(PropertyName = "isBannerRotateImage")]
    //    public int IsBannerRotateImage { get; set; }
    // }
}
