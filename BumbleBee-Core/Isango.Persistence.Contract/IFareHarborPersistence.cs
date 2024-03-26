using Isango.Entities;
using Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor;
using System.Collections.Generic;
using Product = Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor.Product;

namespace Isango.Persistence.Contract
{
    public interface IFareHarborPersistence
    {
        void SaveAllCustomerProtoTypes(List<CustomerProtoTypeCustomerType> customerPrototypes);

        void SaveAllCompanies(List<Supplier> suppliers);

        void SaveAllCompanyMappings(List<CompaniesMapping> companiesMapping);

        List<Product> LoadProducts();

        List<FareHarborUserKey> GetUserKeys();

        void SyncDataBetweenDataBases();
    }
}