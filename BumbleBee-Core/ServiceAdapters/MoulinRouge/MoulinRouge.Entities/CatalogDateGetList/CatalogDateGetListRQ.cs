using System;
using System.Xml.Serialization;
using Util;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetList
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Request
    {
        public Request()
        {
            Body = new EnvelopeBody();
            Header = new object();
        }

        /// <remarks/>
        public object Header { get; set; }

        /// <remarks/>
        public EnvelopeBody Body { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class EnvelopeBody
    {
        /// <remarks/>
        [XmlElementAttribute(Namespace = "http://tempuri.org/")]
        //[XmlRootAttribute("ACP_CatalogDateGetListRequest")]
        //[XmlElement(ElementName = "ACP_CatalogDateGetListRequest")]
        public ACP_CatalogDateGetListRequest ACP_CatalogDateGetListRequest { get; set; }

        public EnvelopeBody()
        {
            ACP_CatalogDateGetListRequest = new ACP_CatalogDateGetListRequest();
        }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class ACP_CatalogDateGetListRequest
    {
        public ACP_CatalogDateGetListRequest()
        {
            context = RequestCredentialsContext.GetContextInstance();
            dateFrom = DateTime.Now;
            dateTo = DateTime.Now.AddDays(3);
            id_Catalog = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("Id_Catalog"));
            ID_Category = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("Id_Category"));
            id_Bloc = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("Id_Bloc"));
            id_Floor = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("Id_Floor"));
        }

        /// <remarks/>
        //[XmlElement(ElementName = "context")]
        public RequestCredentialsContext context { get; set; }

        /// <remarks/>
        //[XmlAttributeAttribute("id_Catalog")]
        public int id_Catalog { get; set; }

        /// <remarks/>
        //[XmlAttributeAttribute("dateFrom")]
        public DateTime dateFrom { get; set; }

        /// <remarks/>
        //[XmlAttributeAttribute("dateTo")]
        public DateTime dateTo { get; set; }

        /// <remarks/>
        // [XmlAttributeAttribute("ID_Category")]
        public int ID_Category { get; set; }

        /// <remarks/>
        //[XmlAttributeAttribute("id_Bloc")]
        public int id_Bloc { get; set; }

        /// <remarks/>
        //[XmlAttributeAttribute("id_Floor")]
        public int id_Floor { get; set; }
    }
}