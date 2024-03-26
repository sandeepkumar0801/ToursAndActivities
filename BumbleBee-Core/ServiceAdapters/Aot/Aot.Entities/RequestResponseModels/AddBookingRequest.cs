using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "Services")]
    public class Services
    {
        [XmlElement(ElementName = "AddServiceInfo")]
        public List<AddServiceInfo> AddServiceInfo { get; set; }
    }

    [XmlRoot(ElementName = "AddBookingRequest")]
    public class AddBookingRequest
    {
        [XmlElement(ElementName = "AgentID")]
        public string AgentId { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Consult")]
        public string Consult { get; set; }

        [XmlElement(ElementName = "DeliveryMethod")]
        public string DeliveryMethod { get; set; }

        [XmlElement(ElementName = "PaymentMethod")]
        public string PaymentMethod { get; set; }

        [XmlElement(ElementName = "PaymentRef")]
        public string PaymentRef { get; set; }

        [XmlElement(ElementName = "ContactDetails")]
        public ContactDetails ContactDetails { get; set; }

        [XmlElement(ElementName = "Comments")]
        public string Comments { get; set; }

        [XmlElement(ElementName = "Services")]
        public Services Services { get; set; }

        [XmlElement(ElementName = "EmailNotification")]
        public string EmailNotification { get; set; }

        [XmlElement(ElementName = "AgentRef")]
        public string AgentRef { get; set; }
    }
}