using System.Collections.Generic;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.AllocSeatsAutomatic
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2003/05/soap-envelope", IsNullable = false, ElementName = "Envelope")]
    public class Response
    {
        private AllocSeatsAutomaticRSEnvelopeBody bodyField;

        /// <remarks/>
        public AllocSeatsAutomaticRSEnvelopeBody Body
        {
            get => bodyField;
            set => bodyField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "EnvelopeBody")]
    public class AllocSeatsAutomaticRSEnvelopeBody
    {
        private ACP_AllocSeatsAutomaticRequestResponse aCP_AllocSeatsAutomaticRequestResponseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_AllocSeatsAutomaticRequestResponse ACP_AllocSeatsAutomaticRequestResponse
        {
            get => aCP_AllocSeatsAutomaticRequestResponseField;
            set => aCP_AllocSeatsAutomaticRequestResponseField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_AllocSeatsAutomaticRequestResponse
    {
        private bool aCP_AllocSeatsAutomaticRequestResultField;

        private string id_TemporaryOrderField;

        private bool isContiguousField;

        private ACP_AllocSeatsAutomaticRequestResponseListAllocResponse listAllocResponseField;

        private int resultField;

        /// <remarks/>
        public bool ACP_AllocSeatsAutomaticRequestResult
        {
            get => aCP_AllocSeatsAutomaticRequestResultField;
            set => aCP_AllocSeatsAutomaticRequestResultField = value;
        }

        /// <remarks/>
        public string id_TemporaryOrder
        {
            get => id_TemporaryOrderField;
            set => id_TemporaryOrderField = value;
        }

        /// <remarks/>
        public bool isContiguous
        {
            get => isContiguousField;
            set => isContiguousField = value;
        }

        /// <remarks/>
        public ACP_AllocSeatsAutomaticRequestResponseListAllocResponse listAllocResponse
        {
            get => listAllocResponseField;
            set => listAllocResponseField = value;
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
    public class ACP_AllocSeatsAutomaticRequestResponseListAllocResponse
    {
        private ACP_AllocSeatsAutomaticRequestResponseListAllocResponseACPO_AllocSAResponse aCPO_AllocSAResponseField;

        /// <remarks/>
        public ACP_AllocSeatsAutomaticRequestResponseListAllocResponseACPO_AllocSAResponse ACPO_AllocSAResponse
        {
            get => aCPO_AllocSAResponseField;
            set => aCPO_AllocSAResponseField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_AllocSeatsAutomaticRequestResponseListAllocResponseACPO_AllocSAResponse
    {
        private bool isContiguousField;

        private string iD_TemporaryOrderRowField;

        private List<ACP_AllocSeatsAutomaticRequestResponseListAllocResponseACPO_AllocSAResponseACPO_AllocSeat> listAllocSeatField;

        /// <remarks/>
        public bool isContiguous
        {
            get => isContiguousField;
            set => isContiguousField = value;
        }

        /// <remarks/>
        public string ID_TemporaryOrderRow
        {
            get => iD_TemporaryOrderRowField;
            set => iD_TemporaryOrderRowField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ACPO_AllocSeat", IsNullable = false)]
        public List<ACP_AllocSeatsAutomaticRequestResponseListAllocResponseACPO_AllocSAResponseACPO_AllocSeat> ListAllocSeat
        {
            get => listAllocSeatField;
            set => listAllocSeatField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_AllocSeatsAutomaticRequestResponseListAllocResponseACPO_AllocSAResponseACPO_AllocSeat
    {
        private int iD_RateField;

        private int iD_RuleField;

        private decimal amountField;

        private List<decimal> amountDetailField;

        private ACP_AllocSeatsAutomaticRequestResponseListAllocResponseACPO_AllocSAResponseACPO_AllocSeatSeat_Detail seat_DetailField;

        private int iD_IdentityField;

        /// <remarks/>
        public int ID_Rate
        {
            get => iD_RateField;
            set => iD_RateField = value;
        }

        /// <remarks/>
        public int ID_Rule
        {
            get => iD_RuleField;
            set => iD_RuleField = value;
        }

        /// <remarks/>
        public decimal Amount
        {
            get => amountField;
            set => amountField = value;
        }

        /// <remarks/>
        public List<decimal> AmountDetail
        {
            get => amountDetailField;
            set => amountDetailField = value;
        }

        /// <remarks/>
        public ACP_AllocSeatsAutomaticRequestResponseListAllocResponseACPO_AllocSAResponseACPO_AllocSeatSeat_Detail Seat_Detail
        {
            get => seat_DetailField;
            set => seat_DetailField = value;
        }

        /// <remarks/>
        public int ID_Identity
        {
            get => iD_IdentityField;
            set => iD_IdentityField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_AllocSeatsAutomaticRequestResponseListAllocResponseACPO_AllocSAResponseACPO_AllocSeatSeat_Detail
    {
        private int iD_SeatField;

        private ushort iD_PhysicalSeatField;

        private int iD_VenueField;

        private ushort iD_CategoryField;

        private int iD_ContingentField;

        private int iD_DesignationField;

        private int iD_DoorField;

        private int iD_FloorField;

        private int iD_BlockField;

        private int iD_AccessField;

        private int iD_TribuneField;

        private int iD_PhotoSeatField;

        private int rankField;

        private int seatField;

        private decimal xField;

        private decimal yField;

        private int typeField;

        private int rotationField;

        private int statusField;

        private bool isNumberedField;

        /// <remarks/>
        public int ID_Seat
        {
            get => iD_SeatField;
            set => iD_SeatField = value;
        }

        /// <remarks/>
        public ushort ID_PhysicalSeat
        {
            get => iD_PhysicalSeatField;
            set => iD_PhysicalSeatField = value;
        }

        /// <remarks/>
        public int ID_Venue
        {
            get => iD_VenueField;
            set => iD_VenueField = value;
        }

        /// <remarks/>
        public ushort ID_Category
        {
            get => iD_CategoryField;
            set => iD_CategoryField = value;
        }

        /// <remarks/>
        public int ID_Contingent
        {
            get => iD_ContingentField;
            set => iD_ContingentField = value;
        }

        /// <remarks/>
        public int ID_Designation
        {
            get => iD_DesignationField;
            set => iD_DesignationField = value;
        }

        /// <remarks/>
        public int ID_Door
        {
            get => iD_DoorField;
            set => iD_DoorField = value;
        }

        /// <remarks/>
        public int ID_Floor
        {
            get => iD_FloorField;
            set => iD_FloorField = value;
        }

        /// <remarks/>
        public int ID_Block
        {
            get => iD_BlockField;
            set => iD_BlockField = value;
        }

        /// <remarks/>
        public int ID_Access
        {
            get => iD_AccessField;
            set => iD_AccessField = value;
        }

        /// <remarks/>
        public int ID_Tribune
        {
            get => iD_TribuneField;
            set => iD_TribuneField = value;
        }

        /// <remarks/>
        public int ID_PhotoSeat
        {
            get => iD_PhotoSeatField;
            set => iD_PhotoSeatField = value;
        }

        /// <remarks/>
        public int Rank
        {
            get => rankField;
            set => rankField = value;
        }

        /// <remarks/>
        public int Seat
        {
            get => seatField;
            set => seatField = value;
        }

        /// <remarks/>
        public decimal X
        {
            get => xField;
            set => xField = value;
        }

        /// <remarks/>
        public decimal Y
        {
            get => yField;
            set => yField = value;
        }

        /// <remarks/>
        public int Type
        {
            get => typeField;
            set => typeField = value;
        }

        /// <remarks/>
        public int Rotation
        {
            get => rotationField;
            set => rotationField = value;
        }

        /// <remarks/>
        public int Status
        {
            get => statusField;
            set => statusField = value;
        }

        /// <remarks/>
        public bool isNumbered
        {
            get => isNumberedField;
            set => isNumberedField = value;
        }
    }
}