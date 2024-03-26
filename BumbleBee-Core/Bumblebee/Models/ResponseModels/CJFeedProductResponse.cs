using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WebAPI.Models.ResponseModels
{
	[Serializable]
	public class CJFeedProductResponse
	{
		[XmlAttribute(DataType = "string", AttributeName = "id")]
		public string id { get; set; }

		public string name { get; set; }
		public string producturl { get; set; }
		public string smallimage { get; set; }

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

	[XmlRoot(Namespace = "www.isango.com", ElementName = "root")]
	public class RootCriteo
	{
		public ProductsList products;
	}

	public class ProductsList
	{
		/* Set the element name and namespace of the XML element.
        By applying an XmlElementAttribute to an array,  you instruct
        the XmlSerializer to serialize the array as a series of XML
        elements, instead of a nested set of elements. */

		[XmlElement(ElementName = "product")]
		public List<CJFeedProductResponse> productList;
	}
}