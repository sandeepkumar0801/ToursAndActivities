using Isango.Entities.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Isango.Entities.Affiliate
{
    public class AffiliateFilter
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "affiliateId")]
        public string AffiliateId { get; set; }

        /// <summary>
        /// Negation/Exclusion for Service filter
        /// </summary>
        public List<int> Regions { get; set; }

        public List<ActivityType> DurationTypes { get; set; }

        public List<int> Activities { get; set; }

        public List<KeyValuePair<int, int>> AffiliateServicesPriority { get; set; }

        public decimal AffiliateMargin { get; set; }

        public bool ActivityFilter { get; set; }

        public bool RegionFilter { get; set; }

        public bool DurationTypeFilter { get; set; }

        public bool AffiliateActivityPriorityFilter { get; set; }

        public bool IsServiceExclusionFilter { get; set; }

        public bool IsMarginFilter { get; set; }
    }
}