using System.Xml.Serialization;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderGetDetail
{
    #region TempOrderGetDetail Request Classes

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
        public TempOrderGetDetailRqEnvelopeBody Body { get; set; }

        public Request()
        {
            Body = new TempOrderGetDetailRqEnvelopeBody();
            Header = new object();
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [XmlRootAttribute(ElementName = "EnvelopeBody")]
    public class TempOrderGetDetailRqEnvelopeBody
    {
        /// <remarks/>
        [XmlElementAttribute(Namespace = "http://tempuri.org/")]
        [XmlAttributeAttribute("ACP_TempOrderGetDetailRequest")]
        public AcpTempOrderGetDetailRequest AcpTempOrderGetDetailRequest { get; set; }

        public TempOrderGetDetailRqEnvelopeBody()
        {
            AcpTempOrderGetDetailRequest = new AcpTempOrderGetDetailRequest();
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class AcpTempOrderGetDetailRequest
    {
        public AcpTempOrderGetDetailRequest()
        {
            Context = RequestCredentialsContext.GetContextInstance();
        }

        /// <remarks/>
        [XmlAttributeAttribute("context")]
        public RequestCredentialsContext Context { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("id_TemporaryOrder")]
        public string IdTemporaryOrder { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("id_TemporaryOrderRow")]
        public string IdTemporaryOrderRow { get; set; }
    }

    #endregion TempOrderGetDetail Request Classes
}