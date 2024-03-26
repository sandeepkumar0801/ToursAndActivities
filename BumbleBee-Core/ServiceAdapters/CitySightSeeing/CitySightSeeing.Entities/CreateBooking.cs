using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities
{
    public class BookingCustomer
    {
        public string country { get; set; }
        public string email { get; set; }
        public string full_name { get; set; }
        public string language { get; set; }
        [JsonProperty(PropertyName = "lastName")]
        public string lastName { get; set; }
        public string mobile { get; set; }
        public string name { get; set; }
    }

    public class Ticket
    {
        public string barcode { get; set; }
        public string reference { get; set; }
        public string type { get; set; }
    }

    public class CreateBookingRequest
    {
        public int? adult { get; set; }
        public string agent { get; set; }
        public string barcode { get; set; }
        public string booking { get; set; }
        public DateTime utcConfirmedAt { get; set; }
        public int? child { get; set; }
        public BookingCustomer customer { get; set; }
        public string date { get; set; }
        public int family { get; set; }
        public int? infant { get; set; }
        public string integration_booking_code { get; set; }
        public string notes { get; set; }
        public int option { get; set; }
        public int product { get; set; }
        public string reference { get; set; }
        public string reservation { get; set; }
        public int? resident { get; set; }
        public int? senior { get; set; }
        public int? student { get; set; }
        public int supplier_id { get; set; }
        public List<Ticket> tickets { get; set; }
        public string time { get; set; }
        public int? youth { get; set; }
    }

}
