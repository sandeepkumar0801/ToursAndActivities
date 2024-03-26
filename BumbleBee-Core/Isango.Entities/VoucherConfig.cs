using System.Collections.Generic;

namespace Isango.Entities
{
    public class VoucherConfig
    {
        public bool IsPercent { get; set; }

        public List<int> ValidProducts { get; set; }

        public List<string> ValidAffiliates { get; set; }

        public List<int> ValidCategories { get; set; }
        public List<int> ValidDestinations { get; set; }
        public decimal ThresholdProductMargin { get; set; }
        public bool ThresholdIsPercent { get; set; }
        public string UTMParameter { get; set; }
        public bool isServiceInclusion { get; set; }
        public bool isDestinationInclusion { get; set; }
        public bool isCategoryInclusion { get; set; }
        public bool isLobInclusion { get; set; }
        public List<int> ValidLobsIds { get; set; }
    }
}