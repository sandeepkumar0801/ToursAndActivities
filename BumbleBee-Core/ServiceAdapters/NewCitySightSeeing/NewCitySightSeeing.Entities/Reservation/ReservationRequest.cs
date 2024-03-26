using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Reservation
{

    public class ReservationRequest
    {
        [JsonProperty("productCode")]
        public string ProductCode { get; set; }
        [JsonProperty("variantCode")]
        public string VariantCode { get; set; }
        [JsonProperty("externalServiceRefCode")]
        public string ExternalServiceRefCode { get; set; }
        [JsonProperty("reservationDate")]
        public DateTime ReservationDate { get; set; }
        [JsonProperty("contact")]
        public Contact Contact { get; set; }
        [JsonProperty("lines")]
        public List<Line> Lines { get; set; }
    }

    public class Contact
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("surname")]
        public string Surname { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("province")]
        public string Province { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
        [JsonProperty("fiscalCode")]
        public string FiscalCode { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
    }

    public class Line
    {
        [JsonProperty("rate")]
        public string Rate { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }

}