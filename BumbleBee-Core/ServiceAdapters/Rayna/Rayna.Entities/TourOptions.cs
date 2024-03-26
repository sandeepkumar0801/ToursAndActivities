using Newtonsoft.Json;
using System.Collections.Generic;


namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class TourOptions
    {
        [JsonProperty(PropertyName = "statuscode")]
        public int Statuscode { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "result")]
        public ResultTourOptions ResultTourOptions { get; set; }
    }

    public class ResultTourOptions
    {
        [JsonProperty(PropertyName = "touroption")]
        public List<Touroption> Touroption { get; set; }
        //[JsonProperty(PropertyName = "operationdays")]
        //public List<Operationday> Operationdays { get; set; }
        //[JsonProperty(PropertyName = "specialdates")]
        //public List<object> Specialdates { get; set; }
        [JsonProperty(PropertyName = "transfertime")]
        public List<TransferTimeTourOption> TransferTimeTourOption { get; set; }
    }

    public class Touroption
    {
        [JsonProperty(PropertyName = "tourId")]
        public int TourId { get; set; }
        [JsonProperty(PropertyName = "tourOptionId")]
        public int TourOptionId { get; set; }
        [JsonProperty(PropertyName = "optionName")]
        public string OptionName { get; set; }
        [JsonProperty(PropertyName = "childAge")]
        public string ChildAge { get; set; }
        [JsonProperty(PropertyName = "infantAge")]
        public string InfantAge { get; set; }
        [JsonProperty(PropertyName = "optionDescription")]
        public string OptionDescription { get; set; }
        [JsonProperty(PropertyName = "cancellationPolicy")]
        public string CancellationPolicy { get; set; }
        [JsonProperty(PropertyName = "cancellationPolicyDescription")]
        public string CancellationPolicyDescription { get; set; }
        [JsonProperty(PropertyName = "childPolicyDescription")]
        public string ChildPolicyDescription { get; set; }
        [JsonProperty(PropertyName = "xmlcode")]
        public string Xmlcode { get; set; }
        [JsonProperty(PropertyName = "xmloptioncode")]
        public string Xmloptioncode { get; set; }
        [JsonProperty(PropertyName = "countryId")]
        public int CountryId { get; set; }
        [JsonProperty(PropertyName = "cityId")]
        public int CityId { get; set; }
        [JsonProperty(PropertyName = "minPax")]
        public int MinPax { get; set; }
        [JsonProperty(PropertyName = "maxPax")]
        public int MaxPax { get; set; }
        [JsonProperty(PropertyName = "duration")]
        public string Duration { get; set; }
        [JsonProperty(PropertyName = "timeZone")]
        public string TimeZone { get; set; }
        [JsonProperty(PropertyName = "isWithoutAdult")]
        public bool IsWithoutAdult { get; set; }
        [JsonProperty(PropertyName = "isTourGuide")]
        public int IsTourGuide { get; set; }
        [JsonProperty(PropertyName = "compulsoryOptions")]
        public bool CompulsoryOptions { get; set; }
        [JsonProperty(PropertyName = "hourlyBasisTypeTour")]
        public bool HourlyBasisTypeTour { get; set; }
        [JsonProperty(PropertyName = "isHideRateBreakup")]
        public bool IsHideRateBreakup { get; set; }
    }

    //public class Operationday
    //{
    //    [JsonProperty(PropertyName = "tourId")]
    //    public int TourId { get; set; }
    //    [JsonProperty(PropertyName = "tourOptionId")]
    //    public int TourOptionId { get; set; }
    //    [JsonProperty(PropertyName = "monday")]
    //    public int Monday { get; set; }
    //    [JsonProperty(PropertyName = "tuesday")]
    //    public int Tuesday { get; set; }
    //    [JsonProperty(PropertyName = "wednesday")]
    //    public int Wednesday { get; set; }
    //    [JsonProperty(PropertyName = "thursday")]
    //    public int Thursday { get; set; }
    //    [JsonProperty(PropertyName = "friday")]
    //    public int Friday { get; set; }
    //    [JsonProperty(PropertyName = "saturday")]
    //    public int Saturday { get; set; }
    //    [JsonProperty(PropertyName = "sunday")]
    //    public int Sunday { get; set; }
    //}

    public class TransferTimeTourOption
    {
        [JsonProperty(PropertyName = "tourId")]
        public int TourId { get; set; }
        [JsonProperty(PropertyName = "tourOptionId")]
        public int TourOptionId { get; set; }
        [JsonProperty(PropertyName = "transferType")]
        public string TransferType { get; set; }
        [JsonProperty(PropertyName = "transferTime")]
        public string TransferTime { get; set; }
        [JsonProperty(PropertyName = "duration")]
        public string Duration { get; set; }
        [JsonProperty(PropertyName = "mobileVoucher")]
        public bool MobileVoucher { get; set; }
        [JsonProperty(PropertyName = "printedVoucher")]
        public bool PrintedVoucher { get; set; }
        [JsonProperty(PropertyName = "instantConfirmation")]
        public bool InstantConfirmation { get; set; }
        [JsonProperty(PropertyName = "onRequest")]
        public bool OnRequest { get; set; }
        [JsonProperty(PropertyName = "requiedhrs")]
        public bool Requiedhrs { get; set; }
    }

}
