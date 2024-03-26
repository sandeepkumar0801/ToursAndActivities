using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels
{
    public class ConfirmRequest
    {
        public List<string> PnrList { get; set; }

        [JsonProperty("API-KEY")]
        public string ApiKey { get; set; }
    }
}