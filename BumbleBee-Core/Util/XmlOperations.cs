using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Util
{
    public static class XmlOperations
    {
        public static string GetOuterXml(XmlParser parser)
        {
            return parser.XmlDocument.OuterXml;
        }

        public static string ResetEncoding(string xmlDocOld, string strEncoding)
        {
            var intIndex = xmlDocOld.IndexOf("encoding", StringComparison.CurrentCulture);
            var intStartIndex = xmlDocOld.IndexOf("\"", intIndex, StringComparison.CurrentCulture);
            var intEndIndex = xmlDocOld.IndexOf("\"", intStartIndex + 1, StringComparison.CurrentCulture);

            var xmlDocNew = $"{xmlDocOld.Substring(0, intStartIndex + 1)}{strEncoding}{xmlDocOld.Substring(intEndIndex, xmlDocOld.Length - intEndIndex)}";

            return xmlDocNew;
        }

        public static void AddInnerTextToNode(string nodeName, int position, XmlParser parser, string innerText)
        {
            var node = GetNode(nodeName, position, parser);
            node.InnerText = innerText;
        }

        public static string AddInnerTextToNode(string nodeName, int position, string fileContent, string innerText)
        {
            var parser = GetXmlParser(fileContent);

            AddInnerTextToNode(nodeName, position, parser, innerText);

            return parser.XmlDocument.OuterXml;
        }

        public static XmlNode GetNode(string nodeName, int position, XmlParser parser)
        {
            var node = parser.GetNodeByName(nodeName, position);
            return node;
        }

        public static string GetInnerTextForNode(string nodeName, int position, XmlParser parser)
        {
            var node = GetNode(nodeName, position, parser);

            return node?.InnerText;
        }

        public static XmlElement GetElement(string elementName, int position, XmlParser parser)
        {
            var element = parser.GetElement(elementName, position);
            return element;
        }

        public static XmlElement GetElement(string elementName, XmlElement parentElement, int position, XmlParser parser)
        {
            var elements = parser.GetAllElements(parentElement, elementName);
            if (elements == null)
                return GetElement(elementName, position, parser);
            return elements.Count > position ? elements[position] : null;
        }

        public static string GetInnerTextForElement(string elementName, int position, XmlParser parser)
        {
            var element = GetElement(elementName, position, parser);
            return element?.InnerText;
        }

        public static void SetInnerTextForElement(string elementName, int position, XmlParser parser, string text)
        {
            var element = GetElement(elementName, position, parser);

            element.InnerText = text;
        }

        public static void SetInnerXmlForElement(string elementName, int position, XmlParser parser, string text)
        {
            var element = GetElement(elementName, position, parser);
            element.InnerXml = text;
        }

        public static void SetInnerXmlForElement(string elementName, XmlElement parentElement, int position, XmlParser parser, string text)
        {
            var element = GetElement(elementName, parentElement, position, parser);
            element.InnerXml = text;
        }

        public static string GetInnerTextForElement(string elementName, XmlElement parentElement, int position, XmlParser parser)
        {
            var element = GetElement(elementName, parentElement, position, parser);
            return element?.InnerText;
        }

        public static void SetInnerTextForElement(string elementName, XmlElement parentElement, int position, XmlParser parser, string text)
        {
            var element = GetElement(elementName, parentElement, position, parser);

            element.InnerText = text;
        }

        public static string GetInnerTextForNode(string nodeName, int position, string fileContent)
        {
            var parser = GetXmlParser(fileContent);
            return GetInnerTextForNode(nodeName, position, parser);
        }

        public static XmlParser GetXmlParser(string fileContent)
        {
            var parser = new XmlParser();
            parser.ParseXml(fileContent);
            return parser;
        }

        public static XmlParser GetXmlParser()
        {
            var parser = new XmlParser();
            return parser;
        }

        /// <summary>
        /// Packs the chars representing Hex values into real binary string containing equivalent Hex values.
        /// </summary>
        /// <param name="strBinData"></param>
        /// <returns></returns>
        public static byte[] Pack(string strBinData)
        {
            int j = 0, i = 0;

            var asciiEnc = new ASCIIEncoding();
            var p = asciiEnc.GetBytes(strBinData);
            var packedData = new byte[strBinData.Length / 2];

            while (i < strBinData.Length)
            {
                var n1 = p[i] >= '0' && p[i] <= '9' ? (byte)(p[i] - '0') : (byte)(p[i] - 'A' + 10);
                i++;

                var n2 = p[i] >= '0' && p[i] <= '9' ? (byte)(p[i] - '0') : (byte)(p[i] - 'A' + 10);
                i++;

                var theByte = (byte)((n1 << 4) | n2);
                packedData[j++] = theByte;
            }
            return packedData;
        }

        public static void RemoveAttribute(string elementName, int position, XmlParser parser, string attributeName)
        {
            var element = parser.GetElement(elementName, position);
            if (element.HasAttributes && element.HasAttribute(attributeName))
                element.Attributes.Remove(element.Attributes[attributeName]);
        }

        /// <summary>
        /// Rename the "OldValue" node to "NewValue" node under "ParentNode"
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public static void RenameNode(XmlElement parentNode, string oldValue, string newValue)
        {
            var objXmlNode = parentNode.SelectSingleNode(oldValue);

            RenameNode(objXmlNode, newValue);
        }

        /// <summary>
        /// Rename "Node" to "NewName"
        /// </summary>
        /// <param name="node"></param>
        /// <param name="newName"></param>
        public static void RenameNode(XmlNode node, string newName)
        {
            if (node.OwnerDocument == null) return;
            var newElement = node.OwnerDocument.CreateElement(newName);
            newElement.InnerXml = node.InnerXml;

            var parentNode = node.ParentNode;

            // old attribute is removed
            parentNode?.ReplaceChild(newElement, node);
        }

        /// <summary>
        /// Convert an XML node Text "yes" or "no" to boolean 0 or 1.
        /// </summary>
        /// <param name="node"></param>
        public static void ConvertToBoolean(XmlNode node)
        {
            if (node.InnerText.ToLower() == "no")
            {
                node.InnerText = "0";
            }
            else if (node.InnerText.ToLower() == "yes")
            {
                node.InnerText = "1";
            }
        }

        public static List<XmlElement> NodeListToListOfElements(XmlNodeList objNodeList)
        {
            var lstNodes = new List<XmlElement>();

            foreach (XmlNode objXmlNode in objNodeList)
                lstNodes.Add((XmlElement)objXmlNode);

            return lstNodes;
        }

        public static string GetElementAttributeValue(string elementName, int position, XmlParser parser, string attributeName)
        {
            var element = parser.GetElement(elementName, position);

            return element == null ? null : parser.GetAttributeValue(element, attributeName);
        }

        public static void SetElementAttributeValue(string elementName, int position, XmlParser parser, string attributeName, string attributeValue)
        {
            var element = parser.GetElement(elementName, position);
            if (element.HasAttributes && element.HasAttribute(attributeName))
                element.Attributes[attributeName].Value = attributeValue;
        }

        public static string GetElementAttributeValue(string elementName, int position, XmlElement parentElement, XmlParser parser, string attributeName)
        {
            var element = GetElement(elementName, parentElement, position, parser);
            return parser.GetAttributeValue(element, attributeName);
        }

        public static void SetElementAttributeValue(string elementName, int position, XmlElement parentElement, XmlParser parser, string attributeName, string attributeValue)
        {
            var element = GetElement(elementName, parentElement, position, parser);
            if (element.HasAttributes && element.HasAttribute(attributeName))
                element.Attributes[attributeName].Value = attributeValue;
        }
    }
}