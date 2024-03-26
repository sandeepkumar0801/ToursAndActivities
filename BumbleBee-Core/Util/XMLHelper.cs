using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Util
{
    public static class XmlHelper
    {
        /// <summary>
        /// Method will add a new attribute to root node of given xml document and will assign the value to attribute
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="rootNode"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public static void AddAttribute(XmlDocument doc, XmlNode rootNode, string attribute, string value)
        {
            var xmlAttribute = doc.CreateAttribute(attribute);
            xmlAttribute.Value = value;
            rootNode.Attributes?.Append(xmlAttribute);
        }

        /// <summary>
        /// Method will add a new node to root node of given xml document and will assign the value to new node
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="rootNode"></param>
        /// <param name="node"></param>
        /// <param name="value"></param>
        public static void AddElement(XmlDocument doc, XmlNode rootNode, string node, string value)
        {
            XmlNode xmlNode = doc.CreateElement(node);
            xmlNode.InnerXml = value;
            rootNode.AppendChild(xmlNode);
        }

        /// <summary>
        /// Method will serialize the object and write in xml file to the path specified
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        public static void SerializeToXml(string filePath, Object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());
            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, obj);
                writer.Flush();
                writer.Close();
            }
        }
    }
}