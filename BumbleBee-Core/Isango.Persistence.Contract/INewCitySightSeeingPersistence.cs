
using Isango.Entities;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Product;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface INewCitySightSeeingPersistence
    {
       void SaveNewCitySightSeeingProducts(List<Products> products);
        
    }
}
