using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class ActivityInfo : Identifier
    {
        [JsonProperty(PropertyName = "country")]
        public IdentifierWithName Country { get; set; }
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "city")]
        public IdentifierWithName City { get; set; }
        [JsonProperty(PropertyName = "imagePath")]
        public string ImagePath { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Desc { get; set; }
        [JsonProperty(PropertyName = "hoursOfOperation")]
        public string OpHours { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "ticketTypeGroups")]
        public List<TicketTypeGroup> TicketTypeGroups;
        [JsonProperty(PropertyName = "series")]
        public List<SeriesOuter> Series;

        [JsonProperty(PropertyName = "types")]
        public List<IdentifierWithName> Types;

        [JsonProperty(PropertyName = "latitude")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public string Longitude { get; set; }
    }
}

public class SeriesOuter
{
    [JsonProperty(PropertyName = "attraction")]
    public int Attraction { get; set; }
    [JsonProperty(PropertyName = "series")]
    public List<SeriesInner> Series { get; set; }
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    [JsonProperty(PropertyName = "start")]
    public DateTime Start { get; set; }
    [JsonProperty(PropertyName = "isRollingDate")]
    public bool IsRollingDate { get; set; }
    [JsonProperty(PropertyName = "end")]
    public DateTime End { get; set; }
    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }
    [JsonProperty(PropertyName = "lastEventDateCreated")]
    public DateTime LastEventDateCreated { get; set; }
    [JsonProperty(PropertyName = "seriesReminder")]
    public object SeriesReminder { get; set; }
}

public class SeriesInner
{
    [JsonProperty(PropertyName = "satCapacity")]
    public int? SatCapacity { get; set; }
    [JsonProperty(PropertyName = "sunCapacity")]
    public int? SunCapacity { get; set; }
    [JsonProperty(PropertyName = "wedCapacity")]
    public int? WedCapacity { get; set; }
    [JsonProperty(PropertyName = "friCapacity")]
    public int? FriCapacity { get; set; }
    [JsonProperty(PropertyName = "isInactive")]
    public bool IsInactive { get; set; }
    [JsonProperty(PropertyName = "start")]
    public DateTime Start { get; set; }
    [JsonProperty(PropertyName = "monCapacity")]
    public int? MonCapacity { get; set; }
    [JsonProperty(PropertyName = "daysOfWeek")]
    public List<object> DaysOfWeek { get; set; }
    [JsonProperty(PropertyName = "capacity")]
    public object Capacity { get; set; }
    [JsonProperty(PropertyName = "totalEventsToday")]
    public int TotalEventsToday { get; set; }
    [JsonProperty(PropertyName = "thuCapacity")]
    public int? ThuCapacity { get; set; }
    [JsonProperty(PropertyName = "name")]
    public object Name { get; set; }
    [JsonProperty(PropertyName = "end")]
    public DateTime End { get; set; }
    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }
    [JsonProperty(PropertyName = "enableEmp")]
    public bool EnableEmp { get; set; }
    [JsonProperty(PropertyName = "events")]
    public List<EventInner> Events { get; set; }
    [JsonProperty(PropertyName = "tueCapacity")]
    public int? TueCapacity { get; set; }
}

public class EventInner
{
    [JsonProperty(PropertyName = "isAdHoc")]
    public bool IsAdHoc { get; set; }
    [JsonProperty(PropertyName = "total")]
    public int? Total { get; set; }
    [JsonProperty(PropertyName = "seriesName")]
    public object SeriesName { get; set; }
    [JsonProperty(PropertyName = "unlimited")]
    public object Unlimited { get; set; }
    [JsonProperty(PropertyName = "available")]
    public int? Available { get; set; }
    [JsonProperty(PropertyName = "id")]
    public int? Id { get; set; }
    [JsonProperty(PropertyName = "time")]
    public DateTime Time { get; set; }
    [JsonProperty(PropertyName = "used")]
    public int? Used { get; set; }
    [JsonProperty(PropertyName = "enableEmp")]
    public bool EnableEmp { get; set; }
    [JsonProperty(PropertyName = "seriesId")]
    public int? SeriesId { get; set; }
}

