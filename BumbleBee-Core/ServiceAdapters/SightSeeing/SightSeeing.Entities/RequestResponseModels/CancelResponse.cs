using Newtonsoft.Json;

namespace ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels
{
    internal class CancelResponseObj
    {
        [JsonProperty("obj")]
        public Obj Obj { get; set; }

        [JsonProperty("status_code")]
        public string StatusCode { get; set; }

        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }

        [JsonProperty("cbObject")]
        public string CbObject { get; set; }
    }

    internal class Obj
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("ErrorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}