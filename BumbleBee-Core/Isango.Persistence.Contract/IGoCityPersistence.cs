
using Isango.Entities;
using ServiceAdapters.GoCity.GoCity.Entities.Product;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IGoCityPersistence
    {
       void SaveGoCityProducts(ProductResponse products);
    }
}
