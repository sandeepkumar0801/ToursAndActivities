using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class Question : Identifier
    {
        [JsonProperty(PropertyName = "optional")]
        public bool IsOptional { get; set; }
        [JsonProperty(PropertyName = "options")]
        public string[] Options { get; set; }
        [JsonProperty(PropertyName = "question")]
        public string QuestionText { get; set; }
        [JsonProperty(PropertyName = "type")]
        public EnumValue Type { get; set; }
    }
}
