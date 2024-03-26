namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CreateAccount
{
    /*
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "EnvelopeBody")]
   * */

    #region CreateAccount Response

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Response : EntityBase
    {
        private CreateAccountRSEnvelopeBody bodyField;

        /// <remarks/>
        public CreateAccountRSEnvelopeBody Body
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
    public class CreateAccountRSEnvelopeBody
    {
        private ACP_CreateAccountRequestResponse aCP_CreateAccountRequestResponseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_CreateAccountRequestResponse ACP_CreateAccountRequestResponse
        {
            get => aCP_CreateAccountRequestResponseField;
            set => aCP_CreateAccountRequestResponseField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_CreateAccountRequestResponse
    {
        private bool aCP_CreateAccountRequestResultField;

        private ACP_CreateAccountRequestResponseIdentity identityField;

        private int id_loginField;

        private int resultField;

        /// <remarks/>
        public bool ACP_CreateAccountRequestResult
        {
            get => aCP_CreateAccountRequestResultField;
            set => aCP_CreateAccountRequestResultField = value;
        }

        /// <remarks/>
        public ACP_CreateAccountRequestResponseIdentity identity
        {
            get => identityField;
            set => identityField = value;
        }

        /// <remarks/>
        public int id_login
        {
            get => id_loginField;
            set => id_loginField = value;
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
    public class ACP_CreateAccountRequestResponseIdentity
    {
        private int iD_IdentityField;

        private int iD_ExterneField;

        private bool isCompanyField;

        private string nameField;

        private string firstNameField;

        private System.DateTime birthDateField;

        private int iD_AppellationField;

        private object listIdentityAddressField;

        private object listIdentityCommunicationField;

        private int sIRETField;

        private bool forceUpdateCRMField;

        /// <remarks/>
        public int ID_Identity
        {
            get => iD_IdentityField;
            set => iD_IdentityField = value;
        }

        /// <remarks/>
        public int ID_Externe
        {
            get => iD_ExterneField;
            set => iD_ExterneField = value;
        }

        /// <remarks/>
        public bool isCompany
        {
            get => isCompanyField;
            set => isCompanyField = value;
        }

        /// <remarks/>
        public string Name
        {
            get => nameField;
            set => nameField = value;
        }

        /// <remarks/>
        public string FirstName
        {
            get => firstNameField;
            set => firstNameField = value;
        }

        /// <remarks/>
        public System.DateTime BirthDate
        {
            get => birthDateField;
            set => birthDateField = value;
        }

        /// <remarks/>
        public int ID_Appellation
        {
            get => iD_AppellationField;
            set => iD_AppellationField = value;
        }

        /// <remarks/>
        public object ListIdentityAddress
        {
            get => listIdentityAddressField;
            set => listIdentityAddressField = value;
        }

        /// <remarks/>
        public object ListIdentityCommunication
        {
            get => listIdentityCommunicationField;
            set => listIdentityCommunicationField = value;
        }

        /// <remarks/>
        public int SIRET
        {
            get => sIRETField;
            set => sIRETField = value;
        }

        /// <remarks/>
        public bool ForceUpdateCRM
        {
            get => forceUpdateCRMField;
            set => forceUpdateCRMField = value;
        }
    }

    #endregion CreateAccount Response
}