using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using System.Collections.Generic;
using Criteria = Isango.Entities.ConsoleApplication.ServiceAvailability.Criteria;

namespace Isango.Service.Contract
{
    public interface IPrioCriteriaService
    {
        List<Activity> GetAvailability(Criteria criteria);

        List<TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts);
    }
}