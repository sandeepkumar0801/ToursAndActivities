using System;

namespace Isango.Entities
{
    public class ProductSupplier
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Supplierinfo { get; set; }
        public string Link { get; set; }
        public int Languageid { get; set; }
        public DateTime CreateDate { get; set; }
    }
}