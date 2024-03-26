using System.Xml.Serialization;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities.ReleaseSeats
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false, ElementName = "Envelope")]
    public class Response : EntityBase
    {
        /// <remarks/>
        public ReleaseSeatsRsEnvelopeBody Body { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRootAttribute(ElementName = "EnvelopeBody")]
    public class ReleaseSeatsRsEnvelopeBody
    {
        public ReleaseSeatsRsEnvelopeBody()
        {
            AcpReleaseSeatsRequestResponse = new AcpReleaseSeatsRequestResponse();
        }

        /// <remarks/>
        [XmlElementAttribute(Namespace = "http://tempuri.org/")]
        [XmlAttributeAttribute("ACP_ReleaseSeatsRequestResponse")]
        public AcpReleaseSeatsRequestResponse AcpReleaseSeatsRequestResponse { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false)]
    public class AcpReleaseSeatsRequestResponse
    {
        /// <remarks/>
        [XmlAttributeAttribute("ACP_ReleaseSeatsRequestResult")]
        public bool AcpReleaseSeatsRequestResult { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute("result")]
        public int Result { get; set; }
    }
}