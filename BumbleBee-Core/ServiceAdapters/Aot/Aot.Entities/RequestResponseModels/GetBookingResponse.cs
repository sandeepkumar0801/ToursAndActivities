using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "Service")]
    public class Service
    {
        [XmlElement(ElementName = "ServiceLineId")]
        public string ServiceLineId { get; set; }

        [XmlElement(ElementName = "Opt")]
        public string Opt { get; set; }

        [XmlElement(ElementName = "Date")]
        public string Date { get; set; }

        [XmlElement(ElementName = "SequenceNumber")]
        public string SequenceNumber { get; set; }

        [XmlElement(ElementName = "SupplierConfirmation")]
        public string SupplierConfirmation { get; set; }

        [XmlElement(ElementName = "Adults")]
        public string Adults { get; set; }

        [XmlElement(ElementName = "Children")]
        public string Children { get; set; }

        [XmlElement(ElementName = "LinePrice")]
        public string LinePrice { get; set; }

        [XmlElement(ElementName = "SCUqty")]
        public string ScUqty { get; set; }

        [XmlElement(ElementName = "puTime")]
        public string PuTime { get; set; }

        [XmlElement(ElementName = "puRemark")]
        public string PuRemark { get; set; }

        [XmlElement(ElementName = "doTime")]
        public string DoTime { get; set; }

        [XmlElement(ElementName = "doRemark")]
        public string DoRemark { get; set; }

        [XmlElement(ElementName = "Comments")]
        public string Comments { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "ServiceExtras")]
        public ServiceExtras ServiceExtras { get; set; }
    }

    [XmlRoot(ElementName = "ServiceExtra")]
    public class ServiceExtra
    {
        [XmlElement(ElementName = "SequenceNumber")]
        public string SequenceNumber { get; set; }

        [XmlElement(ElementName = "Quantity")]
        public string Quantity { get; set; }
    }

    [XmlRoot(ElementName = "ServiceExtras")]
    public class ServiceExtras
    {
        [XmlElement(ElementName = "ServiceExtra")]
        public List<ServiceExtra> ServiceExtra { get; set; }
    }

    [XmlRoot(ElementName = "Services")]
    public class GetBookingServices
    {
        [XmlElement(ElementName = "Service")]
        public List<Service> Service { get; set; }
    }

    [XmlRoot(ElementName = "GetBookingResponse")]
    public class GetBookingResponse
    {
        [XmlElement(ElementName = "Ref")]
        public string Ref { get; set; }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Consult")]
        public string Consult { get; set; }

        [XmlElement(ElementName = "AgentRef")]
        public string AgentRef { get; set; }

        [XmlElement(ElementName = "TravelDate")]
        public string TravelDate { get; set; }

        [XmlElement(ElementName = "EnteredDate")]
        public string EnteredDate { get; set; }

        [XmlElement(ElementName = "BookingStatus")]
        public string BookingStatus { get; set; }

        [XmlElement(ElementName = "Currency")]
        public string Currency { get; set; }

        [XmlElement(ElementName = "TotalPrice")]
        public string TotalPrice { get; set; }

        [XmlElement(ElementName = "DeliveryMethod")]
        public string DeliveryMethod { get; set; }

        [XmlElement(ElementName = "DeliveryCost")]
        public string DeliveryCost { get; set; }

        [XmlElement(ElementName = "PaymentMethod")]
        public string PaymentMethod { get; set; }

        [XmlElement(ElementName = "PaymentRef")]
        public string PaymentRef { get; set; }

        [XmlElement(ElementName = "ContactDetails")]
        public ContactDetails ContactDetails { get; set; }

        [XmlElement(ElementName = "Comments")]
        public string Comments { get; set; }

        [XmlElement(ElementName = "Services")]
        public GetBookingServices Services { get; set; }
    }
}