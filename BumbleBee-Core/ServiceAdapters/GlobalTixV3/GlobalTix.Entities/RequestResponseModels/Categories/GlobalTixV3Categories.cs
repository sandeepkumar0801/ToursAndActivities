using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.Categories
{
    public class GlobalTixV3Categories
    {
        [JsonProperty(PropertyName = "data")]
        public Datum[] Data { get; set; }

        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }

        [JsonProperty(PropertyName = "size")]
        public object Size { get; set; }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }


        public class Datum
        {
            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
        }
    }
}
