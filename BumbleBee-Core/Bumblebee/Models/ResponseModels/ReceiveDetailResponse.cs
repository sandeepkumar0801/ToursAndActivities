using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels
{
	public class ReceiveDetailResponse
    {
        public int FinancialBookingTransactionId { get; set; }
        public string BookingReferenceNumber { get; set; }
        public string AffiliateId { get; set; }
        public string LanguageCode { get; set; }
        public string VoucherEmail { get; set; }
        public int BookedOptionId { get; set; }
        public decimal ChargeAmount { get; set; }
        public string SellCurrency { get; set; }
        public string CurrencySymbol { get; set; }
        public string ServiceName { get; set; }
        public string ServiceOptionName { get; set; }
        public string Remarks { get; set; }
        public string PassengerFirstName { get; set; }
        public string PassengerLastName { get; set; }
        public string PhoneNumber { get; set; }
        public int UserId { get; set; }
        public string CompanyWebsite { get; set; }
        public string LeadPaxName { get; set; }
        public string AffiliateName { get; set; }
        public string CountryID { get; set; }
        public string Status { get; set; }
        public string PaymentGatwayType { get; set; }

        public string AdyenMerchantAccout { get; set; }
    }

	
}