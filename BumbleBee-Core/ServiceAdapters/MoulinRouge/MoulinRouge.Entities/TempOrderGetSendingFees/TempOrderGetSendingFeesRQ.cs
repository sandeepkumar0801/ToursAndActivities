using System.Xml.Serialization;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderGetSendingFees
{
    #region TempOrderGetSendingFees Request Classes

    /*
      [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
      [System.Xml.Serialization.XmlRootAttribute(ElementName = "EnvelopeBody")]
     * */

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [XmlRootAttribute(Namespace = "http://www.w3.org/2003/05/soap-envelope", IsNullable = false, ElementName = "Envelope")]
    public class Request
    {
        /// <remarks/>
        public object Header { get; set; }

        /// <remarks/>
        public TempOrderGetSendingFeesRqEnvelopeBody Body { get; set; }

        public Request()
        {
            Body = new TempOrderGetSendingFeesRqEnvelopeBody();
            Header = new object();
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [XmlRootAttribute(ElementName = "EnvelopeBody")]
    public class TempOrderGetSendingFeesRqEnvelopeBody
    {
        /// <remarks/>
        [XmlElementAttribute(Namespace = "http://tempuri.org/")]
        [XmlAttributeAttribute("ACP_TempOrderGetSendingFeesRequest")]
        public AcpTempOrderGetSendingFeesRequest AcpTempOrderGetSendingFeesRequest { get; set; }

        public TempOrderGetSendingFeesRqEnvelopeBody()
        {
            AcpTempOrderGetSendingFeesRequest = new AcpTempOrderGetSendingFeesRequest();
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class AcpTempOrderGetSendingFeesRequest
    {
        public AcpTempOrderGetSendingFeesRequest()
        {
            Context = RequestCredentialsContext.GetContextInstance();
        }

        /// <remarks/>
        [XmlAttributeAttribute("context")]
        public RequestCredentialsContext Context { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("id_TemporaryOrder")]
        public string IdTemporaryOrder { get; set; }
    }

    #endregion TempOrderGetSendingFees Request Classes
}