using System.Collections.Generic;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetList
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Response : EntityBase
    {
        private CatalogDateGetListResponseEnvelopeBody bodyField;

        /// <remarks/>
        public CatalogDateGetListResponseEnvelopeBody Body
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
    public class CatalogDateGetListResponseEnvelopeBody
    {
        private ACP_CatalogDateGetListRequestResponse aCP_CatalogDateGetListRequestResponseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_CatalogDateGetListRequestResponse ACP_CatalogDateGetListRequestResponse
        {
            get => aCP_CatalogDateGetListRequestResponseField;
            set => aCP_CatalogDateGetListRequestResponseField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_CatalogDateGetListRequestResponse
    {
        private bool aCP_CatalogDateGetListRequestResultField;

        private List<ACP_CatalogDateGetListRequestResponseACPO_CatalogDate> catalogDateListField;

        private int resultField;

        /// <remarks/>
        public bool ACP_CatalogDateGetListRequestResult
        {
            get => aCP_CatalogDateGetListRequestResultField;
            set => aCP_CatalogDateGetListRequestResultField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ACPO_CatalogDate", IsNullable = false)]
        public List<ACP_CatalogDateGetListRequestResponseACPO_CatalogDate> catalogDateList
        {
            get => catalogDateListField;
            set => catalogDateListField = value;
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
    public class ACP_CatalogDateGetListRequestResponseACPO_CatalogDate
    {
        private int iD_CatalogDateField;

        private System.DateTime dateStartField;

        private System.DateTime dateEndField;

        private System.DateTime dateOuvertureField;

        private System.DateTime dateFermetureField;

        private string venueNameField;

        private int iD_VenueField;

        private object venue_AddressField;

        private object commentField;

        private int stockField;

        private bool accept_eTicketField;

        private decimal tVAField;

        private int typeNumerotationField;

        private int typeSeanceField;

        private List<ACP_CatalogDateGetListRequestResponseACPO_CatalogDateACPO_StockDetail> listStockDetailField;

        /// <remarks/>
        public int ID_CatalogDate
        {
            get => iD_CatalogDateField;
            set => iD_CatalogDateField = value;
        }

        /// <remarks/>
        public System.DateTime DateStart
        {
            get => dateStartField;
            set => dateStartField = value;
        }

        /// <remarks/>
        public System.DateTime DateEnd
        {
            get => dateEndField;
            set => dateEndField = value;
        }

        /// <remarks/>
        public System.DateTime DateOuverture
        {
            get => dateOuvertureField;
            set => dateOuvertureField = value;
        }

        /// <remarks/>
        public System.DateTime DateFermeture
        {
            get => dateFermetureField;
            set => dateFermetureField = value;
        }

        /// <remarks/>
        public string VenueName
        {
            get => venueNameField;
            set => venueNameField = value;
        }

        /// <remarks/>
        public int ID_Venue
        {
            get => iD_VenueField;
            set => iD_VenueField = value;
        }

        /// <remarks/>
        public object Venue_Address
        {
            get => venue_AddressField;
            set => venue_AddressField = value;
        }

        /// <remarks/>
        public object Comment
        {
            get => commentField;
            set => commentField = value;
        }

        /// <remarks/>
        public int Stock
        {
            get => stockField;
            set => stockField = value;
        }

        /// <remarks/>
        public bool Accept_eTicket
        {
            get => accept_eTicketField;
            set => accept_eTicketField = value;
        }

        /// <remarks/>
        public decimal TVA
        {
            get => tVAField;
            set => tVAField = value;
        }

        /// <remarks/>
        public int TypeNumerotation
        {
            get => typeNumerotationField;
            set => typeNumerotationField = value;
        }

        /// <remarks/>
        public int TypeSeance
        {
            get => typeSeanceField;
            set => typeSeanceField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ACPO_StockDetail", IsNullable = false)]
        public List<ACP_CatalogDateGetListRequestResponseACPO_CatalogDateACPO_StockDetail> ListStockDetail
        {
            get => listStockDetailField;
            set => listStockDetailField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_CatalogDateGetListRequestResponseACPO_CatalogDateACPO_StockDetail
    {
        private int iD_CategoryField;

        private uint iD_ContingentField;

        private int iD_BlocField;

        private uint iD_FloorField;

        private int stockField;

        /// <remarks/>
        public int ID_Category
        {
            get => iD_CategoryField;
            set => iD_CategoryField = value;
        }

        /// <remarks/>
        public uint ID_Contingent
        {
            get => iD_ContingentField;
            set => iD_ContingentField = value;
        }

        /// <remarks/>
        public int ID_Bloc
        {
            get => iD_BlocField;
            set => iD_BlocField = value;
        }

        /// <remarks/>
        public uint ID_Floor
        {
            get => iD_FloorField;
            set => iD_FloorField = value;
        }

        /// <remarks/>
        public int Stock
        {
            get => stockField;
            set => stockField = value;
        }
    }
}