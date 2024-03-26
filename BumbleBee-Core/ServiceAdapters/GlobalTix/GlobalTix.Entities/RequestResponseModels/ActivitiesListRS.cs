using Newtonsoft.Json;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;


namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class ActivitiesListRS
    {
        [JsonProperty(PropertyName="data")]
        public List<ActivityInfoData> ListData { get; set; }
        [JsonProperty(PropertyName = "size")]
        public int TotalActivities { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool IsSuccess { get; set; }
    }

    public class ActivityInfoData : ActivityInfo
    {
        [JsonProperty(PropertyName = "ticketTypes")]
        public List<TicketType> TicketTypes;
    }

}