using Isango.Entities.GlobalTixV3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.ProductList
{
    public class ProductList
    {

        [JsonProperty(PropertyName = "data")]
        public Datum[] Data { get; set; }

        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }

        [JsonProperty(PropertyName = "size")]
        public object Size { get; set; }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
    }
    public class Datum
    {
        public string Country { get; set; }
        public float OriginalPrice { get; set; }
        //public string Keywords { get; set; }
        //public object FromPrice { get; set; }
        public string City { get; set; }
        public string Currency { get; set; }
        public int Id { get; set; }
        public bool IsOpenDated { get; set; }
        public bool IsOwnContracted { get; set; }
        public bool IsFavorited { get; set; }
        public bool IsBestSeller { get; set; }
       // public object FromReseller { get; set; }
        public string Name { get; set; }
        public bool IsInstantConfirmation { get; set; }
        public string Category { get; set; }
        //public Tickettype Tickettype { get; set; }
        public bool success { get; set; }
    }

}
