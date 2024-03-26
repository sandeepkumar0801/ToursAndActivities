using Isango.Entities.Payment;
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace Isango.Entities
{
    public class CreditCard : PaymentType
    {
        public string CardNumber { get; set; }

        public string CardType { get; set; }
        public string CardHoldersName { get; set; }
        public String CardHoldersEmail { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public string SecurityCode { get; set; }

        public string CardHoldersAddress1 { get; set; }
        public string CardHoldersAddress2 { get; set; }
        public string CardHoldersPhoneNumber { get; set; }
        public string CardHoldersCity { get; set; }
        public string CardHoldersState { get; set; }
        public string CardHoldersCountryName { get; set; }
        public string CardHoldersZipCode { get; set; }
        public string BillingAddressState { get; set; }
        public string BillingAddressCountry { get; set; }
    }
}