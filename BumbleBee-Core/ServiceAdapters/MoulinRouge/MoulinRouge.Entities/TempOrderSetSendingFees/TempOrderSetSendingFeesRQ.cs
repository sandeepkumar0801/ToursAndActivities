namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderSetSendingFees
{
    #region TempOrderSetSendingFees Request Classes

    /*
      [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
      [System.Xml.Serialization.XmlRootAttribute(ElementName = "EnvelopeBody")]
     * */

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Request
    {
        private object headerField;

        private TempOrderSetSendingFeesRQEnvelopeBody bodyField;

        public Request()
        {
            Body = new TempOrderSetSendingFeesRQEnvelopeBody();
            Header = new object();
        }

        /// <remarks/>
        public object Header
        {
            get => headerField;
            set => headerField = value;
        }

        /// <remarks/>
        public TempOrderSetSendingFeesRQEnvelopeBody Body
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
    public class TempOrderSetSendingFeesRQEnvelopeBody
    {
        private ACP_TempOrderSetSendingFeesRequest aCP_TempOrderSetSendingFeesRequestField;

        public TempOrderSetSendingFeesRQEnvelopeBody()
        {
            ACP_TempOrderSetSendingFeesRequest = new ACP_TempOrderSetSendingFeesRequest();
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_TempOrderSetSendingFeesRequest ACP_TempOrderSetSendingFeesRequest
        {
            get => aCP_TempOrderSetSendingFeesRequestField;
            set => aCP_TempOrderSetSendingFeesRequestField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_TempOrderSetSendingFeesRequest
    {
        private RequestCredentialsContext contextField;

        private string id_TemporaryOrderField;

        private ACP_TempOrderSetSendingFeesRequestSendingFee sendingFeeField;

        public ACP_TempOrderSetSendingFeesRequest()
        {
            contextField = RequestCredentialsContext.GetContextInstance();
            id_TemporaryOrder = "0";
            SendingFee = new ACP_TempOrderSetSendingFeesRequestSendingFee();
        }

        /// <remarks/>
        public RequestCredentialsContext context
        {
            get => contextField;
            set => contextField = value;
        }

        /// <remarks/>
        public string id_TemporaryOrder
        {
            get => id_TemporaryOrderField;
            set => id_TemporaryOrderField = value;
        }

        /// <remarks/>
        public ACP_TempOrderSetSendingFeesRequestSendingFee SendingFee
        {
            get => sendingFeeField;
            set => sendingFeeField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_TempOrderSetSendingFeesRequestSendingFee
    {
        private string feeTypeField;

        private int iD_SendingFeeField;

        private object labelField;

        private object commentField;

        private decimal unitAmountField;

        private decimal globalAmountField;

        private int statusField;

        private int typeCalculField;

        private int nombreField;

        public ACP_TempOrderSetSendingFeesRequestSendingFee()
        {
            Label = new object();
            Comment = new object();
        }

        /// <remarks/>
        public string FeeType
        {
            get => feeTypeField;
            set => feeTypeField = value;
        }

        /// <remarks/>
        public int ID_SendingFee
        {
            get => iD_SendingFeeField;
            set => iD_SendingFeeField = value;
        }

        /// <remarks/>
        public object Label
        {
            get => labelField;
            set => labelField = value;
        }

        /// <remarks/>
        public object Comment
        {
            get => commentField;
            set => commentField = value;
        }

        /// <remarks/>
        public decimal UnitAmount
        {
            get => unitAmountField;
            set => unitAmountField = value;
        }

        /// <remarks/>
        public decimal GlobalAmount
        {
            get => globalAmountField;
            set => globalAmountField = value;
        }

        /// <remarks/>
        public int Status
        {
            get => statusField;
            set => statusField = value;
        }

        /// <remarks/>
        public int TypeCalcul
        {
            get => typeCalculField;
            set => typeCalculField = value;
        }

        /// <remarks/>
        public int Nombre
        {
            get => nombreField;
            set => nombreField = value;
        }
    }

    #endregion TempOrderSetSendingFees Request Classes
}