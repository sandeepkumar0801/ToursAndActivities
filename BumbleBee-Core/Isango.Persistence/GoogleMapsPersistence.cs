using Isango.Entities;
using Isango.Entities.GoogleMaps;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class GoogleMapsPersistence : PersistenceBase, IGoogleMapsPersistence
    {
        private readonly ILogger _log;
        public GoogleMapsPersistence(ILogger log)
        {
            _log = log;
        }
        public List<MerchantFeed> GetMerchantData()
        {
            var merchantFeeds = new List<MerchantFeed>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetMerchantFeed))
                {
                    //This proc require more to execute. It was throwing timeoout error. to resolve the issue, CommandTimeout is set to 0
                    command.CommandTimeout = 0;
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var activityData = new GoogleMapsData();
                        while (reader.Read())
                        {
                            merchantFeeds.Add(activityData.GetMerchantData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GoogleMapsPersistence",
                    MethodName = "GetMerchantData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return merchantFeeds;
        }

        public List<AssignedServiceMerchant> GetAssignedServiceMerchant()
        {
            var assignedServiceMerchants = new List<AssignedServiceMerchant>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAssignedServiceMerchant))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var activityData = new GoogleMapsData();
                        while (reader.Read())
                        {
                            assignedServiceMerchants.Add(activityData.GetAssignedServiceMerchant(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GoogleMapsPersistence",
                    MethodName = "GetAssignedServiceMerchant",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return assignedServiceMerchants;
        }

        public List<PassengerType> GetPassengerTypes()
        {
            var passengerTypes = new List<PassengerType>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAllPassengerType))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var activityData = new GoogleMapsData();
                        while (reader.Read())
                        {
                            passengerTypes.Add(activityData.GetPassengerType(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GoogleMapsPersistence",
                    MethodName = "GetPassengerTypes",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return passengerTypes;
        }
    }
}