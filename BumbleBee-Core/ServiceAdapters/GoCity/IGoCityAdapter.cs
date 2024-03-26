using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.GoCity;
using ServiceAdapters.GoCity.GoCity.Entities.Product;


namespace ServiceAdapters.GoCity
{
    public interface IGoCityAdapter
    {
        ProductResponse ProductsAsync(
            GoCityCriteria goCityCriteria,
            string token, out string request, out string response);
        SelectedProduct CreateBooking(
           Booking booking, string token,
           out string request, out string response);

        bool? CancelBooking(string ordernum,
            string customerEmail, string token,
         out string request, out string response);

    }
}