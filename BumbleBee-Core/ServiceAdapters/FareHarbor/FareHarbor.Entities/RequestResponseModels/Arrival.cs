using Newtonsoft.Json;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class Arrival
    {
        [JsonProperty("Display_Text")]
        public string DisplayText { get; set; }

        public string Notes { get; set; }

        [JsonProperty("Notes_Safe_Html")]
        public string NotesSafeHtml { get; set; }

        public string Time { get; set; }
    }
}