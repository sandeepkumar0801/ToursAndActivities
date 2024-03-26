using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace Isango.Entities.Availability
{
    public class Availability
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public int ServiceId { get; set; }
        public int RegionId { get; set; }
        public int ServiceOptionId { get; set; }
        [BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<DateTime, decimal> BaseDateWisePriceAndAvailability { get; set; }
        [BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<DateTime, decimal> CostDateWisePriceAndAvailability { get; set; }
        public string Currency { get; set; }
    }
}