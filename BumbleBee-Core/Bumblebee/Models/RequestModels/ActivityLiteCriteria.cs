using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.RequestModels
{
    public class ActivityLiteCriteria
    {
        public string? AffiliateId { get; set; }
        public string? TokenId { get; set; }
        public string? CategoryIds { get; set; }
    }
}