using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Isango.Entities.Booking
{
    public class BookingDetailResponse
    {
        public string BookingReferenceNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public string CustomerEmail { get; set; }
        public string AffiliateId { get; set; }
        public string CurrencyIsoCode { get; set; }
        public string Language { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
    }

    public class ProductDetail
    {
        public string LeadTravellerName { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }
        public bool IsReceipt { get; set; }
        public bool IsShowSupplierVoucher { get; set; }
        public DateTime TravelDate { get; set; }
        public decimal SellAmount { get; set; }
        public decimal MultiSaveAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public List<Passenger> Passengers { get; set; }
        public bool IsQRCodePerPax { get; set; }
        public string LinkType { get; set; }
        public string LinkValue { get; set; }
        public string AvailabilityReferenceId { get; set; }
        public int BookedOptionId { get; set; }
        public string BookedOptionName { get; set; }

        public int ApiType { get; set; }
        public string isangoVoucherLink { get; set; }
    }

    public class Passenger
    {
        public string LinkType { get; set; }
        public string LinkValue { get; set; }
        public string AgeGroupDescription { get; set; }
        public int PaxCount { get; set; }
        public int PassengerTypeId { get; set; }
        
        [JsonIgnore]
        public string QRCodeValue { get; set; }
    }

    public class BookingDetailResponseWithCancellationStatus 
    {
        public BookingDetailResponse BookingDetail { get; set; }
        public List<string> CancellationStatus { get; set; }
    }
}