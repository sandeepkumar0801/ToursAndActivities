using Isango.Entities;
using Isango.Entities.Activities;
using System.Collections.Generic;

namespace Isango.Service.Contract
{
    public interface ISupplierService
    {
        Criteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo);

        Activity GetAvailability(Activity activity, Criteria criteria, string token);

        Activity MapActivity(Activity activity, List<Activity> activitiesFromAPI, Criteria criteria);
    }
}