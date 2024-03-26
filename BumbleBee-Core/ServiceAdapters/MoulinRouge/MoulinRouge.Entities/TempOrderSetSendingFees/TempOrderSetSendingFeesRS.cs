namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderSetSendingFees
{
    /*
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "EnvelopeBody")]
   * */

    #region TempOrderSetSendingFees Response

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Response
    {
        private TempOrderSetSendingFeesRSEnvelopeBody bodyField;

        /// <remarks/>
        public TempOrderSetSendingFeesRSEnvelopeBody Body
        {
            get => bodyField;
            set => bodyField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "EnvelopeBody")]
    public class TempOrderSetSendingFeesRSEnvelopeBody
    {
        private ACP_TempOrderSetSendingFeesRequestResponse aCP_TempOrderSetSendingFeesRequestResponseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_TempOrderSetSendingFeesRequestResponse ACP_TempOrderSetSendingFeesRequestResponse
        {
            get => aCP_TempOrderSetSendingFeesRequestResponseField;
            set => aCP_TempOrderSetSendingFeesRequestResponseField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_TempOrderSetSendingFeesRequestResponse
    {
        private bool aCP_TempOrderSetSendingFeesRequestResultField;

        private int resultField;

        /// <remarks/>
        public bool ACP_TempOrderSetSendingFeesRequestResult
        {
            get => aCP_TempOrderSetSendingFeesRequestResultField;
            set => aCP_TempOrderSetSendingFeesRequestResultField = value;
        }

        /// <remarks/>
        public int result
        {
            get => resultField;
            set => resultField = value;
        }
    }

    #endregion TempOrderSetSendingFees Response
}