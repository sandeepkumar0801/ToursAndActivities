using System.Collections.Generic;
using System.Linq;
using Isango.Entities.Ventrata;

namespace Isango.Persistence.Contract
{
    public interface IVentrataPersistence
    {
        void SaveProductDetails(List<ProductDetail> productDetails);
        void SaveDestinationDetails(List<Destination> destinations);
        void SaveFaqs(List<FAQ> faqs);
        void SaveOptionDetails(List<Option> options);
        void SaveUnitsDetailsOfOption(List<UnitsForOption> unitDetailsOfOption);

        void SavePackagesInclude(List<PackageInclude> package);
    }
}
