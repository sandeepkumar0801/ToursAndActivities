using System;
using System.Xml.Serialization;

namespace Isango.Entities
{
    [Serializable]
    public class CJFeedProduct
    {
        [XmlAttribute(DataType = "string", AttributeName = "id")]
        public string id { get; set; }

        public string ID { get; set; }
        public string name { get; set; }
        public string producturl { get; set; }
        public string smallimage { get; set; }
        public string bigimage { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public decimal retailprice { get; set; }
        public decimal discount { get; set; }
        public bool recommendable { get; set; }
        public int instock { get; set; }
        public string categoryid1 { get; set; }
        public string categoryid2 { get; set; }
        public string categoryid3 { get; set; }
    }
}