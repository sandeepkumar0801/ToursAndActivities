using System;

namespace Isango.Entities.Master
{
    public class AffiliateAPIData
    {
        public string DisplayName { get; set; }
        public string AffiliateID { get; set; }

        public string AffiliateName { get; set; }

        public string SupportedLanguages { get; set; }

        public string GoogleTrackerID { get; set; }

        public string Email { get; set; }

        public int AffiliateGroupID { get; set; }

        public string Alias { get; set; }

        public bool IsmultiSave { get; set; }

        public Int16 MultiSavePercent { get; set; }

        public string Google_TagManager { get; set; }

        public int LOB { get; set; }

        public string CompanyWebsite { get; set; }
        public string B2BAffiliateID { get; set; }

        public double? DiscountPercent { get; set; }

        public bool WhiteLabelPartner { get; set; }
    }
}