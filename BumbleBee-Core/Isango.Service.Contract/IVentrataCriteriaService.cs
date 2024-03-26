using System.Collections.Generic;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Criteria = Isango.Entities.ConsoleApplication.ServiceAvailability.Criteria;

namespace Isango.Service.Contract
{
    public interface IVentrataCriteriaService
    {
        List<Activity> GetAvailability(Criteria criteria, List<Entities.Ventrata.SupplierDetails> supplierDetails);

        List<TempHBServiceDetail> GetServiceDetails(List<Activity> activities, List<IsangoHBProductMapping> mappedProducts);
    }
}
