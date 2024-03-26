using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Util
{
    /// <summary>
    /// A class to parse XML. The xml can be in a file or in the form of string
    /// </summary>
    public class XmlParser
    {
        #region private members

        private XmlElement m_RootElement;
        private XmlDocument m_XmlDocument;

        #endregion private members

        #region constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public XmlParser()
        {
            m_XmlDocument = new XmlDocument();
        }

        //------------------------------------------------------------------------------------------

        #endregion constructors

        #region properties

        /// <summary>
        /// property to get _xmlDoocument private field
        /// </summary>
        public XmlDocument XmlDocument => m_XmlDocument;

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// property to get the _rootElement private field
        /// </summary>
        public XmlElement XmlRootElement => m_RootElement;

        //------------------------------------------------------------------------------------------

        #endregion properties

        #region public methods

        public bool NodeExist(string xmlPath)
        {
            var xmlNode = XmlDocument.SelectSingleNode(xmlPath);
            return xmlNode != null;
        }

        /// <summary>
        /// method to parse xml in a file. Input is the file name with complete path
        /// </summary>
        /// <param name="fileToParse"></param>
        public void Parse(string fileToParse)
        {
            StreamReader fileReader = null;

            try
            {
                using (fileReader = new StreamReader(new FileStream(fileToParse, FileMode.Open, FileAccess.Read)))
                {
                    var fileContent = fileReader.ReadToEnd();

                    ParseXml(fileContent);
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                fileReader?.Close();
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// method to parse xml in string form
        /// </summary>
        /// <param name="fileContent"></param>
        public void ParseXml(string fileContent)
        {
            m_XmlDocument.LoadXml(fileContent);
            m_RootElement = XmlDocument.DocumentElement;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// method to reset the parser members at runtime
        /// </summary>
        public void ResetParser()
        {
            m_XmlDocument = null;
            m_RootElement = null;

            m_XmlDocument = new XmlDocument();
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the name of the node by name and position
        /// </summary>
        public XmlNode GetNodeByName(string nodeName, int position)
        {
            var nodelist = GetNodeList(nodeName);
            return nodelist.Count < position ? null : nodelist[position];
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Method returns the node list for a given node name. This will be useful iterating
        /// over several nodes for processing.
        /// </summary>
        public XmlNodeList GetNodeList(String nodeName)
        {
            if (XmlDocument == null)
            {
                throw new XmlException("The XML document provided could not be parsed");
            }

            var nodelist = XmlDocument.GetElementsByTagName(nodeName);
            return nodelist;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// This method gives the element for a given name and position. The
        /// return value will be an element only if the positional element is
        /// present.
        /// </summary>
        public XmlElement GetElement(String elementName, int position)
        {
            if (XmlDocument == null || elementName == null || position < 0)
            {
                throw new XmlException("The XML document provided could not be parsed");
            }

            var node = GetNodeByName(elementName, position);

            if (node != null && node.NodeType == XmlNodeType.Element)
            {
                return (XmlElement)node;
            }

            return null;
        }

        //------------------------------------------------------------------------------------------
        public XmlElement GetElement(XmlElement parentElement, int position)
        {
            return GetElement(parentElement, "", position);
        }

        /// <summary>
        /// This method is used to get element against a parent element
        /// and the position.
        /// WARNING: Does not care about the child element's name.
        /// </summary>
        public XmlElement GetElement(XmlElement parentElement, string childElement, int position)
        {
            if (XmlDocument == null || parentElement == null)
            {
                throw new XmlException("The XML document provided could not be parsed");
            }

            if (!parentElement.HasChildNodes) return null;
            var nodeList = parentElement.ChildNodes;
            if (position >= nodeList.Count) return null;
            var node = nodeList[position];

            if (node.NodeType != XmlNodeType.Element)
                return null;

            if (string.IsNullOrEmpty(childElement) || (!string.IsNullOrEmpty(childElement) && node.Name == childElement))
            {
                return (XmlElement)node;
            }

            return null;
        }

        /// <summary>
        /// Returns a child element against a parent.
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="childElement"></param>
        /// <returns></returns>
        public XmlElement GetElementByName(XmlElement parentElement, string childElement)
        {
            if (XmlDocument == null || parentElement == null)
            {
                throw new XmlException("The XML document provided could not be parsed");
            }

            return parentElement.ChildNodes.Cast<XmlNode>().Where(child => child.Name == childElement).Select(node => node).Cast<XmlElement>().FirstOrDefault();
        }

        //------------------------------------------------------------------------------------------

        public List<XmlElement> GetAllElements(string nodeName)
        {
            var nodelist = GetNodeList(nodeName);

            var xmlElemList = new List<XmlElement>();

            foreach (XmlNode node in nodelist)
            {
                if (node.NodeType == XmlNodeType.Element)
                    xmlElemList.Add((XmlElement)node);
            }
            return xmlElemList;
        }

        public List<XmlElement> GetAllElements(XmlElement parentElement)
        {
            return GetAllElements(parentElement, "");
        }

        public List<XmlElement> GetAllElements(XmlElement parentElement, string childElement)
        {
            if (XmlDocument == null || parentElement == null)
            {
                throw new XmlException("The XML document provided could not be parsed");
            }
            var isChildElementDefined = !string.IsNullOrEmpty(childElement);

            return (from XmlNode node in parentElement.ChildNodes where node.NodeType == XmlNodeType.Element let toAdd = !isChildElementDefined || node.Name == childElement where toAdd select node).Cast<XmlElement>().ToList();
        }

        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Method returns text for a given element.
        /// </summary>
        public string GetElementText(XmlElement element)
        {
            if (XmlDocument == null || element == null)
            {
                throw new XmlException("The XML document provided could not be parsed");
            }
            var result = element.InnerText;

            return result;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Method returns value for a particular attribute.
        /// </summary>
        public string GetAttributeValue(XmlElement element, string attribName)
        {
            if (XmlDocument == null || element == null)
            {
                throw new XmlException("The XML document provided could not be parsed");
            }

            var lAttribute = element.GetAttribute(attribName);

            return lAttribute;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Method returns the attribute list for a given element. This method
        /// will be useful when all attributes will be required for a particular
        /// element.
        /// </summary>
        public Hashtable GetAttributeList(XmlElement element)
        {
            var table = new Hashtable();
            int count;

            if (XmlDocument == null || element == null)
            {
                throw new XmlException("The XML document provided could not be parsed");
            }

            var collection = element.Attributes;

            for (count = 0; count < collection.Count; count++)
            {
                table.Add(collection[count].Name, collection[count].Value);
            }

            return table;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the child node collection for a given XMLElement.
        /// The collection will include only those nodes which do not have further children.
        /// IDictionary returned will have node name as key and Inner Text as the Value
        ///  </summary>
        public IDictionary<string, string> GetChildNodeCollection(XmlElement xmlElement)
        {
            var xmlNodes = xmlElement.ChildNodes;
            IDictionary<string, string> childNodes = new Dictionary<string, string>();

            for (var ctr = 0; ctr < xmlNodes.Count; ctr++)
            {
                if (xmlNodes[ctr].ChildNodes.Count == 1)
                {
                    childNodes.Add(xmlNodes[ctr].Name.Trim(), xmlNodes[ctr].InnerText.Trim());
                }
            }

            return childNodes;
        }

        //------------------------------------------------------------------------------------------

        #endregion public methods
    }
}