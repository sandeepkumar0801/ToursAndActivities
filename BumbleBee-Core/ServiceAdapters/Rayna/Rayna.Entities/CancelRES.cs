using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Rayna.Rayna.Entities
{

    public class CancelRES
    {
        [JsonProperty(PropertyName = "statuscode")]
        public int StatusCode { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "result")]
        public ResultCancel ResultCancel { get; set; }
    }

    public class ResultCancel
    {
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
