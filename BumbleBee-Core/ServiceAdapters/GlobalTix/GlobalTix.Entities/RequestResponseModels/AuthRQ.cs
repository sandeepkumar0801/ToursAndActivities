using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class AuthRQ
    {
        [JsonProperty(PropertyName="username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName ="password")]
        public string Password { get; set; }
    }
}
