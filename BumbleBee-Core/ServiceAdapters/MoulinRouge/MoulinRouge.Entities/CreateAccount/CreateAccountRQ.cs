namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CreateAccount
{
    #region CreateAccount Request Classes

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

        private CreateAccountRQEnvelopeBody bodyField;

        public Request()
        {
            Body = new CreateAccountRQEnvelopeBody();
            headerField = new object();
        }

        /// <remarks/>
        public object Header
        {
            get => headerField;
            set => headerField = value;
        }

        /// <remarks/>
        public CreateAccountRQEnvelopeBody Body
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
    public class CreateAccountRQEnvelopeBody
    {
        private ACP_CreateAccountRequest aCP_CreateAccountRequestField;

        public CreateAccountRQEnvelopeBody()
        {
            ACP_CreateAccountRequest = new ACP_CreateAccountRequest();
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_CreateAccountRequest ACP_CreateAccountRequest
        {
            get => aCP_CreateAccountRequestField;
            set => aCP_CreateAccountRequestField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_CreateAccountRequest
    {
        private RequestCredentialsContext contextField;

        private ACP_CreateAccountRequestIdentity identityField;

        private string loginField;

        private string passwordField;

        public ACP_CreateAccountRequest()
        {
            context = RequestCredentialsContext.GetContextInstance();
            identity = new ACP_CreateAccountRequestIdentity();
        }

        /// <remarks/>
        public RequestCredentialsContext context
        {
            get => contextField;
            set => contextField = value;
        }

        /// <remarks/>
        public ACP_CreateAccountRequestIdentity identity
        {
            get => identityField;
            set => identityField = value;
        }

        /// <remarks/>
        public string login
        {
            get => loginField;
            set => loginField = value;
        }

        /// <remarks/>
        public string password
        {
            get => passwordField;
            set => passwordField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    public class ACP_CreateAccountRequestIdentity
    {
        private int iD_IdentityField;

        private int iD_ExterneField;

        private bool isCompanyField;

        private string nameField;

        private string firstNameField;

        private int iD_AppellationField;

        private object listIdentityAddressField;

        private object listIdentityCommunicationField;

        private int sIRETField;

        private int forceUpdateCRMField;

        public ACP_CreateAccountRequestIdentity()
        {
            listIdentityAddressField = new object();
            listIdentityCommunicationField = new object();
        }

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
        public int ForceUpdateCRM
        {
            get => forceUpdateCRMField;
            set => forceUpdateCRMField = value;
        }
    }

    #endregion CreateAccount Request Classes
}