namespace ServiceAdapters.GoogleMaps.Constants
{
    public class UriConstants
    {
        public const string OrderNotification = "/notification/partners/{0}/orders/{1}?updateMask={2}";
        public const string InventoryRealTimeUpdate = "/v1alpha/inventory/partners/{0}/availability:replace";
    }
}