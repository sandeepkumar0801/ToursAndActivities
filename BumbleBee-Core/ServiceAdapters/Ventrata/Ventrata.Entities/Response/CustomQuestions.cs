using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Ventrata.Ventrata.Entities.Response
{

    public class CustomQuestions
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("internalName")]
        public string InternalName { get; set; }
        [JsonProperty("reference")]
        public string Reference { get; set; }
        [JsonProperty("locale")]
        public string Locale { get; set; }
        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }
        [JsonProperty("allowFreesale")]
        public bool AllowFreesale { get; set; }
        [JsonProperty("instantConfirmation")]
        public bool InstantConfirmation { get; set; }
        [JsonProperty("instantDelivery")]
        public bool InstantDelivery { get; set; }
        [JsonProperty("availabilityRequired")]
        public bool AvailabilityRequired { get; set; }
        [JsonProperty("availabilityType")]
        public string AvailabilityType { get; set; }
        [JsonProperty("deliveryFormats")]
        public List<string> DeliveryFormats { get; set; }
        [JsonProperty("deliveryMethods")]
        public List<string> DeliveryMethods { get; set; }
        [JsonProperty("settlementMethods")]
        public List<string> SettlementMethods { get; set; }
        [JsonProperty("redemptionMethod")]
        public string RedemptionMethod { get; set; }
        [JsonProperty("options")]
        public List<CustomQuestionsOption> Options { get; set; }
        [JsonProperty("questions")]
        public List<object> Questions { get; set; }
    }

    public class CustomQuestionsOption
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("_default")]
        public bool _Default { get; set; }
        [JsonProperty("internalName")]
        public string InternalName { get; set; }
        [JsonProperty("reference")]
        public string Reference { get; set; }
        [JsonProperty("availabilityLocalStartTimes")]
        public List<string> AvailabilityLocalStartTimes { get; set; }
        [JsonProperty("cancellationCutoff")]
        public string CancellationCutoff { get; set; }
        [JsonProperty("cancellationCutoffAmount")]
        public int CancellationCutoffAmount { get; set; }
        [JsonProperty("cancellationCutoffUnit")]
        public string CancellationCutoffUnit { get; set; }
        [JsonProperty("visibleContactFields")]
        public List<object> VisibleContactFields { get; set; }
        [JsonProperty("requiredContactFields")]
        public List<object> RequiredContactFields { get; set; }
        [JsonProperty("restrictions")]
        public CustomQuestionsRestrictions Restrictions { get; set; }
        [JsonProperty("units")]
        public List<CustomQuestionsUnit> Units { get; set; }
    }

    public class CustomQuestionsRestrictions
    {
        [JsonProperty("minUnits")]
        public int MinUnits { get; set; }
        [JsonProperty("maxUnits")]
        public object MaxUnits { get; set; }
        [JsonProperty("minPaxCount")]
        public int MinPaxCount { get; set; }
        [JsonProperty("maxPaxCount")]
        public object MaxPaxCount { get; set; }
    }

    public class CustomQuestionsUnit
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("internalName")]
        public string InternalName { get; set; }
        [JsonProperty("reference")]
        public object Reference { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("visibleContactFields")]
        public List<object> VisibleContactFields { get; set; }
        [JsonProperty("requiredContactFields")]
        public List<object> RequiredContactFields { get; set; }
        [JsonProperty("restrictions")]
        public CustomQuestionsRestrictions1 Restrictions { get; set; }
        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }
    }

    public class CustomQuestionsRestrictions1
    {
        [JsonProperty("minAge")]
        public int MinAge { get; set; }
        [JsonProperty("maxAge")]
        public int MaxAge { get; set; }
        [JsonProperty("idRequired")]
        public bool IdRequired { get; set; }
        [JsonProperty("minQuantity")]
        public int MinQuantity { get; set; }
        [JsonProperty("maxQuantity")]
        public object MaxQuantity { get; set; }
        [JsonProperty("paxCount")]
        public int PaxCount { get; set; }
        [JsonProperty("accompaniedBy")]
        public List<string> AccompaniedBy { get; set; }
    }

    public class Question
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("required")]
        public bool Required { get; set; }
        [JsonProperty("inputType")]
        public string InputType { get; set; }
        [JsonProperty("selectOptions")]
        public List<object> SelectOptions { get; set; }
    }

}








