using Isango.Entities;
using Isango.Entities.RedeamV12;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.CreateHold;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.GetAvailability;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.GetRate;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.GetRates;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.PricingSchedule;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ServiceAdapters.RedeamV12
{
    public interface IRedeamV12Adapter
    {
        Task<List<ProductOption>> GetAvailabilities(CanocalizationCriteria criteria, string token);

        Task<GetRatesResponse> GetRates(CanocalizationCriteria criteria, string token);

        Task<List<SupplierData>> GetSuppliers(string token);

        Task<List<ProductData>> GetProducts(CanocalizationCriteria criteria, string token);

        Task<RatesWrapper> GetRatesWrapper(CanocalizationCriteria criteria, string token);

        SelectedProduct CreateHold(SelectedProduct selectedProducts, string token, out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode);

        Task<Dictionary<string, string>> DeleteHold(List<string> holdIds, string token);

        SelectedProduct CreateBooking(SelectedProduct selectedProducts, string tokenId, out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode);

        Task<SelectedProduct> CreateBooking(SelectedProduct selectedProducts, string token);

        Task<Dictionary<string, string>> CancelBooking(List<string> bookingReferenceNumbers, string token);

        bool CancelBooking(string bookingReferenceNumber, string token, out string request, out string response, out HttpStatusCode httpStatusCode);

        Task<GetRateResponse> GetSingleRate(CanocalizationCriteria criteria, string token);

        AvailabilityResponse GetSingleAvailability(CanocalizationCriteria criteria, string token);
        CreateHoldResponse CreateHoldAPIOnly(SelectedProduct selectedProducts, string token,out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode);

        Task<Dictionary<string, Dictionary<string, List<PricingScheduleResponse>>>> GetPricingSchedule(CanocalizationCriteria criteria, string token);
    }
}