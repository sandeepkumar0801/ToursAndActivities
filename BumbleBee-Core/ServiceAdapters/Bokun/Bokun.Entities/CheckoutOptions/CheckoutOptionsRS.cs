using System.Collections.Generic;

namespace ServiceAdapters.Bokun.Bokun.Entities.CheckoutOptions
{
    public class CheckoutOptionsRs
    {
        public List<Option> Options { get; set; }
        public Questions Questions { get; set; }
    }

    public class Option
    {
        public string Type { get; set; }
        public string Label { get; set; }
        public List<MainContactDetail> Questions { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string FormattedAmount { get; set; }
        public bool PartialPayment { get; set; }
        public Invoice Invoice { get; set; }
        public PaymentMethods PaymentMethods { get; set; }
    }

    public class Invoice
    {
        public long Id { get; set; }
        public long IssueDate { get; set; }
        public string Currency { get; set; }
        public List<object> IncludedAppliedTaxes { get; set; }
        public List<object> ExcludedAppliedTaxes { get; set; }
        public Issuer Issuer { get; set; }
        public IssuerCompany IssuerCompany { get; set; }
        public List<object> CustomLineItems { get; set; }
        public List<ProductInvoice> ProductInvoices { get; set; }
        public List<object> Payments { get; set; }
        public AsMoney RemainingAmountAsMoney { get; set; }
        public string IssuerName { get; set; }
        public string InvoiceNumber { get; set; }
        public string RecipientName { get; set; }
        public string RemainingAmountAsText { get; set; }
        public List<object> LodgingTaxes { get; set; }
        public AsMoney TotalDiscountedAsMoney { get; set; }
        public AsMoney TotalAsMoney { get; set; }
        public bool Free { get; set; }
        public string TotalAsText { get; set; }
        public string TotalDiscountedAsText { get; set; }
        public AsMoney TotalDueAsMoney { get; set; }
        public string TotalDueAsText { get; set; }
        public bool ExcludedTaxes { get; set; }
        public AsMoney TotalExcludedTaxAsMoney { get; set; }
        public string TotalExcludedTaxAsText { get; set; }
        public bool IncludedTaxes { get; set; }
        public AsMoney TotalIncludedTaxAsMoney { get; set; }
        public string TotalIncludedTaxAsText { get; set; }
        public AsMoney TotalTaxAsMoney { get; set; }
        public string TotalTaxAsText { get; set; }
        public AsMoney TotalDiscountAsMoney { get; set; }
    }

    public class Issuer
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string ExternalId { get; set; }
        public List<object> Flags { get; set; }
    }

    public class IssuerCompany
    {
    }

    public class ProductInvoice
    {
        public long Id { get; set; }
        public string Currency { get; set; }
        public List<object> IncludedAppliedTaxes { get; set; }
        public List<object> ExcludedAppliedTaxes { get; set; }
        public string Title { get; set; }
        public Issuer Product { get; set; }
        public string ProductCategory { get; set; }
        public string Dates { get; set; }
        public List<object> CustomLineItems { get; set; }
        public List<LineItem> LineItems { get; set; }
        public AsMoney TotalDiscountedAsMoney { get; set; }
        public AsMoney TotalAsMoney { get; set; }
        public bool Free { get; set; }
        public string TotalAsText { get; set; }
        public string TotalDiscountedAsText { get; set; }
        public AsMoney TotalDueAsMoney { get; set; }
        public string TotalDueAsText { get; set; }
        public bool ExcludedTaxes { get; set; }
        public AsMoney TotalExcludedTaxAsMoney { get; set; }
        public string TotalExcludedTaxAsText { get; set; }
        public bool IncludedTaxes { get; set; }
        public AsMoney TotalIncludedTaxAsMoney { get; set; }
        public string TotalIncludedTaxAsText { get; set; }
        public AsMoney TotalTaxAsMoney { get; set; }
        public string TotalTaxAsText { get; set; }
        public AsMoney TotalDiscountAsMoney { get; set; }
    }

    public class LineItem
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Currency { get; set; }
        public long UnitPriceDate { get; set; }
        public long Quantity { get; set; }
        public long UnitPrice { get; set; }
        public long CalculatedDiscount { get; set; }
        public long CustomDiscount { get; set; }
        public long CalculatedDiscountAmount { get; set; }
        public long TaxAmount { get; set; }
        public List<object> TaxesAsMoney { get; set; }
        public string ItemBookingId { get; set; }
        public long CostItemId { get; set; }
        public string CostItemTitle { get; set; }
        public string CostGroupTitle { get; set; }
        public bool SupportsDiscount { get; set; }
        public long Total { get; set; }
        public AsMoney TotalDiscountedAsMoney { get; set; }
        public AsMoney TotalAsMoney { get; set; }
        public string TotalAsText { get; set; }
        public string TotalDiscountedAsText { get; set; }
        public AsMoney TotalDueAsMoney { get; set; }
        public string TotalDueAsText { get; set; }
        public AsMoney TaxAsMoney { get; set; }
        public long TotalDue { get; set; }
        public AsMoney DiscountedUnitPriceAsMoney { get; set; }
        public long Discount { get; set; }
        public string TaxAsText { get; set; }
        public long TotalDiscounted { get; set; }
        public long DiscountedUnitPrice { get; set; }
        public string DiscountedUnitPriceAsText { get; set; }
        public AsMoney UnitPriceAsMoney { get; set; }
        public string UnitPriceAsText { get; set; }
        public AsMoney DiscountAmountAsMoney { get; set; }
        public string DiscountAmountAsText { get; set; }
        public AsMoney CalculatedDiscountAmountAsMoney { get; set; }
    }

    public class AsMoney
    {
        public long Amount { get; set; }
        public string Currency { get; set; }
    }

    public class PaymentMethods
    {
        public List<string> AllowedMethods { get; set; }
    }

    public class MainContactDetail
    {
        public string QuestionId { get; set; }
        public string Label { get; set; }
        public string DataType { get; set; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
        public bool SelectFromOptions { get; set; }
        public bool SelectMultiple { get; set; }
        public string DataFormat { get; set; }
        public List<AnswerOption> AnswerOptions { get; set; }
        public string QuestionCode { get; set; }
        public string Help { get; set; }
        public string Placeholder { get; set; }
        public string Pattern { get; set; }
        public string OriginalQuestion { get; set; }
        public List<string> Flags { get; set; }
        public List<string> Answers { get; set; }
    }

    public class Questions
    {
        public List<MainContactDetail> MainContactDetails { get; set; }
        public List<ActivityBooking> ActivityBookings { get; set; }
    }

    public class ActivityBooking
    {
        public long ActivityId { get; set; }
        public string ActivityTitle { get; set; }
        public List<MainContactDetail> Questions { get; set; }
        public List<Passenger> Passengers { get; set; }
    }

    public class Passenger
    {
        public long PricingCategoryId { get; set; }
        public string PricingCategoryTitle { get; set; }
        public List<MainContactDetail> PassengerDetails { get; set; }
        public List<MainContactDetail> Questions { get; set; }
        public List<object> Extras { get; set; }
    }

    public class AnswerOption
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
}