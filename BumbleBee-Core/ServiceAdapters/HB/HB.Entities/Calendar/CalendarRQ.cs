using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.HB.HB.Entities.Calendar
{
    public class CalendarRq
    {
        [JsonProperty("filters")]
        public List<Filter> Filters { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("paxes")]
        public List<Pax> Paxes { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
    }

    public class Filter
    {
        [JsonProperty("searchFilterItems")]
        public List<Searchfilteritem> SearchFilterItems { get; set; }
    }

    public class Searchfilteritem
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Pax
    {
        [JsonProperty("age")]
        public int Age { get; set; }
    }
}