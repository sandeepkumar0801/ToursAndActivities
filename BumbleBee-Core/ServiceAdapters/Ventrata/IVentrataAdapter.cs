using Isango.Entities.Activities;
using Isango.Entities.Ventrata;

namespace ServiceAdapters.Ventrata
{
    public interface IVentrataAdapter
    {
        List<Activity> GetOptionsForVentrataActivity(VentrataAvailabilityCriteria criteria, string token);
        object CreateReservation(VentrataSelectedProduct selectedProduct, out string request, out string response, string token, List<VentrataPackages> packages);
        object GetAllProducts(string supplierBearerToken, string token, string baseURL);
        object CreateBooking(VentrataSelectedProduct selectedProduct, string token, out string request, out string response);
        string CancelReservationAndBooking(VentrataSelectedProduct selectedProduct, string token, out string request, out string response);

        object GetCustomQuestions(string supplierBearerToken, string token, string baseURL,string id);
    }
}
