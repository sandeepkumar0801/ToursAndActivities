using Isango.Entities;
using Isango.Entities.Activities;
using System.Collections.Generic;

namespace Isango.Service.Contract
{
    public interface IHotelBedsCriteriaService
    {
        List<Activity> GetAvailability(Entities.ConsoleApplication.ServiceAvailability.Criteria serviceCriteria);

        List<Entities.ConsoleApplication.ServiceAvailability.TempHBServiceDetail> GetServiceDetails(
            List<Activity> activities, List<IsangoHBProductMapping> mappedProducts);
    }
}