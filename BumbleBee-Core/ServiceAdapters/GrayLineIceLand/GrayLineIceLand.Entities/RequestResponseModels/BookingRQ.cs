using System;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels
{
    public class BookingRQ
    {
        public int OrderId { get; set; }
        public bool AddExternalBookings { get; set; }
        public string ConfirmToEmail { get; set; }
        public Payment[] Payments { get; set; }
        public Product[] Products { get; set; }
        public string AgentReference { get; set; }
        public string ExternalReference { get; set; }
        public int CreatedBy { get; set; }
        public string CustomerPhone { get; set; }
        public int PriceType { get; set; }
        public string CreatedFrom { get; set; }
        public int Source { get; set; }
        public int CustomerLanguage { get; set; }
        public int AgentProfileId { get; set; }
        public string CurrencyCode { get; set; }
        public bool DontMatchOnAgentReference { get; set; }
        public object[] Notes { get; set; }
        public string RoomNumber { get; set; }
        public string PromoCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public int CountryId { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public int Action { get; set; }
        public string CustomerEmail { get; set; }
        public string MobileNumber { get; set; }
        public string LeadPassengerLastName { get; set; }
        public bool AutoCheckIn { get; set; }
        public string AffiliateId { get; set; }
        public string LeadPassengerFirstName { get; set; }
        public DateTime Created { get; set; }
    }

    public class Product
    {
        public int PickupLocationId { get; set; }
        public int DropOffLocationId { get; set; }
        public object[] AdditionalInfo { get; set; }
        public int DimensionId { get; set; }
        public string PickupTime { get; set; }
        public string RoomType { get; set; }
        public decimal Price { get; set; }
        public int HotelId { get; set; }
        public Passenger[] Passengers { get; set; }
        public int RoomAvailabilityId { get; set; }
        public string DropOffTime { get; set; }
        public int DropOffListId { get; set; }
        public string Departure { get; set; }
        public string Description { get; set; }
        public object[] Notes { get; set; }
        public int RoomTypeId { get; set; }
        public string DropOffFlightNumber { get; set; }
        public string DropOffLocation { get; set; }
        public string PickupFlightNumber { get; set; }
        public string TourNumber { get; set; }
        public int TourDepartureId { get; set; }
        public string ExternalReference { get; set; }
        public string ProductNumber { get; set; }
        public int TourId { get; set; }
        public string PickupLocation { get; set; }
        public int Action { get; set; }
        public string PickupPlaceText { get; set; }
    }
}