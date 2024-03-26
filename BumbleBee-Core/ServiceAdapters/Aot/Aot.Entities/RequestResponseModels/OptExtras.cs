using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "OptExtras")]
    public class OptExtras
    {
        [XmlElement(ElementName = "OptExtra")]
        public List<OptExtra> OptExtra { get; set; }
    }
}