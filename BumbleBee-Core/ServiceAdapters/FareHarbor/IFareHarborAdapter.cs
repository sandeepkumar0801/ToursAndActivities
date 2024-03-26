using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.FareHarbor;
using ServiceAdapters.FareHarbor.FareHarbor.Entities;
using ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels;
using Booking = Isango.Entities.Booking;
using Product = Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor.Product;

namespace ServiceAdapters.FareHarbor
{
    public interface IFareHarborAdapter
    {
        List<Supplier> GetCompanies(FareHarborUserKey userKey, string token);

        Task<List<Supplier>> GetCompaniesAsync();

        List<Activity> GetItems(Supplier supplier, string token);

        Task<List<Activity>> GetItemsAsync(Supplier supplier, string token);

        List<Activity> GetAvailabilities(FareHarborCriteria criteria, string token);

        Task<List<Activity>> GetAvailabilitiesAsync(FareHarborCriteria criteria, string token);

        List<Activity> GetAvailabilitiesByDate(FareHarborCriteria criteria, string token);

        Task<List<Activity>> GetAvailabilitiesByDateAsync(FareHarborCriteria criteria, string token);

        Isango.Entities.Booking.Booking CreateBooking(Isango.Entities.Booking.Booking booking, string token);

        List<FareHarborSelectedProduct> CreateBooking(List<FareHarborSelectedProduct> selectedProducts,
            string bookingReferenceNumber, string token, out string request, out string response);

        FareHarborSelectedProduct CreateBooking(FareHarborSelectedProduct selectedProduct,
           string bookingReferenceNumber, string token, out string request, out string response);

        Task<Isango.Entities.Booking.Booking> CreateBookingAsync(Isango.Entities.Booking.Booking booking, string token);

        List<object> ValidateBooking(Isango.Entities.Booking.Booking booking, string token);

        Task<List<object>> ValidateBookingAsync(Isango.Entities.Booking.Booking booking, string token);

        Isango.Entities.Booking.Booking GetBooking(string companyShortName, string bookingReferenceNumber, string token);

        Task<Isango.Entities.Booking.Booking> GetBookingAsync(string companyShortName, string bookingReferenceNumber, string token);

        bool DeleteBooking(List<FareHarborSelectedProduct> selectedProducts, string token);

        Task<Isango.Entities.Booking.Booking>
            DeleteBookingAsync(string companyShortName, string bookingReferenceNumber, string token);

        object GetLodgings(FareHarborRequest fareHarborRequest, string token);

        Task<object> GetLodgingsAsync(FareHarborRequest fareHarborRequest, string token);

        object GetLodgingsAvailability(FareHarborRequest fareHarborRequest, string token);

        Task<object> GetLodgingsAvailabilityAsync(FareHarborRequest fareHarborRequest, string token);

        Isango.Entities.Booking.Booking UpdateBookingNote(Isango.Entities.Booking.Booking booking, string token);

        Task<Isango.Entities.Booking.Booking> UpdateBookingNoteAsync(Isango.Entities.Booking.Booking booking, string token);

        Isango.Entities.Booking.Booking Rebooking(Isango.Entities.Booking.Booking booking, string token);

        Task<Isango.Entities.Booking.Booking> RebookingAsync(Isango.Entities.Booking.Booking booking, string token);

        ItemDTO GetCustomerPrototypesByProductId(Product product, string token);

        Booking.Booking DeleteBooking(string companyShortName, string bookingReferenceNumber, string userKey, out string request, out string response,
            string token);

        AvailabilityLodgingsResponse GetLodgingsAvailability(string shortName, string pK, string userKey, string token);
    }
}