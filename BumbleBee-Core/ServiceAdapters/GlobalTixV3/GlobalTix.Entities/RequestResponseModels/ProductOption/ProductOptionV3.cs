using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.ProductOption
{
    public class ProductOption
    {
        [JsonProperty(PropertyName = "data")]
        public List<Datum> Datum { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool? Success { get; set; }
    }

    public class Datum
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "termsAndConditions")]
        public string TermsAndConditions { get; set; }
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "publishStart")]
        public DateTime PublishStart { get; set; }
        [JsonProperty(PropertyName = "publishEnd")]
        public DateTime? PublishEnd { get; set; }
        [JsonProperty(PropertyName = "redeemStart")]
        public DateTime? RedeemStart { get; set; }
        [JsonProperty(PropertyName = "redeemEnd")]
        public DateTime? RedeemEnd { get; set; }
        [JsonProperty(PropertyName = "ticketValidity")]
        public string TicketValidity { get; set; }
        [JsonProperty(PropertyName = "ticketFormat")]
        public string TicketFormat { get; set; }
        [JsonProperty(PropertyName = "definedDuration")]
        public int? DefinedDuration { get; set; }
        [JsonProperty(PropertyName = "isFavorited")]
        public bool? IsFavorited { get; set; }
        [JsonProperty(PropertyName = "fromReseller")]
        public object FromReseller { get; set; }
        [JsonProperty(PropertyName = "isCapacity")]
        public bool ?IsCapacity { get; set; }
        [JsonProperty(PropertyName = "timeSlot")]
        public List<string> TimeSlot { get; set; }
        [JsonProperty(PropertyName = "sourceName")]
        public string SourceName { get; set; }
        [JsonProperty(PropertyName = "sourceTitle")]
        public string SourceTitle { get; set; }
        [JsonProperty(PropertyName = "advanceBooking")]
        public Advancebooking AdvanceBooking { get; set; }
        [JsonProperty(PropertyName = "visitDate")]
        public Visitdate VisitDate { get; set; }
        [JsonProperty(PropertyName = "questions")]
        public List<Question> Questions { get; set; }
        [JsonProperty(PropertyName = "howToUse")]
        public List<string> HowToUse { get; set; }
        [JsonProperty(PropertyName = "inclusions")]
        public List<string> Inclusions { get; set; }
        [JsonProperty(PropertyName = "exclusions")]
        public List<object> Exclusions { get; set; }
        [JsonProperty(PropertyName = "isCancellable")]
        public bool? IsCancellable { get; set; }
        [JsonProperty(PropertyName = "cancellationPolicy")]
        public Cancellationpolicy CancellationPolicy { get; set; }
        [JsonProperty(PropertyName = "cancellationNotes")]
        public List<string> CancellationNotes { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "demandType")]
        public string DemandType { get; set; }
        [JsonProperty(PropertyName = "ticketType")]
        public List<Tickettype> TicketType { get; set; }
        [JsonProperty(PropertyName = "keywords")]
        public object Keywords { get; set; }
    }

    public class Advancebooking
    {
        [JsonProperty(PropertyName = "required")]
        public bool? Required { get; set; }
        [JsonProperty(PropertyName = "day")]
        public int? Day { get; set; }
        [JsonProperty(PropertyName = "hour")]
        public int? Hour { get; set; }
        [JsonProperty(PropertyName = "minute")]
        public int? Minute { get; set; }
    }

    public class Visitdate
    {
        [JsonProperty(PropertyName = "request")]
        public bool? Request { get; set; }
        [JsonProperty(PropertyName = "required")]
        public bool? Required { get; set; }
        [JsonProperty(PropertyName = "isOpenDated")]
        public bool? IsOpenDated { get; set; }
    }

    public class Cancellationpolicy
    {
        [JsonProperty(PropertyName = "percentReturn")]
        public float? PercentReturn { get; set; }
        [JsonProperty(PropertyName = "refundDuration")]
        public int? RefundDuration { get; set; }
    }

    public class Question
    {
        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }
        [JsonProperty(PropertyName = "options")]
        public List<string> Options { get; set; }
        [JsonProperty(PropertyName = "optional")]
        public bool? Optional { get; set; }
        [JsonProperty(PropertyName = "question")]
        public string QuestionData { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "questionCode")]
        public object QuestionCode { get; set; }
        [JsonProperty(PropertyName = "isAnswerLater")]
        public object IsAnswerLater { get; set; }
    }

    public class Tickettype
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "sku")]
        public string Sku { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "originalPrice")]
        public float OriginalPrice { get; set; }
        [JsonProperty(PropertyName = "originalMerchantPrice")]
        public float OriginalMerchantPrice { get; set; }
        [JsonProperty(PropertyName = "issuanceLimit")]
        public object IssuanceLimit { get; set; }
        [JsonProperty(PropertyName = "minPurchaseQty")]
        public object MinPurchaseQty { get; set; }
        [JsonProperty(PropertyName = "maxPurchaseQty")]
        public object MaxPurchaseQty { get; set; }
        [JsonProperty(PropertyName = "merchantReference")]
        public object MerchantReference { get; set; }
        [JsonProperty(PropertyName = "ageFrom")]
        public int? AgeFrom { get; set; }
        [JsonProperty(PropertyName = "ageTo")]
        public int? AgeTo { get; set; }
        [JsonProperty(PropertyName = "applyToAllQna")]
        public bool? ApplyToAllQna { get; set; }
        [JsonProperty(PropertyName = "nettPrice")]
        public float NettPrice { get; set; }
        [JsonProperty(PropertyName = "nettMerchantPrice")]
        public float NettMerchantPrice { get; set; }
        [JsonProperty(PropertyName = "minimumSellingPrice")]
        public float? MinimumSellingPrice { get; set; }
        [JsonProperty(PropertyName = "minimumMerchantSellingPrice")]
        public float? MinimumMerchantSellingPrice { get; set; }
        [JsonProperty(PropertyName = "recommendedSellingPrice")]
        public float RecommendedSellingPrice { get; set; }
        [JsonProperty(PropertyName = "useBin")]
        public bool? UseBin { get; set; }
    }

}
