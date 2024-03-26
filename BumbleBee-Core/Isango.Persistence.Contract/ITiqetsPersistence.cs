using Isango.Entities.Tiqets;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface ITiqetsPersistence
    {
        void SaveAllVariants(Dictionary<int, List<ProductVariant>> products);
        void SaveAllDetails(List<ProductDetails> productDetails);
        void SaveTiqetsPackage(List<PackageProducts> PackageIDs);

        void SaveMediaImages(List<ContentMedia> contentMedia);
        void SyncDataInVariantTemp();
    }
}