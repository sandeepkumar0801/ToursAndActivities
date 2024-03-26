using Isango.Entities.Enums;
using Isango.Entities.Payment;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Isango.Entities.Affiliate
{
    public class Affiliate
    {
        /// <summary>
        /// This is used to decide which UI template needs to be considered
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public List<SupportedLanguages> Languages { get; set; }

        public AffiliateGroup Type { get; set; }

        public string Isocode { get; set; }

        public string CurrencySymbol { get; set; }
        public string LanguageCode { get; set; }
        public string CurrencyName { get; set; }//Currency iso code for affiliate
        public decimal DiscountCodePercentage { get; set; }

        /// <summary>
        /// This is used to decide which price and product business rule needs to be considered
        /// </summary>
        public string B2BAffiliateId { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public bool isRiskifiedEnable { get; set; }

        public int AffiliateRegionId { get; set; }

        public string Alias { get; set; }

        public string Subdomain { get; set; }

        public int? AffiliateKey { get; set; }

        public bool SupportMultiPaymentType { get; set; }

        public List<SupportedLanguage> SupportedLanguages { get; set; }

        public List<SupportedCurrency> SupportedCurrencies { get; set; }

        public LineOfBusiness LineOfBusiness { get; set; }

        public PaymentMethodType PaymentMethodType { get; set; }

        public AffiliateConfiguration AffiliateConfiguration { get; set; }

        public AffiliateCompanyDetail AffiliateCompanyDetail { get; set; }

        public AffiliateCredit AffiliateCredit { get; set; }

        /// <summary>
        /// This is use for login and tracking
        /// </summary>
        public GoogleDetail GoogleDetail { get; set; }

        /// <summary>
        /// Usage of Facebook details and how it works
        /// </summary>
        public FacebookDetail FacebookDetail { get; set; }

        public int GroupId { get; set; } //As it is being used in setting of template name

        public string AffiliateBaseURL { get; set; }

        public string TermsAndConditionLink { get; set; }

        public string CompanyWebSite { get; set; }

        /// <summary>
        /// Availability Reference Expiry in minutes
        /// </summary>
        public int AvailabilityReferenceExpiry { get; set; }

        public string WebHookURL { get; set; }

        public bool AdyenStringentAccount { get; set; }

        public bool AdyenStringentAccountRestrictProducts { get; set; }
    }

    public class SupportedLanguages
    {
        public int ID { get; set; }
        public string ShortName { get; set; }
        public string IsoCode { get; set; }
        public string DisplayName { get; set; }
    }
}