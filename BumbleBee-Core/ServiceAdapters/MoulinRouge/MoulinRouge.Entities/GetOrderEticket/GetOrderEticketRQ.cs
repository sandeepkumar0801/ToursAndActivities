namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.GetOrderEticket
{
    #region GetOrderEticket Request Classes

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Request
    {
        private object headerField;

        private GetOrderEticketRQEnvelopeBody bodyField;

        public Request()
        {
            Body = new GetOrderEticketRQEnvelopeBody();
            Header = new object();
        }

        /// <remarks/>
        public object Header
        {
            get => headerField;
            set => headerField = value;
        }

        /// <remarks/>
        public GetOrderEticketRQEnvelopeBody Body
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
    public class GetOrderEticketRQEnvelopeBody
    {
        private ACP_GetOrderEticketRequest aCP_GetOrderEticketRequestField;

        public GetOrderEticketRQEnvelopeBody()
        {
            ACP_GetOrderEticketRequest = new ACP_GetOrderEticketRequest();
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_GetOrderEticketRequest ACP_GetOrderEticketRequest
        {
            get => aCP_GetOrderEticketRequestField;
            set => aCP_GetOrderEticketRequestField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_GetOrderEticketRequest
    {
        private string iD_OrderField;

        private string gUIDField;

        private RequestCredentialsContext contextField;

        public ACP_GetOrderEticketRequest()
        {
            contextField = RequestCredentialsContext.GetContextInstance();
            GUID = string.Empty;
            ID_Order = "0";
        }

        /// <remarks/>
        public RequestCredentialsContext context
        {
            get => contextField;
            set => contextField = value;
        }

        /// <remarks/>
        public string ID_Order
        {
            get => iD_OrderField;
            set => iD_OrderField = value;
        }

        /// <remarks/>
        public string GUID
        {
            get => gUIDField;
            set => gUIDField = value;
        }
    }

    #endregion GetOrderEticket Request Classes
}