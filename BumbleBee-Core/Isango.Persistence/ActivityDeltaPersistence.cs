using Isango.Entities;
using Isango.Entities.Master;
using Isango.Entities.Review;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class ActivityDeltaPersistence : PersistenceBase, IActivityDeltaPersistence
    {
        private readonly ILogger _log;
        public ActivityDeltaPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Method to retrieve Delta Activity review
        /// </summary>
        /// <returns></returns>
        public List<Review> GetDeltaActivityReview()
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaReview))
                {
                    dbCmd.CommandType = CommandType.StoredProcedure;

                    dbCmd.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var reviewList = new List<Review>();
                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            reviewList.Add(activityData.GetReviewMapping(reader));
                        }
                        return reviewList;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityDeltaPersistence",
                    MethodName = "GetDeltaActivityReview",
                 };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method to retrieve Delta Activity PassengerInfo
        /// </summary>
        /// <returns></returns>
        public List<Entities.Booking.PassengerInfo> GetDeltaActivityPassengerInfo()
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaPassengerInfo))
                {
                    dbCmd.CommandType = CommandType.StoredProcedure;
                    dbCmd.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var passengerInfoList = new List<Entities.Booking.PassengerInfo>();
                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            passengerInfoList.Add(activityData.GetPassengerMapping(reader));
                        }
                        return passengerInfoList;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityDeltaPersistence",
                    MethodName = "GetDeltaActivityPassengerInfo",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method to retrieve Delta Activity Ids
        /// </summary>
        /// <returns></returns>
        public List<ActivityIds> GetDeltaActivity()
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaActivity))
                {
                    dbCmd.CommandType = CommandType.StoredProcedure;
                    dbCmd.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var activityList = new List<ActivityIds>();
                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            activityList.Add(activityData.GetActivityIdsMapping(reader));
                        }
                        return activityList;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityDeltaPersistence",
                    MethodName = "GetDeltaActivity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method to retrieve Delta Activity Price
        /// </summary>
        /// <returns></returns>
        public List<ActivityMinPrice> GetDeltaActivityPrice()
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaActivityMinPrice))
                {
                    dbCmd.CommandType = CommandType.StoredProcedure;
                    dbCmd.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var activityPriceList = new List<ActivityMinPrice>();
                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            activityPriceList.Add(activityData.GetDeltaActivityPriceMapping(reader));
                        }
                        return activityPriceList;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityDeltaPersistence",
                    MethodName = "GetDeltaActivityPrice",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method to retrieve Delta Activity Availability
        /// </summary>
        /// <returns></returns>
        public List<ActivityAvailableDays> GetDeltaActivityAvailability()
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetDeltaActivityAvailability))
                {
                    dbCmd.CommandType = CommandType.StoredProcedure;
                    dbCmd.CommandTimeout = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("DeltaSQLTimeOut"));
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var activityAvailableDaysList = new List<ActivityAvailableDays>();
                        var activityData = new ActivityData();
                        while (reader.Read())
                        {
                            activityAvailableDaysList.Add(activityData.GetDeltaActivityAvailableMapping(reader));
                        }
                        return activityAvailableDaysList;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityDeltaPersistence",
                    MethodName = "GetDeltaActivityAvailability",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}