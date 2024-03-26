using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Entities.RequestResponseModels
{
    [XmlRoot(ElementName = "Opts")]
    public class Opts
    {
        [XmlElement(ElementName = "Opt")]
        public List<string> Opt { get; set; }
    }
}