using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.ReleaseSeats
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Request
    {
        public Request()
        {
            Body = new ReleaseSeatsRequesRqEnvelopeBody();
            Header = new object();
        }

        /// <remarks/>
        public object Header { get; set; }

        /// <remarks/>
        public ReleaseSeatsRequesRqEnvelopeBody Body { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRootAttribute(ElementName = "EnvelopeBody")]
    public class ReleaseSeatsRequesRqEnvelopeBody
    {
        public ReleaseSeatsRequesRqEnvelopeBody()
        {
            AcpReleaseSeatsRequest = new AcpReleaseSeatsRequest();
        }

        /// <remarks/>
        [XmlElementAttribute(Namespace = "http://tempuri.org/")]
        [XmlAttributeAttribute("ACP_ReleaseSeatsRequest")]
        public AcpReleaseSeatsRequest AcpReleaseSeatsRequest { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class AcpReleaseSeatsRequest
    {
        public AcpReleaseSeatsRequest()
        {
            Context = RequestCredentialsContext.GetContextInstance();
            ListSeats = new List<int>();
        }

        /// <remarks/>
        [XmlAttributeAttribute("context")]
        public RequestCredentialsContext Context { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("id_TemporaryOrderRow")]
        public string IdTemporaryOrderRow { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("listSeats")]
        [XmlArrayItemAttribute("int", IsNullable = false)]
        public List<int> ListSeats { get; set; }
    }
}