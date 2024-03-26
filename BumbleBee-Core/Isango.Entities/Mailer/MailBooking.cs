using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Entities.Mailer
{
    public class MailBooking
    {
        public string CurrencySymbol { get; set; }
        public double Multisave { get; set; }
        public double Discount { get; set; }
        public double AfterDiscount { get; set; }
        public decimal TotalChargedAmount { get; set; }
        public decimal OrAmount { get; set; }
        public string CustomerName { get; set; }
        public string ReferenceNumber { get; set; }
        public string VoucherLink { get; set; }
        public decimal BookingAmount { get; set; }
        public string BookingAmountLabel { get; set; }
        public string MultisaveDiscountLabel { get; set; }
        public string DiscountCodeLabel { get; set; }
        public string AfterDiscountLabel { get; set; }
        public string ChargedAmountLabel { get; set; }
        public string OrAmountLabel { get; set; }
        public string Language { get; set; }
        public string OrBookingText { get; set; }
        public string CompanyEmail { get; set; }
        public string TermsLink { get; set; }
        public PaymentGatewayType PaymentGateway { get; set; }

        public string AffiliateName { get; set; } = "isango!";
        public string AffiliateURL { get; set; } = "isango!";

        public int? AffiliateGroupID { get; set; }

        public string LogoPath { get; set; }
        public string CustomerEmail { get; set; }
        public string LeadGuestName { get; set; }
        public List<string> HBVoucherList { get; set; }
        public string TermsAndConditionLink { get; set; }
    }
}