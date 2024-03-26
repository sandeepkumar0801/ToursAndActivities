namespace Isango.Entities.RedeamV12
{
    public class SupplierData
    {
        public string SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string PartnerId { get; set; }
        public int Version { get; set; }
       
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public bool ContactPrimary { get; set; }
        public string ContactTitle { get; set; }

        public string BusinessType { get; set; }
        public string Website { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}