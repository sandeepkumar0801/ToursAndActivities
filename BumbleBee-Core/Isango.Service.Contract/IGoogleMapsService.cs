
namespace Isango.Service.Contract
{
    public interface IGoogleMapsService
    {
        bool LoadMerchantFeeds();

        bool LoadServiceAvailabilityFeeds();

        bool InventoryRealTimeUpdate();

        bool OrderNotificationRealTimeUpdate();
    }
}