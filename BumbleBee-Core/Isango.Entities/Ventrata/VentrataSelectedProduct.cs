using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Ventrata
{
    public class VentrataSelectedProduct : SelectedProduct
    {
        public string Uuid { get; set; }
        public string ResellerReference { get; set; }
        public string SupplierReference { get; set; }
        public string BookingStatus { get; set; }
        public bool TestMode { get; set; }
        public bool IsCancellable { get; set; }
        public string ReasonForCancellation { get; set; }
        public VentrataApiBookingDetails ApiBookingDetails { get; set; }
        public string OrderNumber { get; set; }

        public string VentrataBaseURL { get; set; }
        public string VentrataIsPerPaxQRCode { get; set; }

        public List<VentrataPaxMapping> VentrataPaxMappings { get; set; }

    }

    public class VentrataApiBookingDetails
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string PickUpPoint { get; set; }
        public string ProductId { get; set; }
        public string OptionIdBooked { get; set; }
        public string AvailabilityId { get; set; }
        public BookingLevelVoucher VoucherAtBookingLevel { get; set; }
        public List<UnititemInBookedProduct> UnitItems { get; set; }
        public bool CheckedIn { get; set; }
        public bool CheckinAvailable { get; set; }
        public string CheckinUrl { get; set; }
        public List<string> DeliveryMethods { get; set; }
        public string ApiCancellationPolicy { get; set; }

        public bool? IsPackage { get; set; }

    }

    public class BookingLevelVoucher
    {
        public string RedemptionMethod { get; set; }
        public List<BookingLevelVoucherDeliveryoption> DeliveryOptions { get; set; }
    }
    public class BookingLevelVoucherDeliveryoption
    {
        public string DeliveryFormat { get; set; }
        public string DeliveryValue { get; set; }
    }

    public class UnititemInBookedProduct
    {
        public string Uuid { get; set; }
        public object ResellerReference { get; set; }
        public string SupplierReference { get; set; }
        public string UnitId { get; set; }
        public string Status { get; set; }
        public object UtcRedeemedAt { get; set; }
        public Unit Unit { get; set; }
        public PaxLevelTicket TicketPerUnitItem { get; set; }
    }

    public class Unit
    {
        public string Id { get; set; }
        public string InternalName { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string TitlePlural { get; set; }
        public string Subtitle { get; set; }
    }

    public class PaxLevelTicket
    {
        public string RedemptionMethod { get; set; }
        public List<PaxLevelDeliveryoption> DeliveryOptions { get; set; }
    }

    public class PaxLevelDeliveryoption
    {
        public string DeliveryFormat { get; set; }
        public string DeliveryValue { get; set; }
    }

    public class VentrataExtraQuestion
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public string InputType { get; set; }
        public List<object> SelectOptions { get; set; }
    }
}
