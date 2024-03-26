using Isango.Entities;
using Isango.Entities.GoldenTours;
using Isango.Persistence.Contract;
using Logger.Contract;
using ServiceAdapters.TourCMS.TourCMS.Entities.ChannelListResponse;
using ServiceAdapters.TourCMS.TourCMS.Entities.ChannelResponse;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class TourCMSPersistence : PersistenceBase, ITourCMSPersistence
    {
        private readonly ILogger _log;
        public TourCMSPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Save Golden Tours product details in the database
        /// </summary>
        /// <param name="channelListResponse"></param>
        public void SaveChannelData(List<ResponseChannelData> channelListResponse)
        {
            var channelListDataTable = SetChannelDetail(channelListResponse);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertTourCMSChannelListSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(Constant.ChannelListParameter, channelListDataTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;

                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TourCMSPersistence",
                    MethodName = "SaveChannelData",
                    Params = $"{SerializeDeSerializeHelper.Serialize(channelListResponse)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
       
        #region Private Methods
        /// <summary>
        /// Prepare datatable for the Product Details
        /// </summary>
        /// <param name="responseListResponse"></param>
        /// <returns></returns>
        private DataTable SetChannelDetail(List<ResponseChannelData> responseListResponse)
        {
            var dataTable = new DataTable { TableName = Constant.TourCMSChannelList };
            try
            {
                foreach (var property in responseListResponse[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in responseListResponse)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TourCMSPersistence",
                    MethodName = "SetChannelDetail",
                    Params = $"{SerializeDeSerializeHelper.Serialize(responseListResponse)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

         /// <summary>
        /// Save Golden Tours product details in the database
        /// </summary>
        /// <param name="tourListResponse"></param>
        public void SaveTourData(List<Tour> tourListResponse)
        {
            var tourListDataTable = SetTourDetail(tourListResponse);    
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertTourCMSTourListSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(Constant.TourListParameter, tourListDataTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;

                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TourCMSPersistence",
                    MethodName = "SaveTourData",
                    Params = $"{SerializeDeSerializeHelper.Serialize(tourListResponse)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        /// <summary>
        /// Prepare datatable for the Product Details
        /// </summary>
        /// <param name="responseListResponse"></param>
        /// <returns></returns>
        private DataTable SetTourDetail(List<Tour> responseListResponse)
        {
            var dataTable = new DataTable { TableName = Constant.TourCMSTourList };
            try
            {
                foreach (var property in responseListResponse[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in responseListResponse)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TourCMSPersistence",
                    MethodName = "SetTourDetail",
                    Params = $"{SerializeDeSerializeHelper.Serialize(responseListResponse)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }


        /// <summary>
        /// Save Golden Tours product details in the database
        /// </summary>
        /// <param name="tourRateListResponse"></param>
        public void SaveTourRateData(List<TourRateResponse> tourRateListResponse)
        {
            var tourListDataTable = SetTourRateDetail(tourRateListResponse);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertTourCMSTourRateListSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(Constant.TourRateListParameter, tourListDataTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;

                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TourCMSPersistence",
                    MethodName = "SaveTourData",
                    Params = $"{SerializeDeSerializeHelper.Serialize(tourRateListResponse)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        /// <summary>
        /// Prepare datatable for the Product Details
        /// </summary>
        /// <param name="responseListResponse"></param>
        /// <returns></returns>
        private DataTable SetTourRateDetail(List<TourRateResponse> responseListResponse)
        {
            var dataTable = new DataTable { TableName = Constant.TourCMSTourRateList };
            try
            {
                foreach (var property in responseListResponse[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in responseListResponse)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TourCMSPersistence",
                    MethodName = "SetTourRateDetail",
                    Params = $"{SerializeDeSerializeHelper.Serialize(responseListResponse)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        public void InsertRedemptionData(string RedemptionJsondata)
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertTourCMSRedemptionData))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.RedemptionJsonData, DbType.String,
                        RedemptionJsondata);
                    
                    IsangoDataBaseLive.ExecuteNonQuery(dbCmd);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "InsertOrUpdateCancellationStatus",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<int> GetTourCmsChannelId()
        {
           var ChannelId = new List<int>(); // List to hold reference numbers
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetChannelId))
                {
                   

                    dbCmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        while (reader.Read())
                        {
                            var channelId = Convert.ToInt32(reader["channelId"]);
                            ChannelId.Add(channelId); // Add each reference number to the list
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TourCmSPersistance",
                    MethodName = "GetTourCmsChannelId"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return ChannelId;
        }
        #endregion Private Methods
    }
}