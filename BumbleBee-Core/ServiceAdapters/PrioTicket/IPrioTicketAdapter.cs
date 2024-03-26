using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Prio;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceAdapters.PrioTicket
{
    public interface IPrioTicketAdapter
    {
        List<Activity> UpdateOptionforPrioActivity(PrioCriteria criteria, string token);

        Tuple<string, string, string> CreateReservation(PrioSelectedProduct selectedProduct,
            string distributorReference, out string request, out string response, string token);

        Tuple<string, string, string, DateTime> CancelReservation(PrioSelectedProduct selectedProduct,
            string referenceNumber, string token, out string request, out string response);

        Tuple<string, string, string, DateTime> CancelBooking(PrioSelectedProduct selectedProduct, string token, out string request, out string response);

        PrioApi CreateBooking(PrioSelectedProduct selectedProduct, string token, out string request, out string response);

        ActivityOption GetPrioAvailablity(PrioCriteria criteria, string distributorId, string tokenKey, string apiLoggingToken, string code);

        List<PrioPriceAndAvailability> GetPrioTicketDetails(PrioTicketDetailsCriteria criteria);

        object GetPrioTicketDetails(string activityCode, string token);

        string GetPrioProductCurrencyCode(PrioTicketDetailsCriteria criteria);

        Task<ActivityOption> GetPrioAvailablityAsync(PrioCriteria criteria, string distributorId, string tokenKey, string apiLoggingToken, string code);
        TicketListRs GetPrioTicketList(string token);
    }
}