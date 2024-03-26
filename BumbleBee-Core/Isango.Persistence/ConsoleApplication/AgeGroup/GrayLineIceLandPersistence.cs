using Isango.Entities;
using Isango.Entities.ConsoleApplication.AgeGroup.GrayLineIceLand;
using Isango.Persistence.Contract;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class GrayLineIceLandPersistence : PersistenceBase, IGrayLineIceLandPersistence
    {
        private readonly ILogger _log;
        public GrayLineIceLandPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Save all GrayLineIceLand Age Groups
        /// </summary>
        /// <param name="masterAgeGroups"></param>
        public void SaveAllAgeGroups(List<Entities.ConsoleApplication.AgeGroup.GrayLineIceLand.AgeGroup> masterAgeGroups)
        {
            try
            {
                if (masterAgeGroups?.Count > 0)
                {
                    foreach (var ageGroup in masterAgeGroups)
                    {
                        //Prepare Command
                        try
                        {
                            using (var command = APIUploadDb.GetStoredProcCommand(Constant.InsertUpdateAgeGroupsSp))
                            {
                                // Prepare parameter collection
                                APIUploadDb.AddInParameter(command, Constant.GLIAgeGroupId, DbType.Int32, ageGroup.AgeGroupId);
                                APIUploadDb.AddInParameter(command, Constant.Description, DbType.String, ageGroup.Description);
                                APIUploadDb.AddInParameter(command, Constant.Ticked, DbType.Boolean, ageGroup.Ticked);
                                APIUploadDb.AddInParameter(command, Constant.FromAge, DbType.Int32, ageGroup.FromAge);
                                APIUploadDb.AddInParameter(command, Constant.GLIToAge, DbType.Int32, ageGroup.ToAge);

                                APIUploadDb.ExecuteNonQuery(command);
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GrayLineIceLandPersistence",
                    MethodName = "SaveAllAgeGroups",
                    Params = $"{SerializeDeSerializeHelper.Serialize(masterAgeGroups)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save all Activity Age Groups mapping
        /// </summary>
        /// <param name="activityAgeGroups"></param>
        public void SaveAllActivityAgeGroupsMapping(List<ActivityAgeGroup> activityAgeGroups)
        {
            try
            {
                if (activityAgeGroups?.Count > 0)
                {
                    var groupedActivityAgeGroups = activityAgeGroups.GroupBy(x => x.ActivityId);
                    foreach (var groupedActivityAgeGroup in groupedActivityAgeGroups)
                    {
                        try
                        {
                            //Delete All Existing Activity Age Groups
                            DeleteExistingAgeGroupIds(groupedActivityAgeGroup.Key);

                            foreach (var activityAgeGroup in groupedActivityAgeGroup)
                            {
                                //Prepare Command
                                using (var ageGroupCommand = APIUploadDb.GetStoredProcCommand(Constant.InsertUpdateActivityAgeGroupSp))
                                {
                                    // Prepare parameter collection
                                    APIUploadDb.AddInParameter(ageGroupCommand, Constant.GLIAgeGroupId, DbType.Int32, activityAgeGroup.AgeGroupId);
                                    APIUploadDb.AddInParameter(ageGroupCommand, Constant.GLIActivityId, DbType.Int32, activityAgeGroup.ActivityId);

                                    APIUploadDb.ExecuteNonQuery(ageGroupCommand);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GrayLineIceLandPersistence",
                    MethodName = "SaveAllActivityAgeGroupsMapping",
                    Params = $"{SerializeDeSerializeHelper.Serialize(activityAgeGroups)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save all pickup locations
        /// </summary>
        /// <param name="masterPickupLocations"></param>
        public void SaveAllPickupLocations(List<Pickuplocation> masterPickupLocations)
        {
            try
            {
                if (masterPickupLocations?.Count > 0)
                {
                    foreach (var pickupLocation in masterPickupLocations)
                    {
                        //Prepare Command
                        try
                        {
                            using (var command = APIUploadDb.GetStoredProcCommand(Constant.InsertUpdatePickupLocationSp))
                            {
                                // Prepare parameter collection
                                APIUploadDb.AddInParameter(command, Constant.Id, DbType.Int32, pickupLocation.Id);
                                APIUploadDb.AddInParameter(command, Constant.Description, DbType.String, pickupLocation.Description);
                                APIUploadDb.AddInParameter(command, Constant.Ticked, DbType.Boolean, pickupLocation.Ticked);
                                APIUploadDb.AddInParameter(command, Constant.OptionName, DbType.String, pickupLocation.Name);
                                APIUploadDb.AddInParameter(command, Constant.Address, DbType.String, pickupLocation.Address);
                                APIUploadDb.AddInParameter(command, Constant.Lat, DbType.Double, pickupLocation.Lat);
                                APIUploadDb.AddInParameter(command, Constant.Long, DbType.Double, pickupLocation.Long);
                                APIUploadDb.AddInParameter(command, Constant.ZipCode, DbType.String, pickupLocation.ZipCode);
                                APIUploadDb.AddInParameter(command, Constant.City, DbType.String, pickupLocation.City);
                                APIUploadDb.AddInParameter(command, Constant.Price, DbType.Decimal, pickupLocation.Price);
                                APIUploadDb.AddInParameter(command, Constant.ProductId, DbType.Int32, pickupLocation.ProductId);
                                APIUploadDb.AddInParameter(command, Constant.IsCheckinLocation, DbType.Boolean,
                                    pickupLocation.IsCheckinLocation);

                                APIUploadDb.ExecuteNonQuery(command);
                            }
                        }
                        catch (Exception)
                        {
                            // throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GrayLineIceLandPersistence",
                    MethodName = "SaveAllPickupLocations",
                    Params = $"{SerializeDeSerializeHelper.Serialize(masterPickupLocations)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
                    
        }

        /// <summary>
        /// Save all pickup location mappings
        /// </summary>
        /// <param name="activityPickupLocationsList"></param>
        public void SaveAllPickupLocationsMapping(List<ActivityPickupLocation> activityPickupLocationsList)
        {
            try
            {
                if (activityPickupLocationsList?.Count > 0)
                {
                    var groupedActivityPickupLocations = activityPickupLocationsList.GroupBy(x => x.ActivityId);
                    foreach (var groupedActivityPickupLocation in groupedActivityPickupLocations)
                    {
                        try
                        {
                            //Delete All Existing Activity Age Groups
                            DeleteExistingPickupLocationIds(groupedActivityPickupLocation.Key);

                            foreach (var activityPickupLocation in groupedActivityPickupLocation)
                            {
                                //Prepare Command
                                using (var ageGroupCommand =
                                    APIUploadDb.GetStoredProcCommand(Constant.InsertUpdateActivityPickupLocationSp))
                                {
                                    // Prepare parameter collection
                                    APIUploadDb.AddInParameter(ageGroupCommand, Constant.PickupLocationId, DbType.Int32,
                                        activityPickupLocation.PickupLocationId);
                                    APIUploadDb.AddInParameter(ageGroupCommand, Constant.GLIActivityId, DbType.Int32,
                                        activityPickupLocation.ActivityId);
                                    if (activityPickupLocation.PickupTime?.Equals(DateTime.MinValue) == false)
                                    {
                                        APIUploadDb.AddInParameter(ageGroupCommand, Constant.PickupTime, DbType.DateTime,
                                            activityPickupLocation.PickupTime.Value);
                                    }
                                    else
                                    {
                                        APIUploadDb.AddInParameter(ageGroupCommand, Constant.PickupTime, DbType.DateTime, null);
                                    }

                                    APIUploadDb.AddInParameter(ageGroupCommand, Constant.TimePUMinutes, DbType.Int32,
                                        activityPickupLocation.TimePUMinutes);

                                    APIUploadDb.ExecuteNonQuery(ageGroupCommand);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GrayLineIceLandPersistence",
                    MethodName = "SaveAllPickupLocationsMapping",
                    Params = $"{SerializeDeSerializeHelper.Serialize(activityPickupLocationsList)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Data synchronization between Isango databases
        /// </summary>
        public void SyncDataBetweenDataBases()
        {
            try
            {
                using (var ageGroupCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.SyncGLIMasterDataSp))
                {
                    IsangoDataBaseLive.ExecuteNonQuery(ageGroupCommand);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GrayLineIceLandPersistence",
                    MethodName = "SyncDataBetweenDataBases",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #region Private Methods

        /// <summary>
        /// Delete existing age group ids by activity id
        /// </summary>
        /// <param name="activityId"></param>
        private void DeleteExistingAgeGroupIds(int activityId)
        {
            using (var ageGroupCommand = APIUploadDb.GetStoredProcCommand(Constant.DeleteActivityAgeGroupByActivityIdSp))
            {
                // Prepare parameter collection
                APIUploadDb.AddInParameter(ageGroupCommand, Constant.GLIActivityId, DbType.Int32, activityId);
                APIUploadDb.ExecuteNonQuery(ageGroupCommand);
            }
        }

        /// <summary>
        /// Delete existing pickup location ids by activity id
        /// </summary>
        /// <param name="activityId"></param>
        private void DeleteExistingPickupLocationIds(int activityId)
        {
            using (var ageGroupCommand =
                APIUploadDb.GetStoredProcCommand(Constant.DeleteActivityPickupLocationsByActivityIdSp))
            {
                // Prepare parameter collection
                APIUploadDb.AddInParameter(ageGroupCommand, Constant.GLIActivityId, DbType.Int32, activityId);
                APIUploadDb.ExecuteNonQuery(ageGroupCommand);
            }
        }

        #endregion Private Methods
    }
}