//using Isango.Entities.Activities;
//using Isango.Service.Contract;
//using Logger.Contract;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Util;
//using WebAPI.Filters;
//using WebAPI.Mapper;
//using WebAPI.Models.ResponseModels.DeltaActivity;

//namespace WebAPI.Controllers
//{
//    /// <summary>
//    /// Provide details of all delta service related endpoints
//    /// </summary>

//    [Route("api/delta")]
//    [ApiController]

//    public class DeltaActivityController : ApiBaseController
//    {
//        /* No longer used

         
//        private readonly ActivityMapper _activityMapper;
//        private readonly IActivityService _activityService;
//        private readonly ILogger _logger;
//        private readonly IActivityDeltaService _activityDeltaService;

//        /// <summary>
//        /// Parameterized Constructor to initialize all dependencies.
//        /// </summary>
//        /// <param name="activityDeltaService"></param>
//        /// <param name="activityMapper"></param>
//        /// <param name="logger"></param>
//        /// <param name="activityService"></param>
//        public DeltaActivityController(IActivityDeltaService activityDeltaService, ActivityMapper activityMapper, ILogger logger, IActivityService activityService)
//        {
//            _activityMapper = activityMapper;
//            _activityDeltaService = activityDeltaService;
//            _activityService = activityService;
//            _logger = logger;
//        }

//        /// <summary>
//        /// This operation gives the activity delta activity review
//        /// </summary>
//        [SwaggerOperation(Tags = new[] { "Delta Activity" })]
//        [Route("review")]
//        [HttpGet]
//        [ValidateModel]
//        [SwitchableAuthorization]
//        public IHttpActionResult GetReviewData()
//        {
//            var reviewData = _activityDeltaService.GetDeltaReviewAsync().Result;
//            var reviewResponse = _activityMapper.MapReviewData(reviewData);
//            return GetResponseWithHttpActionResult(reviewResponse);
//        }

//        /// <summary>
//        /// This operation gives the activity delta passenger info
//        /// </summary>
//        [SwaggerOperation(Tags = new[] { "Delta Activity" })]
//        [Route("passengerinfo")]
//        [HttpGet]
//        [ValidateModel]
//        [SwitchableAuthorization]
//        public IHttpActionResult GetPassengerInfoData()
//        {
//            var passengerInfoData = _activityDeltaService.GetDeltaPassengerInfoAsync().Result;
//            var passengerInfoResponse = _activityMapper.MapPassengerInfoData(passengerInfoData);
//            return GetResponseWithHttpActionResult(passengerInfoResponse);
//        }

//        /// <summary>
//        /// This operation gives the activity data
//        /// </summary>
//        [SwaggerOperation(Tags = new[] { "Delta Activity" })]
//        [Route("activity")]
//        [HttpGet]
//        [ValidateModel]
//        [SwitchableAuthorization]
//        public IHttpActionResult GetActivityData()
//        {
//            var activityIds = _activityDeltaService.GetDeltaActivityAsync().Result;
//            var resultOnlineActivity = from activity in activityIds.ToList()
//                                       where activity.ServiceStatusID == true
//                                       select activity;

//            var resultOfflineActivity = from activity in activityIds.ToList()
//                                        where !activity.ServiceStatusID
//                                        select activity;

//            var activityResponseList = new List<ActivityResponse>();

//            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
//            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount };

//            //resultOnlineActivity = resultOnlineActivity.OrderBy(x => x.Serviceid).Take(33);

//            if (resultOnlineActivity != null)
//            {
//                var currentDate = DateTime.Now.Date;
//                var totalRecords = resultOnlineActivity.Count();
//                var activityTasks = new List<Task<Activity>>();

//                Parallel.ForEach(resultOnlineActivity, parallelOptions, item =>
//                {
//                    try
//                    {
//                        activityTasks.Add(
//                            Task.Run(()=> _activityService.GetActivityById(item.Serviceid, currentDate, item.LanguageCode))
//                            );
//                    }
//                    catch (Exception ex)
//                    {
//                        var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
//                        {
//                            ClassName = "DeltaActivity",
//                            MethodName = "GetActivityData",
//                            Params = SerializeDeSerializeHelper.Serialize(item),
//                            Token = $"DeltaActivity_{currentDate.ToString(Constants.Constant.dd_MMM_yyyy)}",
//                        };
//                        _logger.Error(isangoErrorEntity, ex);
//                    }
//                });
//                Task.WaitAll(activityTasks.ToArray());

