using Newtonsoft.Json;

namespace ServiceAdapters.Rezdy.Rezdy.Entities.ProductDetails
{
    public class GetProductResponse
    {
        [JsonProperty("product")]
        public Product Product { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }
    }

    public class Product
    {
        [JsonProperty("additionalInformation")]
        public string AdditionalInformation { get; set; }

        [JsonProperty("advertisedPrice")]
        public string AdvertisedPrice { get; set; }

        [JsonProperty("agentPaymentType")]
        public string AgentPaymentType { get; set; }

        [JsonProperty("bookingFields")]
        public BookingField[] BookingFields { get; set; }

        [JsonProperty("bookingMode")]
        public string BookingMode { get; set; }

        [JsonProperty("cancellationPolicyDays")]
        public string CancellationPolicyDays { get; set; }

        [JsonProperty("charter")]
        public string Charter { get; set; }

        [JsonProperty("commissionIncludesExtras")]
        public string CommissionIncludesExtras { get; set; }

        [JsonProperty("confirmMode")]
        public string ConfirmMode { get; set; }

        [JsonProperty("confirmModeMinParticipants")]
        public string ConfirmModeMinParticipants { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("dateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("dateUpdated")]
        public string DateUpdated { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("durationMinutes")]
        public string DurationMinutes { get; set; }

        [JsonProperty("extras")]
        public Extra[] Extras { get; set; }

        [JsonProperty("generalTerms")]
        public string GeneralTerms { get; set; }

        [JsonProperty("images")]
        public Image[] Images { get; set; }

        [JsonProperty("internalCode")]
        public string InternalCode { get; set; }

        [JsonProperty("languages")]
        public string[] Languages { get; set; }

        [JsonProperty("locationAddress")]
        public LocationAddress LocationAddress { get; set; }

        [JsonProperty("maxCommissionNetRate")]
        public string MaxCommissionNetRate { get; set; }

        [JsonProperty("maxCommissionPercent")]
        public string MaxCommissionPercent { get; set; }

        [JsonProperty("minimumNoticeMinutes")]
        public string MinimumNoticeMinutes { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pickupId")]
        public string PickupId { get; set; }

        [JsonProperty("priceOptions")]
        public PriceOption[] PriceOptions { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("productSeoTags")]
        public ProductSeoTag[] ProductSeoTags { get; set; }

        [JsonProperty("productType")]
        public string ProductType { get; set; }

        [JsonProperty("qrCodeType")]
        public string QrCodeType { get; set; }

        [JsonProperty("quantityRequired")]
        public string QuantityRequired { get; set; }

        [JsonProperty("quantityRequiredMax")]
        public string QuantityRequiredMax { get; set; }

        [JsonProperty("quantityRequiredMin")]
        public string QuantityRequiredMin { get; set; }

        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("supplierAlias")]
        public string SupplierAlias { get; set; }

        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }

        [JsonProperty("supplierName")]
        public string SupplierName { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("taxes")]
        public Tax[] Taxes { get; set; }

        [JsonProperty("terms")]
        public string Terms { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("unitLabel")]
        public string UnitLabel { get; set; }

        [JsonProperty("unitLabelPlural")]
        public string UnitLabelPlural { get; set; }

        [JsonProperty("videos")]
        public Video[] Videos { get; set; }

        [JsonProperty("waitListingEnabled")]
        public string WaitListingEnabled { get; set; }

        [JsonProperty("xeroAccount")]
        public string XeroAccount { get; set; }
    }

    public class BookingField
    {
        [JsonProperty("fieldType")]
        public string FieldType { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("listOptions")]
        public string ListOptions { get; set; }

        [JsonProperty("requiredPerBooking")]
        public string RequiredPerBooking { get; set; }

        [JsonProperty("requiredPerParticipant")]
        public string RequiredPerParticipant { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("visiblePerBooking")]
        public string VisiblePerBooking { get; set; }

        [JsonProperty("visiblePerParticipant")]
        public string VisiblePerParticipant { get; set; }
    }

    public class Extra
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("extraPriceType")]
        public string ExtraPriceType { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }
    }

    public class Image
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("itemUrl")]
        public string ItemUrl { get; set; }

        [JsonProperty("largeSizeUrl")]
        public string LargeSizeUrl { get; set; }

        [JsonProperty("mediumSizeUrl")]
        public string MediumSizeUrl { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }
    }

    public class LocationAddress
    {
        [JsonProperty("addressLine")]
        public string AddressLine { get; set; }

        [JsonProperty("addressLine2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public class PriceOption
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("maxQuantity")]
        public string MaxQuantity { get; set; }

        [JsonProperty("minQuantity")]
        public string MinQuantity { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("priceGroupType")]
        public string PriceGroupType { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("seatsUsed")]
        public string SeatsUsed { get; set; }
    }

    public class ProductSeoTag
    {
        [JsonProperty("attrKey")]
        public string AttrKey { get; set; }

        [JsonProperty("attrValue")]
        public string AttrValue { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("metaType")]
        public string MetaType { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }
    }

    public class Tax
    {
        [JsonProperty("compound")]
        public string Compound { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("priceInclusive")]
        public string PriceInclusive { get; set; }

        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }

        [JsonProperty("taxAmount")]
        public string TaxAmount { get; set; }

        [JsonProperty("taxPercent")]
        public string TaxPercent { get; set; }

        [JsonProperty("taxType")]
        public string TaxType { get; set; }
    }

    public class Video
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class RequestStatus
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("warning")]
        public Warning Warning { get; set; }
    }

    public class Error
    {
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }

    public class Warning
    {
        [JsonProperty("warningMessage")]
        public string WarningMessage { get; set; }
    }
}