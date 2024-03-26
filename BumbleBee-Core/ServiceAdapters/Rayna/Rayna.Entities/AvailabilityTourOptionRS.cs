using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Rayna.Rayna.Entities
{
    public class AvailabilityTourOptionRS
    {
        [JsonProperty(PropertyName = "statuscode")]
        public int StatusCode { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "currencysymbol")]
        public string Currencysymbol { get; set; }
        [JsonProperty(PropertyName = "errormessage")]
        public string Errormessage { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "result")]
        public List<AvailabilityOptionResult> AvailabilityOptionResult { get; set; }
    }

    public class AvailabilityOptionResult
    {
        [JsonProperty(PropertyName = "tourId")]
        public int TourId { get; set; }
        [JsonProperty(PropertyName = "tourOptionId")]
        public int TourOptionId { get; set; }
        [JsonProperty(PropertyName = "transferId")]
        public int TransferId { get; set; }
        [JsonProperty(PropertyName = "transferName")]
        public string TransferName { get; set; }
        [JsonProperty(PropertyName = "adultPrice")]
        public decimal AdultPrice { get; set; }
        [JsonProperty(PropertyName = "childPrice")]
        public decimal ChildPrice { get; set; }
        [JsonProperty(PropertyName = "infantPrice")]
        public decimal InfantPrice { get; set; }
        [JsonProperty(PropertyName = "withoutDiscountAmount")]
        public decimal WithoutDiscountAmount { get; set; }
        [JsonProperty(PropertyName = "finalAmount")]
        public decimal FinalAmount { get; set; }
        [JsonProperty(PropertyName = "startTime")]
        public string StartTime { get; set; }
        [JsonProperty(PropertyName = "departureTime")]
        public string DepartureTime { get; set; }
        [JsonProperty(PropertyName = "disableChild")]
        public bool DisableChild { get; set; }
        [JsonProperty(PropertyName = "disableInfant")]
        public bool DisableInfant { get; set; }
        [JsonProperty(PropertyName = "allowTodaysBooking")]
        public bool AllowTodaysBooking { get; set; }
        [JsonProperty(PropertyName = "cutOff")]
        public int CutOff { get; set; }
        [JsonProperty(PropertyName = "isSlot")]
        public bool IsSlot { get; set; }
        [JsonProperty(PropertyName = "isDefaultTransfer")]
        public int IsDefaultTransfer { get; set; }
        [JsonProperty(PropertyName = "rateKey")]
        public object RateKey { get; set; }
        [JsonProperty(PropertyName = "inventoryId")]

        public object InventoryId { get; set; }

        //[JsonProperty(PropertyName = "adultBuyingPrice")]
        //public float AdultBuyingPrice { get; set; }
        //[JsonProperty(PropertyName = "childBuyingPrice")]
        //public float ChildBuyingPrice { get; set; }
        //[JsonProperty(PropertyName = "infantBuyingPrice")]
        //public float InfantBuyingPrice { get; set; }
        //[JsonProperty(PropertyName = "adultSellingPrice")]
        //public float AdultSellingPrice { get; set; }
        //[JsonProperty(PropertyName = "childSellingPrice")]
        //public float ChildSellingPrice { get; set; }
        //[JsonProperty(PropertyName = "infantSellingPrice")]
        //public float InfantSellingPrice { get; set; }
        //[JsonProperty(PropertyName = "companyBuyingPrice")]
        //public float CompanyBuyingPrice { get; set; }
        //[JsonProperty(PropertyName = "companySellingPrice")]
        //public float CompanySellingPrice { get; set; }
        //[JsonProperty(PropertyName = "agentBuyingPrice")]
        //public float AgentBuyingPrice { get; set; }
        //[JsonProperty(PropertyName = "agentSellingPrice")]
        //public float AgentSellingPrice { get; set; }
        //[JsonProperty(PropertyName = "subAgentBuyingPrice")]
        //public float SubAgentBuyingPrice { get; set; }
        //[JsonProperty(PropertyName = "subAgentSellingPrice")]
        //public float SubAgentSellingPrice { get; set; }
        //[JsonProperty(PropertyName = "finalSellingPrice")]
        //public float FinalSellingPrice { get; set; }
        //[JsonProperty(PropertyName = "vatbuying")]
        //public float Vatbuying { get; set; }
        //[JsonProperty(PropertyName = "vatselling")]
        //public float Vatselling { get; set; }
        //[JsonProperty(PropertyName = "currencyFactor")]
        //public float CurrencyFactor { get; set; }
        //[JsonProperty(PropertyName = "agentPercentage")]
        //public float AgentPercentage { get; set; }
        //[JsonProperty(PropertyName = "transferBuyingPrice")]
        //public float TransferBuyingPrice { get; set; }
        //[JsonProperty(PropertyName = "transferSellingPrice")]
        //public float TransferSellingPrice { get; set; }
        //[JsonProperty(PropertyName = "serviceBuyingPrice")]
        //public float ServiceBuyingPrice { get; set; }
        //[JsonProperty(PropertyName = "serviceSellingPrice")]
        //public float ServiceSellingPrice { get; set; }
        //[JsonProperty(PropertyName = "rewardPoints")]
        //public int RewardPoints { get; set; }
        //[JsonProperty(PropertyName = "tourChildAge")]
        //public int TourChildAge { get; set; }
        //[JsonProperty(PropertyName = "maxChildAge")]
        //public int MaxChildAge { get; set; }
        //[JsonProperty(PropertyName = "maxInfantAge")]
        //public int MaxInfantAge { get; set; }
        //[JsonProperty(PropertyName = "minimumPax")]
        //public int MinimumPax { get; set; }
    }

}
