using Isango.Entities;
using Isango.Entities.Master;
using Isango.Entities.Review;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;

namespace Isango.Service
{
    public class ActivityDeltaService : IActivityDeltaService
    {
        private readonly IActivityDeltaPersistence _activityDeltaPersistence;
        private readonly ILogger _log;

        public ActivityDeltaService(IActivityDeltaPersistence activityDeltaPersistence, ILogger log)
        {
            _activityDeltaPersistence = activityDeltaPersistence;
            _log = log;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        /// <summary>
        /// Method will fetch the delta activity review
        /// </summary>
        /// <returns></returns>
        public async Task<List<Review>> GetDeltaReviewAsync()
        {
            try
            {
                var result = _activityDeltaPersistence.GetDeltaActivityReview();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetDeltaReviewAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        /// <summary>
        /// Method will fetch the delta activity passenger Info
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entities.Booking.PassengerInfo>> GetDeltaPassengerInfoAsync()
        {
            try
            {
                var result = _activityDeltaPersistence.GetDeltaActivityPassengerInfo();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetDeltaPassengerInfoAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        /// <summary>
        /// Method will fetch the delta activity
        /// </summary>
        /// <returns></returns>
        public async Task<List<ActivityIds>> GetDeltaActivityAsync()
        {
            try
            {
                var result = _activityDeltaPersistence.GetDeltaActivity();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetDeltaActivityAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method will fetch the delta activity price
        /// </summary>
        /// <returns></returns>
        public async Task<List<ActivityMinPrice>> GetDeltaActivityPriceAsync()
        {
            try
            {
                var result = _activityDeltaPersistence.GetDeltaActivityPrice();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetDeltaActivityPriceAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method will fetch the delta activity price
        /// </summary>
        /// <returns></returns>
        public async Task<List<ActivityAvailableDays>> GetDeltaActivityAvailableAsync()
        {
            try
            {
                var result = _activityDeltaPersistence.GetDeltaActivityAvailability();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetDeltaActivityAvailableAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}
