using System.Collections.Generic;

namespace Isango.Entities.Master
{
    public class AffiliateAPI
    {
        public List<AffiliateAPIData> AffiliateAPIData { get; set; }
        public List<AffiliateCurrency> AffiliateCurrency { get; set; }
        public List<AffiliateEmail> AffiliateEmail { get; set; }
        public List<AffiliatePhone> AffiliatePhone { get; set; }
        public List<AffiliateServices> AffiliateServices { get; set; }
    }
}