using Isango.Entities;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IPickupLocationsCacheManager
    {
        bool CreateCollection();

        List<PickupLocation> GetPickupLocationsByActivity(int productId);

        bool InsertDocuments(PickupLocation pickupLocation);
    }
}