using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.ConsoleApplication.DataDumping;
using Isango.Entities.GoogleMaps;
using Isango.Entities.GoogleMaps.BookingServer;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.GoogleMaps;
using ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO;
using Util;
using CancellationPolicy = Isango.Entities.ConsoleApplication.DataDumping.CancellationPolicy;
using ExtraDetail = Isango.Entities.ConsoleApplication.DataDumping.ExtraDetail;

namespace Isango.Service
{
    public class GoogleMapsService : IGoogleMapsService
    {
        #region prop

        private readonly IGoogleMapsPersistence _googleMapsPersistence;
        private readonly IGoogleMapsAdapter _googleMapsAdapter;
        private readonly IActivityCacheManager _activityCacheManager;
        private readonly IStorageOperationService _storageOperationService;
        private readonly ILogger _logger;

        #endregion prop

        #region ctr

        public GoogleMapsService(IGoogleMapsPersistence googleMapsPersistence,
          IActivityCacheManager activityCacheManager, IStorageOperationService storageOperationService, IGoogleMapsAdapter googleMapsAdapter, ILogger logger)
        {
            _googleMapsPersistence = googleMapsPersistence;
            _activityCacheManager = activityCacheManager;
            _storageOperationService = storageOperationService;
            _googleMapsAdapter = googleMapsAdapter;
            _logger = logger;
        }

        #endregion ctr

        #region Public Method

        public bool LoadMerchantFeeds()
        {
            var affiliateId = ConfigurationManagerHelper.GetValuefromAppSettings("GoogleMapsAffiliatedId");
            try
            {
                var result = _googleMapsPersistence.GetMerchantData();
                if (result.Count > 0)
                {
                    return _googleMapsAdapter.UploadMerchantFeed(result);
                }
                return false;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GoogleMapsService",
                    MethodName = "LoadMerchantFeeds",
                    AffiliateId = affiliateId,
                };
                _logger.Error(isangoErrorEntity, ex);
                return false;
            }
        }

        public bool LoadServiceAvailabilityFeeds()
        {
            var affiliateId = ConfigurationManagerHelper.GetValuefromAppSettings("GoogleMapsAffiliatedId");
            var merchantActivitiesDtos = default(List<MerchantActivitiesDto>);
            var passengerTypes = default(List<PassengerType>);
            var serviceAvailabilityDto = default(ServiceAvailabilityDto);
            try
            {
                merchantActivitiesDtos = GetMerchantActivities();
                if (merchantActivitiesDtos.Count > 0)
                {
                    passengerTypes = _googleMapsPersistence.GetPassengerTypes();
                    serviceAvailabilityDto = new ServiceAvailabilityDto
                    {
                        MerchantActivitiesDtos = merchantActivitiesDtos,
                        PassengerTypes = passengerTypes
                    };
                    _googleMapsAdapter.UploadServiceFeed(serviceAvailabilityDto);
                    _googleMapsAdapter.UploadAvailabilityFeed(serviceAvailabilityDto);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GoogleMapsService",
                    MethodName = "LoadServiceAvailabilityFeeds",
                    AffiliateId = affiliateId,
                };
                _logger.Error(isangoErrorEntity, ex);
                return false;
            }
        }

