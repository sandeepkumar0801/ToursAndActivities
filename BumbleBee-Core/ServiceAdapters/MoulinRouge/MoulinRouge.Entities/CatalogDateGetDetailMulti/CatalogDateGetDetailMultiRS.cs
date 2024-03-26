using System.Collections.Generic;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetDetailMulti
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Response : EntityBase
    {
        private CatalogDateGetDetailMultiEnvelopeBody bodyField;

        /// <remarks/>
        public CatalogDateGetDetailMultiEnvelopeBody Body
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
    public class CatalogDateGetDetailMultiEnvelopeBody
    {
        private ACP_CatalogDateGetDetailMultiRequestResponse aCP_CatalogDateGetDetailMultiRequestResponseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_CatalogDateGetDetailMultiRequestResponse ACP_CatalogDateGetDetailMultiRequestResponse
        {
            get => aCP_CatalogDateGetDetailMultiRequestResponseField;
            set => aCP_CatalogDateGetDetailMultiRequestResponseField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_CatalogDateGetDetailMultiRequestResponse
    {
        private bool aCP_CatalogDateGetDetailMultiRequestResultField;

        private List<ACP_CatalogDateGetDetailMultiRequestResponseACPO_CatalogDatePrice> listCatalogDatePriceField;

        private object listRulesField;

        private object listCatalogProductField;

        private List<ACP_CatalogDateGetDetailMultiRequestResponseACPO_CatalogDateLock> listCatalogDateLockField;

        private int resultField;

        /// <remarks/>
        public bool ACP_CatalogDateGetDetailMultiRequestResult
        {
            get => aCP_CatalogDateGetDetailMultiRequestResultField;
            set => aCP_CatalogDateGetDetailMultiRequestResultField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ACPO_CatalogDatePrice", IsNullable = false)]
        public List<ACP_CatalogDateGetDetailMultiRequestResponseACPO_CatalogDatePrice> ListCatalogDatePrice
        {
            get => listCatalogDatePriceField;
            set => listCatalogDatePriceField = value;
        }

        /// <remarks/>
        public object ListRules
        {
            get => listRulesField;
            set => listRulesField = value;
        }

        /// <remarks/>
        public object ListCatalogProduct
        {
            get => listCatalogProductField;
            set => listCatalogProductField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ACPO_CatalogDateLock", IsNullable = false)]
        public List<ACP_CatalogDateGetDetailMultiRequestResponseACPO_CatalogDateLock> ListCatalogDateLock
        {
            get => listCatalogDateLockField;
            set => listCatalogDateLockField = value;
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
    public class ACP_CatalogDateGetDetailMultiRequestResponseACPO_CatalogDatePrice
    {
        private int iD_CatalogDateField;

        private int iD_RateField;

        private int iD_CategoryField;

        private int iD_ContingentField;

        private int iD_BlocField;

        private int iD_FloorField;

        private decimal amountField;

        private List<decimal> amountDetailField;

        private int stockField;

        private int nbMiniField;

        private int nbMaxiField;

        private bool acceptDematField;

        /// <remarks/>
        public int ID_CatalogDate
        {
            get => iD_CatalogDateField;
            set => iD_CatalogDateField = value;
        }

        /// <remarks/>
        public int ID_Rate
        {
            get => iD_RateField;
            set => iD_RateField = value;
        }

        /// <remarks/>
        public int ID_Category
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
        public int ID_Bloc
        {
            get => iD_BlocField;
            set => iD_BlocField = value;
        }

        /// <remarks/>
        public int ID_Floor
        {
            get => iD_FloorField;
            set => iD_FloorField = value;
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
        public int Stock
        {
            get => stockField;
            set => stockField = value;
        }

        /// <remarks/>
        public int NbMini
        {
            get => nbMiniField;
            set => nbMiniField = value;
        }

        /// <remarks/>
        public int NbMaxi
        {
            get => nbMaxiField;
            set => nbMaxiField = value;
        }

        /// <remarks/>
        public bool AcceptDemat
        {
            get => acceptDematField;
            set => acceptDematField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_CatalogDateGetDetailMultiRequestResponseACPO_CatalogDateLock
    {
        private int iD_CatalogDateField;

        private string catalogDateTypeField;

        private int iD_CategoryField;

        private string categoryField;

        private int iD_ContingentRepasField;

        private string contingentRepasField;

        private int iD_ContingentRevueField;

        private string contingentRevueField;

        private bool lockRepasField;

        private bool lockRevueField;

        /// <remarks/>
        public int ID_CatalogDate
        {
            get => iD_CatalogDateField;
            set => iD_CatalogDateField = value;
        }

        /// <remarks/>
        public string CatalogDateType
        {
            get => catalogDateTypeField;
            set => catalogDateTypeField = value;
        }

        /// <remarks/>
        public int ID_Category
        {
            get => iD_CategoryField;
            set => iD_CategoryField = value;
        }

        /// <remarks/>
        public string Category
        {
            get => categoryField;
            set => categoryField = value;
        }

        /// <remarks/>
        public int ID_ContingentRepas
        {
            get => iD_ContingentRepasField;
            set => iD_ContingentRepasField = value;
        }

        /// <remarks/>
        public string ContingentRepas
        {
            get => contingentRepasField;
            set => contingentRepasField = value;
        }

        /// <remarks/>
        public int ID_ContingentRevue
        {
            get => iD_ContingentRevueField;
            set => iD_ContingentRevueField = value;
        }

        /// <remarks/>
        public string ContingentRevue
        {
            get => contingentRevueField;
            set => contingentRevueField = value;
        }

        /// <summary>
        /// For all the DINNER RATES you have to check LOCKREPAS.
        /// If the value is true this means the customer shouldn’t be able to make a booking.
        /// </summary>
        public bool LockRepas
        {
            get => lockRepasField;
            set => lockRepasField = value;
        }

        /// <summary>
        /// For the SHOW RATE you have to check LOCKREVUE.
        /// If the value is true this means the customer shouldn’t be able to make a booking.
        /// </summary>
        public bool LockRevue
        {
            get => lockRevueField;
            set => lockRevueField = value;
        }
    }
}