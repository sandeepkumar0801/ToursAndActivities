using System;

namespace Isango.Entities
{
    public class IsangoErrorEntity
    {
        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public string Token { get; set; }

        public string AffiliateId { get; set; }

        public string Params { get; set; }
    }
}