using System;

namespace Isango.Entities
{
    [Serializable]
    public class SEOCanonical
    {
        public int ServiceId { get; set; }
        public string CanonicalUrl { get; set; }
        public string LanguageCode { get; set; }
    }
}