using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Entities.Tiqets
{
    public class TiqetsSelectedProduct : SelectedProduct
    {
        public int FactSheetId { get; set; }

        public string TimeSlot { get; set; }

        public List<Variant> Variants { get; set; }

        public TiqetsOrderStatus OrderStatus { get; set; }

        public bool Success { get; set; }

        public string TicketPdfUrl { get; set; }

        public string OrderReferenceId { get; set; }

        public List<string> RequiresVisitorsDetails { get; set; }

        public List<ProductVariantIdName> RequiresVisitorsDetailsWithVariant { get; set; }

        public List<ContractQuestion> ContractQuestions { get; set; }

        public string AffiliateId { get; set; }

        public List<PackageVariant> package_variants { get; set; }

		public List<PackageProduct> packageProductss { get; set; }

			
	}
}