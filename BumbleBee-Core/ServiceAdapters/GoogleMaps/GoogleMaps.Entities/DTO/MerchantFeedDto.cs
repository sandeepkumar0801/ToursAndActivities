using Isango.Entities.GoogleMaps;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO
{
    public class Merchants
    {
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("merchant")]
        public List<Merchant> Merchant { get; set; }
    }

    public class Merchant
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("telephone")]
        public string Telephone { get; set; }

        [JsonProperty("tax_rate")]
        public TaxRate TaxRate { get; set; }

        [JsonProperty("geo")]
        public Geo Geo { get; set; }

        [JsonProperty("tokenization_config")]
        public TokenizationConfig TokenizationConfig { get; set; }
    }

    public class Geo
    {
        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("unstructured_address")]
        public string UnstructuredAddress { get; set; }
    }

    public class Address
    {
        [JsonProperty("locality")]
        public string Locality { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("street_address")]
        public string StreetAddress { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }
    }

    public class TaxRate
    {
        [JsonProperty("micro_percent")]
        public long MicroPercent { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("processing_instruction")]
        public string ProcessingInstruction { get; set; }

        [JsonProperty("shard_number")]
        public string ShardNumber { get; set; }

        [JsonProperty("total_shards")]
        public int TotalShards { get; set; }

        [JsonProperty("generation_timestamp")]
        public string GenerationTimestamp { get; set; }
    }
}