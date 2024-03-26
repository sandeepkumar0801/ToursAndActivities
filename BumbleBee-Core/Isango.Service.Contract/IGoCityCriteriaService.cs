using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using System.Collections.Generic;

namespace Isango.Service.Contract
{
    public interface IGoCityCriteriaService
    {
        List<Activity> GetAvailability(Entities.ConsoleApplication.ServiceAvailability.Criteria serviceCriteria);

        List<TempHBServiceDetail> GetServiceDetails(List<Activity> activities,
            List<IsangoHBProductMapping> mappedProducts);
    }
}