        public bool InventoryRealTimeUpdate()
        {
            var affiliateId = ConfigurationManagerHelper.GetValuefromAppSettings("GoogleMapsAffiliatedId");
            try
            {
                var merchantActivitiesDtos = GetMerchantActivitiesForRealTimeUpdate();
                if (merchantActivitiesDtos.Count > 0)
                {
                    return _googleMapsAdapter.ProcessInventoryRealTimeUpdate(merchantActivitiesDtos);
                }
                return false;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GoogleMapsService",
                    MethodName = "InventoryRealTimeUpdate",
                    AffiliateId = affiliateId,
                };
                _logger.Error(isangoErrorEntity, ex);
                return false;
            }
        }

        public bool OrderNotificationRealTimeUpdate()
        {
            try
            {
                var orderResponses = _storageOperationService.GetCancelledOrders();
                if (orderResponses.Count == 0) return false;

                var orders = new List<Order>();
                foreach (var orderResponse in orderResponses)
                {
                    var order = SerializeDeSerializeHelper.DeSerialize<Order>(orderResponse.Order);
                    orders.Add(order);
                }
                var response = _googleMapsAdapter.ProcessOrderNotification(orders);
                if (response.Count == 0) return false;

                _storageOperationService.UpdateNotifiedOrders(orderResponses);
                return true;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GoogleMapsService",
                    MethodName = "OrderNotificationRealTimeUpdate"
                };
                _logger.Error(isangoErrorEntity, ex);
                return false;
            }
        }

        #endregion Public Method

        #region Private Method

        private List<MerchantActivitiesDto> GetMerchantActivities()
        {
            var assignedServiceMerchants = _googleMapsPersistence.GetAssignedServiceMerchant();
            var partitionKey = DateTime.UtcNow.ToString("dd_MMM_yyyy");
            var serviceDetails = _storageOperationService.GetServiceDetails(partitionKey);
            var activityCollectionsEnglish = _activityCacheManager.GetAllActivities();
            if (serviceDetails.Count == 0 || assignedServiceMerchants.Count == 0 || activityCollectionsEnglish.Count == 0)
            {
                return new List<MerchantActivitiesDto>();
            }
            var activityCollectionsSpanish = _activityCacheManager.GetAllActivities("es");
            var activityCollectionsGerman = _activityCacheManager.GetAllActivities("de");
            var extraDetails = _storageOperationService.GetExtraDetails(partitionKey);
            var cancellationDetails = _storageOperationService.GetCancellationPolicies(partitionKey);
            var distinctMerchants = assignedServiceMerchants.Select(s => s.MerchantId).Distinct().ToList();
            var merchantActivitiesDtos = new List<MerchantActivitiesDto>();
            var logData =
                $"GetAssignedServiceMerchant_{assignedServiceMerchants.Count}_storageServiceDetails_{serviceDetails.Count}_extraDetails_{extraDetails.Count}_distinctMerchants_{distinctMerchants.Count}_acitvityCollectionsEnglish{activityCollectionsEnglish.Count}_acitvityCollectionsSpanish{activityCollectionsSpanish.Count}_acitvityCollectionsGerman{activityCollectionsGerman.Count}";
            _logger.Info($"GoogleMapsService|LoadServiceFeeds|{logData}");
            foreach (var merchant in distinctMerchants)
            {
                try
                {
                    var activitiesDto = new MerchantActivitiesDto { MerchantId = merchant };
                    var activityIds = assignedServiceMerchants.Where(w => w.MerchantId == merchant).Select(s => s.ActivityId)
                        .Distinct();
                    var activityDto = new List<ActivityDto>();

                    foreach (var activityId in activityIds)
                    {
                        //Get activity for en lang
                        try
                        {
                            var activity = activityCollectionsEnglish.FirstOrDefault(w => w.Id == activityId.ToString());
                            if (activity != null)
                            {
                                var acitvityCollections = new List<AcitvityCollection>
                        {
                            new AcitvityCollection()
                            {
                                LanguageCode = "en",
                                Activity = activity
                            }
                        };

                                //Get activity for es(Spanish) lang
                                activity = activityCollectionsSpanish.FirstOrDefault(w => w.Id == activityId.ToString());
                                if (activity != null)
                                {
                                    acitvityCollections.Add(new AcitvityCollection()
                                    {
                                        LanguageCode = "es",
                                        Activity = activity
                                    });
                                }

                                //Get activity for de(German) lang
                                activity = activityCollectionsGerman.FirstOrDefault(w => w.Id == activityId.ToString());
                                if (activity != null)
                                {
                                    acitvityCollections.Add(new AcitvityCollection()
                                    {
                                        LanguageCode = "de",
                                        Activity = activity
                                    });
                                }

                                var activityServiceDetails = serviceDetails.Where(w => w.ActivityId == activityId).ToList();
                                var activityExtraDetails = extraDetails.Where(w => w.ActivityId == activityId).ToList();
                                var activityCancellationPolicies = cancellationDetails.Where(w => w.ActivityId == activityId).ToList();

                                activityDto.Add(new ActivityDto()
                                {
                                    ActivityId = activityId,
                                    AcitvityCollections = acitvityCollections,
                                    ServiceDetails = activityServiceDetails,
                                    ExtraDetails = activityExtraDetails,
                                    CancellationPolicies = activityCancellationPolicies
                                });
                            }
                        }
                        catch (Exception)
                        {

                            /*throw*/
                            ;
                        }
                    }
                    activitiesDto.Activities = activityDto;
                    merchantActivitiesDtos.Add(activitiesDto);

                }
                catch (Exception)
                {

                    //throw;
                }
            }
            return merchantActivitiesDtos;
        }

        private List<MerchantActivitiesDto> GetMerchantActivitiesForRealTimeUpdate()
        {
            var assignedServiceMerchants = _googleMapsPersistence.GetAssignedServiceMerchant();
            var partitionKey = DateTime.UtcNow.ToString("dd_MMM_yyyy");
            var serviceDetails = _storageOperationService.GetServiceDetails(partitionKey);
            if (serviceDetails.Count == 0 || assignedServiceMerchants.Count == 0)
            {
                return new List<MerchantActivitiesDto>();
            }
            var distinctMerchants = assignedServiceMerchants.Select(s => s.MerchantId).Distinct().ToList();
            var merchantActivitiesDtos = new List<MerchantActivitiesDto>();
            foreach (var merchant in distinctMerchants)
            {
                var activitiesDto = new MerchantActivitiesDto { MerchantId = merchant };
                var activityIds = assignedServiceMerchants.Where(w => w.MerchantId == merchant).Select(s => s.ActivityId)
                    .Distinct();
                var activityDto = new List<ActivityDto>();

                foreach (var activityId in activityIds)
                {
                    //Get activity for en lang
                    var isActivityChanged = serviceDetails.Any(w => w.ActivityId == activityId && w.DumpingStatus != DumpingStatus.Unchanged.ToString());
                    if (isActivityChanged)
                    {
                        var activityServiceDetails = serviceDetails.Where(w => w.ActivityId == activityId).ToList();
                        activityDto.Add(new ActivityDto()
                        {
                            ActivityId = activityId,
                            AcitvityCollections = new List<AcitvityCollection>(),
                            ServiceDetails = activityServiceDetails,
                            ExtraDetails = new List<ExtraDetail>(),
                            CancellationPolicies = new List<CancellationPolicy>()
                        });
                    }
                }
                activitiesDto.Activities = activityDto;
                merchantActivitiesDtos.Add(activitiesDto);
            }
            return merchantActivitiesDtos;
        }

        #endregion Private Method
    }
}