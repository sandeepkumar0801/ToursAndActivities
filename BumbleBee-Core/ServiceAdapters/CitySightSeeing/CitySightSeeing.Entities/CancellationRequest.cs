namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities
{
    public class CancellationRequest
    {
        public string agent { get; set; }
        public string barcode { get; set; }
        public string booking { get; set; }

        public int supplier_id { get; set; }
    }
}
