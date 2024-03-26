namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.AllocSeatsAutomatic
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2003/05/soap-envelope", IsNullable = false, ElementName = "Envelope")]
    public class Request
    {
        private object headerField;

        private AllocSeatsAutomaticRQEnvelopeBody bodyField;

        public Request()
        {
            Body = new AllocSeatsAutomaticRQEnvelopeBody();
            Header = new object();
        }

        /// <remarks/>
        public object Header
        {
            get => headerField;
            set => headerField = value;
        }

        /// <remarks/>
        public AllocSeatsAutomaticRQEnvelopeBody Body
        {
            get => bodyField;
            set => bodyField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "Body")]
    public class AllocSeatsAutomaticRQEnvelopeBody
    {
        private ACP_AllocSeatsAutomaticRequest aCP_AllocSeatsAutomaticRequestField;

        public AllocSeatsAutomaticRQEnvelopeBody()
        {
            ACP_AllocSeatsAutomaticRequest = new ACP_AllocSeatsAutomaticRequest();
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_AllocSeatsAutomaticRequest ACP_AllocSeatsAutomaticRequest
        {
            get => aCP_AllocSeatsAutomaticRequestField;
            set => aCP_AllocSeatsAutomaticRequestField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_AllocSeatsAutomaticRequest
    {
        private RequestCredentialsContext contextField;

        private int id_CatalogField;

        private int id_CatalogDateField;

        private int id_PackageField;

        private int id_venueField;

        private ACP_AllocSeatsAutomaticRequestListAllocRequest listAllocRequestField;

        private int id_TemporaryOrderField;

        public ACP_AllocSeatsAutomaticRequest()
        {
            contextField = RequestCredentialsContext.GetContextInstance();
            listAllocRequestField = new ACP_AllocSeatsAutomaticRequestListAllocRequest();
        }

        /// <remarks/>
        public RequestCredentialsContext context
        {
            get => contextField;
            set => contextField = value;
        }

        /// <remarks/>
        public int id_Catalog
        {
            get => id_CatalogField;
            set => id_CatalogField = value;
        }

        /// <remarks/>
        public int id_CatalogDate
        {
            get => id_CatalogDateField;
            set => id_CatalogDateField = value;
        }

        /// <remarks/>
        public int id_Package
        {
            get => id_PackageField;
            set => id_PackageField = value;
        }

        /// <remarks/>
        public int id_venue
        {
            get => id_venueField;
            set => id_venueField = value;
        }

        /// <remarks/>
        public ACP_AllocSeatsAutomaticRequestListAllocRequest listAllocRequest
        {
            get => listAllocRequestField;
            set => listAllocRequestField = value;
        }

        /// <remarks/>
        public int id_TemporaryOrder
        {
            get => id_TemporaryOrderField;
            set => id_TemporaryOrderField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_AllocSeatsAutomaticRequestContext
    {
        private string kEY_DatabaseField;

        private string loginField;

        private string passwordField;

        private int interfaceField;

        private int iD_WitheLabelField;

        /// <remarks/>
        public string KEY_Database
        {
            get => kEY_DatabaseField;
            set => kEY_DatabaseField = value;
        }

        /// <remarks/>
        public string Login
        {
            get => loginField;
            set => loginField = value;
        }

        /// <remarks/>
        public string Password
        {
            get => passwordField;
            set => passwordField = value;
        }

        /// <remarks/>
        public int Interface
        {
            get => interfaceField;
            set => interfaceField = value;
        }

        /// <remarks/>
        public int ID_WitheLabel
        {
            get => iD_WitheLabelField;
            set => iD_WitheLabelField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_AllocSeatsAutomaticRequestListAllocRequest
    {
        private ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequest aCPO_AllocSARequestField;

        public ACP_AllocSeatsAutomaticRequestListAllocRequest()
        {
            aCPO_AllocSARequestField = new ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequest();
        }

        /// <remarks/>
        public ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequest ACPO_AllocSARequest
        {
            get => aCPO_AllocSARequestField;
            set => aCPO_AllocSARequestField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequest
    {
        private ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Category listID_CategoryField;

        private ListID_Contingent listID_ContingentField;

        private ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Bloc listID_BlocField;

        private ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Floor listID_FloorField;

        private ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListQuantity listQuantityField;

        public ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequest()
        {
            listID_BlocField = new ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Bloc();
            listID_CategoryField = new ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Category();
            listID_FloorField = new ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Floor();
            listID_ContingentField = new ListID_Contingent();
            listQuantityField = new ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListQuantity();
        }

        /// <remarks/>
        public ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Category ListID_Category
        {
            get => listID_CategoryField;
            set => listID_CategoryField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
        public ListID_Contingent ListID_Contingent
        {
            get => listID_ContingentField;
            set => listID_ContingentField = value;
        }

        /// <remarks/>
        public ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Bloc ListID_Bloc
        {
            get => listID_BlocField;
            set => listID_BlocField = value;
        }

        /// <remarks/>
        public ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Floor ListID_Floor
        {
            get => listID_FloorField;
            set => listID_FloorField = value;
        }

        /// <remarks/>
        public ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListQuantity ListQuantity
        {
            get => listQuantityField;
            set => listQuantityField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Category
    {
        private int intField;

        [System.Xml.Serialization.XmlElement("int")]
        public int Int
        {
            get => intField;
            set => intField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class ListID_Contingent
    {
        private int intField;

        [System.Xml.Serialization.XmlElement("int")]
        public int Int
        {
            get => intField;
            set => intField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public partial class ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Bloc
    {
        private int intField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElement("int")]
        public int Int
        {
            get => intField;
            set => intField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListID_Floor
    {
        private int intField;

        [System.Xml.Serialization.XmlElement("int")]
        public int Int
        {
            get => intField;
            set => intField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListQuantity
    {
        private ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListQuantityACPO_AllocSARequestQuantity aCPO_AllocSARequestQuantityField;

        public ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListQuantity()
        {
            aCPO_AllocSARequestQuantityField = new ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListQuantityACPO_AllocSARequestQuantity();
        }

        /// <remarks/>
        public ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListQuantityACPO_AllocSARequestQuantity ACPO_AllocSARequestQuantity
        {
            get => aCPO_AllocSARequestQuantityField;
            set => aCPO_AllocSARequestQuantityField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_AllocSeatsAutomaticRequestListAllocRequestACPO_AllocSARequestListQuantityACPO_AllocSARequestQuantity
    {
        private int iD_RateField;

        private int quantityField;

        /// <remarks/>
        public int ID_Rate
        {
            get => iD_RateField;
            set => iD_RateField = value;
        }

        /// <remarks/>
        public int Quantity
        {
            get => quantityField;
            set => quantityField = value;
        }
    }
}