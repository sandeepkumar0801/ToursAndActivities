using Isango.Entities.GlobalTix;
using Isango.Entities.GlobalTixV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Persistence.Contract
{
    public interface IGlobalTixPersistence
    {
        void SaveCountryCityList(List<Entities.GlobalTix.CountryCity> countryCities);
        void SaveCountryCityListV3(List<Entities.GlobalTixV3.CountryCityV3> countryCities);
        void SaveProductInfoListV3(List<Entities.GlobalTixV3.ProductInfoV3> productInfoV3);
        void SaveSaveGlobalTixCategoriesListV3(List<Entities.GlobalTixV3.GlobalTixCategoriesV3> categoriesV3List);
        void SaveGlobalTixProductChangesV3(List<Entities.GlobalTixV3.ProductChangesV3> ProductChangesV3List);
        void SaveAllActivities(List<GlobalTixActivity> gtActivities);

        void SaveGlobalTixPackageOptionsV3(List<Entities.GlobalTixV3.PackageOptionsV3> packageOptionsV3List);

        void SaveGlobalTixPackageTypeIdV3(List<Entities.GlobalTixV3.PackageOptionsV3.PackageType> PackageTypeIdV3List);

        void SaveAllPackages(List<GlobalTixPackage> gtPackages);
        List<Entities.GlobalTix.CountryCity> GetCountryCityList();
        List<Entities.GlobalTixV3.CountryCityV3> GetCountryCityV3(int maxRecords);

        void SaveAllActivitiesV3(List<Entities.GlobalTixV3.ProductList> gtActivities, List<ProductOptionV3> gtProductOption, List<Tickettype> tickettypes);
        //void SaveProductOptionV3(List<ProductOptionV3> gtProductOption);
        void UpdateGlobalTixV3Data();
    }
}
