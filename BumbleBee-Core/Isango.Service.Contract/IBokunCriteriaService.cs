using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Bokun;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using System.Collections.Generic;
using Criteria = Isango.Entities.ConsoleApplication.ServiceAvailability.Criteria;

namespace Isango.Service.Contract
{
    public interface IBokunCriteriaService
    {
        List<Activity> GetAvailability(Criteria serviceCriteria, List<PriceCategory> priceCategoryIdMapping);

        List<GoogleCancellationPolicy> GetBokunCancellationPolicies(List<IsangoHBProductMapping> products, string token);

        List<TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts);
    }
}