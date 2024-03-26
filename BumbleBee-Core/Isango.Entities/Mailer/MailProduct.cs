using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Entities.Mailer
{
    public class MailProduct
    {
        public int? BookedOptionID { get; set; }
        public int Sequence { get; set; }
        public string Name { get; set; }
        public string OptionName { get; set; }
        public int? OptionID { get; set; }
        public decimal Price { get; set; }
        public string CurrencySymbol { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public decimal Discount { get; set; }
        public decimal Multisave { get; set; }
        public int ServiceId { get; set; }
        public bool IsReceipt { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Youths { get; set; }
        public string TravelDate { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public bool IsReceiptException { get; set; }
        public string LeadPaxName { get; set; }
        public string ChildAges { get; set; }
        public List<string> Comments { get; set; }
        public string Code { get; set; }
        public string FileNumber { get; set; }
        public string VatNumber { get; set; }
        public bool IsSmartPhoneVoucher { get; set; }
        public APIType APIType { get; set; }
        public List<byte[]> ConfirmedTicketBytes { get; set; }
        public string OrderID { get; set; }
        public int Infants { get; set; }
        public int Seniors { get; set; }

        public List<AgeGroupDescription> AgeGroupDescription { get; set; }
        public List<PaxWisePdfEntity> PerPaxPdfDetails { get; set; }
        public bool IsShowSupplierVoucher { get; set; }
        public string LinkType { get; set; }
        public string LinkValue { get; set; }

        public string CancellationPolicy { get; set; }
        public int? BUNDLESERVICEID { get; set; }
    }
}