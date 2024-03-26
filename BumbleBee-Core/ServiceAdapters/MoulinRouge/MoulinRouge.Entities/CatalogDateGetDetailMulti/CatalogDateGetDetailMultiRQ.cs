using System.Collections.Generic;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetDetailMulti
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Request
    {
        private object headerField;

        private CatalogDateGetDetailMultiRequestEnvelopeBody bodyField;

        public Request()
        {
            bodyField = new CatalogDateGetDetailMultiRequestEnvelopeBody();
            headerField = new object();
        }

        /// <remarks/>
        public object Header
        {
            get => headerField;
            set => headerField = value;
        }

        /// <remarks/>
        public CatalogDateGetDetailMultiRequestEnvelopeBody Body
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
    public class CatalogDateGetDetailMultiRequestEnvelopeBody
    {
        private ACP_CatalogDateGetDetailMultiRequest aCP_CatalogDateGetDetailMultiRequestField;

        public CatalogDateGetDetailMultiRequestEnvelopeBody()
        {
            aCP_CatalogDateGetDetailMultiRequestField = new ACP_CatalogDateGetDetailMultiRequest();
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://tempuri.org/")]
        public ACP_CatalogDateGetDetailMultiRequest ACP_CatalogDateGetDetailMultiRequest
        {
            get => aCP_CatalogDateGetDetailMultiRequestField;
            set => aCP_CatalogDateGetDetailMultiRequestField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_CatalogDateGetDetailMultiRequest
    {
        private RequestCredentialsContext contextField;

        private int id_CatalogField;

        private List<int> listID_CatalogDateField;

        private int id_BlocField;

        private int id_FloorField;

        public ACP_CatalogDateGetDetailMultiRequest()
        {
            contextField = RequestCredentialsContext.GetContextInstance();
            listID_CatalogDateField = new List<int>();
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
        [System.Xml.Serialization.XmlArrayItemAttribute("int", IsNullable = false)]
        public List<int> listID_CatalogDate
        {
            get => listID_CatalogDateField;
            set => listID_CatalogDateField = value;
        }

        /// <remarks/>
        public int id_Bloc
        {
            get => id_BlocField;
            set => id_BlocField = value;
        }

        /// <remarks/>
        public int id_Floor
        {
            get => id_FloorField;
            set => id_FloorField = value;
        }
    }
}