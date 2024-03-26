using Isango.Entities;
using Isango.Entities.Rezdy;

using ServiceAdapters.Rezdy.Rezdy.Entities.CancelBooking;

namespace ServiceAdapters.Rezdy
{
    public interface IRezdyAdapter
    {
        Task<List<RezdyProduct>> GetAllRezdyProducts(int supplierId, string SupplierAlias, string token);

        RezdyProduct GetProductDetails(string productCode, string token);

        Task<List<ProductOption>> GetAvailability(RezdyCriteria criteria, string token);

        List<SelectedProduct> CreateBooking(List<SelectedProduct> selectedProducts, string token, out string apiRequest, out string apiResponse);

        CancelBookingResponse CancelBooking(string orderNumber, string token, out string apiRequest, out string apiResponse);

        List<RezdyPickUpLocation> GetPickUpLocationDetails(int pickUpId, string token);
    }
}