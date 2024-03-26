using Isango.Entities.Bokun;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IBokunPersistence
    {
        void SaveBokunProductDetails(List<Entities.Bokun.Product> productDetails);

        void SaveBokunCancellationPolicies(List<CancellationPolicy> cancellationPolicies);

        void SaveBokunRates(List<Isango.Entities.Bokun.Rate> Rates);

        void SaveBookableExtras(List<Isango.Entities.Bokun.BookableExtras> bookableExtras);

        void BokunSyncCall();
    }
}