using System.Xml.Serialization;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderGetSendingFees
{
    #region TempOrderGetSendingFees Response

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [XmlRootAttribute(Namespace = "http://www.w3.org/2003/05/soap-envelope", IsNullable = false, ElementName = "Envelope")]
    public class Response : EntityBase
    {
        /// <remarks/>
        public TempOrderGetSendingFeesRsEnvelopeBody Body { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [XmlRootAttribute(ElementName = "EnvelopeBody")]
    public class TempOrderGetSendingFeesRsEnvelopeBody
    {
        /// <remarks/>
        [XmlElementAttribute(Namespace = "http://tempuri.org/")]
        [XmlAttributeAttribute("")]
        public AcpTempOrderGetSendingFeesRequestResponse AcpTempOrderGetSendingFeesRequestResponse { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class AcpTempOrderGetSendingFeesRequestResponse
    {
        /// <remarks/>
        [XmlAttributeAttribute("ACP_TempOrderGetSendingFeesRequestResult")]
        public bool AcpTempOrderGetSendingFeesRequestResult { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("listSendingFees")]
        public AcpTempOrderGetSendingFeesRequestResponseListSendingFees ListSendingFees { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("result")]
        public byte Result { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class AcpTempOrderGetSendingFeesRequestResponseListSendingFees
    {
        /// <remarks/>
        public AcpTempOrderGetSendingFeesRequestResponseListSendingFeesAcpoSendingFee AcpoSendingFee { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class AcpTempOrderGetSendingFeesRequestResponseListSendingFeesAcpoSendingFee
    {
        /// <remarks/>
        public string FeeType { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("ID_SendingFee")]
        public sbyte IdSendingFee { get; set; }

        /// <remarks/>
        public object Label { get; set; }

        /// <remarks/>
        public object Comment { get; set; }

        /// <remarks/>
        public byte UnitAmount { get; set; }

        /// <remarks/>
        public byte GlobalAmount { get; set; }

        /// <remarks/>
        public byte Status { get; set; }

        /// <remarks/>
        public byte TypeCalcul { get; set; }

        /// <remarks/>
        public byte Nombre { get; set; }
    }

    #endregion TempOrderGetSendingFees Response
}