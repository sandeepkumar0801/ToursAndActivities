using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels.Master
{
    public class ActivityMasterResponse
    {
        [JsonProperty(PropertyName = "ID")]
        public Int32 ID { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "PassengerType")]
        public List<PassengerTypeMasterResponse> PassengerTypeMasterResponse { get; set; }
    }
}