using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class CancelOrderRequest
    {
        [JsonProperty(PropertyName = "cancellation_reason")]
        public string CancellationReason { get; set; }
    }
}
