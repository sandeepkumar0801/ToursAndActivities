using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.HotelBeds;
using System;
using System.Collections.Generic;

namespace Isango.Service.Contract
{
    public interface IApiTudeCriteriaService
    {
        Tuple<List<Isango.Entities.Activities.Activity>, List<ApiTudeAgeGroup>> GetAvailability(Entities.ConsoleApplication.ServiceAvailability.Criteria serviceCriteria);

        List<Entities.ConsoleApplication.ServiceAvailability.TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts);
    }
}