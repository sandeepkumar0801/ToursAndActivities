using Isango.Entities.GoogleMaps;
using System.Collections.Generic;
using Isango.Entities.GoogleMaps.BookingServer;
using ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO;

namespace ServiceAdapters.GoogleMaps
{
    public interface IGoogleMapsAdapter
    {
        bool UploadMerchantFeed(List<MerchantFeed> merchantFeeds);

        bool UploadServiceFeed(ServiceAvailabilityDto serviceAvailabilityDto );

        bool UploadAvailabilityFeed(ServiceAvailabilityDto serviceAvailabilityDto);

        bool ProcessInventoryRealTimeUpdate(List<MerchantActivitiesDto> merchantActivitiesDtos);

        List<Order> ProcessOrderNotification(List<Order> orders);
    }
}