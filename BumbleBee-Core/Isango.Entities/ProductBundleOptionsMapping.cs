namespace Isango.Entities
{
    public class ProductBundleOptionsMapping
    {
        public int BundleServiceId { get; set; }
        public int BundleOptionId { get; set; }
        public int ComponentServiceId { get; set; }
        public int ComponentOptionId { get; set; }
        public string ComponentServiceName { get; set; }
        public string LanguageCode { get; set; }
        public int Order { get; set; }
        public string BundleName { get; set; }
        public string BundleOptionName { get; set; }
        public bool IsSameDayBookable { get; set; }
    }
}