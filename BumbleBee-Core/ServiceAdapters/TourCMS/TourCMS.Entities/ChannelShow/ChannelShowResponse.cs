using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.ChannelListResponse
{
    [XmlRoot(ElementName = "response")]
    public class ChannelShowResponse
    {
        [XmlElement(ElementName = "request")]
        public string Request { get; set; }

        [XmlElement(ElementName = "error")]
        public string Error { get; set; }

        [XmlElement(ElementName = "channel")]
        public List<ResponseChannelShow> ResponseChannelShow { get; set; }
    }


    public class ResponseChannelShow
    {
        [XmlElement(ElementName = "channel_id")]
        public string ChannelId { get; set; }

        [XmlElement(ElementName = "account_id")]
        public string Accountid { get; set; }

        [XmlElement(ElementName = "channel_name")]
        public string ChannelName { get; set; }

        [XmlElement(ElementName = "connection_permission")]
        public string ConnectionPermission { get; set; }

        [XmlElement(ElementName = "utc_offset_mins")]
        public string UtcOffsetMins { get; set; }

        [XmlElement(ElementName = "lang")]
        public string Lang { get; set; }

        [XmlElement(ElementName = "tour_count")]
        public string TourCount { get; set; }

        [XmlElement(ElementName = "sale_currency")]
        public string SaleCurrency { get; set; }

        [XmlElement(ElementName = "base_currency")]
        public string BaseCurrency { get; set; }

        [XmlElement(ElementName = "connection_date")]
        public string ConnectionDate { get; set; }

        [XmlElement(ElementName = "home_url")]
        public string HomeUrl { get; set; }

        [XmlElement(ElementName = "credit_card_fee_sale_percentage")]
        public string CreditCardFeeSalePercentage { get; set; }

        [XmlElement(ElementName = "home_url_tracked")]
        public string HomeUrlTracked { get; set; }

        [XmlElement(ElementName = "perm_override_sale_price")]
        public string PermOverrideSalePrice { get; set; }

        [XmlElement(ElementName = "perm_waive_cc_fee")]
        public string PermWaiveccfee { get; set; }


        [XmlElement(ElementName = "logo_url")]
        public string LogoUrl { get; set; }

        [XmlElement(ElementName = "company_name")]
        public string CompanyName { get; set; }

        [XmlElement(ElementName = "booking_style")]
        public string BookingStyle { get; set; }



        [XmlElement(ElementName = "short_desc")]
        public string ShortDesc { get; set; }

        [XmlElement(ElementName = "long_desc")]
        public string LongDesc { get; set; }


        [XmlElement(ElementName = "cancel_policy")]
        public string CancelPolicy { get; set; }

        [XmlElement(ElementName = "terms_and_conditions")]
        public string Termsandconditions { get; set; }

        [XmlElement(ElementName = "email_customer")]
        public string EmailCustomer { get; set; }

        [XmlElement(ElementName = "twitter")]
        public string Twitter { get; set; }

        [XmlElement(ElementName = "tripadvisor")]
        public string TripAdvisor { get; set; }

        [XmlElement(ElementName = "youtube")]
        public string Youtube { get; set; }


        [XmlElement(ElementName = "facebook")]
        public string Facebook { get; set; }

        [XmlElement(ElementName = "address_1")]
        public string Address1 { get; set; }

        [XmlElement(ElementName = "address_city")]
        public string AddressCity { get; set; }

        [XmlElement(ElementName = "address_state")]
        public string AddressState { get; set; }

        [XmlElement(ElementName = "address_country")]
        public string AddressCountry { get; set; }

        [XmlElement(ElementName = "commercial_email_private")]
        public string CommercialEmailPrivate { get; set; }

        [XmlElement(ElementName = "commercial_contactname_private")]
        public string CommercialContactnamePrivate { get; set; }

        [XmlElement(ElementName = "commercial_pitch_private")]
        public string CommercialPitchPrivate { get; set; }

        [XmlElement(ElementName = "commercial_ppl_private")]
        public string CommercialPplPrivate { get; set; }

        [XmlElement(ElementName = "commercial_dir_private")]
        public string CommercialDirPrivate { get; set; }
        [XmlElement(ElementName = "commercial_ppc_private")]
        public string CommercialPpcPrivate { get; set; }
        [XmlElement(ElementName = "commercial_aff_private")]
        public string CommercialAffPrivate { get; set; }
        [XmlElement(ElementName = "commercial_ag_private")]
        public string CommercialAgPrivate { get; set; }
        [XmlElement(ElementName = "commercial_any_private")]
        public string CommercialAnyPrivate { get; set; }
        [XmlElement(ElementName = "commercial_avleadtime_private")]
        public string CommercialAvleadtimePrivate { get; set; }
        [XmlElement(ElementName = "commercial_avtransaction_private")]
        public string CommercialAvtransactionPrivate { get; set; }
        [XmlElement(ElementName = "commercial_avpeople_private")]
        public string CommercialAvpeoplePrivate { get; set; }
        [XmlElement(ElementName = "commercial_avduration_private")]
        public string CommercialAvdurationPrivate { get; set; }
        [XmlElement(ElementName = "commercial_percent_online_private")]
        public string CommercialPercentOnlinePrivate { get; set; }

        [XmlElement(ElementName = "commercial_percent_convert_private")]
        public string CommercialPercentConvertPrivate { get; set; }

        [XmlElement(ElementName = "commercial_avclick2book_p")]
        public string CommercialAvclick2bookP { get; set; }

        [XmlElement(ElementName = "payment_gateway")]
        public PaymentGatewayShow PaymentGateway { get; set; }

        [XmlElement(ElementName = "payment_gateway_marketplace")]
        public PaymentGatewayMarketplace PaymentGatewayMarketplace { get; set; }
    }

    public class PaymentGatewayShow
    {
        [XmlElement(ElementName = "gateway_id")]
        public string GatewayId { get; set; }

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "gateway_type")]
        public string GatewayType { get; set; }

        [XmlElement(ElementName = "take_visa")]
        public string TakeVisa { get; set; }

        [XmlElement(ElementName = "take_mastercard")]
        public string TakeMastercard { get; set; }

        [XmlElement(ElementName = "take_diners")]
        public string TakeDiners { get; set; }

        [XmlElement(ElementName = "take_discover")]
        public string TakeDiscover { get; set; }

        [XmlElement(ElementName = "take_amex")]
        public string TakeAmex { get; set; }

        [XmlElement(ElementName = "take_unionpay")]
        public string TakeUnionpay { get; set; }
    }
    public class PaymentGatewayMarketplace
    {
        [XmlElement(ElementName = "gateway_id")]
        public string GatewayId { get; set; }

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "gateway_type")]
        public string GatewayType { get; set; }

        [XmlElement(ElementName = "take_visa")]
        public string TakeVisa { get; set; }

        [XmlElement(ElementName = "take_mastercard")]
        public string TakeMastercard { get; set; }

        [XmlElement(ElementName = "take_diners")]
        public string TakeDiners { get; set; }

        [XmlElement(ElementName = "take_discover")]
        public string TakeDiscover { get; set; }

        [XmlElement(ElementName = "take_amex")]
        public string TakeAmex { get; set; }

        [XmlElement(ElementName = "take_unionpay")]
        public string TakeUnionpay { get; set; }
    }
}