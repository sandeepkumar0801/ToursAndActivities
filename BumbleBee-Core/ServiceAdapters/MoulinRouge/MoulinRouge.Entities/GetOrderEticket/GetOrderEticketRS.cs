namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.GetOrderEticket
{
    #region GetOrderEticket Response

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Response
    {
        private GetOrderEticketRSEnvelopeBody bodyField;

        public Response()
        {
            Body = new GetOrderEticketRSEnvelopeBody();
        }

        /// <remarks/>
        public GetOrderEticketRSEnvelopeBody Body
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
    public class GetOrderEticketRSEnvelopeBody
    {
        private ACP_GetOrderEticketRequestResponse aCP_GetOrderEticketRequestResponseField;

        public GetOrderEticketRSEnvelopeBody()
        {
            ACP_GetOrderEticketRequestResponse = new ACP_GetOrderEticketRequestResponse();
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_GetOrderEticketRequestResponse ACP_GetOrderEticketRequestResponse
        {
            get => aCP_GetOrderEticketRequestResponseField;
            set => aCP_GetOrderEticketRequestResponseField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_GetOrderEticketRequestResponse
    {
        private bool aCP_GetOrderEticketRequestResultField;

        private byte[] eTicketBytesField;

        private int resultField;

        /// <remarks/>
        public bool ACP_GetOrderEticketRequestResult
        {
            get => aCP_GetOrderEticketRequestResultField;
            set => aCP_GetOrderEticketRequestResultField = value;
        }

        /// <remarks/>
        public byte[] eTicketBytes
        {
            get => eTicketBytesField;
            set => eTicketBytesField = value;
        }

        /// <remarks/>
        public int result
        {
            get => resultField;
            set => resultField = value;
        }
    }

    #endregion GetOrderEticket Response
}