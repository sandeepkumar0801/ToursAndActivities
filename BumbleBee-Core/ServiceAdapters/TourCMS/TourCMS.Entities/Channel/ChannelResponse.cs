using System.Collections.Generic;
namespace ServiceAdapters.TourCMS.TourCMS.Entities.ChannelResponse
{
    public class ChannelResponse
    {
        public string Request { get; set; }
        public string Error { get; set; }
        public List<ResponseChannelData> ResponseChannelList { get; set; }
    }
    public class ResponseChannelData
    {
        public string ChannelId { get; set; }
        public string Accountid { get; set; }
        public string ChannelName { get; set; }
        public string TourCount { get; set; }
        public string LogoUrl { get; set; }
        public string ConnectionPermission { get; set; }
        public string Lang { get; set; }
        public string SaleCurrency { get; set; }
        public string HomeUrl { get; set; }
        public string HomeUrlTracked { get; set; }
        public string ShortDesc { get; set; }
        public string UtcOffsetMins { get; set; }
        public string BaseCurrency { get; set; }
        public string ConnectionDate { get; set; }
        public string CreditCardFeeSalePercentage { get; set; }
        public string PermOverrideSalePrice { get; set; }
        public string PermWaiveccfee { get; set; }
        public string CompanyName { get; set; }
        public string BookingStyle { get; set; }
        public string LongDesc { get; set; }
        public string CancelPolicy { get; set; }
        public string Termsandconditions { get; set; }
        public string EmailCustomer { get; set; }
        public string Twitter { get; set; }
        public string TripAdvisor { get; set; }
        public string Youtube { get; set; }
        public string Facebook { get; set; }
        public string Address1 { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressCountry { get; set; }
        public string CommercialEmailPrivate { get; set; }
        public string CommercialContactnamePrivate { get; set; }
        public string CommercialPitchPrivate { get; set; }
        public string CommercialPplPrivate { get; set; }
        public string CommercialDirPrivate { get; set; }
        public string CommercialPpcPrivate { get; set; }
        public string CommercialAffPrivate { get; set; }
        public string CommercialAgPrivate { get; set; }
        public string CommercialAnyPrivate { get; set; }
        public string CommercialAvleadtimePrivate { get; set; }
        public string CommercialAvtransactionPrivate { get; set; }
        public string CommercialAvpeoplePrivate { get; set; }
        public string CommercialAvdurationPrivate { get; set; }
        public string CommercialPercentOnlinePrivate { get; set; }
        public string CommercialPercentConvertPrivate { get; set; }
        public string CommercialAvclick2bookP { get; set; }
        public string GatewayId { get; set; }
        public string Name { get; set; }
        public string GatewayType { get; set; }
        public string TakeVisa { get; set; }
        public string TakeMastercard { get; set; }
        public string TakeDiners { get; set; }
        public string TakeDiscover { get; set; }
        public string TakeAmex { get; set; }
        public string TakeUnionpay { get; set; }
    }
}