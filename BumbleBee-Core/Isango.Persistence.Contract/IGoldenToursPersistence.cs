using Isango.Entities.GoldenTours;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IGoldenToursPersistence
    {
        void SaveGoldenToursProductDetails(List<ProductDetail> productDetails);
        void SaveGoldenToursPricePeriods(List<Periods> pricePeriods);

        void SaveGoldenToursAgeGroups(List<AgeGroup> ageGroups);
    }
}