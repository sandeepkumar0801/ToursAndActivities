namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities
{
    public class ReservationRequest
    {
        public int adult { get; set; }
        public string agent { get; set; }
        public string date { get; set; }
        public int option { get; set; }
        public int product { get; set; }
        public int supplier_id { get; set; }
        public string time { get; set; }
    }

}
