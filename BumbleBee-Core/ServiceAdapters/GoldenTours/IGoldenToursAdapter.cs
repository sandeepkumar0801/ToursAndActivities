using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.GoldenTours;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.Availability;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.GetBookingDates;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.PickupPoints;

namespace ServiceAdapters.GoldenTours
{
    public interface IGoldenToursAdapter
    {
        List<Activity> GetProductDetails(GoldenToursCriteria criteria, string tokenId);

        Task<List<ProductOption>> GetProductDetailsAsync(GoldenToursCriteria criteria, string tokenId);

        AvailabilityResponse GetAvailability(GoldenToursCriteria criteria, string tokenId);

        Task<AvailabilityResponse> GetAvailabilityAsync(GoldenToursCriteria criteria, string tokenId);

        List<DateTime> GetProductDates(GoldenToursCriteria criteria, string tokenId);

        Task<List<DateTime>> GetProductDatesAsync(GoldenToursCriteria criteria, string tokenId);

        GetBookingDatesResponse GetBookingDates(GoldenToursCriteria criteria, string tokenId);

        Task<GetBookingDatesResponse> GetBookingDatesAsync(GoldenToursCriteria criteria, string tokenId);

        PickupPointsResponse GetPickupPoints(string productId, string tokenId);

        Task<PickupPointsResponse> GetPickupPointsAsync(string productId, string tokenId);

        List<SelectedProduct> CreateBooking(List<SelectedProduct> selectedProducts, string tokenId, out string apiRequest, out string apiResponse);

        Task<List<SelectedProduct>> CreateBookingAsync(List<SelectedProduct> selectedProducts, string tokenId);

        List<ProductOption> GetPriceAvailabilityForDumping(GoldenToursCriteria criteria, string tokenId);

        AgeGroupWrapper GetProductDetailsResponse(GoldenToursCriteria criteria, string tokenId);
    }
}