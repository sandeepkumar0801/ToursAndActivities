using Isango.Entities.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Bokun.Bokun.Entities.CheckAvailabilities
{
    public class CheckAvailabilitiesRs : EntityBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("activityId")]
        public int? ActivityId { get; set; }

        [JsonProperty("activityOwnerId")]
        public int? ActivityOwnerId { get; set; }

        [JsonProperty("availabilityCount")]
        public int? AvailabilityCount { get; set; }

        [JsonProperty("bookedParticipants")]
        public int? BookedParticipants { get; set; }

        [JsonProperty("minParticipants")]
        public int? MinParticipants { get; set; }

        [JsonProperty("minParticipantsToBookNow")]
        public int? MinParticipantsToBookNow { get; set; }

        [JsonProperty("comboActivity")]
        public bool ComboActivity { get; set; }

        [JsonProperty("pickupAllotment")]
        public bool PickupAllotment { get; set; }

        [JsonProperty("pickupAvailabilityCount")]
        public int PickupAvailabilityCount { get; set; }

        [JsonProperty("comboStartTimes")]
        public Combostarttime[] ComboStartTimes { get; set; }

        [JsonProperty("date")]
        public double Date { get; set; }

        [JsonProperty("defaultPrice")]
        public decimal? DefaultPrice { get; set; }

        [JsonProperty("dropoffPrice")]
        public decimal? DropoffPrice { get; set; }

        [JsonProperty("dropoffPricesByCategory")]
        public object DropoffPricesByCategory { get; set; }

        [JsonProperty("extraPrices")]
        public Amounts ExtraPrices { get; set; }

        [JsonProperty("flags")]
        public string[] Flags { get; set; }

        [JsonProperty("flexible")]
        public bool Flexible { get; set; }

        [JsonProperty("localizedDate")]
        public string LocalizedDate { get; set; }

        [JsonProperty("pickupPrice")]
        public decimal? PickupPrice { get; set; }

        [JsonProperty("pickupPricesByCategory")]
        public object PickupPricesByCategory { get; set; }

        [JsonProperty("pricesByCategory")]
        public object PricesByCategory { get; set; }

        [JsonProperty("soldOut")]
        public bool SoldOut { get; set; }

        [JsonProperty("startTime")]
        public string StartTime { get; set; }

        [JsonProperty("startTimeId")]
        public int? StartTimeId { get; set; }

        [JsonProperty("unavailable")]
        public bool Unavailable { get; set; }

        [JsonProperty("unlimitedAvailability")]
        public bool UnlimitedAvailability { get; set; }

        [JsonProperty("guidedLanguages")]
        public string[] GuidedLanguages { get; set; }

        [JsonProperty("defaultRateId")]
        public int? DefaultRateId { get; set; }

        [JsonProperty("rates")]
        public Rate[] Rates { get; set; }

        [JsonProperty("pricesByRate")]
        public Pricesbyrate[] PricesByRate { get; set; }

        [JsonIgnore]
        public Dictionary<PassengerType, int> PaxAgeGroupIds { get; set; }

        #region Adult

        [JsonIgnore]
        public bool IsAdultAllowed { get; set; }

        [JsonIgnore]
        public int MinAdultAge { get; set; }

        [JsonIgnore]
        public int MaxAdultAge { get; set; }

        #endregion Adult

        #region Child

        [JsonIgnore]
        public int MinChildAge { get; set; }

        [JsonIgnore]
        public int MaxChildAge { get; set; }

        [JsonIgnore]
        public bool IsChildAllowed { get; set; }

        #endregion Child

        #region Concession

        [JsonIgnore]
        public bool IsConcessionAllowed { get; set; }

        [JsonIgnore]
        public int MinConcessionAge { get; set; }

        [JsonIgnore]
        public int MaxConcessionAge { get; set; }

        #endregion Concession

        #region Family

        [JsonIgnore]
        public bool IsFamilyAllowed { get; set; }

        [JsonIgnore]
        public int MaxFamilyAge { get; set; }

        [JsonIgnore]
        public int MinFamilyAge { get; set; }

        #endregion Family

        #region Student

        [JsonIgnore]
        public bool IsStudentAllowed { get; set; }

        [JsonIgnore]
        public int MinStudentAge { get; set; }

        [JsonIgnore]
        public int MaxStudentAge { get; set; }

        #endregion Student

        #region Youth

        [JsonIgnore]
        public bool IsYouthAllowed { get; set; }

        public int MinYouthAge { get; set; }

        [JsonIgnore]
        public int MaxYouthAge { get; set; }

        #endregion Youth

        #region Infant

        public bool IsInfantAllowed { get; set; }

        public int MinInfantAge { get; set; }

        [JsonIgnore]
        public int MaxInfantAge { get; set; }

        #endregion Infant

        #region Senior

        [JsonIgnore]
        public bool IsSeniorAllowed { get; set; }

        [JsonIgnore]
        public int MinSeniorAge { get; set; }

        [JsonIgnore]
        public int MaxSeniorAge { get; set; }

        #endregion Senior
    }

    public class Combostarttime
    {
        public int? Id { get; set; }
        public int? TargetActivityId { get; set; }
        public int? TargetStartTimeId { get; set; }
    }

    public class Rate
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("index")]
        public int? Index { get; set; }

        [JsonProperty("rateCode")]
        public string RateCode { get; set; }

        [JsonProperty("pricedPerPerson")]
        public bool PricedPerPerson { get; set; }

        [JsonProperty("tieredPricingEnabled")]
        public bool TieredPricingEnabled { get; set; }

        [JsonProperty("minPerBooking")]
        public int? MinPerBooking { get; set; }

        [JsonProperty("maxPerBooking")]
        public int? MaxPerBooking { get; set; }

        [JsonProperty("cancellationPolicy")]
        public Cancellationpolicy CancellationPolicy { get; set; }

        [JsonProperty("pickupSelectionType")]
        public string PickupSelectionType { get; set; }

        [JsonProperty("pickupPricingType")]
        public string PickupPricingType { get; set; }

        [JsonProperty("pickupPricedPerPerson")]
        public bool PickupPricedPerPerson { get; set; }

        [JsonProperty("dropoffSelectionType")]
        public string DropoffSelectionType { get; set; }

        [JsonProperty("dropoffPricingType")]
        public string DropoffPricingType { get; set; }

        [JsonProperty("dropoffPricedPerPerson")]
        public bool DropoffPricedPerPerson { get; set; }

        [JsonProperty("startTimeIds")]
        public int?[] StartTimeIds { get; set; }

        [JsonProperty("pricingCategoryIds")]
        public int?[] PricingCategoryIds { get; set; }

        [JsonProperty("extraConfigs")]
        public Extraconfig[] ExtraConfigs { get; set; }
    }

    public class Cancellationpolicy
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("penaltyRules")]
        public Penaltyrule[] PenaltyRules { get; set; }

        [JsonProperty("tax")]
        public Tax Tax { get; set; }

        [JsonProperty("policyType")]
        public string PolicyType { get; set; }
    }

    public class Tax
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("included")]
        public bool Included { get; set; }

        [JsonProperty("percentage")]
        public float? Percentage { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public class Penaltyrule
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("cutoffHours")]
        public int? CutoffHours { get; set; }

        [JsonProperty("charge")]
        public float Charge { get; set; }

        [JsonProperty("chargeType")]
        public string ChargeType { get; set; }

        [JsonProperty("percentage")]
        public float Percentage { get; set; }
    }

    public class Extraconfig
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("activityExtraId")]
        public int? ActivityExtraId { get; set; }

        [JsonProperty("selectionType")]
        public string SelectionType { get; set; }

        [JsonProperty("pricingType")]
        public string PricingType { get; set; }
    }

    public class Pricesbyrate
    {
        [JsonProperty("activityRateId")]
        public int? ActivityRateId { get; set; }

        [JsonProperty("pricePerBooking")]
        public Amounts PricePerBooking { get; set; }

        [JsonProperty("pricePerCategoryUnit")]
        public Pricepercategoryunit[] PricePerCategoryUnit { get; set; }

        [JsonProperty("pickupPrice")]
        public Amounts PickupPrice { get; set; }

        [JsonProperty("pickupPricePerCategoryUnit")]
        public Pricepercategoryunit[] PickupPricePerCategoryUnit { get; set; }

        [JsonProperty("dropoffPrice")]
        public Amounts DropoffPrice { get; set; }

        [JsonProperty("dropoffPricePerCategoryUnit")]
        public Pricepercategoryunit[] DropoffPricePerCategoryUnit { get; set; }

        [JsonProperty("extraPricePerUnit")]
        public Pricepercategoryunit[] ExtraPricePerUnit { get; set; }

        [JsonProperty("extraPricePerCategoryUnit")]
        public Pricepercategoryunit[] ExtraPricePerCategoryUnit { get; set; }
    }

    public class Pricepercategoryunit
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("amount")]
        public Amounts Amount { get; set; }

        [JsonProperty("prices")]
        public object[] Prices { get; set; }

        [JsonProperty("minParticipantsRequired")]
        public int? MinParticipantsRequired { get; set; }

        [JsonProperty("maxParticipantsRequired")]
        public int? MaxParticipantsRequired { get; set; }
    }

    public class Amounts
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("amount")]
        public decimal? Amount { get; set; }

        [JsonProperty("prices")]
        public object[] Prices { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}