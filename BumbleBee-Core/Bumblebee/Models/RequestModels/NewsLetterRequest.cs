using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class NewsLetterRequest
    {
        [Required]
        public string EmailId { get; set; }

        [Required]
        public string Name { get; set; }

        public string CustomerOrigin { get; set; }
        public bool IsTopDeals { get; set; }

        [Required]
        public bool ConsentUser { get; set; }

        [Required]
        public ClientInfoRequest ClientInfo { get; set; }

        ////Not mentioned properties
        //public int CountryId { get; set; }
        //public string CountryName { get; set; }
        //public bool IsNbVerified { get; set; }
        //public bool ConsentUser { get; set; }
    }

    public class ClientInfoRequest
    {
        [Required]
        public string CurrencyIsoCode { get; set; }

        [Required]
        public string LanguageCode { get; set; }

        [Required]
        public string AffiliateId { get; set; }

        public string B2BAffiliateId { get; set; }
        public string AffiliateName { get; set; }
        public string AffiliateDisplayName { get; set; }

        [Required]
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

        public decimal MaxMarginLimit { get; set; }

        public string DiscountCode { get; set; }
        public decimal DiscountCodePercentage { get; set; }

        public string CompanyAlias { get; set; }
        public bool IsB2BNetPriceAffiliate { get; set; }

        [Required]
        public string TokenId { get; set; }
    }

    public class NewsLetter
    {
        [Required]
        public string EmailId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LanguageCode { get; set; }

        [Required]
        public string AffiliateId { get; set; }

        [Required]
        public string CustomerOrigin { get; set; }
    }
}