using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Ventrata.Ventrata.Entities.Request
{
    public class BookingAndReservationCancellationReq
    {
        [JsonProperty(PropertyName = "reason")]
        public string ReasonForCancellation { get; set; }
    }
}
