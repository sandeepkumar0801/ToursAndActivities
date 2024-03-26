using Isango.Entities;
using Isango.Entities.Activities;
using System.Collections.Generic;
using Activity = Isango.Entities.Activities.Activity;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.Contract
{
    public interface IAotCriteriaService
    {
        List<Activity> GetAvailability(ServiceAvailability.Criteria criteria);

        List<ServiceAvailability.TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts);

        List<GoogleCancellationPolicy> GetCancellationPolicies(List<IsangoHBProductMapping> aotProducts, string token);
    }
}