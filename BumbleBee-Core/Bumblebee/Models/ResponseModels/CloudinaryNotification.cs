using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.ResponseModels
{
    public class CloudinaryNotification
    {
        public string NotificationType { get; set; }
        public string Source { get; set; }
        public Dictionary<string, ResourceOperation> resources { get; set; }
    }

    public class ResourceOperation
    {
        public Added[] added { get; set; }
        public Added[] removed { get; set; }
        public Added[] updated { get; set; }
        public string resource_type { get; set; }
        public string type { get; set; }
    }

    public class Added
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}