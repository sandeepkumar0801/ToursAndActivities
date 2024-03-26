using Isango.Entities.Activities;
using Isango.Entities.HotelBeds;
using Isango.Entities.Ticket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceAdapters.HotelBeds
{
    public interface ITicketAdapter
    {
        List<Activity> GetTicketAvailability(TicketCriteria criteria, string authString, string token);

        Task<List<Activity>> GetTicketAvailabilityAsync(TicketCriteria criteria, string authString, string token);

        List<HotelBedsSelectedProduct> AddTicket(HotelBedsSelectedProduct hotelBedsSelectedProduct, string authString, string token);

        Task<HotelBedsSelectedProduct> AddTicketAsync(HotelBedsSelectedProduct hotelBedsSelectedProduct,
            string authString, string token);

        HotelBedsSelectedProduct GetTicketPrice(HotelBedsSelectedProduct hotelBedsSelectedProduct, string authString, string token);

        Task<HotelBedsSelectedProduct> GetTicketPriceAsync(HotelBedsSelectedProduct hotelBedsSelectedProduct,
            string authString, string token);

        List<HotelBedsSelectedProduct> PurchaseConfirm(List<HotelBedsSelectedProduct> selectedProducts,
            string authString, string token, out string request, out string response);

        List<HotelBedsSelectedProduct> PurchaseDetails(List<HotelBedsSelectedProduct> selectedProducts, out string requestXml, out string responceXml,
            string authString, string token);

        bool ServiceRemove(List<HotelBedsSelectedProduct> selectedProducts, string authString, string token);

        bool PurchaseCancel(List<HotelBedsSelectedProduct> selectedProducts, string authString, out string requestXml, out string responceXml, string token);
    }
}