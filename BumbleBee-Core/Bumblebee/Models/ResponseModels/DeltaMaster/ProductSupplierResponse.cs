using System;

namespace WebAPI.Models.ResponseModels.Master
{
    public class ProductSupplierResponse
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Supplierinfo { get; set; }
        public string Link { get; set; }
        public int Languageid { get; set; }
        public DateTime CreateDate { get; set; }
    }
}