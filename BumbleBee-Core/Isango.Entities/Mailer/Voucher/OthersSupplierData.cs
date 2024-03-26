using System;
using System.Data;

namespace Isango.Entities.Mailer.Voucher
{
    public class OthersSupplierData
    {
        public string BookedOptionId { get; set; }

        public string ServiceId { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string AddressId { get; set; }
        public string AddressTypeId { get; set; }
        public string AddressTypeName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Postcode { get; set; }
        public string City { get; set; }
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string EmergencyTelephoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string CountryId { get; set; }
        public string CountryName { get; set; }
        public string SupportEmailId { get; set; }

        public OthersSupplierData(IDataReader result)
        {
            BookedOptionId = Convert.ToString(result["bookedoptionid"]).Trim();
            ServiceId = Convert.ToString(result["serviceid"]).Trim();
            SupplierId = Convert.ToString(result["supplierid"]).Trim();
            SupplierName = Convert.ToString(result["suppliername"]).Trim();
            AddressId = Convert.ToString(result["addressid"]).Trim();
            AddressTypeId = Convert.ToString(result["addresstypeid"]).Trim();
            AddressTypeName = Convert.ToString(result["addresstypename"]).Trim();
            AddressLine1 = Convert.ToString(result["addressline1"]).Trim();
            AddressLine2 = Convert.ToString(result["addressline2"]).Trim();
            AddressLine3 = Convert.ToString(result["addressline3"]).Trim();
            AddressLine4 = Convert.ToString(result["addressline4"]).Trim();
            Postcode = Convert.ToString(result["postcode"]).Trim();
            City = Convert.ToString(result["city"]).Trim();
            TelephoneNumber = Convert.ToString(result["telephonenumber"]).Trim();
            FaxNumber = Convert.ToString(result["faxnumber"]).Trim();
            EmergencyTelephoneNumber = Convert.ToString(result["emergencytelephonenumber"]).Trim();
            EmailAddress = Convert.ToString(result["emailaddress"]).Trim();
            CountryId = Convert.ToString(result["countryid"]).Trim();
            CountryName = Convert.ToString(result["countryname"]).Trim();
            SupportEmailId = Convert.ToString(result["supportemailid"]).Trim();
        }
    }
}