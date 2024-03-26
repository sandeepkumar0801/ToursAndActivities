using Isango.Entities;
using Isango.Entities.Redeam;
using ServiceAdapters.Redeam.Redeam.Entities.CreateHold;
using ServiceAdapters.Redeam.Redeam.Entities.GetAvailability;
using ServiceAdapters.Redeam.Redeam.Entities.GetRate;
using ServiceAdapters.Redeam.Redeam.Entities.GetRates;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceAdapters.Redeam
{
    public interface IRedeamAdapter
    {
        Task<List<ProductOption>> GetAvailabilities(RedeamCriteria criteria, string token);

        Task<GetRatesResponse> GetRates(RedeamCriteria criteria, string token);

        Task<List<SupplierData>> GetSuppliers(string token);

        Task<List<ProductData>> GetProducts(RedeamCriteria criteria, string token);

        Task<RatesWrapper> GetRatesWrapper(RedeamCriteria criteria, string token);

        Task<SelectedProduct> CreateHold(SelectedProduct selectedProducts, string token);

        Task<Dictionary<string, string>> DeleteHold(List<string> holdIds, string token);

        SelectedProduct CreateBooking(SelectedProduct selectedProducts, string tokenId, out string apiRequest, out string apiResponse);

        Task<SelectedProduct> CreateBooking(SelectedProduct selectedProducts, string token);

        Task<Dictionary<string, string>> CancelBooking(List<string> bookingReferenceNumbers, string token);

        bool CancelBooking(string bookingReferenceNumber, string token, out string request, out string response);

        Task<GetRateResponse> GetSingleRate(RedeamCriteria criteria, string token);

        AvailabilityResponse GetSingleAvailability(RedeamCriteria criteria, string token);
        Task<CreateHoldResponse> CreateHoldAPIOnly(SelectedProduct selectedProducts, string token);
    }
}