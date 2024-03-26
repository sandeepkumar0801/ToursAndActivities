using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class TicketType : IdentifierWithName
    {
        [JsonProperty(PropertyName = "currency")]
        public string CurrencyCode { get; set; }
        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }
        [JsonProperty(PropertyName = "variation")]
        public EnumValue Variation { get; set; }
        [JsonProperty(PropertyName = "linkId")]
        public int LinkId { get; set; }
        //Property addition for minimum selling price task - IS-11485 - Sellprice for GlobalTix
        //[JsonProperty(PropertyName = "originalPrice")]
        //public decimal OriginalPrice { get; set; }
        //[JsonProperty(PropertyName = "merchantCurrency")]
        //public string OriginalPriceCurrency { get; set; }


        //[JsonProperty(PropertyName = "settlementPrice")]
        //public decimal SettlementPrice { get; set; }
        //[JsonProperty(PropertyName = "merchantCurrency")]
        //public string SettlementPriceCurrency { get; set; }


    }
}
