using Isango.Entities;
using Isango.Entities.Aot;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using OptionGeneralInfoResponse = Isango.Entities.ConsoleApplication.AgeGroup.AOT.OptionGeneralInfoResponse;

namespace ServiceAdapters.Aot
{
    public interface IAotAdapter
    {
        void SetAgentIdPassword(CountryType countryType);

        GetLocationsResponse GetLocations(GetLocationsRequest getLocationRequestst, string token);

        Task<GetLocationsResponse> GetLocationsAsync(GetLocationsRequest getLocationRequest, string token);

        SupplierInfoResponse GetSupplierInformation(SupplierInfoRequest supplierInfoRequest, string token);

        Task<SupplierInfoResponse> GetSupplierInformationAsync(SupplierInfoRequest supplierInfoRequest, string token);

        OptionGeneralInfoResponse GetProductDetails(OptionGeneralInfoRequest optionGeneralInfoRequest, string token);

        Task<OptionGeneralInfoResponse> GetProductDetailsAsync(OptionGeneralInfoRequest optionGeneralInfoRequest, string token);

        object GetDetailedPricingAvailability(AotCriteria criteria, string token, bool isBulkPricingResponseRequired = true);

        Task<object> GetDetailedPricingAvailabilityAsync(AotCriteria criteria, string token);

        object GetBulkPricingAvailabilityDetails(AotCriteria criteria, string token);

        Task<object> GetBulkPricingAvailabilityDetailsAsync(AotCriteria criteria, string token);

        object CreateBooking(List<SelectedProduct> addBookingRequest, string token, out string request, out string response);

        object CreateBooking(List<SelectedProduct> addBookingRequest, string token, string referenceNumber, out string request, out string response);

        Task<object> CreateBookingAsync(AddBookingRequest addBookingRequest, string token);

        CancelServicesResponse CancelEntireBooking(string referenceNumber, string token);

        Task<CancelServicesResponse> CancelEntireBookingAsync(CancelServicesRequest cancelServicesRequest, string token);

        //CancelServiceResponse CancelSingleServiceBooking(string referenceNumber, string serviceLineNumber, string token);

        CancelServiceResponse CancelSingleServiceBooking(string referenceNumber, string serviceLineNumber, string token, out string request, out string response);

        Task<CancelServiceResponse> CancelSingleServiceBookingAsync(CancelServiceRequest cancelServicesRequest, string token);

        ListBookingsResponse GetBookingList(ListBookingsRequest listBookingsRequest, string token);

        Task<ListBookingsResponse> GetBookingListAsync(ListBookingsRequest listBookingsRequest, string token);

        GetBookingResponse GetBooking(GetBookingRequest getBookingRequest, string token);

        Task<GetBookingResponse> GetBookingAsync(GetBookingRequest getBookingRequest, string token);

        AddServiceResponse UpdateBooking(AddServiceRequest addServiceRequest, string token);

        Task<AddServiceResponse> UpdateBookingAsync(AddServiceRequest addServiceRequest, string token);

        T DeSerializeXml<T>(string responseXmlString);

        object GetPricingAvailabilityForDumping(AotCriteria criteria, string token);
    }
}