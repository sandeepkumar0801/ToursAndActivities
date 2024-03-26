using Isango.Entities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using Isango.Entities.GoogleMaps;
using Isango.Persistence.Contract;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.ConsoleApplication.ServiceAvailability
{
    public class ServiceAvailabilityPersistence : PersistenceBase, IServiceAvailabilityPersistence
    {
        private readonly ILogger _log;
        public ServiceAvailabilityPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Delete existing details from HBServiceDetail table
        /// </summary>
        public void DeleteExistingHBServiceDetails()
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.DeleteHBServiceDetailSp))
                {
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityPersistence",
                    MethodName = "DeleteExistingHBServiceDetails",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Sync API Price and Availability data
        /// </summary>
        public void SyncAPIPriceAvailabilities()
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.SyncAPIPriceAvailabilitySp))
                {
                    command.CommandTimeout = 300;
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityPersistence",
                    MethodName = "SyncAPIPriceAvailabilities",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save all error logs
        /// </summary>
        /// <param name="error"></param>
        public void SaveErrorLogs(ErrorLogger error)
        {
            try
            {
                if (error != null)
                {
                    //Prepare Command
                    using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertErrorLoggerSp))
                    {
                        // Prepare parameter collection
                        IsangoDataBaseLive.AddInParameter(command, Constant.Message, DbType.String, error.Message);
                        IsangoDataBaseLive.AddInParameter(command, Constant.Destination, DbType.String, error.Destination);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ErrorCheckInDate, DbType.DateTime, error.CheckInDate);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ErrorCheckOutDate, DbType.DateTime, error.CheckOutDate);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ErrorFactSheetIds, DbType.String, error.FactSheetIds);

                        IsangoDataBaseLive.ExecuteNonQuery(command);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityPersistence",
                    MethodName = "SaveErrorLogs",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Service Availabilities in the database
        /// </summary>
        /// <param name="serviceDetails"></param>
        public void SaveServiceAvailabilities(List<TempHBServiceDetail> serviceDetails)
        {
            var listFailedIds = new Dictionary<int, string>();
            try
            {
                if (serviceDetails == null || serviceDetails.Count <= 0) return;

                var parallelOption = GetParallelOptions();

                var serviceDetailsIndependentBookable = serviceDetails
                                .Where(x =>
                                    x.PassengerTypeId == 1
                                    && (x.Price > 0 || x.SellPrice > 0))?
                                .ToList();



                var serviceIdsAdults = serviceDetailsIndependentBookable.Select(x => x.ActivityId).Distinct().ToList(); serviceIdsAdults.Sort();

                //Get details of passenger where passenger is not adult ,child ,infant,youth and for which adult price is not there
                var queryNonAdults = serviceDetails.Except(serviceDetailsIndependentBookable).ToList();

                queryNonAdults.RemoveAll(s => (serviceIdsAdults.Contains(s.ActivityId)
                                                    && (
                                                        s.PassengerTypeId == 1 ||
                                                        s.PassengerTypeId == 2 ||
                                                        s.PassengerTypeId == 8 ||
                                                        s.PassengerTypeId == 9
                                                     )
                                                )
                                            || (s.Price == 0 && s.SellPrice == 0)
                );

                Parallel.ForEach(queryNonAdults, parallelOption, serviceDetail =>
                {
                    serviceDetailsIndependentBookable.Add(serviceDetail);
                });

                var filteredServiceDetails = serviceDetailsIndependentBookable
                    .Where(z => z.Status.ToUpper() == AvailabilityStatus.AVAILABLE.ToString().ToUpper()
                    || z.Status.ToUpper() == AvailabilityStatus.ONREQUEST.ToString().ToUpper())
                    .OrderBy(x => x.ActivityId)
                    .ThenBy(y => y.ServiceOptionID).ThenBy(z => z.AvailableOn);

                //GroupBy groupByServiceId
                var groupByServiceId = filteredServiceDetails?.GroupBy(x => x.ActivityId);

                foreach (var groupItem in groupByServiceId)
                {
                    try
                    {
                        var groupItemList = groupItem?.ToList();
                        var count = groupItemList?.Count ?? 0;

                        for (int itemCriteria = 0; itemCriteria < count; itemCriteria = (itemCriteria + count))
                        {
                            try
                            {
                                if (groupItemList[itemCriteria] != null && groupItemList?.Count > 0)
                                {
                                    var criteriaRecordsAtTime = groupItemList?.Skip(itemCriteria)?.Take(count)?.ToList();

                                    //Set service details in the DataTable format
                                    var details = SetServiceDetails(criteriaRecordsAtTime);
                                    SaveServiceAvailabilitiesInDB(details);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                if (!listFailedIds.ContainsKey(groupItem.Key))
                                {
                                    listFailedIds.Add(groupItem.Key, ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (listFailedIds == null)
                        {
                            listFailedIds = new Dictionary<int, string>();
                        }
                        if (listFailedIds?.ContainsKey(0) == false)
                        {
                            listFailedIds.Add(0, ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                if (listFailedIds == null)
                {
                    listFailedIds = new Dictionary<int, string>();
                }
                if (listFailedIds?.ContainsKey(1) == false)
                {
                    listFailedIds.Add(1, ex.Message);
                }
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityPersistence",
                    MethodName = "SaveServiceAvailabilities",
                    Params = $"{SerializeDeSerializeHelper.Serialize(listFailedIds)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }



        /// <summary>
        /// Save Service Availabilities in the database
        /// </summary>
        /// <param name="serviceDetails"></param>
        public void SaveServiceAvailabilitiesForTiqets(List<TempHBServiceDetail> serviceDetails)
        {
            var listFailedIds = new Dictionary<int, string>();
            try
            {
                if (serviceDetails == null || serviceDetails.Count <= 0) return;

                //For Tiqets Only for all age Group

                var parallelOptionForTiqets = GetParallelOptions();

                var serviceDetailsTiqetsIndependentBookable = serviceDetails
                        .Where(x =>
                         (x.Price > 0 || x.SellPrice > 0))?.ToList();

                var serviceIdsTiqetsAdults = serviceDetailsTiqetsIndependentBookable.Select(x => x.ActivityId).Distinct().ToList();

                var queryNonAdultsForTiqets = serviceDetails.Except(serviceDetailsTiqetsIndependentBookable).ToList();


                Parallel.ForEach(queryNonAdultsForTiqets, parallelOptionForTiqets, serviceDetailTiqets =>
                {
                    serviceDetailsTiqetsIndependentBookable.Add(serviceDetailTiqets);
                });


                var filteredServiceDetailsForTiqets = serviceDetailsTiqetsIndependentBookable
                    .Where(z => z.Status.ToUpper() == AvailabilityStatus.AVAILABLE.ToString().ToUpper()
                    || z.Status.ToUpper() == AvailabilityStatus.ONREQUEST.ToString().ToUpper())
                    .OrderBy(x => x.ActivityId)
                    .ThenBy(y => y.ServiceOptionID).ThenBy(z => z.AvailableOn);


                var groupByServiceIdTiqets = filteredServiceDetailsForTiqets?.GroupBy(x => x.ActivityId);

                foreach (var groupItem in groupByServiceIdTiqets)
                {
                    try
                    {
                        var groupItemList = groupItem?.ToList();
                        var count = groupItemList?.Count ?? 0;

                        for (int itemCriteria = 0; itemCriteria < count; itemCriteria = (itemCriteria + count))
                        {
                            try
                            {
                                if (groupItemList[itemCriteria] != null && groupItemList?.Count > 0)
                                {
                                    var criteriaRecordsAtTime = groupItemList?.Skip(itemCriteria)?.Take(count)?.ToList();

                                    //Set service details in the DataTable format
                                    var details = SetServiceDetails(criteriaRecordsAtTime);
                                    SaveServiceAvailabilitiesInDBForTiqets(details);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                if (!listFailedIds.ContainsKey(groupItem.Key))
                                {
                                    listFailedIds.Add(groupItem.Key, ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (listFailedIds == null)
                        {
                            listFailedIds = new Dictionary<int, string>();
                        }
                        if (listFailedIds?.ContainsKey(0) == false)
                        {
                            listFailedIds.Add(0, ex.Message);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                if (listFailedIds == null)
                {
                    listFailedIds = new Dictionary<int, string>();
                }
                if (listFailedIds?.ContainsKey(1) == false)
                {
                    listFailedIds.Add(1, ex.Message);
                }
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityPersistence",
                    MethodName = "SaveServiceAvailabilitiesForTiqets",
                    Params = $"{SerializeDeSerializeHelper.Serialize(listFailedIds)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// SaveServiceAvailabilitiesGTix
        /// </summary>
        /// <param name="serviceDetails"></param>
        public void SaveServiceAvailabilitiesGTix(List<TempHBServiceDetail> serviceDetails)
        {
            try
            {
                if (serviceDetails?.Count <= 0) return;
                //Set service details in the DataTable format
                var details = SetServiceDetails(serviceDetails);

                //GroupBy groupByServiceId
                var groupByServiceId = serviceDetails?.OrderBy(x => x.ActivityId).GroupBy(x => x.ActivityId);
                var listFailedIds = new Dictionary<int, string>();
                foreach (var groupItem in groupByServiceId)
                {
                    var groupItemList = groupItem?.ToList();
                    var count = groupItemList?.Count ?? 0;

                    for (int itemCriteria = 0; itemCriteria < count; itemCriteria = (itemCriteria + count))
                    {
                        try
                        {
                            if (groupItemList[itemCriteria] != null && groupItemList?.Count > 0)
                            {
                                var criteriaRecordsAtTime = groupItemList?.Skip(itemCriteria)?.Take(count)?.ToList();

                                //Set service details in the DataTable format
                                var detail = SetServiceDetails(criteriaRecordsAtTime);
                                SaveServiceAvailabilitiesInDB(detail);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (!listFailedIds.ContainsKey(groupItem.Key))
                            {
                                listFailedIds.Add(groupItem.Key, ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityPersistence",
                    MethodName = "SaveServiceAvailabilitiesGTix",
                    Params = $"{SerializeDeSerializeHelper.Serialize(serviceDetails)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveQuestionsInDB(List<ExtraDetailQuestions> Details, int ApiType)
        {
            try
            {
                var dataTable = new DataTable { TableName = Constant.QuestionsTableType };
                var dataTableQuestions = new DataTable { TableName = Constant.AssignedQuestionsTable };
                var dataTableAnswers = new DataTable { TableName = Constant.AnswerOptionsTableType };
                var dataTablePickUpLocations = new DataTable { TableName = Constant.APIPickUpLocationsTableType };
                var dataTableAssignedPickUpLocations = new DataTable { TableName = Constant.APIassignedPickUpLocationsTableType };
                var dataTableDropOfflocations = new DataTable { TableName = Constant.DropOffLocationsTableType };
                var dataTableAssignedDropOffLocations = new DataTable { TableName = Constant.APIAssignedDropofflocationtabletype };

                //Question table Fields
                //dataTable.Columns.Add("ActivityId", typeof(int));
                //dataTable.Columns.Add("OptionId", typeof(int));
                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("Label", typeof(string));
                dataTable.Columns.Add("Required", typeof(bool));
                dataTable.Columns.Add("SelectFromOptions", typeof(bool));
                dataTable.Columns.Add("DataType", typeof(string));
                dataTable.Columns.Add("DefaultValue", typeof(string));
                dataTable.Columns.Add("QuestionType", typeof(string));
                dataTable.Columns.Add("StartTime", typeof(System.TimeSpan));
                dataTable.Columns.Add("Variant", typeof(string));

                dataTableQuestions.Columns.Add("ActivityId", typeof(int));
                dataTableQuestions.Columns.Add("OptionId", typeof(int));
                dataTableQuestions.Columns.Add("Id", typeof(string));

                //Answer Table Fields
                dataTableAnswers.Columns.Add("OptionID", typeof(int));
                dataTableAnswers.Columns.Add("ActivityID", typeof(int));
                dataTableAnswers.Columns.Add("Id", typeof(string));
                dataTableAnswers.Columns.Add("Value", typeof(string));
                dataTableAnswers.Columns.Add("Label", typeof(string));

                ////PickUpLocation Table Fields
                dataTablePickUpLocations.Columns.Add("PickUpLocationID", typeof(int));
                dataTablePickUpLocations.Columns.Add("PickUpLocationName", typeof(string));
                dataTablePickUpLocations.Columns.Add("Variant", typeof(string));
                dataTablePickUpLocations.Columns.Add("StartTime", typeof(System.TimeSpan));

                //AssignedPickUpLocations Table Fields
                dataTableAssignedPickUpLocations.Columns.Add("ActivityId", typeof(int));
                dataTableAssignedPickUpLocations.Columns.Add("OptionId", typeof(int));
                dataTableAssignedPickUpLocations.Columns.Add("PickUpLocationID", typeof(int));

                ////DropOffLocation Table Fields
                dataTableDropOfflocations.Columns.Add("DropOffLocationID", typeof(int));
                dataTableDropOfflocations.Columns.Add("DropOffLocationName", typeof(string));
                dataTableDropOfflocations.Columns.Add("StartTime", typeof(System.TimeSpan));
                dataTableDropOfflocations.Columns.Add("Variant", typeof(string));

                //AssignedDropOffLocation Table Fields
                dataTableAssignedDropOffLocations.Columns.Add("ActivityId", typeof(int));
                dataTableAssignedDropOffLocations.Columns.Add("OptionId", typeof(int));
                dataTableAssignedDropOffLocations.Columns.Add("DropOffLocationID", typeof(int));

                var DistinctPickUpValues = new List<int>();
                var DistinctDropOffValues = new List<int>();
                var DistinctQuestionValues = new List<string>();

                if (Details?.Count > 0)
                {
                    foreach (var ques in Details)
                    {
                        try
                        {
                            //Questions
                            if (ques?.Questions?.Count > 0)
                            {
                                try
                                {
                                    foreach (var q in ques?.Questions)
                                    {
                                        var row = dataTableQuestions.NewRow();

                                        row["ActivityId"] = ques.ActivityId;
                                        row["OptionId"] = ques.OptionId;
                                        row["Id"] = q.Id;

                                        if (DistinctQuestionValues?.Contains(q.Id) != true)
                                        {
                                            DistinctQuestionValues.Add(q.Id);

                                            var row_distinct = dataTable.NewRow();

                                            row_distinct["Id"] = q.Id;
                                            row_distinct["Label"] = q.Label;
                                            row_distinct["Required"] = q.Required;
                                            row_distinct["SelectFromOptions"] = q.SelectFromOptions;
                                            row_distinct["DataType"] = q.DataType;
                                            row_distinct["DefaultValue"] = q.DefaultValue;
                                            row_distinct["QuestionType"] = q.QuestionType;

                                            row_distinct["StartTime"] = ques.StartTime;
                                            row_distinct["Variant"] = ques.Variant;

                                            dataTable.Rows.Add(row_distinct);
                                        }

                                        if (q?.AnswerOptions != null && q?.AnswerOptions.Count > 0)
                                        {
                                            foreach (var ans in q?.AnswerOptions)
                                            {
                                                var row_ans = dataTableAnswers.NewRow();
                                                row_ans["Value"] = ans.Value;
                                                row_ans["Label"] = ans.Label;
                                                row_ans["ActivityID"] = ques.ActivityId;
                                                row_ans["OptionID"] = ques.OptionId;
                                                row_ans["Id"] = q.Id;

                                                dataTableAnswers.Rows.Add(row_ans);
                                            }
                                        }

                                        dataTableQuestions.Rows.Add(row);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //continue;
                                }
                            }

                            //PickUpLocations
                            if (ques?.PickUpLocationID?.Count > 0)
                            {
                                int temp_count = 0;
                                try
                                {
                                    foreach (var ID in ques?.PickUpLocationID)
                                    {
                                        var row = dataTableAssignedPickUpLocations.NewRow();

                                        row["ActivityId"] = ques.ActivityId;
                                        row["OptionId"] = ques.OptionId;
                                        row["PickUpLocationID"] = ID;

                                        if (DistinctPickUpValues?.Contains(ID) != true)
                                        {
                                            DistinctPickUpValues.Add(ID);

                                            var row_distinct = dataTablePickUpLocations.NewRow();

                                            row_distinct["PickUpLocationID"] = ID;
                                            row_distinct["StartTime"] = ques?.StartTime;
                                            row_distinct["Variant"] = ques?.Variant;
                                            row_distinct["PickUpLocationName"] = ques?.PickUpLocationName[temp_count];

                                            dataTablePickUpLocations.Rows.Add(row_distinct);
                                        }

                                        temp_count = temp_count + 1;

                                        dataTableAssignedPickUpLocations.Rows.Add(row);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //continue;
                                }
                            }

                            //DropOffLocations
                            if (ques?.DropOffLocationName?.Count > 0)
                            {
                                int temp_count = 0;
                                try
                                {
                                    foreach (var ID in ques?.DropOffLocationID)
                                    {
                                        var row = dataTableAssignedDropOffLocations.NewRow();

                                        row["ActivityId"] = ques.ActivityId;
                                        row["OptionId"] = ques.OptionId;
                                        row["DropOffLocationID"] = ID;

                                        if (DistinctDropOffValues?.Contains(ID) != true)
                                        {
                                            DistinctDropOffValues.Add(ID);

                                            var row_distinct = dataTableDropOfflocations.NewRow();

                                            row_distinct["DropOffLocationID"] = ID;
                                            row_distinct["StartTime"] = ques?.StartTime;
                                            row_distinct["Variant"] = ques?.Variant;
                                            row_distinct["DropOffLocationName"] = ques?.DropOffLocationName[temp_count];

                                            dataTableDropOfflocations.Rows.Add(row_distinct);
                                        }

                                        temp_count = temp_count + 1;

                                        dataTableAssignedDropOffLocations.Rows.Add(row);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // continue;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw;
                        }
                    }

                    using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.API_Upload)))
                    {
                        // Configure the SqlCommand for Questions and SqlParameter for Questions.
                        var insertCommand_Questions = new SqlCommand(Constant.InsertAPIQuestions, connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        //Pass table Valued parameter to Store Procedure
                        var QuestionParam = insertCommand_Questions.Parameters.AddWithValue(Constant.QuestionParameter, dataTable);
                        QuestionParam.SqlDbType = SqlDbType.Structured;
                        var AnswerParam = insertCommand_Questions.Parameters.AddWithValue(Constant.AnswerParameter, dataTableAnswers);
                        AnswerParam.SqlDbType = SqlDbType.Structured;
                        var APIParam_Questions = insertCommand_Questions.Parameters.AddWithValue(Constant.ParamApiTypeId, ApiType);
                        APIParam_Questions.SqlDbType = SqlDbType.Int;
                        var APIParam_QuestionsTable = insertCommand_Questions.Parameters.AddWithValue(Constant.AssignedQuestionsParameter, dataTableQuestions);
                        APIParam_Questions.SqlDbType = SqlDbType.Int;

                        //PickUpLocations.
                        var insertCommand_PickUpLocations = new SqlCommand(Constant.InsertAPIPickUpLocations, connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        var PickUpParam = insertCommand_PickUpLocations.Parameters.AddWithValue(Constant.AssignedPickUpLocationParameter, dataTableAssignedPickUpLocations);
                        PickUpParam.SqlDbType = SqlDbType.Structured;
                        var APIParam_PickUp = insertCommand_PickUpLocations.Parameters.AddWithValue(Constant.ParamApiTypeId, ApiType);
                        APIParam_PickUp.SqlDbType = SqlDbType.Int;
                        var AssignedPickupParam = insertCommand_PickUpLocations.Parameters.AddWithValue(Constant.PickUpLocationParameter, dataTablePickUpLocations);
                        AssignedPickupParam.SqlDbType = SqlDbType.Structured;

                        //DropOffLocations.
                        var insertCommand_DropOffLocations = new SqlCommand(Constant.InsertAPIDropOffLocations, connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        var DropOffParam = insertCommand_DropOffLocations.Parameters.AddWithValue(Constant.DropOffLocationParameter, dataTableDropOfflocations);
                        DropOffParam.SqlDbType = SqlDbType.Structured;
                        var APIParam_DropOff = insertCommand_DropOffLocations.Parameters.AddWithValue(Constant.ParamApiTypeId, ApiType);
                        APIParam_DropOff.SqlDbType = SqlDbType.Int;
                        var AssignedDropOffParam = insertCommand_DropOffLocations.Parameters.AddWithValue(Constant.AssignedDropOffLocationParameter, dataTableAssignedDropOffLocations);
                        AssignedDropOffParam.SqlDbType = SqlDbType.Structured;

                        try
                        {
                            connection.Open();
                            // Execute the command.
                            if (dataTableQuestions != null && dataTableQuestions.Rows.Count > 0)
                            {
                                insertCommand_Questions.ExecuteNonQuery();
                            }

                            if (dataTablePickUpLocations != null && dataTablePickUpLocations.Rows.Count > 0)
                            {
                                insertCommand_PickUpLocations.ExecuteNonQuery();
                            }

                            if (dataTableDropOfflocations != null && dataTableDropOfflocations.Rows.Count > 0)
                            {
                                insertCommand_DropOffLocations.ExecuteNonQuery();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            if (connection != null)
                                connection.Close();
                        }
                    }

                    using (var connectionLive = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                    {
                        try
                        {
                            connectionLive.Open();
                            var syncAPIQuestions = new SqlCommand(Constant.SyncAPIQuestions, connectionLive)
                            {
                                CommandType = CommandType.StoredProcedure
                            };

                            var APIParam = syncAPIQuestions.Parameters.AddWithValue(Constant.ParamApiTypeId, ApiType);
                            APIParam.SqlDbType = SqlDbType.Int;

                            syncAPIQuestions.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            if (connectionLive != null)
                                connectionLive.Close();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityPersistence",
                    MethodName = "SaveQuestionsInDB",
                    Params = $"{SerializeDeSerializeHelper.Serialize(Details)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method will return iSango Service Availabilities
        /// </summary>
        /// <returns></returns>
        public List<ServiceAvailabilityFeed> GetIsangoServiceAvailabilities()
        {
            var serviceAvailabilityFeeds = new List<ServiceAvailabilityFeed>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetServiceAvailabilityFeed))
                {
                    //This proc require 1-2 min to execute. It was throwing timeoout error. to resolve the issue, CommandTimeout is set to 5 min
                    command.CommandTimeout = 300;
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var activityData = new Data.ActivityData();
                        while (reader.Read())
                        {
                            serviceAvailabilityFeeds.Add(activityData.GetServiceAvailabilityData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilityPersistence",
                    MethodName = "GetIsangoServiceAvailabilities",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return serviceAvailabilityFeeds;
        }

        #region "Private Methods"

        /// <summary>
        /// Set service details in the DataTable format
        /// </summary>
        /// <param name="serviceDetails"></param>
        /// <returns></returns>
        private DataTable SetServiceDetails(List<TempHBServiceDetail> serviceDetails)
        {
            var dataTable = new DataTable { TableName = Constant.PriceAvailabilityTableType };

            //Add column name and its data type in the data table
            foreach (var property in serviceDetails[0].GetType().GetProperties())
            {
                dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
            }

            //Add rows in the data table
            foreach (var serviceDetail in serviceDetails)
            {
                var newRow = dataTable.NewRow();
                foreach (var property in serviceDetail.GetType().GetProperties())
                {
                    newRow[property.Name] = serviceDetail.GetType().GetProperty(property.Name)?.GetValue(serviceDetail, null);
                }

                dataTable.Rows.Add(newRow);
            }

            return dataTable;
        }

        private void SaveServiceAvailabilitiesInDB(DataTable details)
        {
            /// Commented code for the CSV generation... Uncomment to generate CSV

            #region DataTable to CSV generator code

            //var name = new System.Diagnostics.StackFrame(1).GetMethod().Name;
            //var filePath = "C:\\Dumping\\" + name + "_Data.csv";
            //WriteToCsvFile(details, filePath);

            #endregion DataTable to CSV generator code

            // Configure the SqlCommand and SqlParameter.
            try
            {

                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    // Configure the SqlCommand and SqlParameter.
                    using (var insertCommand = new SqlCommand(Constant.InsertTempHBServiceDetailByPaxSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 0
                    })
                    {
                        //Pass table Valued parameter to Store Procedure
                        var tvpParam = insertCommand.Parameters.AddWithValue(Constant.AvailabilityParameter, details);
                        tvpParam.SqlDbType = SqlDbType.Structured;

                        try
                        {
                            connection.Open();
                            // Execute the command.
                            insertCommand.ExecuteNonQuery();
                        }
                        catch (System.Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            if (connection != null)
                                connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ServiceAvailabilitiesPersistence",
                    MethodName = "SaveServiceAvailabilitiesInDB"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private void SaveServiceAvailabilitiesInDBForTiqets(DataTable details)
        {
            /// Commented code for the CSV generation... Uncomment to generate CSV

            #region DataTable to CSV generator code

            //var name = new System.Diagnostics.StackFrame(1).GetMethod().Name;
            //var filePath = "C:\\Dumping\\" + name + "_Data.csv";
            //WriteToCsvFile(details, filePath);

            #endregion DataTable to CSV generator code

            // Configure the SqlCommand and SqlParameter.
            using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
            {
                // Configure the SqlCommand and SqlParameter.
                using (var insertCommand = new SqlCommand(Constant.InsertTempHBServiceDetailTiqetsByPaxSp, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                })
                {
                    //Pass table Valued parameter to Store Procedure
                    var tvpParam = insertCommand.Parameters.AddWithValue(Constant.AvailabilityParameter, details);
                    tvpParam.SqlDbType = SqlDbType.Structured;

                    try
                    {
                        connection.Open();
                        // Execute the command.
                        insertCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        if (connection != null)
                            connection.Close();
                    }
                }
            }
        }


        /// <summary>
        /// This is temporary method added to generate the CSV from the data table for testing purpose
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="filePath"></param>
	    private void WriteToCsvFile(DataTable dataTable, string filePath)
        {
            var fileContent = new StringBuilder();

            foreach (var col in dataTable.Columns)
            {
                fileContent.Append(col.ToString() + ",");
            }

            fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (var column in dr.ItemArray)
                {
                    fileContent.Append("\"" + column.ToString() + "\",");
                }
                fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            }
            System.IO.File.WriteAllText(filePath, fileContent.ToString());
        }

        private ParallelOptions GetParallelOptions()
        {
            var processorCount = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.50) * 1.0));

            if (processorCount <= 0)
            {
                processorCount = 1;
            }

            var parallelOption = new System.Threading.Tasks.ParallelOptions
            {
                MaxDegreeOfParallelism = processorCount
            };

            return parallelOption;
        }

        #endregion "Private Methods"
    }
}