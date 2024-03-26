using Isango.Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Options;

namespace Isango.Entities.Activities
{
    public class ActivityLite : Product
    {
        public bool OnSale { get; set; }
        public string DurationString { get; set; }
        public string ScheduleOperates { get; set; }
        public string ShortName { get; set; }
        public int Priority { get; set; }
        public ActivityType ActivityType { get; set; }
        public string ActualServiceUrl { get; set; }
        public string CoOrdinates { get; set; }
        public double OverAllRating { get; set; }
        public List<int> CategoryIDs { get; set; }
        public List<string> ReasonToBook { get; set; }
        public int FactsheetId { get; set; }
        public DayBadge DayBadge { get; set; }
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, int> PriorityWiseCategory { get; set; }
        public int TotalReviewCount { get; set; }
        public string Code { get; set; }
    }
}