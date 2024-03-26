using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Rezdy
{
    public class RezdyProduct
    {     
        public string AgentPaymentType { get; set; }
        public BookingField[] BookingFields { get; set; }
        public string BookingMode { get; set; }
        public string CancellationPolicyDays { get; set; }
        public string Charter { get; set; }
        public string CommissionIncludesExtras { get; set; }
        public string ConfirmMode { get; set; }
        public string ConfirmModeMinParticipants { get; set; }
        public string Currency { get; set; }
        public string DateCreated { get; set; }
        public string DateUpdated { get; set; }
        public string Description { get; set; }
        public string DurationMinutes { get; set; }
        public Extra[] Extras { get; set; }      
        public Image[] Images { get; set; }
        public string InternalCode { get; set; }
        public string[] Languages { get; set; }
        public LocationAddress LocationAddress { get; set; }
        public string MaxCommissionNetRate { get; set; }
        public string MaxCommissionPercent { get; set; }
        public string MinimumNoticeMinutes { get; set; }
        public string Name { get; set; }
        public string PickupId { get; set; }
        public PriceOption[] PriceOptions { get; set; }
        public string ProductCode { get; set; }
        public ProductSeoTag[] ProductSeoTags { get; set; }
        public string ProductType { get; set; }
        public string QrCodeType { get; set; }
        public bool QuantityRequired { get; set; }
        public int QuantityRequiredMax { get; set; }
        public int QuantityRequiredMin { get; set; }
        public string ShortDescription { get; set; }
        public string SupplierAlias { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string[] Tags { get; set; }
        public Tax[] Taxes { get; set; }
        public string Terms { get; set; }
        public string Timezone { get; set; }
        public string UnitLabel { get; set; }
        public string UnitLabelPlural { get; set; }
        public Video[] Videos { get; set; }
        public string WaitListingEnabled { get; set; }
        public string XeroAccount { get; set; }
    }

    public partial class BookingField
    {
        public string FieldType { get; set; }
        public string Label { get; set; }
        public string ListOptions { get; set; }
        public bool RequiredPerBooking { get; set; }
        public bool RequiredPerParticipant { get; set; }
        public string Value { get; set; }
        public bool VisiblePerBooking { get; set; }
        public bool VisiblePerParticipant { get; set; }
    }

    public partial class Extra
    {
        public string Description { get; set; }
        public string ExtraPriceType { get; set; }
        public string Id { get; set; }
        public Image Image { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
    }

    public partial class Image
    {
        public string Id { get; set; }
        public string ItemUrl { get; set; }
        public string LargeSizeUrl { get; set; }
        public string MediumSizeUrl { get; set; }
        public string ThumbnailUrl { get; set; }
    }

    public partial class LocationAddress
    {
        public string AddressLine { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string PostCode { get; set; }
        public string State { get; set; }
    }

    public partial class PriceOption
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int MaxQuantity { get; set; }
        public int MinQuantity { get; set; }
        public string Price { get; set; }
        public string PriceGroupType { get; set; }
        public string ProductCode { get; set; }
        public int SeatsUsed { get; set; }
    }

    public partial class ProductSeoTag
    {
        public string AttrKey { get; set; }
        public string AttrValue { get; set; }
        public string Id { get; set; }
        public string MetaType { get; set; }
        public string ProductCode { get; set; }
    }

    public partial class Tax
    {
        public string Compound { get; set; }
        public string Label { get; set; }
        public string PriceInclusive { get; set; }
        public string SupplierId { get; set; }
        public string TaxAmount { get; set; }
        public string TaxPercent { get; set; }
        public string TaxType { get; set; }
    }

    public partial class Video
    {
        public string Id { get; set; }
        public string Platform { get; set; }
        public string Url { get; set; }
    }

}
