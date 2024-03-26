using Isango.Entities.Payment;
using System;

namespace Isango.Entities
{
    public class ClientInfo
    {
        public Currency Currency { get; set; }
        public string LanguageCode { get; set; }
        public string AffiliateId { get; set; }

        public string B2BAffiliateId { get; set; }
        public string AffiliateName { get; set; }
        public string AffiliateDisplayName { get; set; }
        public string CountryIp { get; set; }
        public string CityCode { get; set; }
        public string GtmIdentifier { get; set; }
        public string LineOfBusiness { get; set; }
        public string AffiliatePartner { get; set; }
        public string FacebookAppId { get; set; }
        public string FacebookAppSecret { get; set; }
        public bool IsSupplementOffer { get; set; }
        public bool IsB2BAffiliate { get; set; }
        public DateTime WidgetDate { get; set; }
        public PaymentMethodType PaymentMethodType { get; set; }

        public decimal MaxMarginLimit { get; set; }

        public string DiscountCode { get; set; }
        public decimal DiscountCodePercentage { get; set; }

        public string CompanyAlias { get; set; }
        public bool IsB2BNetPriceAffiliate { get; set; }
        public string ApiToken { get; set; }
    }
}