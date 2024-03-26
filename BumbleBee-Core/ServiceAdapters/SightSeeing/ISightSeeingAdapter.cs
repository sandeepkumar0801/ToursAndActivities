using Isango.Entities;
using Isango.Entities.CitySightseeing;

namespace ServiceAdapters.SightSeeing
{
    public interface ISightSeeingAdapter
    {
        List<CitySightseeingSelectedProduct> IssueTicket(List<SelectedProduct> lstSelectedProducts, string bookingReferenceNumber, string token, out string requestXml, out string responseXml);

        Task<List<CitySightseeingSelectedProduct>> IssueTicketAsync(List<SelectedProduct> lstSelectedProducts, string token);

        bool ConfirmTicket(List<SelectedProduct> lstSelectedProducts, string token, out string requestXml, out string responseXml);

        Task<bool> ConfirmTicketAsync(List<SelectedProduct> lstSelectedProducts, string token);

        Task<Dictionary<string, bool>> CancelTicketAsync(List<SelectedProduct> lstSelectedProducts, string token);

        Dictionary<string, bool> CancelTicket(List<SelectedProduct> lstSelectedProducts, string token, out string request, out string response);
    }
}