using System;

namespace Isango.Entities
{
    public class ISangoUser
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public DateTime DateofBirth { get; set; }
        public string UserLoginSource { get; set; }
        public int Age { get; set; }
        public string LanguageCode { get; set; }
        public string CurrencyISOCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int CountryId { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public bool IsEmailNBVerified { get; set; }
        public bool IsGuestLogin { get; set; }
        public string DeliveryAddress { set; get; }
        public string DeliveryCountry { set; get; }
        public string DeliveryCity { set; get; }
        public string DeliveryPostCode { set; get; }
        public bool IsOurEmployee { get; set; }
        public DateTime UserCreationDate { get; set; }
    }
}