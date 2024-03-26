using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderGetDetail
{
    #region TempOrdergetDetail Response

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [XmlRootAttribute(Namespace = "http://www.w3.org/2003/05/soap-envelope", IsNullable = false, ElementName = "Envelope")]
    public class Response
    {
        /// <remarks/>
        public TempOrderGetDetailRsEnvelopeBody Body { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [XmlRootAttribute(ElementName = "EnvelopeBody")]
    public class TempOrderGetDetailRsEnvelopeBody
    {
        /// <remarks/>
        [XmlElementAttribute(Namespace = "http://tempuri.org/")]
        [XmlAttributeAttribute("ACP_TempOrderGetDetailRequestResponse")]
        public AcpTempOrderGetDetailRequestResponse AcpTempOrderGetDetailRequestResponse { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class AcpTempOrderGetDetailRequestResponse
    {
        /// <remarks/>
        [XmlAttributeAttribute("ACP_TempOrderGetDetailRequestResult")]
        public bool AcpTempOrderGetDetailRequestResult { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("listTempOrderRowEvent")]
        public AcpTempOrderGetDetailRequestResponseListTempOrderRowEvent ListTempOrderRowEvent { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("listTempOrderRowProduct")]
        public object ListTempOrderRowProduct { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("listTempOrderRowSub")]
        public object ListTempOrderRowSub { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("listTempOrderCustomers")]
        public object ListTempOrderCustomers { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("sendingFee")]
        public AcpTempOrderGetDetailRequestResponseSendingFee SendingFee { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("result")]
        public int Result { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class AcpTempOrderGetDetailRequestResponseListTempOrderRowEvent
    {
        /// <remarks/>
        [XmlAttributeAttribute("ACPO_TempOrderRowEvent")]
        public AcpTempOrderGetDetailRequestResponseListTempOrderRowEventAcpoTempOrderRowEvent AcpoTempOrderRowEvent { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class AcpTempOrderGetDetailRequestResponseListTempOrderRowEventAcpoTempOrderRowEvent
    {
        /// <remarks/>
        [XmlAttributeAttribute("ID_TemporaryOrderRow")]
        public string IdTemporaryOrderRow { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("ID_Catalog")]
        public int IdCatalog { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("ID_CatalogDate")]
        public ushort IdCatalogDate { get; set; }

        /// <remarks/>

        public System.DateTime DateStart { get; set; }

        /// <remarks/>
        public System.DateTime DateEnd { get; set; }

        /// <remarks/>
        public object VenueName { get; set; }

        /// <remarks/>
        public string CatalogName { get; set; }

        /// <remarks/>
        [XmlArrayItemAttribute("ACPO_AllocSeat", IsNullable = false)]
        public List<AcpTempOrderGetDetailRequestResponseListTempOrderRowEventAcpoTempOrderRowEventAcpoAllocSeat> ListAllocSeat { get; set; }

        /// <remarks/>
        public int Status { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("isContiguous")]
        public bool IsContiguous { get; set; }

        /// <remarks/>
        public AcpTempOrderGetDetailRequestResponseListTempOrderRowEventAcpoTempOrderRowEventInsuranceFee InsuranceFee { get; set; }

        /// <remarks/>
        public object ListProducts { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class AcpTempOrderGetDetailRequestResponseListTempOrderRowEventAcpoTempOrderRowEventAcpoAllocSeat
    {
        /// <remarks/>
        public int IdRate { get; set; }

        /// <remarks/>
        public int IdRule { get; set; }

        /// <remarks/>
        public decimal Amount { get; set; }

        /// <remarks/>
        public List<decimal> AmountDetail { get; set; }

        /// <remarks/>
        public AcpTempOrderGetDetailRequestResponseListTempOrderRowEventAcpoTempOrderRowEventAcpoAllocSeatSeatDetail SeatDetail { get; set; }

        /// <remarks/>
        public int IdIdentity { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class AcpTempOrderGetDetailRequestResponseListTempOrderRowEventAcpoTempOrderRowEventAcpoAllocSeatSeatDetail
    {
        /// <remarks/>
        public int IdSeat { get; set; }

        /// <remarks/>
        public ushort IdPhysicalSeat { get; set; }

        /// <remarks/>
        public int IdVenue { get; set; }

        /// <remarks/>
        public ushort IdCategory { get; set; }

        /// <remarks/>
        public int IdContingent { get; set; }

        /// <remarks/>
        public int IdDesignation { get; set; }

        /// <remarks/>
        public int IdDoor { get; set; }

        /// <remarks/>
        public int IdFloor { get; set; }

        /// <remarks/>
        public int IdBlock { get; set; }

        /// <remarks/>
        public int IdAccess { get; set; }

        /// <remarks/>
        public int IdTribune { get; set; }

        /// <remarks/>
        public int IdPhotoSeat { get; set; }

        /// <remarks/>
        public int Rank { get; set; }

        /// <remarks/>
        public int Seat { get; set; }

        /// <remarks/>
        public decimal X { get; set; }

        /// <remarks/>
        public decimal Y { get; set; }

        /// <remarks/>
        public int Type { get; set; }

        /// <remarks/>
        public int Rotation { get; set; }

        /// <remarks/>
        public int Status { get; set; }

        /// <remarks/>
        public bool IsNumbered { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class AcpTempOrderGetDetailRequestResponseListTempOrderRowEventAcpoTempOrderRowEventInsuranceFee
    {
        /// <remarks/>
        public int IdOrderRow { get; set; }

        /// <remarks/>
        public int IdInsuranceFee { get; set; }

        /// <remarks/>
        public object Label { get; set; }

        /// <remarks/>
        public object Comment { get; set; }

        /// <remarks/>
        public int Amount { get; set; }

        /// <remarks/>
        public int IsAccepted { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class AcpTempOrderGetDetailRequestResponseSendingFee
    {
        /// <remarks/>
        public object FeeType { get; set; }

        /// <remarks/>
        public int IdSendingFee { get; set; }

        /// <remarks/>
        public object Label { get; set; }

        /// <remarks/>
        public object Comment { get; set; }

        /// <remarks/>
        public int UnitAmount { get; set; }

        /// <remarks/>
        public int GlobalAmount { get; set; }

        /// <remarks/>
        public int Status { get; set; }

        /// <remarks/>
        public int TypeCalcul { get; set; }

        /// <remarks/>
        public int Nombre { get; set; }
    }

    #endregion TempOrdergetDetail Response
}