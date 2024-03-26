using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.PackageOptions
{
    public class PackageOptionsList
    {

        [JsonProperty(PropertyName = "data")]
        public List<Datum> data { get; set; }

        [JsonProperty(PropertyName = "error")]
        public object error { get; set; }

        [JsonProperty(PropertyName = "size")]
        public int size { get; set; }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
        
        public class Datum
        {
            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "image")]
            public string Image { get; set; }

            [JsonProperty(PropertyName = "description")]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "termsAndConditions")]
            public string TermsAndConditions { get; set; }

            [JsonProperty(PropertyName = "currency")]
            public string Currency { get; set; }

            [JsonProperty(PropertyName = "publishStart")]
            public DateTime PublishStart { get; set; }

            [JsonProperty(PropertyName = "publishEnd")]
            public object PublishEnd { get; set; }

            [JsonProperty(PropertyName = "redeemStart")]
            public object RedeemStart { get; set; }

            [JsonProperty(PropertyName = "redeemEnd")]
            public object RedeemEnd { get; set; }

            [JsonProperty(PropertyName = "ticketValidity")]
            public string TicketValidity { get; set; }

            [JsonProperty(PropertyName = "ticketFormat")]
            public string TicketFormat { get; set; }

            [JsonProperty(PropertyName = "definedDuration")]
            public int DefinedDuration { get; set; }

            [JsonProperty(PropertyName = "isFavorited")]
            public bool IsFavorited { get; set; }

            [JsonProperty(PropertyName = "fromReseller")]
            public object FromReseller { get; set; }

            [JsonProperty(PropertyName = "sourceName")]
            public string SourceName { get; set; }

            [JsonProperty(PropertyName = "sourceTitle")]
            public string SourceTitle { get; set; }

            [JsonProperty(PropertyName = "isAdditionalBookingInfo")]
            public bool IsAdditionalBookingInfo { get; set; }

            [JsonProperty(PropertyName = "packageType")]
            public List<PackageType> PackageType { get; set; }

            //[JsonProperty(PropertyName = "inclusions")]
            //public List<Inclusion> Inclusions { get; set; }

            [JsonProperty(PropertyName = "keywords")]
            public object Keywords { get; set; }
        }

        //public class Inclusion
        //{
        //    [JsonProperty(PropertyName = "id")]
        //    public int InclusionId { get; set; }

        //    [JsonProperty(PropertyName = "name")]
        //    public string InclusionName { get; set; }

        //    [JsonProperty(PropertyName = "product")]
        //    public string InclusionProduct { get; set; }

        //    [JsonProperty(PropertyName = "attractionId")]
        //    public int InclusionAttractionId { get; set; }
        //}

        public class PackageType
        {
            [JsonProperty(PropertyName = "id")]
            public int PackageTypeId { get; set; }

            [JsonProperty(PropertyName = "sku")]
            public string PackageTypeSku { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string PackageTypeName { get; set; }

            [JsonProperty(PropertyName = "nettPrice")]
            public double PackageTypeNettPrice { get; set; }

            [JsonProperty(PropertyName = "settlementRate")]
            public double PackageTypeSettlementRate { get; set; }

            [JsonProperty(PropertyName = "originalPrice")]
            public double PackageTypeOriginalPrice { get; set; }

            [JsonProperty(PropertyName = "issuanceLimit")]
            public object PackageTypeIssuanceLimit { get; set; }
        }

        


    }
}
