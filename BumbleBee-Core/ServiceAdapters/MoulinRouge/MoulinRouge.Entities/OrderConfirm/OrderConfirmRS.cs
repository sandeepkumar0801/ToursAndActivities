namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.OrderConfirm
{
    /*
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "EnvelopeBody")]
   * */

    #region OrderConfirm Response

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Response
    {
        private OrderConfirmRSEnvelopeBody bodyField;

        /// <remarks/>
        public OrderConfirmRSEnvelopeBody Body
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
    public class OrderConfirmRSEnvelopeBody
    {
        private ACP_OrderConfirmRequestResponse aCP_OrderConfirmRequestResponseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_OrderConfirmRequestResponse ACP_OrderConfirmRequestResponse
        {
            get => aCP_OrderConfirmRequestResponseField;
            set => aCP_OrderConfirmRequestResponseField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_OrderConfirmRequestResponse
    {
        private bool aCP_OrderConfirmRequestResultField;

        private string id_OrderField;

        private int id_orderMRField;

        private ACP_OrderConfirmRequestResponseListOrderRow listOrderRowField;

        private int resultField;

        /// <remarks/>
        public bool ACP_OrderConfirmRequestResult
        {
            get => aCP_OrderConfirmRequestResultField;
            set => aCP_OrderConfirmRequestResultField = value;
        }

        /// <remarks/>
        public string id_Order
        {
            get => id_OrderField;
            set => id_OrderField = value;
        }

        /// <remarks/>
        public int id_orderMR
        {
            get => id_orderMRField;
            set => id_orderMRField = value;
        }

        /// <remarks/>
        public ACP_OrderConfirmRequestResponseListOrderRow listOrderRow
        {
            get => listOrderRowField;
            set => listOrderRowField = value;
        }

        /// <remarks/>
        public int result
        {
            get => resultField;
            set => resultField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_OrderConfirmRequestResponseListOrderRow
    {
        private ACP_OrderConfirmRequestResponseListOrderRowACPO_ConfirmOrderRow aCPO_ConfirmOrderRowField;

        /// <remarks/>
        public ACP_OrderConfirmRequestResponseListOrderRowACPO_ConfirmOrderRow ACPO_ConfirmOrderRow
        {
            get => aCPO_ConfirmOrderRowField;
            set => aCPO_ConfirmOrderRowField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_OrderConfirmRequestResponseListOrderRowACPO_ConfirmOrderRow
    {
        private string iD_OrderRowField;

        private int iD_PackageRowField;

        private string iD_TemporaryOrderRowField;

        private string eticketGUIDField;

        private string[] listeticketGUIDField;

        private ACP_OrderConfirmRequestResponseListOrderRowACPO_ConfirmOrderRowACPO_ConfirmSeat[] listBarCodesField;

        /// <remarks/>
        public string ID_OrderRow
        {
            get => iD_OrderRowField;
            set => iD_OrderRowField = value;
        }

        /// <remarks/>
        public int ID_PackageRow
        {
            get => iD_PackageRowField;
            set => iD_PackageRowField = value;
        }

        /// <remarks/>
        public string ID_TemporaryOrderRow
        {
            get => iD_TemporaryOrderRowField;
            set => iD_TemporaryOrderRowField = value;
        }

        /// <remarks/>
        public string eticketGUID
        {
            get => eticketGUIDField;
            set => eticketGUIDField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public string[] listeticketGUID
        {
            get => listeticketGUIDField;
            set => listeticketGUIDField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ACPO_ConfirmSeat", IsNullable = false)]
        public ACP_OrderConfirmRequestResponseListOrderRowACPO_ConfirmOrderRowACPO_ConfirmSeat[] listBarCodes
        {
            get => listBarCodesField;
            set => listBarCodesField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_OrderConfirmRequestResponseListOrderRowACPO_ConfirmOrderRowACPO_ConfirmSeat
    {
        private string iD_SeatField;

        private string barCodeField;

        private string fiscalNumberField;

        /// <remarks/>
        public string ID_Seat
        {
            get => iD_SeatField;
            set => iD_SeatField = value;
        }

        /// <remarks/>
        public string BarCode
        {
            get => barCodeField;
            set => barCodeField = value;
        }

        /// <remarks/>
        public string FiscalNumber
        {
            get => fiscalNumberField;
            set => fiscalNumberField = value;
        }
    }

    #endregion OrderConfirm Response
}