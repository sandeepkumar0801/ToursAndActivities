using System;
using System.Collections.Generic;

namespace Isango.Entities.Booking
{
    public class ConfirmBookingDetail
    {
        public string BookingReferenceNumber { get; set; }
        public int BookingId { get; set; }
        public string LanguageCode { get; set; }
        public string AffiliateId { get; set; }
        public string CurrencyIsoCode { get; set; }
        public string VoucherEmail { get; set; }
        public DateTime BookingDate { get; set; }
        public List<BookedOption> BookedOptions { get; set; }
    }

    public class BookedOption
    {
        public int BookedOptionId { get; set; }
        public int ServiceId { get; set; }
        public int BookedOptionStatusId { get; set; }
        public string LeadPaxName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceStatus { get; set; }
        public DateTime TravelDate { get; set; }
        public decimal SellAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal MutliSaveDiscount { get; set; }
        public decimal ChargedAmount { get; set; }
        public bool IsReceipt { get; set; }
        public bool IsQRCodePerPax { get; set; }
        public string LinkType { get; set; }
        public string LinkValue { get; set; }
        public bool IsShowSupplierVoucher { get; set; }
        public List<BookedPassengerDetail> BookedPassengerDetails { get; set; }

        public string AvailabilityReferenceId { get; set; }
        public string BookedOptionName { get; set; }

        public string SupplierCode { get; set; }
        public int ApiType { get; set; }
    }

    public class BookedPassengerDetail
    {
        public string AgeGroupDescription { get; set; }
        public int PassengerTypeId { get; set; }
        public int PaxCount { get; set; }

        public List<BookedPassengerQRCodeDetail> QRCodeDetail { get; set; }
    }

    public class BookedPassengerQRCodeDetail
    {
        public int? BookedOptionID { get; set; }

        public string BarCode { get; set; }

        public string PassengerType { get; set; }
    }
}