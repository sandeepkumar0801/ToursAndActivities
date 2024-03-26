using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.GlobalTix;

namespace ServiceAdapters.GlobalTix
{
    public interface IGlobalTixAdapter
    {
        bool CancelByTicket(string supplierReferenceNumber, string token,out string requestJson, out string responseJson);

        bool CancelByBooking(string supplierReferenceNumber, string token, out string requestJson, out string responseJson, bool isNonThailandProduct);

        Booking CreateBooking(List<SelectedProduct> selectedProducts, string bookingReference, string token, out string requestJson, out string responseJson);

        //Task<List<Activity>> GetAvailabilityAndPriceAsync(string countryCode, string cityCode, string token);

        //List<Activity> GetAvailabilityAndPrice(string countryCode, string cityCode, string token);

        Activity GetActivityInformation(GlobalTixCriteria gtCriteria, string token, bool isNonThailandProduct);

		List<CountryCity> GetCountryCityList(string token);

		List<GlobalTixActivity> GetAllActivities(string country, string city, string token, bool isNonThailandProduct);

		List<GlobalTixPackage> GetAllPackages(string token, bool isNonThailandProduct);

        Tuple<List<ContractQuestion>, Dictionary<int, string>> GetTicketTypeDetail(List<string> ticketTypeIds, string token);

    }
}
