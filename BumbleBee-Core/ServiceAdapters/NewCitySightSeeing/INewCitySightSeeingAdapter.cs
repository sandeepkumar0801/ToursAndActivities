using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.NewCitySightSeeing;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Availability;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Product;

namespace ServiceAdapters.NewCitySightSeeing
{
    public interface INewCitySightSeeingAdapter
    {
        List<Products> ProductsAsync(
            NewCitySightSeeingCriteria newCitySightSeeingCriteria,
            string token, out string request, out string response);
        List<ProductOption> GetActivityInformation(
           NewCitySightSeeingCriteria newCitySightSeeingCriteria,
           string token,
           out string request, out string response);
        List<ProductOption> GetActivityAvailability(
            NewCitySightSeeingCriteria newCitySightSeeingCriteria,
            string token, out string request, out string response);

        List<SelectedProduct> CreateReservation(
        Booking booking, string token,
        out string request, out string response);

        List<SelectedProduct> CreateBooking(
        Booking booking, string token,
        out string request, out string response);

        string CancelBooking(string reservationId,
             string isangoRef, string token,
          out string apiRequest, out string apiResponse);

        AvailabilityResponse GetNullVariantData(
           NewCitySightSeeingCriteria newCitySightSeeingCriteria,
           string token,
           out string request, out string response);


        NewCitySightSeeing.Entities.Reservation.ReservationResponse CreateReservation(
     SelectedProduct selectedProduct, string language, string voucherEmailAddress,
     string voucherPhoneNumber, string referenceNumber,
     string zipCode, string address, string city, string token,
     out string request, out string response);

        SelectedProduct CreateReservationProduct(
         SelectedProduct selectedProduct, string language, string voucherEmailAddress,
          string voucherPhoneNumber, string referenceNumber,
          string zipCode, string address, string city, string token,
        out string request, out string response);

         SelectedProduct CreateBookingSingle(
         SelectedProduct selectedProduct, string language, string voucherEmailAddress,
          string voucherPhoneNumber, string referenceNumber, string token,
         out string request, out string response);
    }
}