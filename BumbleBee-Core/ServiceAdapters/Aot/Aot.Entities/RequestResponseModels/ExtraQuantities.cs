using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "ExtraQuantities")]
    public class ExtraQuantities
    {
        [XmlElement(ElementName = "ExtraQuantityItem")]
        public List<ExtraQuantityItem> ExtraQuantityItem { get; set; }
    }
}