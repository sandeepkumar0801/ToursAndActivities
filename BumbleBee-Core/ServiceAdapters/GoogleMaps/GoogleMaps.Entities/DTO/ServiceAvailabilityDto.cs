using System.Collections.Generic;
using Isango.Entities.GoogleMaps;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO
{
    public class ServiceAvailabilityDto
    {
        public List<MerchantActivitiesDto> MerchantActivitiesDtos { get; set; }
        public List<PassengerType> PassengerTypes { get; set; }
    }
}