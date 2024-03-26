using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.HB.HB.Entities.ContentMulti
{
    public class ContentMultiRq
    {
        [JsonProperty("codes")]
        public List<Code> Codes { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
    }

    public class Code
    {
        [JsonProperty("activityCode")]
        public string ActivityCode { get; set; }
    }
}