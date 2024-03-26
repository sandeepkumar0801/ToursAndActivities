using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.GlobalTixV3;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3
{
    public interface IGlobalTixAdapterV3
    {
        List<CountryCityV3> GetCountryCityListV3(string token);
        Task <List<ProductList>> GetAllActivitiesV3Async(int countryId,  string token, bool v);
        Tuple<List<ProductOptionV3>, List<Tickettype>> GetProductOptionV3(string token, int productId, bool isNonThailandProduct);
        List<ProductChangesV3> GetProductChangesV3(string token,int Countryid, bool isNonThailandProduct);
        Tuple<List<PackageOptionsV3>, List<PackageOptionsV3.PackageType>> GetPackageOptionsV3(string token, int? id);
        //List<PackageOptionsV3> GetPackageOptionsV3(string token, int? id);
       // List<ProductList> GetAllActivitiesV3(string countryId, string cityId, string token, bool v);
        List<GlobalTixCategoriesV3> GetCategoriesListV3(string token);

        Activity GetActivityInformation(Isango.Entities.CanocalizationCriteria gtCriteria, string token, bool isNonThailandProduct);
        List<ProductInfoV3> GetProductInfoV3(int id, string token, bool isNonThailandProduct);

        ReservationRS CreateReservation(SelectedProduct selectedProduct, string bookingReference, string token, out string requestJson, out string responseJson, out HttpStatusCode httpStatusCode);

        SelectedProduct CreateBooking(SelectedProduct selectedProducts, string bookingReference, string token, out string requestJson, out string responseJson, out HttpStatusCode httpStatusCode);

        bool CancelByBooking(string supplierReferenceNumber, string token, out string requestJson, out string responseJson, out HttpStatusCode httpStatusCode, bool isNonThailandProduct);
    }
}
