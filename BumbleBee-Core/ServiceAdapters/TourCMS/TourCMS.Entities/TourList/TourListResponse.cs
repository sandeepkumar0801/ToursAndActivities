using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.ChannelListResponse
{
    [XmlRoot(ElementName = "response")]
    public class TourListResponse
    {
        [XmlElement(ElementName ="request")]
        public string Request { get; set; }

        [XmlElement(ElementName ="error")]
        public string Error { get; set; }

        [XmlElement(ElementName = "tour")]
        public List<TourList> ResponseTourList { get; set; }
    }

  
    public class TourList
    {
        [XmlElement(ElementName ="channel_id")]
        public string ChannelId { get; set; }

        [XmlElement(ElementName ="account_id")]
        public string Accountid { get; set; }

        [XmlElement(ElementName = "tour_id")]
        public string TourId { get; set; }

        [XmlElement(ElementName = "tour_code")]
        public string TourCode { get; set; }

        [XmlElement(ElementName = "has_sale")]
        public string HasSale { get; set; }

        [XmlElement(ElementName = "has_d")]
        public string HasD { get; set; }

        [XmlElement(ElementName = "has_f")]
        public string HasF { get; set; }

        [XmlElement(ElementName = "has_h")]
        public string HasH { get; set; }

        [XmlElement(ElementName = "descriptions_last_updated")]
        public string DescriptionsLastUpdated { get; set; }

        [XmlElement(ElementName = "tour_name")]
        public string TourName { get; set; }
    }
}