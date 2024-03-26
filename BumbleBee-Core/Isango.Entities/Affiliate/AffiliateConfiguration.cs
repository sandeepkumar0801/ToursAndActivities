namespace Isango.Entities.Affiliate
{
    public class AffiliateConfiguration
    {
        public bool IsB2BAffiliate { get; set; }

        public bool IsB2BNetPriceAffiliate { get; set; }

        public bool IsCriteoEnabled { get; set; }

        public bool IsOfflineEmail { get; set; }

        public bool IsRegular { get; set; }
        public bool IshowBadge { get; set; }

        //Badge related
        public bool IsShowBadge { get; set; }

        //GatePrice related
        public bool IsSupplementOffer { get; set; }

        public bool IsMultiSave { get; set; }
        public bool IsMarginApplicable { get; set; }

        public decimal MultisavePercentage { get; set; }

        public bool IsPartialBookingSupport { get; set; }
        public bool IsInDestination { get; set; }
        public int Lob { get; set; }
        //public decimal Margin { get; set; }
        public decimal DiscountPercent { get; set; }
        public int PaymentTypeId { get; set; }
        public string DefaultDiscountCode { get; set; }
    }
}