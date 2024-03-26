using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "ExtrasRates")]
    public class ExtrasRates
    {
        [XmlElement(ElementName = "ExtrasRate")]
        public List<ExtrasRate> ExtrasRate { get; set; }
    }
}