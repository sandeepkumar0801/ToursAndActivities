using Isango.Entities.Activities;
using Isango.Entities.PrioHub;
using ServiceAdapters.PrioHub.PrioHub.Entities.GetVoucherRes;
using ServiceAdapters.PrioHub.PrioHub.Entities.ProductListResponse;
using ServiceAdapters.PrioHub.PrioHub.Entities.RouteResponse;
using ReservationData = ServiceAdapters.PrioHub.PrioHub.Entities.ReservationResponse;

namespace ServiceAdapters.PrioHub
{
    public interface IPrioHubAdapter
    {
        List<Item> ProductsAsync(
          PrioHubCriteria PrioHubCriteria,
          string token, out string request, out string response);

        List<ItemRoute> ProductRoutesAsync(
           PrioHubCriteria PrioHubCriteria,
           string token, out string request, out string response);

        List<Activity> UpdateOptionforPrioHubActivity(PrioHubCriteria criteria, string token);

        Tuple<string, string, string,string> CreateReservation(Isango.Entities.PrioHub.PrioHubSelectedProduct selectedProduct,
          string distributorReference, out string request, out string response, string token, string bookingReference,
          string prioHubAvailabilityId, List<PrioHubProductPaxMapping> prioHubProductPaxMapping);

        PrioHubAPITicket CreateBooking(PrioHubSelectedProduct selectedProduct, string token, out string request, out string response,string referenceNumber);

        Tuple<string, string, string, DateTime> CancelReservation(PrioHubSelectedProduct selectedProduct, string token, out string request, out string response);
        Tuple<string, string, string, string, string, DateTime> CancelBooking(PrioHubSelectedProduct selectedProduct, string token, out string request, out string response);

        ReservationData.ReservationResponse CreateReservationOnly(Isango.Entities.PrioHub.PrioHubSelectedProduct selectedProduct,
            string distributorReference, out string request, out string response, string token, string bookingReference,
            string prioHubAvailabilityId, List<PrioHubProductPaxMapping> prioHubProductPaxMapping);

         GetVoucherRes GetVoucher(int distributorId, string bookingOrderId,
         string token, out string request, out string response);
    }
}