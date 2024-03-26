using Isango.Entities.GoogleMaps;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IGoogleMapsPersistence
    {
        List<MerchantFeed> GetMerchantData();

        List<AssignedServiceMerchant> GetAssignedServiceMerchant();

        List<PassengerType> GetPassengerTypes();
    }
}