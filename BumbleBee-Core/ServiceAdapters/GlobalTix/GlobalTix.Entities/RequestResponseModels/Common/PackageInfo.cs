using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class PackageInfo : IdentifierWithName
    {
        [JsonProperty(PropertyName = "imagePath")]
        public string ImagePath { get; set; }
        [JsonProperty(PropertyName = "questions")]
        public List<Question> Questions { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Desc { get; set; }
        [JsonProperty(PropertyName = "settlementRate")]
        public decimal? SettlementRate { get; set; }
        [JsonProperty(PropertyName = "variants")]
        public List<Variant> Variants { get; set; }
		[JsonProperty(PropertyName = "variation")]
		public EnumValue Variation { get; set; }
		[JsonProperty(PropertyName = "payableAmount")]
        public decimal? PayableAmount { get; set; }
        [JsonProperty(PropertyName = "termsAndConditions")]
        public string TermsAndConditions { get; set; }
        [JsonProperty(PropertyName = "publishStart")]
        public DateTime PublishStart { get; set; }
        [JsonProperty(PropertyName = "publishEnd")]
        public DateTime? PublishEnd { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string CurrencyCode { get; set; }
        [JsonProperty(PropertyName = "linkId")]
        public string linkId { get; set; }
    }

    public class Variant
    {
        [JsonProperty(PropertyName = "variantName")]
        public string Name { get; set; }
		[JsonProperty(PropertyName = "packageId")]
		public int PackageId { get; set; }
		[JsonProperty(PropertyName = "originalPrice")]
        public decimal? OriginalPrice { get; set; }
    }
}
