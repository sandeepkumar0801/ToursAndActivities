using System;
namespace Isango.Entities.PrioHub
{
    public class NewPrioProductTypeSeasons
    {
        public int ProductId { get; set; }
        public string ProductTypeId { get; set; }
        public string ProductType { get; set; }
        public string ProductTypeLabel { get; set; }
        public string ProductTypeClass { get; set; }
        public int ProductTypeAgeFrom { get; set; }
        public int ProductTypeAgeTo { get; set; }
        public int ProductTypePax { get; set; }
        public int ProductTypeCapacity { get; set; }
        public string ProductTypePriceType { get; set; }
        public string ProductTypePriceTaxid { get; set; }
        public string ProductTypeListPrice { get; set; }
        public bool ProductTypeDisplayPrice { get; set; }
        public string ProductTypeSalesPrice { get; set; }
        public string ProductTyperesalePrice { get; set; }
        public string ProductTypeSupplierPrice { get; set; }
        public string ProductTypeDiscount { get; set; }
        public string FeeAmount { get; set; }
        public string FeePercentage { get; set; }
    }
}