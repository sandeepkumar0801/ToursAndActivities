using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels.Master
{
    public class RegionWiseMasterResponse
    {
        [JsonProperty(PropertyName = "RegionID")]
        public Int32 RegionID { get; set; }

        [JsonProperty(PropertyName = "RegionName")]
        public string RegionName { get; set; }

        [JsonProperty(PropertyName = "Activities")]
        public List<ActivityMasterResponse> ActivityMasterResponse { get; set; }
    }
}