//                Parallel.ForEach(activityTasks, parallelOptions, activityTask =>
//                {
//                    var activity = default(Activity);
//                    try
//                    {
//                        activity = activityTask?.GetAwaiter().GetResult();
//                        if (activity != null)
//                        {
//                            var activityResponse = _activityMapper.MapActivityData(activity, activity.LanguageCode.ToUpperInvariant(), true);
//                            activityResponseList.Add(activityResponse);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
//                        {
//                            ClassName = "DeltaActivity",
//                            MethodName = "GetActivityData",
//                            Params = SerializeDeSerializeHelper.Serialize(activity),
//                            Token = $"DeltaActivity_{currentDate.ToString(Constants.Constant.dd_MMM_yyyy)}",
//                        };
//                        _logger.Error(isangoErrorEntity, ex);
//                    }
//                });
//            }

//            if (resultOfflineActivity != null)
//            {
//                //foreach (var item in resultOfflineActivity)
//                Parallel.ForEach(resultOfflineActivity, parallelOptions, item =>
//                {
//                    var languageCode = item.LanguageCode.ToUpperInvariant();
//                    if (activityResponseList.Count == 0)
//                    {
//                        activityResponseList = new List<ActivityResponse> { new ActivityResponse
//                         {
//                            ID = item.Serviceid, Status = false, LanguageCode = languageCode,
//                            OnSale=false,IsPaxDetailRequired=false,IsReceipt=false,
//                            IsServiceLevelPickUp=false,IsPackage=false,IsNoIndex=false,
//                            IsFollow=false,IsHighDefinationImages=false, IsGoogleFeed=false,
//                            IsTimeBase=false,TourLaunchDate=DateTime.MinValue,
//                            LiveOnDate =DateTime.MinValue,SupplierID=0
//                         }
//                        };
//                    }
//                    else
//                    {
//                        activityResponseList.AddRange(new List<ActivityResponse> { new ActivityResponse
//                        {
//                            ID = item.Serviceid, Status = false, LanguageCode = languageCode,
//                            OnSale=false,IsPaxDetailRequired=false,IsReceipt=false,
//                            IsServiceLevelPickUp=false,IsPackage=false,IsNoIndex=false,
//                            IsFollow=false,IsHighDefinationImages=false, IsGoogleFeed=false,
//                            IsTimeBase=false,TourLaunchDate=DateTime.MinValue,
//                            LiveOnDate =DateTime.MinValue,SupplierID=0
//                        }
//                        });
//                    }
//                });
//            }

//            return GetResponseWithHttpActionResult(activityResponseList);
//        }

//        /// <summary>
//        /// This operation gives the  delta activity price
//        /// </summary>
//        [SwaggerOperation(Tags = new[] { "Delta Activity" })]
//        [Route("price")]
//        [HttpGet]
//        [ValidateModel]
//        [SwitchableAuthorization]
//        public IHttpActionResult GetActivityPriceData()
//        {
//            var activityPriceData = _activityDeltaService.GetDeltaActivityPriceAsync()?.GetAwaiter().GetResult();
//            var activityPriceResponse = _activityMapper.MapActivityPriceData(activityPriceData);
//            return GetResponseWithHttpActionResult(activityPriceResponse);
//        }

//        /// <summary>
//        /// This operation gives the  delta activity availability
//        /// </summary>
//        [SwaggerOperation(Tags = new[] { "Delta Activity" })]
//        [Route("availability")]
//        [HttpGet]
//        [ValidateModel]
//        [SwitchableAuthorization]
//        public IHttpActionResult GetActivityAvailabilityData()
//        {
//            var activityAvailableData = _activityDeltaService.GetDeltaActivityAvailableAsync().Result;
//            var activityAvailableResponse = _activityMapper.MapActivityAvailableData(activityAvailableData);
//            return GetResponseWithHttpActionResult(activityAvailableResponse);
//        }
//    */
//    }
//}