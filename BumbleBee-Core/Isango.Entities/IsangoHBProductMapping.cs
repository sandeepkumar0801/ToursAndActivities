using Isango.Entities.Enums;

namespace Isango.Entities
{
    public class IsangoHBProductMapping
    {
        /// <summary>
        /// Isango Service/Activity Id
        /// </summary>
        public int IsangoHotelBedsActivityId { get; set; }
        public string HotelBedsActivityCode { get; set; }
        public int IsangoRegionId { get; set; }
        public int FactSheetId { get; set; }
        public int MinAdultCount { get; set; }
        public string DestinationCode { get; set; }
        public string Credentials { get; set; }
        public string Language { get; set; }
        public APIType ApiType { get; set; }

        //TODO: Console Application: Need confirmation
        public int CountryId { get; set; }

        public decimal MarginAmount { get; set; }
        public bool IsMarginPercent { get; set; }
        public int ServiceOptionInServiceid { get; set; }
        public string CurrencyISOCode { get; set; }
        public string SupplierCode { get; set; }
        public string PrefixServiceCode { get; set; }
	    public int PriceTypeId { get; set; }
        public bool IsIsangoMarginApplicable { get; set; }
    }
}