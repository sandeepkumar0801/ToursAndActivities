using Isango.Entities;
using Isango.Entities.Activities;
using System.Collections.Generic;
using ServiceAvailability = Isango.Entities.ConsoleApplication.ServiceAvailability;

namespace Isango.Service.Contract
{
    public interface ITiqetsCriteriaService
    {
        List<Activity> GetAvailability(ServiceAvailability.Criteria criteria);

        List<ServiceAvailability.TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts);
    }
}