﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class CancelByTicketRQ
    {
        [JsonProperty(PropertyName = "ticket_id")]
        public int TicketId { get; set; }
    }
}
