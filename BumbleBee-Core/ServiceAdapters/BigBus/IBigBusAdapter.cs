using Isango.Entities;

namespace ServiceAdapters.BigBus
{
    public interface IBigBusAdapter
    {
        List<SelectedProduct> CreateBooking(List<SelectedProduct> selectedProducts, string token, out string request, out string response);

        Dictionary<string, bool> CancelBooking(List<SelectedProduct> selectedProducts, string token, out string request, out string response);

        List<SelectedProduct> CreateReservation(List<SelectedProduct> selectedProducts, string token, out string request, out string response);

        Dictionary<string, bool> CancelReservation(List<SelectedProduct> selectedProducts, string token, out string request, out string response);
    }
}