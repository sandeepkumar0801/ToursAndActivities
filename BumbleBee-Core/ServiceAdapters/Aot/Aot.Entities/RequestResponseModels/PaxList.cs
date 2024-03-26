using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "PaxList")]
    public class PaxList
    {
        [XmlElement(ElementName = "PaxDetails")]
        public List<PaxDetails> PaxDetails { get; set; }
    }
}