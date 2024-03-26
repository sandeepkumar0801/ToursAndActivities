using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Ventrata.Ventrata.Entities.Request
{
    public class BookingConfirmationReq
    {
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        [JsonProperty(PropertyName = "resellerReference")]
        public string ResellerReference { get; set; }
        [JsonProperty(PropertyName = "contact")]
        public Contact ContactDetails { get; set; }
    }

    public class Contact
    {
        [JsonProperty(PropertyName = "fullName")]
        public string FullName { get; set; }
        [JsonProperty(PropertyName = "emailAddress")]
        public string EmailAddress { get; set; }
        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNo { get; set; }
    }
}
