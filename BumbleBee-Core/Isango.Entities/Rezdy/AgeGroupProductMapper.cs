using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Rezdy
{
    public class AgeGroupProductMapper
    {
        public List<RezdyProductDetail> RezdyProductDetails { get; set; }
        public List<AgeGroupMapping> AgeGroupMappings { get; set; }
        public List<BookingFieldMapping> BookingFields { get; set; }
        public List<ProductWiseExtraDetails> ListOfProductWiseExtraDetails { get; set; }
    }

    public class ProductWiseExtraDetails
    {
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExtraPriceType { get; set; }
        public string Price { get; set; }
    }

    public class AgeGroupMapping
    {
        public int SupplierOptionId { get; set; }
        public string ProductCode { get; set; }
        public string Label { get; set; }       
        public int SeatsUsed { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public string PriceGroupType { get; set; }
    }

    public class RezdyProductDetail
    {
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int SupplierId { get; set; }
        public string SupplierAlias { get; set; }
        public string SupplierName { get; set; }
        public bool QuantityRequired { get; set; }
        public int QuantityRequiredMin { get; set; }
        public int QuantityRequiredMax { get; set; }
        public int CancellationPolicyDays { get; set; }
        public int PickUpId { get; set; }
        public string Currency { get; set; }
        public string InternalCode { get; set; }
    }

    public class BookingFieldMapping
    {
        public string ProductCode { get; set; }
        public string Label { get; set; }
        public bool VisiblePerBooking { get; set; }
        public bool RequiredPerBooking { get; set; }
        public bool VisiblePerParticipant { get; set; }
        public bool RequiredPerParticipant { get; set; }
    }
}
