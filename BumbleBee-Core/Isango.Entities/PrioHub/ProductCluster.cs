using System;
using System.Collections.Generic;

namespace Isango.Entities.PrioHub
{
    public class ProductCluster
    {
        public int ProductParentId { get; set; }
        public int ProductId { get; set; }
        public int ProductSupplierId { get; set; }
        public string ProductSupplierName { get; set; }
        public string ProductTitle { get; set; }
        public decimal ProductFromPrice { get; set; }
        public string ProductCurrencyCode { get; set; }
        public DateTime ProductStartDate { get; set; }
        public string ProductAdmissionType { get; set; }
        public bool ProductTimepickerVisible { get; set; }
    }
}