using Isango.Entities.GoogleMaps;
using ServiceAdapters.GoogleMaps.GoogleMaps.Commands.Contracts;
using System.Collections.Generic;
using Isango.Entities.GoogleMaps.BookingServer;
using System.Threading.Tasks;
using ServiceAdapters.GoogleMaps.GoogleMaps.Entities;
using ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO;
using Util;

namespace ServiceAdapters.GoogleMaps
{
    public class GoogleMapsAdapter : IGoogleMapsAdapter
    {
        #region Properties

        private readonly IMerchantFeedCommandHandler _merchantFeedCommandHandler;
        private readonly IServiceFeedCommandHandler _serviceFeedCommandHandler;
        private readonly IAvailabilityFeedCommandHandler _availabilityFeedCommandHandler;
        private readonly IInventoryRealTimeUpdateCommandHandler _inventoryRealTimeUpdateCommandHandler;
        private readonly IOrderNotificationCommandHandler _orderNotificationCommandHandler;
        private readonly int _maxParallelThreadCount;
        #endregion Properties

        #region Constructor

        public GoogleMapsAdapter(
            IMerchantFeedCommandHandler merchantFeedCommandHandler,
            IServiceFeedCommandHandler serviceFeedCommandHandler,
            IAvailabilityFeedCommandHandler availabilityFeedCommandHandler,
            IInventoryRealTimeUpdateCommandHandler inventoryRealTimeUpdateCommandHandler,
            IOrderNotificationCommandHandler orderNotificationCommandHandler)
        {
            _merchantFeedCommandHandler = merchantFeedCommandHandler;
            _serviceFeedCommandHandler = serviceFeedCommandHandler;
            _availabilityFeedCommandHandler = availabilityFeedCommandHandler;
            _inventoryRealTimeUpdateCommandHandler = inventoryRealTimeUpdateCommandHandler;
            _orderNotificationCommandHandler = orderNotificationCommandHandler;
            _maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
        }

        #endregion Constructor

        #region Public Methods

        public bool UploadMerchantFeed(List<MerchantFeed> merchantFeeds)
        {
            return _merchantFeedCommandHandler.UploadFeed(merchantFeeds, MethodType.MerchantFeed);
        }

        public bool UploadServiceFeed(ServiceAvailabilityDto serviceAvailabilityDto)
        {
            return _serviceFeedCommandHandler.UploadFeed(serviceAvailabilityDto, MethodType.ServiceFeed);
        }

        public bool UploadAvailabilityFeed(ServiceAvailabilityDto serviceAvailabilityDto)
        {
            return _availabilityFeedCommandHandler.UploadFeed(serviceAvailabilityDto, MethodType.AvailabilityFeed);
        }

        public bool ProcessInventoryRealTimeUpdate(List<MerchantActivitiesDto> merchantActivitiesDtos)
        {

            Parallel.ForEach(merchantActivitiesDtos, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount },
                (merchantActivitiesDto) =>
                {
                    if (merchantActivitiesDto.Activities.Count > 0)
                    {
                        _inventoryRealTimeUpdateCommandHandler.Execute(merchantActivitiesDto, MethodType.InventoryRTU);
                    }
                });

            return true;
        }

        public List<Order> ProcessOrderNotification(List<Order> orders)
        {
            var orderResponses = new List<Order>();
            Parallel.ForEach(orders, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, order =>
            {
                var response = _orderNotificationCommandHandler.Execute(order, MethodType.OrderNotificationRTU);
                if(response == null) return;
                var orderResponse = SerializeDeSerializeHelper.DeSerialize<Order>(response.ToString());
                orderResponses.Add(orderResponse);
            });
            return orderResponses;
        }

        #endregion Public Methods
    }
}