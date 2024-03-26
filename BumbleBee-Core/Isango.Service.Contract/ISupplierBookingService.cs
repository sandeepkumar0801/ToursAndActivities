using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.FareHarbor;
using Isango.Entities.HotelBeds;
using Isango.Entities.Prio;
using Isango.Entities.PrioHub;
using Isango.Entities.Rayna;
using Isango.Entities.Tiqets;
using Isango.Entities.Ventrata;
using System.Collections.Generic;

namespace Isango.Service.Contract
{
    public interface ISupplierBookingService
    {
        /// <summary>
        /// Create Ventrata products booking
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        List<BookedProduct> CreateVentrataProductsBooking(ActivityBookingCriteria criteria);

        Dictionary<string, bool> VentrataCancelReservationAndBooking(List<VentrataSelectedProduct> ventrataSelectedProducts, string bookingReferenceNumber, string token, Booking booking);

        Dictionary<string, bool> CancelTiqetsBooking(List<TiqetsSelectedProduct> ventrataSelectedProducts, string bookingReferenceNumber, string token, string languageCode,string affiliateId="");

        /// <summary>
        /// Create Prio Products Booking
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        List<BookedProduct> CreatePrioProductsBooking(ActivityBookingCriteria criteria);

        /// <summary>
        /// Prio Cancel Reservation And Booking
        /// </summary>
        Dictionary<string, bool> PrioCancelReservationAndBooking(List<PrioSelectedProduct> prioSelectedProducts, string bookingReferenceNumber, string token);

        List<BookedProduct> CreateHbActivityBooking(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateGraylineIcelandBooking(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateMoulinRougeProductsBooking(ActivityBookingCriteria criteria);

        List<BookedProduct> GenerateSightSeeingQRCode(ActivityBookingCriteria criteria);

        Dictionary<string, bool> GrayLineIceLandDeleteBooking(List<SelectedProduct> gliSelectedProducts, string supplierReferenceNumber, string bookingReference, string token);

        Dictionary<string, bool> TicketAdapterPurchaseCancel(List<HotelBedsSelectedProduct> selectedProducts, string authentication, string bookingReferenceNumber, string token);

        Dictionary<string, bool> FareharborDeleteBooking(List<FareHarborSelectedProduct> fareHarborSelectedProducts, string bookingReferenceNumberstring, string token);

        Dictionary<string, bool> CancelBokunBooking(List<SelectedProduct> selectedProducts, string confirmationCode, string bookingReferenceNumber, string token);

        Dictionary<string, bool> CancelAotBooking(List<BookedProduct> bookedProducts, string bookingReferenceNumber, string token);

        List<BookedProduct> CreateFareHarborBooking(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateBokunBooking(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateAotBooking(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateTiqetsBooking(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateBigBusBooking(ActivityBookingCriteria criteria);

        Dictionary<string, bool> CancelBigBusBooking(List<SelectedProduct> selectedProducts, string token);

        Dictionary<string, bool> CancelSightSeeingBooking(List<SelectedProduct> selectedProducts, string token);

        List<BookedProduct> CreateGoldenToursProductsBooking(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateRedeamBooking(ActivityBookingCriteria criteria);

        Dictionary<string, bool> CancelRedeamBooking(List<SelectedProduct> selectedProducts, string token);

        List<BookedProduct> CreateRezdyBooking(ActivityBookingCriteria criteria);

        Dictionary<string, bool> CancelRezdyBooking(List<SelectedProduct> selectedProducts, string bookingReferenceNumber, string token);

        List<BookedProduct> CreateGlobalTixBooking(ActivityBookingCriteria criteria);

        Dictionary<string, bool> CancelGlobalTixBooking(List<SelectedProduct> selectedProducts, string token);

        List<BookedProduct> CreateTourCMSActivityBooking(ActivityBookingCriteria criteria);
        Dictionary<string, bool> CancelTourCMSBooking(List<SelectedProduct> selectedProducts,
            string bookingReferenceNumber, string token);

        List<BookedProduct> CreateNewCitySightSeeingBooking(ActivityBookingCriteria criteria);

        Dictionary<string, bool> CancelNewCitySightSeeingBooking(List<SelectedProduct> selectedProducts,
            string bookingReferenceNumber, string token);

        List<BookedProduct> CreateGoCityBooking(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateRaynaBooking(ActivityBookingCriteria criteria);

        Dictionary<string, bool> CancelGoCityBooking(List<SelectedProduct>
           selectedProducts, string bookingReferenceNumber, string token,
           string customerEmail);

      
        List<BookedProduct> CreatePrioHubProductsBooking(ActivityBookingCriteria criteria);

        Dictionary<string, bool> PrioHubCancelReservationAndBooking(List<PrioHubSelectedProduct> prioHubSelectedProducts, string bookingReferenceNumber, string token,string languageCode="", string affiliateId="",string email="");


        void LogBookingFailureInDB(Booking booking, string bookingRefNo, int? serviceID, string tokenID, string apiRefID, string custEmail, string custContact, int? ApiType, int? optionID, string optionName, string avlbltyRefID, string ErrorLevel);

         Dictionary<string, bool> CancelRaynaBooking(List<RaynaSelectedProduct> raynaSelectedProducts, string bookingReferenceNo, string token);
        List<BookedProduct> CreateTiqetsBookingReservation(ActivityBookingCriteria criteria);

        List<BookedProduct> CreatePrioHubProductsBookingReservation(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateTourCMSProductsBookingReservation(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateVentrataProductsBookingReservation(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateNewCitySightSeeingReservation(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateRedeamBookingReservation(ActivityBookingCriteria criteria);

        List<BookedProduct> CreateMoulinRougeBookingReservation(ActivityBookingCriteria criteria);

    }
}