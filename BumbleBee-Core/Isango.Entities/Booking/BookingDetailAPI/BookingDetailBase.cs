using System;
using System.Collections.Generic;
using System.Data;

namespace Isango.Entities.Booking.BookingDetailAPI
{
    public abstract class BookingDetailBase
    {
        public string VoucherType { get; set; }

        //Master Booking Data
        public string BookingRefNo { get; set; }

        public string CurrencyIsoCode { get; set; }

        public string VoucherEmail { get; set; }

        public string PhoneNumber { get; set; }

        public string AffiliateId { get; set; }

        public string LanguageCode { get; set; }

        public string IsProductGift { get; set; }

        public string CurrencySymbol { get; set; }

        public string BookingDate { get; set; }

        public string SubAffiliate { get; set; }

        //Affiliate Detail Data
        public string AffiliateShortName { get; set; }

        public string AffiliateName { get; set; }

        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }

        public string CompanyEmail { get; set; }

        public string ConfirmationBcc { get; set; }

        public bool IsLoginLess { get; set; }

        public bool IsPrimaryLogo { get; set; }
        public bool ShowTermsAndCondition { get; set; }

        public string FromMail { get; set; }

        public string TermsAndConditionLink { get; set; }

        public string LogoPath { get; set; }

        public string MailLogoPath { get; set; }

        public string CompanyWebSite { get; set; }

        public string CustomerServiceNo { get; set; }

        public string CustomerServicenoDesc { get; set; }

        public bool IsNetRateVoucher { get; set; }
        public List<UsefulDownload> UsefulDowloads { get; set; }

        protected BookingDetailBase(IDataReader result)
        {
            //VoucherType will be set in derived classes.

            result.NextResult();
            while (result.Read())
            {
                BookingRefNo = Convert.ToString(result["bookingrefno"]).Trim();
                CurrencyIsoCode = Convert.ToString(result["currencyisocode"]).Trim();
                VoucherEmail = Convert.ToString(result["VoucherEmail"]).Trim();
                AffiliateId = Convert.ToString(result["affiliateid"]).Trim();
                LanguageCode = Convert.ToString(result["languagecode"]).Trim();
                IsProductGift = Convert.ToString(result["IsProductGift"]).Trim();
                CurrencySymbol = Convert.ToString(result["CURRENCYSYMBOL"]).Trim();
                BookingDate = Convert.ToString(result["bookingdate"]).Trim();
                SubAffiliate = Convert.ToString(result["sub_affiliatename"]).Trim();
                PhoneNumber = Convert.ToString(result["CustomerPhoneNo"]).Trim();
            }

            result.NextResult();

            while (result.Read())
            {
                AffiliateShortName = Convert.ToString(result["affiliateshortname"]).Trim();
                AffiliateName = Convert.ToString(result["affiliatename"]).Trim();
                CompanyName = Convert.ToString(result["companyname"]).Trim();
                CompanyAddress = Convert.ToString(result["companyaddress"]).Trim();
                CompanyEmail = Convert.ToString(result["FromMail"]).Trim();
                if (!string.IsNullOrEmpty(Convert.ToString(result["confirmationbcc"]).Trim()))
                {
                    ConfirmationBcc = Convert.ToString(result["confirmationbcc"]).Trim();
                }
                IsLoginLess = Convert.ToBoolean(result["IsLoginLess"]);
                IsPrimaryLogo = Convert.ToBoolean(result["IsPrimaryLogo"]);
                ShowTermsAndCondition = Convert.ToBoolean(result["ShowTermsAndCondition"]);
                FromMail = Convert.ToString(result["FromMail"]).Trim();
                TermsAndConditionLink = Convert.ToString(result["TermsAndConditionLink"]).Trim();
                LogoPath = Convert.ToString(result["LogoPath"]).Trim();
                MailLogoPath = Convert.ToString(result["MailLogoPath"]).Trim();
                if (!string.IsNullOrEmpty(Convert.ToString(result["companywebsite"]).Trim()))
                {
                    CompanyWebSite = Convert.ToString(result["companywebsite"]).Trim();
                    CompanyWebSite = CompanyWebSite.StartsWith("http://") ? CompanyWebSite : "http://" + CompanyWebSite;
                }
                CustomerServiceNo = Convert.ToString(result["customerserviceno"]).Trim();
                if (!string.IsNullOrEmpty(Convert.ToString(result["customerservicenodesc"]).Trim()))
                {
                    CustomerServicenoDesc = Convert.ToString(result["customerservicenodesc"]).Trim();
                }
            }
        }
    }
}