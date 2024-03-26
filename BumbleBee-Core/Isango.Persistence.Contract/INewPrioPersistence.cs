

using ServiceAdapters.PrioHub.PrioHub.Entities.ProductListResponse;
using ServiceAdapters.PrioHub.PrioHub.Entities.RouteResponse;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface INewPrioPersistence
    {
        void SavePrioHubProducts(List<Item> products);
        void SavePrioHubProductsRoutes(List<ItemRoute> products);
    }
}
