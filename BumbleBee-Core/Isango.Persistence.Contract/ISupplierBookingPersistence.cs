using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.RedeamV12;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface ISupplierBookingPersistence
    {
        List<CitySightseeingMapping> GetSightseeingMapping(int optionId);

        void LogPurchaseXML(LogPurchaseXmlCriteria criteria);

        void LogBookingFailureInDB(Booking booking, string bookingRefNo, int? serviceID, string tokenID, string apiRefID, string custEmail, string custContact, int? ApiType, int? optionID, string optionName, string avlbltyRefID, string ErrorLevel);

        void SaveMoulinRougeTicketBytes(Entities.MoulinRouge.MoulinRougeSelectedProduct mulinRougeSelectedProduct);

        void InsertReserveRequest(string token, string availabilityRefID);

        void UpdateReserveRequest(string token, string availabilityRefID, string bookingRefNo);

        List<SupplierData> GetRedeamV12SupplierData();
    }
}