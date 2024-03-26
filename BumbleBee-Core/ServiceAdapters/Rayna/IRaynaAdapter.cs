using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Rayna;
using ServiceAdapters.Rayna.Rayna.Entities;

namespace ServiceAdapters.Rayna
{
    public interface IRaynaAdapter
    {
        Countries CountryData(string token, out string request, out string response);
        CityByCountry CityData(int? countryId, string token, out string request, out string response);
        TourStaticData TourStaticData(int countryId, int cityId, string token, out string request, out string response);
        TourStaticDataById TourStaticDataById(int countryId, int cityId, int tourId, int contractId, string travelDate,
           string token, out string request, out string response);

        TourOptions TourOptions(
           int tourId, int contractId,
          string token, out string request, out string response);

        Activity GetActivity(RaynaCriteria raynaCriteria, string token, out string request, out string response);

        List<SelectedProduct> BookingConfirm(
            List<SelectedProduct> selectedProduct,
            string bookingReference, string voucherPhoneNumber,
            string token, out string request, out string response);

        CancelRES CancelBooking(int bookingId, string referenceNo, string cancellationReason, string token,
         out string apiRequest, out string apiResponse);
    }
}