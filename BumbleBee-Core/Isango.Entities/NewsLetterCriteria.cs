namespace Isango.Entities
{
    public class NewsLetterCriteria
    {
        public string EmailId { get; set; }
        public string LanguageCode { get; set; }
        public string AffiliateId { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public bool IsNbVerified { get; set; }
        public bool ConsentUser { get; set; }
        public string UserName { get; set; }
    }

    public class NewsLetterData
    {
        public string EmailId { get; set; }

        public string Name { get; set; }

        public string LanguageCode { get; set; }

        public string AffiliateId { get; set; }

        public string CustomerOrigin { get; set; }
    }
}