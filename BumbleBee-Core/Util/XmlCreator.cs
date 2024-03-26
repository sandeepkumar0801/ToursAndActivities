using System.Collections.Generic;
using System.Xml;

namespace Util
{
    /// <summary>
    /// A class to create XML.
    /// </summary>
    public class XmlCreator
    {
        #region private members

        private XmlElement _mRootElement;
        private XmlDocument _mXmlDocument;
        private XmlDeclaration _mXmlDeclaration;

        #endregion private members

        #region constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public XmlCreator()
        {
            _mXmlDocument = new XmlDocument();
        }

        /// <summary>
        /// Constructor accepting an existing XmlDocument
        /// </summary>
        /// <param name="objXmlDoc"></param>
        public XmlCreator(XmlDocument objXmlDoc)
        {
            _mXmlDocument = objXmlDoc;
        }

        /// <summary>
        /// Constructor accepting an existing Xml data in string
        /// </summary>
        /// <param name="strXml"></param>
        public XmlCreator(string strXml)
        {
            _mXmlDocument = new XmlDocument();
            _mXmlDocument.LoadXml(strXml);
        }

        #endregion constructors

        /// <summary>
        /// Property to Set encoding (if other then utf-8). Called prior to the call to CreateRootNode().
        /// Calling it afterwords have no effect as root node has already been created.
        /// </summary>
        public string XmlEncoding { get; set; } = "UTF-8";

        /// <summary>
        /// property to get _xmlDoocument private field
        /// </summary>
        public XmlDocument XmlDocument => _mXmlDocument;

        /// <summary>
        /// Create XML root section with declaration.
        /// </summary>
        /// <param name="strRootNode"></param>
        public void CreateRootNode(string strRootNode)
        {
            // Write down the XML declaration
            _mXmlDeclaration = _mXmlDocument.CreateXmlDeclaration("1.0", XmlEncoding, null);

            // Create the root element
            _mRootElement = _mXmlDocument.CreateElement(strRootNode);
            _mXmlDocument.InsertBefore(_mXmlDeclaration, _mXmlDocument.DocumentElement);
            _mXmlDocument.AppendChild(_mRootElement);
        }

        /// <summary>
        /// Add a node to root
        /// </summary>
        /// <param name="node"></param>
        public void AddToRootNode(XmlElement node)
        {
            _mXmlDocument.DocumentElement?.PrependChild(node);
        }

        /// <summary>
        /// Creates a node for this document
        /// </summary>
        /// <param name="strNode"></param>
        /// <returns></returns>
        public XmlElement CreateNode(string strNode)
        {
            return _mXmlDocument.CreateElement(strNode);
        }

        /// <summary>
        /// Creates a CData node for this document
        /// </summary>
        /// <param name="strNode"></param>
        /// <returns></returns>
        public XmlCDataSection CreateCDataNode(string strNode)
        {
            return _mXmlDocument.CreateCDataSection(strNode);
        }

        /// <summary>
        /// Set an attribute and its value for the node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="strAttribName"></param>
        /// <param name="strAttribValue"></param>
        public void SetAttributeOfNode(XmlElement node, string strAttribName, string strAttribValue)
        {
            node.SetAttribute(strAttribName, strAttribValue);
        }

        /// <summary>
        /// Creates a text node for this document
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public XmlText CreateXmlText(string strText)
        {
            return _mXmlDocument.CreateTextNode(strText);
        }

        public void AddChildNode(XmlElement parentNode, XmlText textChildNode)
        {
            parentNode.AppendChild(_mXmlDocument.ImportNode(textChildNode, true));
        }

        public void AddChildNode(XmlElement parentNode, XmlElement childNode)
        {
            parentNode.AppendChild(_mXmlDocument.ImportNode(childNode, true));
        }

        public void AddChildNode(XmlElement parentNode, XmlCDataSection cDataChildNode)
        {
            parentNode.AppendChild(_mXmlDocument.ImportNode(cDataChildNode, true));
        }

        /// <summary>
        /// Adds all the elements present in the list to the given node.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNodeList"></param>
        public void AddChildNodeList(XmlElement parentNode, List<XmlElement> childNodeList)
        {
            foreach (var childNode in childNodeList)
                AddChildNode(parentNode, childNode);
        }

        /// <summary>
        /// Insert a blank node with given name.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeName"></param>
        public void InsertBlankNode(XmlElement parentNode, string nodeName)
        {
            if (parentNode.OwnerDocument == null) return;
            var newElement = parentNode.OwnerDocument.CreateElement(nodeName);
            parentNode.AppendChild(newElement);
        }

        public void ResetCreator()
        {
            _mRootElement = null;
            _mXmlDocument = null;
            _mXmlDeclaration = null;

            _mXmlDocument = new XmlDocument();
        }

        public void SaveXmlToFile(string strPath)
        {
            _mXmlDocument.Save(strPath);
        }

        public void RemoveNodes(List<XmlElement> lstNodes)
        {
            //lstNodes.ForEach(delegate(XmlElement objXmlElement) { RemoveNode(objXmlElement); }); //Using delegates.
            lstNodes.ForEach(RemoveNode); //Using lambda expressions.
        }

        public void RemoveNode(List<XmlElement> lstNodes, int intIndex)
        {
            var objXmlElement = lstNodes[intIndex];

            RemoveNode(objXmlElement);
        }

        public void RemoveNodes(string strNodePath)
        {
            var objNodeList = _mXmlDocument.SelectNodes(strNodePath);
            var lstNodes = XmlOperations.NodeListToListOfElements(objNodeList);

            RemoveNodes(lstNodes);
        }

        public void RemoveNode(string strNodePath, int intIndex)
        {
            var objNodeList = _mXmlDocument.SelectNodes(strNodePath);
            var lstNodes = XmlOperations.NodeListToListOfElements(objNodeList);

            RemoveNode(lstNodes, intIndex);
        }

        public void RemoveNode(XmlElement xmlNode)
        {
            xmlNode.ParentNode?.RemoveChild(xmlNode);
        }
    }
}