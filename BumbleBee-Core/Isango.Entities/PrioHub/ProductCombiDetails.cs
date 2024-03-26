using System;
using System.Collections.Generic;

namespace Isango.Entities.PrioHub
{
    public class ProductCombiDetails
    {
        public string ProductParentId { get; set; }
        public string ProductId { get; set; }
        public string ProductSupplierId { get; set; }
        public string ProductSupplierName { get; set; }
        public string ProductTitle { get; set; }
        public string ProductFromPrice { get; set; }
        public string ProductCurrencyCode { get; set; }
        public DateTime ProductStartDate { get; set; }
        public int ProductBookingWindowProductId { get; set; }
        public int ProductBookingWindowStartTime { get; set; }
        public int ProductBookingWindowEndTime { get; set; }
        public string ProductAdmissionType { get; set; }
        public bool ProductTimepickerVisible { get; set; }
    }
}