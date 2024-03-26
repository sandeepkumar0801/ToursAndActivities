using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class CancelByTicketRS
    {
        [JsonProperty(PropertyName = "success")]
        public bool IsSuccess { get; set; }
        [JsonProperty(PropertyName = "error")]
        public Error Error{ get; set; }
        [JsonProperty(PropertyName = "size")]
        public object size { get; set; }
        [JsonProperty(PropertyName ="data")]
        public object Data { get; set; }
    }

    //public class ErrorData
    //{
    //    public string code { get; set; }
    //    public object errorDetails { get; set; }
    //    public string message { get; set; }
    //}
}
