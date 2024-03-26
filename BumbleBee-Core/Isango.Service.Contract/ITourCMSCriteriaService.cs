using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using System.Collections.Generic;

namespace Isango.Service.Contract
{
    public interface ITourCMSCriteriaService
    {
        List<Activity> GetAvailability(Entities.ConsoleApplication.ServiceAvailability.Criteria criteria);

        List<TempHBServiceDetail> GetServiceDetails(List<Activity> activities,
        List<IsangoHBProductMapping> mappedProducts);
    }
}