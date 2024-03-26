using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.ChannelListResponse
{
    [XmlRoot(ElementName = "response")]
    public class ChannelListResponse
    {
        [XmlElement(ElementName ="request")]
        public string Request { get; set; }

        [XmlElement(ElementName ="error")]
        public string Error { get; set; }

        [XmlElement(ElementName ="channel")]
        public List<ResponseChannelList> ResponseChannelList { get; set; }
    }

  
    public class ResponseChannelList
    {
        [XmlElement(ElementName ="channel_id")]
        public string ChannelId { get; set; }

        [XmlElement(ElementName ="account_id")]
        public string Accountid { get; set; }

        [XmlElement(ElementName ="channel_name")]
        public string ChannelName { get; set; }

        [XmlElement(ElementName ="tour_count")]
        public string TourCount { get; set; }

        [XmlElement(ElementName ="logo_url")]
        public string LogoUrl { get; set; }

        [XmlElement(ElementName ="connection_permission")]
        public string ConnectionPermission { get; set; }

        [XmlElement(ElementName ="lang")]
        public string Lang { get; set; }

        [XmlElement(ElementName ="sale_currency")]
        public string SaleCurrency { get; set; }

       
        [XmlElement(ElementName ="home_url")]
        public string HomeUrl { get; set; }

        [XmlElement(ElementName ="home_url_tracked")]
        public string HomeUrlTracked { get; set; }

        [XmlElement(ElementName ="short_desc")]
        public string ShortDesc { get; set; }
    }

    public  class PaymentGateway
    {
        [XmlElement(ElementName ="gateway_id")]
        public string GatewayId { get; set; }

        [XmlElement(ElementName ="name")]
        public string Name { get; set; }

        [XmlElement(ElementName ="credit_card_fee_sale_percentage")]
        public string CreditCardFeeSalePercentage { get; set; }

        [XmlElement(ElementName ="gateway_type")]
        public string GatewayType { get; set; }

        [XmlElement(ElementName ="take_visa")]
        public string TakeVisa { get; set; }

        [XmlElement(ElementName ="take_mastercard")]
        public string TakeMastercard { get; set; }

        [XmlElement(ElementName ="take_diners")]
        public string TakeDiners { get; set; }

        [XmlElement(ElementName ="take_discover")]
        public string TakeDiscover { get; set; }

        [XmlElement(ElementName ="take_amex")]
        public string TakeAmex { get; set; }

        [XmlElement(ElementName ="take_unionpay")]
        public string TakeUnionpay { get; set; }
     }
}