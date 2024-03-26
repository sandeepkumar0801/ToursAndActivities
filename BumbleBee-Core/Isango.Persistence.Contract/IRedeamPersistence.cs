using Isango.Entities.Redeam;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IRedeamPersistence
    {
        void SaveSuppliers(List<SupplierData> supplierData);

        void SaveProducts(List<ProductData> productData);

        void SaveRates(List<RateData> rateData);

        void SavePrices(List<PriceData> priceData);

        void SaveAgeGroups(List<PassengerTypeData> passengerTypeData);

        void SaveSuppliersV12(List<Isango.Entities.RedeamV12.SupplierData> supplierData);
        void SaveProductsV12(List<Isango.Entities.RedeamV12.ProductData> productData);

        void SaveRatesV12(List<Isango.Entities.RedeamV12.RateData> rateData);

        void SavePricesV12(List<Isango.Entities.RedeamV12.PriceData> priceData);

        void SaveAgeGroupsV12(List<Isango.Entities.RedeamV12.PassengerTypeData> passengerTypeData);
    }
}