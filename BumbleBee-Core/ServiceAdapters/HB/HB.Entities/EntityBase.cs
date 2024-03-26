using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.HB.HB.Entities
{
    public class EntityBase
    {
        [JsonProperty("errors")]
        public List<Error> Errors { get; set; }

        [JsonProperty("operationId")]
        public string OperationId { get; set; }

        [JsonProperty("auditData")]
        public Auditdata AuditData { get; set; }
    }

    public class Error
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("internalDescription")]
        public string InternalDescription { get; set; }
    }

    public class Auditdata
    {
        [JsonProperty("serverId")]
        public string ServerId { get; set; }

        [JsonProperty("environment")]
        public string Environment { get; set; }

        [JsonProperty("processTime")]
        public float ProcessTime { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }
    }
}