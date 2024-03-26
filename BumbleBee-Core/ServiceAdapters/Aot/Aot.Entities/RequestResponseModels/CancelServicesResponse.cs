using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "ServiceStatus")]
    public class ServiceStatus
    {
        [XmlElement(ElementName = "Date")]
        public string Date { get; set; }

        [XmlElement(ElementName = "SequenceNumber")]
        public string SequenceNumber { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
    }

    [XmlRoot(ElementName = "ServiceStatuses")]
    public class ServiceStatuses
    {
        [XmlElement(ElementName = "ServiceStatus")]
        public ServiceStatus ServiceStatus { get; set; }
    }

    [XmlRoot(ElementName = "CancelServicesResponse")]
    public class CancelServicesResponse
    {
        [XmlElement(ElementName = "ServiceStatuses")]
        public List<ServiceStatuses> ServiceStatuses { get; set; }
    }
}