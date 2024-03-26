using System.Collections.Generic;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.OrderConfirm
{
    #region OrderConfirm Request Classes

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

        private OrderConfirmRQEnvelopeBody bodyField;

        public Request()
        {
            Body = new OrderConfirmRQEnvelopeBody();
            Header = new object();
        }

        /// <remarks/>
        public object Header
        {
            get => headerField;
            set => headerField = value;
        }

        /// <remarks/>
        public OrderConfirmRQEnvelopeBody Body
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
    public class OrderConfirmRQEnvelopeBody
    {
        private ACP_OrderConfirmRequest aCP_OrderConfirmRequestField;

        public OrderConfirmRQEnvelopeBody()
        {
            ACP_OrderConfirmRequest = new ACP_OrderConfirmRequest();
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_OrderConfirmRequest ACP_OrderConfirmRequest
        {
            get => aCP_OrderConfirmRequestField;
            set => aCP_OrderConfirmRequestField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_OrderConfirmRequest
    {
        private RequestCredentialsContext contextField;

        private string id_TemporaryOrderField;

        private ACP_OrderConfirmRequestListPaymentMode listPaymentModeField;

        private string id_IdentitymainField;

        private int id_IdentityConsumerField;

        private string iD_DeliveryAddressField;

        private string id_transcodeOrderField;

        private List<object> listSubReaboSeatToCancelField;

        public ACP_OrderConfirmRequest()
        {
            context = RequestCredentialsContext.GetContextInstance();
            listPaymentMode = new ACP_OrderConfirmRequestListPaymentMode();
            listSubReaboSeatToCancel = new List<object>();
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
        public ACP_OrderConfirmRequestListPaymentMode listPaymentMode
        {
            get => listPaymentModeField;
            set => listPaymentModeField = value;
        }

        /// <remarks/>
        public string id_Identitymain
        {
            get => id_IdentitymainField;
            set => id_IdentitymainField = value;
        }

        /// <summary>
        /// ID_Identity From Create Account Call
        /// </summary>
        public int id_IdentityConsumer
        {
            get => id_IdentityConsumerField;
            set => id_IdentityConsumerField = value;
        }

        /// <remarks/>
        public string ID_DeliveryAddress
        {
            get => iD_DeliveryAddressField;
            set => iD_DeliveryAddressField = value;
        }

        /// <remarks/>
        public string id_transcodeOrder
        {
            get => id_transcodeOrderField;
            set => id_transcodeOrderField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ACPO_CancelReaboSeat", IsNullable = false)]
        public List<object> listSubReaboSeatToCancel
        {
            get => listSubReaboSeatToCancelField;
            set => listSubReaboSeatToCancelField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_OrderConfirmRequestListPaymentMode
    {
        private ACP_OrderConfirmRequestListPaymentModeACPO_OrderPaymentMode aCPO_OrderPaymentModeField;

        public ACP_OrderConfirmRequestListPaymentMode()
        {
            ACPO_OrderPaymentMode = new ACP_OrderConfirmRequestListPaymentModeACPO_OrderPaymentMode();
        }

        /// <remarks/>
        public ACP_OrderConfirmRequestListPaymentModeACPO_OrderPaymentMode ACPO_OrderPaymentMode
        {
            get => aCPO_OrderPaymentModeField;
            set => aCPO_OrderPaymentModeField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_OrderConfirmRequestListPaymentModeACPO_OrderPaymentMode
    {
        private int iD_PaymentModeField;

        private decimal amountField;

        private object listEcheancesField;

        private string keyPayReferenceField;

        /// <remarks/>
        public int ID_PaymentMode
        {
            get => iD_PaymentModeField;
            set => iD_PaymentModeField = value;
        }

        /// <remarks/>
        public decimal Amount
        {
            get => amountField;
            set => amountField = value;
        }

        /// <remarks/>
        public object ListEcheances
        {
            get => listEcheancesField;
            set => listEcheancesField = value;
        }

        /// <remarks/>
        public string KeyPayReference
        {
            get => keyPayReferenceField;
            set => keyPayReferenceField = value;
        }
    }

    #endregion OrderConfirm Request Classes
}