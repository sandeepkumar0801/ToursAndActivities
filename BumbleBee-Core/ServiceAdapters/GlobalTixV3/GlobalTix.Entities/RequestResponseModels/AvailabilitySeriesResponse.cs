using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels
{


    public class AvailabilitySeriesResponse
    {
        [JsonProperty(PropertyName = "data")]
        public List<DatumAvailability> DataAvailability { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
    }

    public class DatumAvailability
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "available")]
        public int Available { get; set; }
        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
        [JsonProperty(PropertyName = "used")]
        public int Used { get; set; }
        [JsonProperty(PropertyName = "seriesId")]
        public int SeriesId { get; set; }
        [JsonProperty(PropertyName = "seriesName")]
        public object SeriesName { get; set; }
        [JsonProperty(PropertyName = "unlimited")]
        public object Unlimited { get; set; }
        [JsonProperty(PropertyName = "isAdHoc")]
        public bool IsAdHoc { get; set; }
        [JsonProperty(PropertyName = "isInactive")]
        public bool IsInactive { get; set; }
        [JsonProperty(PropertyName = "enableEmp")]
        public bool EnableEmp { get; set; }
        public int ticketTypeID { get; set; }
       
    }

}



