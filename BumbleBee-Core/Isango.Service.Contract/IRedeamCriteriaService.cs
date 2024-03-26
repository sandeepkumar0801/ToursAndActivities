using Isango.Entities;
using Isango.Entities.Activities;
using System.Collections.Generic;

namespace Isango.Service.Contract
{
    public interface IRedeamCriteriaService
    {
        List<Activity> GetAvailability(Entities.ConsoleApplication.ServiceAvailability.Criteria criteria);

        List<Entities.ConsoleApplication.ServiceAvailability.TempHBServiceDetail> GetServiceDetails(List<Activity> activities,
            List<IsangoHBProductMapping> mappedProducts);
    }
}