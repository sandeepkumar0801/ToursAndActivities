using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "AddServiceResponse")]
    public class AddServiceResponse
    {
        [XmlElement(ElementName = "ServiceLineId")]
        public string ServiceLineId { get; set; }

        [XmlElement(ElementName = "SequenceNumber")]
        public string SequenceNumber { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "LinePrice")]
        public string LinePrice { get; set; }

        [XmlElement(ElementName = "Currency")]
        public string Currency { get; set; }
    }

    [XmlRoot(ElementName = "AddServiceResponses")]
    public class AddServiceResponses
    {
        [XmlElement(ElementName = "AddServiceResponse")]
        public List<AddServiceResponse> AddServiceResponse { get; set; }
    }

    [XmlRoot(ElementName = "AddBookingResponse")]
    public class AddBookingResponse
    {
        [XmlElement(ElementName = "Ref")]
        public string Ref { get; set; }

        [XmlElement(ElementName = "AddServiceResponses")]
        public AddServiceResponses AddServiceResponses { get; set; }
    }
}