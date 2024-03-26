namespace Isango.Entities
{
    public class Supplier
    {
        private string _name;

        public string Name
        {
            get => _name == string.Empty ? AddressLine1 : _name;
            set => _name = value;
        }

        public int SupplierId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string CountryName { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string EmergencyPhoneNumber { get; set; }
        public string EmailId { get; set; }
        public string SupportEmailId { get; set; }
        public string WebsiteUrl { get; set; }

        public string VatNumber
        { get; set; }

        #region FareHarbor Properties

        public string UserKey { get; set; }
        public string ShortName { get; set; }
        public string Currency { get; set; }

        #endregion FareHarbor Properties
    }
}